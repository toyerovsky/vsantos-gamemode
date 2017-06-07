using System;
using GTANetworkServer;

namespace Serverside.WheelMenu
{
    public class WheelMenuItem
    {
        public string Name { get; }

        private Client Sender { get; }
        private object Target { get; }
        private Action<Client, object> Action { get; }

        public WheelMenuItem(string name, Client sender, object target, Action<Client, object> action)
        {
            Name = name;
            Sender = sender;
            Target = target;
            Action = action;
        }

        public void Use()
        {
            Action.Invoke(Sender, Target);   
        }
    }
}