using System.ComponentModel.DataAnnotations;

namespace Serverside.DatabaseEF6.Models
{
    public class TelephoneContact
    {
        [Key]
        //ID kontaktu
        public long COID { get; set; }
        //Id telefonu czyli id przedmiotu telefonu
        public long TID { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
    }
}