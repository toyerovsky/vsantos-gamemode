﻿using GTANetworkServer;
using GTANetworkServer.Constant;
using GTANetworkShared;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Core.Extenstions;
//using Serverside.Core.Description;
using Serverside.Database;
using Serverside.Database.Models;
using Serverside.Items;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Vehicle = Serverside.Database.Models.Vehicle;

namespace Serverside.Controllers
{
    public class VehicleController : IDisposable
    {
        public long VehicleId { get; set; }
        public GTANetworkServer.Vehicle Vehicle { get; set; }
        public Vehicle VehicleData { get; set; }
        public Core.Description.Description Description;

        //Wczytywanie pojazdu
        public VehicleController(Vehicle data)
        {
            VehicleData = data;
            VehicleData.IsSpawned = true;
            
            Initialize();

            //Tylko tak da się nadawać zniszczenia?
            API.shared.breakVehicleDoor(Vehicle, 1, VehicleData.Door1Damage);
            API.shared.breakVehicleDoor(Vehicle, 2, VehicleData.Door2Damage);
            API.shared.breakVehicleDoor(Vehicle, 3, VehicleData.Door3Damage);
            API.shared.breakVehicleDoor(Vehicle, 4, VehicleData.Door4Damage);
            API.shared.breakVehicleWindow(Vehicle, 1, VehicleData.Window1Damage);
            API.shared.breakVehicleWindow(Vehicle, 2, VehicleData.Window2Damage);
            API.shared.breakVehicleWindow(Vehicle, 3, VehicleData.Window3Damage);
            API.shared.breakVehicleWindow(Vehicle, 4, VehicleData.Window4Damage);

            //Dodajemy tuning do pojazdu
            float engineMultipier = 0f;
            float torqueMultipier = 0f;

            foreach (var tuning in VehicleData.Tuning)
            {
                if ((ItemType)tuning.ItemType == ItemType.Tuning)
                {
                    if (tuning.FirstParameter != null && (TuningType)tuning.FirstParameter == TuningType.Speed)
                    {
                        if (tuning.SecondParameter != null) engineMultipier += (float)tuning.SecondParameter;
                        if (tuning.ThirdParameter != null) torqueMultipier += (float)tuning.ThirdParameter;
                    }
                }
            }

            //Te metody działają tak, że ujemny mnożnik zmniejsza 0 to normalnie a dodatni poprawia
            //Pola są potrzebne, ponieważ w salonie będą dostępne 3 wersje pojazdu
            //TODO: tańsza o 10% zmniejszone osiągi o -5f
            //TODO: normalna
            //TODO: droższa o 25% zwiększone osiągi o 5f
            API.shared.setVehicleEnginePowerMultiplier(Vehicle, engineMultipier);
            API.shared.setVehicleEngineTorqueMultiplier(Vehicle, torqueMultipier);

            API.shared.setEntitySyncedData(Vehicle.handle, "_maxfuel", VehicleData.Fuel);
            API.shared.setEntitySyncedData(Vehicle.handle, "_fuel", VehicleData.Fuel);
            API.shared.setEntitySyncedData(Vehicle.handle, "_fuelConsumption", VehicleData.FuelConsumption);
            API.shared.setEntitySyncedData(Vehicle.handle, "_milage", VehicleData.Milage);
            Vehicle.setData("VehicleController", this);
            RPEntityManager.Add(this);
        }

        //Dodawanie pojazdu
        public VehicleController(FullPosition spawnPosition, VehicleHash hash, string numberplate, int numberplatestyle, int creatorId, Color primaryColor,
            Color secondaryColor, float enginePowerMultiplier = 0f, float engineTorqueMultiplier = 0f, Character character = null, Database.Models.Group group = null)
        {
            VehicleData = new Vehicle
            {
                VehicleHash = hash,
                NumberPlate = numberplate,
                NumberPlateStyle = numberplatestyle,
                Character = character,
                Group = group,
                CreatorId = creatorId,
                SpawnPositionX = spawnPosition.Position.X,
                SpawnPositionY = spawnPosition.Position.Y,
                SpawnPositionZ = spawnPosition.Position.Z,
                SpawnRotationX = spawnPosition.Rotation.X,
                SpawnRotationY = spawnPosition.Rotation.Y,
                SpawnRotationZ = spawnPosition.Rotation.Z,
                PrimaryColor = primaryColor.ToHex(),
                SecondaryColor = secondaryColor.ToHex(),
                EnginePowerMultipler = enginePowerMultiplier,
                EngineTorqueMultipler = engineTorqueMultiplier,
                Tuning = new List<Database.Models.Item>(),
            };
               
            Initialize();

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
            RPEntityManager.Add(this);
        }

