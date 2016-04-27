namespace MyCards.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newranking1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserRankings",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RestuarantId = c.Int(nullable: false),
                        rating = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RestuarantId });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserRankings");
        }
    }
}
