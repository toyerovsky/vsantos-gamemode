/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using Serverside.Core.Extensions;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using Newtonsoft.Json;
using Serverside.Admin.Enums;
using Serverside.Database;
using Serverside.Database.Models;
using Serverside.Groups.Enums;
using Serverside.Groups.Stucts;
using Serverside.Items;

namespace Serverside.Groups
{
    public class RPGroupWarehouse : Script
    {
        public static List<WarehouseOrderInfo> CurrentOrders { get; set; } = new List<WarehouseOrderInfo>();

        public RPGroupWarehouse()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "OnPlayerAddWarehouseItem")
            {
                /* Argumenty
                 * args[0] string nameResult,
                 * args[1] string itemTypeResult,
                 * args[2] int costResult, 
                 * args[3] string groupTypeResult, 
                 * args[4] int minimalCostResult, 
                 * args[5] int weeklyCountResult, 
                 * args[6] int firstParameterResult = null, 
                 * args[7] int secondParameterResult = null, 
                 * args[8] int thirdParameterResult = null
                 */

                if (Enum.TryParse(arguments[3].ToString(), out GroupType groupType) &&
                    Enum.TryParse(arguments[1].ToString(), out ItemType itemType))
                {
                    var groupWarehouseItem = new GroupWarehouseItem
                    {
                        Item = new Database.Models.Item
                        {
                            CreatorId = 0,
                            Name = arguments[0].ToString(),
                            ItemType = (int) itemType,
                        },
                        Cost = Convert.ToDecimal(arguments[2]),
                        MinimalCost = Convert.ToDecimal(arguments[4]),
                        ResetCount = Convert.ToInt32(arguments[5]),
                        GroupType = groupType
                    };


                    if ((int?) arguments[6] != null) groupWarehouseItem.Item.FirstParameter = (int) arguments[6];
                    if ((int?)arguments[6] != null) groupWarehouseItem.Item.SecondParameter = (int) arguments[7];
                    if ((int?)arguments[6] != null) groupWarehouseItem.Item.ThirdParameter = (int) arguments[8];

                    ContextFactory.Instance.GroupWarehouseItems.Add(groupWarehouseItem);
                    ContextFactory.Instance.SaveChanges();
                    sender.Notify("Dodawanie przedmiotu zakończyło się ~h~ ~g~ pomyślnie.");
                }
                else
                {
                    sender.Notify("Dodawanie przedmiotu zakończone ~h~ ~r~ niepowodzeniem.");
                }

            }
            else if (eventName == "OnPlayerPlaceOrder")
            {
                /* Argumenty
                 * args[0] - List<WarehouseItemInfo> JSON
                 */

                var player = sender.GetAccountController();
                var group = player.CharacterController.OnDutyGroup;
                if (group != null)
                {
                    var items =
                        JsonConvert.DeserializeObject<List<WarehouseItemInfo>>(arguments[0].ToString());

                    var sum = items.Sum(x => x.ItemInfo.Cost * x.Count);
                    if (group.HasMoney(sum))
                    {
                        var shipment = new GroupWarehouseOrder
                        {
                            Getter = group.GroupData,
                            OrderItemsJson = JsonConvert.SerializeObject(items),
                            ShipmentLog = $"[{DateTime.Now}] Złożenie zamówienia w magazynie. \n"
                        };

                        ContextFactory.Instance.GroupWarehouseOrders.Add(shipment);
                        ContextFactory.Instance.SaveChanges();
                        group.RemoveMoney(sum);

                        CurrentOrders.Add(new WarehouseOrderInfo
                        {
                            Data = ContextFactory.Instance.GroupWarehouseOrders.Single(x => x.OrderItemsJson == shipment.OrderItemsJson && x.Getter == shipment.Getter)
                        });

                        sender.Notify("Zamawianie przesyłki zakończyło się ~h~ ~g~ pomyślnie.");
                    }
                    else
                    {
                        sender.Notify($"Grupa {group.GetColoredName()} nie posiada wystarczającej ilości środków.");
                    }
                }
            }
        }

        private void API_onResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPGroupWarehouse] Uruchomione pomyślnie.", ConsoleColor.DarkMagenta);

            //foreach (var order in ContextFactory.Instance.GroupWarehouseOrders)
            //{
            //    CurrentOrders.Add(order);
            //}
        }

        [Command("dodajprzedmiotmag")]
        public void AddWarehouseItem(Client sender)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.GameMaster4)
            {
                sender.Notify("Nie posiadasz uprawnień do tworzenia grupy.");
                return;
            }

            sender.triggerEvent("ShowAdminWarehouseItemMenu");
        }
    }
}