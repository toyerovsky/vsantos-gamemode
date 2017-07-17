/* Copyright (C) Przemysław Postrach - All Rights Reserved
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
using Serverside.Core.DriveThru.Models;
using Serverside.Core.Extensions;
using Serverside.Database;
using Serverside.Items;
using Item = Serverside.Database.Models.Item;

namespace Serverside.Core.DriveThru
{
    public class RPDriveThru : Script
    {
        private List<DriveThru> DriveThrus { get; set; } = new List<DriveThru>();

        public RPDriveThru()
        {
            API.onResourceStart += OnResourceStart; 
            API.onClientEventTrigger += OnClientEventTrigger;
        }

        private void OnClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "OnPlayerDriveThruBought")
            {
                var money = Convert.ToDecimal(arguments[2]);
                if (!sender.HasMoney(money))
                {
                    sender.Notify("Nie posiadasz wystarczającej ilości gotówki.");
                    return;
                }
                sender.RemoveMoney(money);

                var player = sender.GetAccountController();

                Item item = new Item
                {
                    Name = (string)arguments[0],
                    Character = player.CharacterController.Character,
                    CreatorId = 0,
                    ItemType = (int)ItemType.Food,
                    FirstParameter = (int)arguments[1],
                };

                ContextFactory.Instance.Items.Add(item);
                ContextFactory.Instance.SaveChanges();
                sender.Notify("Dodano przedmiot do ekwipunku.");
            }
        }

        private void OnResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPDriveThru] Uruchomione pomyślnie.", ConsoleColor.DarkMagenta);

            foreach (var driveThru in XmlHelper.GetXmlObjects<DriveThruModel>($"{Constant.ConstantAssemblyInfo.XmlDirectory}DriveThrus\\"))
            {
                DriveThrus.Add(new DriveThru(API, driveThru));
            }
        }

        [Command("dodajdrivethru", GreedyArg = true)]
        public void AddBusStop(Client sender, string name)
        {
            //if (sender.GetAccountController().AccountData.ServerRank < ServerRank.GameMaster)
            //{
            //    sender.Notify("Nie posiadasz uprawnień do usuwania przystanku autobusowego.");
            //    return;
            //}

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
                    var driveThru = new DriveThruModel    
                    {
                        Position = o.position,
                        CreatorForumName = o.GetAccountController().AccountData.Name,
                    };
                    XmlHelper.AddXmlObject(driveThru, $"{Constant.ConstantAssemblyInfo.XmlDirectory}DriveThrus\\");

                    sender.Notify("Dodawanie przystanku zakończyło się pomyślnie.");
                    DriveThrus.Add(new DriveThru(API, driveThru));

                    API.onChatCommand -= Handler;
                }
            }
        }

        [Command("usundrivethru")]
        public void DeleteBusStop(Client sender)
        {
            //if (sender.GetAccountController().AccountData.ServerRank < ServerRank.GameMaster)
            //{
            //    sender.Notify("Nie posiadasz uprawnień do usuwania przystanku autobusowego.");
            //    return;
            //}
            if (DriveThrus.Count == 0)
            {
                sender.Notify("Nie znaleziono drivethru które można usunąć.");
                return;
            }
            var driveThru = DriveThrus.OrderBy(d => d.Data.Position.DistanceTo2D(sender.position)).First();
            if (XmlHelper.TryDeleteXmlObject(driveThru.Data.FilePath))
            {
                sender.Notify("Usuwanie drivethru zakończyło się pomyślnie.");
                DriveThrus.Remove(driveThru);
                driveThru.Dispose();
            }
            else
            {
                sender.Notify("Usuwanie przystanku autobusowego zakończyło się niepomyślnie.");
            }
        }
    }
}