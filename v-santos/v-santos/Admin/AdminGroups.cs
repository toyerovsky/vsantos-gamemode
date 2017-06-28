using System;
using System.Drawing;
using System.Linq;
using GTANetworkServer;
using Serverside.Controllers;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Database.Models;
using Serverside.Groups;
using Serverside.Groups.Base;

namespace Serverside.Admin
{
    public class AdminGroups : Script
    {
        public AdminGroups()
        {
            APIExtensions.ConsoleOutput("[AdminGroups] Uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
        }

        [Command("stworzgrupa", GreedyArg = true)]
        public void CreateGroup(Client sender, int bossId, GroupType type, string name, string tag, string hexColor)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.GameMaster2)
            {
                sender.Notify("Nie posiadasz uprawnień do tworzenia grupy.");
                return;
            }

            var rgb = ColorTranslator.FromHtml(hexColor);

            if (!rgb.IsKnownColor)
            {
                sender.Notify("Wprowadzono nieprawidłowy kolor.");
                return;
            }

            if (RPEntityManager.GetAccountByServerId(bossId) != null)
            {
                var boss = RPEntityManager.GetAccountByServerId(bossId);
                
                if (boss.CharacterController.Character.Workers.Count > 3)
                {
                    GroupController group = GroupController.CreateGroup(name, tag, type, hexColor.ToColor());
                    group.GroupData.Workers.Add(new Worker
                    {
                        Group = group.GroupData,
                        Character = boss.CharacterController.Character,
                        ChatRight = true,
                        DoorsRight = true,
                        OfferFromWarehouseRight = true,
                        PaycheckRight = true,
                        RecrutationRight = true,
                        DutyMinutes = 0,
                        Salary = 0
                    });
                    group.Save();
                }
                else
                {
                    boss.Client.Notify("Nie posiadasz wolnych slotów grupowych.");
                    if (boss.Client != sender)
                        sender.Notify(
                            $"Gracz: {boss.Client.name} nie posiada wolnych slotów grupowych.");
                }
            }
            else
            {
                sender.Notify("Nie znaleziono gracza o podanym Id.");
            }
        }

        [Command("wejdzgrupa", GreedyArg = true)]
        public void JoinGroup(Client sender, long groupId)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.GameMaster)
            {
                sender.Notify("Nie posiadasz uprawnień do ustawienia wchodzenia do grupy.");
                return;
            }

            if (RPEntityManager.GetGroup(groupId) != null)
            {
                var player = sender.GetAccountController();

                GroupController group = RPEntityManager.GetGroup(groupId);

                if (group.GroupData.Workers.Any(p => p.Character == player.CharacterController.Character))
                {
                    sender.Notify("Jesteś już w tej grupie.");
                    return;
                }

                if (player.CharacterController.Character.Workers.Count <= 3)
                {
                    group.GroupData.Workers.Add(new Worker
                    {
                        Group = group.GroupData,
                        Character = sender.GetAccountController().CharacterController.Character,
                        ChatRight = true,
                        DoorsRight = true,
                        OfferFromWarehouseRight = true,
                        PaycheckRight = true,
                        RecrutationRight = true,
                        DutyMinutes = 0,
                        Salary = 0
                    });
                    group.Save();
                }
                else
                {
                    sender.Notify("Nie posiadasz wolnych slotów grupowych.");
                }
            }
            else
            {
                sender.Notify("Nie znaleziono grupy o podanym Id.");
            }
        }
    }
}
