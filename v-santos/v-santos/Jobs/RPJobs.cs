//using System.Collections.Generic;
//using GTANetworkServer;
//using GTANetworkShared;
//using Serverside.Controllers;

//namespace Serverside.Jobs
//{
//    public class RPJobs : Script
//    {
//        //Śmieciarki
//        private List<VehicleController> GcVehicles { get; set; }
//        //Kosiarki
//        private List<VehicleController> GkVehicles { get; set; }  

//        public RPJobs()
//        {
//            API.onResourceStart += API_onResourceStart;  
//        }

//        private void API_onResourceStart()
//        {
//            API.consoleOutput("RPJobs zostało uruchomione pomyślnie.");

//            GcVehicles = new List<VehicleController>
//            {
//                new GarbageCollectorVehicle(),
//                new GarbageCollectorVehicle(),
//                new GarbageCollectorVehicle(),
//                new GarbageCollectorVehicle()
//            };

//            GkVehicles = new List<VehicleController>
//            {
//                new GreenkeeperVehicle(API, new Vector3(-1334f, 132f, 57f), new Vector3(1f, 1f, 1f), VehicleHash.Mower, 1),
//                new GreenkeeperVehicle(API, new Vector3(-1334f, 135f, 57f), new Vector3(1f, 1f, 1f), VehicleHash.Mower, 1),
//                new GreenkeeperVehicle(API, new Vector3(-1334f, 138f, 57f), new Vector3(1f, 1f, 1f), VehicleHash.Mower, 1),
//                new GreenkeeperVehicle(API, new Vector3(-1334f, 129f, 57f), new Vector3(1f, 1f, 1f), VehicleHash.Mower, 1)
//            };
//        }
//    }
//}