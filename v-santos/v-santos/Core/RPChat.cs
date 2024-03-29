﻿/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using Serverside.Controllers;
using Serverside.Core.Extensions;
using Serverside.Groups;
using Serverside.Groups.Base;
using Serverside.Groups.Enums;

namespace Serverside.Core
{
    public enum ChatMessageType
    {
        Quiet = 5,
        Normal = 15,
        PhoneOthers = 16,
        Me = 24,
        Do = 25,
        ServerMe = 26,
        Loud = 30,
        OOC = 31,
        Megaphone = 50,

        ServerDo,
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

            if (sender.GetAccountController().CharacterController.CellphoneController != null)
                SendMessageToNearbyPlayers(sender, message, sender.GetAccountController().CharacterController.CellphoneController.CurrentlyTalking ? ChatMessageType.PhoneOthers : ChatMessageType.Normal);

            SendMessageToNearbyPlayers(sender, message, ChatMessageType.Normal);

            SaidEventArgs eventArgs = new SaidEventArgs(sender, message, ChatMessageType.Normal);
            OnPlayerSaid?.Invoke(this, eventArgs);
        }

        private void API_onChatCommand(Client sender, string command, CancelEventArgs e)
        {
            if (sender.GetAccountController() == null || !sender.GetAccountController().CharacterController.CanCommand) e.Cancel = true;
        }

        #region PLAYER COMMANDS

        [Command("sprobuj", "~y~UŻYJ: ~w~ /sprobuj [treść]", GreedyArg = true)]
        public void Try(Client player, string message)
        {
            SendMessageToNearbyPlayers(player, new Random().Next(2) == 0 ? "zawiódł " : "odniósł sukces " + " próbując " + message, ChatMessageType.ServerMe);

            SaidEventHandler handler = OnPlayerSaid;
            SaidEventArgs eventArgs = new SaidEventArgs(player, message, ChatMessageType.ServerMe);
            handler?.Invoke(this, eventArgs);
        }

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


        [Command("w", "~y~UŻYJ: ~w~ /w [id] [treść]", GreedyArg = true)]
        public void SendPrivateMessageToPlayer(Client sender, int id, string message)
        {
            if (!sender.GetAccountController().CharacterController.CanPM)
            {
                sender.Notify("Nie możesz teraz pisać wiadomości!");
                return;
            }

            if (sender.GetAccountController().ServerId.Equals(id))
            {
                sender.Notify("Nie możesz wysłać wiadomości samemu sobie.");
                return;
            }

            Client getter = RPEntityManager.GetAccount(id).Client;
            if (getter == null)
            {
                sender.Notify("Nie znaleziono gracza o podanym Id.");
                return;
            }

            API.sendChatMessageToPlayer(sender, $"~o~ [{getter.GetAccountController().ServerId}] {getter.name}: {message}");
            API.sendChatMessageToPlayer(getter, $"~o~ [{sender.GetAccountController().ServerId}] {sender.name}: {message}");
        }

        [Command("b", GreedyArg = true)]
        public void SendOOCMessage(Client sender, string message)
        {
            SendMessageToNearbyPlayers(sender, message, ChatMessageType.OOC);
        }

        [Command("go", "~y~UŻYJ: ~w~ /go [slot] [treść]", GreedyArg = true)]
        public void SendMessageOnGroupChat(Client sender, string message)
        {
            var slot = message.Split(' ')[0];
            short groupSlot = slot.All(char.IsDigit) ? Convert.ToInt16(slot) : (short)-1;
            if (groupSlot != -1 && Validator.IsGroupSlotValid(groupSlot))
            {
                sender.Notify("Podany slot grupy jest nieprawidłowy.");
                return;
            }
            
            if (sender.TryGetGroupByUnsafeSlot(groupSlot, out GroupController group) && group != null)
            {
                if (group.CanPlayerWriteOnChat(sender.GetAccountController()))
                {
                    var m = string.Join(" ", message);
                    var clients = RPEntityManager.GetAccounts().Where(a => group.GroupData.Workers
                        .Any(w => w.Character.Id.Equals(a.Value.CharacterController.Character.Id))).Select(c => c.Value.Client).ToList();
                    SendMessageToSpecifiedPlayers(sender, clients, m, ChatMessageType.GroupOOC, $"~{group.GroupData.Color}~");
                }
                else
                {
                    sender.Notify("Nie posiadasz uprawnień do czatu w tej grupie.");
                }
            }
        }

