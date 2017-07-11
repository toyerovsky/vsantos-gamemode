using System;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using Serverside.Admin.Enums;
using Serverside.Controllers;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Database.Models;
using Serverside.Groups.Enums;

namespace Serverside.Admin
{
    public class RPAdminGroups : Script
    {
        public RPAdminGroups()
        {
            APIExtensions.ConsoleOutput("[AdminGroups] Uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
        }

        [Command("stworzgrupe", GreedyArg = true)]
        public void CreateGroup(Client sender, int bossId, GroupType type, string name, string tag, string hexColor)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.GameMaster2)
            {
                sender.Notify("Nie posiadasz uprawnień do tworzenia grupy.");
                return;
            }

            Color hex;
            try
            {
                hex = hexColor.ToColor();
            }
            catch (Exception e)
            {
                sender.Notify("Wprowadzony kolor jest nieprawidłowy.");
                APIExtensions.ConsoleOutput("[Error] Nieprawidłowy kolor [RPAdminGroups]", ConsoleColor.Red);
                APIExtensions.ConsoleOutput(e.Message, ConsoleColor.Red);
                return;
            }

            if (RPEntityManager.GetAccountByServerId(bossId) != null)
            {
                var boss = RPEntityManager.GetAccountByServerId(bossId);
                
                if (boss.CharacterController.Character.Workers.Count < 3)
                {
                    GroupController group = GroupController.CreateGroup(name, tag, type, hex);
                    group.GetWorkers().Add(new Worker
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
                    sender.Notify($"Stworzyłeś grupę {group.GetColoredName()}.");
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

        [Command("wejdzgrupa")]
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

                if (group.GetWorkers().Any(p => p.Character == player.CharacterController.Character))
                {
                    sender.Notify("Jesteś już w tej grupie.");
                    return;
                }

                if (player.CharacterController.Character.Workers.Count < 3)
                {
                    group.GetWorkers().Add(new Worker
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

                    sender.Notify($"Wszedłeś do grupy {group.GetColoredName()}.");
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
