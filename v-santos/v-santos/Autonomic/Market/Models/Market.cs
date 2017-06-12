using System;
using System.Collections.Generic;
using GTANetworkShared;

namespace Serverside.Autonomic.Market.Models
{
    [Serializable]
    public class Market
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Vector3 StartPosition { get; set; }
        public Vector3 EndPosition { get; set; }
    }
}