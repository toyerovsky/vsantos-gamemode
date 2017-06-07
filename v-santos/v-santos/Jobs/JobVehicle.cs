//using System.Linq;
//using GTANetworkServer;
//using GTANetworkShared;
//using Serverside.Controllers;
//using Serverside.Core.Extensions;

//namespace Serverside.Jobs
//{
//    internal class GreenkeeperVehicle : VehicleController
//    {
//        private Greenkeeper Greenkeeper { get; }

//        public GreenkeeperVehicle(API api, Vector3 pos, Vector3 rot, VehicleHash hash, int job) : base(api, pos, rot, hash, job)
//        {
//            Greenkeeper = new Greenkeeper(Handle.GetVehicleController());
//            API.shared.onPlayerEnterVehicle += (sender, vehicle) =>
//            {
//                if (vehicle == Handle && Greenkeeper.Workers.All(w => w.Player.Client != sender))
//                {
//                    var player = sender.GetAccountController();
//                    if (player.CharacterController.Character.Job == Job)
//                    {
//                        Greenkeeper.AddPlayer(player);
//                        Greenkeeper.StartJob(player);
//                        API.shared.triggerClientEvent(player.Client, "JobTextVisibility", true);
//                    }
//                }
//            };

//            API.shared.onPlayerExitVehicle += (sender, vehicle) =>
//            {
//                if (vehicle == Handle)
//                {
//                    API.shared.triggerClientEvent(sender, "JobTextVisibility", false);
//                }
//            };
//        }
//    }
//}