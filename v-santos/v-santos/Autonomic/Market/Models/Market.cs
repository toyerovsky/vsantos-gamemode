using System;
using System.Collections.Generic;
using GrandTheftMultiplayer.Shared.Math;
using Serverside.Interfaces;

namespace Serverside.Autonomic.Market.Models
{
    [Serializable]
    public class Market : IXmlObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Vector3 Center { get; set; }
        public float Radius { get; set; }
        public List<MarketItem> Items { get; set; }
        public string CreatorForumName { get; set; }
        public string FilePath { get; set; }
    }
}