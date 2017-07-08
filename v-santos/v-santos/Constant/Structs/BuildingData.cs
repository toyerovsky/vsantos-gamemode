

using GrandTheftMultiplayer.Shared.Math;

namespace Serverside.Constant.Structs
{
    public struct BuildingData
    {
        public string Name { get; set; }
        public Vector3 InternalPosition { get; set; }

        public BuildingData(string name, Vector3 internalPostion)
        {
            Name = name;
            InternalPosition = internalPostion;
        }
    }
}