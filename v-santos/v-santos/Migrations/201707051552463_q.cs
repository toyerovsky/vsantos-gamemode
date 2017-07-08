namespace Serverside.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class q : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Group", "BossId", c => c.Long(nullable: false));
            AddColumn("dbo.Group", "Color", c => c.String(unicode: false));
            AlterColumn("dbo.Character", "BankAccountNumber", c => c.Int());
            DropColumn("dbo.Character", "Skin");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Character", "Skin", c => c.Int());
            AlterColumn("dbo.Character", "BankAccountNumber", c => c.Long(nullable: false));
            DropColumn("dbo.Group", "Color");
            DropColumn("dbo.Group", "BossId");
        }
    }
}
