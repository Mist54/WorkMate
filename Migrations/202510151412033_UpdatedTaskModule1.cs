namespace WorkMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedTaskModule1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tasks", "AssignedTo", c => c.Int());
            AlterColumn("dbo.Tasks", "AssignedBy", c => c.Int());
            CreateIndex("dbo.Tasks", "AssignedTo");
            CreateIndex("dbo.Tasks", "AssignedBy");
            AddForeignKey("dbo.Tasks", "AssignedBy", "dbo.AppUsers", "Id");
            AddForeignKey("dbo.Tasks", "AssignedTo", "dbo.AppUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tasks", "AssignedTo", "dbo.AppUsers");
            DropForeignKey("dbo.Tasks", "AssignedBy", "dbo.AppUsers");
            DropIndex("dbo.Tasks", new[] { "AssignedBy" });
            DropIndex("dbo.Tasks", new[] { "AssignedTo" });
            AlterColumn("dbo.Tasks", "AssignedBy", c => c.String(maxLength: 256));
            AlterColumn("dbo.Tasks", "AssignedTo", c => c.String(maxLength: 256));
        }
    }
}
