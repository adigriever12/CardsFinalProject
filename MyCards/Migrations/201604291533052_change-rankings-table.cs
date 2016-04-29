namespace MyCards.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changerankingstable : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.UserRankings");
            AddColumn("dbo.UserRankings", "UserRankingId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.UserRankings", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.UserRankings", "Restuarant_RestuarantId", c => c.Int());
            AddPrimaryKey("dbo.UserRankings", "UserRankingId");
            CreateIndex("dbo.UserRankings", "ApplicationUser_Id");
            CreateIndex("dbo.UserRankings", "Restuarant_RestuarantId");
            AddForeignKey("dbo.UserRankings", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.UserRankings", "Restuarant_RestuarantId", "dbo.Restuarants", "RestuarantId");
            DropColumn("dbo.UserRankings", "UserId");
            DropColumn("dbo.UserRankings", "RestuarantId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserRankings", "RestuarantId", c => c.Int(nullable: false));
            AddColumn("dbo.UserRankings", "UserId", c => c.String(nullable: false, maxLength: 128));
            DropForeignKey("dbo.UserRankings", "Restuarant_RestuarantId", "dbo.Restuarants");
            DropForeignKey("dbo.UserRankings", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.UserRankings", new[] { "Restuarant_RestuarantId" });
            DropIndex("dbo.UserRankings", new[] { "ApplicationUser_Id" });
            DropPrimaryKey("dbo.UserRankings");
            DropColumn("dbo.UserRankings", "Restuarant_RestuarantId");
            DropColumn("dbo.UserRankings", "ApplicationUser_Id");
            DropColumn("dbo.UserRankings", "UserRankingId");
            AddPrimaryKey("dbo.UserRankings", new[] { "UserId", "RestuarantId" });
        }
    }
}
