﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Controllers;
using Serverside.Core.Login;
using Serverside.Database;
using Serverside.Groups;
using Serverside.Groups.Base;
using Serverside.Core.Extensions;
using Serverside.Core.Vehicles;

namespace Serverside.Core
{
    public class RPCore : Script
    {
        public RPCore()
        {
            API.onResourceStart += API_onResourceStart;
            API.onResourceStop += API_onResourceStop;
            API.onPlayerBeginConnect += API_onPlayerBeginConnect;
            API.onPlayerConnected += API_onPlayerConnectedHandler;
            API.onPlayerFinishedDownload += API_onPlayerFinishedDownload;
            API.onPlayerDisconnected += API_onPlayerDisconnectedHandler;        
        }

        private void API_onResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPCore] Uruchomione pomyslnie!", ConsoleColor.DarkMagenta);
            //API.getSetting<string>("database_server"), API.getSetting<string>("database_user"), API.getSetting<string>("database_password"), API.getSetting<string>("database_database")
            ContextFactory.SetConnectionParameters("v-santos.pl", "srv", "WL8oTnufAAEFgoIt", "rp"); // NIE WYMAGANE
            RPEntityManager.Init();
            ContextFactory.Instance.SaveChanges();
        }
        private void API_onResourceStop()
        {
            //FDb = null;
            Task dbStop = Task.Run(() =>
            {
                foreach (var p in API.getAllPlayers().Where(x => x.GetAccountController().CharacterController != null))
                {
                    //Zmiana postaci pola Online w postaci po wyłączeniu serwera dla graczy którzy byli online
                    p.GetAccountController().CharacterController.Character.Online = false;

                    //Zmiana w przedmiocie pola CurrentlyInUse na false
                    foreach (var i in p.GetAccountController().CharacterController.Character.Item.Where(i => i.CurrentlyInUse == true).ToList())
                    {
                        i.CurrentlyInUse = false;
                    }
                }
                RPEntityManager.Save(true);
                ContextFactory.Instance.Dispose();
            });
            dbStop.Wait();
        }
        private void API_onPlayerBeginConnect(Client player, CancelEventArgs cancelConnection)
        {
            APIExtensions.ConsoleOutput("PlayerBeginConnect: " + player.socialClubName, ConsoleColor.Blue);
            if (!player.isCEFenabled)
            {
                cancelConnection.Reason = "Aby połączyć się z serwerem musisz włączyć obsługe przeglądarki CEF.";
                cancelConnection.Cancel = true;
            }
        }
        private void API_onPlayerConnectedHandler(Client player)
        {
            APIExtensions.ConsoleOutput("PlayerConnected: " + player.socialClubName, ConsoleColor.Blue);
            if (AccountController.IsAccountBanned(player))
            {
                player.kick("~r~Jesteś zbanowany. Życzymy miłego dnia! :)");
            }
        }
        private void API_onPlayerFinishedDownload(Client player)
        {
            APIExtensions.ConsoleOutput("PlayerFinishedDownload: " + player.socialClubName, ConsoleColor.Blue);
            RPLogin.LoginMenu(player);
        }
        private void API_onPlayerDisconnectedHandler(Client player, string reason)
        {
            APIExtensions.ConsoleOutput("PlayerDisconnected: " + player.socialClubName, ConsoleColor.Blue);
            AccountController account = player.GetAccountController();
            if (account == null) return;
            RPLogin.LogOut(account);
        }
        private void Client_OnPlayerDimensionChanged(object player, DimensionChangeEventArgs e)
        {
            AccountController account = e.Player.GetAccountController();
            account.CharacterController.Character.CurrentDimension = e.CurrentDimension;
            account.CharacterController.Save();
        }

        

        [Command("q")]
        public static void Quit(Client player)
        {
            API.shared.kickPlayer(player);
        }

        #region DEBUG COMMANDS

        [Command("deb1", GreedyArg = true, SensitiveInfo = true, Alias = "avc")]
        public static void Deb1(Client sender, string toggle = "true")
        {
            if (bool.Parse(toggle))
            {
                List<Database.Models.Vehicle> v = sender.GetAccountController().CharacterController.Character.Vehicle.ToList();
                foreach (Database.Models.Vehicle veh in v)
                {
                    API.shared.consoleOutput(GTANetworkServer.Constant.LogCat.Debug, veh.VehicleHash.ToString() + " | " + veh.NumberPlate + " | " + veh.Milage.ToString());
                }
            } else
            {
                Database.Models.Character ch = sender.GetAccountController().CharacterController.Character;
                List<Database.Models.Vehicle> v = ContextFactory.Instance.Vehicles.Where(x => x.Character == ch).ToList();
                foreach (Database.Models.Vehicle veh in v)
                {
                    API.shared.consoleOutput(GTANetworkServer.Constant.LogCat.Debug, veh.VehicleHash.ToString() + " | " + veh.NumberPlate + " | " + veh.Milage.ToString());
                }
            }
        }

        [Command("addvc", GreedyArg = true, SensitiveInfo = true, Alias = "avc")]
        public static void AddVehicle(Client sender, string model, string plate)
        {
            AccountController ac = sender.GetAccountController();
            new VehicleController(new FullPosition(sender.position, sender.rotation), API.shared.vehicleNameToModel(model), plate, 0, int.Parse(ac.AccountId.ToString()), "#FF0000FF".ToColor(), "#00FF00FF".ToColor(), 1, 1, ac.CharacterController.Character);
        }

        [Command("loadvc", GreedyArg = true, SensitiveInfo = true)]
        public static void LoadVehicle(Client sender, string id)
        {
            AccountController ac = sender.GetAccountController();
            new VehicleController(ac.CharacterController.Character.Vehicle.Where(x => x.Id == int.Parse(id)).First());
        }

        [Command("savevc", GreedyArg = true, SensitiveInfo = true)]
        public static void SaveVehicles(Client sender, string id)
        {
            AccountController ac = sender.GetAccountController();
            foreach (var vc in RPEntityManager.GetCharacterVehicles(ac.CharacterController))
            {
                vc.Save();
            }
        }

        [Command("delvc", GreedyArg = true, SensitiveInfo = true)]
        public static void DelVehicle(Client sender)
        {
            AccountController ac = sender.GetAccountController();
            Vehicle veh = API.shared.getEntityFromHandle<Vehicle>(API.shared.getPlayerVehicle(sender));
            VehicleController vc = veh.getData("VehicleController");
            if (vc != null && vc.VehicleData == ac.CharacterController.Character.Vehicle)
                vc.Dispose();
        }
        #endregion
    }
}
