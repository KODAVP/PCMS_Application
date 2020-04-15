using System;
using System.Data.Entity;
using System.Linq;


namespace PcmsTask.Models
{
    public partial class PCMSDBContext : DbContext
    {
        public PCMSDBContext()
            : base("name=PUBLISH")
        {
        }

        public virtual DbSet<Approval> Approvals { get; set; }
        public virtual DbSet<Batch> Batches { get; set; }
        public virtual DbSet<Channel> Channels { get; set; }
        public virtual DbSet<Collection> Collections { get; set; }
        public virtual DbSet<Consent> Consents { get; set; }
        public virtual DbSet<N360File> N360File { get; set; }
        public virtual DbSet<PcmsId> PcmsIds { get; set; }
        public virtual DbSet<Privacy> Privacies { get; set; }
        public virtual DbSet<PrivacyLog> PrivacyLogs { get; set; }
        public virtual DbSet<Userlog> Userlogs { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }

        public virtual DbSet<Rolelog> Rolelogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Privacy>()
                .HasOptional(e => e.Approval)
                .WithRequired(e => e.Privacy);

            modelBuilder.Entity<Privacy>()
                .HasMany(e => e.Consents)
                .WithOptional(e => e.Privacy)
                .HasForeignKey(e => e.privacy_ID);

            modelBuilder.Entity<Privacy>()
                .HasMany(e => e.PrivacyLogs)
                .WithOptional(e => e.Privacy)
                .HasForeignKey(e => e.privacy_ID);
        }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries().Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified));
            var currentUsername = @"Task";
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
                        if (cname.GetValue(entity.Entity) == null) cname.SetValue(entity.Entity, currentUsername, null);
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
                    if (mname.GetValue(entity.Entity) == null) mname.SetValue(entity.Entity, currentUsername, null);
                }
            }
        }
    }
}
