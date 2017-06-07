using System.Collections.Generic;
using GTANetworkServer;
using GTANetworkShared;

namespace Serverside.Jobs
{
    public class RPJobs : Script
    {
        //Śmieciarki
        private List<JobVehicle> GCVehicles { get; set; }
        //Kosiarki
        private List<JobVehicle> GKVehicles { get; set; }  

        public RPJobs()
        {
            API.onResourceStart += API_onResourceStart;  
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("RPJobs zostało uruchomione pomyślnie.");

            GCVehicles = new List<JobVehicle>
            {
                new GarbageCollectorVehicle(API, new Vector3(1712f, -1566f, 112f), new Vector3(-0.1f, -0.1f, 80f), VehicleHash.Trash, 0),
                new GarbageCollectorVehicle(API, new Vector3(1712f, -1580f, 112f), new Vector3(-0.1f, -0.1f, 99f), VehicleHash.Trash, 0),
                new GarbageCollectorVehicle(API, new Vector3(1712f, -1558f, 112f), new Vector3(-0.1f, -0.1f, 80f), VehicleHash.Trash, 0),
                new GarbageCollectorVehicle(API, new Vector3(1710f, -1572f, 112f), new Vector3(-0.1f, -0.1f, 72f), VehicleHash.Trash, 0)
            };

            GKVehicles = new List<JobVehicle>
            {
                new GreenkeeperVehicle(API, new Vector3(-1334f, 132f, 57f), new Vector3(1f, 1f, 1f), VehicleHash.Mower, 1),
                new GreenkeeperVehicle(API, new Vector3(-1334f, 135f, 57f), new Vector3(1f, 1f, 1f), VehicleHash.Mower, 1),
                new GreenkeeperVehicle(API, new Vector3(-1334f, 138f, 57f), new Vector3(1f, 1f, 1f), VehicleHash.Mower, 1),
                new GreenkeeperVehicle(API, new Vector3(-1334f, 129f, 57f), new Vector3(1f, 1f, 1f), VehicleHash.Mower, 1)
            };
        }
    }
}