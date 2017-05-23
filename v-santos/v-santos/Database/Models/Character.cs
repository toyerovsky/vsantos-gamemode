using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public DateTime? CreateAccountTime { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public DateTime? TodayPlayedTime { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }
        public int Model { get; set; }
        public string ModelName { get; set; }

        public virtual ICollection<Vehicle> Vehicle { get; set; }
        public virtual ICollection<Item> Item { get; set; }
        public virtual ICollection<Building> Building { get; set; }

        public List<Group> Group { get; set; }

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

        //public long? FirstGID { get; set; }
        //public long? SecondGID { get; set; }
        //public long? ThirdGID { get; set; }

        public bool HasIDCard { get; set; }
        public bool HasDrivingLicense { get; set; }

        public int PhoneType { get; set; }
        [Index("IX_PhoneNumber", IsUnique = true)]
        public int PhoneNumber { get; set; }

        //Pomysł zezwolenie na bron jako enum, zeby byly różne typy licencji

        public string ForumDescription { get; set; }
        public string Story { get; set; }

        public bool IsAlive { get; set; }

        public int HitPoints { get; set; }

        //public Vector3 LastPosition { get; set; }
        public float LastPositionX { get; set; }
        public float LastPositionY { get; set; }
        public float LastPositionZ { get; set; }
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
        public virtual ICollection<Worker> Job { get; set; }
        public decimal? MoneyJob { get; set; }
        public decimal? JobLimit { get; set; }
    }
}
