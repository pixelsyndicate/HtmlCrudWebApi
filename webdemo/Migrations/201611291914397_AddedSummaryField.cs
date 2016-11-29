namespace webdemo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSummaryField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductEntities", "Summary", c => c.String(maxLength: 500));
            AlterColumn("dbo.ProductEntities", "ProductName", c => c.String(nullable: false, maxLength: 150));
            AlterColumn("dbo.ProductEntities", "IntroductionDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ProductEntities", "Url", c => c.String(nullable: false));
            AlterColumn("dbo.ProductEntities", "Price", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductEntities", "Price", c => c.Double());
            AlterColumn("dbo.ProductEntities", "Url", c => c.String());
            AlterColumn("dbo.ProductEntities", "IntroductionDate", c => c.DateTime());
            AlterColumn("dbo.ProductEntities", "ProductName", c => c.String(maxLength: 150));
            DropColumn("dbo.ProductEntities", "Summary");
        }
    }
}
