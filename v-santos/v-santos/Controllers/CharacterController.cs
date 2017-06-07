﻿using System;
using System.Data.Entity;
using System.Linq;
using GTANetworkServer;
using Serverside.Database;
using Serverside.Database.Models;
using Serverside.Core.Extensions;
using Serverside.Core.Vehicles;
using Serverside.Core;

namespace Serverside.Controllers
{
    public class CharacterController
    {
        public Character Character = new Character();
        public AccountController AccountController { get; private set; }
        //Cellphone controller przypisujemy w przedmiotcie telefonu
        public CellphoneController CellphoneController { get; set; }
        public string FormatName => Character.Name + " " + Character.Surname;
        public long? OnDutyGroupId { get; set; }
        public Core.Description.Description Description { get; set; }

        public event DimensionChangeEventHandler OnPlayerDimensionChanged;

        //ŁADUJE POSTAC
        public CharacterController(AccountController accountController, Character character)
        {
            Character = character;
            AccountController = accountController;
            AccountController.CharacterController = this;
            Character.Account = accountController.Account;
            Character.LastLoginTime = DateTime.Now;
            Character.Online = true;
            ContextFactory.Instance.SaveChanges();

            Description = new Core.Description.Description(AccountController);
        }

        //GENERUJE, ŁADUJE I ZAPISUJE DO DB NOWA POSTAC
        public CharacterController(AccountController accountController, string name, string surname, PedHash model)
        {
            accountController.CharacterController = this;
            Character.Account = accountController.Account;
            Character.Name = name;
            Character.Surname = surname;
            Character.CreateTime = DateTime.Now;
            Character.Model = (int)model;//PedHash.DrFriedlander.GetHashCode(); //Global.GlobalVars._defaultPedModel.GetHashCode();
            Character.ModelName = model.ToString(); //"DrFriedlander";
            Character.HitPoints = 100;
            Character.IsAlive = true;
            ContextFactory.Instance.Characters.Add(Character);
            ContextFactory.Instance.SaveChanges();

            Description = new Core.Description.Description(accountController);
        }

        

        public void Save(bool resourceStop = false)
        {
            foreach (VehicleController v in RPEntityManager.GetCharacterVehicles(this))
            {
                if (v != null)
                {
                    if (resourceStop)
                        v.Dispose();
                    else
                        v.Save();
                }
            }

            CellphoneController?.Save();
            Character.CurrentDimension = AccountController.Client.dimension;
            Character.LastPositionX = AccountController.Client.position.X;
            Character.LastPositionY = AccountController.Client.position.Y;
            Character.LastPositionZ = AccountController.Client.position.Z;
            Character.LastPositionRotZ = AccountController.Client.rotation.Z;
            Character.Model = AccountController.Client.model.GetHashCode();
            ContextFactory.Instance.Characters.Attach(Character);
            ContextFactory.Instance.Entry(Character).State = EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }

        public static bool DoesCharacterExist(string name, string surname)
        {
            return ContextFactory.Instance.Characters.FirstOrDefault(x => x.Name == name && x.Surname == surname) != null;
        }

        //GENERUJE, ŁADUJE I ZAPISUJE DO DB NOWA POSTAC
        public static bool AddCharacter(AccountController account, string name, string surname, PedHash model)
        {
            if (!DoesCharacterExist(name, surname) && AccountController.HasCharacterSlot(account))
            {
                new CharacterController(account, name, surname, model);
                return true;
            }
            return false;
        }

        #region DimensionManager

        public void ChangeDimension(int dimension)
        {
            if (OnPlayerDimensionChanged != null) OnPlayerDimensionChanged.Invoke(this, new DimensionChangeEventArgs(AccountController.Client, API.shared.getEntityDimension(AccountController.Client), dimension));
            API.shared.setEntityDimension(AccountController.Client, dimension);
        }

        #endregion
    }
}