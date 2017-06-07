using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Controllers;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Core.Finders;
using Serverside.Corners.Models;

namespace Serverside.Corners
{
    public class Corner : IDisposable
    {
        public List<CornerBotModel> CornerBots { get; set; }
        public FullPosition Position { get; set; }
        public List<FullPosition> BotPositions { get; set; }

        public Marker Marker { get; set; }
        public ColShape Shape { get; set; }
        private API Api => API.shared;
        private bool CornerBusy { get; set; }
        private AccountController Player { get; set; }
        private CornerBot CurrentBot { get; set; }

        public Corner(CornerModel corner)
        {
            CornerBots = corner.CornerBots;
            Position = corner.Position;
            BotPositions = corner.BotPositions;

            Marker = Api.createMarker(1, Position.Position, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f), 100,
                100, 100, 100);
            Marker.invincible = true;

            Shape = Api.createCylinderColShape(Position.Position, 1f, 2f);

            Shape.onEntityEnterColShape += OnEntityEnterColShapeHandler;
            Shape.onEntityExitColShape += OnEntityExitColShapeHandler;
        }
        
        private void OnEntityEnterColShapeHandler(ColShape shape, NetHandle entity)
        {
            if (DateTime.Now.Hour < 16 || DateTime.Now.Hour > 23)
            {
                API.shared.sendNotificationToPlayer(API.shared.getPlayerFromHandle(entity), "Handel na rogu możliwy jest od 16 do 23.", true);
                return;
            }

            if (!CornerBusy && shape == Shape && Api.getEntityType(entity) == EntityType.Player && API.shared.hasEntityData(entity, "CharacterID"))
            {
                Player = PlayerFinder.FindPlayerByCharacterId(API.shared.getEntityData(entity, "CharacterID"));
                CornerBusy = true;

                Player.Client.Notify("Rozpocząłeś proces sprzedaży, pozostań w znaczniku.", true);

                StartProcess();
                Player.Client.playAnimation("amb@world_human_drug_dealer_hard@male@idle_a", "idle_a", (int)EnumsExtensions.AnimationFlags.Loop | (int)EnumsExtensions.AnimationFlags.AllowPlayerControl | (int) EnumsExtensions.AnimationFlags.Cancellable);
            }
        }

        private void StartProcess()
        {
            //Przychodzące boty
            Timer timer = new Timer(GetRandomTime() * 1000);
            timer.Start();

            timer.Elapsed += (sender, args) =>
            {
                timer.Stop();
                CurrentBot = new CornerBot(new API(), CornerBots[GetRandomBot()].Name, CornerBots[GetRandomBot()].PedHash, BotPositions[0], BotPositions.Where(x => x != BotPositions[0]).ToList(), CornerBots[GetRandomBot()].DrugType, CornerBots[GetRandomBot()].MoneyCount, CornerBots[GetRandomBot()].Greeting, CornerBots[GetRandomBot()].GoodFarewell, CornerBots[GetRandomBot()].BadFarewell, Player, CornerBots[GetRandomBot()].BotId);
                CurrentBot.Intialize();
                CurrentBot.OnTransactionEnd += (o, eventArgs) =>
                {
                    timer.Start();
                    CurrentBot.Dispose();
                };
            };

            Shape.onEntityExitColShape += (shape, entity) =>
            {
                if (CornerBusy && shape == Shape && Api.getEntityType(entity) == EntityType.Player &&
                    entity == Player.Client)
                {
                    timer.Dispose();
                    Player = null;
                }
            };
        }

        public int GetRandomBot()
        {
            return new Random().Next(CornerBots.Count);
        }

        public int GetRandomTime()
        {
            return new Random().Next(90, 180);
        }

        private void OnEntityExitColShapeHandler(ColShape shape, NetHandle entity)
        {
            if (CornerBusy && shape == Shape && Api.getEntityType(entity) == EntityType.Player && entity == Player.Client)
            {
                CornerBusy = false;
                Player.Client.stopAnimation();
                
                if (CurrentBot != null)
                {
                    CurrentBot.Dispose();
                    CurrentBot = null;
                }                
            }
        }
        
        public void Dispose()
        {
            Shape.onEntityEnterColShape -= OnEntityEnterColShapeHandler;
            Shape.onEntityExitColShape -= OnEntityExitColShapeHandler;
        }
    }
}