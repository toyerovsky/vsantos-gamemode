namespace Serverside.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Creator : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "AccessoryId", c => c.Int());
            AddColumn("dbo.Character", "AccessoryTexture", c => c.Int());
            AddColumn("dbo.Character", "EarsId", c => c.Int());
            AddColumn("dbo.Character", "EarsTexture", c => c.Int());
            AddColumn("dbo.Character", "EyebrowsId", c => c.Int());
            AddColumn("dbo.Character", "EyeBrowsOpacity", c => c.Single());
            AddColumn("dbo.Character", "FatherId", c => c.Int());
            AddColumn("dbo.Character", "ShoesId", c => c.Int());
            AddColumn("dbo.Character", "ShoesTexture", c => c.Int());
            AddColumn("dbo.Character", "FirstEyebrowsColor", c => c.Int());
            AddColumn("dbo.Character", "FirstLipstickColor", c => c.Int());
            AddColumn("dbo.Character", "FirstMakeupColor", c => c.Int());
            AddColumn("dbo.Character", "GlassesId", c => c.Int());
            AddColumn("dbo.Character", "GlassesTexture", c => c.Int());
            AddColumn("dbo.Character", "HairId", c => c.Int());
            AddColumn("dbo.Character", "HairTexture", c => c.Int());
            AddColumn("dbo.Character", "HairColor", c => c.Int());
            AddColumn("dbo.Character", "HatId", c => c.Int());
            AddColumn("dbo.Character", "HatTexture", c => c.Int());
            AddColumn("dbo.Character", "LegsId", c => c.Int());
            AddColumn("dbo.Character", "LegsTexture", c => c.Int());
            AddColumn("dbo.Character", "LipstickOpacity", c => c.Single());
            AddColumn("dbo.Character", "MakeupId", c => c.Int());
            AddColumn("dbo.Character", "MakeupOpacity", c => c.Single());
            AddColumn("dbo.Character", "MotherId", c => c.Int());
            AddColumn("dbo.Character", "SecondEyebrowsColor", c => c.Int());
            AddColumn("dbo.Character", "SecondLipstickColor", c => c.Int());
            AddColumn("dbo.Character", "SecondMakeupColor", c => c.Int());
            AddColumn("dbo.Character", "ShapeMix", c => c.Single());
            AddColumn("dbo.Character", "TopId", c => c.Int());
            AddColumn("dbo.Character", "TopTexture", c => c.Int());
            AddColumn("dbo.Character", "TorsoId", c => c.Int());
            AddColumn("dbo.Character", "UndershirtId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Character", "UndershirtId");
            DropColumn("dbo.Character", "TorsoId");
            DropColumn("dbo.Character", "TopTexture");
            DropColumn("dbo.Character", "TopId");
            DropColumn("dbo.Character", "ShapeMix");
            DropColumn("dbo.Character", "SecondMakeupColor");
            DropColumn("dbo.Character", "SecondLipstickColor");
            DropColumn("dbo.Character", "SecondEyebrowsColor");
            DropColumn("dbo.Character", "MotherId");
            DropColumn("dbo.Character", "MakeupOpacity");
            DropColumn("dbo.Character", "MakeupId");
            DropColumn("dbo.Character", "LipstickOpacity");
            DropColumn("dbo.Character", "LegsTexture");
            DropColumn("dbo.Character", "LegsId");
            DropColumn("dbo.Character", "HatTexture");
            DropColumn("dbo.Character", "HatId");
            DropColumn("dbo.Character", "HairColor");
            DropColumn("dbo.Character", "HairTexture");
            DropColumn("dbo.Character", "HairId");
            DropColumn("dbo.Character", "GlassesTexture");
            DropColumn("dbo.Character", "GlassesId");
            DropColumn("dbo.Character", "FirstMakeupColor");
            DropColumn("dbo.Character", "FirstLipstickColor");
            DropColumn("dbo.Character", "FirstEyebrowsColor");
            DropColumn("dbo.Character", "ShoesTexture");
            DropColumn("dbo.Character", "ShoesId");
            DropColumn("dbo.Character", "FatherId");
            DropColumn("dbo.Character", "EyeBrowsOpacity");
            DropColumn("dbo.Character", "EyebrowsId");
            DropColumn("dbo.Character", "EarsTexture");
            DropColumn("dbo.Character", "EarsId");
            DropColumn("dbo.Character", "AccessoryTexture");
            DropColumn("dbo.Character", "AccessoryId");
        }
    }
}
