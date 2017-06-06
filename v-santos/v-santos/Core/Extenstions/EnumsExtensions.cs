using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serverside.Core.Extenstions
{
    class EnumsExtensions
    {
        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }

        public enum VehicleClass
        {
            Compact = 0,
            Sedans = 1,
            SUVs = 2,
            Coupe = 3,
            Muscle = 4,
            SportClassics = 5,
            Sports = 6,
            Super = 7,
            Motorcycles = 8,
            Offroad = 9,
            Industrial = 10,
            Utility = 11,
            Vans = 12,
            Cycle = 13,
            Boat = 14,
            Heli = 15,
            Planes = 16,
            Service = 17,
            Emergency = 18,
            Military = 19,
            Commercial = 20,
            Train = 21,
            Trailer = 22
        }

        public enum Wheel
        {
            FrontLeft,
            FrontRight,
            MiddleLeft,
            MiddleRight,
            RearLeft,
            RearRight,
            BikeFront,
            BikeRear
        }

        public enum Doors
        {
            FrontLeftDoor,
            FrontRightDoor,
            BackLeftDoor,
            BackRightDoor,
            Hood,
            Trunk,
            Back,
            Back2,
        }


    }
}
