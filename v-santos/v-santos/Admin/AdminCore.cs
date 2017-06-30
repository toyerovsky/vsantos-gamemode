using GTANetworkServer;
using GTANetworkServer.Constant;
using GTANetworkShared;
using Serverside.Core;
using Serverside.Core.Extensions;

namespace Serverside.Admin
{
    public class AdminCore : Script
    {
        [Command("ustawrange", "~y~ UŻYJ ~w~ /ustawrange [id] [nazwa]")]
        public void SetRank(Client sender, int id, ServerRank rank)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.Adminadministrator3)
            {
                sender.Notify("Nie posiadasz uprawnień do ustawiania rang.");
                return;
            }

            var controller = RPEntityManager.GetAccountByServerId(id);
            if (controller == null)
            {
                sender.Notify("Nie znaleziono gracza o podanym Id.");
                return;
            }

            controller.AccountData.ServerRank = rank;
            controller.Save();
        }

        [Command("tpc", "~y~ UŻYJ ~w~ /tpc [x] [y] [z]")]
        public void TeleportToCords(Client sender, float x, float y, float z)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.Support)
            {
                sender.Notify("Nie posiadasz uprawnień do teleportu na koordynaty.");
                return;
            }
            sender.position = new Vector3(x, y, z);
        }

        [Command("tp", "~y~ UŻYJ ~w~ /tp [id]")]
        public void TeleportToPlayer(Client sender, int id)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.Support)
            {
                sender.Notify("Nie posiadasz uprawnień do teleportu.");
                return;
            }
            var controller = RPEntityManager.GetAccountByServerId(id);
            if (controller == null)
            {
                sender.Notify("Nie znaleziono gracza o podanym Id.");
                return;
            }
            sender.position = new Vector3(controller.Client.position.X - 5f, controller.Client.position.Y, controller.Client.position.Z + 1f);
        }

        [Command("spec", "~y~ UŻYJ ~w~ /spec [id]")]
        public void SpectatePlayer(Client sender, int id)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.Support)
            {
                sender.Notify("Nie posiadasz uprawnień do obserwowania.");
                return;
            }
            var controller = RPEntityManager.GetAccountByServerId(id);
            if (controller == null)
            {
                sender.Notify("Nie znaleziono gracza o podanym Id.");
                return;
            }
            API.setPlayerToSpectatePlayer(sender, controller.Client);
        }

        [Command("specoff")]
        public void TurnOffSpectating(Client player)
        {
            player.triggerEvent("ResetCamera");
        }

        [Command("addspec", "~y~ UŻYJ ~w~ /addspec [id]", Description = "Polecenie ustawia wybranemu graczowi specowanie na nas.")]
        public void AddSpectator(Client sender, int id)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.Support3)
            {
                sender.Notify("Nie posiadasz uprawnień do ustawienia obserwowania.");
                return;
            }
            var controller = RPEntityManager.GetAccountByServerId(id);
            if (controller == null)
            {
                sender.Notify("Nie znaleziono gracza o podanym Id.");
                return;
            }
            API.setPlayerToSpectator(controller.Client);
        }

        [Command("kick", "~y~ UŻYJ ~w~ /kick [id] (powod)", GreedyArg = true)]
        public void KickPlayer(Client sender, int id, string reason = "")
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.Support)
            {
                sender.Notify("Nie posiadasz uprawnień do ustawienia obserwowania.");
                return;
            }
            var controller = RPEntityManager.GetAccountByServerId(id);
            if (controller == null)
            {
                sender.Notify("Nie znaleziono gracza o podanym Id.");
                return;
            }
            API.kickPlayer(controller.Client, reason);
        }

        [Command("fly")]
        public void StartFying(Client sender)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.Support5)
            {
                sender.Notify("Nie posiadasz uprawnień do ustawienia latania.");
                return;
            }

            if (sender.HasData("FlyState"))
            {
                sender.ResetData("FlyState");
                API.triggerClientEvent(sender, "FreeCamStop");
                return;
            }

            sender.SetData("FlyState", true);
            API.triggerClientEvent(sender, "FreeCamStart", API.getEntityPosition(sender));
        }

        [Command("inv")]
        public void SetPlayerInvicible(Client sender)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.Support5)
            {
                sender.Notify("Nie posiadasz uprawnień do ustawienia niewidzialności.");
                return;
            }
            if (API.getEntityInvincible(sender))
            {
                sender.invincible = false;
                sender.Notify("Wyłączono niewidzialność.");
            }
            else
            {
                sender.invincible = true;
                sender.Notify("Włączono niewidzialność.");
            }
        }
    }
}