﻿/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

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