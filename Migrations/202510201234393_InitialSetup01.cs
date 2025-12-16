namespace WorkMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialSetup01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tasks", "AssignedToUserId", c => c.Int(nullable: false));
            AddColumn("dbo.Tasks", "AssignedByUserId", c => c.Int(nullable: false));
            AddColumn("dbo.Tasks", "AssignedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Tasks", "StartDate", c => c.DateTime());
            AddColumn("dbo.Tasks", "EndDate", c => c.DateTime());
            AddColumn("dbo.Tasks", "Priority", c => c.Int(nullable: false));
            AddColumn("dbo.Tasks", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tasks", "Status");
            DropColumn("dbo.Tasks", "Priority");
            DropColumn("dbo.Tasks", "EndDate");
            DropColumn("dbo.Tasks", "StartDate");
            DropColumn("dbo.Tasks", "AssignedDate");
            DropColumn("dbo.Tasks", "AssignedByUserId");
            DropColumn("dbo.Tasks", "AssignedToUserId");
        }
    }
}
