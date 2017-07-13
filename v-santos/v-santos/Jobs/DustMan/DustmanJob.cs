using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;
using Newtonsoft.Json;
using Serverside.Core;
using Serverside.Database.Models;
using Serverside.Jobs.Base;

namespace Serverside.Jobs.Dustman
{
    public class DustmanJob : JobController
    {
        public List<DustmanVehicle> Vehicles { get; set; } = new List<DustmanVehicle>();

        public DustmanJob(API api, string name, decimal moneyLimit, string jsonDirectory) : base(api, name, moneyLimit, jsonDirectory)
        {
            foreach (var vehicleData in JsonHelper.GetJsonObjects<Vehicle>(jsonDirectory))
            {
                Vehicles.Add(new DustmanVehicle(vehicleData));
            }
        }
    }
}