/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Shared;
using Serverside.Core;
using Serverside.Database.Models;
using Serverside.Jobs.Base;

namespace Serverside.Jobs.Courier
{
    public class CourierVehicle : JobVehicleController
    {

        public CourierVehicle(Vehicle data) : base(data)
        {
        }

        public CourierVehicle(FullPosition spawnPosition, VehicleHash hash, string numberplate, int numberplatestyle, int creatorId, Color primaryColor, Color secondaryColor, float enginePowerMultiplier = 0, float engineTorqueMultiplier = 0, Character character = null, Group @group = null) : base(spawnPosition, hash, numberplate, numberplatestyle, creatorId, primaryColor, secondaryColor, enginePowerMultiplier, engineTorqueMultiplier, character, @group)
        {
        }
    }
}