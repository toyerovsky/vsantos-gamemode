namespace Serverside.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Neww : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroupWarehouseOrder", "OrderItemsJson", c => c.String(unicode: false));
            DropColumn("dbo.GroupWarehouseOrder", "OrderJson");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GroupWarehouseOrder", "OrderJson", c => c.String(unicode: false));
            DropColumn("dbo.GroupWarehouseOrder", "OrderItemsJson");
        }
    }
}
