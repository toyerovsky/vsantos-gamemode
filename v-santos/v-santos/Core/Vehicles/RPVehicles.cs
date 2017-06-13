using System;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using Newtonsoft.Json;
using Serverside.Controllers;
using Serverside.Core.Extensions;
using System.Collections.Generic;

namespace Serverside.Core.Vehicles
{
    public sealed class RPVehicles : Script
    {
        private API Api => API.shared;

        public RPVehicles()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onVehicleDeath += API_onVehicleExplode;
            API.onPlayerEnterVehicle += API_onPlayerEnterVehicle;
            API.onPlayerExitVehicle += API_onPlayerExitVehicle;
            API.onVehicleDoorBreak += API_onVehicleDoorBreak;
            API.onVehicleTyreBurst += API_onVehicleTyreBurst;
            API.onVehicleWindowSmash += API_onVehicleWindowSmash;
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
                API.sendChatMessageToPlayer(sender, (long)sender.GetData("SelectedVehicleID")+"");
                VehicleController userVehicleController = RPEntityManager.GetVehicle((long)sender.GetData("SelectedVehicleID"));
                if (userVehicleController != null)
                {
                    //unspawn
                    //userVehicleController.Save(); // w Dispose jest Save ......
                    userVehicleController.Dispose();
                    Api.sendNotificationToPlayer(sender, "Pojazd został schowany w garażu!");
                }
                else
                {
                    var userVehicle = VehicleController.GetVehicleData(ac.CharacterController, (long)sender.GetData("SelectedVehicleID"));
                    VehicleController controller = new VehicleController(userVehicle);

                    //float engineMultipier = 0f;
                    //float torqueMultipier = 0f;

                    //foreach (var tuning in controller.VehicleData.Tuning)
                    //{
                    //    if ((ItemType)tuning.ItemType == ItemType.Tuning)
                    //    {
                    //        if (tuning.FirstParameter != null && (TuningType)tuning.FirstParameter == TuningType.Speed)
                    //        {
                    //            if (tuning.SecondParameter != null) engineMultipier += (float)tuning.SecondParameter;
                    //            if (tuning.ThirdParameter != null) torqueMultipier += (float)tuning.ThirdParameter;
                    //        }
                    //    }
                    //}

                    //Api.setVehicleEnginePowerMultiplier(controller.Vehicle, engineMultipier);
                    //Api.setVehicleEngineTorqueMultiplier(controller.Vehicle, torqueMultipier);

                    //var primaryColor = userVehicle.PrimaryColor.ToColor();
                    //Api.setVehicleCustomPrimaryColor(controller.Vehicle, primaryColor.red, primaryColor.green, primaryColor.blue);

                    //var secondaryColor = userVehicle.SecondaryColor.ToColor();
                    //Api.setVehicleCustomSecondaryColor(controller.Vehicle, secondaryColor.red, secondaryColor.green, secondaryColor.blue);

                    //Api.setVehicleHealth(controller.Vehicle, (float)userVehicle.Health);
                    //Api.setVehicleWheelType(controller.Vehicle, userVehicle.WheelType);
                    //Api.setVehicleWheelColor(controller.Vehicle, userVehicle.WheelColor);

                    //new VehicleController(ac.CharacterController.Character.Vehicle.Where(x => x.Id == userVehicle.Id).First());

                    Api.sendNotificationToPlayer(sender, "Pojazd został wyprowadzony z garażu!");
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
                    ShowVehiclesInformation(player.Client, player.CharacterController.Character.Vehicles.Single(v => v.Id == sender.GetData("SelectedVehicleID")));
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
                }
            }
            //API.triggerClientEvent(player, "show_vehicle_hud"); // sprawdzanie po stronie klienta
        }

        private void API_onPlayerExitVehicle(Client player, NetHandle vehicle)
        {
            //API.triggerClientEvent(player, "hide_vehicle_hud"); // sprawdzanie po stronie klienta
        }

        private void API_onVehicleDoorBreak(NetHandle vehicle, int index)
        {
            //throw new NotImplementedException("OnVehicleTyreBurst do zrobienia");
        }

        private void API_onVehicleTyreBurst(NetHandle vehicle, int index)
        {
            //throw new NotImplementedException("OnVehicleTyreBurst do zrobienia");
        }

        private void API_onVehicleWindowSmash(NetHandle vehicle, int index)
        {
            //throw new NotImplementedException("OnVehicleWindowSmash do zrobienia");
        }

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
                    VehicleController controller = RPEntityManager.GetVehicle(Api.getPlayerVehicle(player.Client));
                    if (controller == null) return;

                    string tuningJson = JsonConvert.SerializeObject(controller.VehicleData.Tunings);
                    API.triggerClientEvent(sender, "OnPlayerManageVehicle", tuningJson);
                }
                else
                {
                    string vehiclesJson = JsonConvert.SerializeObject(VehicleController.GetVehiclesData(player.CharacterController).Select(v => new
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
