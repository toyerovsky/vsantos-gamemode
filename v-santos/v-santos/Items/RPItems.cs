using GTANetworkServer;
using Serverside.Core;
using Serverside.Core.Telephone;
using Serverside.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Serverside.Items
{
    sealed class RPItems : Script
    {
        public static List<TelephoneCall> CurrentCalls { get; private set; }

        private Dictionary<long, Player> Players
        {
            get { return RPCore.Players; }
        }

        public RPItems()
        {
            API.onResourceStart += API_onResourceStart;
            API.onResourceStop += API_onResourceStop;
            API.onClientEventTrigger += API_onClientEventTrigger;

            CurrentCalls = new List<TelephoneCall>();
        }

        private void API_onResourceStop()
        {
            foreach (var v in Players)
            {
                List<ItemEditor> items = v.Value.Items.Select(i => v.Value.Helper.SelectItem(i.IID)).Where(
                    it => it.CurrentlyInUse != null && it.CurrentlyInUse == true).ToList();

                foreach (var i in items)
                {
                    i.CurrentlyInUse = false;
                    v.Value.Helper.UpdateItem(i);
                }
            }
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("RPItems uruchomione pomyslnie!");
        }

        private Item CreateItem(ItemEditor item)
        {
            var itemType = (ItemType)item.ItemType;
            switch (itemType)
            {
                case ItemType.Food: return new Food(item);
                case ItemType.Weapon: return new Weapon(item);
                case ItemType.WeaponClip: return new WeaponClip(item);
                case ItemType.Mask: return new Mask(item);
                case ItemType.Drug: return new Drug(item);
                case ItemType.Dice: return new Dice(item);
                case ItemType.Watch: return new Watch(item);
                case ItemType.Cloth: return new Cloth(item);
                case ItemType.Transmitter: return new Transmitter(item);
                case ItemType.Cellphone: return new Cellphone(item);
                case ItemType.Tuning: return new Tuning(item);

                default:
                    throw new NotSupportedException($"Podany typ przedmiotu {itemType} nie jest obsługiwany.");
            }
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] args)
        {
            if (eventName == "SelectedItem")
            {
                int index = Convert.ToInt32(args[0]);
                API.setEntityData(sender, "SelectedItem", index);
                API.triggerClientEvent(sender, "SelectOptionItem", index);
            }
            else if (eventName == "UseItem")
            {
                Player player = Players.Single(p => sender.hasData("AccountID") && p.Key == sender.getData("AccountID")).Value;
                int index = Convert.ToInt32(API.getEntityData(sender, "SelectedItem"));

                List<ItemList> userItems = player.Helper.SelectItemsList(Convert.ToInt64(sender.getData("CharacterID")), 1);

                Item item = CreateItem(player.Helper.SelectItem(userItems[index].IID));
                item.UseItem(player);             
            }
            else if (eventName == "InformationsItem")
            {
                Player player = Players.Single(p => sender.hasData("AccountID") && p.Key == sender.getData("AccountID")).Value;

                int index = Convert.ToInt32(API.getEntityData(sender, "SelectedItem"));
                List<ItemList> userItems = player.Helper.SelectItemsList(Convert.ToInt64(sender.getData("CharacterID")), 1);
                
                Item item = CreateItem(player.Helper.SelectItem(userItems[index].IID));
                RPChat.SendMessageToPlayer(sender, item.ItemInfo, ChatMessageType.ServerInfo);
                
            }
            else if (eventName == "UsingInformationsItem")
            {
                Player player = Players.Single(p => sender.hasData("AccountID") && p.Key == sender.getData("AccountID")).Value;
                int index = Convert.ToInt32(API.getEntityData(sender, "SelectedItem"));
                List<ItemList> userItems = player.Helper.SelectItemsList(Convert.ToInt64(sender.getData("CharacterID")), 1);
                ItemEditor userItem = player.Helper.SelectItem(userItems[index].IID);

                Item item = CreateItem(player.Helper.SelectItem(userItems[index].IID));
                RPChat.SendMessageToPlayer(sender, item.UseInfo, ChatMessageType.ServerInfo);
                
            }
            else if (eventName == "BackToItemList")
            {
                Player player = Players.Single(p => sender.hasData("AccountID") && p.Key == sender.getData("AccountID")).Value;
                string itemsJson = JsonConvert.SerializeObject(player.Helper.SelectItemsList(Convert.ToInt64(sender.getData("CharacterID")), 1));
                API.triggerClientEvent(sender, "ShowItems", itemsJson);
            }
            //args[0] to numer na jaki dzwoni
            else if (eventName == "OnPlayerTelephoneCall")
            {
                Player senderPlayer = Players.First(p => p.Key == sender.getData("AccountID")).Value;

                if (senderPlayer.HasData("CellphoneTalking"))
                {
                    RPChat.SendMessageToPlayer(sender, "Obecnie już z kimś rozmawiasz.", ChatMessageType.ServerInfo);
                    return;
                }
                ////animka dzwonienia przez telefon
                //API.shared.playPlayerAnimation(senderPlayer.Client, (int)(AnimationFlags.AllowPlayerControl | AnimationFlags.Loop),
                //    "cellphone@first_person", "cellphone_call_listen_base");

                long? gid;
                
                if (Players.Any(p => p.Value.CellphoneNumber == Convert.ToInt32(args[0])))
                {                   
                    Player getterPlayer = Players.First(t => t.Value.CellphoneNumber == Convert.ToInt32(args[0])).Value;

                    if (getterPlayer.HasSyncedData("CellphoneTalking"))
                    {
                        API.shared.sendChatMessageToPlayer(sender, "~#ffdb00~",
                                "Wybrany abonent prowadzi obecnie rozmowę, spróbuj później.");
                        return;
                    }

                    TelephoneCall telephoneCall = new TelephoneCall(senderPlayer, getterPlayer);
                    CurrentCalls.Add(telephoneCall);
                    
                    telephoneCall.Timer.Elapsed += (o, eventArgs) =>
                    {
                        API.shared.sendChatMessageToPlayer(sender, "~#ffdb00~",
                            "Wybrany abonent ma wyłączony telefon, bądź znajduje się poza zasięgiem, spróbuj później.");
                        telephoneCall.Dispose();
                        CurrentCalls.Remove(telephoneCall);                    
                    };
                }
                else
                {
                    API.shared.sendChatMessageToPlayer(sender, "~#ffdb00~",
                        "Wybrany abonent ma wyłączony telefon, bądź znajduje się poza zasięgiem, spróbuj później.");
                }
            }
            else if (eventName == "OnPlayerTelephoneTurnoff")
            {
                Player player = Players.Single(p => sender.hasData("AccountID") && p.Key == sender.getData("AccountID")).Value;

                ItemEditor telephoneEditor = player.Helper.SelectItem(player.CellphoneId);
                if (telephoneEditor?.CurrentlyInUse != null && (bool)telephoneEditor.CurrentlyInUse)
                {
                    telephoneEditor.CurrentlyInUse = false;
                    player.Helper.UpdateItem(telephoneEditor);

                    TelephoneCall call = CurrentCalls.First(s => s.Sender.Client == sender || s.Getter.Client == sender);
                    if (call != null)
                    {
                        call.Dispose();
                        CurrentCalls.Remove(call);
                    }

                    player.ResetSyncedData("CellphoneID");

                    RPChat.SendMessageToNearbyPlayers(sender, String.Format("wyłącza {0}", telephoneEditor.Name), ChatMessageType.ServerMe);

                    API.sendNotificationToPlayer(sender, String.Format("Telefon {0} został wyłączony.", telephoneEditor.Name));
                }
            }
            else if (eventName == "OnPlayerPullCellphoneRequest")
            {
                Player player = Players.Single(p => sender.hasData("AccountID") && p.Key == sender.getData("AccountID")).Value;

                API.triggerClientEvent(sender, "OnPlayerPulledCellphone", player.Helper.SelectItem(player.CellphoneId).Name,
                    JsonConvert.SerializeObject(player.CellphoneContacts), JsonConvert.SerializeObject(player.CellphoneMessages));
            }
            else if (eventName == "OnPlayerCellphonePickUp")
            {
                TelephoneCall telephoneCall = CurrentCalls.First(s => s.Sender.Client == sender || s.Getter.Client == sender);

                if (telephoneCall.Getter.HasSyncedData("CellphoneTalking"))
                {
                    API.sendChatMessageToPlayer(telephoneCall.Getter.Client, "~#ffdb00~",
                        "Aby odebrać musisz zakończyć bieżące połączenie.");
                    return;
                }

                //API.shared.playPlayerAnimation(telephoneCall.Getter.Client, (int)(AnimationFlags.AllowPlayerControl),
                //    "cellphone@first_person", "cellphone_call_listen_base");

                telephoneCall.Open();

                API.sendChatMessageToPlayer(telephoneCall.Getter.Client, "~#ffdb00~",
                    "Odebrano telefon, aby zakończyć rozmowę naciśnij klawisz END.");
                API.sendChatMessageToPlayer(telephoneCall.Sender.Client, "~#ffdb00~",
                    "Rozmówca odebrał telefon, aby zakończyć rozmowę naciśnij klawisz END.");
            }
            else if (eventName == "OnPlayerCellphoneEnd")
            {
                TelephoneCall telephoneCall = CurrentCalls.First(s => s.Sender.Client == sender || s.Getter.Client == sender);

                if (telephoneCall != null && telephoneCall.CurrentlyTalking)
                {
                    telephoneCall.Dispose();
                    CurrentCalls.Remove(telephoneCall);

                    API.shared.sendChatMessageToPlayer(telephoneCall.Sender.Client, "~#ffdb00~",
                        "Rozmowa zakończona.");
                    API.shared.sendChatMessageToPlayer(telephoneCall.Getter.Client, "~#ffdb00~",
                        "Rozmowa zakończona.");
                }
            }
            //args[0] to numer kontaktu args[1] to nazwa 
            else if (eventName == "OnPlayerTelephoneContactAdded")
            {
                Player senderPlayer = Players.First(p => p.Key == sender.getData("AccountID")).Value;
                if (!senderPlayer.HasSyncedData("CellphoneID"))
                {
                    API.sendNotificationToPlayer(senderPlayer.Client, "Musisz mieć włączony telefon.");
                    return;
                }

                int number = Convert.ToInt32(args[0]);
                string name = args[1].ToString();

                TelephoneContactEditor c = new TelephoneContactEditor()
                {
                    Name = name,
                    Number = number,
                    TID = (long)senderPlayer.GetSyncedData("CellphoneID"), 
                };
                senderPlayer.Helper.AddContact(c);
            }
        }

        #region Komendy
        [Command("p")]
        public void ShowItemsList(Client sender)
        {
            Player player = Players.Single(p => sender.hasData("AccountID") && p.Key == sender.getData("AccountID")).Value;
            API.triggerClientEvent(sender, "ShowItems", JsonConvert.SerializeObject(player.Helper.SelectItemsList(player.Cid, 1)));
        }
        #endregion
    }
}