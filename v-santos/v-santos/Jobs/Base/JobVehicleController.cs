using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Serverside.Controllers;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Database.Models;

namespace Serverside.Jobs.Base
{
    public abstract class JobVehicleController : VehicleController
    {
        protected JobVehicleController(Vehicle data) : base(data)
        {
        }

        protected JobVehicleController(FullPosition spawnPosition, VehicleHash hash, string numberplate, int numberplatestyle, int creatorId, Color primaryColor, Color secondaryColor, float enginePowerMultiplier = 0, float engineTorqueMultiplier = 0, Character character = null, Group @group = null) : base(spawnPosition, hash, numberplate, numberplatestyle, creatorId, primaryColor, secondaryColor, enginePowerMultiplier, engineTorqueMultiplier, character, @group)
        {
        }

        public virtual void Respawn()
        {
            Repair();
            VehicleData.Fuel = GetFuelTankSize((VehicleClass) Vehicle.Class);
            Vehicle.position = new Vector3(VehicleData.SpawnPositionX, VehicleData.SpawnPositionY,
                VehicleData.SpawnPositionZ);
            Vehicle.rotation = new Vector3(VehicleData.SpawnRotationX, VehicleData.SpawnRotationY, 
                VehicleData.SpawnRotationZ);
        }
    }
}