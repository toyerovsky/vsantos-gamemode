using GrandTheftMultiplayer.Server.API;
using Serverside.Jobs.Base;

namespace Serverside.Jobs.Courier
{
    public class CourierJob : JobController
    {
        public CourierJob(API api, string jobName, decimal moneyLimit) : base(api, jobName, moneyLimit)
        {
        }
    }
}