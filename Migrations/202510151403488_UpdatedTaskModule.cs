namespace WorkMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedTaskModule : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tasks", "AssignedTo", c => c.String(maxLength: 256));
            AddColumn("dbo.Tasks", "AssignedBy", c => c.String(maxLength: 256));
            AddColumn("dbo.Tasks", "StartDate", c => c.DateTime());
            AddColumn("dbo.Tasks", "EndDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tasks", "EndDate");
            DropColumn("dbo.Tasks", "StartDate");
            DropColumn("dbo.Tasks", "AssignedBy");
            DropColumn("dbo.Tasks", "AssignedTo");
        }
    }
}
