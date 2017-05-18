using GTANetworkServer;
using Serverside.Core.Extenstions;
using Serverside.DatabaseEF6;
using Serverside.DatabaseEF6.Models;
using System;
using System.Linq;

namespace Serverside.Core
{
    public class CharacterController
    {
        public Character Character = new Character();
        public AccountController AccountController { get; private set; }
        public string FormatName
        {
            get
            {
                return Character.Name + " " + Character.Surname;
            }
        }

        public event DimensionChangeEventHandler OnPlayerDimensionChanged;

        //ŁADUJE POSTAC
        public CharacterController(AccountController accountController, Character character)
        {
            Character = character;
            AccountController = accountController;
            Character.LastLoginTime = DateTime.Now;
            Character.Online = true;
            ContextFactory.Instance.SaveChanges();
        }

        //GENERUJE, ŁADUJE I ZAPISUJE DO DB NOWA POSTAC
        public CharacterController(AccountController accountController, string name, string surname, int model, string modelname)
        {
            accountController.CharacterController = this;
            Character.Name = name;
            Character.Surname = surname;
            Character.CreateAccountTime = DateTime.Now;
            Character.Model = model;//PedHash.DrFriedlander.GetHashCode(); //Global.GlobalVars._defaultPedModel.GetHashCode();
            Character.ModelName = modelname; //"DrFriedlander";
            ContextFactory.Instance.Characters.Add(Character);
            ContextFactory.Instance.SaveChanges();
        }

        public void Save()
        {
            Character.CurrentDimension = AccountController.Client.dimension;
            Character.LastPositionX = AccountController.Client.position.X;
            Character.LastPositionY = AccountController.Client.position.Y;
            Character.LastPositionZ = AccountController.Client.position.Z;
            Character.LastPositionRotZ = AccountController.Client.rotation.Z;
            Character.Model = AccountController.Client.model.GetHashCode();
            ContextFactory.Instance.Characters.Attach(Character);
            ContextFactory.Instance.Entry(Character).State = System.Data.Entity.EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }

        public static bool DoesCharacterExist(string name, string surname)
        {
            if (ContextFactory.Instance.Characters.Where(x => x.Name == name && x.Surname == surname).FirstOrDefault() == null) return false;
            return true;
        }

        //GENERUJE, ŁADUJE I ZAPISUJE DO DB NOWA POSTAC
        public static bool RegisterCharacter(AccountController account, string name, string surname, int model, string modelname)
        {
            if (!DoesCharacterExist(name, surname) && AccountController.HasCharacterSlot(account))
            {
                new CharacterController(account, name, surname, model, modelname);
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