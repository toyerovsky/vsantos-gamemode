using GTANetworkShared;

namespace Serverside.Constant.Structs
{
    public struct BuildingData
    {
        public string Name { get; set; }
        public Vector3 InternalPostion { get; set; }

        public BuildingData(string name, Vector3 internalPostion)
        {
            Name = name;
            InternalPostion = internalPostion;
        }
    }
}