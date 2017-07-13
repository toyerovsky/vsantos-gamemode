using System.ComponentModel.DataAnnotations;

namespace Serverside.Database.Models
{
    public class GroupWarehouseOrder
    {
        [Key]
        public long Id { get; set; }
        public Group Getter { get; set; }
        public string OrderJson { get; set; }
        public string ShipmentLog { get; set; }
    }
}