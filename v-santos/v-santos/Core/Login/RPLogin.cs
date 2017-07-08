using System;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using Serverside.Controllers;
using Serverside.Database;
using Serverside.Database.Models;
using Serverside.Core.Extensions;
using Newtonsoft.Json;
using Serverside.Admin;

namespace Serverside.Core.Login
{
    sealed class RPLogin : Script
    {
        public static ForumDatabaseHelper FDb;

        public RPLogin()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onPlayerBeginConnect += API_onPlayerBeginConnect;
            AccountController.CharacterLoggedIn += RPLogin_OnPlayerLogin;
            FDb = new ForumDatabaseHelper();
        }

        private void API_onPlayerBeginConnect(Client player, CancelEventArgs cancelConnection)
        {
            if (!player.isCEFenabled)
            {
                cancelConnection.Reason = "Aby grać na serwerze musisz włączyć CEF.";
                cancelConnection.Cancel = true;
            }
        }

        private void API_onResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPLogin] Uruchomione pomyslnie!", ConsoleColor.DarkMagenta);
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] args)
        {
            if (eventName == "OnPlayerEnteredLoginData")
            {
                LoginToAccount(player, args[0].ToString(), args[1].ToString());
            }
            //Przy używaniu tego musimy jako args[0] wysłać indeks na liście postaci
            else if (eventName == "OnPlayerSelectedCharacter")
            {
                int characterId = Convert.ToInt32(args[0]);
                CharacterController.SelectCharacter(player, characterId);
            }
        }

        private void RPLogin_OnPlayerLogin(Client sender, AccountController account)
        {
            var chs = account.AccountData.Characters.Where(c => c.IsAlive).Select(x => new { x.Name, x.Surname, x.Money, x.BankMoney }).ToList();
            string json = JsonConvert.SerializeObject(chs);
            sender.triggerEvent("ShowCharacterSelectMenu", json);
        }

        public static void LoginToAccount(Client sender, string email, string password)
        {
            Tuple<long, string, short, string> userData = FDb.CheckPasswordMatch(email, password);
            if (userData.Item1 == -1)
            {
                //args[0] wiadomosc
                //args[1] czas wyswietlania w ms
                sender.triggerEvent("ShowNotification", "Podane login lub hasło są nieprawidłowe, bądź takie konto nie istnieje", 3000);
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
                    Ip = sender.address,
                };

                //Sprawdzenie czy konto z danym userid istnieje jak nie dodanie konta do bazy danych i załadowanie go do core.
                //Dodanie grupy serverowej do konta //toyer
                if (!AccountController.DoesAccountExist(userData.Item1))
                {
                    if (Enum.GetNames(typeof(ServerRank)).Any(e => e == ((ForumGroup)userData.Item3).ToString()))
                        account.ServerRank = (ServerRank)Enum.Parse(typeof(ServerRank), ((ForumGroup)userData.Item3).ToString());
                    else
                        account.ServerRank = ServerRank.Uzytkownik;

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

        public static void LoginMenu(Client player)
        {
            //Nie używać tego wymiaru, jest zajęty na logowanie
            player.dimension = 2137;
            player.position = new Vector3(-1666f, -1020f, 12f);
            API.shared.triggerClientEvent(player, "ShowLoginMenu");
        }

        public static void LogOut(AccountController account)
        {
            account.AccountData.Online = false;
            if (account.CharacterController != null)
                account.CharacterController.Character.Online = false;
            account.Save();
            account.Client.resetData("RP_ACCOUNT");
            RPEntityManager.RemoveAccount(account.AccountId);
        }

        public static void LogOut(Client player)
        {
            AccountController account = player.GetAccountController();
            account.AccountData.Online = false;
            if (account.CharacterController != null)
                account.CharacterController.Character.Online = false;
            account.Save();
            player.ResetData("RP_ACCOUNT");
            RPEntityManager.RemoveAccount(account.AccountId);
        }
    }
}
