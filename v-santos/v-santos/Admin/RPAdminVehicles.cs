/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using Serverside.Core;
using Serverside.Core.Extensions;

namespace Serverside.Admin
{
    public class RPAdminVehicles : Script
    {

        public RPAdminVehicles()
        {
            API.onResourceStart += OnResourceStart;
        }

        private void OnResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPAdminVehicles] Uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
        }

        [Command("kolor", "~y~ UŻYJ ~w~ /kolor [hexPodstawowy] [hexDodatkowy]")]
        public void ChangeVehicleColor(Client sender, string primaryHex, string secondaryHex, long vehicleId = -1)
        {
            if (!sender.isInVehicle && vehicleId == -1)
            {
                sender.Notify("Wsiądź do pojazu lub podaj Id aby ustawić jego kolor.");
                return;
            }

            var vehicle = sender.isInVehicle
                ? sender.vehicle.GetVehicleController()
                : RPEntityManager.GetVehicle(vehicleId);

            Color primaryColor;
            Color secondaryColor;
            try
            {
                primaryColor = primaryHex.ToColor();
                secondaryColor = secondaryHex.ToColor();
            }
            catch (Exception e)
            {
                sender.Notify("Wprowadzony kolor jest nieprawidłowy.");
                APIExtensions.ConsoleOutput("[Error] Nieprawidłowy kolor [RPAdminVehicles]", ConsoleColor.Red);
                APIExtensions.ConsoleOutput(e.Message, ConsoleColor.Red);
                return;
            }

            vehicle?.ChangeColor(primaryColor, secondaryColor);
        }

        [Command("napraw", "~y~ UŻYJ ~w~ /kolor [podstawowy] (dodatkowy)")]
        public void RepairVehicle(Client sender, long vehicleId = -1)
        {
            if (!sender.isInVehicle && vehicleId == -1)
            {
                sender.Notify("Wsiądź do pojazu lub podaj Id aby ustawić jego kolor.");
                return;
            }

            var vehicle = sender.isInVehicle
                ? sender.vehicle.GetVehicleController()
                : RPEntityManager.GetVehicle(vehicleId);

            vehicle?.Repair();
        }
    }
}