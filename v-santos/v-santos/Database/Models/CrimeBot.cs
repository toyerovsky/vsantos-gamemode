using System.ComponentModel.DataAnnotations;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Shared;


namespace Serverside.Database.Models
{
    public class CrimeBot
    {
        [Key]
        public long Id { get; set; }
        public Group Group { get; set; }
        public string Name { get; set; }
        public VehicleHash Vehicle { get; set; }
        public PedHash Model { get; set; }

        public decimal? PistolCost { get; set; }
        public int? PistolCount { get; set; }
        public int? PistolDefaultCount { get; set; }

        public decimal? PistolMk2Cost { get; set; }
        public int? PistolMk2Count { get; set; }
        public int? PistolMk2rDefaultCount { get; set; }

        public decimal? CombatPistolCost { get; set; }
        public int? CombatPistolCount { get; set; }
        public int? CombatPistolDefaultCount { get; set; }

        public decimal? Pistol50Cost { get; set; }
        public int? Pistol50Count { get; set; }
        public int? Pistol50DefaultCount { get; set; }

        public decimal? SNSPistolCost { get; set; }
        public int? SNSPistolCount { get; set; }
        public int? SNSPistolDefaultCount { get; set; }

        public decimal? HeavyPistolCost { get; set; }
        public int? HeavyPistolCount { get; set; }
        public int? HeavyPistolDefaultCount { get; set; }

        public decimal? RevolverCost { get; set; }
        public int? RevolverCount { get; set; }
        public int? RevolverDefaultCount { get; set; }

        public decimal? MicroSMGCost { get; set; }
        public int? MicroSMGCount { get; set; }
        public int? MicroSMGDefaultCount { get; set; }

        public decimal? SMGCost { get; set; }
        public int? SMGCount { get; set; }
        public int? SMGDefaultCount { get; set; }

        public decimal? SMGMk2Cost { get; set; }
        public int? SMGMk2Count { get; set; }
        public int? SMGMk2DefaultCount { get; set; }

        public decimal? MiniSMGCost { get; set; }
        public int? MiniSMGCount { get; set; }
        public int? MiniSMGDefaultCount { get; set; }

        public decimal? AssaultRifleCost { get; set; }
        public int? AssaultRifleCount { get; set; }
        public int? AssaultRifleDefaultCount { get; set; }

        public decimal? AssaultRifleMk2Cost { get; set; }
        public int? AssaultRifleMk2Count { get; set; }
        public int? AssaultRifleMk2DefaultCount { get; set; }

        public decimal? SniperRifleCost { get; set; }
        public int? SniperRifleCount { get; set; }
        public int? SniperRifleDefaultCount { get; set; }

        public decimal? DoubleBarrelShotgunCost { get; set; }
        public int? DoubleBarrelShotgunCount { get; set; }
        public int? DoubleBarrelShotgunDefaultCount { get; set; }

        public decimal? PumpShotgunCost { get; set; }
        public int? PumpShotgunCount { get; set; }
        public int? PumpShotgunDefaultCount { get; set; }

        public decimal? SawnoffShotgunCost { get; set; }
        public int? SawnoffShotgunCount { get; set; }
        public int? SawnoffShotgunDefaultCount { get; set; }

        public decimal? PistolMagazineCost { get; set; }
        public int? PistolMagazineCount { get; set; }
        public int? PistolMagazineDefaultCount { get; set; }

        public decimal? PistolMk2MagazineCost { get; set; }
        public int? PistolMk2MagazineCount { get; set; }
        public int? PistolMk2rMagazineDefaultCount { get; set; }

        public decimal? CombatPistolMagazineCost { get; set; }
        public int? CombatPistolMagazineCount { get; set; }
        public int? CombatPistolMagazineDefaultCount { get; set; }

        public decimal? Pistol50MagazineCost { get; set; }
        public int? Pistol50MagazineCount { get; set; }
        public int? Pistol50MagazineDefaultCount { get; set; }

