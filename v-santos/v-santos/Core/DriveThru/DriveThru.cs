/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Serverside.Core.DriveThru.Models;

namespace Serverside.Core.DriveThru
{
    public class DriveThru : IDisposable
    {
        public DriveThruModel Data { get; set; }
        public Marker DriveThruMarker { get; set; }
        public ColShape DriveThruColshape { get; set; }

        private API Api { get; set; }

        public DriveThru(API api, DriveThruModel data)
        {
            Api = api;
            Data = data;
            DriveThruMarker = Api.createMarker(1, Data.Position, new Vector3(), new Vector3(1f, 1f, 1f),
                new Vector3(1f, 1f, 1f), 255, 106, 148, 40);

            DriveThruColshape = Api.createCylinderColShape(Data.Position, 2f, 5f);

            DriveThruColshape.onEntityEnterColShape += (shape, entity) =>
            {
                if (API.shared.getEntityType(entity) != EntityType.Player) return;

                Api.getPlayerFromHandle(entity).triggerEvent("ShowDriveThruMenu");

            };

            DriveThruColshape.onEntityExitColShape += (shape, entity) =>
            {
                if (API.shared.getEntityType(entity) != EntityType.Player) return;

                Api.getPlayerFromHandle(entity).triggerEvent("DisposeDriveThruMenu");

            };
        }

        public void Dispose()
        {
            Api.deleteColShape(DriveThruColshape);
            Api.deleteEntity(DriveThruMarker);
        }
    }

}