﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

        public List<Client> PlayersInBuilding { get; set; } = new List<Client>();
        public bool DoorsLocked { get; set; } = true;

        //Ładowanie budynku
        public BuildingController(Building data)
        {
            BuildingData = data;
            Initialize();
        }

        //Tworzenie nowego budynku
        public BuildingController(string name, string description, long creatorId, float internalX, float internalY, float internalZ, float externalX, float externalY, float externalZ, decimal cost, int internalDimension, Character character = null, Group group = null)
        {
            BuildingData = new Building
            {
                Name = name,
                Description = description,
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

            //Jeśli budynek jest na sprzedaż marker jest zielony jeśli nie żółty
            
            BuildingMarker = API.shared.createMarker(2, externalPosition, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f),
                new Vector3(1f, 1f, 1f), 255, BuildingData.Cost.HasValue ? 0 : 255, 255, 0);

            InteriorDoorsColshape.onEntityEnterColShape += (s, e) =>
            {
                if (API.shared.getEntityType(e).Equals(EntityType.Player))
                {
                    var player = API.shared.getPlayerFromHandle(e);
                    //args[0] jako true określa że gracz jest na zewnątrz budynku
                    //args[1] to informacje o budynku
                    player.triggerEvent("DrawBuildingComponents", false);
                    player.SetData("CurrentDoors", this);

                    //Doors target jest potrzebne ponieważ Colshape nie ma pola position... dodatkowo przydaje się aby sprawdzać po stronie klienta czy
                    //Gracz jest obecnie w zasięgu jakichś drzwi

                    player.SetSyncedData("DoorsTarget", new Vector3(BuildingData.ExternalPickupPositionX, BuildingData.ExternalPickupPositionY, BuildingData.ExternalPickupPositionZ));
                }
            };

            InteriorDoorsColshape.onEntityExitColShape += (s, e) =>
            {
                if (API.shared.getEntityType(e).Equals(EntityType.Player))
                {
                    var player = API.shared.getPlayerFromHandle(e);
                    player.triggerEvent("DisposeBuildingComponents");
                    player.ResetData("CurrentDoors");
                    player.ResetSyncedData("DoorsTarget");
                }
            };

            ExteriorDoorsColshape.onEntityEnterColShape += (s, e) =>
            {
                //Jeśli podchodzi od zewnątrz rysujemy panel informacji
                if (API.shared.getEntityType(e).Equals(EntityType.Player))
                {
                    var player = API.shared.getPlayerFromHandle(e);
                    //args[0] jako true rysuje panel informacji
                    player.triggerEvent("DrawBuildingComponents", true, new List<string>
                    {
                        this.BuildingData.Name,
                        this.BuildingData.Description,
                        this.BuildingData.EnterCharge.HasValue ? BuildingData.EnterCharge.ToString() : "",
                        this.BuildingData.Cost.HasValue ? BuildingData.Cost.ToString() : ""
                    });

                    player.SetData("CurrentDoors", this);
                    player.SetSyncedData("DoorsTarget", new Vector3(BuildingData.InternalPickupPositionX, BuildingData.InternalPickupPositionY, BuildingData.InternalPickupPositionZ));
                }
            };

            ExteriorDoorsColshape.onEntityExitColShape += (s, e) =>
            {
                if (API.shared.getEntityType(e).Equals(EntityType.Player))
                {
                    var player = API.shared.getPlayerFromHandle(e);
                    player.triggerEvent("DisposeBuildingComponents");
                    player.ResetData("CurrentDoors");
                    player.ResetSyncedData("DoorsTarget");
                }
            };
        }

        public static int GetNextFreeDimension()
        {
            int last = ContextFactory.Instance.Buildings.OrderByDescending(x => x.InternalDimension).Select(x => x.InternalDimension).First();
            if (last == 2137 || last == 666) last++;
            return last;
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

            BuildingMarker.color = new GTANetworkServer.Constant.Color(255, 255, 0);

            sender.RemoveMoney(BuildingData.Cost.Value);
            sender.triggerEvent("ShowShard", "Zakupiono budynek", 5000);
            API.shared.playSoundFrontEnd(sender, "BASE_JUMP_PASSED", "HUD_AWARDS");

            BuildingData.Character = sender.GetAccountController().CharacterController.Character;
            BuildingData.Cost = null;
            Save();
        }

        public void PutItemToBuilding(Client player, Item item)
        {
            BuildingData.Item.Add(item);
            Save();
        }

        public static void PassDoors(Client player)
        {
            if (!player.HasData("CurrentDoors") || !player.HasSyncedData("DoorsTarget")) return;

            BuildingController controller = (BuildingController)player.GetData("CurrentDoors");

            if (controller.DoorsLocked)
            {
                player.Notify("Drzwi są zamknięte.");
                return;
            }

            if (controller.BuildingData.EnterCharge.HasValue &&
                !player.HasMoney(controller.BuildingData.EnterCharge.Value))
            {
                player.Notify("Nie posiadasz wystarczającej ilości gotówki");
                return;
            }

            if (controller.BuildingData.EnterCharge.HasValue) player.RemoveMoney(controller.BuildingData.EnterCharge.Value);

            if (controller.PlayersInBuilding.Contains(player))
            {
                player.position = (Vector3)player.GetSyncedData("DoorsTarget");
                player.dimension = 0;
                controller.PlayersInBuilding.Remove(player);

                player.GetAccountController().CharacterController.CurrentBuilding = null;
            }
            else
            {
                player.position = (Vector3)player.GetSyncedData("DoorsTarget");
                player.dimension = ((BuildingController) player.GetData("CurrentDoors")).InteriorDoorsColshape
                    .dimension;
                controller.PlayersInBuilding.Add(player);
                player.GetAccountController().CharacterController.CurrentBuilding = player.GetData("CurrentDoors");
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
        }
    }
}