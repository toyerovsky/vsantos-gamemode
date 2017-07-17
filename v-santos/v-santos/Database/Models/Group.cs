/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Serverside.Groups;
using Serverside.Groups.Enums;

namespace Serverside.Database.Models
{
    public class Group
    {
        [Key]
        public long Id { get; set; }
        /// <summary>
        /// Pole pomocnicze żeby byłe głąb nie wyrzucił szefa z jego biznesu
        /// </summary>
        public long BossId { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public int Dotation { get; set; }
        public int MaxPayday { get; set; }
        public decimal Money { get; set; }
        public GroupType GroupType { get; set; }
        public string Color { get; set; }
        public virtual ICollection<Worker> Workers { get; set; }
    }
}
