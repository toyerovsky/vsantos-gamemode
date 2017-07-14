using System;
using System.Globalization;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using Serverside.Bank.Models;
using Serverside.Core.Extensions;

namespace Serverside.Bank
{
    public class Atm : IDisposable
    {
        public Marker AtmMarker { get; }
        public CylinderColShape AtmShape { get; }
        public AtmModel Data { get; set; }
        public Blip AtmBlip { get; set; }

        private API Api { get; }

        public Atm(API api, AtmModel data)
        {
            Api = api;
            Data = data;

            AtmMarker = Api.createMarker(1, Data.Position.Position, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f), 100, 100, 100, 100);

            AtmShape = Api.createCylinderColShape(Data.Position.Position, 1f, 2f);

            AtmShape.onEntityEnterColShape += (shape, entity) =>
            {
                if (Api.getEntityType(entity) == EntityType.Player)
                {
                    var player = Api.getPlayerFromHandle(entity).GetAccountController();

                    if (player.CharacterController.Character.BankAccountNumber == null)
                    {
                        player.Client.Notify("Nie posiadasz karty bankomatowej, udaj się do banku, aby założyć konto.");
                        return;
                    }

                    Api.triggerClientEvent(player.Client, "OnPlayerEnteredAtm", JsonConvert.SerializeObject(new
                    {
                        FormatName = player.CharacterController.FormatName,
                        BankMoney = player.CharacterController.Character.BankMoney.ToString(CultureInfo.CurrentCulture),
                        BankAccountNumber = player.CharacterController.Character.BankAccountNumber.ToString()
                    }));
                }
            };

            AtmShape.onEntityExitColShape += (shape, entity) =>
            {
                if (Api.getEntityType(entity) == EntityType.Player)
                {
                    var player = Api.getPlayerFromHandle(entity);
                    Api.triggerClientEvent(player, "OnPlayerExitAtm");
                }
            };

            AtmBlip = Api.createBlip(Data.Position.Position);
            AtmBlip.sprite = 434;
            AtmBlip.color = 69;
            AtmBlip.transparency = 60;
        }

        public void Dispose()
        {
            Api.deleteEntity(AtmMarker);
            Api.deleteColShape(AtmShape);
            AtmBlip.transparency = 0;
        }
    }
}