        public decimal? SNSPistolMagazineCost { get; set; }
        public int? SNSPistolMagazineCount { get; set; }
        public int? SNSPistolMagazineDefaultCount { get; set; }

        public decimal? HeavyPistolMagazineCost { get; set; }
        public int? HeavyPistolMagazineCount { get; set; }
        public int? HeavyPistolMagazineDefaultCount { get; set; }

        public decimal? RevolverMagazineCost { get; set; }
        public int? RevolverMagazineCount { get; set; }
        public int? RevolverMagazineDefaultCount { get; set; }

        public decimal? MicroSMGMagazineCost { get; set; }
        public int? MicroSMGMagazineCount { get; set; }
        public int? MicroSMGMagazineDefaultCount { get; set; }

        public decimal? SMGMagazineCost { get; set; }
        public int? SMGMagazineCount { get; set; }
        public int? SMGMagazineDefaultCount { get; set; }

        public decimal? SMGMk2MagazineCost { get; set; }
        public int? SMGMk2MagazineCount { get; set; }
        public int? SMGMk2MagazineDefaultCount { get; set; }

        public decimal? MiniSMGMagazineCost { get; set; }
        public int? MiniSMGMagazineCount { get; set; }
        public int? MiniSMGMagazineDefaultCount { get; set; }

        public decimal? AssaultRifleMagazineCost { get; set; }
        public int? AssaultRifleMagazineCount { get; set; }
        public int? AssaultRifleMagazineDefaultCount { get; set; }

        public decimal? AssaultRifleMk2MagazineCost { get; set; }
        public int? AssaultRifleMk2MagazineCount { get; set; }
        public int? AssaultRifleMk2MagazineDefaultCount { get; set; }

        public decimal? SniperRifleMagazineCost { get; set; }
        public int? SniperRifleMagazineCount { get; set; }
        public int? SniperRifleMagazineDefaultCount { get; set; }

        public decimal? DoubleBarrelShotgunMagazineCost { get; set; }
        public int? DoubleBarrelShotgunMagazineCount { get; set; }
        public int? DoubleBarrelShotgunMagazineDefaultCount { get; set; }

        public decimal? PumpShotgunMagazineCost { get; set; }
        public int? PumpShotgunMagazineCount { get; set; }
        public int? PumpShotgunMagazineDefaultCount { get; set; }

        public decimal? SawnoffShotgunMagazineCost { get; set; }
        public int? SawnoffShotgunMagazineCount { get; set; }
        public int? SawnoffShotgunMagazineDefaultCount { get; set; }

        public decimal? MarijuanaCost { get; set; }
        public int? MarijuanaCount { get; set; }
        public int? MarijuanaDefaultCount { get; set; }

        public decimal? LsdCost { get; set; }
        public int? LsdCount { get; set; }
        public int? LsdDefaultCount { get; set; }

        public decimal? ExcstasyCost { get; set; }
        public int? ExcstasyCount { get; set; }
        public int? ExcstasyDefaultCount { get; set; }

        public decimal? AmphetamineCost { get; set; }
        public int? AmphetamineCount { get; set; }
        public int? AmphetamineDefaultCount { get; set; }

        public decimal? MetaamphetamineCost { get; set; }
        public int? MetaamphetamineCount { get; set; }
        public int? MetaamphetamineDefaultCount { get; set; }

        public decimal? CrackCost { get; set; }
        public int? CrackCount { get; set; }
        public int? CrackDefaultCount { get; set; }

        public decimal? CocaineCost { get; set; }
        public int? CocaineCount { get; set; }
        public int? CocaineDefaultCount { get; set; }

        public decimal? HasishCost { get; set; }
        public int? HasishCount { get; set; }
        public int? HasishDefaultCount { get; set; }

        public decimal? HeroinCost { get; set; }
        public int? HeroinCount { get; set; }
        public int? HeroinDefaultCount { get; set; }
    }
}