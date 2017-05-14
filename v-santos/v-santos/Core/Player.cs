using System;
using System.Collections.Generic;
using Serverside.Core.Finders;
using Serverside.Core.Money;
using Serverside.Database;

namespace Serverside.Core
{
    public class DimensionEventArgs : EventArgs
    {
        public int LastDimension { get; set; }
        public int CurrentDimension { get; set; }

        public DimensionEventArgs(int lastDimension, int currentDimension)
        {
            LastDimension = lastDimension;
            CurrentDimension = currentDimension;
        }
    }

    public class Player : IDisposable
    {
        public delegate void DimensionChangeHandler(DimensionEventArgs args);

        public event DimensionChangeHandler OnPlayerDimensionChanged;

        public Player(long aid)
        {
            Aid = aid;
            Helper = new MySqlDatabaseHelper();
            PlayerMoneyManager = new MoneyManager();
            OnPlayerDimensionChanged += Player_OnPlayerDimensionChanged;
        }

        private void Player_OnPlayerDimensionChanged(DimensionEventArgs args)
        {
            CharacterEditor editor = this.Editor;
            editor.CurrentDimension = args.CurrentDimension;
            Helper.UpdateCharacter(editor);
        }

        ~Player()
        {
            Dispose(false);
        }

        //Id konta
        public long Aid { get; }      
        public MySqlDatabaseHelper Helper;

        public Client Client => PlayerFinder.FindClientByAid(Aid);

        public int Id => (int) GetData("ServerId");

        public long CellphoneId => (long) GetSyncedData("CellphoneID");

        public int CellphoneNumber => (int) GetData("CellphoneNumber");

        public List<TelephoneContactList> CellphoneContacts => Helper.SelectContactsList(CellphoneId);

        public List<TelephoneMessageList> CellphoneMessages => Helper.SelectMessagesList(CellphoneId);

        //Id postaci
        public long Cid
        {
            get { return Client.getData("CharacterID"); }
        }

        public CharacterEditor Editor => Helper.SelectCharacter(Cid);

        public List<ItemList> Items => Helper.SelectItemsList(Cid, 1);

        public string Nickname => Editor.Name + " " + Editor.Surname;

        public List<long?> Groups
        {
            get
            {
                CharacterEditor editor = Editor;
                return new List<long?>
                {
                    editor.FirstGID,
                    editor.SecondGID,
                    editor.ThirdGID
                };
            }
        }

        #region Metody danych
        public dynamic GetData(string key)
        {
            return API.shared.getEntityData(Client, key);
        }

        /// <summary>
        /// Należy pamiętać, że twórcy GTA Network nie dali typu long do SyncedData i nie działa
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public dynamic GetSyncedData(string key)
        {
            return API.shared.getEntitySyncedData(Client, key);
        }

        public void SetData(string key, object value)
        {
            API.shared.setEntityData(Client, key, value);
        }

        /// <summary>
        /// Należy pamiętać, że twórcy GTA Network nie dali typu long do SyncedData i nie działa
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetSyncedData(string key, object value)
        {
            API.shared.setEntitySyncedData(Client, key, value);
        }

        public bool HasData(string key)
        {
            return API.shared.hasEntityData(Client, key);
        }

        public bool HasSyncedData(string key)
        {
            return API.shared.hasEntitySyncedData(Client, key);
        }

        public void ResetData(string key)
        {
            API.shared.resetEntityData(Client, key);
        }

        public void ResetSyncedData(string key)
        {
            API.shared.resetEntitySyncedData(Client.handle, key);
        }

        public bool TryGetData(string key, out dynamic data)
        {
            if (HasData(key)) 
            {
                data = GetData(key);
                return true;
            }
            data = null;
            return false;
        }

        public bool TryGetSyncedData(string key, out dynamic data)
        {
            if (HasSyncedData(key))
            {
                data = GetSyncedData(key);
                return true;
            }
            data = null;
            return false;
        }
        #endregion

        private MoneyManager PlayerMoneyManager { get; set; }

        public bool HasMoney(decimal count, bool bank = false)
        {
            return PlayerMoneyManager.CanPay(Cid, count, bank);
        }

        /// <summary>
        /// Jeśli pole true, to wtedy wpłacamy do banku, jak pole false to do kieszeni.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="bank"></param>
        public void AddMoney(decimal count, bool bank = false)
        {
            PlayerMoneyManager.AddMoney(Cid, count, bank);
        }

        public void RemoveMoney(decimal count, bool bank = false)
        {
            PlayerMoneyManager.RemoveMoney(Cid, count, bank);
        }

        private int dimension;

        public int Dimension
        {
            get { return dimension; }
            set
            {
                dimension = value;
                ChangeDimension(value);
            }
        }

        private void ChangeDimension(int dimension)
        {
            DimensionEventArgs args = new DimensionEventArgs(API.shared.getEntityDimension(Client), dimension);
            if (OnPlayerDimensionChanged != null) OnPlayerDimensionChanged.Invoke(args);
            API.shared.setEntityDimension(Client, dimension);
        }

        public void Notify(string info, bool flashing = false)
        {
            //Wersja wczesno rozwojowa
            //string[] infoArray;
            //infoArray = info.Split(' ');

            //List<String> infoList = new List<string>();
            //string toAdd = "";
            //foreach (var s in infoArray)
            //{
            //    if (toAdd.Length > 25)
            //    {
            //        infoList.Add(toAdd);
            //        toAdd = "";
            //    }
            //    else
            //    {
            //        toAdd += s;
            //    }
            //}

            //foreach (var item in infoList)
            //{
            //    //Dodajemy wielokropek do wyświetlania każdego oprócz ostatniego powiadomienia
            //    API.shared.sendNotificationToPlayer(this.Client, item.GetEnumerator().Current != infoList.Count ? item + "..." : item, flashing);
            //}

            API.shared.sendNotificationToPlayer(this.Client, info, flashing);
        }

        public bool TryFindGroupBySlot(int slot, out long? gid)
        {
            CharacterEditor editor = Editor;

            if (slot == 1 && editor.FirstGID != null)
            {
                gid = editor.FirstGID;
                return true;
            }
            if (slot == 2 && editor.SecondGID != null)
            {
                gid = editor.SecondGID;
                return true;
            }
            if (slot == 3 && editor.SecondGID != null)
            {
                gid = editor.ThirdGID;
                return true;
            }
            gid = null;
            return false;
        }

        public bool TryAddGroupOnNextSlot(long gid)
        {
            CharacterEditor editor = Editor;
            foreach (var g in Groups)
            {
                if (g == null)
                {
                    if (Groups.IndexOf(g) == 0)
                    {
                        editor.FirstGID = gid;
                    }
                    else if (Groups.IndexOf(g) == 1)
                    {
                        editor.SecondGID = gid;
                    }
                    editor.ThirdGID = gid;
                    Helper.UpdateCharacter(editor);
                    return true;
                }
            }
            return false;
        }

        public bool TryFindOnDutyGroupId(out long? gid)
        {
            if (!HasData("OnDutyGroupID"))
            {
                gid = null;
                return false;
            }
            gid = Convert.ToInt64(GetData("OnDutyGroupID"));
            return true;
        }

        private void ReleaseUnmanagedResources()
        {
            Helper = null;
            PlayerMoneyManager = null;
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            //if (disposing)
            //{
            //    Description?.Dispose();
            //}
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}