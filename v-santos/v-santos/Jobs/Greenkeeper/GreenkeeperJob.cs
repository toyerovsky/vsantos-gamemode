using GrandTheftMultiplayer.Server.API;
using Serverside.Controllers;
using Serverside.Core.Extensions;
using Serverside.Jobs.Base;

namespace Serverside.Jobs.Greenkeeper
{
    public class GreenkeeperJob : JobController
    {

        public void StartJob(AccountController player)
        {
            
            //TODO: wysyłanie zgłoszenia na 911
        }

        public GreenkeeperJob(API api, string jobName, decimal moneyLimit) : base(api, jobName, moneyLimit)
        {
        }
    }
}
