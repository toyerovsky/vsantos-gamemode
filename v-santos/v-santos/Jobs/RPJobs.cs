using System;
using System.Collections.Generic;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Controllers;
using Serverside.Core.Extensions;

namespace Serverside.Jobs
{
    public class RPJobs : Script
    {
        public RPJobs()
        {
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPJobs] Uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
        }
    }
}