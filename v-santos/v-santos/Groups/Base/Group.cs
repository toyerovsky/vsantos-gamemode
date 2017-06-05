using System.Linq;
using Serverside.Controllers;
using Serverside.Database.Models;

namespace Serverside.Groups.Base
{
    public abstract class Group
    {
        public long Id => Controller.Data.Id;
        public Database.Models.Group Data => Controller.Data;
        public GroupController Controller { get; set; }

        private decimal Money
        {
            get => Controller.Data.Money;
            set => Controller.Data.Money = value;
        }

        protected Group(Database.Models.Group group)
        {
            Controller = new GroupController(group);
        }

        public bool HasMoney(decimal money)
        {
            return Money >= money;
        }

        public void AddMoney(decimal money)
        {
            this.Money += money;
            Controller.Save();
        }

        public void RemoveMoney(decimal money)
        {
            this.Money -= money;
            Controller.Save();
        }

        public void AddWorker(AccountController account)
        {
            Controller.Data.Workers.Add(new Worker
            {
                Group = Controller.Data,
                Character = account.CharacterController.Character
            });
            Controller.Save();
        }

        public bool CanPlayerOfferFromWarehouse(AccountController account)
        {
            return Controller.Data.Workers.Single(w => w.Character == account.CharacterController.Character).OfferFromWarehouseRight;
        }

        public bool CanPlayerTakeMoney(AccountController account)
        {
            return Controller.Data.Workers.Single(w => w.Character == account.CharacterController.Character).PaycheckRight;
        }
    }
}
