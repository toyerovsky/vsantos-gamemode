﻿/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using GrandTheftMultiplayer.Shared.Math;
using Serverside.Interfaces;

namespace Serverside.Jobs.Courier.CourierWarehouse.Models
{
    [Serializable]
    public class CourierWarehouseModel : IXmlObject
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public int BlipId { get; set; }
        public string FilePath { get; set; }
        public string CreatorForumName { get; set; }
    }
}
