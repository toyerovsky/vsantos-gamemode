using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using Serverside.Autonomic.Market.Models;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Items;

namespace Serverside.Autonomic.Market
{
    public sealed class RPMarket : Script
    {
        private List <Market> Markets { get; set; }

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
                 */
                var item = new MarketItem
                {
                    Name = arguments[0].ToString(),
                    ItemType = (ItemType)Enum.Parse(typeof(ItemType), (string)arguments[1]),
                    Cost = (decimal)arguments[2]
                };

                var names = (List<string>) arguments[3];
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
        }

        private void OnResourceStartHandler()
        {
            APIExtensions.ConsoleOutput("[RPMarket] Uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
            XmlHelper.GetXmlObjects<Models.Market>($@"{Constant.ConstantAssemblyInfo.XmlDirectory}\Markets\")
                .ForEach(x => Markets.Add(new Market(x)));
        }

        [Command("dodajprzedmiotsklep")]
        public void AddItemToShop(Client sender)
        {
            var values = Enum.GetNames(typeof(ItemType)).ToList();
            var markets = XmlHelper.GetXmlObjects<Models.Market>($@"{Constant.ConstantAssemblyInfo.XmlDirectory}\Markets\").Select(x => new {x.Id, x.Name}).ToList();
            API.triggerClientEvent(sender, "ShowAdminMarketItemMenu", values, markets);
        }

        [Command("kup")]
        public void BuyItemFromShop(Client sender)
        {
            
        }
    }
}