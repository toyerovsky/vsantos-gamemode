using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
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
            if(!sender.HasData("Bus")) return;
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
    }
}