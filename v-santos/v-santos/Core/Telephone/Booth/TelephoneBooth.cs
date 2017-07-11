using System;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Serverside.Core.Telephone.Booth.Models;

namespace Serverside.Core.Telephone.Booth
{
    public class TelephoneBooth : IDisposable
    {
        public TelephoneCall CurrentCall { get; set; }
        public Client CurrentClient { get; set; }

        public TelephoneBoothModel Data { get; set; }
        public Marker Marker { get; set; }
        public CylinderColShape ColShape { get; set; }
        
        private API Api { get; }
        
        public TelephoneBooth(API api, TelephoneBoothModel data)
        {
            Api = api;
            Data = data;

            api.createTextLabel($"~y~BUDKA\n~w~Numer: {Data.Number}", new Vector3(Data.Position.Position.X, Data.Position.Position.Y, Data.Position.Position.Z + 1), 7f, 1f, true);
            ColShape = api.createCylinderColShape(Data.Position.Position, 1f, 2f);
            Marker = api.createMarker(1, Data.Position.Position, new Vector3(0, 0, 0), new Vector3(1f, 1f, 1f),
                new Vector3(1, 1, 1), 100, 255, 0, 0);

            ColShape.onEntityEnterColShape += (shape, entity) =>
            {
                if (shape == ColShape && api.getEntityType(entity) == EntityType.Player && CurrentCall != null)
                {
                    api.sendNotificationToPlayer(api.getPlayerFromHandle(entity), "Ta budka obecnie jest używana.");
                }
                else if (shape == ColShape && api.getEntityType(entity) == EntityType.Player && api.hasEntityData(Marker, "BoothRinging"))
                {
                    CurrentClient = Api.getPlayerFromHandle(entity);
                    CurrentCall?.Open();
                }
                else if (shape == ColShape && api.getEntityType(entity) == EntityType.Player && CurrentClient == null)
                {
                    CurrentClient = Api.getPlayerFromHandle(entity);
                    api.setEntityData(entity, "Booth", this);
                    api.triggerClientEvent(api.getPlayerFromHandle(entity), "OnPlayerEnteredTelephonebooth");
                }
            };

            ColShape.onEntityExitColShape += (shape, entity) =>
            {
                if (shape == ColShape && api.getEntityType(entity) == EntityType.Player && CurrentCall != null)
                {
                    CurrentClient = null;
                    api.resetEntityData(entity, "Booth");
                    CurrentCall?.Dispose();
                }
            };
        }

        public void Dispose()
        {
            CurrentCall?.Dispose();
            Api.deleteColShape(ColShape);
            Api.deleteEntity(Marker);
        }
    }
}