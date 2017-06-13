using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using Serverside.Core.Extensions;

namespace Serverside.Core
{
    public enum ChatMessageType
    {
        Quiet = 5,
        Normal = 15,
        PhoneOthers = 16,
        Me = 20,
        Do = 21,
        ServerMe = 25,
        Loud = 30,
        OOC = 32,

        ServerDo,
        PrivateMessage,
        ServerInfo,
        Phone,
        GroupOOC,
        GroupRadio,
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
            if (sender.GetAccountController() == null || !sender.GetAccountController().CharacterController.CanTalk) return;
            SendMessageToNearbyPlayers(sender, message, sender.GetAccountController().CharacterController.CellphoneController.CurrentlyTalking ? ChatMessageType.PhoneOthers : ChatMessageType.Normal);
            SaidEventHandler handler = OnPlayerSaid;
            SaidEventArgs eventArgs = new SaidEventArgs(sender, message, ChatMessageType.Normal);
            handler?.Invoke(this, eventArgs);
        }

        private void API_onChatCommand(Client sender, string command, CancelEventArgs e)
        {
            if (sender.GetAccountController() == null || !sender.GetAccountController().CharacterController.CanCommand) e.Cancel = true;
        }

        #region Komendy


        [Command("c", "~y~UŻYJ: ~w~ /c [treść]", GreedyArg = true)]
        public void SendQuietMessage(Client player, string message)
        {
            SendMessageToNearbyPlayers(player, message, ChatMessageType.Quiet);

            SaidEventHandler handler = OnPlayerSaid;
            SaidEventArgs eventArgs = new SaidEventArgs(player, message, ChatMessageType.Quiet);
            handler?.Invoke(this, eventArgs);
        }

