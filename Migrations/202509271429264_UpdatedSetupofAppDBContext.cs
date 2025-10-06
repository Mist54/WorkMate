namespace WorkMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedSetupofAppDBContext : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TimeTracks", "CreatedBy", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TimeTracks", "CreatedBy", c => c.String());
        }
    }
}
