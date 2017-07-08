

using System.Collections.Generic;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Shared.Math;

namespace Serverside.Autonomic.FuelStation
{
    public class FuelStation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public float PriceMultipler { get; set; }
        public Color Color { get; set; }
        public Vector3 MarkerPostition { get; set; }
        public List<FuelDistributor> Distributors { get; set; }
        public class FuelDistributor
        {
            public bool Occupied { get; set; }
            public Vector3 MarkerPostition { get; set; }
        }
    }
}
