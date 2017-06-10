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

            //Przenoszę dodawanie konta do RPEntityManager w inne miejsce bo jeśli wyłączymy serwer jak gracz jest w oknie logowania to wywala NullReferenceException

            //tutaj dajemy inne rzeczy które mają być inicjowane po zalogowaniu się na konto, np: wybór postaci.

            //TODO: tutaj ma się wyświetlać nick z forum a nie social club
            client.triggerEvent("ShowNotification", $"Witaj, {accountdata.SocialClub} zostałeś pomyślnie zalogowany. Wybierz postać.", 3000);

            //jezus maria było dobrze to musiałeś zepsuć
            //if (AccountData.Character == null) AccountData.Character = ContextFactory.Instance.Characters.Where(ch => ch.Account.UserId == AccountId).ToList();
            //if (AccountData.Character.Count == 0)
            //{
            //    if (!CharacterController.AddCharacter(this, AccountData.Email, "Testowy", PedHash.Michael))
            //    {
            //        RPChat.SendMessageToPlayer(client, "Postać o wybranym imieniu i nazwisku już istnieje.", ChatMessageType.ServerInfo);
            //        client.kick("Postać o wybranym imieniu i nazwisku już istnieje.");
            //        return;
            //    }
            //}
            
            //w tym momencie po takim przypisaniu do json jego wartość to "[]"
            //string json = JsonConvert.SerializeObject(AccountData.Character.Where(c => c.IsAlive == true).Select(
            //    ch => new
            //    {
            //        ch.Name,
            //        ch.Surname,
            //        ch.Money,
            //        ch.BankMoney
            //    }).ToList());

            string json = JsonConvert.SerializeObject(new
            {
                Name = "test",
                Surname = "test2",
                Money = 500,
                BankMoney = 122,
            });

            API.shared.consoleOutput(json);
            client.triggerEvent("ShowCharacterSelectMenu", json);
            ContextFactory.Instance.SaveChanges();
        }

        public static AccountController GetAccountControllerFromName(string formatname)
        {
            Client client = API.shared.getAllPlayers().FirstOrDefault(x => x.GetAccountController().CharacterController.FormatName.ToLower().Contains(formatname.ToLower()));
            return client != null ? client.GetAccountController() : null;
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
            CharacterController?.Save(resourceStop);

            ContextFactory.Instance.Accounts.Attach(AccountData);
            ContextFactory.Instance.Entry(AccountData).State = EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }

        public static bool DoesAccountExist(long userid)
        {
            return ContextFactory.Instance.Accounts.FirstOrDefault(x => x.UserId == userid) != null;
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