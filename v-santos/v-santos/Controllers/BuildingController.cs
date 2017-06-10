using System;
using System.Collections.Generic;
using System.Data.Entity;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Database;
using Serverside.Database.Models;

namespace Serverside.Controllers
{
    public class BuildingController : IDisposable
    {
        public Building BuildingData { get; set; }
        public ColShape InteriorDoorsColshape { get; set; }
        public ColShape ExteriorDoorsColshape { get; set; }
        public Marker BuildingMarker { get; set; }
        public TextLabel BuildingLabel { get; set; }

        public List<Client> PlayersInBuilding { get; set; } = new List<Client>();
        public bool DoorsLocked { get; set; } = true;

        //Ładowanie budynku
        public BuildingController(Building data)
        {
            BuildingData = data;
            Initialize();
        }

        //Tworzenie nowego budynku
        public BuildingController(long creatorId, float internalX, float internalY, float internalZ, float externalX, float externalY, float externalZ, decimal cost, int internalDimension, Character character = null, Group group = null)
        {
            BuildingData = new Building
            {
                CreatorsId = creatorId,
                InternalPickupPositionX = internalX,
                InternalPickupPositionY = internalY,
                InternalPickupPositionZ = internalZ,
                ExternalPickupPositionX = externalX,
                ExternalPickupPositionY = externalY,
                ExternalPickupPositionZ = externalZ,
                Cost = cost,
                InternalDimension = internalDimension,
                Character = character,
                Group = group
            };
            Initialize();
            ContextFactory.Instance.Buildings.Add(BuildingData);
            ContextFactory.Instance.SaveChanges();
        }

        public void Save()
        {
            ContextFactory.Instance.Buildings.Attach(BuildingData);
            ContextFactory.Instance.Entry(BuildingData).State = EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }

        private void Initialize()
        {
            InteriorDoorsColshape = API.shared.createCylinderColShape(new Vector3(BuildingData.InternalPickupPositionX, BuildingData.InternalPickupPositionY, BuildingData.InternalPickupPositionZ), 5, 5);

            var externalPosition = new Vector3(BuildingData.ExternalPickupPositionX,
                BuildingData.ExternalPickupPositionY, BuildingData.ExternalPickupPositionZ);

            ExteriorDoorsColshape = API.shared.createCylinderColShape(externalPosition, 5, 5);
            ExteriorDoorsColshape.dimension = BuildingData.InternalDimension;
            BuildingMarker = API.shared.createMarker(2, externalPosition, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f),
                new Vector3(1f, 1f, 1f), 255, 100, 100, 100);

            InteriorDoorsColshape.onEntityEnterColShape += (s, e) =>
            {
                if (API.shared.getEntityType(e).Equals(EntityType.Player))
                {
                    var player = API.shared.getPlayerFromHandle(e);
                    //args[0] jako true rysuje panel informacji
                    player.triggerEvent("DrawBuildingComponents", false);
                    player.SetData("CurrentDoors", this);
                    player.SetData("DoorsTarget", new Vector3(BuildingData.ExternalPickupPositionX, BuildingData.ExternalPickupPositionY, BuildingData.ExternalPickupPositionZ));
                }
            };

            InteriorDoorsColshape.onEntityExitColShape += (s, e) =>
            {
                if (API.shared.getEntityType(e).Equals(EntityType.Player))
                {
                    var player = API.shared.getPlayerFromHandle(e);
                    player.triggerEvent("DisposeBuildingComponents");
                    player.ResetData("CurrentDoors");
                    player.ResetData("DoorsTarget");
                }
            };

            ExteriorDoorsColshape.onEntityEnterColShape += (s, e) =>
            {
                if (API.shared.getEntityType(e).Equals(EntityType.Player))
                {
                    var player = API.shared.getPlayerFromHandle(e);
                    //args[0] jako true rysuje panel informacji
                    player.triggerEvent("DrawBuildingComponents", true);
                    player.SetData("CurrentDoors", this);
                    player.SetData("DoorsTarget", new Vector3(BuildingData.InternalPickupPositionX, BuildingData.InternalPickupPositionY, BuildingData.InternalPickupPositionZ));
                }
            };

            ExteriorDoorsColshape.onEntityExitColShape += (s, e) =>
            {
                if (API.shared.getEntityType(e).Equals(EntityType.Player))
                {
                    var player = API.shared.getPlayerFromHandle(e);
                    player.triggerEvent("DisposeBuildingComponents");
                    player.ResetData("CurrentDoors");
                    player.ResetData("DoorsTarget");
                }
            };

            if (BuildingData.Cost.HasValue)
            {
                BuildingLabel = API.shared.createTextLabel($"~g~NA SPRZEDAŻ\n~w~Cena: {BuildingData.Cost.Value}\n(( /kup ))", new Vector3(BuildingData.ExternalPickupPositionX, BuildingData.ExternalPickupPositionY, BuildingData.ExternalPickupPositionZ + 5), 10f, 1f, true);
            }
        }

        public void Buy(Client sender)
        {
            if (!BuildingData.Cost.HasValue)
            {
                sender.Notify("Ten budynek nie jest na sprzedaż");
                return;
            }

            if (!sender.HasMoney(BuildingData.Cost.Value))
            {
                sender.Notify("Nie posiadasz wystarczającej ilości gotówki");
                return;
            }

            API.shared.deleteEntity(BuildingLabel);
            BuildingLabel = null;

            sender.RemoveMoney(BuildingData.Cost.Value);
            sender.triggerEvent("ShowShard", "Zakupiono budynek");
            BuildingData.Character = sender.GetAccountController().CharacterController.Character;
            Save();
        }

        public void PutItemToBuilding(Client player, Item item)
        {
            BuildingData.Item.Add(item);
            Save();
        }

        public static void PassDoors(Client player)
        {
            if (!player.HasData("CurrentDoors") || !player.HasData("DoorsTarget")) return;

            BuildingController controller = (BuildingController)player.GetData("CurrentDoors");

            if (controller.DoorsLocked)
            {
                player.Notify("Drzwi są zamknięte.");
                return;
            }

            if (controller.PlayersInBuilding.Contains(player))
            {
                player.position = (Vector3)player.GetData("DoorsTarget");
                player.dimension = 0;
                controller.PlayersInBuilding.Remove(player);

                player.GetAccountController().CharacterController.CurrentBuilding = null;
            }
            else
            {
                player.position = (Vector3)player.GetData("DoorsTarget");
                player.dimension = ((BuildingController) player.GetData("CurrentDoors")).InteriorDoorsColshape
                    .dimension;
                controller.PlayersInBuilding.Add(player);
                player.GetAccountController().CharacterController.CurrentBuilding =
                    ((BuildingController) player.GetData("CurrentDoors"));
            }
        }

        public static void Knock(Client player)
        {
            if (!player.HasData("CurrentDoors")) return;
            RPChat.SendMessageToNearbyPlayers(player, "unosi dłoń i puka do drzwi budynku", ChatMessageType.ServerMe);
            RPChat.SendMessageToSpecifiedPlayers(player, ((BuildingController)player.GetData("CurrentDoors")).PlayersInBuilding, "Słychać pukanie do drzwi.", ChatMessageType.ServerDo);
        }

        public void Dispose()
        {
            API.shared.deleteColShape(InteriorDoorsColshape);
            API.shared.deleteColShape(ExteriorDoorsColshape);
            API.shared.deleteEntity(BuildingMarker);
            if (BuildingMarker != null) API.shared.deleteEntity(BuildingLabel);
        }
    }
}