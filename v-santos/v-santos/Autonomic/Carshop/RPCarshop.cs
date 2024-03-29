﻿/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Serverside.Admin.Enums;
using Serverside.Autonomic.Carshop.Models;
using Serverside.Controllers;
using Serverside.Core;
using Serverside.Core.Extensions;

namespace Serverside.Autonomic.Carshop
{
    public sealed class RPCarshop : Script
    {
        public static List<CarshopVehicleModel> Vehicles { get; set; } = new List<CarshopVehicleModel>();

        private List<Carshop> Carshops { get; set; } = new List<Carshop>();

        public RPCarshop()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPCarshop] uruchomione pomyślnie.", ConsoleColor.DarkMagenta);

            Vehicles =
                XmlHelper.GetXmlObjects<CarshopVehicleModel>(
                    $@"{Constant.ConstantAssemblyInfo.XmlDirectory}CarshopVehicles\");
            Vehicles.AddRange(new List<CarshopVehicleModel>
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
                new CarshopVehicleModel("Blista", VehicleHash.Blista, VehicleClass.Compact, new decimal(17999), new List<CarshopType>() {CarshopType.Biedny, CarshopType.Sredni}),
                new CarshopVehicleModel("Brioso", VehicleHash.Brioso, VehicleClass.Compact, new decimal(21999), new List<CarshopType>() {CarshopType.Biedny, CarshopType.Sredni}),
                new CarshopVehicleModel("Dilettante", VehicleHash.Dilettante, VehicleClass.Compact, new decimal(12000), new List<CarshopType>() {CarshopType.Biedny, CarshopType.Sredni}),
                new CarshopVehicleModel("Issi", VehicleHash.Issi2, VehicleClass.Compact, new decimal(25000), new List<CarshopType>() {CarshopType.Biedny, CarshopType.Sredni}),
                new CarshopVehicleModel("Panto", VehicleHash.Panto, VehicleClass.Compact, new decimal(10000), new List<CarshopType>() {CarshopType.Biedny, CarshopType.Sredni}),
                new CarshopVehicleModel("Prairie", VehicleHash.Prairie, VehicleClass.Compact, new decimal(22000), new List<CarshopType>() {CarshopType.Biedny, CarshopType.Sredni}),

                //Coupe
                new CarshopVehicleModel("Exemplar", VehicleHash.Exemplar, VehicleClass.Coupe, new decimal(190000), new List<CarshopType>() {CarshopType.Luksus}),
                new CarshopVehicleModel("F620", VehicleHash.F620, VehicleClass.Coupe, new decimal(151999), new List<CarshopType>() {CarshopType.Luksus}),
                new CarshopVehicleModel("Felon", VehicleHash.Felon2, VehicleClass.Coupe, new decimal(158999), new List<CarshopType>() {CarshopType.Luksus}),
                new CarshopVehicleModel("Jackal", VehicleHash.Jackal, VehicleClass.Coupe, new decimal(31000), new List<CarshopType>() {CarshopType.Sredni}),
                new CarshopVehicleModel("Oracle", VehicleHash.Oracle, VehicleClass.Coupe, new decimal(21000), new List<CarshopType>() {CarshopType.Sredni}),
                new CarshopVehicleModel("Sentinel", VehicleHash.Sentinel, VehicleClass.Coupe, new decimal(40000), new List<CarshopType>() {CarshopType.Sredni}),
                new CarshopVehicleModel("Windsor", VehicleHash.Windsor, VehicleClass.Coupe, new decimal(220000), new List<CarshopType>() {CarshopType.Sredni}),
                new CarshopVehicleModel("Zion", VehicleHash.Zion, VehicleClass.Coupe, new decimal(32000), new List<CarshopType>() {CarshopType.Sredni}),

                //SUV
                new CarshopVehicleModel("BeeJay XL", VehicleHash.BJXL, VehicleClass.SUVs, new decimal(16999), new List<CarshopType>() {CarshopType.Biedny, CarshopType.Sredni}),
                new CarshopVehicleModel("Baller I", VehicleHash.Baller, VehicleClass.SUVs, new decimal(14999), new List<CarshopType>() {CarshopType.Biedny, CarshopType.Sredni}),
                new CarshopVehicleModel("Baller II", VehicleHash.Baller2, VehicleClass.SUVs, new decimal(51000), new List<CarshopType>() {CarshopType.Sredni}),
                new CarshopVehicleModel("Cavalcade I", VehicleHash.Cavalcade, VehicleClass.SUVs, new decimal(26000), new List<CarshopType>() {CarshopType.Biedny, CarshopType.Sredni}),
                new CarshopVehicleModel("Cavalcade II", VehicleHash.Cavalcade2, VehicleClass.SUVs, new decimal(71000), new List<CarshopType>() {CarshopType.Sredni, CarshopType.Luksus}),
                new CarshopVehicleModel("Dubsta", VehicleHash.Dubsta2, VehicleClass.SUVs, new decimal(56999), new List<CarshopType>() {CarshopType.Sredni}),
                new CarshopVehicleModel("FQ2", VehicleHash.Fq2, VehicleClass.SUVs, new decimal(52000), new List<CarshopType>() {CarshopType.Sredni}),
                new CarshopVehicleModel("Granger", VehicleHash.Granger, VehicleClass.SUVs, new decimal(61799), new List<CarshopType>() {CarshopType.Sredni}),
                new CarshopVehicleModel("Gresley", VehicleHash.Gresley, VehicleClass.SUVs, new decimal(56999), new List<CarshopType>() { CarshopType.Sredni }),
                new CarshopVehicleModel("Habanero", VehicleHash.Gresley, VehicleClass.SUVs, new decimal(68799), new List<CarshopType>() { CarshopType.Sredni }),
                new CarshopVehicleModel("Huntley S", VehicleHash.Huntley, VehicleClass.SUVs, new decimal(72699), new List<CarshopType>() { CarshopType.Sredni }),
                new CarshopVehicleModel("Landstalker", VehicleHash.Landstalker, VehicleClass.SUVs, new decimal(41899), new List<CarshopType>() { CarshopType.Sredni }),
                new CarshopVehicleModel("Patriot", VehicleHash.Patriot, VehicleClass.SUVs, new decimal(78999), new List<CarshopType>() { CarshopType.Sredni }),
                new CarshopVehicleModel("Radius", VehicleHash.Radi, VehicleClass.SUVs, new decimal(36999), new List<CarshopType>() { CarshopType.Sredni }),
                new CarshopVehicleModel("Rocoto", VehicleHash.Rocoto, VehicleClass.SUVs, new decimal(98000), new List<CarshopType>() { CarshopType.Sredni }),
                new CarshopVehicleModel("Seminole", VehicleHash.Seminole, VehicleClass.SUVs, new decimal(64999), new List<CarshopType>() { CarshopType.Sredni }),
                new CarshopVehicleModel("Serrano", VehicleHash.Serrano, VehicleClass.SUVs, new decimal(28999), new List<CarshopType>() { CarshopType.Sredni }),
                new CarshopVehicleModel("XLS", VehicleHash.Xls, VehicleClass.SUVs, new decimal(54999), new List<CarshopType>() {CarshopType.Sredni}),
                    
                //Sedany
            });

