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
using GrandTheftMultiplayer.Shared;
using Serverside.Controllers;
using Serverside.Core.Extensions;
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

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
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
                    WheelMenu wheel = new WheelMenu(PrepareDataSource(sender, RPEntityManager.GetVehicle((NetHandle)arguments[0])), sender);
                    sender.SetData("WheelMenu", wheel);
                }   
            }
            else if (eventName == "UseWheelMenuItem")
            {
                //args[0] to nazwa opcji
                WheelMenu wheel = (WheelMenu)sender.GetData("WheelMenu");
                wheel.WheelMenuItems.First(x => x.Name == (string)arguments[0]).Use();
                wheel.Dispose();
            }
        }

        private List<WheelMenuItem> PrepareDataSource(Client sender, object target, params object[] args)
        {
            List<WheelMenuItem> menuItems = new List<WheelMenuItem>();
            if (target is Client)
            {
                
            }
            else if (target is VehicleController)
            {
                VehicleController vehicle = (VehicleController) target;
                if (RPVehicles.GetVehicleDoorCount((VehicleHash)vehicle.Vehicle.model) >= 4)
                {
                    menuItems.Add(new WheelMenuItem("Maska", sender, target, (s, e) => RPVehicles.ChangeDoorState(s, ((VehicleController)e).Vehicle.handle, (int)Doors.Hood)));
                    menuItems.Add(new WheelMenuItem("Bagaznik", sender, target, (s, e) => RPVehicles.ChangeDoorState(s, ((VehicleController)e).Vehicle.handle, (int)Doors.Trunk)));
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