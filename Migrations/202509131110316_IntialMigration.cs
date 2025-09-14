namespace WorkMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IntialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 200),
                        CreatedBy = c.String(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                        CreatedBy = c.String(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AppUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AppUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PasswordSalt = c.String(maxLength: 256),
                        IsActive = c.Boolean(nullable: false),
                        Designation = c.String(maxLength: 50),
                        LastLoginAt = c.DateTime(),
                        LastLoginIP = c.String(maxLength: 45),
                        FailedLoginAttempts = c.Int(nullable: false),
                        LockoutEnd = c.DateTime(),
                        CreatedBy = c.String(maxLength: 256),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
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
                "dbo.UserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AppUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AppUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        TagId = c.Int(nullable: false, identity: true),
                        TagName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.TagId);
            
            CreateTable(
                "dbo.TestCaseTags",
                c => new
                    {
                        TagId = c.Int(nullable: false),
                        TestCaseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TagId, t.TestCaseId })
                .ForeignKey("dbo.Tags", t => t.TagId, cascadeDelete: true)
                .ForeignKey("dbo.TestCases", t => t.TestCaseId, cascadeDelete: true)
                .Index(t => t.TagId)
                .Index(t => t.TestCaseId);
            
            CreateTable(
                "dbo.TestCases",
                c => new
                    {
                        TestCaseId = c.Int(nullable: false, identity: true),
                        TestCaseName = c.String(nullable: false, maxLength: 200),
                        TestCaseTimeStamp = c.DateTime(nullable: false),
                        Preconditions = c.String(),
                        Steps = c.String(),
                        ExpectedResult = c.String(),
                        ActualResult = c.String(),
                        Type = c.Int(nullable: false),
                        Priority = c.Int(nullable: false),
                        TestCaseStatus = c.Int(nullable: false),
                        Notes = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        TaskId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TestCaseId)
                .ForeignKey("dbo.Tasks", t => t.TaskId, cascadeDelete: true)
                .Index(t => t.TaskId);
            
            CreateTable(
                "dbo.Tasks",
                c => new
                    {
                        TaskId = c.Int(nullable: false, identity: true),
                        TaskName = c.String(nullable: false, maxLength: 200),
                        TaskDescription = c.String(),
                        CreatedBy = c.String(maxLength: 256),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TaskId);
            
            CreateTable(
                "dbo.TimeTracks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TaskId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Description = c.String(nullable: false, maxLength: 500),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        Status = c.Int(nullable: false),
                        IsManual = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AppUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Tasks", t => t.TaskId, cascadeDelete: true)
                .Index(t => t.TaskId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TimeTracks", "TaskId", "dbo.Tasks");
            DropForeignKey("dbo.TimeTracks", "UserId", "dbo.AppUsers");
            DropForeignKey("dbo.TestCaseTags", "TestCaseId", "dbo.TestCases");
            DropForeignKey("dbo.TestCases", "TaskId", "dbo.Tasks");
            DropForeignKey("dbo.TestCaseTags", "TagId", "dbo.Tags");
            DropForeignKey("dbo.UserRoles", "UserId", "dbo.AppUsers");
            DropForeignKey("dbo.UserLogins", "UserId", "dbo.AppUsers");
            DropForeignKey("dbo.UserClaims", "UserId", "dbo.AppUsers");
            DropForeignKey("dbo.UserRoles", "RoleId", "dbo.Roles");
            DropIndex("dbo.TimeTracks", new[] { "UserId" });
            DropIndex("dbo.TimeTracks", new[] { "TaskId" });
            DropIndex("dbo.TestCases", new[] { "TaskId" });
            DropIndex("dbo.TestCaseTags", new[] { "TestCaseId" });
            DropIndex("dbo.TestCaseTags", new[] { "TagId" });
            DropIndex("dbo.UserLogins", new[] { "UserId" });
            DropIndex("dbo.UserClaims", new[] { "UserId" });
            DropIndex("dbo.AppUsers", "UserNameIndex");
            DropIndex("dbo.UserRoles", new[] { "RoleId" });
            DropIndex("dbo.UserRoles", new[] { "UserId" });
            DropIndex("dbo.Roles", "RoleNameIndex");
            DropTable("dbo.TimeTracks");
            DropTable("dbo.Tasks");
            DropTable("dbo.TestCases");
            DropTable("dbo.TestCaseTags");
            DropTable("dbo.Tags");
            DropTable("dbo.UserLogins");
            DropTable("dbo.UserClaims");
            DropTable("dbo.AppUsers");
            DropTable("dbo.UserRoles");
            DropTable("dbo.Roles");
        }
    }
}
