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
    class DeleteTask : Jobclass
    {
        private IQueryable<Privacy> getDeletePrivacies()
        {
            return dbContext.Privacies.Where(p => p.SENDCHANEL.HasFlag(SendChannel.N360) && p.status == PrivacyStatus.ERASED);
        }

        public void Execute()
        {
            MyLog(@"DeleteTask", BoundType.Inbound, BatchStatus.Begin, @"");
            // 삭제 완료
            IEnumerable<Privacy> deltelist = getDeletePrivacies().ToList();
            int commitcount = 0;
            foreach (Privacy p in deltelist)
            {
                p.status = PrivacyStatus.DELETED;
                p.SENDCHANEL &= ~SendChannel.N360;
                
                dbContext.Entry(p).State = System.Data.Entity.EntityState.Modified;
                string changes = String.Format("{0} deleted.", p.ID);
                dbContext.PrivacyLogs.Add(new PrivacyLog { Privacy = p, creater = @"DeleteTask", changes = changes });

                commitcount++;
                if (commitcount > 1000)
                {
                    commitcount = 0;
                    dbContext.SaveChanges();
                }                
            } // foreach
            dbContext.SaveChanges();
            MyLog(@"DeleteTask", BoundType.Inbound, BatchStatus.Completed, @"");
        }

        private void MyLog(string channelname ,  BoundType bound, BatchStatus status, string message) {
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
