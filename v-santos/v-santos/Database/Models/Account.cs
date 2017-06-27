using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Serverside.Admin;

namespace Serverside.Database.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [Index("IX_UserId", IsUnique = true)]
        public long UserId { get; set; }
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; }
        public long ForumGroup { get; set; }
        public string OtherForumGroups { get; set; }
        [Index("IX_SocialClub", IsUnique = false)]
        [StringLength(50)]
        public string SocialClub { get; set; }  
        [StringLength(16)]
        public string Ip { get; set; }
        public bool Online { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime LastLogin { get; set; }
        public ServerRank ServerRank { get; set; }
        public virtual ICollection<Character> Characters { get; set; }
        public virtual ICollection<Ban> Bans { get; set; }
    }
}
