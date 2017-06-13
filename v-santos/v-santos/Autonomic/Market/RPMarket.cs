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
        private List <Models.Market> Markets { get; set; }

        public RPMarket()
        {
            API.onResourceStart += OnResourceStartHandler;
            API.onClientEventTrigger += OnClientEventTriggerHandler;
        }

        private void OnClientEventTriggerHandler(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "AddMarketItem")
            {
                var item = new MarketItem
                {
                    Name = arguments[0].ToString(),
                    ItemType = (ItemType)Enum.Parse(typeof(ItemType), (string)arguments[1]),
                    Cost = (decimal)arguments[2]
                };


            }
        }

        private void OnResourceStartHandler()
        {
            APIExtensions.ConsoleOutput("[RPMarket] Uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
            Markets = XmlHelper.GetXmlObjects<Models.Market>($@"{Constant.ConstantAssemblyInfo.XmlDirectory}\Markets\");
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