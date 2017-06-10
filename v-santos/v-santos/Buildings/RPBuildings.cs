using System;
using GTANetworkServer;
using Serverside.Controllers;
using Serverside.Core.Extensions;

namespace Serverside.Buildings
{
    public class RPBuildings : Script
    {
        public RPBuildings()
        {
            API.onResourceStart += ResourceStartHandler;
            API.onClientEventTrigger += ClientEventTriggerHandler;
        }

        private void ClientEventTriggerHandler(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "PassDoors")
            {
                BuildingController.PassDoors(sender);
            }
            else if (eventName == "KnockDoors")
            {
                BuildingController.Knock(sender);
            }
        }

        private void ResourceStartHandler()
        {
            APIExtensions.ConsoleOutput("[RPBuildings] Uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
        }
    }
}