﻿/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using Serverside.Core.Bus.Models;
using Serverside.Core.Extensions;

namespace Serverside.Core.Bus
{
    public class RPBus : Script
    {
        private readonly List<BusStop> _busStops = new List<BusStop>();

        public RPBus()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPBus] Uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
            foreach (var stop in XmlHelper.GetXmlObjects<BusStopModel>(Constant.ConstantAssemblyInfo.XmlDirectory + @"BusStops\"))
            {
                _busStops.Add(new BusStop(API, stop));
            }
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            //args[0] Czas
            //args[1] Koszt
            //args[2] Indeks przystanku na jaki chce się udać
            if (eventName == "RequestBus")
            {
                var busStop = _busStops[Convert.ToInt32(arguments[2])];

                BusStop.StartTransport(sender, Convert.ToDecimal(arguments[1]), Convert.ToInt32(arguments[0]),
                    busStop.Data.Center, busStop.Data.Name);
            }
        }

        [Command("bus")]
        public void ShowBusMenu(Client sender)
        {
            //Jeśli gracz nie jest na przystanku to anulujemy proces
            if (!sender.HasData("Bus") || _busStops.Count < 2)
            {
                sender.Notify("Liczba przystanków autobusowych musi być większa lub równa dwa.");
                return;
            }
            //Wybieramy wszystkie przystanki oprócz tego w którym obecnie się znajduje
            var json = JsonConvert.SerializeObject(_busStops.Select(x => new
            {
                x.Data.Name,
                Time = (int)(sender.position.DistanceTo(x.Data.Center) / 2.5f),
                Cost = (int)(sender.position.DistanceTo(x.Data.Center) / 180f)
            }).Where(b => b.Name != ((BusStop)sender.GetData("Bus")).Data.Name));

            API.triggerClientEvent(sender, "ShowBusMenu", json);
        }

        [Command("dodajbusbez", GreedyArg = true)]
        public void AddBusStop(Client sender, string name)
        {
            //if (sender.GetAccountController().AccountData.ServerRank < ServerRank.GameMaster)
            //{
            //    sender.Notify("Nie posiadasz uprawnień do usuwania przystanku autobusowego.");
            //    return;
            //}

            sender.Notify("Ustaw się na środku przystanku aby stworzyć sferyczną strefę o promieniu 5f.");
            sender.Notify("Ustaw się w wybranej pozycji, a następnie wpisz /tu.");
            sender.Notify("...użyj /diag aby poznać swoją obecną pozycję.");

            Vector3 center = null;

            API.onChatCommand += Handler;

            void Handler(Client o, string command, CancelEventArgs cancel)
            {
                if (center == null && o == sender && command == "/tu")
                {
                    cancel.Cancel = true;
                    center = o.position;
                    var busStop = new BusStopModel
                    {
                        Name = name,
                        Center = center,
                        CreatorForumName = o.GetAccountController().AccountData.Name,
                    };
                    XmlHelper.AddXmlObject(busStop, Constant.ConstantAssemblyInfo.XmlDirectory + @"BusStops\", busStop.Name);

                    sender.Notify("Dodawanie przystanku zakończyło się pomyślnie.");
                    _busStops.Add(new BusStop(API, busStop));

                    API.onChatCommand -= Handler;
                }
            }
        }

        [Command("usunbus")]
        public void DeleteBusStop(Client sender)
        {
            //if (sender.GetAccountController().AccountData.ServerRank < ServerRank.GameMaster)
            //{
            //    sender.Notify("Nie posiadasz uprawnień do usuwania przystanku autobusowego.");
            //    return;
            //}

            if (_busStops.Count == 0)
            {
                sender.Notify("Nie znaleziono drivethru które można usunąć.");
                return;
            }
            var busStop = _busStops.OrderBy(a => a.Data.Center.DistanceTo(sender.position)).First();
            if (XmlHelper.TryDeleteXmlObject(busStop.Data.FilePath))
            {
                sender.Notify("Usuwanie przystanku autobusowego zakończyło się pomyślnie.");
                _busStops.Remove(busStop);
                busStop.Dispose();
            }
            else
            {
                sender.Notify("Usuwanie przystanku autobusowego zakończyło się niepomyślnie.");
            }
        }
    }
}