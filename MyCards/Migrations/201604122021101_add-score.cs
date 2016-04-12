namespace MyCards.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addscore : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restuarants", "Score", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Restuarants", "Score");
        }
    }
}
