namespace MyCards.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsuggestionalgoritm : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserRankingData",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RestuarantId = c.Int(nullable: false),
                        rating = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RestuarantId });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserRankingData");
        }
    }
}
