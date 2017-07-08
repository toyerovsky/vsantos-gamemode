namespace Serverside.Controllers.EventArgs
{
    public class ServerIdChangeEventArgs : System.EventArgs
    {
        public int LastId { get; set; }
        public int NewId { get; set; }

        public bool Cancel { get; set; }

        public ServerIdChangeEventArgs(int lastId, int newId)
        {
            LastId = lastId;
            NewId = newId;
        }
    }
}