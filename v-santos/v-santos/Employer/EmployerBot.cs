

using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using Serverside.Core;
using Serverside.Core.Extensions;

namespace Serverside.Employer
{
    public sealed class EmployerBot : Bot
    {
        private API Api { get; }
        

        public EmployerBot(API api, string name, PedHash pedHash, FullPosition position) : base(api, name, pedHash, position)
        {
            Api = api;
        }

        public override void Intialize()
        {
            base.Intialize();
            ColShape employerColShape = Api.createCylinderColShape(BotHandle.position, 3f, 2f);

            employerColShape.onEntityEnterColShape += (shape, entity) =>
            {
                if (API.shared.getEntityType(entity) == EntityType.Player)
                {
                    var sender = API.shared.getPlayerFromHandle(entity);
                    Api.triggerClientEvent(sender, "OnPlayerEnteredEmployer", sender.GetAccountController().CharacterController.Character.MoneyJob.ToString());
                }
            };

            employerColShape.onEntityExitColShape += (shape, entity) =>
            {
                if (API.shared.getEntityType(entity) == EntityType.Player)
                {
                    var sender = API.shared.getPlayerFromHandle(entity);
                    Api.triggerClientEvent(sender, "OnPlayerExitEmployer");
                }
            };
        }
    }
}