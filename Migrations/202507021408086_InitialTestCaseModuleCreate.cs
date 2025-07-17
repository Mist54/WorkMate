namespace WorkMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialTestCaseModuleCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TagModels",
                c => new
                    {
                        TagId = c.Int(nullable: false, identity: true),
                        TagName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.TagId);
            
            CreateTable(
                "dbo.TestCaseTagModels",
                c => new
                    {
                        TagId = c.Int(nullable: false),
                        TestCaseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TagId, t.TestCaseId })
                .ForeignKey("dbo.TagModels", t => t.TagId, cascadeDelete: true)
                .ForeignKey("dbo.TestCaseModels", t => t.TestCaseId, cascadeDelete: true)
                .Index(t => t.TagId)
                .Index(t => t.TestCaseId);
            
            CreateTable(
                "dbo.TestCaseModels",
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
                        CreatedBy = c.String(nullable: false, maxLength: 100),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(nullable: false, maxLength: 100),
                        TaskId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TestCaseId)
                .ForeignKey("dbo.TaskModels", t => t.TaskId, cascadeDelete: true)
                .Index(t => t.TaskId);
            
            CreateTable(
                "dbo.TaskModels",
                c => new
                    {
                        TaskId = c.Int(nullable: false, identity: true),
                        TaskName = c.String(nullable: false, maxLength: 200),
                        TaskDescription = c.String(),
                    })
                .PrimaryKey(t => t.TaskId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TestCaseTagModels", "TestCaseId", "dbo.TestCaseModels");
            DropForeignKey("dbo.TestCaseModels", "TaskId", "dbo.TaskModels");
            DropForeignKey("dbo.TestCaseTagModels", "TagId", "dbo.TagModels");
            DropIndex("dbo.TestCaseModels", new[] { "TaskId" });
            DropIndex("dbo.TestCaseTagModels", new[] { "TestCaseId" });
            DropIndex("dbo.TestCaseTagModels", new[] { "TagId" });
            DropTable("dbo.TaskModels");
            DropTable("dbo.TestCaseModels");
            DropTable("dbo.TestCaseTagModels");
            DropTable("dbo.TagModels");
        }
    }
}
