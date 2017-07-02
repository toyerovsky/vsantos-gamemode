using System;
using GTANetworkShared;
using Serverside.Interfaces;

namespace Serverside.Core.Bus.Models
{
    [Serializable]
    public class BusStopModel : IXmlObject
    {
        public string Name { get; set; }
        public Vector3 Center { get; set; }
        public string CreatorForumName { get; set; }
        public string FilePath { get; set; }
    }
}