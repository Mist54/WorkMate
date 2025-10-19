namespace WorkMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResetBack : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tasks", "AssignedBy", "dbo.AppUsers");
            DropForeignKey("dbo.Tasks", "AssignedTo", "dbo.AppUsers");
            DropIndex("dbo.Tasks", new[] { "AssignedTo" });
            DropIndex("dbo.Tasks", new[] { "AssignedBy" });
            DropColumn("dbo.Tasks", "AssignedTo");
            DropColumn("dbo.Tasks", "AssignedBy");
            DropColumn("dbo.Tasks", "StartDate");
            DropColumn("dbo.Tasks", "EndDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tasks", "EndDate", c => c.DateTime());
            AddColumn("dbo.Tasks", "StartDate", c => c.DateTime());
            AddColumn("dbo.Tasks", "AssignedBy", c => c.Int());
            AddColumn("dbo.Tasks", "AssignedTo", c => c.Int());
            CreateIndex("dbo.Tasks", "AssignedBy");
            CreateIndex("dbo.Tasks", "AssignedTo");
            AddForeignKey("dbo.Tasks", "AssignedTo", "dbo.AppUsers", "Id");
            AddForeignKey("dbo.Tasks", "AssignedBy", "dbo.AppUsers", "Id");
        }
    }
}
