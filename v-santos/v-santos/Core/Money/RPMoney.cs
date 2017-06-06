using System;
using GTANetworkServer;
//using Serverside.Core.Finders;
using Serverside.Core.Extensions;

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

        [Command("plac", "~y~UŻYJ: ~w~ /plac [id] [kwota]", Alias = "pay", GreedyArg = true)]
        public void TransferWalletMoney(Client sender, string id, string unsafeMoneyCount)
        {
            decimal safeMoneyCount;
            int ID;

            if (Validator.IsMoneyStringValid(unsafeMoneyCount) && Validator.IsIntIdValid(id))
            {
                safeMoneyCount = Convert.ToDecimal(unsafeMoneyCount);
                ID = Convert.ToInt32(id);
            }
            else
            {
                sender.Notify("Podano dane w nieprawidłowym formacie.");
                return;
            }

            if (!sender.HasMoney(safeMoneyCount))
            {
                sender.Notify("Nie posiadasz wystarczającej ilości gotówki.");
                return;
            }

            if (sender.GetAccountController().ServerId == ID)
            {
                sender.Notify("Nie możesz podać gotówki samemu sobie.");
                return;
            }
            
            //Client gettingPlayer;

            Client gettingPlayer = API.getPlayersInRadiusOfPlayer(6f, sender).Find(x => x.GetAccountController().ServerId == ID); // łatwe i proste :D
            //if (PlayerFinder.TryFindClientInRadiusOfClientByServerId(sender, ID, 6, out gettingPlayer)) // po chuj te kombinacje XD
            //{
                
                //temu zabieramy
                sender.RemoveMoney(safeMoneyCount);

                //temu dodajemy gotówke
                gettingPlayer.AddMoney(safeMoneyCount);

                API.sendChatMessageToPlayer(sender,
                    $"~g~Osoba {gettingPlayer.name} otrzymała od ciebie ${safeMoneyCount}.");
                API.sendChatMessageToPlayer(gettingPlayer, $"~g~Osoba {sender.name} przekazała ci ${safeMoneyCount}.");
            //}
        }
    }
}