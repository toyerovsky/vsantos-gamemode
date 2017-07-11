namespace Serverside.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class New1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupWarehouseItem",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MinimalCost = c.Decimal(precision: 18, scale: 2),
                        Count = c.Int(nullable: false),
                        ResetCount = c.Int(nullable: false),
                        GroupType = c.Int(nullable: false),
                        Item_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Item", t => t.Item_Id)
                .Index(t => t.Item_Id);
            
            CreateTable(
                "dbo.GroupWarehouseOrder",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OrderJson = c.String(unicode: false),
                        ShipmentLog = c.String(unicode: false),
                        Getter_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Group", t => t.Getter_Id)
                .Index(t => t.Getter_Id);
            
            AddColumn("dbo.Character", "JobReleaseDate", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.Worker", "OrderFromWarehouseRight", c => c.Boolean(nullable: false));
            AddColumn("dbo.Item", "Group_Id", c => c.Long());
            CreateIndex("dbo.Item", "Group_Id");
            AddForeignKey("dbo.Item", "Group_Id", "dbo.Group", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupWarehouseOrder", "Getter_Id", "dbo.Group");
            DropForeignKey("dbo.GroupWarehouseItem", "Item_Id", "dbo.Item");
            DropForeignKey("dbo.Item", "Group_Id", "dbo.Group");
            DropIndex("dbo.GroupWarehouseOrder", new[] { "Getter_Id" });
            DropIndex("dbo.GroupWarehouseItem", new[] { "Item_Id" });
            DropIndex("dbo.Item", new[] { "Group_Id" });
            DropColumn("dbo.Item", "Group_Id");
            DropColumn("dbo.Worker", "OrderFromWarehouseRight");
            DropColumn("dbo.Character", "JobReleaseDate");
            DropTable("dbo.GroupWarehouseOrder");
            DropTable("dbo.GroupWarehouseItem");
        }
    }
}
