/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Serverside.Core;
using Serverside.Core.Extensions;

namespace Serverside.Autonomic.Market
{
    public class Market : IDisposable
    {
        public Models.Market MarketData { get; set; }

        private ColShape MarketColshape { get; set; }
        private API Api { get; set; }
        private Bot MarketNpc { get; set; }
        private Blip MarketBlip { get; set; }

        public Market(API api, Models.Market data)
        {
            Api = api;
            MarketData = data;

            var botInfo =
                Constant.ConstantItems.ConstantNames.OrderBy(x => new Random().Next(Constant.ConstantItems.ConstantNames
                    .Count)).ElementAt(0);

            MarketNpc = new Bot(Api, botInfo.Key, botInfo.Value, new FullPosition(MarketData.Center, new Vector3(1f, 1f, 1f)));

            MarketColshape = Api.createCylinderColShape(data.Center, data.Radius, 5f);

            MarketColshape.onEntityEnterColShape += (shape, entity) =>
            {
                if (API.shared.getEntityType(entity) == EntityType.Player)
                {
                    API.shared.getPlayerFromHandle(entity).SetData("CurrentMarket", this);
                }
            };

            MarketColshape.onEntityExitColShape += (shape, entity) =>
            {
                if (API.shared.getEntityType(entity) == EntityType.Player)
                {
                    API.shared.getPlayerFromHandle(entity).ResetData("CurrentMarket");
                }
            };

            MarketBlip = Api.createBlip(MarketData.Center);
            MarketBlip.sprite = 93;
        }

        public void Dispose()
        {
            MarketNpc?.Dispose();
            Api.deleteColShape(MarketColshape);
            MarketBlip.transparency = 0;
        }
    }
}