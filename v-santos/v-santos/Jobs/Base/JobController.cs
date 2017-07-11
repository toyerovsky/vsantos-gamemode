using System;
using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;
using Serverside.Core.Extensions;
using Serverside.Jobs.Interfaces;

namespace Serverside.Jobs.Base
{
    public abstract class JobController
    {
        public List<IWorker> Workers { get; set; } = new List<IWorker>();
        public List<JobVehicleController> Vehicles { get; set; } = new List<JobVehicleController>();

        public string JobName { get; set; }
        public decimal MoneyLimit { get; set; }

        private API Api { get; set; }

        public JobController(API api, string jobName, decimal moneyLimit)
        {
            Api = api;
            JobName = jobName;
            MoneyLimit = moneyLimit;
        }
    }
}