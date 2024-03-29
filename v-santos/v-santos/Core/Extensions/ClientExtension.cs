﻿/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using Serverside.Controllers;
using Serverside.Core.Money;

namespace Serverside.Core.Extensions
{
    public static class ClientExtensions
    {
        public static AccountController GetAccountController(this Client client)
        {
            if (!client.HasData("RP_ACCOUNT"))
                return null;
            return client.GetData("RP_ACCOUNT") as AccountController;
        }

        public static void Notify(this Client client, string message, bool flashing = false)
        {
            API.shared.sendNotificationToPlayer(client, message, flashing);
        }

        //slot-- żeby numerowanie dla graczy było od 1 do 3
        public static bool TryGetGroupByUnsafeSlot(this Client client, short slot, out GroupController group)
        {
            group = null;
            if (slot > 0 || slot <= 3)
            {
                slot--;
                var groups = RPEntityManager.GetPlayerGroups(client.GetAccountController());
                group = (slot < groups.Count) ? groups[slot] : null;
            }
            return group != null;
        }

        #region Pieniądze
        public static bool HasMoney(this Client client, decimal count, bool bank = false)
        {
           return MoneyManager.CanPay(client, count, bank);
        }

        public static void AddMoney(this Client client, decimal count, bool bank = false)
        {
            MoneyManager.AddMoney(client, count, bank);
        }

        public static void RemoveMoney(this Client client, decimal count, bool bank = false)
        {
            MoneyManager.RemoveMoney(client, count, bank);
        }
        #endregion

        #region Metody danych
        public static dynamic GetData(this Client client, string key)
        {
            return API.shared.getEntityData(client.handle, key);
        }

        /// <summary>
        /// Należy pamiętać, że twórcy GTA Network nie dali typu long do SyncedData i nie działa
        /// </summary>
        /// <param name="client"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static dynamic GetSyncedData(this Client client, string key)
        {
            return API.shared.getEntitySyncedData(client.handle, key);
        }

        public static void SetData(this Client client, string key, object value)
        {
            API.shared.setEntityData(client.handle, key, value);
        }

        /// <summary>
        /// Należy pamiętać, że twórcy GTA Network nie dali typu long do SyncedData i nie działa
        /// </summary>
        /// <param name="client"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetSyncedData(this Client client, string key, object value)
        {
            API.shared.setEntitySyncedData(client, key, value);
        }

        public static bool HasData(this Client client, string key)
        {
            return API.shared.hasEntityData(client.handle, key);
        }

        public static bool HasSyncedData(this Client client, string key)
        {
            return API.shared.hasEntitySyncedData(client.handle, key);
        }

        public static void ResetData(this Client client, string key)
        {
            API.shared.resetEntityData(client.handle, key);
        }

        public static void ResetSyncedData(this Client client, string key)
        {
            API.shared.resetEntitySyncedData(client.handle, key);
        }

        public static bool TryGetData(this Client client, string key, out dynamic data)
        {
            if (client.HasData(key))
            {
                data = client.GetData(key);
                return true;
            }
            data = null;
            return false;
        }

        public static bool TryGetSyncedData(this Client client, string key, out dynamic data)
        {
            if (client.HasSyncedData(key))
            {
                data = client.GetSyncedData(key);
                return true;
            }
            data = null;
            return false;
        }
        #endregion
    }
}
