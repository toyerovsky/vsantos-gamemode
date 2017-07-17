/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using Serverside.Database.Models;

namespace Serverside.Groups.Stucts
{
    public struct WarehouseItemInfo
    {
        public GroupWarehouseItem ItemInfo { get; set; }
        public int Count { get; set; }
    }
}