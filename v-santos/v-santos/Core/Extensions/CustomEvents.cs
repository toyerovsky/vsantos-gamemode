/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using GrandTheftMultiplayer.Server.Elements;
using Serverside.Controllers;

namespace Serverside.Core.Extensions
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

    public delegate void AccountLoginEventHandler(Client sender, AccountController account);
    public delegate void CharacterLoginEventHandler(Client sender, CharacterController account);
}
