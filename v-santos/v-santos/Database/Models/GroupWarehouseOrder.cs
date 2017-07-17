/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System.ComponentModel.DataAnnotations;

namespace Serverside.Database.Models
{
    public class GroupWarehouseOrder
    {
        [Key]
        public long Id { get; set; }
        public Group Getter { get; set; }
        public string OrderItemsJson { get; set; }
        public string ShipmentLog { get; set; }
    }
}