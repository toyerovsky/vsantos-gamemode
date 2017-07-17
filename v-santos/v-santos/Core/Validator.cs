/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

namespace Serverside.Core
{
    public static class Validator
    {
        public static bool IsMoneyValid(decimal moneyToCheck) => moneyToCheck >= 0;

        public static bool IsGroupSlotValid(string groupSlot) => short.TryParse(groupSlot, out short slot) && slot <= 3 && slot >= 1;
       
        public static bool IsCellphoneNumberValid(string number)
        {
            return int.TryParse(number, out int converted) && converted.ToString().Length > 0 && converted.ToString().Length <= 9;
        }

        public static bool IsGroupSlotValid(short slot) => slot <= 3 && slot >= 0;
    }
}