namespace Serverside.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.Account",
                    c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50, storeType: "nvarchar"),
                        UserId = c.Long(nullable: false),
                        Email = c.String(maxLength: 50, storeType: "nvarchar"),
                        ForumGroup = c.Long(nullable: false),
                        OtherForumGroups = c.String(unicode: false),
                        SocialClub = c.String(maxLength: 50, storeType: "nvarchar"),
                        Ip = c.String(maxLength: 16, storeType: "nvarchar"),
                        Online = c.Boolean(nullable: false),
                        LastLogin = c.DateTime(nullable: false, precision: 0),
                        ServerRank = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserId, unique: true)
                .Index(t => t.SocialClub);

            CreateTable(
                    "dbo.Ban",
                    c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Active = c.Boolean(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        AccountId = c.Int(nullable: false),
                        Ip = c.String(maxLength: 16, storeType: "nvarchar"),
                        SocialClub = c.String(maxLength: 24, storeType: "nvarchar"),
                        IsSocialClubBanned = c.Boolean(nullable: false),
                        BanDate = c.DateTime(nullable: false, precision: 0),
                        ExpiryDate = c.DateTime(nullable: false, precision: 0),
                        BanReason = c.String(maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Account", t => t.AccountId, cascadeDelete: true)
                .Index(t => t.AccountId);

            CreateTable(
                    "dbo.Character",
                    c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Online = c.Boolean(nullable: false),
                        CreateTime = c.DateTime(precision: 0),
                        LastLoginTime = c.DateTime(precision: 0),
                        TodayPlayedTime = c.DateTime(precision: 0),
                        Name = c.String(unicode: false),
                        Surname = c.String(unicode: false),
                        Model = c.Int(nullable: false),
                        ModelName = c.String(unicode: false),
                        Money = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BankAccountNumber = c.Int(),
                        BankMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Gender = c.Boolean(nullable: false),
                        Weight = c.Short(nullable: false),
                        BornDate = c.DateTime(nullable: false, precision: 0),
                        Height = c.Short(nullable: false),
                        Force = c.Short(nullable: false),
                        RunningEfficiency = c.Short(nullable: false),
                        DivingEfficiency = c.Short(nullable: false),
                        HasIDCard = c.Boolean(nullable: false),
                        HasDrivingLicense = c.Boolean(nullable: false),
                        ForumDescription = c.String(unicode: false),
                        Story = c.String(unicode: false),
                        IsAlive = c.Boolean(nullable: false),
                        HitPoints = c.Int(nullable: false),
                        LastPositionX = c.Single(nullable: false),
                        LastPositionY = c.Single(nullable: false),
                        LastPositionZ = c.Single(nullable: false),
                        LastPositionRotX = c.Single(nullable: false),
                        LastPositionRotY = c.Single(nullable: false),
                        LastPositionRotZ = c.Single(nullable: false),
                        CurrentDimension = c.Int(nullable: false),
                        BWState = c.Int(nullable: false),
                        IsCreated = c.Boolean(),
                        Freemode = c.Boolean(nullable: false),
                        Job = c.Int(nullable: false),
                        MoneyJob = c.Decimal(precision: 18, scale: 2),
                        JobLimit = c.Decimal(precision: 18, scale: 2),
                        AccessoryId = c.Int(),
                        AccessoryTexture = c.Int(),
                        EarsId = c.Int(),
                        EarsTexture = c.Int(),
                        EyebrowsId = c.Int(),
                        EyeBrowsOpacity = c.Single(),
                        FatherId = c.Int(),
                        ShoesId = c.Int(),
                        ShoesTexture = c.Int(),
                        FirstEyebrowsColor = c.Int(),
                        FirstLipstickColor = c.Int(),
                        FirstMakeupColor = c.Int(),
                        GlassesId = c.Int(),
                        GlassesTexture = c.Int(),
                        HairId = c.Int(),
                        HairTexture = c.Int(),
                        HairColor = c.Int(),
                        HatId = c.Int(),
                        HatTexture = c.Int(),
                        LegsId = c.Int(),
                        LegsTexture = c.Int(),
                        LipstickOpacity = c.Single(),
                        MakeupId = c.Int(),
                        MakeupOpacity = c.Single(),
                        MotherId = c.Int(),
                        SecondEyebrowsColor = c.Int(),
                        SecondLipstickColor = c.Int(),
                        SecondMakeupColor = c.Int(),
                        ShapeMix = c.Single(),
                        TopId = c.Int(),
                        TopTexture = c.Int(),
                        TorsoId = c.Int(),
                        UndershirtId = c.Int(),
                        Account_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Account", t => t.Account_Id)
                .Index(t => t.Account_Id);

            CreateTable(
                    "dbo.Building",
                    c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        EnterCharge = c.Decimal(precision: 18, scale: 2),
                        ExternalPickupPositionX = c.Single(nullable: false),
                        ExternalPickupPositionY = c.Single(nullable: false),
                        ExternalPickupPositionZ = c.Single(nullable: false),
                        InternalPickupPositionX = c.Single(nullable: false),
                        InternalPickupPositionY = c.Single(nullable: false),
                        InternalPickupPositionZ = c.Single(nullable: false),
                        MaxObjectCount = c.Short(nullable: false),
                        CurrentObjectCount = c.Short(nullable: false),
                        SpawnPossible = c.Boolean(nullable: false),
                        HasCCTV = c.Boolean(nullable: false),
                        HasSafe = c.Boolean(nullable: false),
                        InternalDimension = c.Int(nullable: false),
                        Description = c.String(unicode: false),
                        CreatorsId = c.Long(nullable: false),
                        Cost = c.Decimal(precision: 18, scale: 2),
                        Character_Id = c.Long(),
                        Group_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Character", t => t.Character_Id)
                .ForeignKey("dbo.Group", t => t.Group_Id)
                .Index(t => t.Character_Id)
                .Index(t => t.Group_Id);

            CreateTable(
                    "dbo.Group",
                    c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BossId = c.Long(nullable: false),
                        Name = c.String(unicode: false),
                        Tag = c.String(unicode: false),
                        Dotation = c.Int(nullable: false),
                        MaxPayday = c.Int(nullable: false),
                        Money = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GroupType = c.Int(nullable: false),
                        Color = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                    "dbo.Worker",
                    c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Salary = c.Int(nullable: false),
                        DutyMinutes = c.Int(nullable: false),
                        PaycheckRight = c.Boolean(nullable: false),
                        DoorsRight = c.Boolean(nullable: false),
                        RecrutationRight = c.Boolean(nullable: false),
                        ChatRight = c.Boolean(nullable: false),
                        OfferFromWarehouseRight = c.Boolean(nullable: false),
                        FirstRight = c.Boolean(),
                        SecondRight = c.Boolean(),
                        ThirdRight = c.Boolean(),
                        FourthRight = c.Boolean(),
                        FifthRight = c.Boolean(),
                        SixthRight = c.Boolean(),
                        SeventhRight = c.Boolean(),
                        EightRight = c.Boolean(),
                        Character_Id = c.Long(),
                        Group_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Character", t => t.Character_Id)
                .ForeignKey("dbo.Group", t => t.Group_Id)
                .Index(t => t.Character_Id)
                .Index(t => t.Group_Id);

            CreateTable(
                    "dbo.Item",
                    c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        CreatorId = c.Long(nullable: false),
                        Cost = c.Decimal(precision: 18, scale: 2),
                        Weight = c.Int(nullable: false),
                        ItemHash = c.Int(nullable: false),
                        FirstParameter = c.Int(),
                        SecondParameter = c.Int(),
                        ThirdParameter = c.Int(),
                        ItemType = c.Int(nullable: false),
                        CurrentlyInUse = c.Boolean(),
                        Building_Id = c.Long(),
                        Character_Id = c.Long(),
                        Vehicle_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Building", t => t.Building_Id)
                .ForeignKey("dbo.Character", t => t.Character_Id)
                .ForeignKey("dbo.Vehicle", t => t.Vehicle_Id)
                .Index(t => t.Building_Id)
                .Index(t => t.Character_Id)
                .Index(t => t.Vehicle_Id);

            CreateTable(
                    "dbo.Vehicle",
                    c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CreatorId = c.Long(nullable: false),
                        NumberPlate = c.String(unicode: false),
                        NumberPlateStyle = c.Int(nullable: false),
                        VehicleHash = c.Int(nullable: false),
                        SpawnPositionX = c.Single(nullable: false),
                        SpawnPositionY = c.Single(nullable: false),
                        SpawnPositionZ = c.Single(nullable: false),
                        SpawnRotationX = c.Single(nullable: false),
                        SpawnRotationY = c.Single(nullable: false),
                        SpawnRotationZ = c.Single(nullable: false),
                        IsSpawned = c.Boolean(nullable: false),
                        EnginePowerMultipler = c.Single(nullable: false),
                        EngineTorqueMultipler = c.Single(nullable: false),
                        Health = c.Single(nullable: false),
                        Milage = c.Single(nullable: false),
                        Fuel = c.Single(nullable: false),
                        FuelTank = c.Single(nullable: false),
                        FuelConsumption = c.Single(nullable: false),
                        Door1Damage = c.Boolean(nullable: false),
                        Door2Damage = c.Boolean(nullable: false),
                        Door3Damage = c.Boolean(nullable: false),
                        Door4Damage = c.Boolean(nullable: false),
                        Window1Damage = c.Boolean(nullable: false),
                        Window2Damage = c.Boolean(nullable: false),
                        Window3Damage = c.Boolean(nullable: false),
                        Window4Damage = c.Boolean(nullable: false),
                        PrimaryColor = c.String(unicode: false),
                        SecondaryColor = c.String(unicode: false),
                        WheelType = c.Int(nullable: false),
                        WheelColor = c.Int(nullable: false),
                        Character_Id = c.Long(),
                        Group_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Character", t => t.Character_Id)
                .ForeignKey("dbo.Group", t => t.Group_Id)
                .Index(t => t.Character_Id)
                .Index(t => t.Group_Id);

            CreateTable(
                    "dbo.Description",
                    c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Title = c.String(unicode: false),
                        Content = c.String(unicode: false),
                        Character_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Character", t => t.Character_Id)
                .Index(t => t.Character_Id);

            CreateTable(
                    "dbo.CrimeBot",
                    c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        Vehicle = c.Int(nullable: false),
                        Model = c.Int(nullable: false),
                        PistolCost = c.Decimal(precision: 18, scale: 2),
                        PistolCount = c.Int(),
                        PistolDefaultCount = c.Int(),
                        PistolMk2Cost = c.Decimal(precision: 18, scale: 2),
                        PistolMk2Count = c.Int(),
                        PistolMk2rDefaultCount = c.Int(),
                        CombatPistolCost = c.Decimal(precision: 18, scale: 2),
                        CombatPistolCount = c.Int(),
                        CombatPistolDefaultCount = c.Int(),
                        Pistol50Cost = c.Decimal(precision: 18, scale: 2),
                        Pistol50Count = c.Int(),
                        Pistol50DefaultCount = c.Int(),
                        SNSPistolCost = c.Decimal(precision: 18, scale: 2),
                        SNSPistolCount = c.Int(),
                        SNSPistolDefaultCount = c.Int(),
                        HeavyPistolCost = c.Decimal(precision: 18, scale: 2),
                        HeavyPistolCount = c.Int(),
                        HeavyPistolDefaultCount = c.Int(),
                        RevolverCost = c.Decimal(precision: 18, scale: 2),
                        RevolverCount = c.Int(),
                        RevolverDefaultCount = c.Int(),
                        MicroSMGCost = c.Decimal(precision: 18, scale: 2),
                        MicroSMGCount = c.Int(),
                        MicroSMGDefaultCount = c.Int(),
                        SMGCost = c.Decimal(precision: 18, scale: 2),
                        SMGCount = c.Int(),
                        SMGDefaultCount = c.Int(),
                        SMGMk2Cost = c.Decimal(precision: 18, scale: 2),
                        SMGMk2Count = c.Int(),
                        SMGMk2DefaultCount = c.Int(),
                        MiniSMGCost = c.Decimal(precision: 18, scale: 2),
                        MiniSMGCount = c.Int(),
                        MiniSMGDefaultCount = c.Int(),
                        AssaultRifleCost = c.Decimal(precision: 18, scale: 2),
                        AssaultRifleCount = c.Int(),
                        AssaultRifleDefaultCount = c.Int(),
                        AssaultRifleMk2Cost = c.Decimal(precision: 18, scale: 2),
                        AssaultRifleMk2Count = c.Int(),
                        AssaultRifleMk2DefaultCount = c.Int(),
                        SniperRifleCost = c.Decimal(precision: 18, scale: 2),
                        SniperRifleCount = c.Int(),
                        SniperRifleDefaultCount = c.Int(),
                        DoubleBarrelShotgunCost = c.Decimal(precision: 18, scale: 2),
                        DoubleBarrelShotgunCount = c.Int(),
                        DoubleBarrelShotgunDefaultCount = c.Int(),
                        PumpShotgunCost = c.Decimal(precision: 18, scale: 2),
                        PumpShotgunCount = c.Int(),
                        PumpShotgunDefaultCount = c.Int(),
                        SawnoffShotgunCost = c.Decimal(precision: 18, scale: 2),
                        SawnoffShotgunCount = c.Int(),
                        SawnoffShotgunDefaultCount = c.Int(),
                        PistolMagazineCost = c.Decimal(precision: 18, scale: 2),
                        PistolMagazineCount = c.Int(),
                        PistolMagazineDefaultCount = c.Int(),
                        PistolMk2MagazineCost = c.Decimal(precision: 18, scale: 2),
                        PistolMk2MagazineCount = c.Int(),
                        PistolMk2rMagazineDefaultCount = c.Int(),
                        CombatPistolMagazineCost = c.Decimal(precision: 18, scale: 2),
                        CombatPistolMagazineCount = c.Int(),
                        CombatPistolMagazineDefaultCount = c.Int(),
                        Pistol50MagazineCost = c.Decimal(precision: 18, scale: 2),
                        Pistol50MagazineCount = c.Int(),
                        Pistol50MagazineDefaultCount = c.Int(),
                        SNSPistolMagazineCost = c.Decimal(precision: 18, scale: 2),
                        SNSPistolMagazineCount = c.Int(),
                        SNSPistolMagazineDefaultCount = c.Int(),
                        HeavyPistolMagazineCost = c.Decimal(precision: 18, scale: 2),
                        HeavyPistolMagazineCount = c.Int(),
                        HeavyPistolMagazineDefaultCount = c.Int(),
                        RevolverMagazineCost = c.Decimal(precision: 18, scale: 2),
                        RevolverMagazineCount = c.Int(),
                        RevolverMagazineDefaultCount = c.Int(),
                        MicroSMGMagazineCost = c.Decimal(precision: 18, scale: 2),
                        MicroSMGMagazineCount = c.Int(),
                        MicroSMGMagazineDefaultCount = c.Int(),
                        SMGMagazineCost = c.Decimal(precision: 18, scale: 2),
                        SMGMagazineCount = c.Int(),
                        SMGMagazineDefaultCount = c.Int(),
                        SMGMk2MagazineCost = c.Decimal(precision: 18, scale: 2),
                        SMGMk2MagazineCount = c.Int(),
                        SMGMk2MagazineDefaultCount = c.Int(),
                        MiniSMGMagazineCost = c.Decimal(precision: 18, scale: 2),
                        MiniSMGMagazineCount = c.Int(),
                        MiniSMGMagazineDefaultCount = c.Int(),
                        AssaultRifleMagazineCost = c.Decimal(precision: 18, scale: 2),
                        AssaultRifleMagazineCount = c.Int(),
                        AssaultRifleMagazineDefaultCount = c.Int(),
                        AssaultRifleMk2MagazineCost = c.Decimal(precision: 18, scale: 2),
                        AssaultRifleMk2MagazineCount = c.Int(),
                        AssaultRifleMk2MagazineDefaultCount = c.Int(),
                        SniperRifleMagazineCost = c.Decimal(precision: 18, scale: 2),
                        SniperRifleMagazineCount = c.Int(),
                        SniperRifleMagazineDefaultCount = c.Int(),
                        DoubleBarrelShotgunMagazineCost = c.Decimal(precision: 18, scale: 2),
                        DoubleBarrelShotgunMagazineCount = c.Int(),
                        DoubleBarrelShotgunMagazineDefaultCount = c.Int(),
                        PumpShotgunMagazineCost = c.Decimal(precision: 18, scale: 2),
                        PumpShotgunMagazineCount = c.Int(),
                        PumpShotgunMagazineDefaultCount = c.Int(),
                        SawnoffShotgunMagazineCost = c.Decimal(precision: 18, scale: 2),
                        SawnoffShotgunMagazineCount = c.Int(),
                        SawnoffShotgunMagazineDefaultCount = c.Int(),
                        MarijuanaCost = c.Decimal(precision: 18, scale: 2),
                        MarijuanaCount = c.Int(),
                        MarijuanaDefaultCount = c.Int(),
                        LsdCost = c.Decimal(precision: 18, scale: 2),
                        LsdCount = c.Int(),
                        LsdDefaultCount = c.Int(),
                        ExcstasyCost = c.Decimal(precision: 18, scale: 2),
                        ExcstasyCount = c.Int(),
                        ExcstasyDefaultCount = c.Int(),
                        AmphetamineCost = c.Decimal(precision: 18, scale: 2),
                        AmphetamineCount = c.Int(),
                        AmphetamineDefaultCount = c.Int(),
                        MetaamphetamineCost = c.Decimal(precision: 18, scale: 2),
                        MetaamphetamineCount = c.Int(),
                        MetaamphetamineDefaultCount = c.Int(),
                        CrackCost = c.Decimal(precision: 18, scale: 2),
                        CrackCount = c.Int(),
                        CrackDefaultCount = c.Int(),
                        CocaineCost = c.Decimal(precision: 18, scale: 2),
                        CocaineCount = c.Int(),
                        CocaineDefaultCount = c.Int(),
                        HasishCost = c.Decimal(precision: 18, scale: 2),
                        HasishCount = c.Int(),
                        HasishDefaultCount = c.Int(),
                        HeroinCost = c.Decimal(precision: 18, scale: 2),
                        HeroinCount = c.Int(),
                        HeroinDefaultCount = c.Int(),
                        Group_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Group", t => t.Group_Id)
                .Index(t => t.Group_Id);

            CreateTable(
                    "dbo.TelephoneContact",
                    c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PhoneNumber = c.Int(nullable: false),
                        Name = c.String(unicode: false),
                        Number = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                    "dbo.TelephoneMessage",
                    c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PhoneNumber = c.Int(nullable: false),
                        Content = c.String(unicode: false),
                        SenderNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.CrimeBot", "Group_Id", "dbo.Group");
            DropForeignKey("dbo.Description", "Character_Id", "dbo.Character");
            DropForeignKey("dbo.Item", "Vehicle_Id", "dbo.Vehicle");
            DropForeignKey("dbo.Vehicle", "Group_Id", "dbo.Group");
            DropForeignKey("dbo.Vehicle", "Character_Id", "dbo.Character");
            DropForeignKey("dbo.Item", "Character_Id", "dbo.Character");
            DropForeignKey("dbo.Item", "Building_Id", "dbo.Building");
            DropForeignKey("dbo.Building", "Group_Id", "dbo.Group");
            DropForeignKey("dbo.Worker", "Group_Id", "dbo.Group");
            DropForeignKey("dbo.Worker", "Character_Id", "dbo.Character");
            DropForeignKey("dbo.Building", "Character_Id", "dbo.Character");
            DropForeignKey("dbo.Character", "Account_Id", "dbo.Account");
            DropForeignKey("dbo.Ban", "AccountId", "dbo.Account");
            DropIndex("dbo.CrimeBot", new[] { "Group_Id" });
            DropIndex("dbo.Description", new[] { "Character_Id" });
            DropIndex("dbo.Vehicle", new[] { "Group_Id" });
            DropIndex("dbo.Vehicle", new[] { "Character_Id" });
            DropIndex("dbo.Item", new[] { "Vehicle_Id" });
            DropIndex("dbo.Item", new[] { "Character_Id" });
            DropIndex("dbo.Item", new[] { "Building_Id" });
            DropIndex("dbo.Worker", new[] { "Group_Id" });
            DropIndex("dbo.Worker", new[] { "Character_Id" });
            DropIndex("dbo.Building", new[] { "Group_Id" });
            DropIndex("dbo.Building", new[] { "Character_Id" });
            DropIndex("dbo.Character", new[] { "Account_Id" });
            DropIndex("dbo.Ban", new[] { "AccountId" });
            DropIndex("dbo.Account", new[] { "SocialClub" });
            DropIndex("dbo.Account", new[] { "UserId" });
            DropTable("dbo.TelephoneMessage");
            DropTable("dbo.TelephoneContact");
            DropTable("dbo.CrimeBot");
            DropTable("dbo.Description");
            DropTable("dbo.Vehicle");
            DropTable("dbo.Item");
            DropTable("dbo.Worker");
            DropTable("dbo.Group");
            DropTable("dbo.Building");
            DropTable("dbo.Character");
            DropTable("dbo.Ban");
            DropTable("dbo.Account");
        }
    }
}
