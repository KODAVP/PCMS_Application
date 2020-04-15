using PcmsTask.Commons;
using PcmsTask.Models;
using System;
using Renci.SshNet;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;



/*
-- MMS InBound Dev
INSERT INTO dbo.Channels (name, bound, type,athour, usage , host, account, pwd , path, action , Instantrun, exportpath, backuppath , modifieddate) 
VALUES ('MMS', 0 , 0 ,2 , 1 , 'bluehorse.pfizer.co.kr', 'account' ,'password' ,'RemotePath' , 0, 0, '\\SDCUNS600VFS02\kr_pcms\dev\mms\import' ,'\\SDCUNS600VFS02\kr_pcms\backup\dev\mms\import' , SYSDATETIME());

 */

namespace PcmsTask.Jobs
{
    class MMSInbound : Jobclass
    {
        protected Channel myChannel;
        public MMSInbound(Channel c)
        {
            myChannel = c;
        }

        public void Execute()
        {
            JobLog(BatchStatus.Begin);
            myChannel = dbContext.Channels.Find(myChannel.ID);
            try
            {

                // 가져오기
                //added by venkat for use service account while accessing file son 5th march 2020
                using (ImpersonateUser u = new ImpersonateUser())
                {
                    using (SftpClient sftp = new SftpClient(myChannel.host, myChannel.account, myChannel.pwd))
                    {
                        try
                        {
                            sftp.Connect();
                            sftp.ChangeDirectory(myChannel.path);
                        }
#pragma warning disable CS0168 // The variable 'e' is declared but never used
                        catch (Exception e)
#pragma warning restore CS0168 // The variable 'e' is declared but never used
                        {
                            JobLog(myChannel, BatchStatus.Error, @"Cannot connect SFTP:" + myChannel.host + "[" + myChannel.account + "/" + myChannel.pwd + "]");
                            JobLog(BatchStatus.Completed);
                            SMTPHelper.SendAlertInterface(myChannel.name, @"Cannot connect SFTP:" + myChannel.host + "[" + myChannel.account + "/" + myChannel.pwd + "]");
                            return;
                        }


                        var filter = new Regex(@"^mms_\d{8}_pcms.csv$");
                        var files = sftp.ListDirectory(myChannel.path).Where(f => filter.IsMatch(f.Name)).ToList();
                        foreach (var file in files)
                        {
                            if (CheckFile(myChannel, file.FullName))
                            {
                                // 이미 존재 패스
                            }
                            else
                            {
                                // 찾았음 표시
                                CollectionLog(myChannel, file.Name, file.FullName, CollectionStatus.FIND);
                            }
                        }
                        // 다운로드
                        var downloadlist = dbContext.Collections.Where(c => c.status == CollectionStatus.FIND && c.channelId == myChannel.ID);
                        try
                        {
                            foreach (var clt in downloadlist)
                            {

                                Stream fileStream = File.OpenWrite(Path.Combine(myChannel.exportpath, clt.name));
                                sftp.DownloadFile(clt.ftpname, fileStream);
                                CollectionLog(clt, CollectionStatus.DOWNLOAD);
                                fileStream.Flush();
                                fileStream.Close();

                            }
                        }
                        catch (Exception e)
                        {   
                            //added by venkat for exception habding on 5th march 2020
                            JobLog(myChannel, BatchStatus.Error, e.Message);
                            JobLog(BatchStatus.Completed);
                            SMTPHelper.SendAlertInterface(myChannel.name,e.Message);
                            return;
                        }

                        sftp.Disconnect();
                    }

                    // Parsing
                    List<Collection> collectionlist = dbContext.Collections.Where(c => c.channelId == myChannel.ID && c.status == CollectionStatus.DOWNLOAD).ToList();
                    foreach (Collection collection in collectionlist)
                    {
                        string line;
                        StreamReader file;
                        try
                        {
                            file = new StreamReader(Path.Combine(myChannel.exportpath, collection.name));
                        }
                        catch (Exception e)
                        {
                            continue;
                        }
                        line = file.ReadLine();
                        try
                        {
                            List<MMSFile> mms = new List<MMSFile>();

                            while ((line = file.ReadLine()) != null)
                            {
                                string[] arr = line.Split('|');
                                mms.Add(new MMSFile(collection, arr));
                            }
                            file.Close();

                            // privacy 추가
                            foreach (MMSFile m in mms)
                            {
                                Privacy p = new Privacy(
                                    m.COMPANY     // WKP_NAME
                                    , m.TEL    // WKP_TEL
                                    , m.ZIP  // ZIP
                                    , string.Empty   // PROVINCE
                                    , string.Empty   // CITY
                                    , m.ADDRESS1   // DONG
                                    , m.ADDRESS2   // STREET
                                    , m.ADDRESS1 + " " + m.ADDRESS2   // FUKKADDR
                                    , m.CATEGORY // IND_SP
                                    , string.Empty    // TITLE
                                    , m.NAME   // FULL_NAME
                                    , m.EMAIL    // EMAIL
                                    , m.HP   // MOBILE
                                    , "MMS"    // CONSENT_SOURCE
                                    , m.SUB_CHANNEL // CONSENT_SUB_SOURCE
                                    , string.Empty, string.Empty            // LINK Re / LINK PHONE
                                    , CommonUtil.toUtcDT(m.AGR_DATE)
                                    , m.AGR_VER                     // CONSENT_VERSION
                                    , chkOptIn2(m.AGREE1)
                                    , chkOptIn2(m.AGREE2)
                                    , chkOptIn2(m.AGREE3)
                                    , true
                                    , !chkOptIn2(m.RCV_MAIL)
                                    , true
                                );
                                p.Channel = myChannel;
                                p.CONSENT_SOURCE = @"MMS";

                                bool bExist = false;
                                List<Privacy> tmp = dbContext.Privacies.Where(pp => pp.EMAIL == m.EMAIL && pp.CONSENT_SOURCE.Equals(@"MMS")).ToList();
                                foreach (Privacy pitem in tmp)
                                {
                                    bExist = true;
                                    Privacy priv = pitem;
                                    List<string> tracelist = new List<string>();

                                    if (!IsEqual(m.COMPANY, priv.WKP_NAME))
                                    {
                                        tracelist.Add("근무처(병원명) : " + priv.WKP_NAME + " => " + m.COMPANY);
                                        priv.WKP_NAME = m.COMPANY;
                                    }
                                    if (!IsEqual(m.TEL, priv.WKP_TEL))
                                    {
                                        tracelist.Add("근무지연락처 : " + priv.WKP_TEL + " => " + m.TEL);
                                        priv.WKP_TEL = m.TEL;
                                    }
                                    if (!IsEqual(m.ZIP, priv.ZIP))
                                    {
                                        tracelist.Add("우편번호 : " + priv.ZIP + " => " + m.ZIP);
                                        priv.ZIP = m.ZIP;
                                    }
                                    if (!IsEqual(m.ADDRESS1, priv.DONG))
                                    {
                                        tracelist.Add("주소(군/구/동) : " + priv.DONG + " => " + m.ADDRESS1);
                                        priv.DONG = m.ADDRESS1;
                                    }
                                    if (!IsEqual(m.ADDRESS2, priv.STREET))
                                    {
                                        tracelist.Add("상세주소 : " + priv.STREET + " => " + m.ADDRESS2);
                                        priv.STREET = m.ADDRESS2;
                                    }
                                    string tmpaddr = m.ADDRESS1 + " " + m.ADDRESS2;
                                    if (!IsEqual(tmpaddr, priv.FULL_ADDR))
                                    {
                                        tracelist.Add("주소 : " + priv.FULL_ADDR + " => " + tmpaddr);
                                        priv.FULL_ADDR = tmpaddr;
                                    }
                                    if (!IsEqual(m.CATEGORY, priv.IND_SP))
                                    {
                                        tracelist.Add("진료과 : " + priv.IND_SP + " => " + m.CATEGORY);
                                        priv.IND_SP = m.CATEGORY;
                                    }
                                    if (!IsEqual(m.NAME, priv.IND_FULL_NAME))
                                    {
                                        tracelist.Add("고객명 : " + priv.IND_FULL_NAME + " => " + m.NAME);
                                        priv.IND_FULL_NAME = m.NAME;
                                    }
                                    if (!IsEqual(m.HP, priv.MOBILE))
                                    {
                                        tracelist.Add("핸드폰 : " + priv.MOBILE + " => " + m.HP);
                                        priv.MOBILE = m.HP;
                                    }
                                    if (!IsEqual(m.SUB_CHANNEL, priv.CONSENT_SUB_SOURCE))
                                    {
                                        tracelist.Add("채널(부) : " + priv.CONSENT_SUB_SOURCE + " => " + m.SUB_CHANNEL);
                                        priv.CONSENT_SUB_SOURCE = m.SUB_CHANNEL;
                                    }
                                    if (!chkOptIn2(m.RCV_MAIL) != priv.Unsubscribe)
                                    {
                                        tracelist.Add("수신거부 : " + priv.Unsubscribe + " => " + !chkOptIn2(m.RCV_MAIL));
                                        priv.Unsubscribe = !chkOptIn2(m.RCV_MAIL);
                                    }
                                    // 동의서 변경시
                                    if (priv.CONSENT == null)
                                    {
                                        priv.Consents.Add(new Consent
                                        {
                                            CONSENT_DATE = CommonUtil.toUtcDT(m.AGR_DATE),
                                            CONSENT_VERSION = m.AGR_VER,
                                            CONSENT_USE = chkOptIn2(m.AGREE1),
                                            CONSENT_TRUST = chkOptIn2(m.AGREE2),
                                            CONSENT_ABROAD = chkOptIn2(m.AGREE3),
                                            CONSENT_SIGN = true,
                                            CONSENT_SOURCE = "MMS"
                                        });
                                    }
                                    else if (priv.CONSENT != null && CommonUtil.toUtcDT(m.AGR_DATE) != priv.CONSENTDATE)
                                    {
                                        priv.Consents.Add(new Consent
                                        {
                                            CONSENT_DATE = CommonUtil.toUtcDT(m.AGR_DATE),
                                            CONSENT_VERSION = m.AGR_VER,
                                            CONSENT_USE = chkOptIn2(m.AGREE1),
                                            CONSENT_TRUST = chkOptIn2(m.AGREE2),
                                            CONSENT_ABROAD = chkOptIn2(m.AGREE3),
                                            CONSENT_SIGN = true,
                                            CONSENT_SOURCE = "MMS"
                                        });
                                    }
                                    // 변경사항 저장
                                    if (tracelist.Count() > 0)
                                    {
                                        string changes = tracelist.Aggregate((a, b) => a + ", " + b);
                                        dbContext.PrivacyLogs.Add(new PrivacyLog { Privacy = priv, creater = @"MMS", changes = changes });
                                    }

                                    dbContext.Entry(priv).State = System.Data.Entity.EntityState.Modified;
                                    dbContext.SaveChanges();
                                } // for 존재하면 업데이트
                                if (bExist == false)
                                {
                                    // mms 에서 온 데이터가 없는 경우 새로 추가
                                    dbContext.Privacies.Add(p);
                                    dbContext.SaveChanges();

                                    List<string> tracelist = new List<string>();
                                    tracelist.Add("근무처(병원명) :  => " + p.WKP_NAME);
                                    tracelist.Add("근무지연락처 :  => " + p.WKP_TEL);
                                    tracelist.Add("우편번호 :  => " + p.ZIP);
                                    tracelist.Add("주소 :  => " + p.FULL_ADDR);
                                    tracelist.Add("진료과 :  => " + p.IND_SP);
                                    tracelist.Add("고객명 :  => " + p.IND_FULL_NAME);
                                    tracelist.Add("핸드폰 :  => " + p.MOBILE);
                                    tracelist.Add("채널(부) :  => " + p.CONSENT_SUB_SOURCE);
                                    tracelist.Add("채널 :  => " + p.CONSENT_SOURCE);
                                    tracelist.Add("수신거부 :  => " + p.Unsubscribe);
                                    string changes = tracelist.Aggregate((a, b) => a + ", " + b);

                                    dbContext.PrivacyLogs.Add(new PrivacyLog { Privacy = p, creater = @"MMS", changes = changes });
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            JobLog(myChannel, BatchStatus.Error, e.Message);
                            SMTPHelper.SendAlertInterface(myChannel.name, e.Message);

                        }
                        finally
                        {
                            if (file != null) file.Dispose();
                        }
                        // 완료처리
                        collection.status = CollectionStatus.COMPLETED;
                        dbContext.Entry(collection).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        // 백업처리
                        try
                        {
                            File.Move(Path.Combine(myChannel.exportpath, collection.name), Path.Combine(myChannel.backuppath, collection.name));
                        }
#pragma warning disable CS0168 // The variable 'e' is declared but never used
                        catch (Exception e)
                        {
#pragma warning restore CS0168 // The variable 'e' is declared but never used
                            File.Move(Path.Combine(myChannel.exportpath, collection.name), Path.Combine(myChannel.backuppath, Guid.NewGuid().ToString()));
                        }

                    }
                }
            }
            catch(Exception e)
            {
                //added by venkat for exception habding on 5th march 2020
                JobLog(myChannel, BatchStatus.Error, e.Message);
                JobLog(BatchStatus.Completed);
                SMTPHelper.SendAlertInterface(myChannel.name, e.Message);

            }
            JobLog(BatchStatus.Completed);
        }

        private bool chkOptIn2(string val)
        {
            if (string.IsNullOrEmpty(val)) return false;
            if (val == @"Y") return true;
            return false;
        }

        protected void JobLog(BatchStatus batchStatus)
        {
            JobLog(myChannel, batchStatus);
        }
    }
}
