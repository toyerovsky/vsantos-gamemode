using System;
using System.Drawing;
using System.Linq;
using GTANetworkServer;
using GTANetworkServer.Constant;
using Serverside.Controllers;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Core.Finders;
using Serverside.Database;
using Serverside.Database.Models;
using Serverside.Groups;
using Serverside.Groups.Base;
using Group = Serverside.Groups.Base.Group;

namespace Serverside.Admin
{
    public class AdminGroups : Script
    {
        public AdminGroups()
        {
            API.consoleOutput("AdminGroups zostało uruchomione pomyślnie.");
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

                    if (boss.GetAccountController().CharacterController.Character.Worker.Count > 3)
                    {
                        Group group = RPGroups.CreateGroup(g);
                        group.Controller = new GroupController(g);
                        group.Data.Workers.Add(new Worker()
                        {
                            Group = group.Controller.Data,
                            Character = boss.GetAccountController().CharacterController.Character,
                            ChatRight = true,
                            DoorsRight = true,
                            OfferFromWarehouseRight = true,
                            PaycheckRight = true,
                            RecrutationRight = true,
                            DutyMinutes = 0,
                            Salary = 0
                        });
                        group.Controller.Save();
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

            if (RPGroups.Groups.Any(g => g.Id == gid))
            {
                Group group = RPGroups.Groups.First(g => g.Id == gid);

                if (group.Data.Workers.Any(p => p.Character == player.CharacterController.Character))
                {
                    sender.Notify("Jesteś już w tej grupie.");
                    return;
                }

                if (player.CharacterController.Character.Worker.Count <= 3)
                {
                    group.Data.Workers.Add(new Worker()
                    {
                        Group = group.Controller.Data,
                        Character = sender.GetAccountController().CharacterController.Character,
                        ChatRight = true,
                        DoorsRight = true,
                        OfferFromWarehouseRight = true,
                        PaycheckRight = true,
                        RecrutationRight = true,
                        DutyMinutes = 0,
                        Salary = 0
                    });
                    group.Controller.Save();
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
