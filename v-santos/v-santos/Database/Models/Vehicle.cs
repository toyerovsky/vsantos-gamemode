using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GrandTheftMultiplayer.Shared;


namespace Serverside.Database.Models
{
    public class Vehicle
    {
        [Key]
        public long Id { get; set; }
        public Character Character { get; set; }
        public Group Group { get; set; }
        //Zamysł: Przy tworzeniu przedmiotu będzie zapisywane kto go stworzył, a jak stworzy go serwer to 0
        public long CreatorId { get; set; }

        //public string Name { get; set; }
        public string NumberPlate { get; set; }

        public int NumberPlateStyle { get; set; }
        public virtual VehicleHash VehicleHash { get; set; }

        //public Vector3 SpawnPosition { get; set; }
        public float SpawnPositionX { get; set; }
        public float SpawnPositionY { get; set; }
        public float SpawnPositionZ { get; set; }

        //public Vector3 RotationPosition { get; set; }
        public float SpawnRotationX { get; set; }
        public float SpawnRotationY { get; set; }
        public float SpawnRotationZ { get; set; }

        public bool IsSpawned { get; set; }
        public float EnginePowerMultipler { get; set; }
        public float EngineTorqueMultipler { get; set; }
        public float Health { get; set; }
        public float Milage { get; set; }
        public float Fuel { get; set; }
        public float FuelTank { get; set; }
        public float FuelConsumption { get; set; }
        public bool Door1Damage { get; set; }
        public bool Door2Damage { get; set; }
        public bool Door3Damage { get; set; }
        public bool Door4Damage { get; set; }
        public bool Window1Damage { get; set; }
        public bool Window2Damage { get; set; }
        public bool Window3Damage { get; set; }
        public bool Window4Damage { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        public int WheelType { get; set; }
        public int WheelColor { get; set; }

        public virtual ICollection<Item> Tunings { get; set; }
    }
}
