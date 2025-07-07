namespace WorkMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTableNames1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TestCases", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Tasks", "CreatedBy", c => c.String(maxLength: 256));
            AddColumn("dbo.Tasks", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Tasks", "ModifiedBy", c => c.String());
            AddColumn("dbo.Tasks", "ModifiedDate", c => c.DateTime());
            AddColumn("dbo.Tasks", "IsDeleted", c => c.Boolean(nullable: false));
            AlterColumn("dbo.TestCases", "CreatedBy", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TestCases", "CreatedBy", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.Tasks", "IsDeleted");
            DropColumn("dbo.Tasks", "ModifiedDate");
            DropColumn("dbo.Tasks", "ModifiedBy");
            DropColumn("dbo.Tasks", "CreatedDate");
            DropColumn("dbo.Tasks", "CreatedBy");
            DropColumn("dbo.TestCases", "IsDeleted");
        }
    }
}
