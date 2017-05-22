using GTANetworkServer;
using GTANetworkShared;
using Serverside.Controllers;

namespace Serverside.Extensions
{
    public static class NetHandleExtensions
    {
        public static VehicleController GetVehicleController(this NetHandle handle)
        {
            if (API.shared.getEntityType(handle) != EntityType.Vehicle)
                return null;
            return API.shared.getEntityData(handle, "VehicleController");
        }
    }
}