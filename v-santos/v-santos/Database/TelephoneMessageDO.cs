namespace Serverside.Database
{
    public class TelephoneMessageList
    {
        //ID wiadomości
        public long MID { get; set; }
        //Id telefonu zyli id przedmiotu telefonu
        public long TID { get; set; }
        public string Contenet { get; set; }
        public int SenderNumber { get; set; }
    }

    public class TelephoneMessageEditor
    {
        //ID wiadomości
        public long MID { get; set; }
        //Id telefonu zyli id przedmiotu telefonu
        public long TID { get; set; }
        public string Contenet { get; set; }
        public int SenderNumber { get; set; }
    }
}