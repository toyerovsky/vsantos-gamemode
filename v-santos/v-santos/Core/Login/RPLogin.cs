using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using Newtonsoft.Json;
using Serverside.Database;

namespace Serverside.Core.Login
{
    sealed class RPLogin : Script
    {
        private MySqlDatabaseHelper db;

        public delegate void OnPlayerLoginHandler(Player player);

        public delegate void OnCharacterNotCreatedHandler(Player player);

        public static event OnCharacterNotCreatedHandler OnCharacterNotCreated;

        public static event OnPlayerLoginHandler OnPlayerLogin;

        public RPLogin()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onPlayerFinishedDownload += API_onPlayerFinishedDownload;
            OnPlayerLogin += RPLogin_OnPlayerLogin;
            API.onChatCommand += API_onChatCommand;
            API.onPlayerBeginConnect += API_onPlayerBeginConnect;
            API.onResourceStop += API_onResourceStop;
            API.onPlayerDisconnected += API_onPlayerDisconnected;

            db = new MySqlDatabaseHelper();
        }

        private void API_onPlayerDisconnected(Client sender, string reason)
        {
            if (RPCore.Players.All(p => !sender.hasData("AccountID") || sender.hasData("AccountID") && p.Key != sender.getData("AccountID"))) return;

            var player = RPCore.Players.First(p => p.Key == sender.getData("AccountID"));
            RPCore.Players.Remove(player.Key);
            var items = player.Value.Helper.SelectItemsList(player.Value.Cid, 1).Select(t => player.Value.Helper.SelectItem(t.IID)).ToList().Where(p => p.CurrentlyInUse == true);

            //foreach (var i in items)
            //{
            //    if (i.CurrentlyInUse != null)
            //    {
            //        i.CurrentlyInUse = false;
            //        if (i.ItemType == (int) ItemType.Mask) i.FirstParameter -= 1;
            //        player.Value.Helper.UpdateItem(i);
            //    }
            //}

            player.Value.Dispose();          
        }

        private void API_onResourceStop()
        {
            db = null;
        }

        private void API_onPlayerBeginConnect(Client player, CancelEventArgs cancelConnection)
        {
            if (!player.isCEFenabled)
            {
                cancelConnection.Reason = "Aby połączyć się z serwerem musisz włączyć przeglądarki CEF.";
                cancelConnection.Cancel = true;
            }
        }

        private void API_onChatCommand(Client sender, string command, CancelEventArgs e)
        {
            if (!sender.getData("CanCommand"))
            {
                e.Cancel = true;
            }
        }

        private void RPLogin_OnPlayerLogin(Player sender)
        {
            sender.SetData("CanTalk", true);
            sender.SetData("CanNarrate", true);
            sender.SetData("CanPM", true);
            sender.SetData("CanCommand", true);
            sender.SetData("CanPay", true);

            //TODO Rozwiązanie pomocnicze
            //Tutaj będzie trzeba przechowywać wszystkie opcje ustawiane w menu użytkownika i wczytywać je
            //sender.SetSyncedData("ClickMuted", false);

            API.triggerClientEvent(sender.Client, "Money_Changed",
                String.Format(@"${0}", sender.Editor.Money));

            API.triggerClientEvent(sender.Client, "ToggleHud", true);
            RPChat.SendMessageToPlayer(sender.Client, String.Format("Witaj, {0} zostałeś pomyślnie zalogowany!", sender.Nickname), ChatMessageType.ServerInfo);
        }

        private void API_onPlayerFinishedDownload(Client player)
        {
            player.setData("CanTalk", false);
            player.setData("CanNarrate", false);
            player.setData("CanPM", false);
            player.setData("CanCommand", false);
            
            //Nie używać tego wymiaru, jest zajęty na logowanie
            player.dimension = 2137;
            player.position = new Vector3(-1666f, -1020f, 12f);

            API.triggerClientEvent(player, "ShowLoginCef", true, new Vector3(-1650f, -1030f, 50f), new Vector3(0f, 0f, 180f));        
            API.triggerClientEvent(player, "ToggleHud", false);
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("RPLogin uruchomione pomyslnie!");
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] args)
        {
            if (eventName == "OnPlayerEnteredLoginData")
            {
                if (player.hasData("AccountID")) return;

                Serverside.Login.Login login = new Serverside.Login.Login(args[0].ToString(), args[1].ToString());
                if (login.LogInPlayer(player))
                {
                    API.triggerClientEvent(player, "ShowLoginCef", false);
                    string charactersJson = JsonConvert.SerializeObject(db.SelectCharactersList(player.getData("AccountID")));
                    //true pokaz, false zniszcz
                    API.triggerClientEvent(player, "ShowCharacterSelectCef", true, charactersJson);
                    RPChat.SendMessageToPlayer(player, "Używaj klawiszy A i D, aby przewijać swoje postacie.", ChatMessageType.ServerInfo);
                }
            }
            //Przy używaniu tego musimy jako args[0] wysłać UID postaci
            else if (eventName == "OnPlayerSelectedCharacter")
            {
                Player loggedPlayer = new Player(player.getData("AccountID"));

                if (RPCore.Players.Any(p => p.Key == loggedPlayer.Aid))
                {
                    SettleCollisionProblem(loggedPlayer);
                }

                CharacterEditor character = loggedPlayer.Helper.SelectCharacter(Convert.ToInt64(args[0]));
                loggedPlayer.SetData("ServerId", CalculatePlayerId());
                loggedPlayer.SetData("CharacterID", character.CID);

                RPCore.Players.Add(loggedPlayer.Aid, loggedPlayer);

                player.nametag = "(" + player.getData("ServerId").ToString() + ") " + character.Name + " " + character.Surname;
                
                API.shared.setPlayerName(player, loggedPlayer.Nickname);
                API.shared.setEntityPosition(player, character.LastPosition);
                
                //player.dimension = editor.CurrentDismension;
                player.dimension = 0;

                if (character.BWState > 0)
                {
                    API.shared.setPlayerHealth(player, -1);
                }
                else
                {
                    API.shared.setPlayerHealth(player, character.HitPoits);
                }

                API.triggerClientEvent(player, "ShowCharacterSelectCef", false);
                if (OnPlayerLogin != null) OnPlayerLogin.Invoke(loggedPlayer);
            }
        }

        private static int CalculatePlayerId()
        {
            var players = RPCore.Players.Values.ToList();
            if (players.Count == 0) return 0;

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Id != i)
                {
                    return i;
                }
            }
            return players.Count + 1;
        }

        private void SettleCollisionProblem(Player loggingPlayer)
        {
            var players = API.getAllPlayers();
            players.Remove(loggingPlayer.Client);

            if (players.All(p => p.getData("AccountID") != loggingPlayer.Aid))
            {
                RPCore.Players.Single(p => p.Value.Aid == loggingPlayer.Aid).Value.Dispose();
                RPCore.Players.Remove(loggingPlayer.Aid);
                return;
            }

            Player colidingPlayer = RPCore.Players.Single(p => p.Value.Aid == loggingPlayer.Aid).Value;
            RPChat.SendMessageToPlayer(loggingPlayer.Client, String.Format("Osoba o IP: {0} znajduje się obecnie na twoim koncie. Została ona wyrzucona z serwera. Rozważ zmianę hasła.", 
                colidingPlayer.Client.address), ChatMessageType.ServerInfo);

            API.kickPlayer(colidingPlayer.Client, String.Format("Osoba o IP: {0} zalogowała się na konto. Rozważ zmianę hasła.", loggingPlayer.Client.address));
        }
    }
}
