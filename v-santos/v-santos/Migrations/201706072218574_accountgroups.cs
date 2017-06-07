namespace Serverside.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class accountgroups : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "MainGroup", c => c.Long(nullable: false));
            AddColumn("dbo.Account", "OtherGroups", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Account", "OtherGroups");
            DropColumn("dbo.Account", "MainGroup");
        }
    }
}
