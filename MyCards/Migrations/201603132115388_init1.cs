namespace MyCards.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restuarants", "Image", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Restuarants", "Image");
        }
    }
}
