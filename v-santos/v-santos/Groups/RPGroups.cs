using System;
using System.Linq;
using System.Timers;
using GTANetworkServer;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Controllers;
using Serverside.Core.Extenstions;
using Serverside.Groups.Base;

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

        public static GroupController CreateGroup(Database.Models.Group editor)
        {
            var groupType = editor.GroupType;
            switch (groupType)
            {
                case GroupType.Urzad: return new CityHall(editor);
                default:
                    throw new NotSupportedException($"Nie rozpoznano typu grupy: {groupType}.");
            }
        }

        #region PLAYER COMMANDS

        [Command("prowadz", "~y~ UŻYJ ~w~ /pro(wadz) (id)", Alias = "pro")]
        public void ForcePlayerToGo(Client sender, int id = -1)
        {
            var group = sender.GetAccountController().CharacterController.OnDutyGroup;
            if (group == null) return;
            if (group.Data.GroupType != GroupType.Policja || !((Police)group).CanPlayerDoPolice(sender.GetAccountController()))
            {
                sender.Notify("Twoja grupa, bądź postać nie posiada uprawnień do używania prowadzenia.");
                return;
            }

            AccountController getter;
            if (id != -1 && Validator.IsIdValid(id))
            {
                getter = RPEntityManager.GetAccountByServerId(id);
                if (getter != null)
                {
                    var distance = getter.Client.position.DistanceTo2D(sender.position);
                    if (distance > 3 || distance < -3)
                    {
                        sender.Notify("Podany gracz znajduje się za daleko.");
                        return;
                    }
                }
                else
                {
                    sender.Notify("Nie znaleziono gracza o podanym Id.");
                    return;
                }
            }
            else
            {
                getter = sender.position.GetNearestPlayer().GetAccountController();
            }

            getter.Client.freezePosition = true;
            API.playPedAnimation(getter.Client, false, "mp_arresting", "walk");
        }

        [Command("kajdanki", "~y~ UŻYJ ~w~ /kaj(danki) (id)", Alias = "kaj")]
        public void CuffPlayer(Client sender, int id = -1)
        {
            var group = sender.GetAccountController().CharacterController.OnDutyGroup;
            if (group == null) return;
            if (group.Data.GroupType != GroupType.Policja || !((Police)group).CanPlayerDoPolice(sender.GetAccountController()))
            {
                sender.Notify("Twoja grupa, bądź postać nie posiada uprawnień do używania kajdanek.");
                return;
            }

            AccountController getter;
            if (id != -1 && Validator.IsIdValid(id))
            {
                getter = RPEntityManager.GetAccountByServerId(id);
                if (getter != null)
                {
                    var distance = getter.Client.position.DistanceTo2D(sender.position);
                    if (distance > 3 || distance < -3)
                    {
                        sender.Notify("Podany gracz znajduje się za daleko.");
                        return;
                    }
                }
                else
                {
                    sender.Notify("Nie znaleziono gracza o podanym Id.");
                    return;
                }
            }
            else
            {
                getter = sender.position.GetNearestPlayer().GetAccountController();
            }

            getter.Client.freezePosition = true;
            API.playPedAnimation(getter.Client, false, "arrest", "arrest_fallback_high_cop");
            API.playPedAnimation(getter.Client, false, "rcmme_amanda1", "arrest_ama");
        }

        [Command("m", "~y~ UŻYJ ~w~ /m [tekst]", GreedyArg = true)]
        public void SayThroughTheMegaphone(Client player, string message)
        {

        }

        [Command("gduty")]
        public void EnterDuty(Client sender, short slot)
        {
            Timer dutyTimer = new Timer(60000);

            var player = sender.GetAccountController();
            if (player.CharacterController.OnDutyGroup != null)
            {
                sender.Notify(
                    $"Zszedłeś ze służby grupy: {player.CharacterController.OnDutyGroup.Data.Name}");

                player.CharacterController.OnDutyGroup = null;
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

                if (sender.TryGetGroupBySlot(Convert.ToInt16(slot), out GroupController group))
                {
                    var worker =
                        group.Data.Workers.Single(x => x.Character.Id == player.CharacterController.Character.Id);

                    dutyTimer.Start();

                    dutyTimer.Elapsed += (o, args) =>
                    {
                        worker.DutyMinutes += 1;
                        group.Save();
                    };

                    //var color = ColorConverter.ConvertFromString(group.Data.Color);

                    sender.nametag = $"( {player.ServerId} ) ( {group.Data.Name} ) {sender.name}";
                    if (group.Data.Color.Equals(null)) sender.nametagColor = group.Data.Color;

                    player.CharacterController.OnDutyGroup = group;
                    sender.Notify(
                        $"Wszedłeś na służbę grupy: {APIExtensions.GetColoredString(group.Data.Color.ToHex(), group.Data.Name)}");

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
        public void TakeMoneyFromGroup(Client sender, short slot, decimal safeMoneyCount)
        {
            if (!Validator.IsMoneyValid(safeMoneyCount))
            {
                sender.Notify("Podano kwotę gotówki w nieprawidłowym formacie.");
            }

            if (sender.TryGetGroupBySlot(Convert.ToInt16(slot), out GroupController group))
            {
                if (group.CanPlayerTakeMoney(sender.GetAccountController()))
                {
                    if (group.HasMoney(safeMoneyCount))
                    {
                        group.RemoveMoney(safeMoneyCount);
                        sender.AddMoney(safeMoneyCount);

                        sender.Notify($"Wypłacono ${safeMoneyCount} z konta grupy {group.Data.Name}.");
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
        public void PutMoneyIntoGroup(Client sender, short groupSlot, decimal safeMoneyCount)
        {
            if (!Validator.IsMoneyValid(safeMoneyCount))
            {
                sender.Notify("Podano kwotę gotówki w nieprawidłowym formacie.");
            }

            var player = sender.GetAccountController();
            
            if (sender.TryGetGroupBySlot(Convert.ToInt16(groupSlot), out GroupController group))
            {
                if (sender.HasMoney(safeMoneyCount))
                {
                    sender.RemoveMoney(safeMoneyCount);
                    group.AddMoney(safeMoneyCount);

                    player.Client.Notify($"Wpłacono ${safeMoneyCount} na konto grupy {group.Data.Name}.");
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