using System;
using System.Collections.Generic;
using Serverside.Core;
using Serverside.Interfaces;

namespace Serverside.Corners.Models
{
    [Serializable]
    public class CornerModel : IXmlObject
    {
        public List<CornerBotModel> CornerBots { get; set; }
        public FullPosition Position { get; set; }
        public List<FullPosition> BotPositions { get; set; }
        public string CreatorForumName { get; set; }
        public string FilePath { get; set; }
    }
}