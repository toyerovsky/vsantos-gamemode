using System;
using GrandTheftMultiplayer.Shared.Math;

namespace Serverside.Jobs.Courier.CourierWarehouse.Models
{
    [Serializable]
    public class CourierWarehouseModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public int BlipId { get; set; } 
    }
}
