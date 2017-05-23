using GTANetworkShared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Serverside.DatabaseEF6.Models
{
    public class Building
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }

        //public int OwnerType { get; set; } // lepiej sprawdzać czy characterid lub groupid == null
        public Character Character { get; set; }
        public Group Group { get; set; }

        public ICollection<Item> Item { get; set; }

        public decimal EnterCharge { get; set; }

        //public Vector3 ExternalPickupPosition { get; set; }
        public float ExternalPickupPositionX { get; set; }
        public float ExternalPickupPositionY { get; set; }
        public float ExternalPickupPositionZ { get; set; }

        //public Vector3 InternalPickupPosition { get; set; }
        public float InternalPickupPositionX { get; set; }
        public float InternalPickupPositionY { get; set; }
        public float InternalPickupPositionZ { get; set; }

        public short MaxObjectCount { get; set; }
        public short CurrentObjectCount { get; set; }

        public bool SpawnPossible { get; set; }
        public bool HasCCTV { get; set; }
        public bool HasSafe { get; set; }

        public int InternalDismension { get; set; }

        public string Description { get; set; }
        //Zamysł: Przy tworzeniu przedmiotu będzie zapisywane kto go stworzył, a jak stworzy go serwer to 0
        public long CreatorsUID { get; set; }
    }
}
