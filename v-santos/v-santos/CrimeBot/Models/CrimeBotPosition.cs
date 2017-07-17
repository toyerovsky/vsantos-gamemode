/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

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