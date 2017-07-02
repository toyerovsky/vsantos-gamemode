using System;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Items;

namespace Serverside.CrimeBot.Models
{
    [Serializable]
    public class CrimeBotItem
    {
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public int Count { get; set; }
        public int DefaultCount { get; set; }
        public ItemType Type { get; set; }
        public string DatabaseField { get; set; }
        
        public CrimeBotItem(string name, decimal cost, int count, int defaultCount, ItemType type, string databaseField)
        {
            Name = name;
            Cost = cost;
            Count = count;
            DefaultCount = defaultCount;
            Type = type;
            DatabaseField = databaseField;
        }
    }
}