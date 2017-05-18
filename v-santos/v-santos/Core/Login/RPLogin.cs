using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using Newtonsoft.Json;
using Serverside.Database;
using Serverside.DatabaseEF6;
using Serverside.DatabaseEF6.Models;
using Serverside.Core.Extenstions;

namespace Serverside.Core.Login
{
    sealed class RPLogin : Script
    {
        public static ForumDatabaseHelper FDb;

        public RPLogin()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onResourceStop += API_onResourceStop;
            FDb = new ForumDatabaseHelper();
        }

        //private void API_onPlayerDisconnected(Client sender, string reason)
        //{
        //    if (RPCore.Players.All(p => !sender.hasData("AccountID") || sender.hasData("AccountID") && p.Key != sender.getData("AccountID"))) return;

        //    var player = RPCore.Players.First(p => p.Key == sender.getData("AccountID"));
        //    RPCore.Players.Remove(player.Key);
        //    var items = ItemDatabaseHelper.SelectItemsList(player.Value.Editor).Select(t => ItemDatabaseHelper.SelectItem(t.ItemId)).ToList().Where(p => p.CurrentlyInUse == true);

        //    //foreach (var i in items)
        //    //{
        //    //    if (i.CurrentlyInUse != null)
        //    //    {
        //    //        i.CurrentlyInUse = false;
        //    //        if (i.ItemType == (int) ItemType.Mask) i.FirstParameter -= 1;
        //    //        player.Value.Helper.UpdateItem(i);
        //    //    }
        //    //}

        //    player.Value.Dispose();          
        //}

        private void API_onResourceStop()
        {

        }

