namespace MyCards.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixalltables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Groupun_Restuarant", "RankingsSum", c => c.Int(nullable: false));
            AddColumn("dbo.Groupun_Restuarant", "RankningUsersSum", c => c.Int(nullable: false));
            AddColumn("dbo.Laumi_Restuarant", "RankingsSum", c => c.Int(nullable: false));
            AddColumn("dbo.Laumi_Restuarant", "RankningUsersSum", c => c.Int(nullable: false));
            DropColumn("dbo.Restuarants", "Score");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Restuarants", "Score", c => c.Int(nullable: false));
            DropColumn("dbo.Laumi_Restuarant", "RankningUsersSum");
            DropColumn("dbo.Laumi_Restuarant", "RankingsSum");
            DropColumn("dbo.Groupun_Restuarant", "RankningUsersSum");
            DropColumn("dbo.Groupun_Restuarant", "RankingsSum");
        }
    }
}
