/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
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
            vehicle.Repair();
        }
    }
}
