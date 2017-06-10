using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Serverside.Database.Models
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

        public decimal? EnterCharge { get; set; }

        public float ExternalPickupPositionX { get; set; }
        public float ExternalPickupPositionY { get; set; }
        public float ExternalPickupPositionZ { get; set; }

        public float InternalPickupPositionX { get; set; }
        public float InternalPickupPositionY { get; set; }
        public float InternalPickupPositionZ { get; set; }

        public short MaxObjectCount { get; set; }
        public short CurrentObjectCount { get; set; }

        public bool SpawnPossible { get; set; }
        public bool HasCCTV { get; set; }
        public bool HasSafe { get; set; }


        public int InternalDimension { get; set; }

        public string Description { get; set; }
        public long CreatorsId { get; set; }
        public decimal? Cost { get; set; }
    }
}
