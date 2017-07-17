/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using GrandTheftMultiplayer.Shared;

namespace Serverside.Constant
{
    public class VehicleInfo
    {
        public VehicleHash Hash { get; set; }
        public int HorsePower { get; set; }
        public float EngineCapacity { get; set; }

        public VehicleInfo(VehicleHash hash, int horsePower, float engineCapacity)
        {
            Hash = hash;
            HorsePower = horsePower;
            EngineCapacity = engineCapacity;
        }
    }
}