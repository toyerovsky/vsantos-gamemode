using System;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Database.Models;
using Serverside.Jobs.Base;
using Serverside.Jobs.Enums;

namespace Serverside.Jobs.Dustman
{
    internal class DustmanVehicle : JobVehicleController
    {
        private DustmanWorker WorkerInVehicle { get; set; }
        private API Api { get; set; }

        public DustmanVehicle(API api, FullPosition spawnPosition, VehicleHash hash, string numberplate, int numberplatestyle, int creatorId, Color primaryColor, Color secondaryColor, float enginePowerMultiplier = 0, float engineTorqueMultiplier = 0, Character character = null, Group group = null) : base(spawnPosition, hash, numberplate, numberplatestyle, creatorId, primaryColor, secondaryColor, enginePowerMultiplier, engineTorqueMultiplier, character, @group)
        {
            Api = api;
            Api.onPlayerEnterVehicle += OnPlayerEnterVehicle;
            Api.onVehicleHealthChange += OnVehicleHealthChange;
        }

        private void OnVehicleHealthChange(NetHandle entity, float oldValue)
        {
            if (entity == Vehicle)
            {
                decimal charge = Convert.ToDecimal(oldValue - Vehicle.health) / 10;
                WorkerInVehicle.CurrentSalary -= charge;
            }
        }

        private void OnPlayerEnterVehicle(Client player, NetHandle vehicle)
        {
            if (player.GetAccountController().CharacterController.Character.Job != (int) JobType.Smieciarz)
            {
                player.Notify("Aby skorzystać z tego pojazdu musisz podjąć pracę ~h~ Śmieciarz");
                return;
            }

            player.Notify("Pojazd do którego wsiadłeś zapewnił Ci pracodawca. Jesteś zobowiązany umową do pokrycia wszelkich strat.");

            WorkerInVehicle = new DustmanWorker(Api, player.GetAccountController(), this);
            WorkerInVehicle.Start();
        }
    }
}