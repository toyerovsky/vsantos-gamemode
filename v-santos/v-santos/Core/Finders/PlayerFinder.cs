using System.Linq;
using GTANetworkServer;

namespace Serverside.Core.Finders
{
    public static class PlayerFinder
    {
        public static bool TryFindClientByServerId(Client sender, int id, out Client result)
        {
            if (API.shared.getAllPlayers().Any(player => player.hasData("ServerId") && player.getData("ServerId") == id))
            {
                result = API.shared.getAllPlayers().First(player => player.hasData("ServerId") && player.getData("ServerId") == id);
                return true;
            }

            result = null;
            API.shared.sendNotificationToPlayer(sender, "Nie znaleziono gracza o podanym ID.");
            return false;
        }

        public static bool TryFindClientInRadiusOfPlayerByServerId(Client sender, int id, float radius, out Client result)
        {
            if (API.shared.getAllPlayers().Any(player => player.hasData("ServerId") && player.getData("ServerId") == id))
            {
                result = API.shared.getPlayersInRadiusOfPlayer(radius, sender).First(p => p.hasData("ServerId") && p.getData("ServerId") == id);
                return true;
            }

            result = null;
            API.shared.sendNotificationToPlayer(sender, "W pobliżu nie ma gracza o podanym ID.");
            return false;
        }

        /// <summary>
        /// Znajdź klienta po ID Konta
        /// </summary>
        /// <param name="aid"></param>
        /// <returns></returns>
        public static Client FindClientByAid(long aid)
        {
            return API.shared.getAllPlayers().First(player => player.hasData("AccountID") && player.getData("AccountID") == aid);
        }

        public static Client FindClientByCid(long cid)
        {
            return API.shared.getAllPlayers().First(player => player.hasData("CharacterID") && player.getData("CharacterID") == cid);
        }
    }
}