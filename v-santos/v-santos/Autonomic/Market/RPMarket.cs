using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using Newtonsoft.Json;
using Serverside.Autonomic.Market.Models;
using Serverside.Core;
using Serverside.Core.Bus;
using Serverside.Core.Bus.Models;
using Serverside.Core.Extensions;
using Serverside.Database.Models;
using Serverside.Items;

namespace Serverside.Autonomic.Market
{
    public sealed class RPMarket : Script
    {
        private List<Market> Markets { get; set; }

        public RPMarket()
        {
            API.onResourceStart += OnResourceStartHandler;
            API.onClientEventTrigger += OnClientEventTriggerHandler;
        }

        private void OnClientEventTriggerHandler(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "AddMarketItem")
            {
                /*
                 * args[0] nameResult
                 * args[1] typeResult
                 * args[2] decimal costResult
                 * args[3] List<string> names
                 * args[4] FirstParameter
                 * args[5] SecondParameter
                 * args[6] ThirdParameter
                 */
                var item = new MarketItem
                {
                    Name = arguments[0].ToString(),
                    ItemType = (ItemType)Enum.Parse(typeof(ItemType), (string)arguments[1]),
                    Cost = (decimal)arguments[2],
                    FirstParameter = (int)arguments[4],
                    SecondParameter = (int)arguments[5],
                    ThirdParameter = (int)arguments[6]
                };

                var names = (List<string>)arguments[3];
                foreach (var name in names)
                {
                    var market = Markets.First(x => x.MarketData.Name == name);
                    if (market != null)
                    {
                        market.MarketData.Items.Add(item);
                        XmlHelper.AddXmlObject(market, $@"{Constant.ConstantAssemblyInfo.XmlDirectory}\Markets\", market.MarketData.Name);
                    }
                }
            }
            else if (eventName == "OnPlayerBoughtMarketItem")
            {
                //args[0] index
                if (!sender.HasData("CurrentMarket")) return;
                var market = (Market)sender.GetData("CurrentMarket");
                var item = market.MarketData.Items[(int) arguments[0]];
                if (!sender.HasMoney(item.Cost))
                {
                    sender.Notify("Nie posiadasz wystarczającej ilości gotówki.");
                    return;
                }
                sender.RemoveMoney(item.Cost);

                var controller = sender.GetAccountController();
                controller.CharacterController.Character.Items.Add(new Database.Models.Item
                {
                    CreatorId = 0,
                    ItemType = (int)item.ItemType,
                    Name = item.Name,
                    Character = controller.CharacterController.Character,
                    FirstParameter = item.FirstParameter,
                    SecondParameter = item.SecondParameter,
                    ThirdParameter = item.ThirdParameter
                });
                controller.Save();
            }
        }

        private void OnResourceStartHandler()
        {
            //TODO: Wczytywanie wszystkich IPL sklepów
            
                APIExtensions.ConsoleOutput("[RPMarket] Uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
                XmlHelper.GetXmlObjects<Models.Market>($@"{Constant.ConstantAssemblyInfo.XmlDirectory}\Markets\")
                    .ForEach(x => Markets.Add(new Market(x)));          
        }

        [Command("dodajprzedmiotsklep")]
        public void AddItemToShop(Client sender)
        {
            var values = Enum.GetNames(typeof(ItemType)).ToList();
            var markets = XmlHelper.GetXmlObjects<Models.Market>($@"{Constant.ConstantAssemblyInfo.XmlDirectory}\Markets\").Select(x => new { x.Id, x.Name }).ToList();
            API.triggerClientEvent(sender, "ShowAdminMarketItemMenu", values, markets);
        }

        [Command("kup")]
        public void BuyItemFromShop(Client sender)
        {
            if (!sender.HasData("CurrentMarket"))
            {
                sender.Notify("Nie znajdujesz się w sklepie");
                return;
            }
            var market = (Market) sender.GetData("CurrentMarket");
            if (market.MarketData.Items == null || market.MarketData.Items.Count == 0) return;
            sender.triggerEvent("ShowMarketMenu", JsonConvert.SerializeObject(market.MarketData.Items));
        }

        [Command("dodajsklep", "~y~UŻYJ ~w~ /dodajsklep [nazwa]")]
        public void AddMarket(Client sender, string name)
        {
            sender.Notify("Ustaw się na środku sklepu aby stworzyć sferyczną strefę o wybranym promeniu.");
            sender.Notify("Ustaw się w wybranej pozycji, a następnie wpisz /tu.");
            sender.Notify("...użyj /diag aby poznać swoją obecną pozycję.");

            Vector3 center = null;
            float radius = float.MinValue;

            API.onChatCommand += Handler;

            void Handler(Client o, string command, CancelEventArgs cancel)
            {
                if (center == null && o == sender && command == "/tu")
                {
                    cancel.Cancel = true;
                    center = o.position;
                    sender.Notify("Przejdź do pozycji końca promienia i wpisz /tu.");
                }
                else if (center != null && radius.Equals(float.MinValue) && o == sender && command == "/tu")
                {
                    radius = center.DistanceTo2D(o.position);
                    var market = new Models.Market
                    {
                        Id = XmlHelper.GetXmlObjects<Models.Market>(Constant.ConstantAssemblyInfo.XmlDirectory + @"Markets\").Count,
                        Name = name,
                        Center = center,
                        Radius = radius
                    };
                    XmlHelper.AddXmlObject(market, Constant.ConstantAssemblyInfo.XmlDirectory + @"Markets\", market.Name);
                    Markets.Add(new Market(market));
                    API.onChatCommand -= Handler;
                }
            }
        }
    }
}