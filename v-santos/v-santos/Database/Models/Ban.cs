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