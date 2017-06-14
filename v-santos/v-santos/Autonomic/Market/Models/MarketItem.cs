using System;
using Serverside.Items;

namespace Serverside.Autonomic.Market.Models
{
    [Serializable]
    public class MarketItem
    {
        public string Name { get; set; }
        public ItemType ItemType { get; set; }
        public decimal Cost { get; set; } 
        public int FirstParameter { get; set; }
        public int SecondParameter { get; set; }
        public int ThirdParameter { get; set; }
    }
}