namespace MyCards.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userdatafix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserRankings", "Restuarant_RestuarantId", "dbo.Restuarants");
            DropIndex("dbo.UserRankings", new[] { "Restuarant_RestuarantId" });
            AddColumn("dbo.UserRankings", "RestuarantId", c => c.Int(nullable: false));
            AddColumn("dbo.UserRankings", "Type", c => c.Int(nullable: false));
            DropColumn("dbo.UserRankings", "Restuarant_RestuarantId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserRankings", "Restuarant_RestuarantId", c => c.Int());
            DropColumn("dbo.UserRankings", "Type");
            DropColumn("dbo.UserRankings", "RestuarantId");
            CreateIndex("dbo.UserRankings", "Restuarant_RestuarantId");
            AddForeignKey("dbo.UserRankings", "Restuarant_RestuarantId", "dbo.Restuarants", "RestuarantId");
        }
    }
}
