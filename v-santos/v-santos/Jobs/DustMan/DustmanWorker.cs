using System;
using System.Collections.Generic;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Controllers;
using Serverside.Core.Extensions;

namespace Serverside.Jobs.DustMan
{
    public class DustmanWorker : IDisposable
    {
        public AccountController Player { get; }
        private List<Vector3> NonVisitedPoints { get; set; }
        private bool InProgress { get; set; }
        private Random Random { get; set; }
        private int Count { get; set; }

        private NetHandle Vehicle { get; }

        private ColShape CurrentShape { get; set; }

        public DustmanWorker(AccountController player, NetHandle vehicle)
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
}