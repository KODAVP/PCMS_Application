namespace PrivacyConsentDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmigrationconsent : DbMigration
    {
        public override void Up()
        {
            AddColumn("Consents", "CONSENT_MARKETING_AGREEMENT", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Consents", "CONSENT_MARKETING_AGREEMENT");
        }
    }
}
