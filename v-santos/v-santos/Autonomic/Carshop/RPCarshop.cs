using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using GTANetworkServer.Constant;
using GTANetworkShared;
using Newtonsoft.Json;
using Serverside.Autonomic.Carshop.Models;
using Serverside.Controllers;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Core.Extenstions;

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
                    JsonConvert.SerializeObject(Vehicles.Where(v => v.Category == VehicleClass.Compact));

                string coupesJson =
                    JsonConvert.SerializeObject(Vehicles.Where(v => v.Category == VehicleClass.Coupe));

                string suvsJson =
                    JsonConvert.SerializeObject(Vehicles.Where(v => v.Category == VehicleClass.SUVs));

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
                    new CarshopVehicle("Blista", VehicleHash.Blista, VehicleClass.Compact, new decimal(17999)),
                    new CarshopVehicle("Brioso", VehicleHash.Brioso, VehicleClass.Compact, new decimal(21999)),
                    new CarshopVehicle("Dilettante", VehicleHash.Dilettante, VehicleClass.Compact, new decimal(12000)),
                    new CarshopVehicle("Issi", VehicleHash.Issi2, VehicleClass.Compact, new decimal(25000)),
                    new CarshopVehicle("Panto", VehicleHash.Panto, VehicleClass.Compact, new decimal(10000)),
                    new CarshopVehicle("Prairie", VehicleHash.Prairie, VehicleClass.Compact, new decimal(22000)),

                    //Coupe
                    new CarshopVehicle("Exemplar", VehicleHash.Exemplar, VehicleClass.Coupe, new decimal(190000)),
                    new CarshopVehicle("F620", VehicleHash.F620, VehicleClass.Coupe, new decimal(151999)),
                    new CarshopVehicle("Felon", VehicleHash.Felon2, VehicleClass.Coupe, new decimal(158999)),
                    new CarshopVehicle("Jackal", VehicleHash.Jackal, VehicleClass.Coupe, new decimal(31000)),
                    new CarshopVehicle("Oracle", VehicleHash.Oracle, VehicleClass.Coupe, new decimal(21000)),
                    new CarshopVehicle("Sentinel", VehicleHash.Sentinel, VehicleClass.Coupe, new decimal(40000)),
                    new CarshopVehicle("Windsor", VehicleHash.Windsor, VehicleClass.Coupe, new decimal(220000)),
                    new CarshopVehicle("Zion", VehicleHash.Zion, VehicleClass.Coupe, new decimal(32000)),

                    //SUV
                    new CarshopVehicle("BeeJay XL", VehicleHash.BJXL, VehicleClass.SUVs, new decimal(16999)),
                    new CarshopVehicle("Baller I", VehicleHash.Baller, VehicleClass.SUVs, new decimal(14999)),
                    new CarshopVehicle("Baller II", VehicleHash.Baller2, VehicleClass.SUVs, new decimal(51000)),
                    new CarshopVehicle("Cavalcade I", VehicleHash.Cavalcade, VehicleClass.SUVs, new decimal(26000)),
                    new CarshopVehicle("Cavalcade II", VehicleHash.Cavalcade2, VehicleClass.SUVs, new decimal(71000)),
                    new CarshopVehicle("Dubsta", VehicleHash.Dubsta2, VehicleClass.SUVs, new decimal(56999)),
                    new CarshopVehicle("FQ2", VehicleHash.Fq2, VehicleClass.SUVs, new decimal(52000)),
                    new CarshopVehicle("Granger", VehicleHash.Granger, VehicleClass.SUVs, new decimal(61799)),
                    new CarshopVehicle("Gresley", VehicleHash.Gresley, VehicleClass.SUVs, new decimal(56999)),
                    new CarshopVehicle("Habanero", VehicleHash.Gresley, VehicleClass.SUVs, new decimal(68799)),
                    new CarshopVehicle("Huntley S", VehicleHash.Huntley, VehicleClass.SUVs, new decimal(72699)),
                    new CarshopVehicle("Landstalker", VehicleHash.Landstalker, VehicleClass.SUVs, new decimal(41899)),
                    new CarshopVehicle("Patriot", VehicleHash.Patriot, VehicleClass.SUVs, new decimal(78999)),
                    new CarshopVehicle("Radius", VehicleHash.Radi, VehicleClass.SUVs, new decimal(36999)),
                    new CarshopVehicle("Rocoto", VehicleHash.Rocoto, VehicleClass.SUVs, new decimal(98000)),
                    new CarshopVehicle("Seminole", VehicleHash.Seminole, VehicleClass.SUVs, new decimal(64999)),
                    new CarshopVehicle("Serrano", VehicleHash.Serrano, VehicleClass.SUVs, new decimal(28999)),
                    new CarshopVehicle("XLS", VehicleHash.Xls, VehicleClass.SUVs, new decimal(54999))
                    
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
                    new VehicleController(new FullPosition(new Vector3(-50, -1680, 29.5), new Vector3(0, 0, 0)),
                        vehicleHash, "", 0, 0, APIExtensions.GetRandomColor(), APIExtensions.GetRandomColor(), 0f, 0f, sender.GetAccountController().CharacterController.Character);
                }
                else
                {
                    sender.Notify("Nie posiadasz wystarczającej ilości gotówki.");
                }
            }
        }
    }
}
