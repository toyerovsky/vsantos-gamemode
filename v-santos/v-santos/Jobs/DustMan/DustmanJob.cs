using GrandTheftMultiplayer.Server.API;
using Serverside.Jobs.Base;

namespace Serverside.Jobs.Dustman
{
    public class DustmanJob : JobController
    {
        public DustmanJob(API api, string name, decimal moneyLimit) : base(api, name, moneyLimit)
        {
        }
    }
}