﻿using System;
using System.Collections.Generic;
using Serverside.Core;

namespace Serverside.Corners.Models
{
    [Serializable]
    public class CornerModel
    {
        public List<CornerBotModel> CornerBots { get; set; }
        public FullPosition Position { get; set; }
        public List<FullPosition> BotPositions { get; set; }
    }
}