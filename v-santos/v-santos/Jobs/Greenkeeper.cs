using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Controllers;
using Serverside.Core.Extensions;

namespace Serverside.Jobs
{
    public class GKWorker : IDisposable
    {
        public AccountController Player { get; }

        private bool InProgress { get; set; }
        private int Count { get; set; }
        private VehicleController Vehicle { get; }
        private List<Vector3> VisitedPoints { get; set; }

        private ColShape GolfclubShape { get; set; }

        public GKWorker(AccountController player, VehicleController vehicle)
        {
            Player = player;
            InProgress = true;
            Vehicle = vehicle;

            API.shared.onUpdate += OnUpdateHandler;
            VisitedPoints = new List<Vector3>();
        }

        private void OnUpdateHandler()
        {
            if (API.shared.getPlayerVehicle(Player.Client) == Vehicle.Vehicle)
            {
                if (VisitedPoints.Any(p => p.DistanceTo(Player.Client.position) < 20) || VisitedPoints.Any(p => p == Player.Client.position)) return;
                Count++;
                VisitedPoints.Add(Player.Client.position);
                API.shared.triggerClientEvent(Player.Client, "JobText_Changed", $"{Count}/500m\u00B2"); 
            }
        }

        public void Dispose()
        {
            InProgress = false;
        }
    }

    public class Greenkeeper
    {
        public List<GKWorker> Workers { get; }
        private VehicleController Vehicle { get; set; }

        public Greenkeeper(VehicleController vehicle)
        {
            Workers = new List<GKWorker>();
            Vehicle = vehicle;
        }

        public void AddPlayer(AccountController player)
        {
            Workers.Add(new GKWorker(player, Vehicle));
        }

        public void RemovePlayer(AccountController player)
        {
            Workers.Single(p => p.Player == player).Dispose();
            Workers.Remove(Workers.First(p => p.Player == player));
        }

        public void StartJob(AccountController player)
        {
            player.Client.Notify("Rozpocząłeś pracę ogrodnika, jeździj po polu golfowym, aby kosić trawnik.");
            player.Client.Notify("Jeżeli wyjedziesz z obszaru pola golfowego zostanie wysłane zgłoszenie na numer alarmowy.");
            //TODO: wysyłanie zgłoszenia na 911
        }
    }
}
