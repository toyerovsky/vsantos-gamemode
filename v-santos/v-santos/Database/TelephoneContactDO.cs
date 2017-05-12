namespace Serverside.Database
{
    public class TelephoneContactList
    {
        //Id kontaktu
        public long COID { get; set; }
        //Id telefonu czyli id przedmiotu telefonu
        public long TID { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
    }

    public class TelephoneContactEditor
    {
        //ID kontaktu
        public long COID { get; set; }
        //Id telefonu czyli id przedmiotu telefonu
        public long TID { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
    }
}