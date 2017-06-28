namespace Serverside.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class New : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "ForumGroup", c => c.Long(nullable: false));
            AddColumn("dbo.Account", "OtherForumGroups", c => c.String(unicode: false));
            AddColumn("dbo.Account", "ServerRank", c => c.Int(nullable: false));
            DropColumn("dbo.Account", "MainGroup");
            DropColumn("dbo.Account", "OtherGroups");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Account", "OtherGroups", c => c.String(unicode: false));
            AddColumn("dbo.Account", "MainGroup", c => c.Long(nullable: false));
            DropColumn("dbo.Account", "ServerRank");
            DropColumn("dbo.Account", "OtherForumGroups");
            DropColumn("dbo.Account", "ForumGroup");
        }
    }
}
