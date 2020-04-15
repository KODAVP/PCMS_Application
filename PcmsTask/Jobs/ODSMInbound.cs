using PcmsTask.Commons;
using PcmsTask.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcmsTask.Jobs
{
    class ODSMInbound : Jobclass
    {
        protected Channel myChannel;
        public ODSMInbound(Channel c)
        {
            myChannel = c;
        }
        //private ODSMDBContext odsmDB = new ODSMDBContext();



        public void Execute()
        {
            //ODSMDBContext odsmDB = new ODSMDBContext();

            JobLog(myChannel, BatchStatus.Begin);
            myChannel = dbContext.Channels.Find(myChannel.ID);
            // Channel 정보 획득
            // 접속
            ODSMEntities odsm = new ODSMEntities();
#pragma warning disable CS0219 // The variable 'ncnt' is assigned but its value is never used
            int ncnt = 0;
#pragma warning restore CS0219 // The variable 'ncnt' is assigned but its value is never used
            // 마지막 동작일 가져오기
            PrivacyLog logone = null;
            try
            {
                logone = dbContext.PrivacyLogs.Where(pl => pl.creater == @"ODSM").OrderByDescending(pl => pl.createdate).First();
            }
#pragma warning disable CS0168 // The variable 'e' is declared but never used
            catch (Exception e) {
#pragma warning restore CS0168 // The variable 'e' is declared but never used
                logone = null;
            }
            try
            {
                if (logone == null) {
                    logone = dbContext.PrivacyLogs.Where(pl => pl.creater == @"TASK" && (pl.changes.Contains(@"NucleusKey") || pl.changes.Contains(@"OneKey"))).OrderByDescending(pl => pl.createdate).First();
                }
            }
#pragma warning disable CS0168 // The variable 'e' is declared but never used
            catch (Exception e)
#pragma warning restore CS0168 // The variable 'e' is declared but never used
            {
                logone = null;
            }
            
            try
            {
                IEnumerable<VW_CONSENT> list = null;
                if(logone != null )
                    list = odsm.VW_CONSENT.Where(vc => vc.ODSK_MODIFY_DATE >= logone.createdate).ToList();
                else
                    list = odsm.VW_CONSENT.ToList();

                foreach (VW_CONSENT va in list)
                {
                    Privacy privacy = dbContext.Privacies.Where(p => p.pcmsid.Equals(va.PCMS_ID)).FirstOrDefault();
                    if (privacy != null && (privacy.NucleusKey != va.NUC_ID || privacy.OneKey != va.ONEKEY))
                    {
                        List<string> tracelist = new List<string>();
                        if (!IsEqual(privacy.NucleusKey, va.NUC_ID))
                        {
                            tracelist.Add("NucleusKey : " + privacy.NucleusKey + " => " + va.NUC_ID);
                            privacy.NucleusKey = va.NUC_ID;
                        }
                        if (!IsEqual(privacy.OneKey, va.ONEKEY))
                        {
                            tracelist.Add("OneKey : " + privacy.OneKey + " => " + va.ONEKEY);
                            privacy.OneKey = va.ONEKEY;
                        }
                        
                        privacy.status = Status.PrivacyStatus.GRANTED;

                        dbContext.Entry(privacy).State = System.Data.Entity.EntityState.Modified;
                        if (tracelist.Count() > 0)
                        {
                            string changes = tracelist.Aggregate((a, b) => a + ", " + b);
                            dbContext.PrivacyLogs.Add(new PrivacyLog { Privacy = privacy, creater = @"ODSM", changes = changes });
                        }
                        
                    }                    
                }
                dbContext.SaveChanges();
            }
            catch (Exception e) {
                dbContext.SaveChanges();
                JobLog(myChannel, BatchStatus.Error, e.Message);
                SMTPHelper.SendAlertInterface(myChannel.name, e.Message);

            }
            JobLog(myChannel, BatchStatus.Completed);
        }
    }
}
