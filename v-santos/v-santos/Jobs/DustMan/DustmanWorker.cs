/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Serverside.Controllers;
using Serverside.Core.Extensions;
using Serverside.Jobs.Base;
using Serverside.Jobs.Dustman.Models;

namespace Serverside.Jobs.Dustman
{
    public class DustmanWorker : JobWorkerController
    {
        private List<GarbageModel> NonVisitedPoints { get; set; }
        private bool InProgress { get; set; }
        private int Count { get; set; }

        private DateTime NextUpdate { get; set; }

        private CylinderColShape CurrentShape { get; set; }

        public DustmanWorker(API api, AccountController player, JobVehicleController vehicle) : base (api, player, vehicle)
        {

            Api.onUpdate += OnUpdate;
            Player = player;
            InProgress = true;
            NonVisitedPoints = RPJobs.Garbages;
            JobVehicle = vehicle;
        }

        private void OnUpdate()
        {
            if (!Player.Client.isInVehicle && DateTime.Now >= NextUpdate && Player.Client.position.DistanceTo2D(JobVehicle.Vehicle.position) > 25)
            {
                Player.Client.Notify("Oddaliłeś się od swojego pojazdu. Praca została przerwana.");
                Stop();
                JobVehicle.Respawn();
            }
        }

        public override void Start()
        {
            var v = NonVisitedPoints[new Random().Next(NonVisitedPoints.Count)];
            NonVisitedPoints.Remove(v);

            API.shared.triggerClientEvent(Player.Client, "DrawJobComponents", v, 318);
            CurrentShape = API.shared.createCylinderColShape(v.Position, 2f, 3f);
            CurrentShape.onEntityEnterColShape += CurrentShapeOnEntityEnterColShape;
        }

        public override void Stop()
        {
            InProgress = false;
            NonVisitedPoints = null;
            API.shared.deleteColShape(CurrentShape);
            CurrentShape = null;
            API.shared.triggerClientEvent(Player.Client, "DisposeJobComponents");
        }

        private void CurrentShapeOnEntityEnterColShape(ColShape shape, NetHandle entity)
        {
            if (entity == Player.Client && Player.Client.position.DistanceTo2D(JobVehicle.Vehicle.position) <= 25 && !Player.Client.isInVehicle && Count < 10)
            {
                //Dodać animację
                Count++;
                Player.Client.Notify($"Pomyślnie rozładowano kontener. Zapełnienie: {Count}/10.");
                API.shared.deleteColShape(shape);
                API.shared.playSoundFrontEnd(API.shared.getPlayerFromHandle(entity), "CHECKPOINT_NORMAL", "HUD_MINI_GAME_SOUNDSET");
                DrawNextPoint(false);
            }
            else if (entity == Player.Client && Player.Client.position.DistanceTo2D(JobVehicle.Vehicle.position) <= 25 && !Player.Client.isInVehicle && Count == 10)
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
                API.shared.triggerClientEvent(Player.Client, "DrawJobComponents", RPJobs.GetRandomGarbage().Position, 318);
                CurrentShape = API.shared.createCylinderColShape(RPJobs.GetRandomGarbage().Position, 2f, 3f);
                CurrentShape.onEntityEnterColShape += CurrentShapeOnEntityEnterColShape;
                return;
            }

            var v = NonVisitedPoints[new Random().Next(NonVisitedPoints.Count)];
            NonVisitedPoints.Remove(v);

            API.shared.triggerClientEvent(Player.Client, "DrawJobComponents", v, 318);
            CurrentShape = API.shared.createCylinderColShape(v.Position, 2f, 3f);

            CurrentShape.onEntityEnterColShape += CurrentShapeOnEntityEnterColShape;
        }

        public void Redraw(Vector3 lastPosition)
        {
            CurrentShape = API.shared.createCylinderColShape(lastPosition, 2f, 3f);
            CurrentShape.onEntityEnterColShape += CurrentShapeOnEntityEnterColShape;

            API.shared.triggerClientEvent(Player.Client, "DrawJobComponents", lastPosition, 318);
        }

        public Vector3 GetLastPoint() => CurrentShape.Center;

    }
}