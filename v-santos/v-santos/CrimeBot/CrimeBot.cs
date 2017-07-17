/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Timers;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using Newtonsoft.Json;
using Serverside.Constant;
using Serverside.Controllers;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.CrimeBot.Models;
using Serverside.Database;
using Serverside.Groups.Base;
using Serverside.Items;

namespace Serverside.CrimeBot
{
    public sealed class CrimeBot : Bot
    {
        private API Api { get; }
        private GroupController Group { get; }
        private Database.Models.CrimeBot CrimeBotData { get; }

        private List<CrimeBotItem> Items { get; set; } = new List<CrimeBotItem>();

        private ColShape BotShape { get; set; }
        private AccountController Player { get; }
        private VehicleController Vehicle { get; set; }
        private FullPosition VehiclePosition { get; }

        public CrimeBot(AccountController player, CrimeGroup group, FullPosition vehiclePosition, API api, string name, PedHash hash, FullPosition position) : base(api, name, hash, position)
        {
            Api = api;
            Player = player;
            Group = group;
            VehiclePosition = vehiclePosition;

            CrimeBotData = ContextFactory.Instance.CrimeBots.Single(c => c.Group.Id == group.Id);

            var properties = new List<PropertyInfo> {null};
            properties.AddRange(typeof(Database.Models.CrimeBot).GetProperties()
                .Where(f => f.GetValue(CrimeBotData) != null && (f.PropertyType == typeof(int?) || f.PropertyType == typeof(decimal?))));

            if (properties.Count % 3 != 1)
            {
                player.Client.Notify($"Konfiguracja bota grupy {Group.GetColoredName()} jest nieporawna, skontaktuj się z administratorem.");
                return;
            }

            for (int i = 0; i < properties.Count; i += 3)
            {
                if (i == 0) continue;

                var info = ConstantItems.GetCrimeBotItemName(properties[i - 2].Name);
                Items.Add(new CrimeBotItem(info.Item1, ((decimal?)properties[i - 2].GetValue(CrimeBotData)).Value, ((int?)properties[i - 1].GetValue(CrimeBotData)).Value, ((int?)properties[i].GetValue(CrimeBotData)).Value, info.Item2, properties[i - 1].Name));
                
            }
            Items.ForEach(x => API.shared.sendChatMessageToPlayer(Player.Client, $"Nazwa {x.Name} Koszt {x.Cost}, Ilość {x.Count}, Pole {x.DatabaseField}"));
        }

        public override void Intialize()
        {
            base.Intialize();

            Vehicle = new VehicleController(VehiclePosition, CrimeBotData.Vehicle, CrimeBotData.Name, 0, 0, new Color(0, 0, 0), new Color(0, 0, 0));
            Vehicle.Vehicle.openDoor(5);
            BotHandle.playScenario("WORLD_HUMAN_SMOKING");

            BotShape = Api.createCylinderColShape(BotHandle.position, 3f, 3f);
            BotShape.onEntityEnterColShape += BotShape_onEntityEnterColShape;
            Api.onClientEventTrigger += Api_onClientEventTrigger;

            Timer timer = new Timer(1800000);
            timer.Start();
            timer.Elapsed += (o, e) =>
            {
                Dispose(true);
                timer.Dispose();
            };
        }

        private void Api_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            //args 0 to string JSON z js który mówi co gracz kupił
            if (eventName == "OnCrimeBotBought")
            {
                List<CrimeBotItem> items = JsonConvert.DeserializeObject<List<CrimeBotItem>>(arguments[0].ToString()).Where(item => item.Count > 0).ToList();

                decimal sum = 0;
                foreach (var item in items)
                {
                    if (item.Count != 0) sum += item.Cost * item.Count;
                }

                if (!sender.HasMoney(sum))
                {
                    SendMessageToNerbyPlayers($"Co to jest? Brakuje ${sum - sender.GetAccountController().CharacterController.Character.Money}, forsa w gotówce", ChatMessageType.Normal);
                    return;
                }

                //Sprawdzamy czy gracz nie chce kupić więcej niż ma bot
                if (items.Any(crimeBotItem => Items.First(x => x.Name == crimeBotItem.Name).Count < crimeBotItem.Count))
                {
                    return;
                }

                foreach (var i in items)
                {
                    //TYPY: 0 broń, 1 amunicja, 2 narkotyki 
                    var item = new Database.Models.Item();
                    
                    if (i.Type == ItemType.Weapon)
                    {
                        var data = ConstantItems.GetWeaponData(i.Name);

                        item.Character = sender.GetAccountController().CharacterController.Character;
                        item.CreatorId = 0;
                        item.Name = i.Name;
                        item.ItemType = (int)i.Type;
                        item.FirstParameter = (int)data.Item1;
                        item.SecondParameter = data.Item2;
                    }
                    else if (i.Type == ItemType.WeaponClip)
                    {
                        var data = ConstantItems.GetWeaponData(i.Name);

                        item.Character = sender.GetAccountController().CharacterController.Character;
                        item.CreatorId = 0;
                        item.Name = i.Name;
                        item.ItemType = (int)i.Type;
                        item.FirstParameter = (int)data.Item1;
                        item.SecondParameter = data.Item2;
                    }
                    else if (i.Type == ItemType.Drug)
                    {
                        item.Character = sender.GetAccountController().CharacterController.Character;
                        item.CreatorId = 0;
                        item.Name = i.Name;
                        item.ItemType = (int) i.Type;
                        item.FirstParameter = (int) Enum.Parse(typeof(DrugType), i.Name);
                    }
                    else
                    {
                        return;
                    }

                    //Ja sobie zdaję sprawę że to karygodne
                    var field = typeof(Database.Models.CrimeBot).GetProperties().Single(f => f.Name == i.DatabaseField);
                    field.SetValue(CrimeBotData, (int)field.GetValue(CrimeBotData) - i.Count);
                    ContextFactory.Instance.Items.Add(item);
                }

                sender.RemoveMoney(sum);

                ContextFactory.Instance.CrimeBots.Attach(CrimeBotData);
                ContextFactory.Instance.Entry(CrimeBotData).State = EntityState.Modified;
                ContextFactory.Instance.SaveChanges();

                EndTransaction();
                Dispose(false);
            }
        }

        private void BotShape_onEntityEnterColShape(ColShape shape, NetHandle entity)
        {
            if (Api.getEntityType(entity) == EntityType.Player && entity == Player.Client)
            {
                Api.triggerClientEvent(Player.Client, "ShowCrimeBotCef", JsonConvert.SerializeObject(Items.OrderBy(x => x.Type)));
            }
            else if (Api.getEntityType(entity) == EntityType.Player)
            {
                SendMessageToNerbyPlayers("Odwal się", ChatMessageType.Normal);
            }
        }

        public override void Dispose()
        {
            Api.onClientEventTrigger -= Api_onClientEventTrigger;
            base.Dispose();
        }

        private void Dispose(bool disposing)
        {
            BotShape.onEntityEnterColShape -= BotShape_onEntityEnterColShape;
            Api.triggerClientEvent(Player.Client, "DisposeCrimeBotComponents");
            if (disposing)
            {
                Dispose();
            }
        }

        private void EndTransaction()
        {
            SendMessageToNerbyPlayers("Interesy z Tobą to przyjemność", ChatMessageType.Normal);
            Vehicle.Vehicle.closeDoor(5);
        }
    }
}