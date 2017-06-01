using System.ComponentModel.DataAnnotations;

namespace Serverside.Database.Models
{
    public class TelephoneMessage
    {
        [Key]
        //ID wiadomości
        public long Id { get; set; }
        //Id telefonu czyli id przedmiotu telefonu
        public int PhoneNumber { get; set; }
        public string Content { get; set; }
        public int SenderNumber { get; set; }
    }
}