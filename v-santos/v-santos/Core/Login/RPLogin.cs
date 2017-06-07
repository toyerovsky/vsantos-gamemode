using System;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Controllers;
using Serverside.Database;
using Serverside.Database.Models;
using Serverside.Core.Extensions;

namespace Serverside.Core.Login
{
    sealed class RPLogin : Script
    {
        public static ForumDatabaseHelper FDb;

        public static event LoginEventHandler OnPlayerLogin;

        public RPLogin()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onPlayerBeginConnect += API_onPlayerBeginConnect;
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
            //Przy używaniu tego musimy jako args[0] wysłać UID postaci
            else if (eventName == "OnPlayerSelectedCharacter")
            {
                AccountController loggedAccount = player.GetAccountController();

                var id = Convert.ToInt64(args[0]);
                AccountController.LoadCharacter(loggedAccount, ContextFactory.Instance.Characters.FirstOrDefault(x => x.Id == id));
                Character character = loggedAccount.CharacterController.Character;

                if (character != null)
                {
                    player.nametag = "(" + RPEntityManager.CalculateServerId(loggedAccount) + ") " + character.Name + " " + character.Surname;

                    API.shared.setPlayerName(player, character.Name + " " + character.Surname);
                    player.setSkin((PedHash)character.Model);

                    API.shared.setEntityPosition(player, new Vector3(character.LastPositionX, character.LastPositionY, character.LastPositionZ));

                    player.dimension = 0;

                    if (character.BWState > 0)
                    {
                        API.shared.setPlayerHealth(player, -1);
                    }
                    else
                    {
                        API.shared.setPlayerHealth(player, character.HitPoints);
                    }

                    API.triggerClientEvent(player, "ShowCharacterSelectCef", false);

                    player.SetData("CanTalk", true);
                    player.SetData("CanNarrate", true);
                    player.SetData("CanPM", true);
                    player.SetData("CanCommand", true);
                    player.SetData("CanPay", true);

                    API.triggerClientEvent(player, "Money_Changed", $"${character.Money}");
                    API.triggerClientEvent(player, "ToggleHud", true);
                    RPChat.SendMessageToPlayer(player,
                        $"Witaj, twoja postać {character.Name + " " + character.Surname} została pomyślnie załadowana, życzymy miłej gry!", ChatMessageType.ServerInfo);
                }
                if (OnPlayerLogin != null) OnPlayerLogin.Invoke(loggedAccount);
            }
        }

        [Command("/dlogin", GreedyArg = true, SensitiveInfo = true, Alias = "l")]
        public void DebugLoginToAccount(Client sender, string email, string password)
        {
            long userId = FDb.CheckPasswordMatch(email, password);
            if (userId == -1)
            {
                API.shared.sendChatMessageToPlayer(sender, "Podane login lub hasło są nieprawidłowe, bądź takie konto nie istnieje");
            }
            else
            {
                //Sprawdzenie czy konto z danym userid istnieje jak nie dodanie konta do bazy danych i załadowanie go do core.
                if (!AccountController.RegisterAccount(sender, userId, email))
                {
                    //Sprawdzenie czy ktoś już jest zalogowany z tego konta.
                    AccountController _ac = RPEntityManager.GetAccount(userId);
                    if (_ac != null)
                    {
                        if (_ac.Account.Online)
                        {
                            API.shared.kickPlayer(_ac.Client);
                            RPChat.SendMessageToPlayer(sender,
                                $"Osoba o IP: {_ac.Account.Ip} znajduje się obecnie na twoim koncie. Została ona wyrzucona z serwera. Rozważ zmianę hasła.", ChatMessageType.ServerInfo);
                        }
                    }
                    AccountController.LoadAccount(sender, userId);
                }
            }
        }

        public static bool LoginToAccount(Client sender, string email, string password)
        {
            long userId = FDb.CheckPasswordMatch(email, password);
            if (userId == -1)
            {
                API.shared.sendNotificationToPlayer(sender, "Podane login lub hasło są nieprawidłowe, bądź takie konto nie istnieje");
                return false;
            }
            //Sprawdzenie czy konto z danym userid istnieje jak nie dodanie konta do bazy danych i załadowanie go do core.
            if (!AccountController.RegisterAccount(sender, userId, email))
            {
                //Sprawdzenie czy ktoś już jest zalogowany z tego konta.
                AccountController _ac = RPEntityManager.GetAccount(userId);
                if (_ac != null)
                {
                    if (_ac.Account.Online)
                    {
                        API.shared.kickPlayer(_ac.Client);
                        RPChat.SendMessageToPlayer(sender, $"Osoba o IP: {_ac.Account.Ip} znajduje się obecnie na twoim koncie. Została ona wyrzucona z serwera. Rozważ zmianę hasła.", ChatMessageType.ServerInfo);
                    }
                }
                AccountController.LoadAccount(sender, userId);
            }
            return true;
        }

        public static void LoginMenu(Client player)
        {
            player.SetData("CanTalk", false);
            player.SetData("CanNarrate", false);
            player.SetData("CanPM", false);
            player.SetData("CanCommand", true);

            //Nie używać tego wymiaru, jest zajęty na logowanie
            player.dimension = 2137;
            player.position = new Vector3(-1666f, -1020f, 12f);

            API.shared.triggerClientEvent(player, "ShowLoginCef", true, new Vector3(-1650f, -1030f, 50f), new Vector3(0f, 0f, 180f));
            API.shared.triggerClientEvent(player, "ToggleHud", false);
        }

        public static void LogOut(AccountController account)
        {
            account.Account.Online = false;
            if (account.CharacterController != null)
                account.CharacterController.Character.Online = false;
            account.Save();
            account.Client.resetData("RP_ACCOUNT");
            RPEntityManager.RemoveAccount(account.AccountId);
        }

        public static void LogOut(Client player)
        {
            AccountController account = player.GetAccountController();
            account.Account.Online = false;
            if (account.CharacterController != null)
                account.CharacterController.Character.Online = false;
            account.Save();
            player.ResetData("RP_ACCOUNT");
            RPEntityManager.RemoveAccount(account.AccountId);
        }
    }
}
