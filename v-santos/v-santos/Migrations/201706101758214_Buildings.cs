namespace Serverside.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Buildings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Building", "Cost", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Building", "EnterCharge", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Building", "EnterCharge", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Building", "Cost");
        }
    }
}
