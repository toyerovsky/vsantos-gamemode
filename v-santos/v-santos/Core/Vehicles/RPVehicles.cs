using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using GTANetworkServer.Constant;
using GTANetworkShared;
using Newtonsoft.Json;
using Serverside.Controllers;
using Serverside.Core.Extensions;
using Serverside.Items;

namespace Serverside.Core.Vehicles
{
    enum Doors
    {
        Hood,
        Trunk,
        Back,
        Back2
    }

    class RPVehicles : Script
    {
        private API Api => API.shared;

        public RPVehicles()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onResourceStop += API_onResourceStop;
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("RPVehicles zostało uruchomione pomyslnie!");
        }

        private void API_onResourceStop()
        {
            foreach (var v in Api.getAllVehicles())
            {
                if (v.GetVehicleController() != null)
                {
                    var controller = v.GetVehicleController();
                    controller.Dispose();
                }
            }
        }

        #region Komendy
        [Command("fastcar", "~y~UŻYJ: ~w~ /fastcar model moc moment_obr (podajac 0 ignorujesz parametr)")]
        public void CreateFastCar(Client sender, VehicleHash model, float moc, float torque)
        {
            var rot = API.getEntityRotation(sender.handle);
            var veh = Api.createVehicle(model, sender.position, new Vector3(0, 0, rot.Z), new Random().Next(50), new Random().Next(50));

            Api.setVehicleEngineTorqueMultiplier(veh, torque);
            Api.setVehicleEnginePowerMultiplier(veh, moc);

            Api.setPlayerIntoVehicle(sender, veh, -1);
        }

