using System;

namespace Serverside.Core
{
    public static class Validator
    {
        public static bool IsMoneyStringValid(string moneyToCheck)
        {
            int money;
            try
            {
                money = Int32.Parse(moneyToCheck);
            }
            catch
            {
                return false;
            }
            return money >= 0;
        }

        public static bool IsIntIdValid(string id)
        {
            int ID;
            try
            {
                ID = Int32.Parse(id);
            }
            catch
            {
                return false;
            }
            return ID >= 0;
        }

        public static bool IsGroupSlotValid(string groupSlot)
        {
            int slot;
            try
            {
                slot = Int32.Parse(groupSlot);
            }
            catch
            {
                return false;
            }
            return slot <= 3 && slot >= 1;
        }

        public static bool IsLongIdValid(string longid)
        {
            long ID;
            try
            {
                ID = long.Parse(longid);
            }
            catch
            {
                return false;
            }
            return ID >= 0;
        }

        public static bool IsCellphoneNumberValid(string number)
        {
            int convertedNumber;
            try
            {
                convertedNumber = int.Parse(number);
            }
            catch
            {
                return false;
            }
            return convertedNumber >= 0 && convertedNumber <= 100000000;
        }
    }
}