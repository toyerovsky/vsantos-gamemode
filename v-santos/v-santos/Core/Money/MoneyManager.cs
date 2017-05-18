using System;
using GTANetworkServer;
using Serverside.Database;
using Serverside.DatabaseEF6;
using Serverside.DatabaseEF6.Models;
using Serverside.Core.Extenstions;

namespace Serverside.Core.Money
{
    public class MoneyChangedEventArgs : EventArgs
    {
        public Client Player { get; }

        public MoneyChangedEventArgs(Client player)
        {
            Player = player;
        }
    }
    
    public class MoneyManager
    {
        public delegate void MoneyChangedEventHandler(Client sender, decimal newvalue, decimal oldvalue);

        public event MoneyChangedEventHandler MoneyChanged;

        API api = new API();

        public MoneyManager()
        {
            MoneyChanged += RPMoney_MoneyChanged;
        }

        private void RPMoney_MoneyChanged(Client sender, decimal newvalue, decimal oldvalue)
        {
            //Character givingCharacterEditor = CharacterDatabaseHelper.SelectCharacter(Convert.ToInt64(api.getEntityData(sender.handle, "CharacterID")));
            //tu sie wysyla event, zeby narysopwalo graczowi stan jego gotowki
            api.triggerClientEvent(sender, "Money_Changed", String.Format(@"${0}", newvalue));
        }

        public bool CanPay(AccountController account, decimal count, bool bank = false)
        {
            //Character editor = CharacterDatabaseHelper.SelectCharacter(cid);
            if (bank)
            {
                return account.CharacterController.Character.BankMoney >= count;
            }
            return account.CharacterController.Character.Money >= count;
        }

        public void AddMoney(AccountController account, decimal count, bool bank = false)
        {
            //Character character = CharacterDatabaseHelper.SelectCharacter(cid);
            decimal newvalue = 0;
            decimal oldvalue = 0;
            if (bank)
            {
                account.CharacterController.Character.BankMoney += count;
                newvalue = account.CharacterController.Character.BankMoney;
            }
            else
            {
                account.CharacterController.Character.Money += count;
                newvalue = account.CharacterController.Character.Money;
            }
            oldvalue = newvalue - count;
            account.CharacterController.Save();
            //CharacterDatabaseHelper.UpdateCharacter(character);
            //MoneyChangedEventArgs e = new MoneyChangedEventArgs(account.Client);
            if (MoneyChanged != null) MoneyChanged.Invoke(account.Client, newvalue, oldvalue);
        }

        public void RemoveMoney(AccountController account, decimal count, bool bank = false)
        {
            //Character character = CharacterDatabaseHelper.SelectCharacter(account);
            decimal newvalue = 0;
            decimal oldvalue = 0;
            if (bank)
            {
                account.CharacterController.Character.BankMoney -= count;
                newvalue = account.CharacterController.Character.BankMoney;
            }
            else
            {
                account.CharacterController.Character.Money -= count;
                newvalue = account.CharacterController.Character.Money;
            }
            oldvalue = newvalue + count;
            account.CharacterController.Save();
            //CharacterDatabaseHelper.UpdateCharacter(character);
            //MoneyChangedEventArgs e = new MoneyChangedEventArgs(account.Client);
            if (MoneyChanged != null) MoneyChanged.Invoke(account.Client, newvalue, oldvalue);
        }
    }
}