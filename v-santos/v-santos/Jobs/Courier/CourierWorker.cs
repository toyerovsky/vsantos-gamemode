/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using GrandTheftMultiplayer.Server.API;
using Serverside.Controllers;
using Serverside.Jobs.Base;

namespace Serverside.Jobs.Courier
{
    public class CourierWorker : JobWorkerController
    {
        public CourierWorker(AccountController player, JobVehicleController jobVehicle, API api) : base(api, player, jobVehicle)
        {
        }
    }
}