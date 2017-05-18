using System.ComponentModel.DataAnnotations;

namespace Serverside.DatabaseEF6.Models
{
    public class TelephoneMessage
    {
        [Key]
        //ID wiadomości
        public long Id { get; set; }
        //Id telefonu zyli id przedmiotu telefonu
        public int PhoneNumber { get; set; }
        public string Content { get; set; }
        public int SenderNumber { get; set; }
    }
}