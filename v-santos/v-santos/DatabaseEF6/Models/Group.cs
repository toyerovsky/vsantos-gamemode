using System.ComponentModel.DataAnnotations;

namespace Serverside.DatabaseEF6.Models
{
    public class Group
    {
        [Key]
        public long GroupId { get; set; }

        public string Name { get; set; }
        public string Tag { get; set; }

        public int Dotation { get; set; }
        public int MaxPayday { get; set; }

        public decimal Money { get; set; }

        public int GroupType { get; set; }

        //TODO Dodać do helpera
        public string Color { get; set; }
    }
}
