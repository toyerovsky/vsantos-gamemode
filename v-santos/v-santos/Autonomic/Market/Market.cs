
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;

using Serverside.Core;
using Serverside.Core.Extensions;

namespace Serverside.Autonomic.Market
{
    public class Market
    {
        public Models.Market MarketData { get; set; }

        private ColShape MarketColshape { get; set; }
        private API Api { get; set; }
        private Bot MarkerNpc { get; set; }

        public Market(API api, Models.Market data)
        {
            Api = api;
            MarketData = data;

            //MarkerNpc = new Bot(api, );

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
        }
    }
}