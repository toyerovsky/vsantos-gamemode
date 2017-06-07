﻿using System;
using GTANetworkServer;
using Serverside.Items;

namespace Serverside.Corners.Models
{
    [Serializable]
    public class CornerBotModel
    {
        public int BotId { get; set; }
        public string Name { get; set; }
        public PedHash PedHash { get; set; }
        public DrugType DrugType { get; set; }
        public decimal MoneyCount { get; set; }
        public string Greeting { get; set; }
        public string GoodFarewell { get; set; }
        public string BadFarewell { get; set; }
    }
}