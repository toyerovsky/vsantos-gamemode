using GTANetworkServer;
using GTANetworkShared;
using Serverside.Core.Extensions;

namespace Serverside.Autonomic.Market
{
    public class Market
    {
        public Models.Market MarketData { get; set; }
        private ColShape MarketColshape { get; set; }

        public Market(Models.Market data)
        {
            MarketData = data;

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