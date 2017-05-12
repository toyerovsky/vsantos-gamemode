using System;
using GTANetworkShared;

namespace Serverside.Core
{
    [Serializable]
    public class FullPosition
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Direction { get; set; }

        public FullPosition()
        {
        }

        public FullPosition(Vector3 position, Vector3 rotation, Vector3 direction = null)
        {
            Position = position;
            Rotation = rotation;
            Direction = direction;
        }
    }
}
