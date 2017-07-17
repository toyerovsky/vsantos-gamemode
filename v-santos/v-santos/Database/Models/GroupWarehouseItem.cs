/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System.ComponentModel.DataAnnotations;
using Serverside.Groups;
using Serverside.Groups.Enums;

namespace Serverside.Database.Models
{
    //Tabela do trzymania rpzedmiotów bazowych w magazynie
    public class GroupWarehouseItem
    {
        [Key]
        public long Id { get; set; }
        public Item Item { get; set; }
        public decimal Cost { get; set; }
        public decimal? MinimalCost { get; set; }
        //Ile jest obecnie?
        public int Count { get; set; }
        //Ile nadaje się co tydzień?
        public int ResetCount { get; set; }
        public GroupType GroupType { get; set; }
    }
}