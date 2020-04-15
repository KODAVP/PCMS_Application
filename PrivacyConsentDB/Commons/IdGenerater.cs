using log4net;
using PrivacyConsentDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace PrivacyConsentDB.Commons
{
    public class IdGenerater
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);        

        public static string Generater() {
            PCMSDBContext dbContext = new PCMSDBContext();
            PcmsId id = new PcmsId();
            using (System.Data.Entity.DbContextTransaction dbTran = dbContext.Database.BeginTransaction())
            {
                try
                {                    
                    dbContext.PcmsIds.Add(id);
                    dbContext.SaveChanges();
                    
                    id.KEY = String.Format("P{0:D6}", id.ID);
                    dbContext.SaveChanges();
                    dbTran.Commit();
                }
                catch (Exception e)
                {
                    log.Error(e);
                    dbTran.Rollback();
                }
                dbTran.Dispose();
            }
            return id.KEY;
        }
    }
}