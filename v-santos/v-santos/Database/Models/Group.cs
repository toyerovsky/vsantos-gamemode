using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Serverside.Groups;
using Serverside.Groups.Base;

namespace Serverside.Database.Models
{
    public class Group
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }
        public string Tag { get; set; }

        public int Dotation { get; set; }
        public int MaxPayday { get; set; }

        public decimal Money { get; set; }

        public GroupType GroupType { get; set; }

        //TODO Dodać do helpera
        public string Color { get; set; }

        public virtual ICollection<Worker> Workers { get; set; }
    }
}
