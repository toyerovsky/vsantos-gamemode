
using GrandTheftMultiplayer.Server.Elements;
using Serverside.Core.Extensions;

namespace Serverside.Bank
{
    public static class BankHelper
    {
        public static void TakeMoneyToBank(Client player, decimal count)
        {
            if (player.HasMoney(count))
            {
                player.RemoveMoney(count);
                player.AddMoney(count, true);

                player.Notify(
                    $"Wpłacono ${count} na konto o numerze {player.GetAccountController().CharacterController.Character.BankAccountNumber}");
            }
            else
            {
                player.Notify("Nie posiadasz wystarczającej ilości gotówki.");
            }
        }

        public static void GiveMoneyFromBank(Client player, decimal count)
        {
            if (player.HasMoney(count, true))
            {
                player.RemoveMoney(count, true);
                player.AddMoney(count);

                player.Notify(
                    $"Wypłacono ${count} z konta o numerze {player.GetAccountController().CharacterController.Character.BankAccountNumber}");
            }
            else
            {
                player.Notify("Nie posiadasz wystarczającej ilości środków na koncie bankowym.");
            }
        }
    }
}