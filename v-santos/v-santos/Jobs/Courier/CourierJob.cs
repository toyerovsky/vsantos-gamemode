/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;
using Serverside.Core;
using Serverside.Database.Models;
using Serverside.Jobs.Base;

namespace Serverside.Jobs.Courier
{
    public class CourierJob : JobController
    {
        public List<CourierVehicle> Vehicles { get; set; } = new List<CourierVehicle>();

        public CourierJob(API api, string jobName, decimal moneyLimit, string jsonDirectory) : base(api, jobName, moneyLimit, jsonDirectory)
        {
            foreach (var vehicleData in JsonHelper.GetJsonObjects<Vehicle>(jsonDirectory))
            {
                Vehicles.Add(new CourierVehicle(vehicleData));
            }
        }
    }
}