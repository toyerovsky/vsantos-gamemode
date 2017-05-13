using System.ComponentModel.DataAnnotations;

namespace Serverside.DatabaseEF6.Models
{
    public class Description
    {
        [Key]
        public long DID { get; set; }
        public long CID { get; set; }
        public string Title { get; set; }
        public string Descriptionn { get; set; }
    }
}