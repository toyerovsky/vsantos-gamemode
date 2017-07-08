using System.Globalization;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using Serverside.Core.Extensions;

namespace Serverside.Core.Money
{    
    public static class MoneyManager
    {
        private delegate void MoneyChangedEventHandler(Client sender);

        private static event MoneyChangedEventHandler MoneyChanged;

        private static readonly API Api = new API();

        static MoneyManager()
        {
            MoneyChanged += RPMoney_MoneyChanged;
        }

        private static void RPMoney_MoneyChanged(Client sender)
        {
            Api.triggerClientEvent(sender, "MoneyChanged", sender.GetAccountController().CharacterController.Character.Money.ToString(CultureInfo.InvariantCulture));
        }

        public static bool CanPay(Client sender, decimal count, bool bank = false)
        {
            var player = sender.GetAccountController();
            if (bank) return player.CharacterController.Character.BankMoney >= count;
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
                MoneyChanged?.Invoke(sender);
            }
            player.CharacterController.Save();
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
                MoneyChanged?.Invoke(sender);
            }
            player.CharacterController.Save();
        }
    }
}