        [Command("m", "~y~ UŻYJ ~w~ /m [tekst]", GreedyArg = true)]
        public void SayThroughTheMegaphone(Client sender, string message)
        {
            var group = sender.GetAccountController().CharacterController.OnDutyGroup;
            if (group == null) return;
            if (group.GroupData.GroupType != GroupType.Policja || !((Police)group).CanPlayerUseMegaphone(sender.GetAccountController()))
            {
                sender.Notify("Twoja grupa, bądź postać nie posiada uprawnień do używania megafonu.");
                return;
            }
            SendMessageToNearbyPlayers(sender, message, ChatMessageType.Megaphone);
        }

        #endregion

        #region ADMIN COMMANDS

        [Command("ado", GreedyArg = true)]
        public void SendAdministratorDoMessage(Client player, string message)
        {
            message = PrepareMessage(player.name, message, ChatMessageType.ServerDo, out string color);
            API.shared.sendChatMessageToAll(color, message);
        }

        #endregion

        public static void SendMessageToSpecifiedPlayers(Client sender, List<Client> players, string message, ChatMessageType chatMessageType, string color = "")
        {
            message = PrepareMessage(sender.name, message, chatMessageType, out string messageColor);
            if (color != "") messageColor = color;
            if (chatMessageType == ChatMessageType.GroupOOC)
            {
                message = $"[{sender.GetAccountController().ServerId}] {sender.name}: {message}";
            }

            foreach (var p in players)
            {
                API.shared.sendChatMessageToPlayer(p, messageColor, message);
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
            message = PrepareMessage(player.name, message, chatMessageType, out string messageColor);
            if (color != "") messageColor = color;
            switch (chatMessageType)
            {
                case ChatMessageType.OOC:
                    message = $"(( [{player.GetAccountController().ServerId}] {player.name} {message} ))";
                    messageColor = "~#CCCCCC~";
                    break;
            }

            //Dla każdego klienta w zasięgu wyświetl wiadomość, zasięg jest pobierany przez rzutowanie enuma do floata
            API.shared.getPlayersInRadiusOfPlayer((float) chatMessageType, player)
                .ForEach(c => API.shared.sendChatMessageToPlayer(c, messageColor, message));
        }

        public static void SendMessageToPlayer(Client player, string message, ChatMessageType chatMessageType)
        {
            message = PrepareMessage(player.name, message, chatMessageType, out string color);

            if (chatMessageType == ChatMessageType.Phone)
            {
                color = "~#FFDB00~";
                message = player.GetAccountController().CharacterController.Character.Gender
                    ? $"Głos z telefonu (Mężczyzna): {message}"
                    : $"Głos z telefonu (Kobieta): {message}";
            }
            API.shared.sendChatMessageToPlayer(player, color, message);
        }

        private static string PrepareMessage(string name, string message, ChatMessageType chatMessageType, out string color)
        {
            color = string.Empty;

            if (char.IsLower(message.First()))
            {
                message = $"{char.ToUpper(message[0])}{message.Substring(1)}";
            }

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
                    message = $"[INFO] ~w~ {message}";
                    color = "~#6A9828~";
                    break;
                case ChatMessageType.ServerDo:
                    message = $"** {message} **";
                    color = "~#847DB7~";
                    break;
                case ChatMessageType.Megaphone:
                    message = $"{name} \U0001F4E3 {message}";
                    color = "~#FFDB00~";
                    break;
            }

            if (message.Last() != '.' && message.Last() != '!' && message.Last() != '?')
            {
                message = $"{message}.";
            }

            return message;
        }
    }
}
