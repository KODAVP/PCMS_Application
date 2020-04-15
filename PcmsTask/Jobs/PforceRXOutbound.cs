using PcmsTask.Commons;
using PcmsTask.Models;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static PcmsTask.Commons.Status;

/*
    -- PforceRX OutBound Dev
    INSERT INTO dbo.Channels (name, bound, type,athour, usage , host, account, pwd , path, action , Instantrun, exportpath, backuppath , modifieddate) 
    VALUES ('PforceRX', 1 , 3 ,3 , 1 , '', '' ,'' ,'\\SDCUNS600VFS02\kr_pcms\dev\pforcerx\export' , 0, 0, '' ,'\\SDCUNS600VFS02\kr_pcms\backup\dev\pforcerx\export' , SYSDATETIME());
*/
namespace PcmsTask.Jobs
{
    class PforceRXOutbound : Jobclass
    {
        private const String phonepattern = @"[0-9]{2,3}[0-9]{3,4}[0-9]{4}";
        private const String emailpattern = @"^([0-9a-zA-Z]" + //Start with a digit or alphabetical
                                        @"([\+\-_\.][0-9a-zA-Z]+)*" + // No continuous or ending +-_. chars in email
                                        @")+" +
                                        @"@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,17})$";
        protected Channel myChannel;
        public PforceRXOutbound(Channel c)
        {
            myChannel = c;
        }

