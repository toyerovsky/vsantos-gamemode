using GrandTheftMultiplayer.Server.API;
using Serverside.Controllers;
using Serverside.Jobs.Base;

namespace Serverside.Jobs.Courier
{
    public class CourierWorker : JobWorker
    {
        public CourierWorker(AccountController player, JobVehicleController jobVehicle, API api) : base(api, player, jobVehicle)
        {
        }
    }
}