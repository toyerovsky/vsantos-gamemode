using GTANetworkServer;
using Serverside.Core.Extenstions;
using Serverside.DatabaseEF6;
using Serverside.DatabaseEF6.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Serverside.Core
{
    public class AccountController
    {
        public long AccountId;
        public Account Account;
        public Client Client { get; private set; }
        public CharacterController CharacterController;
        public int ServerId
        {
            get
            {
                return RPCore.CalculateServerId(this);
            }
        }
        public AccountController(Account accountdata, Client client)
        {
            Account = accountdata;
            Client = client;

            client.setData("RP_ACCOUNT", this);
            API.shared.sendNotificationToPlayer(client, string.Format("~w~Witaj zalogowałeś się na konto {0}.", Account.Email));
            API.shared.sendNotificationToPlayer(client, string.Format("~w~Ostatnie logowanie: {0}", Account.LastLogin.ToString()));
            API.shared.sendNotificationToPlayer(client, string.Format("~w~Z adresu IP: {0}", Account.Ip));

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

        public static void LoadAccount(Client sender, long UserId)
        {
            Account AccountData = ContextFactory.Instance.Accounts.Where(x => x.Id == UserId).FirstOrDefault();
            new AccountController(AccountData, sender);
        }

        public static bool RegisterAccount(Client sender, long userid, string email)
        {
            if (!DoesAccountExist(userid))
            {
                Account account = new Account();
                account.UserId = userid;
                account.Email = email;
                account.SocialClub = sender.name;
                account.Ip = sender.address;

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
            if (account.Account.Character == null) return true;
            if (account.Account.Character.Count() > 3)
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
            ContextFactory.Instance.Entry(Account).State = System.Data.Entity.EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }

        public static bool DoesAccountExist(long userid)
        {
            if (ContextFactory.Instance.Accounts.Where(x => x.UserId == userid).FirstOrDefault() == null) return false;
            return true;
        }

        public static bool IsAccountBanned(Client player)
        {
            var ipban = ContextFactory.Instance.Bans.FirstOrDefault(ban => ban.Active == true && ban.Ip == player.address);
            if (ipban != null) return true;
            var socialclub = ContextFactory.Instance.Bans.FirstOrDefault(ban => ban.Active == true && ban.IsSocialClubBanned == true && ban.SocialClub == player.socialClubName);
            if (socialclub != null) return true;
            return false;
        }

        public static bool IsAccountBanned(Account account)
        {
            var accountban = ContextFactory.Instance.Bans.FirstOrDefault(ban => ban.Active == true && ban.AccountId == account.Id);
            if (accountban != null) return true;
            return false;
        }
    }
}