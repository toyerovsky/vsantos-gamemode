using GTANetworkServer;

namespace Serverside.Autonomic.Market
{
    public class Market
    {
        public Models.Market MarketData { get; set; }

        private ColShape MarketColshape { get; set; }

        public Market(Models.Market data)
        {
            MarketData = data;
        }
    }
}