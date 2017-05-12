using System;
using GTANetworkServer;
using Serverside.Core.Finders;

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
            if (!player.hasData("CharacterID"))
            {
                RPChat.SendMessageToPlayer(player, "Twoja postać nie posiada identyfikatora postaci, zaloguj się ponownie.", ChatMessageType.ServerInfo);
                return;
            }

            MoneyManager manager = new MoneyManager();

            if (!manager.CanPay(player.getData("CharacterID"), safeMoneyCount))
            {
                RPChat.SendMessageToPlayer(player, "Nie posiadasz wystarczającej ilości gotówki.", ChatMessageType.ServerInfo);
                return;
            }

            if (Convert.ToInt32(api.getEntityData(player.handle, "ID")).Equals(ID))
            {
                api.sendChatMessageToPlayer(player, "Nie możesz podać gotówki samemu sobie.");
                return;
            }
            
            Client gettingPlayer;

            if (PlayerFinder.TryFindClientInRadiusOfPlayerByServerId(player, ID, 6, out gettingPlayer))
            {
                if (!gettingPlayer.hasData("CharacterID"))
                {
                    RPChat.SendMessageToPlayer(gettingPlayer, "Twoja postać nie posiada identyfikatora postaci, zaloguj się ponownie.", ChatMessageType.ServerInfo);
                    return;
                }

                //temu zabieramy
                manager.RemoveMoney(player.getData("CharacterID"), safeMoneyCount);

                //temu dodajemy gotówke
                manager.AddMoney(gettingPlayer.getData("CharacterID"), safeMoneyCount);

                api.sendChatMessageToPlayer(player, String.Format("~g~Osoba {0} otrzymała od ciebie ${1}.", gettingPlayer.name, safeMoneyCount));
                api.sendChatMessageToPlayer(gettingPlayer, String.Format("~g~Osoba {0} przekazała ci ${1}.", player.name, safeMoneyCount));
            }
        }
    }
}