        private List<PforceRXFile> initTargetData()
        {
            List<PforceRXFile> flist = new List<PforceRXFile>();

            Setting ct = dbContext.Settings.Where(s => s.type == SettingType.ConsentTerm).First();

            IEnumerable<Privacy> list =
                dbContext.Privacies.Where(p => p.OneKey != null && p.OneKey.Length > 0
                        && ( p.status == PrivacyStatus.GRANTED || p.status== PrivacyStatus.ERASED || p.status == PrivacyStatus.DELETED) //added erased/deleted condition by venkat for apaccr-287 on 3/9/2020
                        && !p.SENDCHANEL.HasFlag(SendChannel.PFORCERX)
                        && !p.SENDCHANEL.HasFlag(SendChannel.PFORCERX_INVALID)
                        && !p.SENDCHANEL.HasFlag(SendChannel.PFORCERX_ONEKEY_DUP)
                        && (p.Approval == null || p.Approval.status == 2)
                    ).OrderByDescending(p => p.modifieddate).ToList();

            List<String> lstOneKey = new List<string>();
            foreach (Privacy p in list)
            {

                // Mandatory Check
                //p.EMAIL = Regex.Replace(p.EMAIL, @" ", "");Commented by Nagaraju as part of IND30331337i and added below if condition fro If.
                if (!string.IsNullOrEmpty(p.EMAIL))
                {
                    p.EMAIL = Regex.Replace(p.EMAIL, @" ", "");
                }

                if (string.IsNullOrEmpty(p.MOBILE) && string.IsNullOrEmpty(p.EMAIL))
                {
                    p.SENDCHANEL |= SendChannel.PFORCERX_INVALID;
                    dbContext.Entry(p).State = System.Data.Entity.EntityState.Modified;
                    continue;
                }
                else if (!string.IsNullOrEmpty(p.MOBILE) && string.IsNullOrEmpty(p.EMAIL))
                {
                    string temp = Regex.Replace(p.MOBILE, @"[^0-9a-zA-Z가-힣]", "");
                    if (Regex.IsMatch(temp, phonepattern) == false)
                    {
                        p.SENDCHANEL |= SendChannel.PFORCERX_INVALID;
                        dbContext.Entry(p).State = System.Data.Entity.EntityState.Modified;
                        continue;
                    }
                }
                else if (!string.IsNullOrEmpty(p.EMAIL) && string.IsNullOrEmpty(p.MOBILE))
                {
                    p.EMAIL = Regex.Replace(p.EMAIL, @" ", "");
                    if (p.EMAIL.Equals(@"*****") == false)
                    {
                        if (Regex.IsMatch(p.EMAIL, emailpattern) == false)
                        {
                            p.SENDCHANEL |= SendChannel.PFORCERX_INVALID;
                            dbContext.Entry(p).State = System.Data.Entity.EntityState.Modified;
                            continue;
                        }
                    }
                }
                else
                {
                    p.EMAIL = Regex.Replace(p.EMAIL, @" ", "");
                    string temp = Regex.Replace(p.MOBILE, @"[^0-9a-zA-Z가-힣]", "");
                    if (Regex.IsMatch(p.EMAIL, emailpattern) == false || Regex.IsMatch(temp, phonepattern) == false)
                    {
                        p.SENDCHANEL |= SendChannel.PFORCERX_INVALID;
                        dbContext.Entry(p).State = System.Data.Entity.EntityState.Modified;
                        continue;
                    }
                }
                // CONSENT_SOURCE
                if (string.IsNullOrEmpty(p.CONSENT_SOURCE))
                {
                    p.SENDCHANEL |= SendChannel.PFORCERX_INVALID;
                    dbContext.Entry(p).State = System.Data.Entity.EntityState.Modified;
                    continue;
                }

                // Onekey Dup avoid
                bool bMatch = false;
                foreach (String s in lstOneKey)
                {
                    if (s != null && s.Equals(p.OneKey))
                    {
                        bMatch = true;
                        break;
                    }
                }
                if (bMatch)
                {
                    p.SENDCHANEL |= SendChannel.PFORCERX_ONEKEY_DUP;
                    p.SENDCHANEL |= SendChannel.PFORCERX;
                    dbContext.Entry(p).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    lstOneKey.Add(p.OneKey);

                    p.SENDCHANEL |= SendChannel.PFORCERX;
                    dbContext.Entry(p).State = System.Data.Entity.EntityState.Modified;
                    PforceRXFile pr = new PforceRXFile(p);
                    pr.EXPIRATION_DATE = p.CONSENTDATE.AddYears(int.Parse(ct.value));
                    flist.Add(pr);
                }
            }
            dbContext.SaveChanges();
            return flist;
        }
        private string exportData(List<PforceRXFile> datalist, string exportPath, string backuppath)
        {
            string uploadfilename = "KR_CONSENT_IN_" + DateTime.UtcNow.ToString("yyyyMMdd") + ".txt";
            string fullpath = Path.Combine(exportPath, uploadfilename);

            using (ImpersonateUser u = new ImpersonateUser())
            {
                DirectoryInfo di = new DirectoryInfo(exportPath);
                if (di.Exists == false)
                {
                    di.Create();
                }
                StreamWriter fileStream = new StreamWriter(new FileStream(fullpath, FileMode.OpenOrCreate, FileAccess.ReadWrite), Encoding.UTF8);
                try
                {
                    fileStream.WriteLine(PforceRXFile.exportHeader());
                    foreach (PforceRXFile n in datalist)
                    {
                        fileStream.WriteLine(n.Export());
                    }

                }
                catch (Exception e)
                {
                    JobLog(myChannel, BatchStatus.Error, e.Message);
                    SMTPHelper.SendAlertInterface(myChannel.name, e.Message);

                    return null;
                }
                finally
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
                // 백업
                try
                {
                    File.Copy(fullpath, Path.Combine(backuppath, uploadfilename), true);
                }
                catch (Exception e)
                {
                    JobLog(myChannel, BatchStatus.Error, e.Message);
                    SMTPHelper.SendAlertInterface(myChannel.name, e.Message);

                    return null;
                }
            }
            return fullpath;
        }
        public void Execute()
        {
            JobLog(BatchStatus.Begin);
            Channel channel = dbContext.Channels.Find(myChannel.ID);

            // 미처리 채널 데이터를 파일로 export
            List<PforceRXFile> flist = initTargetData();
            //if (flist.Count() > 0)
            {
                // 파일생성        
                string uploadfilename = exportData(flist, channel.path, channel.backuppath);

                CollectionLog(channel, uploadfilename, channel.exportpath, CollectionStatus.UPLOAD);
            }
            ChannelStatus(channel, ActionStatus.Waiting);
            JobLog(BatchStatus.Completed);
        }


        protected void JobLog(BatchStatus batchStatus)
        {
            JobLog(myChannel, batchStatus);
        }
    }
}
