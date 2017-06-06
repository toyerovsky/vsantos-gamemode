using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using Serverside.Core.Extensions;

namespace Serverside.Core
{
    public enum ChatMessageType
    {
        Normal = 15,
        Quiet = 5,
        Loud = 30,
        Me = 20,
        ServerMe = 21,
        Do = 22,
        ServerDo,
        PrivateMessage,
        ServerInfo,
        Phone,
        PhoneOthers,
        GroupOOC
    }

    public sealed class RPChat : Script
    {
        public RPChat()
        {
            API.onResourceStart += API_onResourceStart;
            API.onChatMessage += API_onChatMessageHandler;
            API.onChatCommand += API_onChatCommand;
        }

        private void API_onResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPChat] Uruchomione pomyslnie!", ConsoleColor.DarkMagenta);
        }

        public static event SaidEventHandler OnPlayerSaid;

        private void API_onChatMessageHandler(Client sender, string message, CancelEventArgs e)
        {
            e.Cancel = true;
            if (!sender.TryGetData("CanTalk", out dynamic data)) return;
            if (!data) return;
            SendMessageToNearbyPlayers(sender, message, sender.hasSyncedData("CellphoneTalking") ? ChatMessageType.PhoneOthers : ChatMessageType.Normal);
            SaidEventHandler handler = OnPlayerSaid;
            SaidEventArgs eventArgs = new SaidEventArgs(sender, message, ChatMessageType.Normal);
            if (handler != null) handler.Invoke(this, eventArgs);
        }

        private void API_onChatCommand(Client sender, string command, CancelEventArgs e)
        {
            if (!sender.TryGetData("CanCommand", out dynamic data)) return;
            if (!(bool)data)
            {
                e.Cancel = true;
            }
        }

        #region Komendy

        [Command("c", "~y~UŻYJ: ~w~ /c [treść]", GreedyArg = true)]
        public void SendQuietMessage(Client player, string message)
        {
            SendMessageToNearbyPlayers(player, message, ChatMessageType.Quiet);

            SaidEventHandler handler = OnPlayerSaid;
            SaidEventArgs eventArgs = new SaidEventArgs(player, message, ChatMessageType.Quiet);
            if (handler != null) handler.Invoke(this, eventArgs);
        }

        [Command("k", "~y~UŻYJ: ~w~ /k [treść]", GreedyArg = true)]
        public void SendScreamMessage(Client player, string message)
        {
            SendMessageToNearbyPlayers(player, message, ChatMessageType.Loud);

            SaidEventHandler handler = OnPlayerSaid;
            SaidEventArgs eventArgs = new SaidEventArgs(player, message, ChatMessageType.Loud);
            if (handler != null) handler.Invoke(this, eventArgs);
        }

        [Command("me", "~y~UŻYJ: ~w~ /me [czynność]", GreedyArg = true)]
        public void SendMeMessage(Client player, string message)
        {
            SendMessageToNearbyPlayers(player, message, ChatMessageType.Me);
        }

        [Command("do", "~y~UŻYJ: ~w~ /do [czynność]", GreedyArg = true)]
        public void SendDoMessage(Client player, string message)
        {
            SendMessageToNearbyPlayers(player, message, ChatMessageType.Do);
        }


        [Command("w", "~y~UŻYJ: ~w~ /w [id] [treść]", GreedyArg = true)]
        public void SendPrivateMessageToPlayer(Client sender, int ID, string message)
        {
            if (!sender.getData("CanPM"))
            {
                API.sendNotificationToPlayer(sender, "Nie możesz teraz pisać wiadomości!");
                return;
            }

            if (sender.hasData("ServerId") && Convert.ToInt32(sender.getData("ServerId")).Equals(ID))
            {
                API.shared.sendNotificationToPlayer(sender, "Nie możesz wysłać wiadomości samemu sobie.");
                return;
            }

            Client getter = RPCore.GetAccount(ID).Client;
            //if (!PlayerFinder.TryFindClientByServerId(sender, ID, out getter)) return;
            SendMessageToPlayer(getter, message, ChatMessageType.PrivateMessage, sender);
            SendMessageToPlayer(sender, message, ChatMessageType.PrivateMessage, getter);
        }

        //[Command("go", "~y~UŻYJ: ~w~ /go [slot] [treść]", GreedyArg = true)]
        //public void SendMessageOnGroupChat(Client sender, string groupSlot, string message)
        //{
        //    if (!Validator.IsGroupSlotValid(groupSlot))
        //    {
        //        SendMessageToPlayer(sender, "Podany slot grupy jest nieprawidłowy.", ChatMessageType.ServerInfo);
        //    }

        //    var player = RPCore.Players.First(p => p.Key == sender.getData("AccountID")).Value;

        //    int slot = Convert.ToInt32(groupSlot);

        //    //CharacterEditor editor = player.Editor;

        //    Group gid;

        //    if (player.TryFindGroupBySlot(slot, out gid))
        //    {
        //        if (gid != null)
        //        {
        //            var workers = WorkerDatabaseHelper.SelectWorkersList(gid);
        //            var worker = workers.First(w => w.Character == sender.getData("CharacterID"));

        //            //var workerEditor = WorkerDatabaseHelper.SelectWorker(worker.WorkerId);

        //            if (worker.ChatRight)
        //            {
        //                List<Client> clients = API.getAllPlayers().Where(getter => getter.getData("CharacterID") != null).Where(getter => workers.Any(p => p.Character == getter.getData("CharacterID"))).ToList();
        //                SendMessageToSpecifiedPlayers(sender, clients, message, ChatMessageType.GroupOOC, gid.Color);
        //            }
        //            else
        //            {
        //                API.sendNotificationToPlayer(sender, "Nie posiadasz uprawnień do czatu w tej grupie.");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        API.sendNotificationToPlayer(sender, "Nie posiadasz grupy w tym slocie.");
        //    }
        //}

        #endregion

        private static void SendMessageToSpecifiedPlayers(Client sender, List<Client> players, string message, ChatMessageType chatMessageType, string color = null)
        {
            if (message.Last() != '.' || message.Last() != '!' || message.Last() != '?')
            {
                message += ".";
            }

            if (char.IsLower(message.First()))
            {
                message = char.ToUpper(message.First()) + message.Substring(1);
            }

            if (chatMessageType == ChatMessageType.GroupOOC)
            {
                message = String.Format("[{0}] {1}: {2}", sender.getData("ServerId"), sender.name, message);
            }

            foreach (var p in players)
            {
                API.shared.sendChatMessageToPlayer(p, color, message);
            }
        }

        public static void SendMessageToNearbyPlayers(Client player, string message, ChatMessageType chatMessageType)
        {
            if (message.Last() != '.' && message.Last() != '!' && message.Last() != '?')
            {
                if (chatMessageType != ChatMessageType.Loud)
                {
                    message += ".";
                }
            }

            if (char.IsLower(message.First()))
            {
                if (chatMessageType != ChatMessageType.Me && chatMessageType != ChatMessageType.ServerMe)
                {
                    message = char.ToUpper(message.First()) + message.Substring(1);
                }
            }

            string color = null;

            if (chatMessageType == ChatMessageType.Normal)
            {
                message = player.name + " mówi: " + message;
                color = "~#FFFFFF~";
            }
            else if (chatMessageType == ChatMessageType.Quiet)
            {
                message = player.name + " szepcze: " + message;
                color = "~#FFFFFF~";
            }
            else if (chatMessageType == ChatMessageType.Loud)
            {
                message = player.name + " krzyczy: " + message + "!";
                color = "~#FFFFFF~";
            }
            else if (chatMessageType == ChatMessageType.Me)
            {
                message = "** " + player.name + " " + message;
                color = "~#C2A2DA~";
            }
            else if (chatMessageType == ChatMessageType.ServerMe)
            {
                message = "* " + player.name + " " + message;
                color = "~#C2A2DA~";
            }
            else if (chatMessageType == ChatMessageType.Do)
            {
                message = "** " + message + " (( " + player.name + " )) **";
                color = "~#847DB7~";
            }
            else if (chatMessageType == ChatMessageType.PhoneOthers)
            {
                message = player.name + " mówi (telefon): " + message;
                color = "~#FFFFFF~";
            }

            List<Client> clients = API.shared.getPlayersInRadiusOfPlayer((float)chatMessageType, player);

            //Dla każdego klienta w zasięgu wyświetl wiadomość, zasięg jest pobierany przez rzutowanie enuma do floata

            foreach (Client c in clients)
            {
                API.shared.sendChatMessageToPlayer(c, color, message);
            }
        }

        public static void SendMessageToPlayer(Client player, string message, ChatMessageType chatMessageType, Client secondPlayer = null)
        {
            if (char.IsLower(message.First()))
            {
                message = char.ToUpper(message.First()) + message.Substring(1);
            }

            if (message.Last() != '.' && message.Last() != '!' && message.Last() != '?')
            {
                message += ".";
            }

            string color = null;

            if (chatMessageType == ChatMessageType.ServerInfo)
            {
                API.shared.sendChatMessageToPlayer(player, "~g~ [INFO] ~w~" + message);
            }
            else if (chatMessageType == ChatMessageType.PrivateMessage && secondPlayer != null)
            {
                API.shared.sendChatMessageToPlayer(player, String.Format("~o~ [{0}] {1}: {2} ~w~", secondPlayer.getData("ServerId"), secondPlayer.name, message));
            }
            //else if (chatMessageType == ChatMessageType.PrivateMessageGetter && secondPlayer != null)
            //{
            //    API.shared.sendChatMessageToPlayer(player, String.Format("~o~ [{0}] {1}: {2} ~w~", secondPlayer.getData("ServerId"), secondPlayer.name, message));
            //}
            else if (chatMessageType == ChatMessageType.Phone)
            {
                color = "~#ffdb00~";
                API.shared.sendChatMessageToPlayer(player, color, Convert.ToBoolean(player.getData("CharacterGender")) ? "Głos z telefonu (Mężczyzna): "
                                                                                                                         + message : "Głos z telefonu (Kobieta): " + message);
            }
        }
    }
}
