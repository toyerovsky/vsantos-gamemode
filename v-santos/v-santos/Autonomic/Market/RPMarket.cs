using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using Serverside.Autonomic.Market.Models;
using Serverside.Controllers;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Items;

namespace Serverside.Autonomic.Market
{
    public sealed class RPMarket : Script
    {
        private List<MarketItem> MarketItems { get; set; }

        public RPMarket()
        {
            API.onResourceStart += OnResourceStartHandler;
            API.onClientEventTrigger += OnClientEventTriggerHandler;
        }

        private void OnClientEventTriggerHandler(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "AddMarketItem")
            {
                MarketItem item = new MarketItem()
                {
                    Name = arguments[0].ToString(),
                    ItemType = (ItemType)Enum.Parse(typeof(ItemType), (string)arguments[1]),
                    Cost = (decimal)arguments[2]
                };
                XmlHelper.AddXmlObject(item, $@"{Constant.ConstantAssemblyInfo.XmlDirectory}\MarketItems\", item.Name);
            }
        }

        private void OnResourceStartHandler()
        {
            APIExtensions.ConsoleOutput("[RPMarket] Uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
            MarketItems = XmlHelper.GetXmlObjects<MarketItem>($@"{Constant.ConstantAssemblyInfo.XmlDirectory}\MarketItems\");
        }

        [Command("dodajprzedmiotsklep")]
        public void AddItemToShop(Client sender)
        {

            List<string> values = Enum.GetNames(typeof(ItemType)).ToList();
            API.triggerClientEvent(sender, "ShowAdminMarketItemMenu", values);
        }

        [Command("kup")]
        public void BuyItemFromShop(Client sender)
        {
            
        }
    }
}