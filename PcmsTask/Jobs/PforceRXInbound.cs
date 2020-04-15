using PcmsTask.Commons;
using PcmsTask.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using static PcmsTask.Commons.Status;


namespace PcmsTask.Jobs
{
    class PforceRXInbound : Jobclass
    {
        protected Channel myChannel;
        public PforceRXInbound(Channel c)
        {
            myChannel = c;
        }
        public void Execute()
        {
           

            bool bTrjStarted = false;
            JobLog(BatchStatus.Begin);
            myChannel = dbContext.Channels.Find(myChannel.ID);
            log.Info(MethodBase.GetCurrentMethod().DeclaringType.Name);
            try
            {
                using (ImpersonateUser u = new ImpersonateUser())
                {
                    // 가져오기
                    var filter = new Regex(@"^KR_CONSENT_OUT_\d{8}.txt$");
                    if (Directory.Exists(myChannel.path))
                    {
                        DirectoryInfo di = new DirectoryInfo(myChannel.path);
                        var files = di.GetFiles().Where(f => filter.IsMatch(f.Name)).ToList();
                        foreach (var item in files)
                        {
                            //if (!CheckFile(myChannel, item.Name)) comment by Venkat for apaccr-287
                                if (!CheckFile(myChannel, item.FullName))
                                {
                                    // 찾았음 표시
                                    CollectionLog(myChannel, item.Name, item.FullName, CollectionStatus.DOWNLOAD);
                            }
                        }
                    }

                    ODSMEntities ODSM = new ODSMEntities();

                    // Parsing 
                    List<Collection> paringlist = dbContext.Collections.Where(c => c.status == CollectionStatus.DOWNLOAD && c.channelId == myChannel.ID).ToList();
                    foreach (Collection pfile in paringlist)
                    {
                        // 파일이 없는 경우
                        if (!File.Exists(Path.Combine(myChannel.path, pfile.name)))
                        {
                            Collection one = dbContext.Collections.Find(pfile.ID);
                            one.status = CollectionStatus.ERROR;
                            dbContext.SaveChanges();
                            continue;
                        }
                        // 실제 파일 입력
                        using (DbContextTransaction dbTran = upsertContext.Database.BeginTransaction())
                        {
                            bTrjStarted = true;
                            try
                            {
                                // 임포트
                                string line;
                                PforceRXFile f = null;
                                StreamReader file = new StreamReader(Path.Combine(myChannel.path, pfile.name));
                                Collection one = upsertContext.Collections.Find(pfile.ID);
                                one.status = CollectionStatus.COMPLETED;
                                line = file.ReadLine();
                                //added by venkat 
                                /*Changes done as part of APACCR-287
                                 * Assuming will get Opt Out records first and later Opt In records
                                 * Inserting "Opt Out" record in database if not avaiable and skipiing remaining "Opt In" record in that file with respective to nucleus id and (email/phone)
                                 * done bug fix while loging in collection log changed to pass "full name" of file alonng with ftp details.
                                 * */
                                List<Privacy> inserteddata = new List<Privacy>();
                                // end of comment 
                                while ((line = file.ReadLine()) != null)
                                {
                                    string[] arr = line.Split('|');
                                    f = new PforceRXFile(one, arr);

                                   
                                    List<Privacy> templist = upsertContext.Privacies.Where(pt => pt.NucleusKey == f.IND_ID && ((pt.EMAIL != null && pt.EMAIL == f.CONSENT_EMAIL) || (pt.MOBILE != null && pt.MOBILE.Replace("-", "") == f.CONSENT_MOBILE.Replace("-", "")))).ToList();

                                    if (inserteddata.Where(insert => insert.NucleusKey == f.IND_ID && ((insert.EMAIL != null && insert.EMAIL == f.CONSENT_EMAIL) || (insert.MOBILE != null && insert.MOBILE.Replace("-", "") == f.CONSENT_MOBILE.Replace("-", "")))).ToList().Count() == 0)
                                    {
                                        // 원키나 PCMSID가 아니라 N키여서 여러 항목이 동시에 삭제될 가능성이 있음.
                                        if (templist.Count() > 0 && !string.IsNullOrEmpty(f.IND_ID) && f.CONSENT_STATUS == false)
                                        {
                                            //comment below line as part of apaccr-287 by venkat on 3/9/2020
                                            /*List<Privacy> templist = upsertContext.Privacies.Where(pt => pt.NucleusKey == f.IND_ID).ToList(); */

                                            foreach (Privacy p in templist)
                                            {

                                                /* comment as part of apaccr-287 by venkat on 3/9/2020
                                                p.IND_FIRSTNAME = "*****";
                                                p.IND_LASTNAME = "*****";
                                                p.IND_FULL_NAME = "*****";
                                                p.MOBILE = "*****";
                                                p.EMAIL = "*****";
                                                p.CONSENT.CONSENT_ABROAD = false;
                                                p.CONSENT.CONSENT_SIGN = false;
                                                p.CONSENT.CONSENT_TRUST = false;
                                                p.CONSENT.CONSENT_USE = false; */
                                                p.Unsubscribe = true;
                                                /* comment as part of apaccr-287 by venkat 
                                                p.status = PrivacyStatus.ERASED; 
                                                p.SENDCHANEL &= ~SendChannel.N360; */

                                                upsertContext.Entry(p).State = System.Data.Entity.EntityState.Modified;
                                                upsertContext.PrivacyLogs.Add(new PrivacyLog { Privacy = p, creater = @"PFORCERX", changes = @"OptOut NucleusKey :  => " + p.NucleusKey });
                                                upsertContext.SaveChanges();
                                            }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                Privacy p = new Privacy();
                                                p.pcmsid = IdGenerater.Generater();
                                                p.IND_ID = f.IND_ID;
                                                p.NucleusKey = f.IND_ID;

                                                if (f.CONSENT_EMAIL != null && f.CONSENT_EMAIL.Length > 0) p.EMAIL = f.CONSENT_EMAIL;
                                                if (f.CONSENT_MOBILE != null && f.CONSENT_MOBILE.Length > 0) p.MOBILE = f.CONSENT_MOBILE;
                                                p.CONSENT_SOURCE = f.CONSENT_SOURCE;
                                                p.COUNTRY_CD = f.COUNTRY_CD;
                                                p.Unsubscribe = !f.CONSENT_STATUS;
                                                p.Consents.Add(new Consent
                                                {
                                                    CONSENT_USE = f.CONSENT_STATUS,
                                                    CONSENT_TRUST = f.CONSENT_STATUS,
                                                    CONSENT_ABROAD = f.CONSENT_STATUS,
                                                    CONSENT_SIGN = f.CONSENT_STATUS,
                                                    CONSENT_DATE = f.CONSENT_DATE,
                                                    CONSENT_SOURCE = f.CONSENT_SOURCE
                                                });

                                                p.channelId = myChannel.ID;
                                                p.status = PrivacyStatus.IMPORTED;

                                                VW_CONSENT_ALIGNMENT vca = ODSM.VW_CONSENT_ALIGNMENT.Where(x => x.D_NUC_ID.Equals(f.IND_ID)).FirstOrDefault();
                                                if (vca != null)
                                                {
                                                    p.IND_FULL_NAME = vca.D_NAME;
                                                    p.ZIP = vca.H_POSTCODE;
                                                    p.WKP_NAME = vca.H_NAME;
                                                    p.WKP_TEL = vca.H_PHONE;
                                                    p.FULL_ADDR = vca.H_ADDR;
                                                    p.IND_SP = vca.D_SPEC;
                                                    p.OneKey = vca.D_ONEKEY_ID;
                                                }

                                                upsertContext.Privacies.Add(p);
                                                upsertContext.SaveChanges();

                                                //added by Venkat for apaccr-287 
                                                inserteddata.Add(p);
                                                // end of comment

                                                List<string> tracelist = new List<string>();
                                                tracelist.Add("NucleusKey :  => " + p.NucleusKey);
                                                tracelist.Add("OneKey :  => " + p.OneKey);
                                                tracelist.Add("EMAIL :  => " + p.EMAIL);
                                                tracelist.Add("MOBILE :  => " + p.MOBILE);
                                                tracelist.Add("CONSENT_SOURCE :  => " + p.CONSENT_SOURCE);
                                                tracelist.Add("COUNTRY_CD :  => " + p.COUNTRY_CD);
                                                tracelist.Add("IND_FULL_NAME :  => " + p.IND_FULL_NAME);
                                                tracelist.Add("ZIP :  => " + p.ZIP);
                                                tracelist.Add("WKP_NAME :  => " + p.WKP_NAME);
                                                tracelist.Add("WKP_TEL :  => " + p.WKP_TEL);
                                                tracelist.Add("FULL_ADDR :  => " + p.FULL_ADDR);
                                                tracelist.Add("IND_SP :  => " + p.IND_SP);
                                                string changes = tracelist.Aggregate((a, b) => a + ", " + b);
                                                upsertContext.PrivacyLogs.Add(new PrivacyLog { Privacy = p, creater = @"PFORCERX", changes = changes });
                                                upsertContext.SaveChanges();

                                            }
                                            catch (Exception e)
                                            {
                                                JobLog(myChannel, BatchStatus.Error, e.Message);
                                                SMTPHelper.SendAlertInterface(myChannel.name, e.Message);

                                            }
                                        }
                                    }
                                }
                                dbTran.Commit();
                                bTrjStarted = false;
                                file.Close();
                                // Backup으로 이동
                                File.Move(Path.Combine(myChannel.path, pfile.name), Path.Combine(myChannel.backuppath, pfile.name));
                            }
                            catch (Exception e)
                            {
                                log.Error(e);
                                if (bTrjStarted) dbTran.Rollback();
                                JobLog(myChannel, BatchStatus.Error, e.Message);
                                SMTPHelper.SendAlertInterface(myChannel.name, e.Message);

                            }
                            finally
                            {
                                dbTran.Dispose();
                            }
                        }
                    }
                }
                ChannelStatus(myChannel, ActionStatus.Waiting);
                JobLog(BatchStatus.Completed);
            }
            catch (Exception e)
            {   
                //added by venkat for exception habding on 5th march 2020
                JobLog(myChannel, BatchStatus.Error, e.Message);
                JobLog(BatchStatus.Completed);
                SMTPHelper.SendAlertInterface(myChannel.name, e.Message);
            }
        }

        protected void JobLog(BatchStatus batchStatus)
        {
            JobLog(myChannel, batchStatus);
        }
    }
}
