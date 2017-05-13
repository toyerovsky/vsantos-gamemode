using System.ComponentModel.DataAnnotations;

namespace Serverside.DatabaseEF6.Models
{
    public class TelephoneMessage
    {
        [Key]
        //ID wiadomości
        public long MID { get; set; }
        //Id telefonu zyli id przedmiotu telefonu
        public long TID { get; set; }
        public string Contenet { get; set; }
        public int SenderNumber { get; set; }
    }
}