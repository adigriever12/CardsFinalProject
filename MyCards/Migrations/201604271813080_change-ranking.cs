namespace MyCards.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeranking : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.UserRankingData");
            AlterColumn("dbo.UserRankingData", "UserId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.UserRankingData", new[] { "UserId", "RestuarantId" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.UserRankingData");
            AlterColumn("dbo.UserRankingData", "UserId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.UserRankingData", new[] { "UserId", "RestuarantId" });
        }
    }
}
