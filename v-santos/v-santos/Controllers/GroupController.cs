using System;
using System.Collections.Generic;
using System.Data.Entity;
using Serverside.Database;
using Serverside.Groups;

using Serverside.Core;
using Serverside.Database.Models;
using System.Linq;
using GrandTheftMultiplayer.Server.Constant;
using Serverside.Core.Extensions;
using Serverside.Groups.Base;

namespace Serverside.Controllers
{
    public abstract class GroupController
    {
        public Group GroupData { get; set; }

        public long Id => GroupData.Id;

        public List<AccountController> PlayersOnDuty { get; } = new List<AccountController>();

        protected GroupController(Group groupData)
        {
            this.GroupData = groupData;
            RPEntityManager.Add(this);
        }

        /// <summary>
        /// Dodawanie nowej grupy
        /// </summary>
        /// <param name="name">Nazwa grupy</param>
        /// <param name="tag">Tag grupy</param>
        /// <param name="type">Typ grupy</param>
        /// <param name="color">Kolor grupy</param>
        protected GroupController(string name, string tag, GroupType type, Color color)
        {
            GroupData = new Group {Workers = new List<Worker>()};
            this.GroupData.Name = name;
            this.GroupData.Tag = tag;
            this.GroupData.GroupType = type;
            this.GroupData.Color = color.ToHex();
            ContextFactory.Instance.Groups.Add(GroupData);
            ContextFactory.Instance.SaveChanges();
            RPEntityManager.Add(this);
        }

        public string GetColoredName() => $"{GroupData.Color.ToColor().ToRocstar()} ~h~ {GroupData.Name} ~w~";

        public bool HasMoney(decimal money) => GroupData.Money >= money;

        public void AddMoney(decimal money)
        {
            GroupData.Money += money;
            Save();
        }

        public void RemoveMoney(decimal money)
        {
            GroupData.Money -= money;
            Save();
        }

        public void AddWorker(AccountController account)
        {
            GroupData.Workers.Add(new Worker
            {
                Group = GroupData,
                Character = account.CharacterController.Character,
                DutyMinutes = 0,
                ChatRight = false,
                DoorsRight = false,
                OfferFromWarehouseRight = false,
                PaycheckRight = false,
                RecrutationRight = false,
                Salary = 0
            });
            Save();
        }

        public void RemoveWorker(AccountController account)
        { 
            GroupData.Workers.Remove(GroupData.Workers.Single(w => w.Character.Id == account.CharacterController.Character.Id));
            Save();
        }

        public List<Worker> GetWorkers()
        {
            return GroupData.Workers.Where(w => w.Character != null).ToList();
        }

        public bool CanPlayerOffer(AccountController account)
        {
            return GroupData.Workers.Single(w => w.Character == account.CharacterController.Character).OfferFromWarehouseRight;
        }

        /// <summary>
        /// Czy gracz może zapraszać i wypraszać ludzi z grupy
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool CanPlayerManageWorkers(AccountController account)
        {
            return GroupData.Workers.Single(w => w.Character == account.CharacterController.Character).RecrutationRight;
        }

        public bool CanPlayerTakeMoney(AccountController account)
        {
            return GroupData.Workers.Single(w => w.Character == account.CharacterController.Character).PaycheckRight;
        }

        public bool CanPlayerWriteOnChat(AccountController account)
        {
            return GroupData.Workers.Single(w => w.Character == account.CharacterController.Character).ChatRight;
        }

        public bool ContainsWorker(AccountController account)
        {
            return GroupData.Workers.Any(w => w.Character.Id == account.CharacterController.Character.Id);
        }

        public void Save()
        {
            ContextFactory.Instance.Groups.Attach(GroupData);
            ContextFactory.Instance.Entry(GroupData).State = EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }

        public static void LoadGroups()
        {
            foreach (var group in ContextFactory.Instance.Groups)
            {
                CreateGroup(group);
            }
        }

        //Tworzenie grupy
        public static GroupController CreateGroup(string name, string tag, GroupType type, Color color)
        {
            switch (type)
            {
                case GroupType.Przestepcza: return new CrimeGroup(name, tag, type, color);
                case GroupType.Urzad: return new CityHall(name, tag, type, color);
                case GroupType.Policja: return new Police(name, tag, type, color);
                default:
                    throw new NotSupportedException($"Nie rozpoznano typu grupy: {type}.");
            }
        }

        //Ładowanie grupy
        public static GroupController CreateGroup(Group editor)
        {
            var groupType = editor.GroupType;
            switch (groupType)
            {
                case GroupType.Przestepcza: return new CrimeGroup(editor);
                case GroupType.Urzad: return new CityHall(editor);
                case GroupType.Policja: return new Police(editor);
                default:
                    throw new NotSupportedException($"Nie rozpoznano typu grupy: {groupType}.");
            }
        }

    }
}