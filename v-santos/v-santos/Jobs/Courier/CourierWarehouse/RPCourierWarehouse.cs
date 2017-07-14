using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using Serverside.Core.Extensions;
using 

namespace Serverside.Jobs.Courier.CourierWarehouse
{
    public class RPCourierWarehouse : Script 
    {
        public List<CourierWarehouse> Warehouses { get; set; } = new List<CourierWarehouse>();
        
        public RPCourierWarehouse()
        {
            API.onResourceStart += OnResourceStart;
        }

        private void OnResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPCourierWarehouse] Uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
        }
        
    }
}