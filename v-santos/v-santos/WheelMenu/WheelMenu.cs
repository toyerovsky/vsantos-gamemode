/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.Elements;
using Newtonsoft.Json;
using Serverside.Core.Extensions;

namespace Serverside.WheelMenu
{
    public class WheelMenu : IDisposable
    {
        public List<WheelMenuItem> WheelMenuItems { get; }
        private Client Sender { get; }

        public WheelMenu(List<WheelMenuItem> wheelMenuItems, Client sender)
        {
            WheelMenuItems = wheelMenuItems;
            Sender = sender;
            Show();
        }

        private void Show()
        {
            Sender.triggerEvent("ShowWheelMenu", JsonConvert.SerializeObject(WheelMenuItems.Select(a => new
            {
                a.Name               
            })));
        }

        public void Dispose()
        {
            Sender.ResetData("WheelMenu");
        }
    }
}