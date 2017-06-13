namespace Serverside.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniqueAccountUserId : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Account", "UserId", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Account", new[] { "UserId" });
        }
    }
}
