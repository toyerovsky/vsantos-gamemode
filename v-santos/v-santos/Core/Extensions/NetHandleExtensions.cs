using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using Serverside.Controllers;

namespace Serverside.Core.Extensions
{
    public static class VehicleExtensions
    {
        public static VehicleController GetVehicleController(this Vehicle handle)
        {
            if (!handle.hasData("VehicleController"))
                return null;
            return API.shared.getEntityData(handle, "VehicleController");
        }
    }
}