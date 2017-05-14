using System.ComponentModel.DataAnnotations;

namespace Serverside.DatabaseEF6.Models
{
    public class TelephoneMessage
    {
        [Key]
        //ID wiadomości
        public long MessageId { get; set; }
        //Id telefonu zyli id przedmiotu telefonu
        [StringLength(9)]
        public long PhoneNumber { get; set; }
        public string Content { get; set; }
        [StringLength(9)]
        public int SenderNumber { get; set; }
    }
}