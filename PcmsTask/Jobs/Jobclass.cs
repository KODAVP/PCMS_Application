using log4net;
using PcmsTask.Commons;
using PcmsTask.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace PcmsTask.Jobs
{
    public class Jobclass
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected PCMSDBContext dbContext = new PCMSDBContext();
        protected PCMSDBContext upsertContext = new PCMSDBContext();
        
        public void JobLog(Channel c, BatchStatus batchStatus ) {
            using (System.Data.Entity.DbContextTransaction dbTran = upsertContext.Database.BeginTransaction())
            {
                try
                {
                    Batch batch = new Batch { name = c.name, bound = c.bound, status = batchStatus };                    
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
        public void JobLog(Channel c, BatchStatus batchStatus, string message)
        {
            using (System.Data.Entity.DbContextTransaction dbTran = upsertContext.Database.BeginTransaction())
            {
                try
                {
                    Batch batch = new Batch { name = c.name, bound = c.bound, status = batchStatus, message = message };
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
        protected Boolean CheckFile(Channel channel, string ftpname)
        {
            Boolean b = false;
            try
            {
                Collection collection = dbContext.Collections.Where(c => c.channelId == channel.ID && c.ftpname == ftpname && c.status != CollectionStatus.FIND).FirstOrDefault();
                if (collection != null) b = true;
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return b;
        }
        protected void ChannelStatus(Channel channel, ActionStatus status) {
            using (System.Data.Entity.DbContextTransaction dbTran = upsertContext.Database.BeginTransaction())
            {
                try
                {
                    Channel c = upsertContext.Channels.Find(channel.ID);
                    c.action = status;
                    upsertContext.Entry(c).State = EntityState.Modified;
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
        protected void CollectionLog(Channel channel, string name, string ftpname, CollectionStatus status)
        {
            using (System.Data.Entity.DbContextTransaction dbTran = upsertContext.Database.BeginTransaction())
            {
                try
                {
                    Collection collection = new Collection { channelId = channel.ID, name = name, ftpname = ftpname, status = status };
                    upsertContext.Collections.Add(collection);
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

        protected Collection CollectionLog(Collection collection, CollectionStatus status)
        {
            using (System.Data.Entity.DbContextTransaction dbTran = upsertContext.Database.BeginTransaction())
            {
                try
                {
                    Collection one = upsertContext.Collections.Find(collection.ID);
                    one.status = status;
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
            return collection;
        }

        protected bool IsEqual(string a, string b)
        {
            if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b)) return true;
            if (a == b) return true;
            return false;
        }
    }
}