/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using System.Linq;
using System.Timers;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Serverside.Controllers;
using Serverside.Core.Extensions;
using Serverside.Corners.Models;

namespace Serverside.Corners
{
    public class Corner : IDisposable
    {
        public CornerModel Data { get; set; }

        public Marker Marker { get; set; }
        public ColShape Shape { get; set; }
        private ServerAPI Api => API.shared;
        private bool CornerBusy { get; set; }
        private AccountController Player { get; set; }
        private CornerBot CurrentBot { get; set; }

        public Corner(CornerModel corner)
        {
            Data = corner;

            Marker = Api.createMarker(1, Data.Position.Position, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f), 100,
                100, 100, 100);
            Marker.invincible = true;

            Shape = Api.createCylinderColShape(Data.Position.Position, 1f, 2f);

            Shape.onEntityEnterColShape += OnEntityEnterColShapeHandler;
        }
        
        private void OnEntityEnterColShapeHandler(ColShape shape, NetHandle entity)
        {
            if (DateTime.Now.Hour < 16 || DateTime.Now.Hour > 23)
            {
                API.shared.sendNotificationToPlayer(API.shared.getPlayerFromHandle(entity), "Handel na rogu możliwy jest od 16 do 23.", true);
                return;
            }

            if (!CornerBusy && shape == Shape && Api.getEntityType(entity) == EntityType.Player)
            {
                Player = Api.getPlayerFromHandle(entity).GetAccountController();

                CornerBusy = true;

                Player.Client.Notify("Rozpocząłeś proces sprzedaży, pozostań w znaczniku.", true);

                StartProcess();
                Player.Client.playAnimation("amb@world_human_drug_dealer_hard@male@idle_a", "idle_a", (int)AnimationFlags.Loop | (int)AnimationFlags.AllowPlayerControl | (int) AnimationFlags.Cancellable);
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

                var random = new Random().Next(Data.CornerBots.Count);

                CurrentBot = new CornerBot(
                    new API(), Data.CornerBots[random].Name, Data.CornerBots[random].PedHash, Data.BotPositions[0], Data.BotPositions.Where(x => x != Data.BotPositions[0]).ToList(), Data.CornerBots[random].DrugType, Data.CornerBots[random].MoneyCount, Data.CornerBots[random].Greeting, Data.CornerBots[random].GoodFarewell, Data.CornerBots[random].BadFarewell, Player, Data.CornerBots[random].BotId);

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
                    entity == Player.Client.handle)
                {
                    timer.Dispose();

                    CornerBusy = false;
                    Player.Client.stopAnimation();

                    if (CurrentBot != null)
                    {
                        CurrentBot.Dispose();
                        CurrentBot = null;
                    }

                    Player = null;
                }
            };
        }

        private int GetRandomTime() => new Random().Next(90, 180);
        
        public void Dispose()
        {
            Shape.onEntityEnterColShape -= OnEntityEnterColShapeHandler;
            Api.deleteColShape(Shape);
            Api.deleteEntity(Marker);
        }
    }
}