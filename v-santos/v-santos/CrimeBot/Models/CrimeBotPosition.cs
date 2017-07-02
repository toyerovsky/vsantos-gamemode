using System;
using Serverside.Core;

namespace Serverside.CrimeBot.Models
{
    [Serializable]
    public class CrimeBotPosition
    {
        public string Name { get; set; }
        public FullPosition BotPosition { get; set; }
        public FullPosition VehiclePosition { get; set; }
        public string CreatorForumName { get; set; }
    }
}