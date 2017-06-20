using System;
using GTANetworkServer;
//using Serverside.Core.Finders;
using Serverside.Core.Extensions;
using Serverside.Core.Extenstions;

namespace Serverside.Core.Money
{
    public class RPMoney : Script
    {
        public RPMoney()
        {
            API.onResourceStart += OnResourceStartHandler;
        }

        private void OnResourceStartHandler()
        {
            APIExtensions.ConsoleOutput("[RPMoney] Uruchomione pomyslnie.", ConsoleColor.DarkMagenta);
        }

        [Command("plac", "~y~UŻYJ: ~w~ /plac [id] [kwota]", Alias = "pay")]
        public void TransferWalletMoney(Client sender, int id, decimal safeMoneyCount)
        {
            if (!sender.GetAccountController().CharacterController.CanPay) return;

            if (!sender.HasMoney(safeMoneyCount))
            {
                sender.Notify("Nie posiadasz wystarczającej ilości gotówki.");
                return;
            }

            if (sender.GetAccountController().ServerId == id)
            {
                sender.Notify("Nie możesz podać gotówki samemu sobie.");
                return;
            }

            Client gettingPlayer = API.getPlayersInRadiusOfPlayer(6f, sender).Find(x => x.GetAccountController().ServerId == id);
            if (gettingPlayer == null)
            {
                sender.Notify("Nie znaleziono gracza o podanym Id");
                return;
            }

            //temu zabieramy
            sender.RemoveMoney(safeMoneyCount);

            //temu dodajemy gotówke
            gettingPlayer.AddMoney(safeMoneyCount);

            API.sendChatMessageToPlayer(sender,
                $"~g~Osoba {gettingPlayer.name} otrzymała od ciebie ${safeMoneyCount}.");
            API.sendChatMessageToPlayer(gettingPlayer, $"~g~Osoba {sender.name} przekazała ci ${safeMoneyCount}.");
        }
    }
}