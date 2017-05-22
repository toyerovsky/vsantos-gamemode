using System;
using System.Data.Entity;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Core;
using Serverside.Core.Description;
using Serverside.Database;
using Serverside.Database.Models;
using Vehicle = Serverside.Database.Models.Vehicle;

namespace Serverside.Controllers
{
    public class VehicleController : IDisposable
    {
        public long VehicleId { get; set; }
        public GTANetworkServer.Vehicle Vehicle { get; set; }

        public Vehicle Data = new Vehicle();
        public Core.Description.Description Description;

        //Wczytywanie pojazdu
        public VehicleController(Vehicle data)
        {
            Data = data;
            Data.IsSpawned = true;
            Vehicle = API.shared.createVehicle(data.VehicleHash,
                new Vector3(data.SpawnPositionX, data.SpawnPositionY, data.SpawnPositionZ),
                new Vector3(data.SpawnRotationX, data.SpawnRotationY, data.SpawnPositionZ), data.PrimaryColor,
                data.SecondaryColor);
            Vehicle.setData("VehicleController", this);
        }

        //Dodawanie pojazdu
        public VehicleController(FullPosition spawnPosition, Character character, string name, int creatorId, int primaryColor, 
            int secondaryColor, float enginePowerMultiplier, float engineTorqueMultiplier)
        {
            this.Data.Name = name;
            this.Data.NumberPlate = "";
            this.Data.Character = character;
            this.Data.CreatorId = creatorId;
            this.Data.SpawnPositionX = spawnPosition.Position.X;
            this.Data.SpawnPositionY = spawnPosition.Position.Y;
            this.Data.SpawnPositionZ = spawnPosition.Position.Z;
            this.Data.SpawnRotationX = spawnPosition.Rotation.X;
            this.Data.SpawnRotationY = spawnPosition.Rotation.Y;
            this.Data.SpawnRotationZ = spawnPosition.Rotation.Z;
            this.Data.PrimaryColor = primaryColor;
            this.Data.SecondaryColor = secondaryColor;
            this.Data.EnginePowerMultipler = enginePowerMultiplier;
            this.Data.EngineTorqueMultipler = engineTorqueMultiplier;
            this.Data.IsSpawned = true;
            ContextFactory.Instance.Vehicles.Add(Data);
            ContextFactory.Instance.SaveChanges();

            Vehicle = API.shared.createVehicle(Data.VehicleHash,
                new Vector3(Data.SpawnPositionX, Data.SpawnPositionY, Data.SpawnPositionZ),
                new Vector3(Data.SpawnRotationX, Data.SpawnRotationY, Data.SpawnPositionZ), Data.PrimaryColor,
                Data.SecondaryColor);
            Vehicle.setData("VehicleController", this);
        }

        public void Save()
        {
            this.Data.PrimaryColor = this.Vehicle.primaryColor;
            this.Data.SecondaryColor = this.Vehicle.secondaryColor;
            this.Data.EnginePowerMultipler = this.Vehicle.enginePowerMultiplier;
            this.Data.NumberPlateStyle = this.Vehicle.numberPlateStyle;
            this.Data.NumberPlate = this.Vehicle.numberPlate;

            ContextFactory.Instance.Vehicles.Attach(Data);
            ContextFactory.Instance.Entry(Data).State = EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }

        public void ChangeSpawnPosition()
        {
            this.Data.SpawnPositionX = this.Vehicle.position.X;
            this.Data.SpawnPositionY = this.Vehicle.position.Y;
            this.Data.SpawnPositionZ = this.Vehicle.position.Z;
            this.Data.SpawnRotationX = this.Vehicle.rotation.X;
            this.Data.SpawnRotationY = this.Vehicle.rotation.Y;
            this.Data.SpawnRotationZ = this.Vehicle.rotation.Z;
            Save();
        }

        public void Dispose()
        {
            Data.IsSpawned = false;
            Save();
            API.shared.deleteEntity(this.Vehicle);
        }
    }
}