//using System;
//using System.Collections.Generic;
//using Serverside.Core.Money;
//using Serverside.Database;
//using Serverside.DatabaseEF6;
//using Serverside.DatabaseEF6.Models;

//namespace Serverside.Core
//{
//    public class DimensionEventArgs : EventArgs
//    {
//        public int LastDimension { get; set; }
//        public int CurrentDimension { get; set; }

//        public DimensionEventArgs(int lastDimension, int currentDimension)
//        {
//            LastDimension = lastDimension;
//            CurrentDimension = currentDimension;
//        }
//    }

//    public class Player : IDisposable
//    {
//        public delegate void DimensionChangeHandler(DimensionEventArgs args);

//        public event DimensionChangeHandler OnPlayerDimensionChanged;

//        public Player(long aid)
//        {
//            Aid = aid;
//            PlayerMoneyManager = new MoneyManager();
//            OnPlayerDimensionChanged += Player_OnPlayerDimensionChanged;
//        }

//        private void Player_OnPlayerDimensionChanged(DimensionEventArgs args)
//        {
//            Character editor = this.Editor;
//            editor.CurrentDimension = args.CurrentDimension;
//            CharacterDatabaseHelper.UpdateCharacter(editor);
//        }

//        ~Player()
//        {
//            Dispose(false);
//        }

//        //Id konta
//        public long Aid { get; }

//        public Client Client => PlayerFinder.FindClientByAid(Aid);

//        public int Id => (int)GetData("ServerId");

//        public long CellphoneId => (long)GetSyncedData("CellphoneID");

//        public int CellphoneNumber => (int)GetData("CellphoneNumber");

//        public List<TelephoneContact> CellphoneContacts => TelephoneContactDatabaseHelper.SelectContactsList(CellphoneId);

//        public List<TelephoneMessage> CellphoneMessages => TelephoneMessageDatabaseHelper.SelectMessagesList(CellphoneId);

//        //Id postaci
//        public long Cid
//        {
//            get { return Client.getData("CharacterID"); }
//        }

//        public Character Editor => CharacterDatabaseHelper.SelectCharacter(Cid);

//        public List<Item> Items => ItemDatabaseHelper.SelectItemsList(Editor);

//        public string Nickname => Editor.Name + " " + Editor.Surname;

//        public List<Group> Groups { get { return Editor.Group; } }

//        //public List<long?> Groups
//        //{
//        //    get
//        //    {
//        //        Character editor = Editor;
//        //        return new List<long?>
//        //        {
//        //            editor.FirstGID,
//        //            editor.SecondGID,
//        //            editor.ThirdGID
//        //        };
//        //    }
//        //}

//        #region Metody danych
//        public dynamic GetData(string key)
//        {
//            return API.shared.getEntityData(Client, key);
//        }

//        /// <summary>
//        /// Należy pamiętać, że twórcy GTA Network nie dali typu long do SyncedData i nie działa
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        public dynamic GetSyncedData(string key)
//        {
//            return API.shared.getEntitySyncedData(Client, key);
//        }

//        public void SetData(string key, object value)
//        {
//            API.shared.setEntityData(Client, key, value);
//        }

//        /// <summary>
//        /// Należy pamiętać, że twórcy GTA Network nie dali typu long do SyncedData i nie działa
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="value"></param>
//        public void SetSyncedData(string key, object value)
//        {
//            API.shared.setEntitySyncedData(Client, key, value);
//        }

//        public bool HasData(string key)
//        {
//            return API.shared.hasEntityData(Client, key);
//        }

//        public bool HasSyncedData(string key)
//        {
//            return API.shared.hasEntitySyncedData(Client, key);
//        }

//        public void ResetData(string key)
//        {
//            API.shared.resetEntityData(Client, key);
//        }

//        public void ResetSyncedData(string key)
//        {
//            API.shared.resetEntitySyncedData(Client.handle, key);
//        }

//        public bool TryGetData(string key, out dynamic data)
//        {
//            if (HasData(key))
//            {
//                data = GetData(key);
//                return true;
//            }
//            data = null;
//            return false;
//        }

//        public bool TryGetSyncedData(string key, out dynamic data)
//        {
//            if (HasSyncedData(key))
//            {
//                data = GetSyncedData(key);
//                return true;
//            }
//            data = null;
//            return false;
//        }
//        #endregion

