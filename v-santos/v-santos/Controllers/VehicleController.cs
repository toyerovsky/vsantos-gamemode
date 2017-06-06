using System;
using System.Data.Entity;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Core;
//using Serverside.Core.Description;
using Serverside.Database;
using Serverside.Database.Models;
using Vehicle = Serverside.Database.Models.Vehicle;
using Serverside.Core.Extensions;
using GTANetworkServer.Constant;

namespace Serverside.Controllers
{
    public class VehicleController : IDisposable
    {
        public long VehicleId { get; set; }
        public GTANetworkServer.Vehicle Vehicle { get; set; }

        public Vehicle VehicleData = new Vehicle();
        public Core.Description.Description Description;

        //Wczytywanie pojazdu
        public VehicleController(Vehicle data)
        {
            VehicleData = data;
            VehicleData.IsSpawned = true;
            Vehicle = API.shared.createVehicle(data.VehicleHash,
                new Vector3(data.SpawnPositionX, data.SpawnPositionY, data.SpawnPositionZ),
                new Vector3(data.SpawnRotationX, data.SpawnRotationY, data.SpawnPositionZ), 0, 0);
            API.shared.setVehicleNumberPlate(Vehicle.handle, data.NumberPlate);
            API.shared.setVehicleNumberPlateStyle(Vehicle.handle, data.NumberPlateStyle);
            Color primary = VehicleData.PrimaryColor.ToColor();
            Color secondary = VehicleData.SecondaryColor.ToColor();
            API.shared.setVehicleCustomPrimaryColor(Vehicle, primary.red, primary.green, primary.blue);
            API.shared.setVehicleCustomSecondaryColor(Vehicle, primary.red, primary.green, primary.blue);
            API.shared.setVehicleEnginePowerMultiplier(Vehicle, VehicleData.EnginePowerMultipler);
            API.shared.setVehicleEngineTorqueMultiplier(Vehicle, VehicleData.EngineTorqueMultipler);
            API.shared.setEntitySyncedData(Vehicle.handle, "_maxfuel", VehicleData.Fuel);
            API.shared.setEntitySyncedData(Vehicle.handle, "_fuel", VehicleData.Fuel);
            API.shared.setEntitySyncedData(Vehicle.handle, "_fuelConsumption", VehicleData.FuelConsumption);
            API.shared.setEntitySyncedData(Vehicle.handle, "_milage", VehicleData.Milage);
            Vehicle.setData("VehicleController", this);
            RPCore.Add(this);
        }

        //Dodawanie pojazdu
        public VehicleController(FullPosition spawnPosition, VehicleHash hash, string numberplate, int numberplatestyle, int creatorId, Color primaryColor,
            Color secondaryColor, float enginePowerMultiplier = 1.0f, float engineTorqueMultiplier = 1.0f, Character character = null, Group group = null)
        {
            this.VehicleData.VehicleHash = hash;
            this.VehicleData.NumberPlate = numberplate;
            this.VehicleData.NumberPlateStyle = numberplatestyle;
            this.VehicleData.Character = character;
            this.VehicleData.Group = group;
            this.VehicleData.CreatorId = creatorId;
            this.VehicleData.SpawnPositionX = spawnPosition.Position.X;
            this.VehicleData.SpawnPositionY = spawnPosition.Position.Y;
            this.VehicleData.SpawnPositionZ = spawnPosition.Position.Z;
            this.VehicleData.SpawnRotationX = spawnPosition.Rotation.X;
            this.VehicleData.SpawnRotationY = spawnPosition.Rotation.Y;
            this.VehicleData.SpawnRotationZ = spawnPosition.Rotation.Z;
            this.VehicleData.PrimaryColor = primaryColor.ToHex();
            this.VehicleData.SecondaryColor = secondaryColor.ToHex();
            this.VehicleData.EnginePowerMultipler = enginePowerMultiplier;
            this.VehicleData.EngineTorqueMultipler = engineTorqueMultiplier;

            Vehicle = API.shared.createVehicle(VehicleData.VehicleHash,
                new Vector3(VehicleData.SpawnPositionX, VehicleData.SpawnPositionY, VehicleData.SpawnPositionZ),
                new Vector3(VehicleData.SpawnRotationX, VehicleData.SpawnRotationY, VehicleData.SpawnPositionZ), 0, 0);
            API.shared.setVehicleNumberPlate(Vehicle.handle, VehicleData.NumberPlate);
            API.shared.setVehicleNumberPlateStyle(Vehicle.handle, VehicleData.NumberPlateStyle);
            Color primary = VehicleData.PrimaryColor.ToColor();
            Color secondary = VehicleData.SecondaryColor.ToColor();
            API.shared.setVehicleCustomPrimaryColor(Vehicle.handle, primary.red, primary.green, primary.blue);
            API.shared.setVehicleCustomSecondaryColor(Vehicle.handle, secondary.red, secondary.green, secondary.blue);
            API.shared.setVehicleEnginePowerMultiplier(Vehicle.handle, VehicleData.EnginePowerMultipler);
            API.shared.setVehicleEngineTorqueMultiplier(Vehicle.handle, VehicleData.EngineTorqueMultipler);

            this.VehicleData.FuelTank = GetFuelTankSize((VehicleClass)Vehicle.Class);
            API.shared.setEntitySyncedData(Vehicle.handle, "_maxfuel", VehicleData.Fuel);
            this.VehicleData.Fuel = VehicleData.FuelTank * 0.2f;
            API.shared.setEntitySyncedData(Vehicle.handle, "_fuel", VehicleData.Fuel);
            this.VehicleData.FuelConsumption = Vehicle.maxAcceleration / 0.2f;
            API.shared.setEntitySyncedData(Vehicle.handle, "_fuelConsumption", VehicleData.FuelConsumption);
            this.VehicleData.Milage = 0.0f;
            API.shared.setEntitySyncedData(Vehicle.handle, "_milage", VehicleData.Milage);
            this.VehicleData.IsSpawned = true;

            ContextFactory.Instance.Vehicles.Add(VehicleData);
            ContextFactory.Instance.SaveChanges();

            Vehicle.setData("VehicleController", this);
            RPCore.Add(this);
        }

