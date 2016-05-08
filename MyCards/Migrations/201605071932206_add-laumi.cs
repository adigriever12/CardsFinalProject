namespace MyCards.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addlaumi : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Laumi_Restuarant",
                c => new
                    {
                        Laumi_RestuarantId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Perks = c.String(),
                        Phone = c.String(),
                        Image = c.Binary(),
                        Location_LocationId = c.Int(),
                    })
                .PrimaryKey(t => t.Laumi_RestuarantId)
                .ForeignKey("dbo.Locations", t => t.Location_LocationId)
                .Index(t => t.Location_LocationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Laumi_Restuarant", "Location_LocationId", "dbo.Locations");
            DropIndex("dbo.Laumi_Restuarant", new[] { "Location_LocationId" });
            DropTable("dbo.Laumi_Restuarant");
        }
    }
}
