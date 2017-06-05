namespace Serverside.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class small_rename : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "CreateTime", c => c.DateTime(precision: 0));
            DropColumn("dbo.Character", "CreateAccountTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Character", "CreateAccountTime", c => c.DateTime(precision: 0));
            DropColumn("dbo.Character", "CreateTime");
        }
    }
}
