using System;
using GTANetworkShared;
using Serverside.Core.Extenstions;

namespace Serverside.Autonomic.Carshop.Models
{
    [Serializable]
    public class CarshopVehicle
    {
        public string Name { get; set; }
        public VehicleHash Hash { get; set; }
        public VehicleClass Category { get; set; }
        public decimal Cost { get; set; }

        public CarshopVehicle(string name, VehicleHash hash, VehicleClass category, decimal cost)
        {
            Name = name;
            Hash = hash;
            Category = category;
            Cost = cost;
        }

        public CarshopVehicle() { }
    }
}