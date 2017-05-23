﻿using System;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using Newtonsoft.Json;
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
                if (LoginToAccount(player, args[0].ToString(), args[1].ToString()))
                {
                    AccountController accountcontroller = player.GetAccountController();
                    RPChat.SendMessageToPlayer(player,
                        $"Witaj, {accountcontroller.Account.SocialClub} zostałeś pomyślnie zalogowany. Wybierz postać którą chcesz grać.", ChatMessageType.ServerInfo);
                    API.triggerClientEvent(player, "ShowLoginCef", false);

                    //DEBUG
                    if (accountcontroller.Account.Character.Count == 0)
                    {
                        ContextFactory.Instance.Characters.Add(new Character()
                        {
                            Account = accountcontroller.Account,
                            Name = accountcontroller.Account.Email,
                            Surname = "test",
                            IsAlive = true,
                            Model = (int)PedHash.Michael
                        });
                        ContextFactory.Instance.SaveChanges();
                    }

                    API.triggerClientEvent(player, "ShowCharacterSelectCef", true, JsonConvert.SerializeObject(accountcontroller.Account.Character.Where(c => c.IsAlive).Select(
                        ch => new
                        {
                            ch.Id,
                            ch.Name,
                            ch.Surname,
                            ch.Money,
                            ch.BankMoney
                        }).ToList()));
                    RPChat.SendMessageToPlayer(player, "Używaj strzałek, aby przewijać swoje postacie.", ChatMessageType.ServerInfo);
                }
            }
            //Przy używaniu tego musimy jako args[0] wysłać UID postaci
            else if (eventName == "OnPlayerSelectedCharacter")
            {
                AccountController loggedAccount = player.GetAccountController();

                var id = Convert.ToInt64(args[0]);
                Character dbCharacter = ContextFactory.Instance.Characters.FirstOrDefault(x => x.Id == id);

                AccountController.LoadCharacter(loggedAccount, dbCharacter);

                if (dbCharacter != null)
                {
                    player.nametag = "(" + RPCore.CalculateServerId(loggedAccount) + ") " + dbCharacter.Name + " " + dbCharacter.Surname;

                    API.shared.setPlayerName(player, dbCharacter.Name + " " + dbCharacter.Surname);
                    API.shared.setEntityPosition(player, new Vector3(dbCharacter.LastPositionX, dbCharacter.LastPositionY, dbCharacter.LastPositionZ));

                    player.dimension = 0;

                    if (dbCharacter.BWState > 0)
                    {
                        API.shared.setPlayerHealth(player, -1);
                    }
                    else
                    {
                        API.shared.setPlayerHealth(player, dbCharacter.HitPoints);
                    }

                    API.triggerClientEvent(player, "ShowCharacterSelectCef", false);

                    player.SetData("CanTalk", true);
                    player.SetData("CanNarrate", true);
                    player.SetData("CanPM", true);
                    player.SetData("CanCommand", true);
                    player.SetData("CanPay", true);

                    API.triggerClientEvent(player, "Money_Changed", $"${dbCharacter.Money}");
                    API.triggerClientEvent(player, "ToggleHud", true);
                    RPChat.SendMessageToPlayer(player,
                        $"Witaj, twoja postać {dbCharacter.Name + " " + dbCharacter.Surname} została pomyślnie załadowana, życzymy miłej gry!", ChatMessageType.ServerInfo);
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
                if (!AccountController.RegisterAccount(sender, userId))
                {
                    //Sprawdzenie czy ktoś już jest zalogowany z tego konta.
                    AccountController _ac = RPCore.GetAccount(userId);
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
                API.shared.sendChatMessageToPlayer(sender, "Podane login lub hasło są nieprawidłowe, bądź takie konto nie istnieje");
                return false;
            }
            //Sprawdzenie czy konto z danym userid istnieje jak nie dodanie konta do bazy danych i załadowanie go do core.
            if (!AccountController.RegisterAccount(sender, userId))
            {
                //Sprawdzenie czy ktoś już jest zalogowany z tego konta.
                AccountController _ac = RPCore.GetAccount(userId);
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
            account.Save();
            account.Account.Online = false;
            ContextFactory.Instance.SaveChanges();
            account.Client.resetData("RP_ACCOUNT");
            RPCore.RemoveAccount(account.AccountId);
        }

        public static void LogOut(Client player)
        {
            AccountController account = player.GetAccountController();
            account.Save();
            account.Account.Online = false;
            ContextFactory.Instance.SaveChanges();
            player.ResetData("RP_ACCOUNT");
            RPCore.RemoveAccount(account.AccountId);
        }
    }
}
