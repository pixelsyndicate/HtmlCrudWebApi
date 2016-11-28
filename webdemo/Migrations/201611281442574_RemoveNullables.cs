namespace webdemo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveNullables : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProductEntities", "ProductName", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductEntities", "ProductName", c => c.String(maxLength: 50));
        }
    }
}
