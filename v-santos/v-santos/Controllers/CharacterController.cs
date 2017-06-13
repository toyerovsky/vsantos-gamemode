using System;
using System.Data.Entity;
using System.Linq;
using GTANetworkServer;
using Serverside.Database;
using Serverside.Database.Models;
using Serverside.Core.Extensions;
using Serverside.Core;
using GTANetworkShared;

namespace Serverside.Controllers
{
    public class CharacterController
    {
        public static event CharacterLoginEventHandler OnPlayerCharacterLogin;
        public Character Character { get; set; }
        public AccountController AccountController { get; private set; }
        //Cellphone controller przypisujemy w przedmiotcie telefonu
        public CellphoneController CellphoneController { get; set; }
        public string FormatName => Character.Name + " " + Character.Surname;
        public long? OnDutyGroupId { get; set; }
        public Core.Description.Description Description { get; set; }
        public CharacterCreator.CharacterCreator CharacterCreator { get; set; }
        public BuildingController CurrentBuilding { get; set; }

        public event DimensionChangeEventHandler OnPlayerDimensionChanged;

        //Pola determinujące co gracz może robić w danym momencie
        //Np żeby kiedy jest nieprzytomny nie mógł mówić
        public bool CanPM { get; set; }
        public bool CanCommand { get; set; }
        public bool CanTalk { get; set; }
        public bool CanNarrate { get; set; }

        //ŁADUJE POSTAC
        public CharacterController(AccountController accountController, Character character)
        {
            Character = character;
            AccountController = accountController;
            AccountController.CharacterController = this;
            Character.Account = accountController.AccountData;
            Character.LastLoginTime = DateTime.Now;
            Character.Online = true;
            ContextFactory.Instance.SaveChanges();

            if (Character.Freemode) CharacterCreator = new CharacterCreator.CharacterCreator(this);
            Description = new Core.Description.Description(AccountController);
        }

        //GENERUJE, ŁADUJE I ZAPISUJE DO DB NOWA POSTAC
        public CharacterController(AccountController accountController, string name, string surname, PedHash model)
        {
            Character = new Character();
            accountController.CharacterController = this;
            Character.Account = accountController.AccountData;
            Character.Name = name;
            Character.Surname = surname;
            Character.Money = 10000;
            Character.BankMoney = 1000000;
            Character.CreateTime = DateTime.Now;
            Character.Model = (int)model;
            Character.ModelName = model.ToString();
            Character.HitPoints = 100;
            Character.IsAlive = true;
            Character.LastPositionX = 18.7854f;
            Character.LastPositionY = -736.714f;
            Character.LastPositionZ = 44.2173f;
            Character.LastPositionRotZ = -155.618f;
            ContextFactory.Instance.Characters.Add(Character);
            ContextFactory.Instance.SaveChanges();

            if (Character.Freemode) CharacterCreator = new CharacterCreator.CharacterCreator(this);
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
            if (AccountController != null)
            {
                Character.CurrentDimension = AccountController.Client.dimension;
                Character.LastPositionX = AccountController.Client.position.X;
                Character.LastPositionY = AccountController.Client.position.Y;
                Character.LastPositionZ = AccountController.Client.position.Z;
                Character.LastPositionRotZ = AccountController.Client.rotation.Z;
                Character.Model = AccountController.Client.model.GetHashCode();
            }
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

        public static void SelectCharacter(Client player, int selectId)
        {
            AccountController account = player.GetAccountController();
            if (account == null)
            {
                player.triggerEvent("ShowNotification", "Nie udało się załadować twojej postaci... Skontaktuj się z Administratorem!", 5000);
                return;
            }

            if (account.AccountData.Characters.Count() == 0)
            {
                //API.shared.sendChatMessageToPlayer(player, "You have no characters.");
                player.triggerEvent("ShowNotification", "Twoje konto nie posiada żadnych postaci!", 3000);
            }
            else
            {
                long characterid = account.AccountData.Characters.ToList()[selectId].Id;
                Character characterData = ContextFactory.Instance.Characters.FirstOrDefault(x => x.Id == characterid);
                CharacterController characterController = new CharacterController(account, characterData);
                characterController.LoginCharacter(account);
            }
        }

        public void LoginCharacter(AccountController accountController)
        {
            AccountController = accountController;
            accountController.CharacterController = this;

            Character character = accountController.CharacterController.Character;

            accountController.Client.nametag = "(" + RPEntityManager.CalculateServerId(accountController) + ") " + character.Name + " " + character.Surname;

            API.shared.setPlayerName(accountController.Client, character.Name + " " + character.Surname);
            accountController.Client.setSkin((PedHash)character.Model);

            API.shared.setEntityPosition(accountController.Client, new Vector3(character.LastPositionX, character.LastPositionY, character.LastPositionZ));

            accountController.Client.dimension = 0;

            if (character.BWState > 0)
            {
                API.shared.setPlayerHealth(accountController.Client, -1);
            }
            else
            {
                API.shared.setPlayerHealth(accountController.Client, character.HitPoints);
            }

            API.shared.triggerClientEvent(accountController.Client, "ShowCharacterSelectCef", false);

            accountController.Client.SetData("CanTalk", true);
            accountController.Client.SetData("CanNarrate", true);
            accountController.Client.SetData("CanPM", true);
            accountController.Client.SetData("CanCommand", true);
            accountController.Client.SetData("CanPay", true);

            API.shared.triggerClientEvent(accountController.Client, "Money_Changed", $"${character.Money}");
            API.shared.triggerClientEvent(accountController.Client, "ToggleHud", true);
            RPChat.SendMessageToPlayer(accountController.Client, $"Witaj, twoja postać {character.Name + " " + character.Surname} została pomyślnie załadowana, życzymy miłej gry!", ChatMessageType.ServerInfo);
            if (OnPlayerCharacterLogin != null) OnPlayerCharacterLogin.Invoke(AccountController.Client, this);
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