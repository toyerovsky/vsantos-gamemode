using System;
using System.Data.Entity.Core.Common;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Core;
using Serverside.Core.Extensions;

namespace Serverside.Admin
{
    public class AdminCore : Script
    {
        [Command("tpc", "~UŻYJ~ /tpc [x] [y] [z]")]
        public void TeleportToCords(Client player, float x, float y, float z)
        {
            player.position = new Vector3(x, y, z);
        }

        [Command("tp", "~y~ UŻYJ ~w~ /tp [id]")]
        public void TeleportToPlayer(Client player, int id)
        {
            var controller = RPEntityManager.GetAccountByServerId(id);
            if (controller == null)
            {
                player.Notify("Nie znaleziono gracza o podanym Id.");
                return;
            }
            player.position = new Vector3(controller.Client.position.X - 5f, controller.Client.position.Y, controller.Client.position.Z + 1f);
        }

        [Command("spec", "~y~ UŻYJ ~w~ /spec [id]")]
        public void SpectatePlayer(Client player, int id)
        {
            var controller = RPEntityManager.GetAccountByServerId(id);
            if (controller == null)
            {
                player.Notify("Nie znaleziono gracza o podanym Id.");
                return;
            }
            API.setPlayerToSpectatePlayer(player, controller.Client);
        }

        [Command("specoff")]
        public void TurnOffSpectating(Client player)
        {
            player.triggerEvent("ResetCamera");
        }

        [Command("addspec", "~y~ UŻYJ ~w~ /addspec [id]", Description = "Polecenie ustawia wybranemu graczowi specowanie na nas.")]
        public void AddSpectator(Client player, int id)
        {
            var controller = RPEntityManager.GetAccountByServerId(id);
            if (controller == null)
            {
                player.Notify("Nie znaleziono gracza o podanym Id.");
                return;
            }
            API.setPlayerToSpectator(controller.Client);
        }

        [Command("kick", "~y~ UŻYJ ~w~ /kick [id] (powod)", GreedyArg = true)]
        public void KickPlayer(Client player, int id, string reason = "")
        {
            var controller = RPEntityManager.GetAccountByServerId(id);
            if (controller == null)
            {
                player.Notify("Nie znaleziono gracza o podanym Id.");
                return;
            }
            API.kickPlayer(controller.Client, reason);
        }

        [Command("fly")]
        public void StartFying(Client sender)
        {
            if (sender.HasData("FlyState"))
            {
                sender.ResetData("FlyState");
                API.triggerClientEvent(sender, "FreeCamStop");
            }

            sender.SetData("FlyState", true);
            API.triggerClientEvent(sender, "FreeCamStart", API.getEntityPosition(sender));
        }

        [Command("inv")]
        public void SetPlayerInvicible(Client player)
        {
            if (API.getEntityInvincible(player))
            {
                player.invincible = false;
                player.Notify("Wyłączono niewidzialność.");
            }
            else
            {
                player.invincible = true;
                player.Notify("Włączono niewidzialność.");
            }
        }
    }
}