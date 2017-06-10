using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Serverside.Database.Models
{
    public class Character
    {
        public Character()
        {
            Vehicle = new List<Vehicle>();
            Item = new List<Item>();
            Building = new List<Building>();
        }

        [Key]
        public long Id { get; set; }
        public Account Account { get; set; }

        public bool Online { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public DateTime? TodayPlayedTime { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }
        public int Model { get; set; }
        public string ModelName { get; set; }

        public virtual ICollection<Vehicle> Vehicle { get; set; }
        public virtual ICollection<Item> Item { get; set; }
        public virtual ICollection<Building> Building { get; set; }
        public virtual ICollection<Description> Descriptions { get; set; }
        public virtual ICollection<Worker> Worker { get; set; }

        public decimal Money { get; set; }
        public long BankAccountNumber { get; set; }
        public decimal BankMoney { get; set; }

        //Tak wiem, że to niepoprawne politycznie
        public bool Gender { get; set; }

        public short Weight { get; set; }
        public DateTime BornDate { get; set; }
        public short Height { get; set; }

        public short Force { get; set; }
        public short RunningEfficiency { get; set; }
        public short DivingEfficiency { get; set; }

        public bool HasIDCard { get; set; }
        public bool HasDrivingLicense { get; set; }

        //Pomysł zezwolenie na bron jako enum, zeby byly różne typy licencji

        public string ForumDescription { get; set; }
        public string Story { get; set; }

        public bool IsAlive { get; set; }

        public int HitPoints { get; set; }

        //public Vector3 LastPosition { get; set; }
        public float LastPositionX { get; set; }
        public float LastPositionY { get; set; }
        public float LastPositionZ { get; set; }
        public float LastPositionRotX { get; set; }
        public float LastPositionRotY { get; set; }
        public float LastPositionRotZ { get; set; }

        // public Vector3 SpawnPosition { get; set; }
        public int CurrentDimension { get; set; }

        public int BWState { get; set; }

        //TODO Dodać do helpera
        public bool? IsCreated { get; set; }
        //Pole determinuje czy gracz będzie grał w gotowym skinie, czy może sam sobie zrobi
        public bool Freemode { get; set; }
        public int? Skin { get; set; }

        //TODO Dodać wszystkie pola na możliwości ubrania
        public int Job { get; set; }
        public decimal? MoneyJob { get; set; }
        public decimal? JobLimit { get; set; }

        //Kreator postaci
        public int? AccessoryId { get; set; }
        public int? AccessoryTexture { get; set; }
        public int? EarsId { get; set; }
        public int? EarsTexture { get; set; }
        public int? EyebrowsId { get; set; }
        public float? EyeBrowsOpacity { get; set; }
        public int? FatherId { get; set; }
        public int? ShoesId { get; set; }
        public int? ShoesTexture { get; set; }
        public int? FirstEyebrowsColor { get; set; }
        public int? FirstLipstickColor { get; set; }
        public int? FirstMakeupColor { get; set; }
        public int? GlassesId { get; set; }
        public int? GlassesTexture { get; set; }
        public int? HairId { get; set; }
        public int? HairTexture { get; set; }
        public int? HairColor { get; set; }
        public int? HatId { get; set; }
        public int? HatTexture { get; set; }
        public int? LegsId { get; set; }
        public int? LegsTexture { get; set; }
        public float? LipstickOpacity { get; set; }
        public int? MakeupId { get; set; }
        public float? MakeupOpacity { get; set; }
        public int? MotherId { get; set; }
        public int? SecondEyebrowsColor { get; set; }
        public int? SecondLipstickColor { get; set; }
        public int? SecondMakeupColor { get; set; }
        public float? ShapeMix { get; set; }
        public int? TopId { get; set; }
        public int? TopTexture { get; set; }
        public int? TorsoId { get; set; }
        public int? UndershirtId { get; set; }
    }
}
