using GTANetworkServer;
using GTANetworkShared;
using Serverside.Controllers;
using Serverside.Core.Extensions;
using Serverside.Jobs.DustMan;
using System.Collections.Generic;
using System.Linq;

namespace Serverside.Jobs.Dustman
{
    public static class GarbageCollectorHelper
    {
        public static List<Vector3> GarbagePositions
        {
            get
            {
                return new List<Vector3>
                {
                    new Vector3(-10f, -1033f, 28f),
                    new Vector3(7f, -1031f, 29.16f),
                    new Vector3(128f, -1056f, 29.1f),
                    new Vector3(474f, -602f, 28f),
                    new Vector3(439f, -1063f, 29.21f),
                    new Vector3(94f, -1440f, 29f),
                    new Vector3(138f, -1667f, 29f),
                    new Vector3(867f, -1576f, 30f),
                    new Vector3(826f, -1062f, 27f),
                    new Vector3(740f, -987.4f, 24f)
                };
            }
        }

        public static Vector3 DestinationPosition
        {
            get
            {
                return new Vector3();
            }
        }
    }

    public class Dustman
    {
        public List<DustmanWorker> Workers { get; } = new List<DustmanWorker>();
        public VehicleController Controller { get; set; }

        public Dustman(VehicleController vehicle)
        {
            Controller = vehicle;
            AccountController.OnPlayerCharacterLogin += RPLogin_OnPlayerLogin;
        }

        private void RPLogin_OnPlayerLogin(Client sender, AccountController account)
        {
            //Tutaj na wypadek jak gracz dostanie kicka to żeby miał nadal ten sam postęp.
            //&& player.Editor.LastLoginTime.HasValue && player.Editor.LastLoginTime.Value.Minute.CompareTo(DateTime.Now.Minute) < 30
            if (Workers.Any(w => w.Player.AccountId == account.AccountId))
            {
                sender.Notify("Zostałeś rozłączony z serwerem, zadbaliśmy aby zapisać postępy...");
                sender.Notify("twojej pracy.");

                //var point = Workers.Single(p => p.Player.GetAccountController().CharacterController.Character.Id == player.CharacterController.Character.Id).GetLastPoint();

                Workers.Remove(Workers.Single(p => p.Player.CharacterController.Character.Id == account.CharacterController.Character.Id));
                var worker = new DustmanWorker(account, Controller.Vehicle);
                Workers.Add(worker);
            }
        }

        public void AddPlayer(AccountController player)
        {
            //Workers.Add(new DustmanWorker(player, DustmanWorker));
        }

        public void RemovePlayer(AccountController player)
        {
            Workers.Single(p => p.Player == player).Dispose();
            Workers.Remove(Workers.First(p => p.Player == player));
        }

        public void StartJob(AccountController player)
        {
            player.Client.Notify("Rozpocząłeś pracę operatora śmieciarki, udaj się do wyznaczonych punków,...");
            player.Client.Notify("aby rozładować śmietniki.");
            Workers.Single(p => p.Player == player).Start();
        }
    }
}