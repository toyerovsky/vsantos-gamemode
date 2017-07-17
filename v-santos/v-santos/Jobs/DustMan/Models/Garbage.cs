/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using GrandTheftMultiplayer.Shared.Math;
using Serverside.Interfaces;

namespace Serverside.Jobs.Dustman.Models
{
    [Serializable]
    public class GarbageModel : IXmlObject
    {
        public int GtaPropId { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public string FilePath { get; set; }
        public string CreatorForumName { get; set; }
    }
}