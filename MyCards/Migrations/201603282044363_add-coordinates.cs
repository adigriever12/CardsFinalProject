namespace MyCards.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcoordinates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Locations", "lat", c => c.String());
            AddColumn("dbo.Locations", "lng", c => c.String());
            DropColumn("dbo.Locations", "City");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Locations", "City", c => c.String());
            DropColumn("dbo.Locations", "lng");
            DropColumn("dbo.Locations", "lat");
        }
    }
}