//        private MoneyManager PlayerMoneyManager { get; set; }

//        public bool HasMoney(decimal count, bool bank = false)
//        {
//            return PlayerMoneyManager.CanPay(Cid, count, bank);
//        }

//        /// <summary>
//        /// Jeśli pole true, to wtedy wpłacamy do banku, jak pole false to do kieszeni.
//        /// </summary>
//        /// <param name="count"></param>
//        /// <param name="bank"></param>
//        public void AddMoney(decimal count, bool bank = false)
//        {
//            PlayerMoneyManager.AddMoney(Cid, count, bank);
//        }

//        public void RemoveMoney(decimal count, bool bank = false)
//        {
//            PlayerMoneyManager.RemoveMoney(Cid, count, bank);
//        }

//        private int dimension;

//        public int Dimension
//        {
//            get { return dimension; }
//            set
//            {
//                dimension = value;
//                ChangeDimension(value);
//            }
//        }

//        private void ChangeDimension(int dimension)
//        {
//            DimensionEventArgs args = new DimensionEventArgs(API.shared.getEntityDimension(Client), dimension);
//            if (OnPlayerDimensionChanged != null) OnPlayerDimensionChanged.Invoke(args);
//            API.shared.setEntityDimension(Client, dimension);
//        }

//        public void Notify(string info, bool flashing = false)
//        {
//            //Wersja wczesno rozwojowa
//            //string[] infoArray;
//            //infoArray = info.Split(' ');

//            //List<String> infoList = new List<string>();
//            //string toAdd = "";
//            //foreach (var s in infoArray)
//            //{
//            //    if (toAdd.Length > 25)
//            //    {
//            //        infoList.Add(toAdd);
//            //        toAdd = "";
//            //    }
//            //    else
//            //    {
//            //        toAdd += s;
//            //    }
//            //}

//            //foreach (var item in infoList)
//            //{
//            //    //Dodajemy wielokropek do wyświetlania każdego oprócz ostatniego powiadomienia
//            //    API.shared.sendNotificationToPlayer(this.Client, item.GetEnumerator().Current != infoList.Count ? item + "..." : item, flashing);
//            //}

//            API.shared.sendNotificationToPlayer(this.Client, info, flashing);
//        }

//        public bool TryFindGroupBySlot(int slot, out Group group)
//        {
//            Character editor = Editor;
//            group = null;

//            if (Editor.Group.Count == 0) return false;

            

//            if(Editor.Group[slot] != null)
//            {
//                group = Editor.Group[slot];
//                return true;
//            }

//            return false;
//            //if (slot == 1 && editor.FirstGID != null)
//            //{
//            //    gid = editor.FirstGID;
//            //    return true;
//            //}
//            //if (slot == 2 && editor.SecondGID != null)
//            //{
//            //    gid = editor.SecondGID;
//            //    return true;
//            //}
//            //if (slot == 3 && editor.SecondGID != null)
//            //{
//            //    gid = editor.ThirdGID;
//            //    return true;
//            //}
//            //gid = null;
//            //return false;
//        }

//        //public bool TryAddGroupOnNextSlot(long gid)
//        //{
//        //    Character editor = Editor;
//        //    foreach (var g in Groups)
//        //    {
//        //        if (g == null)
//        //        {
//        //            if (Groups.IndexOf(g) == 0)
//        //            {
//        //                editor.FirstGID = gid;
//        //            }
//        //            else if (Groups.IndexOf(g) == 1)
//        //            {
//        //                editor.SecondGID = gid;
//        //            }
//        //            editor.ThirdGID = gid;
//        //            CharacterDatabaseHelper.UpdateCharacter(editor);
//        //            return true;
//        //        }
//        //    }
//        //    return false;
//        //}

//        public bool TryFindOnDutyGroupId(out long? gid)
//        {
//            if (!HasData("OnDutyGroupID"))
//            {
//                gid = null;
//                return false;
//            }
//            gid = Convert.ToInt64(GetData("OnDutyGroupID"));
//            return true;
//        }

//        private void ReleaseUnmanagedResources()
//        {
//            PlayerMoneyManager = null;
//        }

//        private void Dispose(bool disposing)
//        {
//            ReleaseUnmanagedResources();
//            //if (disposing)
//            //{
//            //    Description?.Dispose();
//            //}
//        }

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }
//    }
//}