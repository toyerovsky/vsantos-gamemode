using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GTANetworkServer;
using Serverside.Core;
using Serverside.Database;
using Serverside.Database.Models;
using Serverside.Core.Extensions;
using Newtonsoft.Json;

namespace Serverside.Controllers
{
    public class AccountController
    {
        public long AccountId;
        public Account AccountData;
        public Client Client { get; private set; }
        public CharacterController CharacterController;
        public int ServerId => RPEntityManager.CalculateServerId(this);

        public AccountController(Account accountdata, Client client)
        {
            AccountData = accountdata;
            Client = client;

            client.setData("RP_ACCOUNT", this);

            AccountData.LastLogin = DateTime.Now;
            AccountData.Online = true;

            RPEntityManager.AddAccount(AccountId, this);
            //tutaj dajemy inne rzeczy które mają być inicjowane po zalogowaniu się na konto, np: wybór postaci.

            //TODO: tutaj ma się wyświetlać nick z forum a nie social club
            client.triggerEvent("ShowNotification", $"Witaj, {accountdata.SocialClub} zostałeś pomyślnie zalogowany. Wybierz postać.", 3000);

            //Nie działa zmieniam
            //if (Account.Character == null || Account.Character.Count == 0)
            //{
            //    if (!CharacterController.AddCharacter(this, Account.Email, "Testowy", PedHash.Michael))
            //    {
            //        RPChat.SendMessageToPlayer(client, "Postać o wybranym imieniu i nazwisku już istnieje.", ChatMessageType.ServerInfo);
            //        client.kick("Postać o wybranym imieniu i nazwisku już istnieje.");
            //        return;
            //    }
            //}

            if (AccountData.Character.Count == 0)
            {
                AccountData.Character.Add(new Character
                {
                    Account = AccountData,
                    Name = AccountData.Email,
                    Surname = "Testowy",
                    Model = (int)PedHash.Michael,
                });
            }

            var characters = AccountData.Character.ToList();

            string json = JsonConvert.SerializeObject(characters.Where(c => c.IsAlive && c.Account == AccountData).Select(
                ch => new
                {
                    ch.Id,
                    ch.Name,
                    ch.Surname,
                    ch.Money,
                    ch.BankMoney
                }).ToList());

            client.triggerEvent("ShowCharacterSelectMenu", json);


            ContextFactory.Instance.SaveChanges();
        }

        public static AccountController GetAccountControllerFromName(string formatname)
        {
            Client client = API.shared.getAllPlayers().FirstOrDefault(x => x.GetAccountController().CharacterController.FormatName.ToLower().Contains(formatname.ToLower()));
            if (client != null) return client.GetAccountController();
            return null;
        }

        public static void LoadAccount(Client sender, long userId)
        {
            new AccountController(ContextFactory.Instance.Accounts.FirstOrDefault(x => x.UserId == userId), sender);
        }

        public static bool RegisterAccount(Client sender, Account account)
        {
            if (!DoesAccountExist(account.Id))
            {
                ContextFactory.Instance.Accounts.Add(account);
                ContextFactory.Instance.SaveChanges();

                new AccountController(account, sender);
                return true;
            }
            return false;
        }

        public static List<Character> GetCharacters(AccountController account)
        {
            return account.AccountData.Character.ToList();
        }

        public static void LoadCharacter(AccountController account, Character character)
        {
            new CharacterController(account, character);
        }

        public static bool HasCharacterSlot(AccountController account)
        {
            if (account.AccountData.Character?.Count() > 3)
            {
                API.shared.sendChatMessageToPlayer(account.Client, "~r~Błąd: ~w~Osiągnąłeś maksymalny limit postaci na konto!");
                return false;
            }
            return true;
        }

        public void Save(bool resourceStop = false)
        {
            //tutaj wywołać metody synchronizacji danych z innych controllerów np character.
            if (CharacterController != null)
                CharacterController.Save(resourceStop);

            ContextFactory.Instance.Accounts.Attach(AccountData);
            ContextFactory.Instance.Entry(AccountData).State = EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }

        public static bool DoesAccountExist(long userid)
        {
            if (ContextFactory.Instance.Accounts.FirstOrDefault(x => x.UserId == userid) == null) return false;
            return true;
        }

        public static bool IsAccountBanned(Client player)
        {
            var ipban = ContextFactory.Instance.Bans.FirstOrDefault(ban => ban.Active && ban.Ip == player.address);
            if (ipban != null) return true;
            var socialclub = ContextFactory.Instance.Bans.FirstOrDefault(ban => ban.Active && ban.IsSocialClubBanned && ban.SocialClub == player.socialClubName);
            if (socialclub != null) return true;
            return false;
        }

        public static bool IsAccountBanned(Account account)
        {
            var accountban = ContextFactory.Instance.Bans.FirstOrDefault(ban => ban.Active && ban.AccountId == account.Id);
            if (accountban != null) return true;
            return false;
        }
    }
}