        /// <summary>
        /// Konstruktor do dodawania pojazdów bez obsługi bazy danych
        /// </summary>
        /// <param name="spawnPosition"></param>
        /// <param name="hash"></param>
        /// <param name="numberplate"></param>
        /// <param name="numberplatestyle"></param>
        /// <param name="creatorId"></param>
        /// <param name="primaryColor"></param>
        /// <param name="secondaryColor"></param>
        /// <param name="enginePowerMultiplier"></param>
        /// <param name="engineTorqueMultiplier"></param>
        protected VehicleController(FullPosition spawnPosition, VehicleHash hash, string numberplate, int numberplatestyle, int creatorId, Color primaryColor,
            Color secondaryColor, float enginePowerMultiplier = 0f, float engineTorqueMultiplier = 0f)
        {
            this._nonDbVehicle = true;

            VehicleData = new Vehicle
            {
                VehicleHash = hash,
                NumberPlate = numberplate,
                NumberPlateStyle = numberplatestyle,
                CreatorId = creatorId,
                SpawnPositionX = spawnPosition.Position.X,
                SpawnPositionY = spawnPosition.Position.Y,
                SpawnPositionZ = spawnPosition.Position.Z,
                SpawnRotationX = spawnPosition.Rotation.X,
                SpawnRotationY = spawnPosition.Rotation.Y,
                SpawnRotationZ = spawnPosition.Rotation.Z,
                PrimaryColor = primaryColor.ToHex(),
                SecondaryColor = secondaryColor.ToHex(),
                EnginePowerMultipler = enginePowerMultiplier,
                EngineTorqueMultipler = engineTorqueMultiplier,
                Tuning = new List<Database.Models.Item>(),
            };

            Initialize();

            this.VehicleData.FuelTank = GetFuelTankSize((VehicleClass)Vehicle.Class);
            API.shared.setEntitySyncedData(Vehicle.handle, "_maxfuel", VehicleData.Fuel);
            this.VehicleData.Fuel = VehicleData.FuelTank * 0.2f;
            API.shared.setEntitySyncedData(Vehicle.handle, "_fuel", VehicleData.Fuel);
            this.VehicleData.FuelConsumption = Vehicle.maxAcceleration / 0.2f;
            API.shared.setEntitySyncedData(Vehicle.handle, "_fuelConsumption", VehicleData.FuelConsumption);
            this.VehicleData.Milage = 0.0f;
            API.shared.setEntitySyncedData(Vehicle.handle, "_milage", VehicleData.Milage);
            this.VehicleData.IsSpawned = true;
        }

        private void Initialize()
        {
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
            API.shared.setVehicleWheelType(Vehicle, VehicleData.WheelType);
            API.shared.setVehicleWheelColor(Vehicle, VehicleData.WheelColor);

            API.shared.setVehicleEngineStatus(Vehicle, false);
            API.shared.setVehicleLocked(Vehicle, true);
        }

        public static Vehicle GetVehicleData(long id)
        {
            return ContextFactory.Instance.Vehicles.Single(x => x.Id == id);
        }

        public static Vehicle GetVehicleData(CharacterController cc, long id)
        {
            return cc?.Character.Vehicle.Single(x => x.Id == id);
        }

        public static List<Vehicle> GetVehiclesData(CharacterController cc)
        {
            return cc.Character.Vehicle.ToList();
        }

        //Pojazdy z prac nie są trzymane w bazie danych
        private bool _nonDbVehicle;
        public void Save()
        {
            VehicleData.Health = Vehicle.health;
            VehicleData.Door1Damage = API.shared.isVehicleDoorBroken(Vehicle, 1);
            VehicleData.Door2Damage = API.shared.isVehicleDoorBroken(Vehicle, 2);
            VehicleData.Door3Damage = API.shared.isVehicleDoorBroken(Vehicle, 3);
            VehicleData.Door4Damage = API.shared.isVehicleDoorBroken(Vehicle, 4);
            VehicleData.Window1Damage = API.shared.isVehicleWindowBroken(Vehicle, 1);
            VehicleData.Window2Damage = API.shared.isVehicleWindowBroken(Vehicle, 2);
            VehicleData.Window3Damage = API.shared.isVehicleWindowBroken(Vehicle, 3);
            VehicleData.Window4Damage = API.shared.isVehicleWindowBroken(Vehicle, 4);
            VehicleData.PrimaryColor = Vehicle.customPrimaryColor.ToHex();
            VehicleData.SecondaryColor = Vehicle.customSecondaryColor.ToHex();
            VehicleData.EnginePowerMultipler = Vehicle.enginePowerMultiplier;
            VehicleData.NumberPlateStyle = Vehicle.numberPlateStyle;
            VehicleData.NumberPlate = Vehicle.numberPlate;
            VehicleData.Fuel = API.shared.getEntitySyncedData(Vehicle.handle, "_fuel");
            VehicleData.FuelConsumption = API.shared.getEntitySyncedData(Vehicle.handle, "_fuelConsumption");
            VehicleData.Milage = API.shared.getEntitySyncedData(Vehicle.handle, "_milage");

            if (_nonDbVehicle) return;
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
            if (_nonDbVehicle) return;
            Save();
        }

        private static float GetFuelTankSize(VehicleClass vc)
        {
            switch (vc)
            {
                case VehicleClass.Compact:
                    return 70.0f;
                case VehicleClass.Sedans:
                    return 80.0f;
                case VehicleClass.SUVs:
                    return 85.0f;
                case VehicleClass.Coupe:
                    return 60.0f;
                case VehicleClass.Muscle:
                    return 70.0f;
                case VehicleClass.SportClassics:
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
                case VehicleClass.Cycle:
                    return 0.0f;
                case VehicleClass.Boat:
                    return 200.0f;
                case VehicleClass.Heli:
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
                case VehicleClass.Train:
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
            if(!_nonDbVehicle) Save();
            API.shared.deleteEntity(this.Vehicle);
            RPEntityManager.Remove(this);
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