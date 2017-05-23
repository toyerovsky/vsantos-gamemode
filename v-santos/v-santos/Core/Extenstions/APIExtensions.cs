using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;

namespace Serverside.Core.Extensions
{
    public static class APIExtensions
    {
        public static List<Client> GetNearestPlayers(this Vector3 position)
        {
            //Stopwatch _debug = new Stopwatch();
            //_debug.Start();
            return API.shared.getAllPlayers().Select(n => n).OrderBy(n => new Vector3(n.position.X, n.position.Y, n.position.Z).DistanceTo(position)).ToList();
            //API.shared.sendNotificationToPlayer(player, _debug.ElapsedTicks.ToString(), true); // DEBUG
            //_debug.Reset();
        }

        public static Client GetNearestPlayer(this Vector3 position)
        {
            return position.GetNearestPlayers()[0];
        }

        public static void ConsoleOutput(string message, ConsoleColor color)
        {
            Console.BackgroundColor = color;
            API.shared.consoleOutput(message);
            Console.ResetColor();
        }
    }
}
