using System;
using System.Data.Entity;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Controllers;
using Serverside.Database;
using Serverside.Core.Extensions;
using Serverside.Core;

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
        Brakes
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
        protected API Api = API.shared;

        protected Database.Models.Item Data { get; }

        public string ItemInfo => $"Ten przedmiot to: {Data.Name} o Id: {Data.Id}";

        public virtual string UseInfo { get; }

        protected Item(Database.Models.Item item)
        {
            Data = item;
        }

        public virtual void UseItem(AccountController player)
        {
            //TODO Pomysł, można tutaj dopisywać wszystkie logi używania przedmiotów
        }

        protected void PrepareCurrentlyInUse()
        {
            Data.CurrentlyInUse = false;
            Save();
        }

        protected void Save()
        {
            ContextFactory.Instance.Items.Attach(Data);
            ContextFactory.Instance.Entry(Data).State = EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }

        protected void Delete()
        {
            ContextFactory.Instance.Items.Remove(Data);
            ContextFactory.Instance.Entry(Data).State = EntityState.Deleted;
            ContextFactory.Instance.SaveChanges();
        }
    }

    internal class Food : Item
    {
        /// <summary>
        /// Pierwszy parametr to ilość HP do przyznania
        /// </summary>
        /// <param name="item"></param>
        public Food(Database.Models.Item item) : base(item)
        {
        }

        public override void UseItem(AccountController player)
        {
            RPChat.SendMessageToNearbyPlayers(player.Client, $"zjada {Data.Name}", ChatMessageType.ServerMe);
            Api.setPlayerHealth(player.Client, player.Client.health + Convert.ToInt32(Data.FirstParameter));
            Delete();
        }

        public override string UseInfo => $"Ten przedmiot odnawia: {Data.FirstParameter} punktów życia.";
    }

    internal class Weapon : Item
    {
        /// <summary>
        /// Pierwszy parametr to Hash broni, a drugi to ilość amunicji
        /// </summary>
        /// <param name="item"></param>
        public Weapon(Database.Models.Item item) : base(item) { }

        public override void UseItem(AccountController player)
        {
            if (!Data.CurrentlyInUse.HasValue) PrepareCurrentlyInUse();

            if (Data.SecondParameter <= 0)
            {
                player.Client.Notify("Twoja broń nie ma amunicji.");
                return;
            }

            if (Data.CurrentlyInUse != null && (Data.CurrentlyInUse.Value && Data.FirstParameter.HasValue))
            {
                Data.CurrentlyInUse = false;
                Data.SecondParameter = Api.getPlayerWeaponAmmo(player.Client, (WeaponHash)Data.FirstParameter);
                Save();
                Api.removePlayerWeapon(player.Client, (WeaponHash)Data.FirstParameter);
                Api.onPlayerDisconnected -= API_onPlayerDisconnected;
            }
            else if (Data.CurrentlyInUse != null && (!Data.CurrentlyInUse.Value && Data.SecondParameter > 0))
            {
                //API.givePlayerWeapon(Client player, WeaponHash weaponHash, int ammo, bool equipNow, bool ammoLoaded);
                if (Data.FirstParameter.HasValue)
                    Api.givePlayerWeapon(player.Client, (WeaponHash)Data.FirstParameter, Data.SecondParameter.Value, true, true);

                Data.CurrentlyInUse = true;
                Save();

                Api.onPlayerDisconnected += API_onPlayerDisconnected;
            }
        }

        private void API_onPlayerDisconnected(Client sender, string reason)
        {
            var player = sender.GetAccountController();

            if (Data.ItemType == (int)ItemType.Weapon)
            {
                if (Data.FirstParameter.HasValue)
                    Data.SecondParameter = Api.getPlayerWeaponAmmo(player.Client, (WeaponHash)Data.FirstParameter);
                Save();
            }
            Api.onPlayerDisconnected -= API_onPlayerDisconnected;

        }
    }

    internal class WeaponClip : Item
    {
        /// <summary>
        /// Pierwszy parametr to hash broni do której pasuje, a drugi to ilość amunicji
        /// </summary>
        /// <param name="item"></param>
        public WeaponClip(Database.Models.Item item) : base(item) { }

        public override void UseItem(AccountController player)
        {
        }

        //public override string UseInfo => $"Ten przedmiot dodaje {Data.SecondParameter} naboi do broni {Constant.ConstantItems.GunNames.Single(p => p.Key.Equals(Data.FirstParameter)).Value}";
    }

    internal class Mask : Item
    {
        /// <summary>
        /// Pierwszy parametr to liczba liczba użyć do zniszczenia
        /// </summary>
        /// <param name="item"></param>
        public Mask(Database.Models.Item item) : base(item) { }

        public override void UseItem(AccountController player)
        {
            if (!Data.CurrentlyInUse.HasValue) PrepareCurrentlyInUse();
            var encryptedName = $"Nieznajomy {player.Client.name.GetHashCode().ToString().Substring(1, 6)}";

            if (Data.CurrentlyInUse != null && Data.CurrentlyInUse.Value)
            {
                RPChat.SendMessageToNearbyPlayers(player.Client, "zdejmuje kominiarkę", ChatMessageType.Me);

                player.Client.name = player.CharacterController.FormatName;
                player.Client.resetNametag();

                Data.CurrentlyInUse = false;
                if (Data.FirstParameter.HasValue && Data.FirstParameter.Value == 0)
                {
                    Delete();
                    return;
                }
                Save();
            }
            else
            {
                RPChat.SendMessageToNearbyPlayers(player.Client, "zakłada kominiarkę", ChatMessageType.Me);
                player.Client.name = encryptedName;
                player.Client.resetNametag();

                Data.FirstParameter -= 1;
                Data.CurrentlyInUse = true;

                Save();
            }
        }

        public override string UseInfo => "Ten przedmiot ukrywa twoją nazwę wyświetlaną.";
    }

    internal class Drug : Item
    {
        public Drug(Database.Models.Item item) : base(item) { }
    }

    internal class Dice : Item
    {
        /// <summary>
        /// Pierwszy parametr to liczba oczek na kostce
        /// </summary>
        /// <param name="item"></param>
        public Dice(Database.Models.Item item) : base(item) { }

        public override void UseItem(AccountController player)
        {
            Random random = new Random();
            if (Data.FirstParameter != null)
                RPChat.SendMessageToNearbyPlayers(player.Client,
                    $"wyrzucił {random.Next(1, Data.FirstParameter.Value)} oczek z {Data.FirstParameter} możliwych",
            ChatMessageType.ServerMe);
        }

        public override string UseInfo => $"Ten przedmiot zwraca losową liczbę od 1 do {Data.FirstParameter}";
    }

    internal class Watch : Item
    {
        /// <summary>
        /// Brak parametrów
        /// </summary>
        /// <param name="item"></param>
        public Watch(Database.Models.Item item) : base(item) { }

        public override void UseItem(AccountController player)
        {
            RPChat.SendMessageToNearbyPlayers(player.Client, $"spogląda na zegarek {Data.Name}", ChatMessageType.Me);
            Api.sendNotificationToPlayer(player.Client, "Godzina: " + DateTime.Now.ToShortTimeString());
        }

        public override string UseInfo => "Ten przedmiot pokazuje bieżącą godzinę.";
    }

    internal class Cloth : Item
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public Cloth(Database.Models.Item item) : base(item) { }

        public override void UseItem(AccountController player)
        {
            if (!Data.CurrentlyInUse.HasValue) PrepareCurrentlyInUse();

        }

        public override string UseInfo => "Ten przedmiot zmienia ubranie twojej postaci.";
    }

    internal class Transmitter : Item
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public Transmitter(Database.Models.Item item) : base(item) { }

        public override void UseItem(AccountController player)
        {
            if (!Data.CurrentlyInUse.HasValue) PrepareCurrentlyInUse();
        }
        
        public override string UseInfo => $"Przedmiot służy do komunikacji na podanej częstotliwości w zasięgu: {Data.SecondParameter}";
    }

    internal class Cellphone : Item
    {
        /// <summary>
        /// 1 parametr to liczba kontaktów do zapisania, 2 to liczba sms możliwych do zapisania, 3 to numer telefonu
        /// </summary>
        /// <param name="item"></param>
        public Cellphone(Database.Models.Item item) : base(item) { }

        public override void UseItem(AccountController player)
        {
            if (!Data.CurrentlyInUse.HasValue) PrepareCurrentlyInUse();
            if (Data.CurrentlyInUse != null && !Data.CurrentlyInUse.Value)
            {
                if (!Data.ThirdParameter.HasValue) APIExtensions.ConsoleOutput("[Error] Do numeru został przypisany null.", ConsoleColor.Red);
                else
                {
                    RPChat.SendMessageToNearbyPlayers(player.Client, $"włącza {Data.Name}", ChatMessageType.ServerMe);
                    player.Client.SetSyncedData("CellphoneID", (int)Data.Id);
                    player.Client.Notify($"Telefon {Data.Name} został włączony naciśnij klawisz END, aby go używać.");
                    Data.CurrentlyInUse = true; 
                    player.CharacterController.Save();

                    player.CharacterController.CellphoneController = new CellphoneController(Data);
                }
            }
            else
            {
                RPChat.SendMessageToNearbyPlayers(player.Client, $"wyłącza {Data.Name}", ChatMessageType.ServerMe);

                player.CharacterController.Save();
                player.CharacterController.CellphoneController = null;
                Data.CurrentlyInUse = false;
                player.Client.Notify($"Telefon {Data.Name} został wyłączony.");

            }
        }

        public override string UseInfo => $"Telefon: {Data.Name} może przechowywać: {Data.FirstParameter} kontaktów, oraz {Data.SecondParameter} wiadomości SMS.";
    }

    internal class Tuning : Item
    {
        /// <summary>
        /// 1 to typ tuningu, pozostałe parametry zależą od typu tuningu
        /// dla 1 (speed) 2 parametr engineMultipilier, 3 torque
        /// dla 2 (brakes) 2 parametr to moc hamowania
        /// </summary>
        /// <param name="item"></param>
        public Tuning(Database.Models.Item item) : base(item) { }

        public override string UseInfo
        {
            get
            {
                if (Data.FirstParameter.HasValue && (TuningType)Data.FirstParameter == TuningType.Speed)
                {
                    return $"Tuning: {Data.Name} zwiększa prędkość maksymalną o: {Data.SecondParameter} procent, oraz zwiększa moment obrotowy o: {Data.ThirdParameter} procent.";
                }
                return $"Tuning: {Data.Name} zwiększa moc hamulcy o: {Data.SecondParameter} procent.";
            }
        }
    }
}