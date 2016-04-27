namespace MyCards.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleterankingsold : DbMigration
    {
        public override void Up()
        {
            //DropTable("dbo.UserRankingData");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserRankingData",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RestuarantId = c.Int(nullable: false),
                        rating = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RestuarantId });
            
        }
    }
}
