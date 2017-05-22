using System;
using GTANetworkServer;
using Serverside.Controllers;
using Serverside.Core;

namespace Serverside.Extensions
{
    public class DimensionChangeEventArgs : EventArgs
    {
        public DimensionChangeEventArgs(Client player, int currentdimension, int olddimension)
        {
            Player = player;
            CurrentDimension = currentdimension;
            ChatMessageType = olddimension;
        }

        public Client Player { get; }
        public int CurrentDimension { get; }
        public int ChatMessageType { get; }
    }
    public delegate void DimensionChangeEventHandler(object sender, DimensionChangeEventArgs e);

    public delegate void LoginEventHandler(AccountController sender);
}
