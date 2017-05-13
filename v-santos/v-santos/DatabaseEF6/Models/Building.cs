using GTANetworkShared;
using System.ComponentModel.DataAnnotations;

namespace Serverside.DatabaseEF6.Models
{
    public class Building
    {
        [Key]
        public long BID { get; set; }
        public string Name { get; set; }

        public int OwnerType { get; set; }
        public long OwnerUID { get; set; }

        public decimal EnterCharge { get; set; }

        public Vector3 ExternalPickupPosition { get; set; }
        public Vector3 InternalPickupPosition { get; set; }

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
