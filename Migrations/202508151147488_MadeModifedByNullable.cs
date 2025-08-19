namespace WorkMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeModifedByNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TestCases", "ModifiedBy", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TestCases", "ModifiedBy", c => c.String(nullable: false, maxLength: 100));
        }
    }
}
