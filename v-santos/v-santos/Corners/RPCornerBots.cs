﻿using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Corners.Models;
using Serverside.Items;

namespace Serverside.Corners
{
    public class RPCornerBots : Script
    {
        public RPCornerBots()
        {
            API.onClientEventTrigger += ClientEventTriggerHandler;    
        }

        private void ClientEventTriggerHandler(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "AddCornerBot")
            {
                CornerBotModel bot = new CornerBotModel
                {
                    BotId = XmlHelper.GetXmlObjects<CornerBotModel>(Constant.ConstantAssemblyInfo.XmlDirectory + @"CornerBots\").Count + 1,
                    Name = Convert.ToString(arguments[0]),
                    PedHash = (PedHash) arguments[1],
                    MoneyCount = Convert.ToDecimal(arguments[2]),
                    DrugType = (DrugType) Enum.Parse(typeof(DrugType), (string) arguments[3]),
                    Greeting = (string) arguments[4],
                    GoodFarewell = (string) arguments[5],
                    BadFarewell = (string) arguments[6]
                };
                XmlHelper.AddXmlObject(bot, Constant.ConstantAssemblyInfo.XmlDirectory + @"CornerBots\");
                sender.Notify("Dodanie NPC zakończone pomyślnie.");
            }
        }

        [Command("dodajbotc")]
        public void AddCornerBot(Client sender)
        {
            List<int> values = Enum.GetNames(typeof(PedHash)).Select(ped => (int) Enum.Parse(typeof(PedHash), ped)).ToList();
            API.triggerClientEvent(sender, "ShowCornerBotMenu", values);
        } 
    }
}