        public void Save()
        {
            this.VehicleData.Health = this.Vehicle.health;
            this.VehicleData.PrimaryColor = this.Vehicle.customPrimaryColor.ToHex();
            this.VehicleData.SecondaryColor = this.Vehicle.customSecondaryColor.ToHex();
            this.VehicleData.EnginePowerMultipler = this.Vehicle.enginePowerMultiplier;
            this.VehicleData.NumberPlateStyle = this.Vehicle.numberPlateStyle;
            this.VehicleData.NumberPlate = this.Vehicle.numberPlate;
            this.VehicleData.Fuel = API.shared.getEntitySyncedData(Vehicle.handle, "_fuel");
            this.VehicleData.FuelConsumption = API.shared.getEntitySyncedData(Vehicle.handle, "_fuelConsumption");
            this.VehicleData.Milage = API.shared.getEntitySyncedData(Vehicle.handle, "_milage");

            ContextFactory.Instance.Vehicles.Attach(VehicleData);
            ContextFactory.Instance.Entry(VehicleData).State = EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }

        public void ChangeFuelConsumption(float fc)
        {
            API.shared.setEntitySyncedData(Vehicle.handle, "_fuelConsumption", fc);
            Save();
        }

        public void ChangeSpawnPosition()
        {
            this.VehicleData.SpawnPositionX = this.Vehicle.position.X;
            this.VehicleData.SpawnPositionY = this.Vehicle.position.Y;
            this.VehicleData.SpawnPositionZ = this.Vehicle.position.Z;
            this.VehicleData.SpawnRotationX = this.Vehicle.rotation.X;
            this.VehicleData.SpawnRotationY = this.Vehicle.rotation.Y;
            this.VehicleData.SpawnRotationZ = this.Vehicle.rotation.Z;
            Save();
        }

        private static float GetFuelTankSize(VehicleClass vc)
        {
            switch (vc)
            {
                case VehicleClass.Compacts:
                    return 70.0f;
                case VehicleClass.Sedans:
                    return 80.0f;
                case VehicleClass.SUVs:
                    return 85.0f;
                case VehicleClass.Coupes:
                    return 60.0f;
                case VehicleClass.Muscle:
                    return 70.0f;
                case VehicleClass.SportsClassics:
                    return 70.0f;
                case VehicleClass.Sports:
                    return 60.0f;
                case VehicleClass.Super:
                    return 60.0f;
                case VehicleClass.Motorcycles:
                    return 40.0f;
                case VehicleClass.Offroad:
                    return 80.0f;
                case VehicleClass.Industrial:
                    return 200.0f;
                case VehicleClass.Utility:
                    return 100.0f;
                case VehicleClass.Vans:
                    return 80.0f;
                case VehicleClass.Cycles:
                    return 0.0f;
                case VehicleClass.Boats:
                    return 200.0f;
                case VehicleClass.Helicopters:
                    return 400.0f;
                case VehicleClass.Planes:
                    return 0.0f;
                case VehicleClass.Service:
                    return 250.0f;
                case VehicleClass.Emergency:
                    return 90.0f;
                case VehicleClass.Military:
                    return 250.0f;
                case VehicleClass.Commercial:
                    return 250.0f;
                case VehicleClass.Trains:
                    return 0.0f;
                default:
                    return 70.0f;
            }
        }

        //private class FuelData
        //{
        //    float FuelTank { get; set; }
        //    float FuelConsumption { get; set; }
        //    FuelData(float ft, float fc)
        //    {
        //        FuelTank = ft;
        //        FuelConsumption = fc;
        //    }
        //}

        private void ReleaseUnmanagedResources()
        {
            VehicleData.IsSpawned = false;
            Save();
            API.shared.deleteEntity(this.Vehicle);
            RPCore.Remove(this);
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                Description?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~VehicleController()
        {
            Dispose(false);
        }
    }
}