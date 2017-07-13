using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Shared.Math;
using Serverside.Controllers;
using Serverside.Core.Extensions;
using Serverside.Jobs.Base;

namespace Serverside.Jobs.Greenkeeper
{
    public class GreenkeeperWorker : JobWorkerController
    {

        private bool InProgress { get; set; }
        private int Count { get; set; }
        private List<Vector3> VisitedPoints { get; set; }

        public GreenkeeperWorker(API api, AccountController player, JobVehicleController vehicle) : base(api, player, vehicle)
        {
            player.Client.triggerEvent("JobTextVisibility", true);
            Player = player;
            InProgress = true;
            JobVehicle = vehicle;

            Api.onUpdate += OnUpdateHandler;
            VisitedPoints = new List<Vector3>();
        }

        private void OnUpdateHandler()
        {
            if (Player.Client.isInVehicle && Player.Client.vehicle == JobVehicle.Vehicle)
            {
                if (VisitedPoints.Any(p => p.DistanceTo(Player.Client.position) < 20) || VisitedPoints.Any(p => p == Player.Client.position)) return;
                Count++;
                VisitedPoints.Add(Player.Client.position);
                API.shared.triggerClientEvent(Player.Client, "JobText_Changed", $"{Count}/300m\u00B2");
            }
        }

        public override void Start()
        {
            Player.Client.Notify("Rozpocząłeś pracę ogrodnika, jeździj po polu golfowym, aby kosić trawnik.");
            Player.Client.Notify("Jeżeli wyjedziesz z obszaru pola golfowego zostanie wysłane zgłoszenie na numer alarmowy.");
        }

        public override void Stop()
        {
            InProgress = false;
            Api.onUpdate -= OnUpdateHandler;
        }
    }
}