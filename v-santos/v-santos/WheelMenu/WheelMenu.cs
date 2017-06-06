using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using Newtonsoft.Json;
using Serverside.Core.Extensions;

namespace Serverside.Core.WheelMenu
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