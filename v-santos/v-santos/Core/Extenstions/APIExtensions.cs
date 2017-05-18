using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serverside.Core.Extenstions
{
    public static class APIExtensions
    {
        public static List<Client> GetNearestPlayers(this Vector3 position)
        {
            //Stopwatch _debug = new Stopwatch();
            //_debug.Start();
            List<Client> _ret = new List<Client>();
            _ret = API.shared.getAllPlayers().Select(n => n).OrderBy(n => new Vector3(n.position.X, n.position.Y, n.position.Z).DistanceTo(position)).ToList();
            //API.shared.sendNotificationToPlayer(player, _debug.ElapsedTicks.ToString(), true); // DEBUG
            //_debug.Reset();
            return _ret;
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
