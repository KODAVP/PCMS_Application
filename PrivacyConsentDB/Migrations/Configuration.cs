namespace PrivacyConsentDB.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<PrivacyConsentDB.Models.PCMSDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(PrivacyConsentDB.Models.PCMSDBContext context)
        {
            
        }
    }
}
