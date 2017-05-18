using GTANetworkServer;

namespace Serverside.Core.Extenstions
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

        #region Metody danych
        public static dynamic GetData(this Client client, string key)
        {
            return API.shared.getEntityData(client.handle, key);
        }

        /// <summary>
        /// Należy pamiętać, że twórcy GTA Network nie dali typu long do SyncedData i nie działa
        /// </summary>
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
