using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using Newtonsoft.Json;
using Serverside.Constant;
using Serverside.Controllers;
using Serverside.Core.Extensions;
using Serverside.Core.Extenstions;

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
            else if (eventName == "AddBuilding")
            {
                /* Argumenty
                 * args[0] name 
                 * args[1] cost
                 * args[2] interior
                 * args[3] description = ""
                 */

                if (ConstantItems.DefaultInteriors.All(i => i.Name != (string)arguments[2]) || !sender.HasData("AdminDoorPosition")) return;

                var internalPosition = ConstantItems.DefaultInteriors.First(i => i.Name == (string)arguments[2]).InternalPostion;
                Vector3 externalPosition = sender.GetData("AdminDoorPosition");

                var building = new BuildingController((string)arguments[0], (string)arguments[3], sender.GetAccountController().AccountId, internalPosition.X,
                    internalPosition.Y, internalPosition.Z, externalPosition.X, externalPosition.Y, externalPosition.Z, Convert.ToDecimal(arguments[1]), BuildingController.GetNextFreeDimension());
                building.Save();

                sender.Notify("Dodawanie budynku zakończyło się pomyślnie.");
                sender.position = externalPosition;
            }
            else if (eventName == "EdingBuildingInfo")
            {
                /* Argumenty
                 * args[0] name 
                 * args[1] description
                 * args[2] enterCharge
                 */

                BuildingController building = sender.HasData("CurrentDoors") ? sender.GetData("CurrentDoors") : sender.GetAccountController().CharacterController.CurrentBuilding;
                building.BuildingData.Name = arguments[0].ToString();
                building.BuildingData.Description = arguments[1].ToString();
                building.BuildingData.EnterCharge = Convert.ToDecimal(arguments[2]);
                building.Save();
            }
        }

        private void ResourceStartHandler()
        {
            APIExtensions.ConsoleOutput("[RPBuildings] Uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
        }

        [Command("stworzbudynek")]
        public void CreateBuilding(Client sender)
        {
            sender.Notify("Ustaw się w pozycji markera, a następnie wpisz /tu.");
            sender.Notify("...użyj /diag aby poznać swoją obecną pozycję.");

            API.onChatCommand += Handler;

            void Handler(Client o, string command, CancelEventArgs cancel)
            {
                if (o == sender && command == "/tu")
                {
                    cancel.Cancel = true;
                    o.SetData("AdminDoorPosition", o.position);
                    sender.triggerEvent("ShowAdminBuildingMenu", JsonConvert.SerializeObject(ConstantItems.DefaultInteriors));
                    API.onChatCommand -= Handler;
                }
            }
        }

        [Command("drzwi")]
        public void ManageBuilding(Client sender)
        {
            if (sender.GetAccountController().CharacterController.CurrentBuilding == null || !sender.HasData("CurrentDoors"))
            {
                sender.Notify("Aby otworzyć panel zarządzania budynkiem musisz znajdować...");
                sender.Notify("...się w markerze bądź środku budynku.");
                return;
            }
            
            //Robimy tak, żeby można było otwierać panel z budynku i z zewnątrz
            BuildingController building = sender.HasData("CurrentDoors") ? sender.GetData("CurrentDoors") : sender.GetAccountController().CharacterController.CurrentBuilding;

            //TODO: Dodanie, żeby właściciel grupy mógł zarządzać budynkiem grupowym
            if (building.BuildingData.Character.Id != sender.GetAccountController().CharacterController.Character.Id)
            {
                sender.Notify("Nie jesteś właścicielem tego budynku.");
                return;
            }

            var info = new List<string>
            {
                building.BuildingData.Name,
                building.BuildingData.Description,
                building.BuildingData.EnterCharge.ToString()
            };

            sender.triggerEvent("ShowBuildingManagePanel", info);
        }
    }
}