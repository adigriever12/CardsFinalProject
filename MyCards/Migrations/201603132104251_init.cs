namespace MyCards.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cuisines",
                c => new
                    {
                        CuisineId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.CuisineId);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        LocationId = c.Int(nullable: false, identity: true),
                        Address = c.String(),
                        City = c.String(),
                    })
                .PrimaryKey(t => t.LocationId);
            
            CreateTable(
                "dbo.Restuarants",
                c => new
                    {
                        RestuarantId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Kosher = c.String(),
                        Phone = c.String(),
                        HandicapAccessibility = c.Boolean(nullable: false),
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
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        ReviewId = c.Int(nullable: false, identity: true),
                        Comment = c.String(),
                        Rating = c.Int(nullable: false),
                        Restuarant_RestuarantId = c.Int(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ReviewId)
                .ForeignKey("dbo.Restuarants", t => t.Restuarant_RestuarantId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.Restuarant_RestuarantId)
                .Index(t => t.User_Id);
            
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
                        HomeLocation_LocationId = c.Int(),
                        WorkLocation_LocationId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Locations", t => t.HomeLocation_LocationId)
                .ForeignKey("dbo.Locations", t => t.WorkLocation_LocationId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.HomeLocation_LocationId)
                .Index(t => t.WorkLocation_LocationId);
            
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
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
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
                "dbo.VisitHistories",
                c => new
                    {
                        VisitHistoryId = c.Int(nullable: false, identity: true),
                        Restuarant_RestuarantId = c.Int(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.VisitHistoryId)
                .ForeignKey("dbo.Restuarants", t => t.Restuarant_RestuarantId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.Restuarant_RestuarantId)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VisitHistories", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.VisitHistories", "Restuarant_RestuarantId", "dbo.Restuarants");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Reviews", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "WorkLocation_LocationId", "dbo.Locations");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "HomeLocation_LocationId", "dbo.Locations");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Reviews", "Restuarant_RestuarantId", "dbo.Restuarants");
            DropForeignKey("dbo.Restuarants", "Location_LocationId", "dbo.Locations");
            DropForeignKey("dbo.Restuarants", "Cuisine_CuisineId", "dbo.Cuisines");
            DropForeignKey("dbo.Restuarants", "Category_CategoryId", "dbo.Categories");
            DropIndex("dbo.VisitHistories", new[] { "User_Id" });
            DropIndex("dbo.VisitHistories", new[] { "Restuarant_RestuarantId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "WorkLocation_LocationId" });
            DropIndex("dbo.AspNetUsers", new[] { "HomeLocation_LocationId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Reviews", new[] { "User_Id" });
            DropIndex("dbo.Reviews", new[] { "Restuarant_RestuarantId" });
            DropIndex("dbo.Restuarants", new[] { "Location_LocationId" });
            DropIndex("dbo.Restuarants", new[] { "Cuisine_CuisineId" });
            DropIndex("dbo.Restuarants", new[] { "Category_CategoryId" });
            DropTable("dbo.VisitHistories");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Reviews");
            DropTable("dbo.Categories");
            DropTable("dbo.Restuarants");
            DropTable("dbo.Locations");
            DropTable("dbo.Cuisines");
        }
    }
}
