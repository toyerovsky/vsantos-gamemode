using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using Serverside.Admin;
using Serverside.Admin.Enums;
using Serverside.Constant;
using Serverside.Controllers;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Database;

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
                /* Arguments
                 * args[0] string name 
                 * args[1] decimal cost
                 * args[2] string interiorName
                 * args[3] string description = ""
                 */

                if (ConstantItems.DefaultInteriors.All(i => i.Name != (string)arguments[2]) || !sender.HasData("AdminDoorPosition")) return;

                var internalPosition = ConstantItems.DefaultInteriors.First(i => i.Name == (string)arguments[2]).InternalPosition;
                Vector3 externalPosition = sender.GetData("AdminDoorPosition");

                var building = new BuildingController((string)arguments[0], (string)arguments[3], sender.GetAccountController().AccountId, internalPosition.X, internalPosition.Y, internalPosition.Z, externalPosition.X, externalPosition.Y, externalPosition.Z, Convert.ToDecimal(arguments[1]), BuildingController.GetNextFreeDimension());

                building.Save();

                sender.Notify("Dodawanie budynku zakończyło się ~h~~g~pomyślnie.");
                sender.position = externalPosition;
            }
            else if (eventName == "EdingBuildingInfo")
            {
                /* Arguments
                 * args[0] name 
                 * args[1] description
                 * args[2] enterCharge
                 */

                BuildingController building = sender.HasData("CurrentDoors") ? sender.GetData("CurrentDoors") : sender.GetAccountController().CharacterController.CurrentBuilding;
                building.BuildingData.Name = arguments[0].ToString();
                building.BuildingData.Description = arguments[1].ToString();
                if (decimal.TryParse(arguments[2].ToString(), out decimal result)) building.BuildingData.EnterCharge = result;
                building.Save();
            }
        }

        private void ResourceStartHandler()
        {
            APIExtensions.ConsoleOutput("[RPBuildings] Uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
        }

        #region PLAYER COMMANDS

        [Command("drzwizamknij")]
        public void ChangeBuildingLockState(Client sender)
        {
            if (!sender.HasData("CurrentDoors"))
            {
                sender.Notify("Nie znajdujesz się przy drzwiach.");
                return;
            }

            BuildingController building = sender.GetData("CurrentDoors");

            //TODO: Dodanie zeby pracownicy mogli otwierać budynki grupowe zgodnie z uprawnieniami
            if (building.BuildingData.Character == null || building.BuildingData.Character.Id != sender.GetAccountController().CharacterController.Character.Id)
            {
                sender.Notify("Nie jesteś właścicielem tego budynku.");
                return;
            }

            string text = building.DoorsLocked ? "otwarte" : "zamknięte";
            sender.Notify($"Drzwi zostały {text}");
            building.DoorsLocked = !building.DoorsLocked;
        }

        [Command("drzwi", "~y~UŻYJ ~w~ /drzwi")]
        public void ManageBuilding(Client sender, long id = -1)
        {
            //Dla administracji
            if (id != -1)
            {
                var buildindController = RPEntityManager.GetBuilding(id);
                if (buildindController != null)
                {
                    var adminInfo = new List<string>
                    {
                        buildindController.BuildingData.Name,
                        buildindController.BuildingData.Description,
                        buildindController.BuildingData.EnterCharge.ToString()
                    };

                    sender.SetData("CurrentDoors", buildindController);
                    sender.triggerEvent("ShowBuildingManagePanel", adminInfo);
                    return;
                }
                sender.Notify("Budynek o podanym Id nie istnieje.");
                return;
            }

            //CurrentBuilding jest po to żeby gracz mógł zarządzać budynkiem ze środka oraz do ofert
            if (sender.GetAccountController().CharacterController.CurrentBuilding != null ||
                sender.HasData("CurrentDoors"))
            {
                BuildingController building = sender.HasData("CurrentDoors")
                    ? sender.GetData("CurrentDoors")
                    : sender.GetAccountController().CharacterController.CurrentBuilding;

                //TODO: Dodanie, żeby właściciel grupy mógł zarządzać budynkiem grupowym
                if (building.BuildingData.Character == null || building.BuildingData.Character.Id != sender.GetAccountController().CharacterController.Character.Id)
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
            else
            {
                sender.Notify("Aby otworzyć panel zarządzania budynkiem musisz znajdować...");
                sender.Notify("...się w markerze bądź środku budynku.");
            }
        }

        #endregion

        #region ADMIN COMMANDS

        [Command("usundrzwi", "~y~UŻYJ ~w~ /usundrzwi (id)")]
        public void DeleteBuilding(Client sender, long id = -1)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.GameMaster4)
            {
                sender.Notify("Nie posiadasz uprawnień do usuwania drzwi.");
                return;
            }

            if (id == -1 && !sender.hasData("CurrentDoors"))
            {
                sender.Notify("Aby usunąć budynek musisz podać jego Id, lub...");
                sender.Notify("...znajdować się w jego drzwiach.");
            }

            if (sender.HasData("CurrentDoors"))
            {
                var building = (BuildingController)sender.GetData("CurrentDoors");
                building.Dispose();
                ContextFactory.Instance.Buildings.Remove(building.BuildingData);
                ContextFactory.Instance.SaveChanges();
                return;
            }

            if (id != -1 && RPEntityManager.GetBuilding(id) != null)
            {
                var building = RPEntityManager.GetBuilding(id);
                building.Dispose();
                ContextFactory.Instance.Buildings.Remove(building.BuildingData);
                ContextFactory.Instance.SaveChanges();
                return;
            }

            sender.Notify("Podany budynek nie istnieje.");
        }

        [Command("dodajdrzwi")]
        public void CreateBuilding(Client sender)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.GameMaster4)
            {
                sender.Notify("Nie posiadasz uprawnień do tworzenia drzwi.");
                return;
            }

            sender.Notify("Ustaw się w pozycji markera, a następnie wpisz /tu.");
            sender.Notify("...użyj /diag aby poznać swoją obecną pozycję.");

            API.onChatCommand += Handler;

            void Handler(Client o, string command, CancelEventArgs cancel)
            {
                if (o == sender && command == "/tu")
                {
                    cancel.Cancel = true;
                    if (RPEntityManager.GetBuildings().Any(b => b.BuildingMarker.position.DistanceTo(o.position) < 5))
                    {
                        sender.Notify("W bliskim otoczeniu tego budynku znajduje się już inny budynek.");
                        return;
                    }

                    o.SetData("AdminDoorPosition", o.position);
                    sender.triggerEvent("ShowAdminBuildingMenu", JsonConvert.SerializeObject(ConstantItems.DefaultInteriors));
                    sender.Notify("Dodawanie budynku zakończyło się ~h~~g~pomyślnie.");
                    API.onChatCommand -= Handler;
                }
            }
        }
        #endregion
    }
}