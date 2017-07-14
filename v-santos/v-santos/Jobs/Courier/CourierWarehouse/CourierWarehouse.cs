using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using Serverside.Core.Extensions;
using Serverside.Database.Models;
using Serverside.Groups;
using Serverside.Groups.Stucts;
using Serverside.Jobs.Courier.CourierWarehouse.Models;
using Serverside.Jobs.Enums;

namespace Serverside.Jobs.Courier.CourierWarehouse
{
    public class CourierWarehouse
    {
        public CourierWarehouseModel Data { get; set; }
        
        public Marker WarehouseMarker { get; set; }    
        public ColShape WarehouseColshape { get; set; }
        public Blip WarehouseBlip { get; set; }
        
        private API Api { get; set; }
        
        public CourierWarehouse(API api, CourierWarehouseModel data)
        {
            Api = api;
            Data = data;
            WarehouseMarker = Api.createMarker(0, Data.Position, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f),
                new Vector3(1f, 1f, 1f), 255, 255, 0, 0);
            WarehouseColshape = Api.createCylinderColShape(Data.Position, 5f, 5f);
            WarehouseBlip = Api.createBlip(Data.Position);
            WarehouseBlip.sprite = 479;

            WarehouseColshape.onEntityEnterColShape += (shape, entity) =>
            {
                if (Api.getEntityType(entity) == EntityType.Player)
                {
                    var player = Api.getPlayerFromHandle(entity).GetAccountController();
                    if (player.CharacterController.Character.Job != (int)JobType.Kurier)
                    {
                        player.Client.Notify("Aby podjąć pracę kuriera udaj się do pracodawcy.");
                        return;
                    }

                    if (RPGroupWarehouse.CurrentOrders.Count == 0)
                    {
                        player.Client.Notify("Obecnie nie ma żadnych paczek w magazynie.");
                        return;
                    }

                    player.Client.triggerEvent("ShowCourierMenu", JsonConvert.SerializeObject(RPGroupWarehouse.CurrentOrders.Select(x => new
                    {
                        Id = x.Data.Id,
                        Getter = x.Data.Getter.Name,
                    })));
                }
            };

            WarehouseColshape.onEntityExitColShape += (shape, entity) =>
            {
                if (Api.getEntityType(entity) == EntityType.Player)
                {
                    var player = Api.getPlayerFromHandle(entity).GetAccountController();
                    if (player.CharacterController.Character.Job != (int)JobType.Kurier) return;
                    player.Client.triggerEvent("DisposeCourierMenu");
                }
            };
        }
    }
}