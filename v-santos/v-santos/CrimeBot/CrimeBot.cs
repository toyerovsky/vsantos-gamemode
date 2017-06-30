using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Timers;
using GTANetworkServer;
using GTANetworkShared;
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
        private Vehicle Vehicle { get; set; }
        private FullPosition VehiclePosition { get; }

        public CrimeBot(AccountController player, CrimeGroup group, FullPosition vehiclePosition, API api, string name, PedHash hash, FullPosition position) : base(api, name, hash, position)
        {
            Api = api;
            Player = player;
            Group = group;
            VehiclePosition = vehiclePosition;

            decimal tempCost = Decimal.MinValue;
            int tempDefaultCount = Int32.MinValue;
            int tempCount = Int32.MinValue;

            CrimeBotData = ContextFactory.Instance.CrimeBots.Single(c => c.Group.Id == group.Id);

            //Dorzucamy tutaj string żeby sposób z dzieleniem przez 3 miał sens
            var fields = typeof(Database.Models.CrimeBot).GetFields().Where(f => f.FieldType == typeof(int?) || f.FieldType == typeof(decimal?) || f.FieldType == typeof(string)).ToList();

            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i].GetValue(CrimeBotData) != null && fields[i].Name.Contains("Cost"))
                {
                    tempCost = ((decimal?)fields[i].GetValue(CrimeBotData)).Value;
                }
                else if (fields[i].GetValue(null) != null && fields[i].Name.Contains("Default"))
                {
                    tempDefaultCount = ((int?)fields[i].GetValue(CrimeBotData)).Value;
                }
                else if (fields[i].GetValue(null) != null && fields[i].Name.Contains("Count"))
                {
                    tempCount = ((int?)fields[i].GetValue(CrimeBotData)).Value;
                }

                if (i % 3 == 0 && (tempCost == Decimal.MinValue || tempDefaultCount == Int32.MinValue || tempCount == Int32.MinValue))
                {
                    tempCost = Decimal.MinValue;
                    tempDefaultCount = Int32.MinValue;
                    tempCount = Int32.MinValue;
                    continue;
                }

                if (tempCost != Decimal.MinValue && tempDefaultCount != Int32.MinValue && tempCount != Int32.MinValue)
                {
                    var info = ConstantItems.GetCrimeBotItemName(fields[i].Name);
                    Items.Add(new CrimeBotItem(info.Item1, tempCost, tempCount, tempDefaultCount, info.Item2, fields[i].Name.Replace("Default", "")));
                    tempCost = Decimal.MinValue;
                    tempDefaultCount = Int32.MinValue;
                    tempCount = Int32.MinValue;
                }
            }
        }

        public override void Intialize()
        {
            base.Intialize();
            //Rozwiązanie pomocnicze
            Vehicle = Api.createVehicle(CrimeBotData.Vehicle, VehiclePosition.Position, VehiclePosition.Rotation, 0, 0);
            Vehicle.invincible = true;
            Vehicle.engineStatus = false;
            Vehicle.openDoor(5);

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
                List<CrimeBotItem> items = JsonConvert.DeserializeObject<List<CrimeBotItem>>(arguments[0].ToString());

                decimal sum = 0;
                foreach (var item in items)
                {
                    if (item.Cost != null) sum += item.Cost.Value;
                }

                if (!sender.HasMoney(sum))
                {
                    SendMessageToNerbyPlayers($"Co to jest? Brakuje ${sum - sender.GetAccountController().CharacterController.Character.Money}, forsa w gotówce", ChatMessageType.Normal);
                    return;
                }

                //Sprawdzamy czy gracz nie chce kupić więcej niż ma bot
                foreach (var crimeBotItem in items)
                {
                    if (Items.First(x => x.Name == crimeBotItem.Name).Count < crimeBotItem.Count) return;
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
                        item.ItemType = (int)i.Type;
                        item.FirstParameter = (int)Enum.Parse(typeof(DrugType), i.Name);
                    }

                    //Ja sobie zdaję sprawę że to karygodne
                    var field = typeof(Database.Models.CrimeBot).GetFields().Single(f => f.Name == i.DatabaseField);
                    field.SetValue(CrimeBotData, ((int)field.GetValue(CrimeBotData) - 1));
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
                Api.triggerClientEvent(Player.Client, "OnPlayerEnteredCrimeBot", JsonConvert.SerializeObject(Items));
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
            Vehicle.closeDoor(5);
            BotHandle.movePosition(Vehicle.position, 3000);

            while (BotHandle.position != Vehicle.position)
            {
                NameLabel.position = BotHandle.position;
            }

            //(Ped ped, Vehicle vehicle, int seatIndex)
            Api.sendNativeToPlayersInRange(BotHandle.position, 30f, Hash.SET_PED_INTO_VEHICLE, BotHandle, Vehicle, -1);
            Api.setVehicleLocked(Vehicle, true);
        }
    }
}