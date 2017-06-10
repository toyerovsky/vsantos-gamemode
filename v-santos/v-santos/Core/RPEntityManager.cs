using GTANetworkShared;
using Serverside.Controllers;
using Serverside.Database;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;

namespace Serverside.Core
{
    public static class RPEntityManager
    {
        private static readonly SortedList<long, AccountController> Accounts = new SortedList<long, AccountController>();
        private static readonly List<VehicleController> Vehicles = new List<VehicleController>();
        private static readonly List<GroupController> Groups = new List<GroupController>();

        public static void Init()
        {
            GroupController.LoadGroups(); //Ładuje grupy z bazy danych do zmiennej Groups
        }

        #region ACCOUNT METHODS

        public static void AddAccount(long accountId, AccountController accountController)
        {
            Accounts.Add(accountId, accountController);
        }

        public static void RemoveAccount(long accountId)
        {
            Accounts.Remove(accountId);
        }

        public static AccountController GetAccount(long id)
        {
            if (id > -1) return Accounts.Get(id);
            return null;
        }

        public static AccountController GetAccount(int id)
        {
            if (id > -1) return Accounts.Values.ElementAtOrDefault(id);
            return null;
        }

        public static AccountController GetAccountByServerId(int id)
        {
            if (id > -1) return Accounts.Values.First(x => x.ServerId == id);
            return null;
        }

        public static SortedList<long, AccountController> GetAccounts()
        {
            return Accounts;
        }

        public static int CalculateServerId(AccountController account)
        {
            return Accounts.IndexOfValue(account);
        }

        public static void Save(bool resourceStop = false)
        {
            foreach (var account in Accounts)
            {
                account.Value.Save(resourceStop);
            }
            ContextFactory.Instance.SaveChanges();
        }

        #endregion

        #region VEHICLE METHODS
        public static void Add(VehicleController vc)
        {
            Vehicles.Add(vc);
        }

        public static void Remove(VehicleController vc)
        {
            Vehicles.Remove(vc);
        }

        public static VehicleController GetVehicle(Vehicle vehicle)
        {
            return Vehicles.Find(x => x.Vehicle == vehicle);
        }

        public static VehicleController GetVehicle(NetHandle vehicle)
        {
            return Vehicles.Find(x => x.Vehicle.handle == vehicle);
        }

        public static VehicleController GetVehicle(long id)
        {
            return Vehicles.Find(x => x.VehicleData.Id == id);
        }

        public static List<VehicleController> GetCharacterVehicles(CharacterController cc)
        {
            if (cc == null) return new List<VehicleController>();
            return Vehicles.Where(x => x.VehicleData.Character == cc.Character).ToList();
        }
        #endregion

        #region GROUP METHODS
        public static void Add(GroupController vc)
        {
            Groups.Add(vc);
        }

        public static void Remove(GroupController vc)
        {
            Groups.Remove(vc);
        }

        public static GroupController GetGroup(long id)
        {
            return Groups.Find(x => x.GroupId == id);
        }

        public static GroupController GetGroup(string groupName)
        {
            return Groups.Find(x => x.Data.Name.StartsWith(groupName.ToLower()));
        }

        public static List<GroupController> GetPlayerGroups(AccountController controller)
        {
            return Groups.Where(
                g => g.Data.Workers.Any(x => x.Character.Id == controller.CharacterController.Character.Id)).ToList();
        }

        public static List<GroupController> GetGroups()
        {
            return Groups;
        }
        #endregion
    }
}
