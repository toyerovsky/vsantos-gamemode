/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using Serverside.Controllers;

namespace Serverside.Core.Extensions
{
    public static class VehicleExtensions
    {
        public static VehicleController GetVehicleController(this Vehicle handle)
        {
            if (!handle.hasData("VehicleController"))
                return null;
            return API.shared.getEntityData(handle, "VehicleController");
        }
    }
}