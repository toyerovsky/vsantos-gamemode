using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Controllers;
using Serverside.Core.Login;
using Serverside.Database;
using Serverside.Core.Extensions;
using Serverside.Database.Models;
using Vehicle = GTANetworkServer.Vehicle;

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
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onUpdate += OnUpdateHandler;
        }

        private float _currentRotation = 0;
        private void OnUpdateHandler()
        {
            if (RPEntityManager.GetBuildings().Count == 0) return;
            //Kręcące się markery od budynków
            if (Math.Abs(_currentRotation - 360f) < 0.4) _currentRotation = 0;
            _currentRotation += 0.4f;

            foreach (var building in RPEntityManager.GetBuildings())
            {
                building.BuildingMarker.rotation = new Vector3(building.BuildingMarker.rotation.X, building.BuildingMarker.rotation.Y, _currentRotation);
            }
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            //args[0] float X
            //args[1] float Y
            //args[2] float Z
            if (eventName == "ChangePosition")
            {
                sender.position = new Vector3((float)arguments[0], (float)arguments[1], (float)arguments[2]);
            }
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
            Task dbStop = Task.Run(() =>
            {
                foreach (KeyValuePair<long, AccountController> p in RPEntityManager.GetAccounts().Where(x => x.Value != null))
                {
                    AccountController ac = p.Value;
                    //Zmiana postaci pola Online w postaci po wyłączeniu serwera dla graczy którzy byli online
                    ac.CharacterController.Character.Online = false;

                    //Zmiana w przedmiocie pola CurrentlyInUse na false
                    foreach (var i in ac.CharacterController.Character.Items.Where(i => i.CurrentlyInUse == true).ToList())
                    {
                        i.CurrentlyInUse = false;
                    }
                    ac.Client.resetData("RP_ACCOUNT");
                    ac.AccountData.Online = false;
                    ac.Save(true);
                }
                ContextFactory.Instance.SaveChanges();
                ContextFactory.Destroy();
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

        [Command("/dlogin", GreedyArg = true, SensitiveInfo = true, Alias = "l")]
        public void DebugLoginToAccount(Client sender, string email, string password)
        {
            ForumDatabaseHelper FDb = new ForumDatabaseHelper();
            Tuple<long, string, short, string> userData = FDb.CheckPasswordMatch(email, password);
            if (userData.Item1 == -1)
            {
                API.shared.sendChatMessageToPlayer(sender, "Podane login lub hasło są nieprawidłowe, bądź takie konto nie istnieje");
            }
            else
            {
                Account account = new Account
                {
                    UserId = userData.Item1,
                    Name = userData.Item2,
                    ForumGroup = userData.Item3,
                    OtherForumGroups = userData.Item4,
                    Email = email,
                    SocialClub = sender.name,
                    Ip = sender.address
                };


                //Sprawdzenie czy konto z danym userid istnieje jak nie dodanie konta do bazy danych i załadowanie go do core.
                if (!AccountController.DoesAccountExist(userData.Item1))
                {
                    AccountController.RegisterAccount(sender, account);
                }
                else
                {
                    //Sprawdzenie czy ktoś już jest zalogowany z tego konta.
                    AccountController _ac = RPEntityManager.GetAccount(userData.Item1);
                    if (_ac != null)
                    {
                        if (_ac.AccountData.Online)
                        {
                            API.shared.kickPlayer(_ac.Client);
                            RPChat.SendMessageToPlayer(sender,
                                $"Osoba o IP: {_ac.AccountData.Ip} znajduje się obecnie na twoim koncie. Została ona wyrzucona z serwera. Rozważ zmianę hasła.", ChatMessageType.ServerInfo);
                        }
                    }
                    AccountController.LoadAccount(sender, userData.Item1);
                }
            }
        }

        [Command("deb1", GreedyArg = true, SensitiveInfo = true, Alias = "avc")]
        public static void Deb1(Client sender, string toggle = "true")
        {
            if (bool.Parse(toggle))
            {
                List<Database.Models.Vehicle> v = sender.GetAccountController().CharacterController.Character.Vehicles.ToList();
                foreach (Database.Models.Vehicle veh in v)
                {
                    API.shared.consoleOutput(GTANetworkServer.Constant.LogCat.Debug, veh.VehicleHash.ToString() + " | " + veh.NumberPlate + " | " + veh.Milage.ToString());
                }
            }
            else
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
            new VehicleController(ac.CharacterController.Character.Vehicles.First(x => x.Id == int.Parse(id)));
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
            if (vc != null && vc.VehicleData == ac.CharacterController.Character.Vehicles)
                vc.Dispose();
        }
        #endregion
    }
}
