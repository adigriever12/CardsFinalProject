namespace MyCards.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Restuarants", "Location_LocationId", "dbo.Locations");
            DropIndex("dbo.Restuarants", new[] { "Location_LocationId" });
            AddColumn("dbo.Restuarants", "Address", c => c.String());
            DropColumn("dbo.Restuarants", "Location_LocationId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Restuarants", "Location_LocationId", c => c.Int());
            DropColumn("dbo.Restuarants", "Address");
            CreateIndex("dbo.Restuarants", "Location_LocationId");
            AddForeignKey("dbo.Restuarants", "Location_LocationId", "dbo.Locations", "LocationId");
        }
    }
}
