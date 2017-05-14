using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Core;
using Serverside.Database;

namespace Serverside.Items
{
    #region Enumy
    public enum ItemType
    {
        Food,
        Weapon,
        WeaponClip,
        Mask,
        Drug,
        Dice,
        Watch,
        Cloth,
        Transmitter,
        Alcohol,
        Cellphone,
        Tuning
    }

    public enum TuningType
    {
        Speed,
        Brakes,
    }

    public enum DrugType
    {
        Marihuana,
        Lsd,
        Ekstazy,
        Amfetamina,
        Metaamfetamina,
        Crack,
        Kokaina,
        Haszysz,
        Heroina
    }
    #endregion

    internal abstract class Item
    {
        protected API API = API.shared;

        protected long ItemId { get; set; }
        protected string Name { get; set; }

        protected int? FirstParameter { get; set; }
        protected int? SecondParameter { get; set; }
        protected int? ThirdParameter { get; set; }

        public virtual string ItemInfo { get; }

        public virtual string UseInfo { get; }

        public virtual void UseItem(Player player)
        {
            //TODO Pomysł, można tutaj dopisywać wszystkie logi używania przedmiotów
        }

        protected void PrepareCurrentlyInUse()
        {
            ItemEditor editor = RPCore.Db.SelectItem(ItemId);
            editor.CurrentlyInUse = false;
            RPCore.Db.UpdateItem(editor);
        }
    }

    internal class Food : Item
    {
        /// <summary>
        /// Pierwszy parametr to ilość HP do przyznania
        /// </summary>
        /// <param name="item"></param>
        public Food(ItemEditor item)
        {
            ItemId = item.IID;
            Name = item.Name;
            FirstParameter = item.FirstParameter;
        }

        public override void UseItem(Player player)
        {
            RPChat.SendMessageToNearbyPlayers(player.Client, String.Format("zjada {0}", Name), ChatMessageType.ServerMe);
            API.setPlayerHealth(player.Client, player.Client.health + Convert.ToInt32(FirstParameter));
            RPCore.Db.DeleteItem(ItemId);
        }

        public override string ItemInfo
        {
            get { return "Ten przedmiot to " + Name + " o ItemId: " + ItemId; }
        }

        public override string UseInfo
        {
            get { return "Ten przedmiot odnawia: " + FirstParameter + " punktów życia."; }
        }

    }

    internal class Weapon : Item
    {
        /// <summary>
        /// Pierwszy parametr to Hash broni, a drugi to ilość amunicji
        /// </summary>
        /// <param name="item"></param>
        public Weapon(ItemEditor item)
        {
            ItemId = item.IID;
            Name = item.Name;
            FirstParameter = item.FirstParameter;
            SecondParameter = item.SecondParameter;
        }

        public override void UseItem(Player player)
        {
            
            if(RPCore.Db.SelectItem(ItemId).CurrentlyInUse == null)
            {
                PrepareCurrentlyInUse();
            }

            if (RPCore.Db.SelectItem(ItemId).SecondParameter <= 0)
            {
                player.Notify("Twoja broń nie ma amunicji.");
            }

            if (Convert.ToBoolean(RPCore.Db.SelectItem(ItemId).CurrentlyInUse))
            {
                var itemEditor = RPCore.Db.SelectItem(ItemId);
                itemEditor.CurrentlyInUse = false;
                itemEditor.SecondParameter = API.getPlayerWeaponAmmo(player.Client, (WeaponHash)FirstParameter);
                RPCore.Db.UpdateItem(itemEditor);
                if (FirstParameter != null) API.removePlayerWeapon(player.Client, (WeaponHash)FirstParameter);

                API.onPlayerDisconnected -= API_onPlayerDisconnected;
            }
            else if (!Convert.ToBoolean(RPCore.Db.SelectItem(ItemId).CurrentlyInUse) && SecondParameter > 0)
            {
                //API.givePlayerWeapon(Client player, WeaponHash weaponHash, int ammo, bool equipNow, bool ammoLoaded);
                if (FirstParameter != null)
                    API.givePlayerWeapon(player.Client, (WeaponHash)FirstParameter, Convert.ToInt32(SecondParameter), true, true);

                var itemEditor = RPCore.Db.SelectItem(ItemId);
                itemEditor.CurrentlyInUse = true;
                RPCore.Db.UpdateItem(itemEditor);

                API.onPlayerDisconnected += API_onPlayerDisconnected;
            }
        }

        private void API_onPlayerDisconnected(Client sender, string reason)
        {
            Player player = RPCore.Players.Single(p => sender.hasData("AccountID") && p.Key == sender.getData("AccountID")).Value;
            foreach (var item in player.Items)
            {
                if (item.ItemType == (int)ItemType.Weapon)
                {
                    var itemEditor = RPCore.Db.SelectItem(ItemId);
                    itemEditor.SecondParameter = API.getPlayerWeaponAmmo(player.Client, (WeaponHash)FirstParameter);
                    RPCore.Db.UpdateItem(itemEditor);
                }
            }

            API.onPlayerDisconnected -= API_onPlayerDisconnected;
        }

