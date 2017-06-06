using System.Linq;
using GTANetworkServer;
using Serverside.Core.Extensions;

namespace Serverside.Core.Finders
{
    public static class PlayerFinder
    {
        public static bool TryFindClientByServerId(int id, out Client result)
        {
            result = null;
            if (API.shared.getAllPlayers().Any(player => player.GetAccountController().ServerId == id)) result =
                    API.shared.getAllPlayers().Single(x => x.GetAccountController().ServerId == id);
            return result != null;
        }

        public static bool TryFindClientInRadiusOfClientByServerId(Client sender, int id, float radius, out Client result)
        {
            result = null;
            if (API.shared.getAllPlayers().Any(player => player.GetAccountController().ServerId == id))
                result = API.shared.getPlayersInRadiusOfPlayer(radius, sender)
                    .Single(p => p.GetAccountController().ServerId == id);
            return result != null;
        }

        public static Client FindPlayerByAccountId(long accountId)
        {
            return API.shared.getAllPlayers().Single(player => player.GetAccountController().AccountId == accountId);
        }

        public static Client FindPlayerByCharacterId(long characterId)
        {
            return API.shared.getAllPlayers().Single(player => player.GetAccountController().CharacterController.Character.Id == characterId);
        }
    }
}
