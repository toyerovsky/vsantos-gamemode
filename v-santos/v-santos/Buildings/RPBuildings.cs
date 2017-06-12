using System;
using System.Linq;
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
            else if (eventName == "BuyBuilding")
            {
                if (!sender.HasData("CurrentDoors")) return;
                BuildingController controller = sender.GetData("CurrentDoors");
                controller.Buy(sender);
            }
        }

        private void ResourceStartHandler()
        {
            APIExtensions.ConsoleOutput("[RPBuildings] Uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
        }

        [Command("stworzbudynek")]
        public void CreateBuilding(Client sender)
        {
            sender.triggerEvent("ShowAdminBuildingMenu", Constant.ConstantItems.DefaultInteriors.Select(x => x.Key).ToList());
        }

        [Command("drzwi")]
        public void ManageBuilding(Client sender)
        {
            //panel zarządzanie budynkiem dla graczy
        }
    }
}