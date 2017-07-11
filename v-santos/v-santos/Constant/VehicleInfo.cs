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