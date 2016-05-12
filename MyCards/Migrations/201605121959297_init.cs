namespace MyCards.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.American_Restuarant",
                c => new
                    {
                        American_RestuarantId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Perks = c.String(),
                        Phone = c.String(),
                        Expiration = c.String(),
                        Image = c.Binary(),
                        RankingsSum = c.Int(nullable: false),
                        RankningUsersSum = c.Int(nullable: false),
                        Location_LocationId = c.Int(),
                    })
                .PrimaryKey(t => t.American_RestuarantId)
                .ForeignKey("dbo.Locations", t => t.Location_LocationId)
                .Index(t => t.Location_LocationId);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        LocationId = c.Int(nullable: false, identity: true),
                        Address = c.String(),
                        lat = c.String(),
                        lng = c.String(),
                    })
                .PrimaryKey(t => t.LocationId);
            
            CreateTable(
                "dbo.Cuisines",
                c => new
                    {
                        CuisineId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.CuisineId);
            
            CreateTable(
                "dbo.Groupun_Restuarant",
                c => new
                    {
                        Groupun_RestuarantId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        CopunDescription = c.String(),
                        Kosher = c.String(),
                        Expiration = c.String(),
                        Hours = c.String(),
                        PhoneAndContent = c.String(),
                        Image = c.Binary(),
                        RankingsSum = c.Int(nullable: false),
                        RankningUsersSum = c.Int(nullable: false),
                        Category_CategoryId = c.Int(),
                        Location_LocationId = c.Int(),
                    })
                .PrimaryKey(t => t.Groupun_RestuarantId)
                .ForeignKey("dbo.Categories", t => t.Category_CategoryId)
                .ForeignKey("dbo.Locations", t => t.Location_LocationId)
                .Index(t => t.Category_CategoryId)
                .Index(t => t.Location_LocationId);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.CategoryId);
            
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
                        RankingsSum = c.Int(nullable: false),
                        RankningUsersSum = c.Int(nullable: false),
                        Location_LocationId = c.Int(),
                    })
                .PrimaryKey(t => t.Laumi_RestuarantId)
                .ForeignKey("dbo.Locations", t => t.Location_LocationId)
                .Index(t => t.Location_LocationId);
            
            CreateTable(
                "dbo.Restuarants",
                c => new
                    {
                        RestuarantId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        RankingsSum = c.Int(nullable: false),
                        RankningUsersSum = c.Int(nullable: false),
                        Kosher = c.String(),
                        Phone = c.String(),
                        HandicapAccessibility = c.Boolean(nullable: false),
                        Image = c.Binary(),
                        OpeningHours = c.String(),
                        Category_CategoryId = c.Int(),
                        Cuisine_CuisineId = c.Int(),
                        Location_LocationId = c.Int(),
                    })
                .PrimaryKey(t => t.RestuarantId)
                .ForeignKey("dbo.Categories", t => t.Category_CategoryId)
                .ForeignKey("dbo.Cuisines", t => t.Cuisine_CuisineId)
                .ForeignKey("dbo.Locations", t => t.Location_LocationId)
                .Index(t => t.Category_CategoryId)
                .Index(t => t.Cuisine_CuisineId)
                .Index(t => t.Location_LocationId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.UserRankings",
                c => new
                    {
                        UserRankingId = c.Int(nullable: false, identity: true),
                        RestuarantId = c.Int(nullable: false),
                        Ranking = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserRankingId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserRankings", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Restuarants", "Location_LocationId", "dbo.Locations");
            DropForeignKey("dbo.Restuarants", "Cuisine_CuisineId", "dbo.Cuisines");
            DropForeignKey("dbo.Restuarants", "Category_CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Laumi_Restuarant", "Location_LocationId", "dbo.Locations");
            DropForeignKey("dbo.Groupun_Restuarant", "Location_LocationId", "dbo.Locations");
            DropForeignKey("dbo.Groupun_Restuarant", "Category_CategoryId", "dbo.Categories");
            DropForeignKey("dbo.American_Restuarant", "Location_LocationId", "dbo.Locations");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.UserRankings", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Restuarants", new[] { "Location_LocationId" });
            DropIndex("dbo.Restuarants", new[] { "Cuisine_CuisineId" });
            DropIndex("dbo.Restuarants", new[] { "Category_CategoryId" });
            DropIndex("dbo.Laumi_Restuarant", new[] { "Location_LocationId" });
            DropIndex("dbo.Groupun_Restuarant", new[] { "Location_LocationId" });
            DropIndex("dbo.Groupun_Restuarant", new[] { "Category_CategoryId" });
            DropIndex("dbo.American_Restuarant", new[] { "Location_LocationId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.UserRankings");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Restuarants");
            DropTable("dbo.Laumi_Restuarant");
            DropTable("dbo.Categories");
            DropTable("dbo.Groupun_Restuarant");
            DropTable("dbo.Cuisines");
            DropTable("dbo.Locations");
            DropTable("dbo.American_Restuarant");
        }
    }
}
