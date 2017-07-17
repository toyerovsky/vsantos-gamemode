/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;

using Serverside.Core.Extensions;

namespace Serverside.Core
{
    public enum ShowType
    {
        Dowod,
        Prawko
    }

    public class RPMiscCommands : Script
    {
        public RPMiscCommands()
        {
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPMiscCommands] uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
        }

        [Command("id", "~y~UŻYJ ~w~ /id [nazwa]", GreedyArg = true)]
        public void ShowPlayersWithSimilarName(Client sender, string name)
        {
            if (!RPEntityManager.GetAccounts().Any(x => x.Value.CharacterController.FormatName.ToLower().StartsWith(name)))
            {
                sender.Notify("Nie znaleziono gracza o podanej nazwie.");
                return;
            }

            var accounts = RPEntityManager.GetAccounts()
                .Where(x => x.Value.CharacterController.FormatName.ToLower().StartsWith(name));

            RPChat.SendMessageToPlayer(sender, "Znalezieni gracze: ", ChatMessageType.ServerInfo);
            foreach (var account in accounts)
            {
                RPChat.SendMessageToPlayer(sender, $"({account.Value.ServerId}) {account.Value.CharacterController.FormatName}", ChatMessageType.ServerInfo);
            }
        }

        [Command("pokaz", "~y~ UŻYJ ~w~ /pokaz [typ] [id]")]
        public void Show(Client sender, ShowType type, int id)
        {
            if (API.getPlayersInRadiusOfPlayer(6f, sender).All(x => x.GetAccountController().ServerId != id))
            {
                sender.Notify("W twoim otoczeniu nie znaleziono gracza o podanym Id.");
                return;
            }

            var player = sender.GetAccountController();
            var getter = API.getPlayersInRadiusOfPlayer(6f, sender)
                .Single(x => x.GetAccountController().ServerId == id);

            if (type == ShowType.Dowod)
            {
                RPChat.SendMessageToNearbyPlayers(player.Client, $"pokazuje dowód osobisty {getter.name}", ChatMessageType.ServerMe);
                getter.Notify($"Osoba {player.CharacterController.FormatName} pokazała Ci swój dowód osobisty.");
            }
            else if (type == ShowType.Prawko)
            {
                RPChat.SendMessageToNearbyPlayers(player.Client, $"pokazuje prawo jazdy {getter.name}", ChatMessageType.ServerMe);
                getter.Notify($"Osoba {player.CharacterController.FormatName} pokazała Ci swoje prawo jazdy.");
            }
        }
    }
}