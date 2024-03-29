﻿/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using System.Collections;
using System.Linq;
using Newtonsoft.Json;
using Serverside.Controllers;
using Serverside.Core.Extensions;
using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;

namespace Serverside.Core.Vehicles
{
    public sealed class RPVehicles : Script
    {
        
        public RPVehicles()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onVehicleDeath += API_onVehicleExplode;
            API.onPlayerEnterVehicle += API_onPlayerEnterVehicle;
            //API.onPlayerExitVehicle += API_onPlayerExitVehicle;
            //API.onVehicleDoorBreak += API_onVehicleDoorBreak;
            //API.onVehicleTyreBurst += API_onVehicleTyreBurst;
            //API.onVehicleWindowSmash += API_onVehicleWindowSmash;
            API.onVehicleHealthChange += API_onVehicleHealthChange;
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("RPVehicles zostało uruchomione pomyslnie!");
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] args)
        {
            if (eventName == "OnPlayerSelectedVehicle")
            {
                sender.SetData("SelectedVehicleID", Convert.ToInt64(args[0]));
            }

            else if (eventName == "OnPlayerSpawnVehicle")
            {
                AccountController ac = sender.GetAccountController();

                VehicleController userVehicleController = RPEntityManager.GetVehicle((long)sender.GetData("SelectedVehicleID"));
                if (userVehicleController != null)
                {
                    //unspawn
                    userVehicleController.Dispose();
                    sender.Notify("Pojazd został schowany w garażu!");
                }
                else
                {
                    var userVehicle = sender.GetAccountController().CharacterController.Character.Vehicles
                        .Single(v => v.Id == (long) sender.GetData("SelectedVehicleID"));
                    VehicleController controller = new VehicleController(userVehicle);

                    int blip = 225;
                    if (controller.Vehicle.Class == (int) VehicleClass.Motorcycles) blip = 226;
                    if (controller.Vehicle.Class == (int) VehicleClass.Utility ||
                        controller.Vehicle.Class == (int) VehicleClass.Service ||
                        controller.Vehicle.Class == (int) VehicleClass.Industrial) blip = 447;
                    if (controller.Vehicle.Class == (int) VehicleClass.Heli) blip = 481;
                    if (controller.Vehicle.Class == (int) VehicleClass.Planes) blip = 307;

                    sender.triggerEvent("DrawVehicleComponents", controller.Vehicle.position, blip, 24);

                    sender.Notify("Pojazd został wyprowadzony z garażu!");
                }
                sender.ResetData("SelectedVehicleID");
            }
            else if (eventName == "OnPlayerParkVehicle")
            {
                if (RPEntityManager.GetVehicle(API.getPlayerVehicle(sender)) == null) return;

                var controller = RPEntityManager.GetVehicle(API.getPlayerVehicle(sender));
                controller.ChangeSpawnPosition();
                sender.Notify("Pojazd został zaparkowany.");

            }
            else if (eventName == "OnPlayerInformationsVehicle")
            {
                var player = sender.GetAccountController();

                if (player.CharacterController.Character.Vehicles.Any(v => v.Id == sender.GetData("SelectedVehicleID")))
                {
                    ShowVehiclesInformation(sender, player.CharacterController.Character.Vehicles.Single(v => v.Id == sender.GetData("SelectedVehicleID")));
                }

            }
            else if (eventName == "OnPlayerInformationsInVehicle")
            {
                var vehicle = RPEntityManager.GetVehicle(API.shared.getPlayerVehicle(sender));
                if (vehicle == null) return;

                float enginePower = (float)((vehicle.VehicleData.EnginePowerMultipler - 1.0) * 20.0 + 80);

                sender.Notify("Nazwa pojazdu: " + vehicle.VehicleData.VehicleHash.ToString() +
                              "\nRejestracja pojazdu: " + vehicle.VehicleData.NumberPlate + "\nMoc silnika: " + enginePower + " KM");

            }
            else if (eventName == "OnPlayerChangeLockVehicle")
            {
                var vehicle = API.getPlayerVehicle(sender);
                sender.Notify(API.getVehicleLocked(vehicle) ? "Twój pojazd został otwarty." : "Twój pojazd został zamkniety.");
                API.setVehicleLocked(vehicle, !API.getVehicleLocked(vehicle));
            }
            else if (eventName == "OnPlayerEngineStateChangeVehicle")
            {
                var vehicle = API.getPlayerVehicle(sender);
                API.sendNotificationToPlayer(sender, API.getVehicleEngineStatus(vehicle) ? "Pojazd został zgaszony." : "Pojazd został uruchomiony.");
                API.setVehicleEngineStatus(vehicle, !API.getVehicleEngineStatus(vehicle));
            }
        }

        public void API_onVehicleExplode(NetHandle vehicle)
        {
            //throw new NotImplementedException("API_onVehicleExplode do zrobienia");
        }

        private void API_onPlayerEnterVehicle(Client player, NetHandle vehicle)
        {
            AccountController account = player.GetAccountController();
            if (account == null) return;

            VehicleController vc = RPEntityManager.GetVehicle(vehicle);
            if (vc != null)
            {
                if (vc.VehicleData.Character == account.CharacterController.Character)
                {
                    API.sendNotificationToPlayer(player, "Wsiadłeś do swojego pojazdu.");
                    player.triggerEvent("DisposeVehicleComponents");
                }
            }
            //API.triggerClientEvent(player, "show_vehicle_hud"); // sprawdzanie po stronie klienta
        }

        //private void API_onPlayerExitVehicle(Client sender, NetHandle vehicle)
        //{
        //    //API.triggerClientEvent(player, "hide_vehicle_hud"); // sprawdzanie po stronie klienta
        //}

        //private void API_onVehicleDoorBreak(NetHandle vehicle, int index)
        //{
        //    //throw new NotImplementedException("OnVehicleTyreBurst do zrobienia");
        //}

        //private void API_onVehicleTyreBurst(NetHandle vehicle, int index)
        //{
        //    //throw new NotImplementedException("OnVehicleTyreBurst do zrobienia");
        //}

        //private void API_onVehicleWindowSmash(NetHandle vehicle, int index)
        //{
        //    //throw new NotImplementedException("OnVehicleWindowSmash do zrobienia");
        //}

        private void API_onVehicleHealthChange(NetHandle entity, float oldValue)
        {
            if (API.getEntityType(entity) == EntityType.Player)
            {
                Client client = API.getPlayerFromHandle(entity);
                if (API.isPlayerInAnyVehicle(client))
                {

                    double t = Math.Floor(Math.Max(49, oldValue - client.vehicle.health) / 50);
                    if (t > 0)
                        APIExtensions.PopRandomsTyres(client.vehicle, int.Parse(t.ToString()));

                    float vehHealth = API.getVehicleHealth(client.vehicle);
                    float damageTaken = Math.Max(0, oldValue - vehHealth);
                    float healthToSet = vehHealth - damageTaken;
                    API.setVehicleHealth(client.vehicle, healthToSet);
                }
                else
                {
                    //var test = API.fetchNativeFromPlayer<bool>(client, Hash.GET_PLAYER_TARGET_ENTITY); // CHUJ NIE DZIAŁA BO DEVY Z GTA:N TO DEBILE :D
                    //Vehicle nearest = APIExtensions.GetNearestVehicle(client.position); // nie działa tak jak powinno... <0ms, ~50ticks
                    //Vehicle nearest = APIExtensions.GetVehicleHasBeenDamagedByPlayer(client); // działa ale jest pewnie wolniejsze..... ~100ms
                    //if (nearest != null)
                    //{
                    //    if (nearest.health > 50)
                    //    {
                    //        double t = Math.Floor((oldValue - nearest.health) / 50);
                    //        API.sendNotificationToPlayer(client, t.ToString());
                    //        if (t > 0)
                    //            APIExtensions.PopRandomsTyres(nearest, int.Parse(t.ToString()));
                    //    }

                    //    float veh_health = API.getVehicleHealth(nearest);
                    //    float damage_taken = oldValue - veh_health;
                    //    float health_to_set = veh_health - damage_taken;
                    //    API.sendChatMessageToAll(client.socialClubName + " OLD: " + oldValue.ToString() + " | NEW:" + veh_health + " | TAKEN: " + damage_taken + " | SET: " + health_to_set);
                    //    if (veh_health > 0 && health_to_set > 0)
                    //    {
                    //        API.setVehicleHealth(nearest, health_to_set);
                    //    }
                    //}
                }
                //Ten event zwraca w parametrze entity osobe która zadaje damage(?). 
                //Co z tego, że mogę sprawdzić kto jest atakującym i jaka była poprzedna wartość health w parametrze oldValue, jak nie moge określić któremu pojazdowi ten damage został zadany.
                //Określanie pojazdu na podstawie dystansu względem atakującego nie sprawdza się w przypadku bronii palnej gdzie ktoś stojąc przy pojeździe strzela do innego pojazdu który stoi dalej.
                //Paranoja :C
                //działa na natywach ale za wolno....
            }
        }

        #region Komendy
        [Command("v", "~y~UŻYJ: ~w~ /v (z)")]
        public void ShowVehiclesList(Client sender, string trigger = null)
        {
            AccountController player = sender.GetAccountController();
            if (trigger == null)
            {
                if (API.isPlayerInAnyVehicle(sender))
                {
                    VehicleController controller = RPEntityManager.GetVehicle(API.getPlayerVehicle(player.Client));
                    if (controller == null) return;

                    string tuningJson = JsonConvert.SerializeObject(controller.VehicleData.Tunings.Select(i => new
                    {
                        i.Name
                    }));
                    string itemsInVehicleJson = JsonConvert.SerializeObject(controller.VehicleData.ItemsInVehicle.Select(i => new
                    {
                        i.Name
                    }));
                    string playerGroups = JsonConvert.SerializeObject(RPEntityManager.GetPlayerGroups(sender.GetAccountController())
                        .Select(g => new
                        {
                            g.GroupData.Name
                        }));

                    API.triggerClientEvent(sender, "OnPlayerManageVehicle", tuningJson, itemsInVehicleJson, playerGroups);
                }
                else
                {
                    string vehiclesJson = JsonConvert.SerializeObject(player.CharacterController.Character.Vehicles.Select(v => new
                    {
                        Id = v.Id,
                        Name = v.VehicleHash.ToString(),
                        Plate = v.NumberPlate
                    }));

                    API.triggerClientEvent(sender, "OnPlayerShowVehicles", vehiclesJson);
                }
            }
            //v zamknij
            else if (trigger == "z")
            {
                ChangePlayerVehicleLockState(sender);
            }
        }
        #endregion

        public static void ChangeDoorState(Client sender, NetHandle vehicle, int doorId)
        {
            if (API.shared.isVehicleDoorBroken(vehicle, doorId))
            {
                RPChat.SendMessageToPlayer(sender, "Drzwi wyglądają na zepsute, nie chcą nawet drgnąć.", ChatMessageType.ServerDo);
                return;
            }
            API.shared.setVehicleDoorState(vehicle, doorId, !API.shared.getVehicleDoorState(vehicle, doorId));
        }

        //Nie dajemy tutaj VehicleController, żeby gracz mógł sprawdzić też informacje odspawnowanego auta
        public static void ShowVehiclesInformation(Client sender, Database.Models.Vehicle data, bool shortInfo = false)
        {
            if (!shortInfo && data.Character.Id == sender.GetAccountController().CharacterController.Character.Id)
            {
                float enginePower = (float)((data.EnginePowerMultipler - 1.0) * 20.0 + 80);

                sender.Notify($"Nazwa pojazdu: {data.VehicleHash.ToString()} \nRejestracja pojazdu: {data.NumberPlate} \nMoc silnika: {enginePower}KM");
            }
            else
            {
                sender.Notify("\nRejestracja pojazdu: " + data.NumberPlate);
            }
        }

        /// <summary>
        /// Metoda zamyka/otwiera pojazd należący do gracza który jest blisko niego
        /// </summary>
        /// <param name="player"></param>
        public static void ChangePlayerVehicleLockState(Client player)
        {
            AccountController controller = player.GetAccountController();

            //Wybieramy wszystkie pojazdy które należą do gracza i są oddalone o 10F
            //var vehicles = API.shared.getAllVehicles()
            //    .Where(v => v.GetVehicleController().VehicleData.Character.Id == controller.CharacterController.Character.Id &&
            //    v.GetVehicleController().Vehicle.position.DistanceTo(player.position) < 10);
            List<VehicleController> vehicles = RPEntityManager.GetCharacterVehicles(controller.CharacterController).Where(x => x.Vehicle.position.DistanceTo(player.position) < 10).ToList();

            //Jeśli jakiś pomysłowy gracz zapragnie postawić 2 pojazdy obok siebie, żeby sprawdzić działanie to zamknie mu obydwa
            foreach (VehicleController vehicle in vehicles)
            {
                if (API.shared.getEntityPosition(vehicle.Vehicle).DistanceTo(player.position) <= 7)
                {
                    player.Notify(!API.shared.getVehicleLocked(vehicle.Vehicle) ? "Twój pojazd został zamknięty." : "Twój pojazd został otwarty.");
                    API.shared.setVehicleLocked(vehicle.Vehicle, !API.shared.getVehicleLocked(vehicle.Vehicle));
                }
            }
        }

        public static int GetVehicleDoorCount(VehicleHash vehicle)
        {
            if (API.shared.getVehicleClass(vehicle).Equals(0) || API.shared.getVehicleClass(vehicle).Equals(20) || API.shared.getVehicleClass(vehicle).Equals(3) || API.shared.getVehicleClass(vehicle).Equals(7) || API.shared.getVehicleClass(vehicle).Equals(20) || API.shared.getVehicleClass(vehicle).Equals(15) || API.shared.getVehicleClass(vehicle).Equals(19) || API.shared.getVehicleClass(vehicle).Equals(10) || API.shared.getVehicleClass(vehicle).Equals(4) || API.shared.getVehicleClass(vehicle).Equals(5) || API.shared.getVehicleClass(vehicle).Equals(6)) return 4;
            if (API.shared.getVehicleClass(vehicle).Equals(13) || API.shared.getVehicleClass(vehicle).Equals(8) || API.shared.getVehicleClass(vehicle).Equals(14)) return 0;
            return 6;
        }

    }
}
