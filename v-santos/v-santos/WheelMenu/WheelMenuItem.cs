﻿/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using GrandTheftMultiplayer.Server.Elements;

namespace Serverside.WheelMenu
{
    public class WheelMenuItem
    {
        public string Name { get; }
        private Client Sender { get; }
        private object Target { get; }
        private Action<Client, object> WheelAction { get; }

        public WheelMenuItem(string name, Client sender, object target, Action<Client, object> wheelAction)
        {
            Name = name;
            Sender = sender;
            Target = target;
            WheelAction = wheelAction;

        }

        public void Use()
        {
            WheelAction.Invoke(Sender, Target);   
        }
    }
}