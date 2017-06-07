using GTANetworkServer;
using Serverside.Core;
using Serverside.Core.Extensions;

namespace Serverside.Offers
{
    public static class OfferActions
    {
        public static void GiveIdCard(Client getter)
        {
            var player = getter.GetAccountController();
            player.CharacterController.Character.HasIDCard = true;
            player.CharacterController.Save();
        }

        public static void GiveDrivingLicense(Client getter)
        {
            var player = getter.GetAccountController();
            player.CharacterController.Character.HasDrivingLicense = true;
            player.CharacterController.Save();
        }

        public static void RepairVehicle(Client getter)
        {
            var vehicle = RPEntityManager.GetVehicle(API.shared.getPlayerVehicle(getter));
            if (vehicle == null) return;
            vehicle.Vehicle.repair();
            vehicle.Save();
        }
    }
}
