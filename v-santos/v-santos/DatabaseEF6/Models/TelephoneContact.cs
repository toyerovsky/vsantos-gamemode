using System.ComponentModel.DataAnnotations;

namespace Serverside.DatabaseEF6.Models
{
    public class TelephoneContact
    {
        [Key]
        //ID kontaktu
        public long ContactId { get; set; }
        //Id telefonu czyli id przedmiotu telefonu
        [StringLength(9)]
        public long PhoneNumber { get; set; }
        public string Name { get; set; }
        [StringLength(9)]
        public int Number { get; set; }
    }
}