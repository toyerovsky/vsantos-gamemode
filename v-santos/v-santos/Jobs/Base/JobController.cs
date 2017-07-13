using System;
using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;
using Serverside.Core.Extensions;

namespace Serverside.Jobs.Base
{
    public abstract class JobController
    {
        public List<JobWorkerController> Workers { get; set; } = new List<JobWorkerController>();

        public string JobName { get; set; }
        public decimal MoneyLimit { get; set; }
        public string XmlDirectory { get; set; }

        private API Api { get; set; }

        protected JobController(API api, string jobName, decimal moneyLimit, string xmlDirectory)
        {
            Api = api;
            JobName = jobName;
            MoneyLimit = moneyLimit;
            XmlDirectory = xmlDirectory;
        }
    }
}