using System.ComponentModel.DataAnnotations;

namespace Serverside.Database.Models
{
    public class Description
    {
        [Key]
        public long Id { get; set; }
        public Character Character { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}