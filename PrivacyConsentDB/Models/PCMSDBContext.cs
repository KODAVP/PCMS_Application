using System;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PrivacyConsentDB.Models
{
    public class PCMSDBContext : DbContext
    {
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Batch> Batchs { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Privacy> Privacies { get; set; }
        public DbSet<Approval> Approvals { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Company> Companies { get; set; }


        // connectionString setting        
        public PCMSDBContext() : base("PUBLISH")
        {
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public int SaveChanges2noDate() {
            return base.SaveChanges();
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries().Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified));
            var currentUsername = !string.IsNullOrEmpty(System.Web.HttpContext.Current?.User?.Identity?.Name) ? HttpContext.Current.User.Identity.Name : "Anonymous";
            currentUsername = currentUsername.Substring(currentUsername.IndexOf('\\') + 1).Replace("\\", "").ToUpper();

            foreach (var entity in entities)
            {                
                if (entity.State == EntityState.Added)
                {
                    var cdate = entity.Entity.GetType().GetProperty("createdate");
                    if (cdate != null)
                    {
                        cdate.SetValue(entity.Entity, DateTime.UtcNow, null);
                    }
                    var cname = entity.Entity.GetType().GetProperty("creater");
                    if (cname != null)
                    {
                        if(cname.GetValue(entity.Entity) == null )
                            cname.SetValue(entity.Entity, currentUsername, null);
                    }
                }                                
                var mdate = entity.Entity.GetType().GetProperty("modifieddate");
                if (mdate != null)
                {
                    mdate.SetValue(entity.Entity, DateTime.UtcNow, null);
                }
                var mname = entity.Entity.GetType().GetProperty("modifier");
                if (mname != null)
                {
                    mname.SetValue(entity.Entity, currentUsername, null);
                }
            }
        }

        public System.Data.Entity.DbSet<PrivacyConsentDB.Models.Agreement> Agreements { get; set; }

        public System.Data.Entity.DbSet<PrivacyConsentDB.Models.Userlog> Userlogs { get; set; }

        public System.Data.Entity.DbSet<PrivacyConsentDB.Models.Rolelog> Rolelogs { get; set; }

        public System.Data.Entity.DbSet<PrivacyConsentDB.Models.PcmsId> PcmsIds { get; set; }

        public System.Data.Entity.DbSet<PrivacyConsentDB.Models.PrivacyLog> Privacylogs { get; set; }

        public System.Data.Entity.DbSet<PrivacyConsentDB.Models.Consent> Consents { get; set; }

        public System.Data.Entity.DbSet<PrivacyConsentDB.AccessRoles> AccessRoles { get; set; }

        public System.Data.Entity.DbSet<PrivacyConsentDB.AccessAuthorities> AccessAuthorities { get; set; }

        public System.Data.Entity.DbSet<PrivacyConsentDB.AccessPaths> AccessPaths { get; set; }
        
    }

    
}