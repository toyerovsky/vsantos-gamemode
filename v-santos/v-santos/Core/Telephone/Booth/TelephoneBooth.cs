using System;
using System.Linq;
using System.Xml.Serialization;
using GTANetworkServer;
using GTANetworkShared;

namespace Serverside.Core.Telephone.Booth
{
    [Serializable]
    public class TelephoneBooth
    {
        public FullPosition Position { get; set; }
        [XmlIgnore]
        public Marker Marker { get; set; }
        [XmlIgnore]
        public CylinderColShape ColShape { get; set; }
        public int Number { get; set; }
        public int Cost { get; set; }

        public TelephoneBooth()
        { }

        public TelephoneBooth(int number, FullPosition position, int cost)
        {
            Number = number;
            Position = position;
            Cost = cost;
        }

        public void Intialize(API api)
        {
            api.createTextLabel($"BUDKA\nNumer: {Number}", new Vector3(Position.Position.X, Position.Position.Y, Position.Position.Z + 1), 7f, 1f, true);
            ColShape = api.createCylinderColShape(Position.Position, 1f, 2f);
            Marker = api.createMarker(1, Position.Position, new Vector3(0, 0, 0), new Vector3(1f, 1f, 1f),
                new Vector3(1, 1, 1), 100, 255, 0, 0);

            Marker.setData("BoothNumber", Number);

            ColShape.onEntityEnterColShape += (shape, entity) =>
            {
                if (api.getEntityType(entity) == EntityType.Player && api.hasEntityData(Marker, "BoothBusy"))
                {
                    api.sendNotificationToPlayer(api.getPlayerFromHandle(entity), "Ta budka obecnie jest używana.");
                }
                else if (api.getEntityType(entity) == EntityType.Player && api.hasEntityData(Marker, "BoothRinging"))
                {
                    api.setEntityData(Marker, "BoothBusy", true);
                    TelephoneCall call = RPTelephoneBooth.CurrentBoothCalls.First(g => g.BoothNumber == Number);
                    call.Open();
                }
                else if (api.getEntityType(entity) == EntityType.Player)
                {
                    api.setEntityData(entity, "BoothNumber", Number);
                    api.triggerClientEvent(api.getPlayerFromHandle(entity), "OnPlayerEnteredTelephonebooth");
                }
            };

            ColShape.onEntityExitColShape += (shape, entity) =>
            {
                if (api.getEntityType(entity) == EntityType.Player && api.hasEntityData(Marker, "thisooththisusy"))
                {
                    api.resetEntityData(Marker, "BoothBusy");
                    api.resetEntityData(entity, "BoothNumber");

                    TelephoneCall call = RPTelephoneBooth.CurrentBoothCalls.First(g => g.BoothNumber == Number);
                    call.Dispose();
                }
            };
        }
    }
}