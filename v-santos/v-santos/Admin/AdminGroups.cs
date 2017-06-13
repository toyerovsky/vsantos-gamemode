using System;
using System.Drawing;
using System.Linq;
using GTANetworkServer;
using Serverside.Controllers;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Core.Finders;
using Serverside.Database.Models;
using Serverside.Groups;

namespace Serverside.Admin
{
    public class AdminGroups : Script
    {
        public AdminGroups()
        {
            APIExtensions.ConsoleOutput("[AdminGroups] Uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
        }

        [Command("stworzgrupa", GreedyArg = true)]
        public void CreateGroup(Client sender, string bossId, string type, string name, string tag, string hexColor)
        {
            var player = sender.GetAccountController();

            if (!Validator.IsIntIdValid(bossId))
            {
                sender.Notify("Wprowadzono ID w nieprawidłowym formacie.");
                return;
            }

            var rgb = ColorTranslator.FromHtml(hexColor);

            if (!rgb.IsKnownColor)
            {
                sender.Notify("Wprowadzono nieprawidłowy kolor.");
                return;
            }
            Client boss;

            if (PlayerFinder.TryFindClientByServerId(Convert.ToInt32(bossId), out boss))
            {
                GroupType groupType;
                if (Enum.TryParse(type, true, out groupType))
                {
                    Database.Models.Group g = new Database.Models.Group()
                    {
                        Name = name,
                        Tag = tag,
                        Color = new GTANetworkServer.Constant.Color(rgb.R, rgb.G, rgb.B),
                        GroupType = groupType
                    };

                    if (boss.GetAccountController().CharacterController.Character.Workers.Count > 3)
                    {
                        GroupController group = RPGroups.CreateGroup(g);
                        group.Data.Workers.Add(new Worker()
                        {
                            Group = group.Data,
                            Character = boss.GetAccountController().CharacterController.Character,
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
                        boss.Notify("Nie posiadasz wolnych slotów grupowych.");
                        if (boss != sender) sender.Notify(
                            $"Gracz: {boss.name} nie posiada wolnych slotów grupowych.");
                    }
                }
                else
                {
                    sender.Notify("Wprowadzono nieprawidłowy typ grupy.");
                }
            }
            else
            {
                sender.Notify("Nie znaleziono gracza o podanym ID.");
            }
        }

        [Command("wejdzgrupa", GreedyArg = true)]
        public void JoinGroup(Client sender, string groupId)
        {
            var player = sender.GetAccountController();

            if (!Validator.IsLongIdValid(groupId))
            {
                sender.Notify("Wprowadzono ID w nieprawidłowym formacie.");
                return;
            }

            long gid = long.Parse(groupId);

            if (RPEntityManager.GetGroup(gid) != null)
            {
                GroupController group = RPEntityManager.GetGroup(gid);

                if (group.Data.Workers.Any(p => p.Character == player.CharacterController.Character))
                {
                    sender.Notify("Jesteś już w tej grupie.");
                    return;
                }

                if (player.CharacterController.Character.Workers.Count <= 3)
                {
                    group.Data.Workers.Add(new Worker()
                    {
                        Group = group.Data,
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
                sender.Notify("Nie istnieje grupa o takim ID.");
            }
        }
    }
}
