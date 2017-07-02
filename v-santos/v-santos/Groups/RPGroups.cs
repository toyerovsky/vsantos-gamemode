using System;
using System.Linq;
using System.Timers;
using GTANetworkServer;
using Newtonsoft.Json;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Controllers;
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

        #region PLAYER COMMANDS

        [Command("prowadz", "~y~ UŻYJ ~w~ /pro(wadz) (id)", Alias = "pro")]
        public void ForcePlayerToGo(Client sender, int id = -1)
        {
            var group = sender.GetAccountController().CharacterController.OnDutyGroup;
            if (group == null) return;
            if (group.GroupData.GroupType != GroupType.Policja || !((Police)group).CanPlayerDoPolice(sender.GetAccountController()))
            {
                sender.Notify("Twoja grupa, bądź postać nie posiada uprawnień do używania prowadzenia.");
                return;
            }

            AccountController getter = null;
            float distance;
            if (id != -1)
            {
                getter = RPEntityManager.GetAccountByServerId(id);
                if (getter != null)
                {
                    distance = getter.Client.position.DistanceTo2D(sender.position);
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
                var ac = sender.position.GetNearestPlayer();
                if (ac != null)
                {
                    distance = ac.position.DistanceTo2D(sender.position);
                    if (distance < 3 && distance > -3)
                    {
                        getter = ac.GetAccountController();
                    }
                }
            }

            if (getter == null) return;
            getter.Client.freezePosition = true;
            API.playPedAnimation(getter.Client, false, "mp_arresting", "walk");
        }

        [Command("kajdanki", "~y~ UŻYJ ~w~ /kaj(danki) (id)", Alias = "kaj")]
        public void CuffPlayer(Client sender, int id = -1)
        {
            var group = sender.GetAccountController().CharacterController.OnDutyGroup;
            if (group == null) return;
            if (group.GroupData.GroupType != GroupType.Policja || !((Police)group).CanPlayerDoPolice(sender.GetAccountController()))
            {
                sender.Notify("Twoja grupa, bądź postać nie posiada uprawnień do używania kajdanek.");
                return;
            }

            AccountController getter = null;
            float distance;
            if (id != -1)
            {
                getter = RPEntityManager.GetAccountByServerId(id);
                if (getter != null)
                {
                    distance = getter.Client.position.DistanceTo2D(sender.position);
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
                var ac = sender.position.GetNearestPlayer();
                if (ac != null)
                {
                    distance = ac.position.DistanceTo2D(sender.position);
                    if (distance < 3 && distance > -3)
                    {
                        getter = ac.GetAccountController();
                    }
                }
            }

            if (getter == null) return;
            getter.Client.freezePosition = true;
            API.playPedAnimation(getter.Client, false, "arrest", "arrest_fallback_high_cop");
            API.playPedAnimation(getter.Client, false, "rcmme_amanda1", "arrest_ama");
        }



        [Command("gduty")]
        public void EnterDuty(Client sender, short slot)
        {
            Timer dutyTimer = new Timer(60000);

            var player = sender.GetAccountController();
            if (player.CharacterController.OnDutyGroup != null)
            {
                sender.Notify(
                    $"Zszedłeś ze służby grupy: {player.CharacterController.OnDutyGroup.GetColoredName()}");

                player.CharacterController.OnDutyGroup.PlayersOnDuty.Remove(player);
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
                    sender.Notify("Podany slot grupy nie jest poprawny.");
                    return;
                }

                if (sender.TryGetGroupByUnsafeSlot(Convert.ToInt16(slot), out GroupController group))
                {
                    var worker =
                        group.GroupData.Workers.Single(x => x.Character.Id == player.CharacterController.Character.Id);

                    dutyTimer.Start();

                    dutyTimer.Elapsed += (o, args) =>
                    {
                        worker.DutyMinutes += 1;
                        group.Save();
                    };

                    sender.nametag = $"( {player.ServerId} ) ( {group.GroupData.Name} ) {sender.name}";
                    if (group.GroupData.Color.Equals(null)) sender.nametagColor = group.GroupData.Color;

                    player.CharacterController.OnDutyGroup = group;
                    player.CharacterController.OnDutyGroup.PlayersOnDuty.Add(player);
                    sender.Notify(
                        $"Wszedłeś na służbę grupy: {group.GetColoredName()}");

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

            if (sender.TryGetGroupByUnsafeSlot(slot, out GroupController group))
            {
                if (group.CanPlayerTakeMoney(sender.GetAccountController()))
                {
                    if (group.HasMoney(safeMoneyCount))
                    {
                        group.RemoveMoney(safeMoneyCount);
                        sender.AddMoney(safeMoneyCount);

                        sender.Notify($"Wypłacono ${safeMoneyCount} z konta grupy {group.GetColoredName()}.");
                    }
                    else
                    {
                        sender.Notify($"Grupa {group.GetColoredName()}, nie posiada tyle pieniędzy na koncie.");
                    }
                }
                else
                {
                    sender.Notify("Nie posiadasz uprawnień do wypłacania gotówki.");
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

            if (sender.TryGetGroupByUnsafeSlot(groupSlot, out GroupController group))
            {
                if (sender.HasMoney(safeMoneyCount))
                {
                    sender.RemoveMoney(safeMoneyCount);
                    group.AddMoney(safeMoneyCount);

                    sender.Notify($"Wpłacono ${safeMoneyCount} na konto grupy {group.GetColoredName()}.");
                }
                else
                {
                    sender.Notify("Nie posiadasz tyle gotówki.");
                }
            }
            else
            {
                sender.Notify("Nie posiadasz grupy w tym slocie.");
            }
        }

        [Command("g")]
        public void ShowGroupMenu(Client sender, short slot)
        {
            var player = sender.GetAccountController();
            if (RPEntityManager.GetPlayerGroups(player).Count == 0)
            {
                sender.Notify("Nie jesteś członkiem żadnej grupy.");
                return;
            }

            if (!Validator.IsGroupSlotValid(slot))
            {
                sender.Notify("Podano dane w nieprawidłowym formacie.");
                return;
            }

            if (sender.TryGetGroupByUnsafeSlot(slot, out GroupController group))
            {
                sender.triggerEvent("ShowGroupMenu", JsonConvert.SerializeObject(new
                {
                    group.GroupData.Name,
                    group.GroupData.Tag,
                    group.GroupData.Money,
                    Color = group.GroupData.Color.ToHex(),
                    //To jest raczej kosztowne, ale nie widzę innej opcji
                    PlayerOnLine = JsonConvert.SerializeObject(group.GroupData.Workers.Where(x => x.Character.Online).Select(w => new
                    {
                        ServerId = RPEntityManager.GetAccountByCharacterId(w.Character.Id).ServerId,
                        Name = $"{w.Character.Name} {w.Character.Surname}",
                        Salary = w.Salary,
                        DutyTime = w.DutyMinutes,
                        OnDuty = group.PlayersOnDuty.Contains(RPEntityManager.GetAccountByCharacterId(w.Character.Id))
                    }))
                }), group.CanPlayerManageWorkers(player));
            }
            else
            {
                sender.Notify("Nie posiadasz grupy w tym slocie.");
            }
        }

        [Command("gzapros")]
        public void InvitePlayerToGroup(Client sender, short groupSlot, int id)
        {
            var player = sender.GetAccountController();

            if (sender.TryGetGroupByUnsafeSlot(groupSlot, out GroupController group))
            {
                if (group.CanPlayerManageWorkers(sender.GetAccountController()))
                {
                    var getter = RPEntityManager.GetAccountByServerId(id);
                    if (getter == null)
                    {
                        sender.Notify("Nie znaleziono gracza o podanym Id.");
                        return;
                    }

                    if (group.ContainsWorker(getter))
                    {
                        sender.Notify($"{getter.CharacterController.FormatName} już znajduje się w grupie {group.GetColoredName()}");
                        return;
                    }
                    group.AddWorker(getter);
                    getter.Client.Notify($"Zostałeś zaproszony do grupy {group.GetColoredName()} przez {sender.name}.");
                    sender.Notify($"Zaprosiłeś gracza {getter.Client.name} do grupy {group.GetColoredName()}.");
                }
                else
                {
                    sender.Notify("Nie posiadasz uprawnień do zarządzania pracownikami.");
                }
            }
            else
            {
                sender.Notify("Nie posiadasz grupy w tym slocie.");
            }
        }

        [Command("gwypros")]
        public void RemovePlayerFromGroup(Client sender, short groupSlot, int id)
        {
            var player = sender.GetAccountController();

            if (sender.TryGetGroupByUnsafeSlot(groupSlot, out GroupController group))
            {
                if (group.CanPlayerManageWorkers(sender.GetAccountController()))
                {
                    var getter = RPEntityManager.GetAccountByServerId(id);
                    if (getter == null)
                    {
                        sender.Notify("Nie znaleziono gracza o podanym Id.");
                        return;
                    }

                    if (group.ContainsWorker(getter))
                    {
                        sender.Notify($"{getter.CharacterController.FormatName} nie należy do grupy {group.GetColoredName()}");
                        return;
                    }

                    group.RemoveWorker(getter);
                    getter.Client.Notify($"Zostałeś wyproszony z grupy {group.GetColoredName()} przez {sender.name}.");
                    sender.Notify($"Wyprosiłeś gracza {getter.Client.name} z grupy {group.GetColoredName()}.");
                }
                else
                {
                    player.Client.Notify("Nie posiadasz uprawnień do zarządzania pracownikami.");
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