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
        private static readonly List<BuildingController> Buildings = new List<BuildingController>();

        public static void Init()
        {
            GroupController.LoadGroups(); //Ładuje grupy z bazy danych do zmiennej Groups
            BuildingController.LoadBuildings();
        }

        #region ACCOUNT METHODS

        public static void AddAccount(long accountId, AccountController accountController) => Accounts.Add(accountId, accountController);

        public static void RemoveAccount(long accountId) => Accounts.Remove(accountId);

        public static AccountController GetAccount(long accountId) => accountId > -1 ? Accounts.Get(accountId) : null;
        
        public static AccountController GetAccountByServerId(int id) => id > -1 ? Accounts.Values.ElementAtOrDefault(id) : null;

        public static AccountController GetAccountByCharacterId(long characterId)
        {
            return characterId > -1 ? Accounts.Values.Single(ch => ch.CharacterController.Character.Id == characterId) : null;
        }

        public static SortedList<long, AccountController> GetAccounts() => Accounts;

        public static int CalculateServerId(AccountController account) => Accounts.IndexOfValue(account);

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
        public static void Add(VehicleController vc) => Vehicles.Add(vc);
        
        public static void Remove(VehicleController vc) => Vehicles.Remove(vc);

        public static VehicleController GetVehicle(Vehicle vehicle) => Vehicles.Find(x => x.Vehicle == vehicle);

        public static VehicleController GetVehicle(NetHandle vehicle) => Vehicles.Find(x => x.Vehicle.handle == vehicle);

        public static VehicleController GetVehicle(long id) => Vehicles.Find(x => x.VehicleData.Id == id);

        public static List<VehicleController> GetCharacterVehicles(CharacterController cc)
        {
            return cc.Character == null ? new List<VehicleController>() : Vehicles.Where(x => x.VehicleData.Character == cc.Character).ToList();
        }
        #endregion

        #region GROUP METHODS
        public static void Add(GroupController group) => Groups.Add(group);

        public static void Remove(GroupController group) => Groups.Remove(group);

        public static GroupController GetGroup(long groupId) => Groups.Find(x => x.GroupId == groupId);

        public static GroupController GetGroup(string groupName) => Groups.Single(x => x.Data.Name.StartsWith(groupName.ToLower()));

        public static List<GroupController> GetPlayerGroups(AccountController controller)
        {
            return Groups.Where(
                g => g.Data.Workers.Any(x => x.Character.Id == controller.CharacterController.Character.Id)).ToList();
        }

        public static List<GroupController> GetGroups() => Groups;
        #endregion

        #region BUILDING METHODS
        public static void Add(BuildingController building) => Buildings.Add(building);

        public static void Remove(BuildingController building) => Buildings.Remove(building);

        public static BuildingController GetBuilding(long buildingId) => Buildings.Find(x => x.BuildingId == buildingId);

        public static BuildingController GetBuilding(string buildingName)
        {
            return Buildings.Single(x => x.BuildingData.Name.StartsWith(buildingName.ToLower()));
        }

        public static List<BuildingController> GetPlayerBuildings(AccountController controller)
        {
            return Buildings.Where(b => b.BuildingData.Character.Id == controller.CharacterController.Character.Id).ToList();
        }

        public static List<BuildingController> GetBuildings() => Buildings;

        #endregion
    }
}