            XmlHelper.GetXmlObjects<CarshopModel>(
                $@"{Constant.ConstantAssemblyInfo.XmlDirectory}Carshops\").ForEach(c => Carshops.Add(new Carshop(API, c)));

        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "OnPlayerBoughtVehicle")
            {
                //arguments[0] to nazwa pojazdu

                VehicleHash vehicleHash;
                if (!Enum.TryParse(arguments[0].ToString(), out vehicleHash)) return;

                CarshopVehicleModel vehicle = Vehicles.First(v => v.Name == arguments[0].ToString());

                if (sender.HasMoney(vehicle.Cost))
                {
                    sender.RemoveMoney(vehicle.Cost);

                    new VehicleController(new FullPosition(new Vector3(-50, -1680, 29.5), new Vector3(0, 0, 0)),
                        vehicleHash, "", 0, 0, new Color().GetRandomColor(), new Color().GetRandomColor(), 0f, 0f, sender.GetAccountController().CharacterController.Character);
                    sender.Notify($"Pojazd {vehicleHash.ToString()} zakupiony ~h~~g~pomyślnie.");
                }
                else
                {
                    sender.Notify("Nie posiadasz wystarczającej ilości gotówki.");
                }
            }
        }

        [Command("dodajautosalon", "~y~ UŻYJ ~w~ /dodajautosalon [model] [koszt] [typ salonu]")]
        public void AddVehicleToCarshop(Client sender, VehicleHash hash, VehicleClass vehicleClass, decimal cost, CarshopType type, CarshopType type2 = CarshopType.Empty)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.GameMaster)
            {
                sender.Notify("Nie posiadasz uprawnień do tworzenia pojazdu w salonie.");
                return;
            }

            if (Vehicles.Any(v => v.Hash == hash))
            {
                sender.Notify("Podany pojazd jest już dodany.");
                return;
            }

            if (!Validator.IsMoneyValid(cost))
            {
                sender.Notify("Wprowadzona kwota gotówki jest nieprawidłowa.");
                return;
            }


            var types = new List<CarshopType> { type };
            if (type2 != CarshopType.Empty) types.Add(type2);

            CarshopVehicleModel vehicle =
                new CarshopVehicleModel(hash.ToString(), hash, vehicleClass, cost, types)
                {
                    CreatorForumName = sender.GetAccountController().AccountData.Name
                };

            XmlHelper.AddXmlObject(vehicle, $@"{Constant.ConstantAssemblyInfo.XmlDirectory}CarshopVehicles\", vehicle.Name);
        }

        [Command("dodajsalon", "~y~ UŻYJ ~w~ /dodajsalon [nazwa] [typ]")]
        public void AddCarshop(Client sender, string name, CarshopType type)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.GameMaster)
            {
                sender.Notify("Nie posiadasz uprawnień do tworzenia grupy.");
                return;
            }

            sender.Notify("Ustaw się w wybranej pozycji, a następnie wpisz /tu.");
            sender.Notify("...użyj /diag aby poznać swoją obecną pozycję.");

            API.onChatCommand += Handler;

            void Handler(Client o, string command, CancelEventArgs cancel)
            {
                if (o == sender && command == "/tu")
                {
                    CarshopModel data = new CarshopModel
                    {
                        CreatorForumName = o.GetAccountController().AccountData.Name,
                        Position = o.position,
                        Type = type,
                    };
                    data.CreatorForumName = o.GetAccountController().AccountData.Name;

                    XmlHelper.AddXmlObject(data, $@"{Constant.ConstantAssemblyInfo.XmlDirectory}Carshops\");
                    Carshops.Add(new Carshop(API, data)); //Nowa instancja bankomatu spawnuje go w świecie gry
                    sender.Notify("Dodawanie salonu zakończyło się ~h~~g~pomyślnie.");
                    API.onChatCommand -= Handler;
                }
            }
        }



        [Command("usunsalon", "~y~ UŻYJ ~w~ /usunsalon")]
        public void DeleteCarshop(Client sender)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.GameMaster)
            {
                sender.Notify("Nie posiadasz uprawnień do usuwania salonu samochodowego.");
                return;
            }

            if (Carshops.Count == 0)
            {
                sender.Notify("Nie znaleziono salonu pojazdów który można usunąć.");
                return;
            }

            var carshop = Carshops.OrderBy(a => a.Data.Position.DistanceTo(sender.position)).First();
            if (XmlHelper.TryDeleteXmlObject(carshop.Data.FilePath))
            {
                sender.Notify("Usuwanie salonu zakończyło się ~h~~g~pomyślnie");
                Carshops.Remove(carshop);
                carshop.Dispose();
            }
            else
            {
                sender.Notify("Usuwanie salonu zakończyło się ~h~~r~niepomyślnie.");
            }
        }
    }
}