        public override string ItemInfo
        {
            get { return "Ten przedmiot to " + Name + " o ItemId: " + ItemId; }
        }
    }

    internal class WeaponClip : Item
    {
        /// <summary>
        /// Pierwszy parametr to hash broni do której pasuje, a drugi to ilość amunicji
        /// </summary>
        /// <param name="item"></param>
        public WeaponClip(ItemEditor item)
        {
            ItemId = item.IID;
            Name = item.Name;
            FirstParameter = item.FirstParameter;
            SecondParameter = item.SecondParameter;
        }

        public override void UseItem(Player player)
        {
            if (!player.HasData("CharacterID") || FirstParameter == null || SecondParameter == null) return;

            List<ItemList> weapons = RPCore.Db.SelectItemsList(player.GetData("CharacterID"), 1);
            weapons = (List<ItemList>)weapons.Where(v => v.ItemType == (int)ItemType.Weapon);

            //TODO poprawić wybieranie w ogole dziwnie dziala

            //API.triggerClientEvent(player.Client, "SelectWeaponToLoad", RPCore.Db.ConvertCollectionToClient(weapons), ItemId);
        }

        public override string ItemInfo
        {
            get { return "Ten przedmiot to " + Name + " o ItemId: " + ItemId; }
        }
        public override string UseInfo
        {
            get { return "Ten przedmiot dodaje " + SecondParameter + " naboi do broni " + (WeaponHash)FirstParameter; }
        }
    }

    internal class Mask : Item
    {
        /// <summary>
        /// Pierwszy parametr to liczba liczba użyć do zniszczenia
        /// </summary>
        /// <param name="item"></param>
        public Mask(ItemEditor item)
        {
            ItemId = item.IID;
            Name = item.Name;
            FirstParameter = item.FirstParameter;
        }

        public override void UseItem(Player player)
        {
            var encryptedName = "Nieznajomy " + player.Nickname.GetHashCode().ToString().Substring(1, 6);

            if (Convert.ToBoolean(RPCore.Db.SelectItem(ItemId).CurrentlyInUse) && player.HasData("CharacterID"))
            {
                RPChat.SendMessageToNearbyPlayers(player.Client, "zdejmuje kominiarkę", ChatMessageType.Me);

                CharacterEditor character = RPCore.Db.SelectCharacter(player.GetData("CharacterID"));
                player.Client.name = character.Name + " " + character.Surname;
                player.Client.resetNametag();

                var itemEditor = RPCore.Db.SelectItem(ItemId);
                itemEditor.CurrentlyInUse = false;
                RPCore.Db.UpdateItem(itemEditor);
                if (FirstParameter == 0) RPCore.Db.DeleteItem(ItemId);
            }
            else
            {
                RPChat.SendMessageToNearbyPlayers(player.Client, "zakłada kominiarkę", ChatMessageType.Me);
                player.Client.name = encryptedName;
                player.Client.resetNametag();

                var maskEditor = RPCore.Db.SelectItem(ItemId);
                maskEditor.FirstParameter -= 1;
                maskEditor.CurrentlyInUse = true;

                RPCore.Db.UpdateItem(maskEditor);
            }
        }

        public override string ItemInfo
        {
            get { return "Ten przedmiot to " + Name + " o ItemId: " + ItemId; }
        }

        public override string UseInfo
        {
            get { return "Ten przedmiot ukrywa twoją nazwę wyświetlaną."; }
        }
    }

    internal class Drug : Item
    {
        public Drug(ItemEditor item)
        {
            ItemId = item.IID;
            Name = item.Name;
            FirstParameter = item.FirstParameter;
        }

        public override string ItemInfo
        {
            get { return "Ten przedmiot to " + Name + " o ItemId: " + ItemId; }
        }
    }

    internal class Dice : Item
    {
        /// <summary>
        /// Pierwszy parametr to liczba oczek na kostce
        /// </summary>
        /// <param name="item"></param>
        public Dice(ItemEditor item)
        {
            ItemId = item.IID;
            Name = item.Name;
            FirstParameter = item.FirstParameter;
        }

        public override void UseItem(Player player)
        {
            Random random = new Random();
            RPChat.SendMessageToNearbyPlayers(player.Client, String.Format(
                "wyrzucił {0} oczek z {1} możliwych", random.Next(1, Convert.ToInt32(FirstParameter)), FirstParameter), ChatMessageType.ServerMe);
        }

        public override string ItemInfo
        {
            get { return "Ten przedmiot to " + Name + " o ItemId: " + ItemId; }
        }

        public override string UseInfo
        {
            get { return "Ten przedmiot zwraca losową liczbę od 1 do " + FirstParameter; }
        }
    }

