/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using Serverside.Controllers;
using Serverside.Database.Models;

namespace Serverside.Groups.Stucts
{
    public struct WarehouseOrderInfo
    {
        public AccountController CurrentCourier { get; set; }
        public GroupWarehouseOrder Data { get; set; }
    }
}