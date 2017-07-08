namespace Serverside.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class namefetch : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "Name", c => c.String(maxLength: 50, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Account", "Name");
        }
    }
}
