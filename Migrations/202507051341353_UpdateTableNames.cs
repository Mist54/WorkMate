namespace WorkMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTableNames : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.TagModels", newName: "Tags");
            RenameTable(name: "dbo.TestCaseTagModels", newName: "TestCaseTags");
            RenameTable(name: "dbo.TestCaseModels", newName: "TestCases");
            RenameTable(name: "dbo.TaskModels", newName: "Tasks");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Tasks", newName: "TaskModels");
            RenameTable(name: "dbo.TestCases", newName: "TestCaseModels");
            RenameTable(name: "dbo.TestCaseTags", newName: "TestCaseTagModels");
            RenameTable(name: "dbo.Tags", newName: "TagModels");
        }
    }
}
