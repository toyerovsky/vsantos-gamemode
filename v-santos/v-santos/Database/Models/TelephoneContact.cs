using System.ComponentModel.DataAnnotations;

namespace Serverside.Database.Models
{
    public class TelephoneContact
    {
        [Key]
        //ID kontaktu
        public long Id { get; set; }
        //Id telefonu czyli id przedmiotu telefonu
        public int PhoneNumber { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
    }
}