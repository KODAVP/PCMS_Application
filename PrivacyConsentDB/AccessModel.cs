namespace PrivacyConsentDB
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AccessModel : DbContext
    {
        public AccessModel()
            : base("name=AccessModel")
        {
        }

        public virtual DbSet<AccessAuthorities> AccessAuthorities { get; set; }
        public virtual DbSet<AccessPaths> AccessPaths { get; set; }
        public virtual DbSet<AccessRoles> AccessRoles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccessPaths>()
                .Property(e => e.path)
                .IsUnicode(false);

            modelBuilder.Entity<AccessPaths>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<AccessPaths>()
                .HasMany(e => e.AccessAuthorities)
                .WithRequired(e => e.AccessPaths)
                .HasForeignKey(e => e.pathID);

            modelBuilder.Entity<AccessRoles>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<AccessRoles>()
                .HasMany(e => e.AccessAuthorities)
                .WithRequired(e => e.AccessRoles)
                .HasForeignKey(e => e.roleID);
        }
    }
}