        [Command("v", "~y~UŻYJ: ~w~ /v (z)")]
        public void ShowVehiclesList(Client sender, string trigger = null)
        {
            var player = sender.GetAccountController();
            if (trigger == null)
            {
                if (API.isPlayerInAnyVehicle(sender))
                {
                    var controller = Api.getPlayerVehicle(player.Client).GetVehicleController();
                    if (controller == null) return;

                    string tuningJson = JsonConvert.SerializeObject(controller.VehicleData.Tuning);
                    API.triggerClientEvent(sender, "OnPlayerManageVehicle", tuningJson);
                }
                else
                {
                    string vehiclesJson = JsonConvert.SerializeObject(player.CharacterController.Character.Vehicle);
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

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] args)
        {
            if (eventName == "OnPlayerSelectedVehicle")
            {
                sender.SetData("SelectedVehicleID", Convert.ToInt64(args[0]));
            }

            else if (eventName == "OnPlayerSpawnVehicle")
            {
                var userVehicle = sender.GetAccountController().CharacterController.Character.Vehicle.Single(v => v.Id == sender.GetData("SelectedVehicleID"));
                sender.ResetData("SelectedVehicleID");
                VehicleHash userVehicleHash = (VehicleHash)userVehicle.VehicleHash;
                if (Api.getAllVehicles().Any(v => v.GetVehicleController().VehicleData.Id == userVehicle.Id))
                {
                    //unspawn
                    var vehicle = Api.getAllVehicles().Single(v => v.GetVehicleController().VehicleData.Id == userVehicle.Id).GetVehicleController();
                    userVehicle.Health = Convert.ToInt32(Api.getVehicleHealth(vehicle.Vehicle));
                    userVehicle.Door1Damage = Api.isVehicleDoorBroken(vehicle.Vehicle, 1);
                    userVehicle.Door2Damage = Api.isVehicleDoorBroken(vehicle.Vehicle, 2);
                    userVehicle.Door3Damage = Api.isVehicleDoorBroken(vehicle.Vehicle, 3);
                    userVehicle.Door4Damage = Api.isVehicleDoorBroken(vehicle.Vehicle, 4);
                    userVehicle.Window1Damage = Api.isVehicleWindowBroken(vehicle.Vehicle, 1);
                    userVehicle.Window2Damage = Api.isVehicleWindowBroken(vehicle.Vehicle, 2);
                    userVehicle.Window3Damage = Api.isVehicleWindowBroken(vehicle.Vehicle, 3);
                    userVehicle.Window4Damage = Api.isVehicleWindowBroken(vehicle.Vehicle, 4);

                    vehicle.Dispose();

                    Api.sendNotificationToPlayer(sender, "Pojazd został usnpawnowany!");
                }
                else
                {
                    VehicleController controller = new VehicleController(userVehicle);

                    float engineMultipier = 0f;
                    float torqueMultipier = 0f;

                    foreach (var tuning in controller.VehicleData.Tuning)
                    {
                        if ((ItemType)tuning.ItemType == ItemType.Tuning)
                        {
                            if (tuning.FirstParameter != null && (TuningType)tuning.FirstParameter == TuningType.Speed)
                            {
                                if (tuning.SecondParameter != null) engineMultipier += (float)tuning.SecondParameter;
                                if (tuning.ThirdParameter != null) torqueMultipier += (float)tuning.ThirdParameter;
                            }
                        }
                    }

                    Api.setVehicleEnginePowerMultiplier(controller.Vehicle, engineMultipier);
                    Api.setVehicleEngineTorqueMultiplier(controller.Vehicle, torqueMultipier);
                    Api.setVehicleEngineStatus(controller.Vehicle, false);
                    Api.setVehicleLocked(controller.Vehicle, true);
                    Api.breakVehicleDoor(controller.Vehicle, 1, userVehicle.Door1Damage);
                    Api.breakVehicleDoor(controller.Vehicle, 2, userVehicle.Door2Damage);
                    Api.breakVehicleDoor(controller.Vehicle, 3, userVehicle.Door3Damage);
                    Api.breakVehicleDoor(controller.Vehicle, 4, userVehicle.Door4Damage);
                    Api.breakVehicleWindow(controller.Vehicle, 1, userVehicle.Window1Damage);
                    Api.breakVehicleWindow(controller.Vehicle, 2, userVehicle.Window2Damage);
                    Api.breakVehicleWindow(controller.Vehicle, 3, userVehicle.Window3Damage);
                    Api.breakVehicleWindow(controller.Vehicle, 4, userVehicle.Window4Damage);
                    //Api.setVehiclePrimaryColor(controller.Vehicle, userVehicle.PrimaryColor);
                    //Api.setVehicleSecondaryColor(controller.Vehicle, userVehicle.SecondaryColor);
                    Api.setVehicleHealth(controller.Vehicle, (float)userVehicle.Health);
                    Api.setVehicleWheelType(controller.Vehicle, userVehicle.WheelType);
                    Api.setVehicleWheelColor(controller.Vehicle, userVehicle.WheelColor);
                 
                    Api.sendNotificationToPlayer(sender, "Pojazd został zespawnowany!");
                }
            }
            else if (eventName == "OnPlayerParkVehicle")
            {
                if (API.getPlayerVehicle(sender).GetVehicleController() == null) return;

                var controller = API.getPlayerVehicle(sender).GetVehicleController();
                controller.ChangeSpawnPosition();
                sender.Notify("Pojazd został zaparkowany.");

            }
            else if (eventName == "OnPlayerInformationsVehicle")
            {
                var player = sender.GetAccountController();

                if (player.CharacterController.Character.Vehicle.Any(v => v.Id == sender.GetData("SelectedVehicleID")))
                {
                    ShowVehiclesInformation(player.Client, player.CharacterController.Character.Vehicle.Single(v => v.Id == sender.GetData("SelectedVehicleID")));
                }

            }
            else if (eventName == "OnPlayerInformationsInVehicle")
            {
                var vehicle = API.shared.getPlayerVehicle(sender).GetVehicleController();
                if (vehicle == null) return;

                float enginePower = (float)((vehicle.VehicleData.EnginePowerMultipler - 1.0) * 20.0 + 80);


                //TODO zmienić to
                //sender.Notify("Nazwa pojazdu: " + vehicle.VehicleData.Name +
                //              "\nRejestracja pojazdu: " + vehicle.VehicleData.NumberPlate+ "\nMoc silnika: " + enginePower + " KM");

            }
            else if (eventName == "OnPlayerChangeLockVehicle")
            {
                var vehicle = Api.getPlayerVehicle(sender);
                Api.sendNotificationToPlayer(sender, Api.getVehicleLocked(vehicle) ? "Twój pojazd został otwarty." : "Twój pojazd został zamkniety.");
                Api.setVehicleLocked(vehicle, !Api.getVehicleLocked(vehicle));
            }
            else if (eventName == "OnPlayerEngineStateChangeVehicle")
            {
                var vehicle = Api.getPlayerVehicle(sender);
                Api.sendNotificationToPlayer(sender, Api.getVehicleEngineStatus(vehicle) ? "Pojazd został zgaszony." : "Pojazd został uruchomiony.");
                Api.setVehicleEngineStatus(vehicle, !Api.getVehicleEngineStatus(vehicle));
            }
        }

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
                
                //Co jest z polem name?
                //sender.Notify($"Nazwa pojazdu: {data.Name} \nRejestracja pojazdu: {data.NumberPlate} \nMoc silnika: {enginePower}KM");

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
            var controller = player.GetAccountController();

            //Wybieramy wszystkie pojazdy które należą do gracza i są oddalone o 10F
            var vehicles = API.shared.getAllVehicles()
                .Where(v => v.GetVehicleController().VehicleData.Character.Id == controller.CharacterController.Character.Id &&
                v.GetVehicleController().Vehicle.position.DistanceTo(player.position) < 10);

            //Jeśli jakiś pomysłowy gracz zapragnie postawić 2 pojazdy obok siebie, żeby sprawdzić działanie to zamknie mu obydwa
            foreach (var vehicle in vehicles)
            {
                if (API.shared.getEntityPosition(vehicle).DistanceTo(player.position) <= 7)
                {
                    player.Notify(!API.shared.getVehicleLocked(vehicle) ? "Twój pojazd został zamknięty." : "Twój pojazd został otwarty.");
                    API.shared.setVehicleLocked(vehicle, !API.shared.getVehicleLocked(vehicle));
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
