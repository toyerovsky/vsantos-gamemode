using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GTANetworkServer;
using Serverside.Core;
using Serverside.Database;
using Serverside.Database.Models;
using Serverside.Extensions;

namespace Serverside.Controllers
{
    public class AccountController
    {
        public long AccountId;
        public Account Account;
        public Client Client { get; private set; }
        public CharacterController CharacterController;
        public int ServerId => RPCore.CalculateServerId(this);

        public AccountController(Account accountdata, Client client)
        {
            Account = accountdata;
            Client = client;

            client.setData("RP_ACCOUNT", this);
            API.shared.sendNotificationToPlayer(client, $"~w~Witaj zalogowałeś się na konto {Account.Email}.");
            API.shared.sendNotificationToPlayer(client, $"~w~Ostatnie logowanie: {Account.LastLogin}");
            API.shared.sendNotificationToPlayer(client, $"~w~Z adresu IP: {Account.Ip}");

            Account.LastLogin = DateTime.Now;
            Account.Online = true;

            RPCore.AddAccount(AccountId, this);
            //tutaj dajemy inne rzeczy które mają być inicjowane po zalogowaniu się na konto, np: wybór postaci.


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
            Account accountData = ContextFactory.Instance.Accounts.FirstOrDefault(x => x.UserId == userId);
            new AccountController(accountData, sender);
        }

        public static bool RegisterAccount(Client sender, long userid)
        {
            if (!DoesAccountExist(userid))
            {
                Account account = new Account
                {
                    UserId = userid,
                    SocialClub = sender.name,
                    Ip = sender.address
                };

                ContextFactory.Instance.Accounts.Add(account);
                ContextFactory.Instance.SaveChanges();

                new AccountController(account, sender);
                return true;
            }
            return false;
        }

        public static List<Character> GetCharacters(AccountController account)
        {
            return account.Account.Character.ToList();
        }

        public static void LoadCharacter(AccountController account, Character character)
        {
            new CharacterController(account, character);
        }

        public static bool HasCharacterSlot(AccountController account)
        {
            if (account.Account.Character?.Count() > 3)
            {
                API.shared.sendChatMessageToPlayer(account.Client, "~r~Błąd: ~w~Osiągnąłeś maksymalny limit postaci na konto!");
                return false;
            }
            return true;
        }

        public void Save()
        {
            //tutaj wywołać metody synchronizacji danych z innych controllerów np character.
            CharacterController.Save();
            ContextFactory.Instance.Accounts.Attach(Account);
            ContextFactory.Instance.Entry(Account).State = EntityState.Modified;
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