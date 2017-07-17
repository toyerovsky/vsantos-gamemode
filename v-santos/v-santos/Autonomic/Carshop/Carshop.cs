/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using Serverside.Autonomic.Carshop.Models;
using Serverside.Core.Extensions;

namespace Serverside.Autonomic.Carshop
{
    public class Carshop : IDisposable
    {
        public CarshopModel Data { get; set; }
        public Marker CarshopMarker { get; set; }
        public CylinderColShape CarshopColshape { get; set; }
        public Blip CarshopBlip { get; set; }
        
        private API Api { get; set; }

        public Carshop(API api, CarshopModel data)
        {
            Api = api;
            Data = data;
            CarshopMarker = Api.createMarker(0, Data.Position, new Vector3(1f, 1f, 1f), new Vector3(1f, 1f, 1f),
                new Vector3(1f, 1f, 1f), 255, 106, 148, 40);

            CarshopColshape = Api.createCylinderColShape(Data.Position, 2f, 5f);
            CarshopColshape.onEntityEnterColShape += (shape, entity) =>
            {
                if (API.shared.getEntityType(entity) != EntityType.Player) return;

                var player = Api.getPlayerFromHandle(entity);
                string compactsJson =
                    JsonConvert.SerializeObject(RPCarshop.Vehicles.Where(v => v.Category == VehicleClass.Compact && v.CarshopTypes.Contains(Data.Type)));

                string coupesJson =
                    JsonConvert.SerializeObject(RPCarshop.Vehicles.Where(v => v.Category == VehicleClass.Coupe && v.CarshopTypes.Contains(Data.Type)));

                string suvsJson =
                    JsonConvert.SerializeObject(RPCarshop.Vehicles.Where(v => v.Category == VehicleClass.SUVs && v.CarshopTypes.Contains(Data.Type)));

                string sedansJson =
                    JsonConvert.SerializeObject(RPCarshop.Vehicles.Where(v => v.Category == VehicleClass.Sedans && v.CarshopTypes.Contains(Data.Type)));

                string sportsJson =
                    JsonConvert.SerializeObject(RPCarshop.Vehicles.Where(v => v.Category == VehicleClass.Sports && v.CarshopTypes.Contains(Data.Type)));

                string motorcyclesJson =
                    JsonConvert.SerializeObject(RPCarshop.Vehicles.Where(v => v.Category == VehicleClass.Motorcycles && v.CarshopTypes.Contains(Data.Type)));

                string bicyclesJson =
                    JsonConvert.SerializeObject(RPCarshop.Vehicles.Where(v => v.Category == VehicleClass.Cycle && v.CarshopTypes.Contains(Data.Type)));

                Api.triggerClientEvent(player, "OnPlayerEnteredCarshop", compactsJson, coupesJson, suvsJson, sedansJson, sportsJson, motorcyclesJson, bicyclesJson);
            };

            CarshopBlip = Api.createBlip(Data.Position);
            CarshopBlip.sprite = 490;
            CarshopBlip.transparency = 100;
        }

        public void Dispose()
        {
            Api.deleteColShape(CarshopColshape);
            Api.deleteEntity(CarshopMarker);
            CarshopBlip.transparency = 0;
        }
    }
}