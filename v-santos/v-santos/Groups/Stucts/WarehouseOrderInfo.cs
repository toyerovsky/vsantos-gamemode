using Serverside.Controllers;
using Serverside.Database.Models;

namespace Serverside.Groups.Stucts
{
    public struct WarehouseOrderInfo
    {
        public AccountController CurrentCourier { get; set; }
        public GroupWarehouseOrder Data { get; set; }
    }
}