using System;
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

namespace Serverside.Core
{
    public class RPCore : Script
    {
        //public static MySqlDatabaseHelper Db;

        //Robimy słownik wszystkich klientów żeby można było potem korzystać z helpera tego gracza
        //long to ID konta
        private static readonly SortedList<long, AccountController> Accounts = new SortedList<long, AccountController>();
        private static readonly List<VehicleController> VehicleControllers = new List<VehicleController>();

        public static readonly List<Group> Groups = new List<Group>();

        public RPCore()
        {
            API.onResourceStart += API_onResourceStart;
            API.onResourceStop += API_onResourceStop;
            API.onPlayerBeginConnect += API_onPlayerBeginConnect;
            API.onPlayerConnected += API_onPlayerConnectedHandler;
            API.onPlayerFinishedDownload += API_onPlayerFinishedDownload;
            API.onPlayerDisconnected += API_onPlayerDisconnectedHandler;
            API.onVehicleHealthChange += OnVehicleHealthChange;

        }

        private void OnVehicleHealthChange(NetHandle entity, float oldValue)
        {
            if (API.getEntityType(entity) == EntityType.Player)
            {
                Client client = API.getPlayerFromHandle(entity);
                if (API.isPlayerInAnyVehicle(client))
                {

                    double t = Math.Floor(Math.Max(49, oldValue - client.vehicle.health) / 50);
                    //API.sendNotificationToPlayer(client, t.ToString());
                    if (t > 0)
                        APIExtensions.PopRandomsTyres(client.vehicle, int.Parse(t.ToString()));

                    float vehHealth = API.getVehicleHealth(client.vehicle);
                    float damageTaken = Math.Max(0, oldValue - vehHealth);
                    float healthToSet = vehHealth - damageTaken;
                    //API.sendChatMessageToAll(client.socialClubName + " OLD: " + oldValue.ToString() + " | NEW:" + veh_health + " | TAKEN: " + damage_taken + " | SET: " + health_to_set);
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

        private void API_onResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPCore] Uruchomione pomyslnie!", ConsoleColor.DarkMagenta);
            //API.getSetting<string>("database_server"), API.getSetting<string>("database_user"), API.getSetting<string>("database_password"), API.getSetting<string>("database_database")
            ContextFactory.SetConnectionParameters("v-santos.pl", "srv", "WL8oTnufAAEFgoIt", "rp"); // NIE WYMAGANE
            ContextFactory.Instance.SaveChanges();

            foreach (var group in ContextFactory.Instance.Groups)
            {
                Groups.Add(RPGroups.CreateGroup(group));
            }
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

                    foreach (var v in API.getAllVehicles())
                    {
                        if (v.GetVehicleController() != null)
                        {
                            var controller = v.GetVehicleController();
                            controller.Dispose();
                        }
                    }
                }

                Save();
                ContextFactory.Instance.Dispose();
            });
            dbStop.Wait();
        }

        public void Save()
        {
            foreach (var account in Accounts)
            {
                account.Value.Save();
            }
            ContextFactory.Instance.SaveChanges();
        }

        public static void AddAccount(long accountId, AccountController accountController)
        {
            Accounts.Add(accountId, accountController);
        }

        public static void RemoveAccount(long accountId)
        {
            Accounts.Remove(accountId);
        }

        public static AccountController GetAccount(long id)
        {
            if (id > -1) return Accounts.Get(id);
            return null;
        }

        public static AccountController GetAccount(int id)
        {
            if (id > -1) return Accounts.Values.ElementAtOrDefault(id);
            return null;
        }

        public static AccountController GetAccountByServerId(int id)
        {
            if (id > -1) return Accounts.Values.Where(x => x.ServerId == id).First();
            return null;
        }

        public static void Add(VehicleController vc)
        {
            VehicleControllers.Add(vc);
        }

        public static void Remove(VehicleController vc)
        {
            VehicleControllers.Remove(vc);
        }

        public static VehicleController GetVehicle(Vehicle vehicle)
        {
            return VehicleControllers.Find(x => x.Vehicle == vehicle);
        }

        public static VehicleController GetVehicle(NetHandle vehicle)
        {
            return VehicleControllers.Find(x => x.Vehicle.handle == vehicle);
        }

        public static VehicleController GetVehicle(int id)
        {
            return VehicleControllers.Find(x => x.VehicleData.Id == id);
        }

        public static int CalculateServerId(AccountController account)
        {
            return Accounts.IndexOfValue(account);
        }

        public static string GetColoredString(string color, string text)
        {
            return "~" + color + "~" + text;
        }

        [Command("q")]
        public static void Quit(Client player)
        {
            API.shared.kickPlayer(player);
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
            foreach (var vc in VehicleControllers.Where(x => x.VehicleData.Character == ac.CharacterController.Character && x.VehicleData.IsSpawned == true).ToList())
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
    }
}
