using System.Collections.Generic;
using GTANetworkServer;
using Serverside.Database;
using Serverside.DatabaseEF6;
using System.Threading.Tasks;
using System;
using GTANetworkShared;
using Serverside.DatabaseEF6.Models;
using System.Linq;

namespace Serverside.Core
{
    public sealed class RPCore : Script
    {
        //public static MySqlDatabaseHelper Db;
        public static ForumDatabaseHelper FDb;
        //Robimy słownik wszystkich klientów żeby można było potem korzystać z helpera tego gracza
        //long to ID konta
        public static Dictionary<long, Player> Players;
        private static Dictionary<long, AccountController> Accounts = new Dictionary<long, AccountController>();


        public RPCore()
        {
            API.onResourceStart += API_onResourceStart;
            API.onResourceStop += API_onResourceStop;
            API.onPlayerConnected += API_onPlayerConnectedHandler;
            API.onPlayerDisconnected += API_onPlayerDisconnectedHandler;
        }

        private void API_onPlayerDisconnectedHandler(Client player, string reason)
        {
            AccountController account = player.getData("RP_ACCOUNT");
            if (account == null) return;
            LogOut(account);
        }

        private void API_onPlayerConnectedHandler(Client player)
        {
            if (AccountController.IsAccountBanned(player))
            {
                player.kick("~r~You are banned from this server.");
            }
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("RPCore uruchomione pomyslnie!");
            ContextFactory.SetConnectionParameters(API.getSetting<string>("database_server"), API.getSetting<string>("database_user"), API.getSetting<string>("database_password"), API.getSetting<string>("database_database")); // NIE WYMAGANE
            ContextFactory.Instance.SaveChanges();
            FDb = new ForumDatabaseHelper();

            Players = new Dictionary<long, Player>();
        }

        private void API_onResourceStop()
        {
            FDb = null;
            Task DBStop = Task.Run(() =>
            {
                ContextFactory.Instance.SaveChanges();
                ContextFactory.Instance.Dispose();
            });
            DBStop.Wait();
        }

        public static bool LoginToAccount(Client sender, string email, string password)
        {
            long AccountId = FDb.CheckPasswordMatch(email, password);
            if (AccountId == -1)
            {
                API.shared.sendChatMessageToPlayer(sender, "Podane login lub hasło są nieprawidłowe, bądź takie konto nie istnieje");
                //Console.WriteLine("Podane login lub hasło są nieprawidłowe, bądź takie konto nie istnieje");
                return false;
            }
            else
            {
                //Sprawdzenie czy ktoś już jest zalogowany z tego konta.
                if (Accounts.ContainsKey(AccountId))
                {
                    AccountController onlineplayer = Accounts[AccountId];
                    if (onlineplayer.Account.Online)
                    {
                        API.shared.kickPlayer(onlineplayer.Client);
                        RPChat.SendMessageToPlayer(sender, String.Format("Osoba o IP: {0} znajduje się obecnie na twoim koncie. Została ona wyrzucona z serwera. Rozważ zmianę hasła.", onlineplayer.Account.Ip), ChatMessageType.ServerInfo);
                        return true;
                    }               
                }
                Account AccountData = ContextFactory.Instance.Accounts.Where(x => x.Id == AccountId).FirstOrDefault();
                new AccountController(AccountData, sender);
                return true;
            }
        }

        public static void LogOut(AccountController account)
        {
            account.Save();
            account.Account.Online = false;
            ContextFactory.Instance.SaveChanges();
            account.Client.resetData("RP_ACCOUNT");
        }

        public static void Add(long _AccountId, AccountController _AccountController)
        {
            Accounts.Add(_AccountId, _AccountController);
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
