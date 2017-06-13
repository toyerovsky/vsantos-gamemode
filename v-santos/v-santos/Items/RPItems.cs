using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using Newtonsoft.Json;
using Serverside.Core;
using Serverside.Core.Telephone;
using Serverside.Database.Models;
using Serverside.Groups;
//using Serverside.Groups.CrimeBots;
using Serverside.Core.Extensions;

namespace Serverside.Items
{
    sealed class RPItems : Script
    {
        public RPItems()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("RPItems uruchomione pomyslnie!");
        }

        private Item CreateItem(Database.Models.Item item)
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
                var player = sender.GetAccountController();
                int index = Convert.ToInt32(API.getEntityData(sender, "SelectedItem"));

                List<Database.Models.Item> userItems = player.CharacterController.Character.Items.ToList();

                Item item = CreateItem(userItems[index]);
                item.UseItem(player);
            }
            else if (eventName == "InformationsItem")
            {
                var player = sender.GetAccountController();

                int index = Convert.ToInt32(API.getEntityData(sender, "SelectedItem"));
                List<Database.Models.Item> userItems = player.CharacterController.Character.Items.ToList();

                Item item = CreateItem(userItems[index]);
                RPChat.SendMessageToPlayer(sender, item.ItemInfo, ChatMessageType.ServerInfo);

            }
            else if (eventName == "UsingInformationsItem")
            {
                var player = sender.GetAccountController();

                int index = Convert.ToInt32(API.getEntityData(sender, "SelectedItem"));
                List<Database.Models.Item> userItems = player.CharacterController.Character.Items.ToList();

                Item item = CreateItem(userItems[index]);
                RPChat.SendMessageToPlayer(sender, item.UseInfo, ChatMessageType.ServerInfo);

            }
            else if (eventName == "BackToItemList")
            {
                var player = sender.GetAccountController();
                string itemsJson = JsonConvert.SerializeObject(player.CharacterController.Character.Items.ToList());
                API.triggerClientEvent(sender, "ShowItems", itemsJson);
            }
            //args[0] to numer na jaki dzwoni
            else if (eventName == "OnPlayerTelephoneCall")
            {
                var player = sender.GetAccountController();

                if (Convert.ToInt32(args[0]) == 555 && player.CharacterController.OnDutyGroupId.HasValue &&
                    player.CharacterController.Character.Workers.First(x => x.Group.Id == player.CharacterController.OnDutyGroupId).Group.GroupType == GroupType.CrimeGroup)
                {
                    //CrimeGroup group =
                    //    RPGroups.Groups.Single(g => g.Id == player.CharacterController.OnDutyGroupId.Value) as
                    //        CrimeGroup;
                    //if (group != null && group.Data.GroupType == GroupType.CrimeGroup &&
                    //    group.CanPlayerCallCrimeBot(sender.GetAccountController()))
                    //{
                    //    List<string> names = CrimeBotHelper.GetPositions().Select(n => n.Name).ToList();
                    //    API.shared.triggerClientEvent(sender, "OnPlayerCalledCrimeBot", names);
                    //    return;
                    //}
                }

                if (sender.GetAccountController().CharacterController.CellphoneController.CurrentlyTalking)
                {
                    sender.Notify("Obecnie prowadzisz rozmowę telefoniczną. Zakończ ją klawiszem END.");
                    return;
                }
                ////animka dzwonienia przez telefon
                //API.shared.playPlayerAnimation(senderPlayer.Client, (int)(AnimationFlags.AllowPlayerControl | AnimationFlags.Loop),
                //    "cellphone@first_person", "cellphone_call_listen_base");

                if (API.getAllPlayers().Any(p => p.GetAccountController().CharacterController.CellphoneController.Number == (int)args[0]))
                {
                    var getter = API.getAllPlayers()
                        .Single(x => x.GetAccountController().CharacterController.CellphoneController.Number ==
                                     (int) args[0]);

                    if (getter.GetAccountController().CharacterController.CellphoneController.CurrentlyTalking)
                    {
                        API.shared.sendChatMessageToPlayer(sender, "~#ffdb00~",
                            "Wybrany abonent prowadzi obecnie rozmowę, spróbuj później.");
                        return;
                    }

                    var call = new TelephoneCall(sender, getter);
                    sender.GetAccountController().CharacterController.CellphoneController.CurrentCall = call;

                    call.Timer.Elapsed += (o, eventArgs) =>
                    {
                        API.shared.sendChatMessageToPlayer(sender, "~#ffdb00~",
                            "Wybrany abonent ma wyłączony telefon, bądź znajduje się poza zasięgiem, spróbuj później.");
                        call.Dispose();
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
                var call = sender.GetAccountController().CharacterController.CellphoneController.CurrentCall;
                call?.Dispose();

                //TODO Wyłączanie telefonu

            }
            //Rządanie otworzenia okienka telefonu
            else if (eventName == "OnPlayerPullCellphoneRequest")
            {
                var cellphone = sender.GetAccountController().CharacterController.CellphoneController;
                API.triggerClientEvent(sender, "OnPlayerPulledCellphone", cellphone.Data.Name,
                    JsonConvert.SerializeObject(cellphone.Contacts),
                    JsonConvert.SerializeObject(cellphone.Messages));
            }
            //Odebranie rozmowy
            else if (eventName == "OnPlayerCellphonePickUp")
            {
                var cellphone = sender.GetAccountController().CharacterController.CellphoneController;
                TelephoneCall telephoneCall = cellphone.CurrentCall;

                if (telephoneCall.Getter.GetAccountController().CharacterController.CellphoneController.CurrentlyTalking)
                {
                    API.sendChatMessageToPlayer(telephoneCall.Getter, "~#ffdb00~",
                        "Aby odebrać musisz zakończyć bieżące połączenie.");
                    return;
                }

                //API.shared.playPlayerAnimation(telephoneCall.Getter.Client, (int)(AnimationFlags.AllowPlayerControl),
                //    "cellphone@first_person", "cellphone_call_listen_base");

                telephoneCall.Open();

                API.sendChatMessageToPlayer(telephoneCall.Getter, "~#ffdb00~",
                    "Odebrano telefon, aby zakończyć rozmowę naciśnij klawisz END.");
                API.sendChatMessageToPlayer(telephoneCall.Sender, "~#ffdb00~",
                    "Rozmówca odebrał telefon, aby zakończyć rozmowę naciśnij klawisz END.");
            }
            else if (eventName == "OnPlayerCellphoneEnd")
            {
                var controller = sender.GetAccountController().CharacterController.CellphoneController;

                if (controller != null && controller.CurrentlyTalking)
                {
                    sender.GetAccountController().CharacterController.CellphoneController.CurrentCall = null;

                    API.shared.sendChatMessageToPlayer(controller.CurrentCall.Sender, "~#ffdb00~",
                        "Rozmowa zakończona.");
                    API.shared.sendChatMessageToPlayer(controller.CurrentCall.Getter, "~#ffdb00~",
                        "Rozmowa zakończona.");
                }
            }
            //args[0] to numer kontaktu args[1] to nazwa 
            else if (eventName == "OnPlayerTelephoneContactAdded")
            {
                if (!sender.GetAccountController().CharacterController.Character.Items
                    .Any(i => i.ItemType == (int)ItemType.Cellphone && i.CurrentlyInUse.HasValue &&
                              i.CurrentlyInUse.Value))
                {
                    sender.Notify("Musisz mieć włączony telefon.");
                    return;
                }

                int number = Convert.ToInt32(args[0]);
                string name = args[1].ToString();

                TelephoneContact c = new TelephoneContact
                {
                    Name = name,
                    Number = number,
                    Id = sender.GetAccountController().CharacterController.Character.Items
                        .Single(i => i.ItemType == (int)ItemType.Cellphone && i.CurrentlyInUse.HasValue &&
                                     i.CurrentlyInUse.Value).Id
                };
                sender.GetAccountController().CharacterController.CellphoneController.Contacts.Add(c);
                sender.GetAccountController().CharacterController.CellphoneController.Save();
            }
        }



        #region Komendy
        [Command("p")]
        public void ShowItemsList(Client sender)
        {
            API.triggerClientEvent(sender, "ShowItems", JsonConvert.SerializeObject(sender.GetAccountController().CharacterController.Character.Items.ToList()));
        }
        #endregion
    }
}