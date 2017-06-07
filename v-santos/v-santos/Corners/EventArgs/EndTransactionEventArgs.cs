namespace Serverside.Corners.EventArgs
{
    public class EndTransactionEventArgs : System.EventArgs
    {
        public bool Good { get; set; }

        public EndTransactionEventArgs(bool good)
        {
            Good = good;
        }
    }
}
