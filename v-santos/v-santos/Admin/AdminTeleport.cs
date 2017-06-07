using System;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Core;
using Serverside.Core.Extensions;

namespace Serverside.Admin
{
    public class AdminTeleport : Script
    {
        [Command("tpc")]
        public void TeleportToCords(Client player, string x, string y, string z)
        {
            player.position = new Vector3(float.Parse(x), float.Parse(y), float.Parse(z));
        }

        [Command("tp")]
        public void TeleportToPlayer(Client player, string id)
        {
            if (!Validator.IsIntIdValid(id))
            {
                API.sendNotificationToPlayer(player, "Wprowadzono dane w nieprawidłowym formacie.");
            }

            Client getter = API.getAllPlayers().Single(x => x.GetAccountController().ServerId == Convert.ToInt32(id));
            player.position = new Vector3(getter.position.X - 5f, getter.position.Y, getter.position.Z + 2f);
        }
    }
}