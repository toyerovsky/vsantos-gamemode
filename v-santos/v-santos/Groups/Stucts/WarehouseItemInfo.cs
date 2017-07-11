using Serverside.Database.Models;

namespace Serverside.Groups.Stucts
{
    public struct WarehouseItemInfo
    {
        public GroupWarehouseItem ItemInfo { get; set; }
        public int Count { get; set; }
    }
}