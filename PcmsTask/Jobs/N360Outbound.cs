using PcmsTask.Commons;
using PcmsTask.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static PcmsTask.Commons.Status;

namespace PcmsTask.Jobs
{
    public class N360Outbound : Jobclass
    {
        protected Channel myChannel;
        public N360Outbound(Channel c)
        {
            myChannel = c;
        }

        private IQueryable<Privacy> getPrivacies() {
            //return dbContext.Privacies.Where(p => !p.SENDCHANEL.HasFlag(SendChannel.N360) && !p.SENDCHANEL.HasFlag(SendChannel.PFORCERX_INVALID) && (p.Approval == null || p.Approval.status == 2) ).OrderBy(p => p.ID);
            return dbContext.Privacies.Where(p => !p.SENDCHANEL.HasFlag(SendChannel.N360) && (p.Approval == null || p.Approval.status == 2)).OrderBy(p => p.ID);
        }
        private int countTargetData() {
            int rowcnt = getPrivacies().Count();
            return rowcnt;
        }
        private List<N360File> initTargetData() {
            List<N360File> flist = new List<N360File>();
            IEnumerable<Privacy> list = getPrivacies().ToList();
            Setting setting = dbContext.Settings.Where(s => s.type == SettingType.ConsentTerm).First();
            int periodYear = Int32.Parse(setting.value);

            foreach (Privacy p in list)
            {
                // 국외이전 동의가 필요.
                if (p.CONSENT_ABROAD || p.status == PrivacyStatus.DELETED || p.status == PrivacyStatus.ERASED) {
                    p.SENDCHANEL |= SendChannel.N360;
                    dbContext.Entry(p).State = System.Data.Entity.EntityState.Modified;
                    flist.Add(new N360File(p, periodYear));
                }                
            }
            dbContext.SaveChanges();
            return flist;
        }
        private string exportHeader(string exportPath)
        {
            string uploadfilename = "PFIZER_NUC_PCMS_" + DateTime.UtcNow.ToString("yyyyMMddTHHmmss") + ".txt";
            string fullpath = Path.Combine(exportPath, uploadfilename);
            using (ImpersonateUser u = new ImpersonateUser())
            {
                DirectoryInfo di = new DirectoryInfo(exportPath);
                if (di.Exists == false)
                {
                    di.Create();
                }
                StreamWriter fileStream = new StreamWriter(fullpath, true);
                try
                {
                    fileStream.WriteLine(N360File.exportHeader());
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
            }
            return fullpath;
        }
        private void exportData(List<N360File> datalist, string exportPath, string appendfile ) {
            string uploadfilename = "PFIZER_NUC_PCMS_" + DateTime.UtcNow.ToString("yyyyMMddTHHmmss") + ".txt";
            string fullpath = Path.Combine(exportPath, uploadfilename);
            if (!string.IsNullOrEmpty(appendfile)) fullpath = appendfile;

            using (ImpersonateUser u = new ImpersonateUser())
            {
                DirectoryInfo di = new DirectoryInfo(exportPath);
                if (di.Exists == false)
                {
                    di.Create();
                }
                StreamWriter fileStream = new StreamWriter(fullpath,true);
                try
                {
                    foreach (N360File n in datalist)
                    {
                        fileStream.WriteLine(n.Export());
                    }
                }
                catch (Exception e)
                {
                    JobLog(myChannel, BatchStatus.Error , e.Message);
                    SMTPHelper.SendAlertInterface(myChannel.name, e.Message);

                }
                finally
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
        }
        public void Execute()
        {
            JobLog(BatchStatus.Begin);
            myChannel = dbContext.Channels.Find(myChannel.ID);
            //CommonUtil.LogWrite("BatchStatus.Begin");
            Channel channel = dbContext.Channels.Where(c => c.name == @"N360" && c.bound == BoundType.Outbound).FirstOrDefault();
            //ChannelStatus(channel, ActionStatus.Running);
            //CommonUtil.LogWrite("ActionStatus.Running");
            // 미처리 채널 데이터를 파일로 export
            string uploadfilename = exportHeader(channel.path);
            //CommonUtil.LogWrite("exportHeader");
            try
            {                
                if(countTargetData() > 0)
                {
                    //CommonUtil.LogWrite("initTargetData Before");
                    List<N360File> flist = initTargetData();
                    // 파일생성        
                    //CommonUtil.LogWrite("initTargetData After");
                    exportData(flist, channel.path, uploadfilename);
                    //CommonUtil.LogWrite("exportData ");
                }
                // 업로드
                if (!string.IsNullOrEmpty(channel.exportpath))
                {
                    XferHelper.Call(XferHelper.XferActionEnum.Export, channel.path, channel.exportpath);
                }
                else
                {
                    XferHelper.Call(XferHelper.XferActionEnum.Export, @"xfer_n360_export.txt");
                }
            }
            catch (Exception e)
            {
                JobLog(myChannel, BatchStatus.Error, e.Message);
                SMTPHelper.SendAlertInterface(myChannel.name, e.Message);
            }
            // 백업                
            CollectionLog(channel, uploadfilename, channel.exportpath, CollectionStatus.UPLOAD);
            //added by venkat for use service accoutn while accessing file son 5th march 2020
            using (ImpersonateUser u = new ImpersonateUser())
            {
                File.Move(uploadfilename, Path.Combine(channel.backuppath, Path.GetFileName(uploadfilename)));
            }
            JobLog(BatchStatus.Completed);
        }


        protected void JobLog(BatchStatus batchStatus)
        {
            JobLog(myChannel, batchStatus);
        }
    }
}