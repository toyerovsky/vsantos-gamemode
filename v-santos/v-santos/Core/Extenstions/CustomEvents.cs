using GTANetworkServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serverside.Core.Extenstions
{
    public class SaidEventArgs : EventArgs
    {
        public SaidEventArgs(Client player, string message, ChatMessageType chatMessageType)
        {
            Player = player;
            Message = message;
            ChatMessageType = chatMessageType;
        }

        public Client Player { get; }
        public string Message { get; }
        public ChatMessageType ChatMessageType { get; }
    }
    public delegate void SaidEventHandler(object sender, SaidEventArgs e);

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

    public class OnPlayerLoginEventArgs : EventArgs
    {
        public OnPlayerLoginEventArgs(Client player)
        {
            Player = player;
        }

        public Client Player { get; }
    }
    public delegate void OnPlayerLoginEventHandler(object sender, OnPlayerLoginEventArgs e);

    public class OnCharacterNotCreatedEventArgs : EventArgs
    {
        public OnCharacterNotCreatedEventArgs(Client player)
        {
            Player = player;
        }

        public Client Player { get; }
    }
    public delegate void OnCharacterNotCreatedEventHandler(object sender, OnCharacterNotCreatedEventArgs e);
}
