/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using System.ComponentModel.DataAnnotations;

namespace Serverside.Database.Models
{
    public class Ban
    {
        [Key]
        public int Id { get; set; }
        public bool Active { get; set; }

        public int CreatorId { get; set; }
        public int AccountId { get; set; }

        [StringLength(16)]
        public string Ip { get; set; }

        [StringLength(24)]
        public string SocialClub { get; set; }
        public bool IsSocialClubBanned { get; set; }

        public DateTime BanDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        [StringLength(128)]
        public string BanReason { get; set; }
    }
}