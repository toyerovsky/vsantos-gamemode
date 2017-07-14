using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;

namespace Serverside.Jobs.Courier.CourierWarehouse.Models
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
        }

    }
}