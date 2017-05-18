using GTANetworkShared;
using System.ComponentModel.DataAnnotations;

namespace Serverside.DatabaseEF6.Models
{
    public class Vehicle
    {
        [Key]
        public long Id { get; set; }
        public Character Character { get; set; }
        public Group Group { get; set; }
        //Zamysł: Przy tworzeniu przedmiotu będzie zapisywane kto go stworzył, a jak stworzy go serwer to 0
        public long CreatorId { get; set; }

        /// Typy właścicieli: 1 gracz, 2 grupa
        //public int OwnerType { get; set; } // lepiej sprawdzać czy characterid lub groupid == null

        public string Name { get; set; }
        public string Registration { get; set; }

        public virtual VehicleHash VehicleHash { get; set; }

        //public Vector3 SpawnPosition { get; set; }
        public float SpawnPositionX { get; set; }
        public float SpawnPositionY { get; set; }
        public float SpawnPositionZ { get; set; }

        //public Vector3 RotationPosition { get; set; }
        public float RotationPositionX { get; set; }
        public float RotationPositionY { get; set; }
        public float RotationPositionZ { get; set; }

        public bool IsSpawned { get; set; }
        public double EngineMultipler { get; set; }
        public double Health { get; set; }
        public bool Door1Damage { get; set; }
        public bool Door2Damage { get; set; }
        public bool Door3Damage { get; set; }
        public bool Door4Damage { get; set; }
        public bool Window1Damage { get; set; }
        public bool Window2Damage { get; set; }
        public bool Window3Damage { get; set; }
        public bool Window4Damage { get; set; }
        public int PrimaryColor { get; set; }
        public int SecondaryColor { get; set; }
        public int WheelType { get; set; }
        public int WheelColor { get; set; }
    }
}
