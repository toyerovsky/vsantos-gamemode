using GTANetworkServer;
using GTANetworkShared;

namespace Serverside.Core.Vehicles
{
    public class RPIndicators : Script
    {
        public RPIndicators()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (eventName == "toggle_indicator_left")
            {
                var veh = API.getPlayerVehicle(player);
                int indicator = 1;
                Vector3 pos = API.getEntityPosition(player);

                if (API.hasEntitySyncedData(veh, "indicator_left") && API.getEntitySyncedData(veh, "indicator_left") == true)
                {
                    API.resetEntitySyncedData(veh, "indicator_left");
                    API.sendNativeToPlayersInRange(pos, 500f, 0xB5D45264751B7DF0, veh, indicator, false);
                }
                else
                {
                    API.setEntitySyncedData(veh, "indicator_left", true);
                    API.sendNativeToPlayersInRange(pos, 500f, 0xB5D45264751B7DF0, veh, indicator, true);
                }
            }
            else if (eventName == "toggle_indicator_right")
            {
                var veh = API.getPlayerVehicle(player);
                int indicator = 0;
                Vector3 pos = API.getEntityPosition(player);

                if (API.hasEntitySyncedData(veh, "indicator_right") && API.getEntitySyncedData(veh, "indicator_right") == true)
                {
                    API.resetEntitySyncedData(veh, "indicator_right");
                    API.sendNativeToPlayersInRange(pos, 500f, 0xB5D45264751B7DF0, veh, indicator, false);
                }
                else
                {
                    API.setEntitySyncedData(veh, "indicator_right", true);
                    API.sendNativeToPlayersInRange(pos, 500f, 0xB5D45264751B7DF0, veh, indicator, true);
                }
            }
        }
    }
}