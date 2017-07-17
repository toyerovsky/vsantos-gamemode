/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using Newtonsoft.Json;
using Serverside.Core.Extensions;

namespace Serverside.Core.Description
{
    public class RPDescriptions : Script
    {
        public RPDescriptions()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPDescriptions] Uruchomione pomyslnie.", ConsoleColor.DarkMagenta);
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        { 
            if (eventName == "OnPlayerAddDescription")
            {
                var player = sender.GetAccountController();

                if (player.CharacterController.Character.Descriptions.Count > 3)
                {
                    API.sendNotificationToPlayer(sender, "Liczba twoich opisów nie może być większa niż 3.");
                    return;
                }
                //arguments[0] tytul, arguments[1] opis
                Database.Models.Description description = new Database.Models.Description()
                {
                    Character = sender.GetAccountController().CharacterController.Character,
                    Title = arguments[0].ToString(),
                    Content = arguments[1].ToString()
                };

                player.CharacterController.Character.Descriptions.Add(description);
                player.Save();

                API.sendNotificationToPlayer(sender, "Twój opis został pomyślnie dodany.");

                API.triggerClientEvent(sender, "ShowDescriptionsCef", false);

                string descriptionsJson = JsonConvert.SerializeObject(player.CharacterController.Character.Descriptions);

                API.triggerClientEvent(sender, "ShowDescriptionsCef", true, descriptionsJson);
            }
            else if (eventName == "OnPlayerEditDescription")
            {
                if (arguments[0] != null && arguments[1] != null && arguments[2] != null)
                {
                    //arguments[0] index na liście, arguments[1] tytul, arguments[2] opis
                    var player = sender.GetAccountController();

                    var description = player.CharacterController.Character.Descriptions.ToList()[0];

                    description.Title = arguments[1].ToString();
                    description.Content = arguments[2].ToString();

                    player.Save();
                    
                    API.sendNotificationToPlayer(sender, "Twój opis został pomyślnie edytowany.");

                    API.triggerClientEvent(sender, "ShowDescriptionsCef", false);

                    string descriptionsClient = JsonConvert.SerializeObject(player.CharacterController.Character.Descriptions.ToList());

                    API.triggerClientEvent(sender, "ShowDescriptionsCef", true, descriptionsClient);
                }
            }
            else if (eventName == "OnPlayerDeleteDescription")
            {
                if (arguments[0] != null)
                {
                    var player = sender.GetAccountController();
                    List<Database.Models.Description> descriptions = player.CharacterController.Character.Descriptions.ToList(); 

                    var description = descriptions[Convert.ToInt32(arguments[0])];

                    player.CharacterController.Character.Descriptions.Remove(description);
                    player.Save();

                    API.sendNotificationToPlayer(sender, "Twój opis został pomyślnie usunięty.");
                    API.triggerClientEvent(sender, "ShowDescriptionsCef", false);

                    string descriptionsClient = JsonConvert.SerializeObject(player.CharacterController.Character.Descriptions);

                    API.triggerClientEvent(sender, "ShowDescriptionsCef", true, descriptionsClient);
                }
            }
            else if (eventName == "OnPlayerSetDescription")
            {
                //args[0] to string opisu
                if (arguments[0].ToString() != "")
                {
                    var player = sender.GetAccountController().CharacterController;
                    player.Description.Value = arguments[0].ToString();
                }
            }
            else if (eventName == "OnPlayerResetDescription")
            {
                var player = sender.GetAccountController().CharacterController;
                player.Description.ResetCurrentDescription();
            }
        }

        [Command("opis")]
        public void ShowDescriptionsList(Client sender)
        {
            var player = sender.GetAccountController().CharacterController;
            string descriptionsClient = JsonConvert.SerializeObject(player.Character.Descriptions);
            API.triggerClientEvent(sender, "ShowDescriptionsCef", descriptionsClient);
        }
    }
}