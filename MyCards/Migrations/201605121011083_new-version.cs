namespace MyCards.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newversion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restuarants", "RankingsSum", c => c.Int(nullable: false));
            AddColumn("dbo.Restuarants", "RankningUsersSum", c => c.Int(nullable: false));
            AddColumn("dbo.UserRankings", "Ranking", c => c.Int(nullable: false));
            DropColumn("dbo.Restuarants", "AverageRanking");
            DropColumn("dbo.UserRankings", "Rating");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserRankings", "Rating", c => c.Double(nullable: false));
            AddColumn("dbo.Restuarants", "AverageRanking", c => c.Double(nullable: false));
            DropColumn("dbo.UserRankings", "Ranking");
            DropColumn("dbo.Restuarants", "RankningUsersSum");
            DropColumn("dbo.Restuarants", "RankingsSum");
        }
    }
}
