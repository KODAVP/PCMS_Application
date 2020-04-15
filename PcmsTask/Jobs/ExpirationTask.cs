using PcmsTask.Commons;
using PcmsTask.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PcmsTask.Commons.Status;

namespace PcmsTask.Jobs
{
    class ExpirationTask : Jobclass
    {
#pragma warning disable CS0414 // The field 'ExpirationTask.TaskName' is assigned but its value is never used
        private static string TaskName = @"Expiration";
#pragma warning restore CS0414 // The field 'ExpirationTask.TaskName' is assigned but its value is never used

        private IQueryable<Privacy> getExpirePrivacies(DateTime expiredDate)
        {   
            return dbContext.Consents.Where(cst => cst.CONSENT_DATE < expiredDate).Select(cst => cst.Privacy).Distinct().Where(p => p.SENDCHANEL.HasFlag(SendChannel.N360));
        }
        
        public void Execute()
        {
            MyLog(@"ExpirationTask", BoundType.Inbound, BatchStatus.Begin, @"");
            Setting setting = dbContext.Settings.Where(s => s.type == SettingType.ConsentTerm).First();
            DateTime expiredDate = DateTime.UtcNow.AddYears(-1 * Int32.Parse(setting.value));
            // 개인정보 삭제
            IEnumerable<Privacy> list = getExpirePrivacies(expiredDate).ToList();
            int commitcount = 0;
            foreach (Privacy p in list) {
                // 다시 비교하는 이유는 마지막 동의날짜와 비교하기 위해서.
                if (p.CONSENTDATE < expiredDate) {
                    // 개인정보 삭제
                    p.IND_FIRSTNAME = "*****";
                    p.IND_LASTNAME = "*****";
                    p.IND_FULL_NAME = "*****";
                    p.MOBILE = "*****";
                    p.EMAIL = "*****";
                    p.status = PrivacyStatus.ERASED;
                    p.SENDCHANEL &= ~SendChannel.N360;

                    dbContext.Entry(p).State = System.Data.Entity.EntityState.Modified;
                    string changes = String.Format("{0} masked.", p.ID);
                    dbContext.PrivacyLogs.Add(new PrivacyLog { Privacy = p, creater = @"ExpirationTask", changes = changes });

                    commitcount++;
                    if (commitcount > 1000) {
                        commitcount = 0;
                        dbContext.SaveChanges();
                    }                    
                }
            } // foreach privacy  list
            dbContext.SaveChanges();
            MyLog(@"ExpirationTask", BoundType.Inbound, BatchStatus.Completed, @"");
        }


        private void MyLog(string channelname, BoundType bound, BatchStatus status, string message)
        {
            using (System.Data.Entity.DbContextTransaction dbTran = upsertContext.Database.BeginTransaction())
            {
                try
                {
                    Batch batch = new Batch { name = channelname, bound = bound, status = status, message = message };
                    upsertContext.Batches.Add(batch);
                    upsertContext.SaveChanges();
                    dbTran.Commit();
                }
                catch (Exception e)
                {
                    log.Error(e);
                    dbTran.Rollback();
                }
                dbTran.Dispose();
            }
        }
    }
}
