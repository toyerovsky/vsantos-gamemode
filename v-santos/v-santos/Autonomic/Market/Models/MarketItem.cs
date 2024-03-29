﻿/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using Serverside.Interfaces;
using Serverside.Items;

namespace Serverside.Autonomic.Market.Models
{
    [Serializable]
    public class MarketItem : IXmlObject
    {
        public string Name { get; set; }
        public ItemType ItemType { get; set; }
        public decimal Cost { get; set; } 
        public int? FirstParameter { get; set; }
        public int? SecondParameter { get; set; }
        public int? ThirdParameter { get; set; }
        public string CreatorForumName { get; set; }
        public string FilePath { get; set; }
    }
}