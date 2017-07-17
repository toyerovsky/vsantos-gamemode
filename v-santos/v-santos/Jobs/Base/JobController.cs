/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

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
        public string JsonDirectory { get; set; }

        private API Api { get; set; }

        protected JobController(API api, string jobName, decimal moneyLimit, string xmlDirectory)
        {
            Api = api;
            JobName = jobName;
            MoneyLimit = moneyLimit;
            JsonDirectory = xmlDirectory;
        }
    }
}