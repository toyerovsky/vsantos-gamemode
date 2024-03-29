﻿/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using GrandTheftMultiplayer.Server.Constant;
using Serverside.Interfaces;
using Serverside.Items;

namespace Serverside.Corners.Models
{
    [Serializable]
    public class CornerBotModel : IXmlObject
    {
        public int BotId { get; set; }
        public string Name { get; set; }
        public PedHash PedHash { get; set; }
        public DrugType DrugType { get; set; }
        public decimal MoneyCount { get; set; }
        public string Greeting { get; set; }
        public string GoodFarewell { get; set; }
        public string BadFarewell { get; set; }
        public string CreatorForumName { get; set; }
        public string FilePath { get; set; }
    }
}