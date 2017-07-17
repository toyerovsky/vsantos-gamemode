/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using System.Linq;
using Serverside.Database;
using Serverside.Items;

namespace Serverside.Core.Telephone
{
    public static class TelephoneHelper
    {
        public static int GetNextFreeTelephoneNumber()
        {
            var numbers = ContextFactory.Instance.Items.Where(i => i.ItemType == (int)ItemType.Cellphone);

            Random r = new Random();
            int number = r.Next(100000000);

            while (numbers.Any(t => t.ThirdParameter == number))
            {
                number = r.Next(100000000);
            }
            return number;
        }
    }
}