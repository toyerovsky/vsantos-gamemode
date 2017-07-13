using System;
using System.IO;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using Serverside.Admin.Enums;
using Serverside.Core;
using Serverside.Core.Extensions;

namespace Serverside.Admin
{
    public class RPAdminCore : Script
    {
        [Command("ustawrange", "~y~ UŻYJ ~w~ /ustawrange [id] [nazwa]")]
        public void SetRank(Client sender, int id, ServerRank rank)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.Adminadministrator3)
            {
                sender.Notify("Nie posiadasz uprawnień do ustawiania rang.");
                return;
            }

            var controller = RPEntityManager.GetAccountByServerId(id);
            if (controller == null)
            {
                sender.Notify("Nie znaleziono gracza o podanym Id.");
                return;
            }

            controller.AccountData.ServerRank = rank;
            controller.Save();
            sender.Notify($"Nadałeś {controller.CharacterController.FormatName} ({controller.AccountData.Name}) rangę {controller.AccountData.ServerRank.ToString()}");
        }

        [Command("tpc", "~y~ UŻYJ ~w~ /tpc [x] [y] [z]")]
        public void TeleportToCords(Client sender, float x, float y, float z)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.Support)
            {
                sender.Notify("Nie posiadasz uprawnień do teleportu na koordynaty.");
                return;
            }
            sender.position = new Vector3(x, y, z);
            sender.Notify($"Teleportowałeś się na X:{x} Y:{y} Z:{z}");
        }

        [Command("tpmap", "~y~ UŻYJ ~w~ /tpmap")]
        public void TeleportToWaypoint(Client sender)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.Support)
            {
                sender.Notify("Nie posiadasz uprawnień do teleportu na waypoint.");
                return;
            }
            sender.triggerEvent("GetWaypointPosition");

            Action<Vector3> teleportAction = position => sender.position = position;

            sender.SetData("WaypointVectorHandler", teleportAction);
        }
        

        [Command("tp", "~y~ UŻYJ ~w~ /tp [id]")]
        public void TeleportToPlayer(Client sender, int id)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.Support)
            {
                sender.Notify("Nie posiadasz uprawnień do teleportu.");
                return;
            }
            var controller = RPEntityManager.GetAccountByServerId(id);
            if (controller == null)
            {
                sender.Notify("Nie znaleziono gracza o podanym Id.");
                return;
            }
            sender.position = new Vector3(controller.Client.position.X - 5f, controller.Client.position.Y, controller.Client.position.Z + 1f);
        }

        [Command("spec", "~y~ UŻYJ ~w~ /spec [id]")]
        public void SpectatePlayer(Client sender, int id)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.Support)
            {
                sender.Notify("Nie posiadasz uprawnień do obserwowania.");
                return;
            }
            var controller = RPEntityManager.GetAccountByServerId(id);
            if (controller == null)
            {
                sender.Notify("Nie znaleziono gracza o podanym Id.");
                return;
            }
            API.setPlayerToSpectatePlayer(sender, controller.Client);
            sender.Notify($"Włączono obserwowanie na gracza {controller.CharacterController.FormatName}");
        }

        [Command("specoff")]
        public void TurnOffSpectating(Client sender)
        {
            API.setPlayerToSpectator(sender);
            sender.Notify("~r~ ~h~Wyłączono ~w~obserwowanie.");
        }

        [Command("addspec", "~y~ UŻYJ ~w~ /addspec [id]", Description = "Polecenie ustawia wybranemu graczowi specowanie na nas.")]
        public void AddSpectator(Client sender, int id)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.Support3)
            {
                sender.Notify("Nie posiadasz uprawnień do ustawienia obserwowania.");
                return;
            }
            var controller = RPEntityManager.GetAccountByServerId(id);
            if (controller == null)
            {
                sender.Notify("Nie znaleziono gracza o podanym Id.");
                return;
            }
            API.setPlayerToSpectator(controller.Client);
            sender.Notify("~g~ ~h~Włączono ~w~obserwowanie.");
        }

        [Command("kick", "~y~ UŻYJ ~w~ /kick [id] (powod)", GreedyArg = true)]
        public void KickPlayer(Client sender, int id, string reason = "")
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.Support)
            {
                sender.Notify("Nie posiadasz uprawnień do ustawienia obserwowania.");
                return;
            }
            var controller = RPEntityManager.GetAccountByServerId(id);
            if (controller == null)
            {
                sender.Notify("Nie znaleziono gracza o podanym Id.");
                return;
            }
            API.kickPlayer(controller.Client, reason);
        }

        [Command("fly")]
        public void StartFying(Client sender)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.Support5)
            {
                sender.Notify("Nie posiadasz uprawnień do ustawienia latania.");
                return;
            }

            if (sender.HasData("FlyState"))
            {
                sender.ResetData("FlyState");
                API.triggerClientEvent(sender, "FreeCamStop");
                sender.Notify("~r~ ~h~Wyłączono ~w~latanie.");
                return;
            }

            sender.SetData("FlyState", true);
            API.triggerClientEvent(sender, "FreeCamStart", API.getEntityPosition(sender));
            sender.Notify("~g~ ~h~Włączono ~w~latanie.");
        }

        [Command("god")]
        public void SetPlayerInvicible(Client sender)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.Support)
            {
                sender.Notify("Nie posiadasz uprawnień do ustawienia nieśmiertelności.");
                return;
            }
            if (API.getEntityInvincible(sender))
            {
                sender.invincible = false;
                sender.Notify("~r~Wyłączono ~w~nieśmiertelność.");
            }
            else
            {
                sender.invincible = true;
                sender.Notify("~g~~h~Włączono ~w~nieśmiertelność.");
            }
        }

        [Command("inv")]
        public void SetPlayerInvisible(Client sender)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.Support)
            {
                sender.Notify("Nie posiadasz uprawnień do ustawienia niewidzialności.");
                return;
            }

            if (sender.transparency == 0)
            {
                sender.transparency = 1;
                sender.Notify("~r~Wyłączono ~w~niewidzialności.");
            }   
            else
            {
                sender.transparency = 0;
                sender.Notify("~g~~h~Włączono ~w~niewidzialności.");
            }
        }

        [Command("savepos", "~y~ UŻYJ ~w~ /savepos [nazwa]", GreedyArg = true)]
        public void SaveCustomPosition(Client sender, string name)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.Support)
            {
                return;
            }

            string path = $@"{Constant.ConstantAssemblyInfo.WorkingDirectory}\Files\CustomPositions.txt";


            if (!Directory.Exists($@"{Constant.ConstantAssemblyInfo.WorkingDirectory}\Files\"))
            {
                Directory.CreateDirectory($@"{Constant.ConstantAssemblyInfo.WorkingDirectory}\Files\");

                APIExtensions.ConsoleOutput($@"[File] Utworzono ścieżkę {Constant.ConstantAssemblyInfo.WorkingDirectory}\Files\", ConsoleColor.Blue);
            }

            if (!File.Exists(path))
            {
                File.Create(path);

                APIExtensions.ConsoleOutput($@"[File] Utworzono plik {path}", ConsoleColor.Blue);
            }

            var data = File.ReadAllLines(path).ToList();
            
            data.Add($"[{DateTime.Now}] {name} Pos: {sender.position} Rot: {sender.rotation} Autor: {sender.GetAccountController().AccountData.Name}");

            File.WriteAllLines(path, data);
            sender.Notify($"Zapisano pozycję: {name}");
        }
    }
}