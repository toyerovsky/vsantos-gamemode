using GTANetworkServer;
using Serverside.DatabaseEF6;
using Serverside.DatabaseEF6.Models;
using System;
using System.Linq;

namespace Serverside.Core
{
    public class AccountController
    {
        public long AccountId;
        public Account Account;
        public Client Client { get; private set; }
        //public CharacterController CharacterController;
        public AccountController(Account accountdata, Client client)
        {
            Account = accountdata;
            Client = client;

            client.setData("RP_ACCOUNT", Account);
            API.shared.sendNotificationToPlayer(client, string.Format("~w~Witaj zalogowałeś się na konto {0}.", Account.Email));
            API.shared.sendNotificationToPlayer(client, string.Format("~w~Ostatnie logowanie: {0}", Account.LastLogin.ToString()));
            API.shared.sendNotificationToPlayer(client, string.Format("~w~Z adresu IP: {0}", Account.Ip));

            Account.LastLogin = DateTime.Now;
            Account.Online = true;
            ContextFactory.Instance.SaveChanges();
            RPCore.Add(AccountId, this);
            //tutaj dajemy inne rzeczy które mają być inicjowane po zalogowaniu się na konto, np: wybór postaci.
        }

        public void Save()
        {
            //tutaj wywołać metody synchronizacji danych z innych controllerów np character.
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