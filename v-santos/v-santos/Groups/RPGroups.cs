﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows.Media;
using GTANetworkServer;
using Serverside.Core;
using Serverside.Extensions;
using Serverside.Groups.Base;
using Color = GTANetworkServer.Constant.Color;

namespace Serverside.Groups
{
    public class RPGroups : Script
    {
        public RPGroups()
        {
            API.onResourceStart += OnResourceStartHandler;
        }

        private void OnResourceStartHandler()
        {
            APIExtensions.ConsoleOutput("[RPGroups] Uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
        }

        public static List<Group> Groups => RPCore.Groups;

        public static Group CreateGroup(Database.Models.Group editor)
        {
            var groupType = (GroupType)editor.GroupType;
            switch (groupType)
            {
                case GroupType.CrimeGroup: return new CrimeGroup(editor);
                case GroupType.Taxi: return new Taxi(editor);
                case GroupType.Police: return new Base.Police(editor);
                default:
                    throw new NotSupportedException($"Nie rozpoznano typu grupy: {groupType}.");
            }
        }

        #region Komendy
        [Command("gduty")]
        public void EnterDuty(Client sender, string slot)
        {
            Timer dutyTimer = new Timer(60000);

            var player = sender.GetAccountController();
            if (player.CharacterController.OnDutyGroupId.HasValue)
            {
                sender.Notify(
                    $"Zszedłeś ze służby grupy: {player.CharacterController.Character.Worker.Single(g => g.Group.Id == player.CharacterController.OnDutyGroupId).Group.Name}");

                player.CharacterController.OnDutyGroupId = null;
                sender.resetNametagColor();
                sender.nametag = $"( {player.ServerId} ) {player.CharacterController.FormatName}";
                dutyTimer.Stop();
                dutyTimer.Dispose();
            }
            else
            {
                if (!Validator.IsGroupSlotValid(slot))
                {
                    sender.Notify("Wprowadzono dane w nieprawidłowym formacie.");
                    return;
                }

                if (sender.TryGetGroupBySlot(Convert.ToInt16(slot), out Group group))
                {
                    var worker =
                        group.Data.Workers.Single(x => x.Character.Id == player.CharacterController.Character.Id);

                    dutyTimer.Start();

                    dutyTimer.Elapsed += (o, args) =>
                    {
                        worker.DutyMinutes += 1;
                        group.Controller.Save();
                    };

                    var color = ColorConverter.ConvertFromString(group.Data.Color);

                    sender.nametag = "[" + sender.getData("ServerId").ToString() + "]" + "( " + group.Data.Name + " ) " + sender.name;
                    if (color != null) sender.nametagColor = (Color) color;

                    player.CharacterController.OnDutyGroupId = group.Id;
                    sender.Notify(
                        $"Wszedłeś na służbę grupy: {RPCore.GetColoredString(group.Data.Color, group.Data.Name)}");

                    API.onPlayerDisconnected += (client, reason) =>
                    {
                        if (client == sender) dutyTimer.Dispose();
                    };
                }
                else
                {
                    sender.Notify("Nie posiadasz grupy w tym slocie.");
                }
            }
        }

        [Command("gwyplac")]
        public void TakeMoneyFromGroup(Client sender, string slot, string moneyCount)
        {
            if (!Validator.IsGroupSlotValid(slot) || !Validator.IsMoneyStringValid(moneyCount))
            {
                API.shared.sendNotificationToPlayer(sender, "Wprowadzono dane w niepoprawnym formacie.");
                return;
            }

            if (sender.TryGetGroupBySlot(Convert.ToInt16(slot), out Group group))
            {
                decimal safeMoneyCount = Convert.ToDecimal(moneyCount);

                if (group.CanPlayerTakeMoney(sender.GetAccountController()))
                {
                    if (group.HasMoney(safeMoneyCount))
                    {
                        group.RemoveMoney(safeMoneyCount);
                        sender.AddMoney(safeMoneyCount);

                        sender.Notify($"Wypłacono ${moneyCount} z konta grupy {group.Data.Name}.");
                    }
                    else
                    {
                        sender.Notify($"Grupa {group.Data.Name}, nie posiada tyle pieniędzy na koncie.");
                    }
                }
                else
                {
                    sender.Notify($"Nie posiadasz uprawnień do wypłacania gotówki w grupie {group.Data.Name}.");
                }
            }
            else
            {
                sender.Notify("Nie posiadasz grupy w tym slocie.");
            }
        }

        [Command("gwplac")]
        public void PutMoneyIntoGroup(Client sender, string groupSlot, string moneyCount)
        {
            if (!Validator.IsGroupSlotValid(groupSlot) || !Validator.IsMoneyStringValid(moneyCount))
            {
                API.shared.sendNotificationToPlayer(sender, "Wprowadzono dane w niepoprawnym formacie.");
                return;
            }

            var player = sender.GetAccountController();
            
            if (sender.TryGetGroupBySlot(Convert.ToInt16(groupSlot), out Group group))
            {
                decimal safeMoneyCount = Convert.ToDecimal(moneyCount);


                if (sender.HasMoney(safeMoneyCount))
                {
                    sender.RemoveMoney(safeMoneyCount);
                    group.AddMoney(safeMoneyCount);

                    player.Client.Notify($"Wpłacono ${moneyCount} na konto grupy {group.Data.Name}.");
                }
                else
                {
                    player.Client.Notify("Nie posiadasz tyle gotówki.");
                }
            }
            else
            {
                player.Client.Notify("Nie posiadasz grupy w tym slocie.");
            }
        }
        
        #endregion
    }
}