using System.Collections.Generic;
using GTANetworkServer;
using Serverside.Database;
using Serverside.DatabaseEF6;
using System.Threading.Tasks;
using System;
using GTANetworkShared;
using Serverside.DatabaseEF6.Models;
using System.Linq;
using Serverside.Core.Login;
using Serverside.Core.Extenstions;

namespace Serverside.Core
{
    public class RPCore : Script
    {
        //public static MySqlDatabaseHelper Db;

        //Robimy słownik wszystkich klientów żeby można było potem korzystać z helpera tego gracza
        //long to ID konta
        private static SortedList<long, AccountController> Accounts = new SortedList<long, AccountController>();
        public event DimensionChangeEventHandler OnPlayerDimensionChanged;
        public static event OnCharacterNotCreatedEventHandler OnCharacterNotCreated;
        public static event OnPlayerLoginEventHandler OnPlayerLogin;


        public RPCore()
        {
            API.onResourceStart += API_onResourceStart;
            API.onResourceStop += API_onResourceStop;
            API.onPlayerBeginConnect += API_onPlayerBeginConnect;
            API.onPlayerConnected += API_onPlayerConnectedHandler;
            API.onPlayerFinishedDownload += API_onPlayerFinishedDownload;
            API.onPlayerDisconnected += API_onPlayerDisconnectedHandler;
            OnPlayerDimensionChanged += Client_OnPlayerDimensionChanged;
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
            ContextFactory.SetConnectionParameters(API.getSetting<string>("database_server"), API.getSetting<string>("database_user"), API.getSetting<string>("database_password"), API.getSetting<string>("database_database")); // NIE WYMAGANE
            ContextFactory.Instance.SaveChanges();
            //FDb = new ForumDatabaseHelper();

            //Players = new Dictionary<long, Player>();
        }

        private void API_onResourceStop()
        {
            //FDb = null;
            Task DBStop = Task.Run(() =>
            {
                Save();
                //ContextFactory.Instance.SaveChanges();
                ContextFactory.Instance.Dispose();
            });
            DBStop.Wait();
        }

        public void Save()
        {
            foreach (var account in Accounts)
            {
                account.Value.Save();
            }    
            ContextFactory.Instance.SaveChanges();
        }

        public static void AddAccount(long _AccountId, AccountController _AccountController)
        {
            Accounts.Add(_AccountId, _AccountController);
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

        public static int CalculateServerId(AccountController account)
        {
            return Accounts.IndexOfValue(account);
        }

        public static string GetColoredString(string color, string text)
        {
            return "~" + color + "~" + text;
        }

        [Command("q")]
        public void Quit(Client player)
        {
            API.shared.kickPlayer(player);
        }
    }
}
