using System.Globalization;
using GTANetworkServer;
using GTANetworkShared;
using Newtonsoft.Json;
using Serverside.Bank.Models;
using Serverside.Core.Extensions;

namespace Serverside.Bank
{
    public class Atm
    {
        public Marker AtmMarker { get; }
        public CylinderColShape AtmShape { get; }
        public AtmModel Data { get; set; }

        private API Api { get; }

        public Atm(API api, AtmModel data)
        {
            Api = api;
            Data = data;

            AtmMarker = Api.createMarker(1, Data.Position.Position, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f), 100, 100, 100, 100);
            AtmMarker.invincible = true;

            AtmShape = Api.createCylinderColShape(Data.Position.Position, 1f, 2f);

            AtmShape.onEntityEnterColShape += (shape, entity) =>
            {
                if (Api.getEntityType(entity) == EntityType.Player)
                {
                    var player = Api.getPlayerFromHandle(entity).GetAccountController();
                    Api.triggerClientEvent(player.Client, "OnPlayerEnteredAtm", JsonConvert.SerializeObject(new
                    {
                        FormatName = player.CharacterController.FormatName,
                        BankMoney = player.CharacterController.Character.BankMoney.ToString(CultureInfo.CurrentCulture),
                        AccountNumber = player.CharacterController.Character.BankAccountNumber.ToString()
                    }));
                }
            };

            AtmShape.onEntityExitColShape += (shape, entity) =>
            {
                if (Api.getEntityType(entity) == EntityType.Player)
                {
                    var player = Api.getPlayerFromHandle(entity).GetAccountController();
                    Api.triggerClientEvent(player.Client, "OnPlayerExitAtm");
                }
            };
        }
    }
}