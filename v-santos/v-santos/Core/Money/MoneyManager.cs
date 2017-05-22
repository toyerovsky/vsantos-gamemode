using System;
using GTANetworkServer;
using Serverside.Core.Finders;
using Serverside.Extensions;

namespace Serverside.Core.Money
{    
    public class MoneyManager
    {
        private delegate void MoneyChangedEventHandler(Client sender);

        private static event MoneyChangedEventHandler MoneyChanged;

        private static readonly API Api = new API();

        public MoneyManager()
        {
            MoneyChanged += RPMoney_MoneyChanged;
        }

        private static void RPMoney_MoneyChanged(Client sender)
        {
            //tu sie wysyla event, zeby narysopwalo graczowi stan jego gotowki
            Api.triggerClientEvent(sender, "Money_Changed",
                $@"${sender.GetAccountController().CharacterController.Character.Money}");
        }

        public static bool CanPay(Client sender, decimal count, bool bank = false)
        {
            var player = sender.GetAccountController();
            if (bank)
            {
                return player.CharacterController.Character.BankMoney >= count;
            }
            return player.CharacterController.Character.Money >= count;
        }

        public static void AddMoney(Client sender, decimal count, bool bank = false)
        {
            var player = sender.GetAccountController();
            if (bank)
            {
                player.CharacterController.Character.BankMoney += count;
            }
            else
            {
                player.CharacterController.Character.Money += count;
            }
            player.CharacterController.Save();
            if (MoneyChanged != null) MoneyChanged.Invoke(sender);
        }

        public static void RemoveMoney(Client sender, decimal count, bool bank = false)
        {
            var player = sender.GetAccountController();
            if (bank)
            {
                player.CharacterController.Character.BankMoney -= count;
            }
            else
            {
                player.CharacterController.Character.Money -= count;
            }
            player.CharacterController.Save();
            if (MoneyChanged != null) MoneyChanged.Invoke(sender);
        }
    }
}