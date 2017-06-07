using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using Newtonsoft.Json;
using Serverside.Autonomic.Carshop.Models;
using Serverside.Controllers;
using Serverside.Core;
using Serverside.Core.Extensions;

namespace Serverside.Autonomic.Carshop
{
    public sealed class RPCarshop : Script
    {
        public RPCarshop()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPCarshop] uruchomione pomyślnie.", ConsoleColor.DarkMagenta);

            CarshopMarker = API.createMarker(1, new Vector3(-41, -1674.76, 28.5), new Vector3(0, 0, 0), new Vector3(0, 0, 0),
                new Vector3(1, 1, 1), 100, 255, 0, 0);

            CarshopColshape = API.createCylinderColShape(new Vector3(-41, -1674.76, 28.5), 2f, 5f);
            CarshopColshape.onEntityEnterColShape += (shape, entity) =>
            {
                if (API.shared.getEntityType(entity) != EntityType.Player) return;

                var player = API.getPlayerFromHandle(entity);
                string compactsJson =
                    JsonConvert.SerializeObject(Vehicles.Where(v => v.Category == VehicleCategory.Kompaktowe));

                string coupesJson =
                    JsonConvert.SerializeObject(Vehicles.Where(v => v.Category == VehicleCategory.Coupe));

                string suvsJson =
                    JsonConvert.SerializeObject(Vehicles.Where(v => v.Category == VehicleCategory.SUV));

                API.triggerClientEvent(player, "OnPlayerEnteredCarshop", compactsJson, coupesJson, suvsJson);
            };
        }


        public List<CarshopVehicle> Vehicles
        {
            get
            {
                List<CarshopVehicle> vehicles = new List<CarshopVehicle>
                {
                    //Używane 
                    //Rahapsody
                    //Blista2

                    //Do zrobienia
                    //CogCabrio
                    //Oracle2
                    //Sentinel2
                    //Windsor2


                    //Kompaktowe
                    new CarshopVehicle("Blista", VehicleHash.Blista, VehicleCategory.Kompaktowe, new decimal(17999)),
                    new CarshopVehicle("Brioso", VehicleHash.Brioso, VehicleCategory.Kompaktowe, new decimal(21999)),
                    new CarshopVehicle("Dilettante", VehicleHash.Dilettante, VehicleCategory.Kompaktowe, new decimal(12000)),
                    new CarshopVehicle("Issi", VehicleHash.Issi2, VehicleCategory.Kompaktowe, new decimal(25000)),
                    new CarshopVehicle("Panto", VehicleHash.Panto, VehicleCategory.Kompaktowe, new decimal(10000)),
                    new CarshopVehicle("Prairie", VehicleHash.Prairie, VehicleCategory.Kompaktowe, new decimal(22000)),

                    //Coupe
                    new CarshopVehicle("Exemplar", VehicleHash.Exemplar, VehicleCategory.Coupe, new decimal(190000)),
                    new CarshopVehicle("F620", VehicleHash.F620, VehicleCategory.Coupe, new decimal(151999)),
                    new CarshopVehicle("Felon", VehicleHash.Felon2, VehicleCategory.Coupe, new decimal(158999)),
                    new CarshopVehicle("Jackal", VehicleHash.Jackal, VehicleCategory.Coupe, new decimal(31000)),
                    new CarshopVehicle("Oracle", VehicleHash.Oracle, VehicleCategory.Coupe, new decimal(21000)),
                    new CarshopVehicle("Sentinel", VehicleHash.Sentinel, VehicleCategory.Coupe, new decimal(40000)),
                    new CarshopVehicle("Windsor", VehicleHash.Windsor, VehicleCategory.Coupe, new decimal(220000)),
                    new CarshopVehicle("Zion", VehicleHash.Zion, VehicleCategory.Coupe, new decimal(32000)),

                    //SUV
                    new CarshopVehicle("BeeJay XL", VehicleHash.BJXL, VehicleCategory.SUV, new decimal(16999)),
                    new CarshopVehicle("Baller I", VehicleHash.Baller, VehicleCategory.SUV, new decimal(14999)),
                    new CarshopVehicle("Baller II", VehicleHash.Baller2, VehicleCategory.SUV, new decimal(51000)),
                    new CarshopVehicle("Cavalcade I", VehicleHash.Cavalcade, VehicleCategory.SUV, new decimal(26000)),
                    new CarshopVehicle("Cavalcade II", VehicleHash.Cavalcade2, VehicleCategory.SUV, new decimal(71000)),
                    new CarshopVehicle("Dubsta", VehicleHash.Dubsta2, VehicleCategory.SUV, new decimal(56999)),
                    new CarshopVehicle("FQ2", VehicleHash.Fq2, VehicleCategory.SUV, new decimal(52000)),
                    new CarshopVehicle("Granger", VehicleHash.Granger, VehicleCategory.SUV, new decimal(61799)),
                    new CarshopVehicle("Gresley", VehicleHash.Gresley, VehicleCategory.SUV, new decimal(56999)),
                    new CarshopVehicle("Habanero", VehicleHash.Gresley, VehicleCategory.SUV, new decimal(68799)),
                    new CarshopVehicle("Huntley S", VehicleHash.Huntley, VehicleCategory.SUV, new decimal(72699)),
                    new CarshopVehicle("Landstalker", VehicleHash.Landstalker, VehicleCategory.SUV, new decimal(41899)),
                    new CarshopVehicle("Patriot", VehicleHash.Patriot, VehicleCategory.SUV, new decimal(78999)),
                    new CarshopVehicle("Radius", VehicleHash.Radi, VehicleCategory.SUV, new decimal(36999)),
                    new CarshopVehicle("Rocoto", VehicleHash.Rocoto, VehicleCategory.SUV, new decimal(98000)),
                    new CarshopVehicle("Seminole", VehicleHash.Seminole, VehicleCategory.SUV, new decimal(64999)),
                    new CarshopVehicle("Serrano", VehicleHash.Serrano, VehicleCategory.SUV, new decimal(28999)),
                    new CarshopVehicle("XLS", VehicleHash.Xls, VehicleCategory.SUV, new decimal(54999))
                    
                    //Sedany
                };
                
                return vehicles;
            }
        }

        private Marker CarshopMarker { get; set; }
        private CylinderColShape CarshopColshape { get; set; }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "OnPlayerBoughtVehicle")
            {
                //arguments[0] to nazwa pojazdu
                
                VehicleHash vehicleHash;
                if (!Enum.TryParse(arguments[0].ToString(), out vehicleHash)) return;

                CarshopVehicle vehicle = Vehicles.First(v => v.Name == arguments[0].ToString());

                if (sender.HasMoney(vehicle.Cost))
                {
                    sender.RemoveMoney(vehicle.Cost);
                    Random r = new Random();
                    //Co to jest
                    //new VehicleController(new FullPosition(new Vector3(-50, -1680, 29.5), new Vector3(0, 0, 0)),
                    //    vehicle.Name, 0, r.Next(0, 150), r.Next(0, 150), 0f, 0f);

                }
                else
                {
                    sender.Notify("Nie posiadasz wystarczającej ilości gotówki.");
                }
            }
        }
    }
}
