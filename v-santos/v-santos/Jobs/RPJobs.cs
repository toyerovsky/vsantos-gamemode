using System;
using GrandTheftMultiplayer.Server.API;
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