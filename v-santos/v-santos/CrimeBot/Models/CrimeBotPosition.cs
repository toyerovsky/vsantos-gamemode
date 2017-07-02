using System;
using Serverside.Core;
using Serverside.Interfaces;

namespace Serverside.CrimeBot.Models
{
    [Serializable]
    public class CrimeBotPosition : IXmlObject
    {
        public string Name { get; set; }
        public FullPosition BotPosition { get; set; }
        public FullPosition VehiclePosition { get; set; }
        public string CreatorForumName { get; set; }
        public string FilePath { get; set; }
    }
}