        [Command("k", "~y~UŻYJ: ~w~ /k [treść]", GreedyArg = true)]
        public void SendScreamMessage(Client player, string message)
        {
            SendMessageToNearbyPlayers(player, message, ChatMessageType.Loud);

            SaidEventHandler handler = OnPlayerSaid;
            SaidEventArgs eventArgs = new SaidEventArgs(player, message, ChatMessageType.Loud);
            handler?.Invoke(this, eventArgs);
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

        [Command("ado")]
        public void SendAdministratorDoMessage(Client player, string message)
        {
            API.shared.sendChatMessageToAll("~#847DB7~", $"** {message} **");
        }

        [Command("w", "~y~UŻYJ: ~w~ /w [id] [treść]", GreedyArg = true)]
        public void SendPrivateMessageToPlayer(Client sender, int ID, string message)
        {
            if (!sender.GetAccountController().CharacterController.CanPM)
            {
                sender.Notify("Nie możesz teraz pisać wiadomości!");
                return;
            }

            if (sender.GetAccountController().ServerId.Equals(ID))
            {
                sender.Notify("Nie możesz wysłać wiadomości samemu sobie.");
                return;
            }

            Client getter = RPEntityManager.GetAccount(ID).Client;
            SendMessageToPlayer(getter, message, ChatMessageType.PrivateMessage, sender);
            SendMessageToPlayer(sender, message, ChatMessageType.PrivateMessage, getter);
        }

        [Command("b", GreedyArg = true)]
        public void SendOOCMessage(Client sender, string message)
        {
            SendMessageToNearbyPlayers(sender, message, ChatMessageType.OOC);
        }

        [Command("go", "~y~UŻYJ: ~w~ /go [slot] [treść]", GreedyArg = true)]
        public void SendMessageOnGroupChat(Client sender, short groupSlot, string message)
        {
            if (Validator.IsGroupSlotValid(groupSlot))
            {
                sender.Notify("Podany slot grupy jest nieprawidłowy.");
                return;
            }

            var groups = RPEntityManager.GetPlayerGroups(sender.GetAccountController());

            if (groups.Count - 1 >= groupSlot && groups[groupSlot] != null)
            {
                if (groups[groupSlot].CanPlayerWriteOnChat(sender.GetAccountController()))
                {
                    var clients = RPEntityManager.GetAccounts().Where(a => groups[groupSlot].Data.Workers
                        .Any(w => w.Character.Id.Equals(a.Value.CharacterController.Character.Id))).Select(c => c.Value.Client).ToList();
                    SendMessageToSpecifiedPlayers(sender, clients, message, ChatMessageType.GroupOOC, $"~{groups[groupSlot].Data.Color.ToHex()}~");
                }
                else
                {
                    sender.Notify("Nie posiadasz uprawnień do czatu w tej grupie.");
                }
            }
        }

        #endregion

        public static void SendMessageToSpecifiedPlayers(Client sender, List<Client> players, string message, ChatMessageType chatMessageType, string color = "")
        {
            message = PrepareMessage(sender.name, message, chatMessageType);

            if (chatMessageType == ChatMessageType.GroupOOC)
            {
                message = $"[{sender.GetAccountController().ServerId}] {sender.name}: {message}";
            }

            foreach (var p in players)
            {
                API.shared.sendChatMessageToPlayer(p, color, message);
            }
        }

        /// <summary>
        /// Przy czacie grupowym podajemy kolor grupy
        /// </summary>
        /// <param name="player"></param>
        /// <param name="message"></param>
        /// <param name="chatMessageType"></param>
        /// <param name="color"></param>
        public static void SendMessageToNearbyPlayers(Client player, string message, ChatMessageType chatMessageType, string color = "")
        {
            message = PrepareMessage(player.name, message, chatMessageType);
            switch (chatMessageType)
            {
                case ChatMessageType.OOC:
                    message = $"(( [{player.GetAccountController().ServerId}] {player.name} {message} ))";
                    color = "~#CCCCCC~";
                    break;
            }

            var clients = API.shared.getPlayersInRadiusOfPlayer((float)chatMessageType, player);
            //Dla każdego klienta w zasięgu wyświetl wiadomość, zasięg jest pobierany przez rzutowanie enuma do floata
            foreach (var c in clients)
            {
                API.shared.sendChatMessageToPlayer(c, color, message);
            }
        }

        public static void SendMessageToPlayer(Client player, string message, ChatMessageType chatMessageType, Client secondPlayer = null)
        {
            message = PrepareMessage(player.name, message, chatMessageType);
            string color = "";

            if (chatMessageType == ChatMessageType.PrivateMessage && secondPlayer != null)
            {
                message = $"~o~ [{secondPlayer.GetAccountController().ServerId}] {secondPlayer.name}: {message}";
            }
            else if (chatMessageType == ChatMessageType.Phone)
            {
                color = "~#ffdb00~";
                message = player.GetAccountController().CharacterController.Character.Gender
                    ? $"Głos z telefonu (Mężczyzna): {message}"
                    : $"Głos z telefonu (Kobieta): {message}";
            }
            API.shared.sendChatMessageToPlayer(player, color, message);
        }

        private static string PrepareMessage(string name, string message, ChatMessageType chatMessageType)
        {
            string color = string.Empty;
            switch (chatMessageType)
            {
                case ChatMessageType.Normal:
                    message = $"{name} mówi: {message}";
                    color = "~#FFFFFF~";
                    break;
                case ChatMessageType.Quiet:
                    message = $"{name} szepcze: {message}";
                    color = "~#FFFFFF~";
                    break;
                case ChatMessageType.Loud:
                    message = $"{name} krzyczy: {message}!";
                    color = "~#FFFFFF~";
                    break;
                case ChatMessageType.Me:
                    message = $"** {name} {message}";
                    color = "~#C2A2DA~";
                    break;
                case ChatMessageType.ServerMe:
                    message = $"* {name} {message}";
                    color = "~#C2A2DA~";
                    break;
                case ChatMessageType.Do:
                    message = $"** {message} (( {name} )) **";
                    color = "~#847DB7~";
                    break;
                case ChatMessageType.PhoneOthers:
                    message = $"{name} mówi(telefon): {message}";
                    color = "~#FFFFFF~";
                    break;
                case ChatMessageType.ServerInfo:
                    message = $"~g~ [INFO] ~w~ {message}";
                    break;
                case ChatMessageType.ServerDo:
                    message = $"** {message} **";
                    color = "~#847DB7~";
                    break;
            }

            if (char.IsLower(message.First()))
            {
                message = char.ToUpper(message.First()) + message.Substring(1);
            }

            if (message.Last() != '.' && message.Last() != '!' && message.Last() != '?')
            {
                message += ".";
            }
            return color + message;
        }
    }
}
