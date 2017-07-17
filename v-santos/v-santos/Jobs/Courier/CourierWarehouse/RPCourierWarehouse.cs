/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Groups;
using Serverside.Jobs.Courier.CourierWarehouse.Models;

namespace Serverside.Jobs.Courier.CourierWarehouse
{
    public class RPCourierWarehouse : Script 
    {
        public List<CourierWarehouse> Warehouses { get; set; } = new List<CourierWarehouse>();
        
        public RPCourierWarehouse()
        {
            API.onResourceStart += OnResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "OnPlayerTakePackage")
            {
                var package = RPGroupWarehouse.CurrentOrders.Single(x => x.Data.Id == (int)arguments[0]);
                if (package.CurrentCourier != null)
                {
                    sender.Notify("Ktoś obecnie dostarcza tę paczkę.");
                    return;
                }

                sender.Notify($"Podjąłeś się dostarczenia przesyłki do: {RPEntityManager.GetGroup(package.Data.Getter.Id).GetColoredName()}");
                package.CurrentCourier = sender.GetAccountController();
                RPGroupWarehouse.CurrentOrders.Remove(package);

                Timer timer = new Timer(1800000);
                timer.Start();
                timer.Elapsed += (o, args) =>
                {
                    if (package.CurrentCourier.CharacterController.Character.Online)
                    {
                        package.CurrentCourier.Client.Notify("Nie dostarczyłeś paczki na czas.");
                    }

                    package.CurrentCourier = null;
                    RPGroupWarehouse.CurrentOrders.Add(package);
                };

            }
        }

        private void OnResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPCourierWarehouse] Uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
        }
        
        [Command("dodajmagazyn", "~y~ UŻYJ ~w~ /dodajmagazyn [nazwa]")]
        public void AddVehicleToJob(Client sender, string name)
        {
            //if (sender.GetAccountController().AccountData.ServerRank < ServerRank.GameMaster)
            //{
            //    sender.Notify("Nie posiadasz uprawnień do dodawania auta do pracy.");
            //    return;
            //}

            sender.Notify("Ustaw się w wybranej pozycji a następnie wpisz /tu");

            API.onChatCommand += Handler;

            void Handler(Client o, string command, CancelEventArgs cancel)
            {
                if (o == sender && command == "/tu")
                {
                    var warehouse = new CourierWarehouseModel()
                    {
                        Name = name,
                        Position = o.position,
                        CreatorForumName = o.GetAccountController().AccountData.Name
                    };
                    Core.XmlHelper.AddXmlObject(warehouse, $@"{Constant.ConstantAssemblyInfo.XmlDirectory}CourierWarehouses\");

                    Warehouses.Add(new CourierWarehouse(API, warehouse));
                    o.Notify("Dodawanie magazynu do pracy kuriera zakończyło się ~h~ ~g~pomyślnie.");
                    API.onChatCommand -= Handler;           
                }
            }
        }
    }
}