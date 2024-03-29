﻿/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using Serverside.Items;

namespace Serverside.CrimeBot.Models
{
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