        private void API_onResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPLogin] Uruchomione pomyslnie!", ConsoleColor.DarkMagenta);
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] args)
        {
            if (eventName == "OnPlayerEnteredLoginData")
            {
                //if (!player.HasData("RP_ACCOUNT")) return;

                //Serverside.Login.Login login = new Serverside.Login.Login(args[0].ToString(), args[1].ToString());
                if (LoginToAccount(player, args[0].ToString(), args[1].ToString()))
                {
                    AccountController accountcontroller = player.GetAccountController();
                    RPChat.SendMessageToPlayer(player, String.Format("Witaj, {0} zostałeś pomyślnie zalogowany. Wybierz postać którą chcesz grać.", accountcontroller.Account.SocialClub), ChatMessageType.ServerInfo);
                    API.triggerClientEvent(player, "ShowLoginCef", false);
                    string charactersJson = JsonConvert.SerializeObject(accountcontroller.Account.Character.ToList());
                    //true pokaz, false zniszcz
                    API.triggerClientEvent(player, "ShowCharacterSelectCef", true, charactersJson);
                    RPChat.SendMessageToPlayer(player, "Używaj klawiszy A i D, aby przewijać swoje postacie.", ChatMessageType.ServerInfo);
                }
            }
            //Przy używaniu tego musimy jako args[0] wysłać UID postaci
            else if (eventName == "OnPlayerSelectedCharacter")
            {
                AccountController LoggedAccount = player.GetAccountController();

                Character DbCharacter = ContextFactory.Instance.Characters.Where(x => x.Id == Convert.ToInt64(args[0])).FirstOrDefault();

                AccountController.LoadCharacter(LoggedAccount, DbCharacter);

                player.nametag = "(" + RPCore.CalculateServerId(LoggedAccount) + ") " + DbCharacter.Name + " " + DbCharacter.Surname;

                API.shared.setPlayerName(player, DbCharacter.Name + " " + DbCharacter.Surname);
                API.shared.setEntityPosition(player, new Vector3(DbCharacter.LastPositionX, DbCharacter.LastPositionY, DbCharacter.LastPositionZ));

                //player.dimension = editor.CurrentDismension;
                player.dimension = 0;

                if (DbCharacter.BWState > 0)
                {
                    API.shared.setPlayerHealth(player, -1);
                }
                else
                {
                    API.shared.setPlayerHealth(player, DbCharacter.HitPoints);
                }

                API.triggerClientEvent(player, "ShowCharacterSelectCef", false);

                player.SetData("CanTalk", true);
                player.SetData("CanNarrate", true);
                player.SetData("CanPM", true);
                player.SetData("CanCommand", true);
                player.SetData("CanPay", true);

                //TODO Rozwiązanie pomocnicze
                //Tutaj będzie trzeba przechowywać wszystkie opcje ustawiane w menu użytkownika i wczytywać je
                //sender.SetSyncedData("ClickMuted", false);

                API.triggerClientEvent(player, "Money_Changed", String.Format(@"${0}", DbCharacter.Money));
                API.triggerClientEvent(player, "ToggleHud", true);
                RPChat.SendMessageToPlayer(player, String.Format("Witaj, twoja postać {0} została pomyślnie załadowana, życzymy miłej gry!", DbCharacter.Name + " " + DbCharacter.Surname), ChatMessageType.ServerInfo);

                //if (OnPlayerLogin != null) OnPlayerLogin.Invoke(loggedPlayer.Client);
            }
        }

        [Command("/dlogin", GreedyArg = true, SensitiveInfo = true, Alias = "l")]
        public void DebugLoginToAccount(Client sender, string email, string password)
        {
            long UserId = FDb.CheckPasswordMatch(email, password);
            if (UserId == -1)
            {
                API.shared.sendChatMessageToPlayer(sender, "Podane login lub hasło są nieprawidłowe, bądź takie konto nie istnieje");
            }
            else
            {
                //Sprawdzenie czy konto z danym userid istnieje jak nie dodanie konta do bazy danych i załadowanie go do core.
                if (!AccountController.RegisterAccount(sender, UserId))
                {
                    //Sprawdzenie czy ktoś już jest zalogowany z tego konta.
                    AccountController _ac = RPCore.GetAccount(UserId);
                    if (_ac != null)
                    {
                        if (_ac.Account.Online)
                        {
                            API.shared.kickPlayer(_ac.Client);
                            RPChat.SendMessageToPlayer(sender, String.Format("Osoba o IP: {0} znajduje się obecnie na twoim koncie. Została ona wyrzucona z serwera. Rozważ zmianę hasła.", _ac.Account.Ip), ChatMessageType.ServerInfo);
                        }
                    }
                    AccountController.LoadAccount(sender, UserId);
                }
            }
        }

        public static bool LoginToAccount(Client sender, string email, string password)
        {
            long UserId = FDb.CheckPasswordMatch(email, password);
            if (UserId == -1)
            {
                API.shared.sendChatMessageToPlayer(sender, "Podane login lub hasło są nieprawidłowe, bądź takie konto nie istnieje");
                return false;
            }
            else
            {
                //Sprawdzenie czy konto z danym userid istnieje jak nie dodanie konta do bazy danych i załadowanie go do core.
                if (!AccountController.RegisterAccount(sender, UserId))
                {
                    //Sprawdzenie czy ktoś już jest zalogowany z tego konta.
                    AccountController _ac = RPCore.GetAccount(UserId);
                    if (_ac != null)
                    {
                        if (_ac.Account.Online)
                        {
                            API.shared.kickPlayer(_ac.Client);
                            RPChat.SendMessageToPlayer(sender, String.Format("Osoba o IP: {0} znajduje się obecnie na twoim koncie. Została ona wyrzucona z serwera. Rozważ zmianę hasła.", _ac.Account.Ip), ChatMessageType.ServerInfo);
                        }
                    }
                    AccountController.LoadAccount(sender, UserId);
                }
                return true;
            }
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
            player.resetData("RP_ACCOUNT");
            RPCore.RemoveAccount(account.AccountId);
        }

        //private static int CalculatePlayerId()
        //{
        //    var players = RPCore.Players.Values.ToList();
        //    if (players.Count == 0) return 0;

        //    for (int i = 0; i < players.Count; i++)
        //    {
        //        if (players[i].Id != i)
        //        {
        //            return i;
        //        }
        //    }
        //    return players.Count + 1;
        //}

        //private void SettleCollisionProblem(Player loggingPlayer)
        //{
        //    var players = API.getAllPlayers();
        //    players.Remove(loggingPlayer.Client);

        //    if (players.All(p => p.getData("AccountID") != loggingPlayer.Aid))
        //    {
        //        RPCore.Players.Single(p => p.Value.Aid == loggingPlayer.Aid).Value.Dispose();
        //        RPCore.Players.Remove(loggingPlayer.Aid);
        //        return;
        //    }

        //    Player colidingPlayer = RPCore.Players.Single(p => p.Value.Aid == loggingPlayer.Aid).Value;
        //    RPChat.SendMessageToPlayer(loggingPlayer.Client, String.Format("Osoba o IP: {0} znajduje się obecnie na twoim koncie. Została ona wyrzucona z serwera. Rozważ zmianę hasła.", 
        //        colidingPlayer.Client.address), ChatMessageType.ServerInfo);

        //    API.kickPlayer(colidingPlayer.Client, String.Format("Osoba o IP: {0} zalogowała się na konto. Rozważ zmianę hasła.", loggingPlayer.Client.address));
        //}
    }
}
