using System;
using GTANetworkServer;
using Serverside.Core.Extensions;

namespace Serverside.Buildings
{
    public class RPBuildings : Script
    {
        public RPBuildings()
        {
            API.onResourceStart += ResourceStartHandler;
        }

        private void ResourceStartHandler()
        {
            APIExtensions.ConsoleOutput("[RPBuildings] Uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
        }
    }
}