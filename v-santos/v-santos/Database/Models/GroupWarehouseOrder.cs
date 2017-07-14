using System.ComponentModel.DataAnnotations;

namespace Serverside.Database.Models
{
    public class GroupWarehouseOrder
    {
        [Key]
        public long Id { get; set; }
        public Group Getter { get; set; }
        public string OrderItemsJson { get; set; }
        public string ShipmentLog { get; set; }
    }
}