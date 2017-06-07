using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Controllers;
using Serverside.Core.Extensions;
using Serverside.Core.Login;


namespace Serverside.Jobs
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

    public class GCWorker : IDisposable
    {
        public AccountController Player { get; }
        private List<Vector3> NonVisitedPoints { get; set; }
        private bool InProgress { get; set; }
        private Random Random { get; set; }
        private int Count { get; set; }

        private NetHandle Vehicle { get; }

        private ColShape CurrentShape { get; set; }

        public GCWorker(AccountController player, NetHandle vehicle)
        {
            Player = player;
            InProgress = true;
            NonVisitedPoints = GarbageCollectorHelper.GarbagePositions;
            Random = new Random();
            Vehicle = vehicle;
        }

        public void Dispose()
        {
            InProgress = false;
            NonVisitedPoints = null;
            Random = null;
            API.shared.deleteColShape(CurrentShape);
            CurrentShape = null;
            API.shared.triggerClientEvent(Player.Client, "DisposeJobComponents");
        }

        public void Start()
        {
            var v = NonVisitedPoints[Random.Next(NonVisitedPoints.Count)];
            NonVisitedPoints.Remove(v);

            API.shared.triggerClientEvent(Player.Client, "DrawJobComponents", v, 318);
            CurrentShape = API.shared.createCylinderColShape(v, 2f, 3f);
            CurrentShape.setData("Position", v);
            CurrentShape.onEntityEnterColShape += CurrentShapeOnEntityEnterColShape;
        }

        private void CurrentShapeOnEntityEnterColShape(ColShape shape, NetHandle entity)
        {
            if (API.shared.getEntityPosition(entity).DistanceTo2D(API.shared.getEntityPosition(Vehicle)) < 25 && API.shared.getEntityType(entity) == EntityType.Player && API.shared.getPlayerVehicle(API.shared.getPlayerFromHandle(entity)).IsNull && Count < 10)
            {
                //Dodać animację
                Count++;
                Player.Client.Notify(String.Format("Pomyślnie rozładowano kontener. Zapełnienie: {0}/10.", Count));
                API.shared.deleteColShape(shape);
                API.shared.playSoundFrontEnd(API.shared.getPlayerFromHandle(entity), "CHECKPOINT_NORMAL", "HUD_MINI_GAME_SOUNDSET");
                DrawNextPoint(false);
            }
            else if (API.shared.getEntityPosition(entity).DistanceTo2D(API.shared.getEntityPosition(Vehicle)) < 25 && API.shared.getEntityType(entity) == EntityType.Player && API.shared.getPlayerVehicle(API.shared.getPlayerFromHandle(entity)).IsNull && Count == 10)
            {
                Player.Client.Notify("Śmieciarka została zapełniona udaj się na wysypisko, aby ją rozładować.");
                API.shared.playSoundFrontEnd(API.shared.getPlayerFromHandle(entity), "CHECKPOINT_NORMAL", "HUD_MINI_GAME_SOUNDSET");
                DrawNextPoint(true);
                InProgress = false;
            }
            else if (!InProgress)
            {
                var characterController = Player.CharacterController;
                for (int i = 0; i < Count; i++)
                {
                    characterController.Character.MoneyJob += 25;
                }
                characterController.Save();
                Player.Client.Notify(
                    $"Zakończyłeś pracę operatora śmieciarki, zarobiłeś: ${characterController.Character.MoneyJob}.");
            }
            else
            {
                Player.Client.Notify("Twoja śmieciarka jest za daleko, aby można było pomyślnie załadować kontener.");
            }
        }

        private void DrawNextPoint(bool end)
        {
            API.shared.triggerClientEvent(Player.Client, "DisposeJobComponents");
            if (end)
            {
                API.shared.triggerClientEvent(Player.Client, "DrawJobComponents", GarbageCollectorHelper.DestinationPosition, 318);
                CurrentShape = API.shared.createCylinderColShape(GarbageCollectorHelper.DestinationPosition, 2f, 3f);
                CurrentShape.setData("Position", GarbageCollectorHelper.DestinationPosition);
                CurrentShape.onEntityEnterColShape += CurrentShapeOnEntityEnterColShape;
                return;
            }

            var v = NonVisitedPoints[Random.Next(NonVisitedPoints.Count)];
            NonVisitedPoints.Remove(v);

            API.shared.triggerClientEvent(Player.Client, "DrawJobComponents", v, 318);
            CurrentShape = API.shared.createCylinderColShape(v, 2f, 3f);
            CurrentShape.setData("Position", v);
            CurrentShape.onEntityEnterColShape += CurrentShapeOnEntityEnterColShape;
        }

        public void Redraw(Vector3 lastPosition)
        {
            CurrentShape = API.shared.createCylinderColShape(lastPosition, 2f, 3f);
            CurrentShape.setData("Position", lastPosition);
            CurrentShape.onEntityEnterColShape += CurrentShapeOnEntityEnterColShape;

            API.shared.triggerClientEvent(Player.Client, "DrawJobComponents", lastPosition, 318);
        }

        public Vector3 GetLastPoint()
        {
            return (Vector3)CurrentShape.getData("Position");
        }
    }

    public class GarbageCollector
    {
        public List<GCWorker> Workers { get; set; }
        private NetHandle VehicleHandle { get; }

        public GarbageCollector(NetHandle vehicle)
        {
            Workers = new List<GCWorker>();
            VehicleHandle = vehicle;
            RPLogin.OnPlayerLogin += RPLogin_OnPlayerLogin;
        }

        private void RPLogin_OnPlayerLogin(AccountController player)
        {
            //Tutaj na wypadek jak gracz dostanie kicka to żeby miał nadal ten sam postęp.
            //&& player.Editor.LastLoginTime.HasValue && player.Editor.LastLoginTime.Value.Minute.CompareTo(DateTime.Now.Minute) < 30
            if (Workers.Any(w => w.Player.AccountId == player.AccountId))
            {
                player.Client.Notify("Zostałeś rozłączony z serwerem, zadbaliśmy aby zapisać postępy...");
                player.Client.Notify("twojej pracy.");

                //var point = Workers.Single(p => p.Player.GetAccountController().CharacterController.Character.Id == player.CharacterController.Character.Id).GetLastPoint();

                Workers.Remove(Workers.Single(p => p.Player.CharacterController.Character.Id == player.CharacterController.Character.Id));
                var worker = new GCWorker(player, VehicleHandle);
                Workers.Add(worker);
            }
        }

        public void AddPlayer(AccountController player)
        {
            Workers.Add(new GCWorker(player, VehicleHandle));
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