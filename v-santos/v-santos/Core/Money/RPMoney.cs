using System;
using GTANetworkServer;
using Serverside.Core.Extenstions;

namespace Serverside.Core.Money
{
    public class RPMoney : Script
    {
        private API api = new API();       
        
        [Command("plac", "~y~UŻYJ: ~w~ /plac [id] [kwota]", Alias = "pay", GreedyArg = true)]
        public void TransferWalletMoney(Client player, string id, string unsafeMoneyCount)
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
                RPChat.SendMessageToPlayer(player, "Podano dane w nieprawidłowym formacie.", ChatMessageType.ServerInfo);
                return;
            }

            //Sprawdzamy czy gracz ma na pewno wszystkie potrzebne dane
            if (!player.HasData("RP_ACCOUNT"))
            {
                RPChat.SendMessageToPlayer(player, "Twoja postać nie posiada identyfikatora postaci, zaloguj się ponownie.", ChatMessageType.ServerInfo);
                return;
            }

            MoneyManager manager = new MoneyManager();

            if (!manager.CanPay(player.GetAccountController(), safeMoneyCount))
            {
                RPChat.SendMessageToPlayer(player, "Nie posiadasz wystarczającej ilości gotówki.", ChatMessageType.ServerInfo);
                return;
            }

            if (Convert.ToInt32(player.GetAccountController().ServerId).Equals(ID))
            {
                api.sendChatMessageToPlayer(player, "Nie możesz podać gotówki samemu sobie.");
                return;
            }
            
            Client gettingPlayer;

            //if (PlayerFinder.TryFindClientInRadiusOfPlayerByServerId(player, ID, 6, out gettingPlayer))
            gettingPlayer = player.position.GetNearestPlayer();
            if(gettingPlayer != null)
            {
                if (!gettingPlayer.HasData("RP_ACCOUNT"))
                {
                    RPChat.SendMessageToPlayer(gettingPlayer, "Twoja postać nie posiada identyfikatora postaci, zaloguj się ponownie.", ChatMessageType.ServerInfo);
                    return;
                }

                //temu zabieramy
                manager.RemoveMoney(player.GetAccountController(), safeMoneyCount);

                //temu dodajemy gotówke
                manager.AddMoney(gettingPlayer.GetAccountController(), safeMoneyCount);

                api.sendChatMessageToPlayer(player, String.Format("~g~Osoba {0} otrzymała od ciebie ${1}.", gettingPlayer.name, safeMoneyCount));
                api.sendChatMessageToPlayer(gettingPlayer, String.Format("~g~Osoba {0} przekazała ci ${1}.", player.name, safeMoneyCount));
            }
        }
    }
}