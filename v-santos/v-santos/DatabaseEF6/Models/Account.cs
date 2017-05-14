﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Serverside.DatabaseEF6.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; }
        [Index("IX_SocialClub", IsUnique = true)]
        [StringLength(50)]
        public string SocialClub { get; set; }
        [StringLength(16)]
        public string Ip { get; set; }
        public bool Online { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime LastLogin { get; set; }
        public virtual ICollection<Character> Character { get; set; }
        //public virtual ICollection<Ban> Ban { get; set; }
    }
}