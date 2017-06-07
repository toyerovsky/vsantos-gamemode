using System.Data.Entity;
using Serverside.Database;
using Serverside.Groups;
using GTANetworkServer.Constant;
using Serverside.Core;
using Serverside.Database.Models;
using System.Linq;

namespace Serverside.Controllers
{
    public abstract class GroupController
    {
        public Group Data { get; set; }

        public long GroupId { get; set; }

        public GroupController(Group data)
        {
            this.Data = data;
            RPEntityManager.Add(this);
        }

        /// <summary>
        /// Dodawanie nowej grupy
        /// </summary>
        /// <param name="name">Nazwa grupy</param>
        /// <param name="tag">Tag grupy</param>
        /// <param name="type">Typ grupy</param>
        /// <param name="color">Kolor grupy</param>
        public GroupController(string name, string tag, GroupType type, Color color)
        {
            this.Data.Name = name;
            this.Data.Tag = tag;
            this.Data.GroupType = type;
            this.Data.Color = color;
            ContextFactory.Instance.Groups.Add(Data);
            ContextFactory.Instance.SaveChanges();
            RPEntityManager.Add(this);
        }

        public static void LoadGroups()
        {
            //foreach (var group in ContextFactory.Instance.Groups)
            //{
            //    new GroupController(group);
            //}
            //API.shared.consoleOutput("[GM] Załadowano " + ContextFactory.Instance.Groups.Count() + " grup.");
        }

        public static string GetName(int groupId)
        {
            if (groupId > -1)
            {
                return RPEntityManager.GetGroups()[groupId].Data.Name;
            }
            return "Zła wartość GroupID";
        }

        public bool HasMoney(decimal money)
        {
            return Data.Money >= money;
        }

        public void AddMoney(decimal money)
        {
            Data.Money += money;
            Save();
        }

        public void RemoveMoney(decimal money)
        {
            Data.Money -= money;
            Save();
        }

        public void AddWorker(AccountController account)
        {
            Data.Workers.Add(new Worker
            {
                Group = Data,
                Character = account.CharacterController.Character
            });
            Save();
        }

        public bool CanPlayerOfferFromWarehouse(AccountController account)
        {
            return Data.Workers.Single(w => w.Character == account.CharacterController.Character).OfferFromWarehouseRight;
        }

        public bool CanPlayerTakeMoney(AccountController account)
        {
            return Data.Workers.Single(w => w.Character == account.CharacterController.Character).PaycheckRight;
        }

        public void Save()
        {
            ContextFactory.Instance.Groups.Attach(Data);
            ContextFactory.Instance.Entry(Data).State = EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }
    }
}