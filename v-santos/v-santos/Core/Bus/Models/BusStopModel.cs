using System;
using GTANetworkShared;

namespace Serverside.Core.Bus.Models
{
    [Serializable]
    public class BusStopModel
    {
        public string Name { get; set; }
        public Vector3 Center { get; set; }

        public BusStopModel() { }
    }
}