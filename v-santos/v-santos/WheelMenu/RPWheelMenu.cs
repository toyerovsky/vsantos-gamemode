using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient.Memcached;
using Serverside.Controllers;
using Serverside.Core.Extensions;
using Serverside.Core.Extenstions;
using Serverside.Core.WheelMenu;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Core.Vehicles;
using Serverside.Core;

namespace Serverside.WheelMenu
{
    public sealed class RPWheelMenu : Script
    {
        public RPWheelMenu()
        {
            API.onResourceStart += OnResourceStartHandler;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void OnResourceStartHandler()
        {
            APIExtensions.ConsoleOutput("[RPWheelMenu] Uruchomione pomyslnie.", ConsoleColor.DarkMagenta);
        }

        private void API_onClientEventTrigger(GTANetworkServer.Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "RequestWheelMenu")
            {
                //args[0] to NetHandle które przyszło z RayCast
                //nie używam as ponieważ do struktury nie wolno
                if (!(arguments[0] is NetHandle)) return;
                if (API.shared.getPlayerFromHandle((NetHandle)arguments[0]) != null)
                {

                }
                else if (RPEntityManager.GetVehicle((NetHandle)arguments[0]) != null)
                {
                    Core.WheelMenu.WheelMenu wheel = new Core.WheelMenu.WheelMenu(PrepareDataSource(sender, RPEntityManager.GetVehicle((NetHandle)arguments[0])), sender);
                    sender.SetData("WheelMenu", wheel);
                }   
            }
            else if (eventName == "UseWheelMenuItem")
            {
                //args[0] to nazwa opcji
                Core.WheelMenu.WheelMenu wheel = (Core.WheelMenu.WheelMenu)sender.GetData("WheelMenu");
                wheel.WheelMenuItems.First(x => x.Name == (string)arguments[0]).Use();
                wheel.Dispose();
            }
        }

        private List<WheelMenuItem> PrepareDataSource(GTANetworkServer.Client sender, object target, params object[] args)
        {
            List<WheelMenuItem> menuItems = new List<WheelMenuItem>();
            if (target is GTANetworkServer.Client)
            {
                
            }
            else if (target is VehicleController)
            {
                VehicleController vehicle = (VehicleController) target;
                if (RPVehicles.GetVehicleDoorCount((VehicleHash)vehicle.Vehicle.model) >= 4)
                {
                    menuItems.Add(new WheelMenuItem("Maska", sender, target, (s, e) => RPVehicles.ChangeDoorState(s, ((VehicleController)e).Vehicle.handle, (int)EnumsExtensions.Doors.Hood)));
                    menuItems.Add(new WheelMenuItem("Bagaznik", sender, target, (s, e) => RPVehicles.ChangeDoorState(s, ((VehicleController)e).Vehicle.handle, (int)EnumsExtensions.Doors.Trunk)));
                }
                if (vehicle.VehicleData.Character == sender.GetAccountController().CharacterController.Character)
                {
                    menuItems.Add(new WheelMenuItem("Zamek", sender, null, (s, e) => RPVehicles.ChangePlayerVehicleLockState(s)));
                }
                menuItems.Add(new WheelMenuItem("Rejestracja", sender, target, (s, e) => RPVehicles.ShowVehiclesInformation(s, ((VehicleController)e).VehicleData, true)));
            }
            return menuItems;
        }
    }
}