using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Core;
using Serverside.Core.Extensions;

namespace Serverside.Jobs
{
    public abstract class JobVehicle
    {
        protected API Api { get; set; }
        private Vector3 Position { get; set; }
        private Vector3 Rotation { get; set; }
        protected NetHandle Handle { get; set; }
        private VehicleHash Hash { get; set; }

        protected int Job { get; set; }

        protected JobVehicle(API api, Vector3 pos, Vector3 rot, VehicleHash hash, int job)
        {
            Api = api;
            Position = pos;
            Rotation = rot;
            Hash = hash;
            Job = job;

            Handle = Api.createVehicle(Hash, Position, Rotation, 0, 0);
            
            Api.onPlayerEnterVehicle += Api_onPlayerEnterVehicle;
        }

        private void Api_onPlayerEnterVehicle(Client sender, NetHandle vehicle)
        {
            //TODO rozwiązać kradnięcie pojazdów przez graczy
            if (vehicle == Handle)
            {
                var player = sender.GetAccountController();
                if (player.CharacterController.Character.Job != Job)
                {
                    Api.warpPlayerOutOfVehicle(sender);
                    sender.Notify("Aby używać tego pojazdu musisz podjąć odpowiednią pracę!");
                }
            }
        }

        public void Respawn()
        {
            API.shared.setEntityPosition(Handle, Position);
            API.shared.setEntityRotation(Handle, Rotation);
            API.shared.repairVehicle(Handle);
            
            //TODO uzupełnianie paliwa
        }
    }

    internal class GarbageCollectorVehicle : JobVehicle
    {
        private GarbageCollector GarbageCollector { get; }

        public GarbageCollectorVehicle(API api, Vector3 pos, Vector3 rot, VehicleHash hash, int job) : base(api, pos, rot, hash, job)
        {
            GarbageCollector = new GarbageCollector(Handle);
            Api.onPlayerEnterVehicle += (sender, vehicle) =>
            {
                if (vehicle == Handle && GarbageCollector.Workers.All(w => w.Player.Client != sender))
                {
                    var player = sender.GetAccountController();
                    if (player.CharacterController.Character.Job == Job)
                    {
                        GarbageCollector.AddPlayer(player);
                        GarbageCollector.StartJob(player);
                    }
                }
            };
        }
    }

    internal class GreenkeeperVehicle : JobVehicle
    {
        private Greenkeeper Greenkeeper { get; }

        public GreenkeeperVehicle(API api, Vector3 pos, Vector3 rot, VehicleHash hash, int job) : base(api, pos, rot, hash, job)
        {
            Greenkeeper = new Greenkeeper(Handle.GetVehicleController());
            Api.onPlayerEnterVehicle += (sender, vehicle) =>
            {
                if (vehicle == Handle && Greenkeeper.Workers.All(w => w.Player.Client != sender))
                {
                    var player = sender.GetAccountController();
                    if (player.CharacterController.Character.Job == Job)
                    {
                        Greenkeeper.AddPlayer(player);
                        Greenkeeper.StartJob(player);
                        API.shared.triggerClientEvent(player.Client, "JobTextVisibility", true);
                    }
                }
            };

            Api.onPlayerExitVehicle += (sender, vehicle) =>
            {
                if (vehicle == Handle)
                {
                    API.shared.triggerClientEvent(sender, "JobTextVisibility", false);
                }
            };
        }
    }
}