    internal class Watch : Item
    {
        /// <summary>
        /// Brak parametrów
        /// </summary>
        /// <param name="item"></param>
        public Watch(ItemEditor item)
        {
            ItemId = item.IID;
            Name = item.Name;
        }

        public override void UseItem(Player player)
        {
            RPChat.SendMessageToNearbyPlayers(player.Client, String.Format("spogląda na zegarek {0}", Name), ChatMessageType.Me);
            API.sendNotificationToPlayer(player.Client, "Godzina: " + DateTime.Now.ToShortTimeString());
        }

        public override string ItemInfo
        {
            get { return "Ten przedmiot to " + Name + " o ItemId: " + ItemId; }
        }

        public override string UseInfo
        {
            get { return "Ten przedmiot pokazuje bieżącą godzinę."; }
        }
    }

    internal class Cloth : Item
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public Cloth(ItemEditor item)
        {
            ItemId = item.IID;
            Name = item.Name;
        }

        public override void UseItem(Player player)
        {

        }

        public override string ItemInfo
        {
            get { return "Ten przedmiot to " + Name + " o ItemId: " + ItemId; }
        }

        public override string UseInfo
        {
            get { return "Ten przedmiot zmienia ubranie twojej postaci."; }
        }
    }

    internal class Transmitter : Item
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public Transmitter(ItemEditor item)
        {
            ItemId = item.IID;
            Name = item.Name;
            FirstParameter = item.FirstParameter;
        }

        public override void UseItem(Player player)
        {

        }

        public override string ItemInfo
        {
            get { return "Ten przedmiot to " + Name + " o ItemId: " + ItemId; }
        }

        public override string UseInfo
        {
            get { return "Przedmiot służy do komunikacji na podanej częstotliwości w zasięgu:" + SecondParameter; }
        }
    }

    internal class Cellphone : Item
    {
        /// <summary>
        /// 1 parametr to liczba kontaktów do zapisania, 2 to liczba sms możliwych do zapisania, 3 to numer telefonu
        /// </summary>
        /// <param name="item"></param>
        public Cellphone(ItemEditor item)
        {
            ItemId = item.IID;
            Name = item.Name;
            FirstParameter = item.FirstParameter;
            SecondParameter = item.SecondParameter;
            ThirdParameter = item.ThirdParameter;
        }

        public override void UseItem(Player player)
        {
            var editor = player.Helper.SelectItem(ItemId);

            if (editor.CurrentlyInUse != null && !(bool)editor.CurrentlyInUse)
            {
                RPChat.SendMessageToNearbyPlayers(player.Client, String.Format("włącza {0}", Name), ChatMessageType.ServerMe);
                player.SetData("CellphoneNumber", (int)ThirdParameter);
                player.SetSyncedData("CellphoneID", (int)ItemId);

                player.Notify(String.Format("Telefon {0} został włączony naciśnij klawisz END, aby go używać.", Name));
                editor.CurrentlyInUse = true;
                player.Helper.UpdateItem(editor);
            }
            else
            {
                RPChat.SendMessageToNearbyPlayers(player.Client, String.Format("wyłącza {0}", Name), ChatMessageType.ServerMe);
                player.ResetSyncedData("CellphoneID");
                player.ResetData("CellphoneNumber");

                player.Notify(String.Format("Telefon {0} został wyłączony.", Name));
                editor.CurrentlyInUse = false;
                player.Helper.UpdateItem(editor);
            }
        }

        public override string ItemInfo
        {
            get { return "Ten przedmiot to " + Name + " o ItemId: " + ItemId; }
        }

        public override string UseInfo
        {
            get
            {
                return "Telefon: " + Name + " może przechowywać: " + FirstParameter + " kontaktów, oraz " +
                  SecondParameter + " wiadomości SMS.";
            }
        }
    }

    internal class Tuning : Item
    {
        /// <summary>
        /// 1 to typ tuningu, pozostałe parametry zależą od typu tuningu
        /// dla 1 (speed) 2 parametr engineMultipilier, 3 torque
        /// dla 2 (brakes) 2 parametr to moc hamowania
        /// </summary>
        /// <param name="item"></param>
        public Tuning(ItemEditor item)
        {
            ItemId = item.IID;
            Name = item.Name;
            FirstParameter = item.FirstParameter;
            SecondParameter = item.SecondParameter;
            ThirdParameter = item.ThirdParameter;
        }

        public override string ItemInfo
        {
            get { return "Ten przedmiot to " + Name + " o ItemId: " + ItemId; }
        }

        public override string UseInfo
        {
            get
            {
                if ((TuningType)FirstParameter == TuningType.Speed)
                {
                    return "Tuning: " + Name + " zwiększa prędkość maksymalną o: " + SecondParameter +
                           " procent, oraz zwiększa moment obrotowy o: " +
                           ThirdParameter + " procent.";
                }
                return "Tuning: " + Name + " zwiększa moc hamulcy o: " + SecondParameter + " procent.";
            }
        }
    }
}