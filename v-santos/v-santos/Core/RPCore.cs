using System.Collections.Generic;
using GTANetworkServer;
using Serverside.Database;

namespace Serverside.Core
{
    public sealed class RPCore : Script
    {
        public static MySqlDatabaseHelper Db;
        //Robimy słownik wszystkich klientów żeby można było potem korzystać z helpera tego gracza
        //long to ID konta
        public static Dictionary<long, Player> Players; 

        public RPCore()
        {
            API.onResourceStart += API_onResourceStart;
            API.onResourceStop += API_onResourceStop;
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("RPCore uruchomione pomyslnie!");

            Db = new MySqlDatabaseHelper();
            Players = new Dictionary<long, Player>();
        }

        private void API_onResourceStop()
        {
            Db = null;
        }

        public static string GetColoredString(string color, string text)
        {
            return "~" + color + "~" + text;
        }

        [Command("q")]
        public void Quit(Client player)
        {
            API.shared.kickPlayer(player);
        }
    }
}
