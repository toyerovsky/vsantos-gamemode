using System.ComponentModel.DataAnnotations;

namespace Serverside.DatabaseEF6.Models
{
    public class Description
    {
        [Key]
        public long Id { get; set; }
        public Character Character { get; set; }
        public string Title { get; set; }
        public string Descriptionn { get; set; }
    }
}