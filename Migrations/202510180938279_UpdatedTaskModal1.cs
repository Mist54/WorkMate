namespace WorkMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedTaskModal1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tasks", "Status", c => c.Int(nullable: false));
            DropColumn("dbo.Tasks", "AssignedToUserName");
            DropColumn("dbo.Tasks", "AssignedByUserName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tasks", "AssignedByUserName", c => c.String());
            AddColumn("dbo.Tasks", "AssignedToUserName", c => c.String());
            DropColumn("dbo.Tasks", "Status");
        }
    }
}
