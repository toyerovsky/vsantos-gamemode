using System;
using Serverside.Interfaces;

namespace Serverside.Core.ServerInfo.Models
{
    public class ServerInfoData : IXmlObject
    {
        public string FilePath { get; set; }
        public string CreatorForumName { get; set; }

        public DateTime JobsResetTime { get; set; }
        public DateTime CrimeBotResetTime { get; set; }
        public DateTime GroupsPayDayTime { get; set; }
    }
}