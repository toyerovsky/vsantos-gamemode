﻿using System;
using System.Collections.Generic;
using GrandTheftMultiplayer.Shared;
using Serverside.Core.Extensions;
using Serverside.Interfaces;

namespace Serverside.Autonomic.Carshop.Models
{
    [Serializable]
    public class CarshopVehicleModel : IXmlObject
    {
        public string Name { get; set; }
        public VehicleHash Hash { get; set; }
        public VehicleClass Category { get; set; }
        public decimal Cost { get; set; }
        public string FilePath { get; set; }
        public string CreatorForumName { get; set; }
        public List<CarshopType> CarshopTypes { get; set; }

        public CarshopVehicleModel(string name, VehicleHash hash, VehicleClass category, decimal cost, List<CarshopType> types)
        {
            Name = name;
            Hash = hash;
            Category = category;
            Cost = cost;
            CarshopTypes = types;
        }

        public CarshopVehicleModel() { }
    }
}