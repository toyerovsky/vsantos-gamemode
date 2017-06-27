using System;
using System.Data.Entity;
using System.Linq;
using GTANetworkServer;
using Serverside.Core;
using Serverside.Database;
using Serverside.Database.Models;
using Serverside.Core.Extensions;

namespace Serverside.Controllers
{
    public class AccountController
    {
        public static event AccountLoginEventHandler OnPlayerCharacterLogin;

        public long AccountId => AccountData.UserId;
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

            //tutaj dajemy inne rzeczy które mają być inicjowane po zalogowaniu się na konto, np: wybór postaci.

            //to zrobimy to bezpiecznie :) // DeMoN
            string[] ip = AccountData.Ip.Split('.');
            string safeIp = $"{ip[0]}.{ip[1]}.***.***";
            client.triggerEvent("ShowNotification", $"Witaj, {AccountData.Name} zostałeś pomyślnie zalogowany. ~n~Ostatnie logowanie: {AccountData.LastLogin} z adresu IP: {safeIp}", 5000);

            if (AccountData.Characters == null || AccountData.Characters.Count == 0)
            {
                if (!CharacterController.AddCharacter(this, AccountData.Email, "Testowy", PedHash.Michael))
                {
                    //RPChat.SendMessageToPlayer(client, "Postać o wybranym imieniu i nazwisku już istnieje.", ChatMessageType.ServerInfo);
                    //client.kick("Postać o wybranym imieniu i nazwisku już istnieje.");
                    //return;
                }
            }

            ContextFactory.Instance.SaveChanges();
            RPEntityManager.AddAccount(AccountId, this);
            OnPlayerCharacterLogin?.Invoke(client, this);
        }

        public static AccountController GetAccountControllerFromName(string formatname)
        {
            Client client = RPEntityManager.GetAccounts().First(x => x.Value.CharacterController.FormatName.ToLower().Contains(formatname.ToLower())).Value.Client;
            return client?.GetAccountController();
        }

        public static void LoadAccount(Client sender, long userId)
        {
            new AccountController(ContextFactory.Instance.Accounts.FirstOrDefault(x => x.UserId == userId), sender);
        }

        public static void RegisterAccount(Client sender, Account account)
        {
            ContextFactory.Instance.Accounts.Add(account);
            ContextFactory.Instance.SaveChanges();

            new AccountController(account, sender);
        }

        public static bool HasCharacterSlot(AccountController account)
        {
            if (account.AccountData.Characters != null && account.AccountData.Characters.Count > 3)
            {
                API.shared.sendChatMessageToPlayer(account.Client, "~r~Błąd: ~w~Osiągnąłeś maksymalny limit postaci na konto!");
                return false;
            }
            return true;
        }

        public void Save(bool resourceStop = false)
        {
            //tutaj wywołać metody synchronizacji danych z innych controllerów np character.
            CharacterController?.Save(resourceStop);

            ContextFactory.Instance.Accounts.Attach(AccountData);
            ContextFactory.Instance.Entry(AccountData).State = EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }

        public static bool DoesAccountExist(long userid)
        {
            return ContextFactory.Instance.Accounts.Where(x => x.UserId == userid).ToList().Count != 0;
        }

        public static bool IsAccountBanned(Client player)
        {
            var ipban = ContextFactory.Instance.Bans.FirstOrDefault(ban => ban.Active && ban.Ip == player.address);
            if (ipban != null) return true;
            var socialclub = ContextFactory.Instance.Bans.FirstOrDefault(ban => ban.Active && ban.IsSocialClubBanned && ban.SocialClub == player.socialClubName);
            return socialclub != null;
        }

        public static bool IsAccountBanned(Account account)
        {
            return ContextFactory.Instance.Bans.FirstOrDefault(ban => ban.Active && ban.AccountId == account.Id) != null;
        }
    }
}