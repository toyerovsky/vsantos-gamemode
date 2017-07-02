using System;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Core.Bus.Models;
using System.Timers;
using Serverside.Core.Extensions;

namespace Serverside.Core.Bus
{
    public class BusStop : IDisposable
    {
        public BusStopModel Data { get; set; }
        public SphereColShape Shape { get; set; }
        
        private API Api { get; }

        public BusStop(API api, BusStopModel data)
        {
            Api = api;
            Data = data;

            //Tworzymy napis na przystanku
            Api.createTextLabel($"~y~PRZYSTANEK~w~\n {data.Name}", Data.Center, 15f, 0.64f, true);
            
            Shape = Api.createSphereColShape(data.Center, 5f);
            Shape.onEntityEnterColShape += OnEntityEnterColshapeHandler;
            Shape.onEntityExitColShape += OnEntityExitColshapeHandler;
        }

        private void OnEntityEnterColshapeHandler(ColShape shape, NetHandle entity)
        {
            if (Api.getEntityType(entity) == EntityType.Player)
            {
                Api.getPlayerFromHandle(entity).SetData("Bus", this);
            }
        }

        private void OnEntityExitColshapeHandler(ColShape shape, NetHandle entity)
        {
            if (Api.getEntityType(entity) == EntityType.Player)
            {
                Api.getPlayerFromHandle(entity).ResetData("Bus");
            }
        }

        public static void StartTransport(Client player, decimal cost, int seconds, Vector3 position, string name)
        {
            //TODO: Przez pierwsze 5h autobus za darmo dla nowych graczy
            if (!player.HasMoney(cost))
            {
                player.Notify("Nie posiadasz wystarczającej ilości gotówki.");
                return;
            }

            player.RemoveMoney(cost);

            API.shared.sendNativeToPlayer(player, Hash.DO_SCREEN_FADE_OUT, 2000); //Zaciemnianie ekranu
            RPChat.SendMessageToNearbyPlayers(player, $"wsiadł do autobusu zmierzającego w stronę {name}", ChatMessageType.ServerMe);

            player.dimension = 666; //Nie używać tego wymiaru jest zajęty na autobusy

            //Teleport po danym czasie
            Timer busTimer = new Timer(seconds * 1000);
            busTimer.Start();
            busTimer.Elapsed += (sender, args) =>
            {
                player.position = position;
                API.shared.sendNativeToPlayer(player, Hash.DO_SCREEN_FADE_IN, 2000); //Odciemnianie ekranu
                player.dimension = 0;
                RPChat.SendMessageToNearbyPlayers(player, "wysiadł z autobusu", ChatMessageType.ServerMe);

                busTimer.Stop();
                busTimer.Dispose();
            };

            //TODO: Nie działa - Wyświatla się pod zaciemnionym ekranem
            //Timer shardTimer = new Timer(1000);
            //shardTimer.Start();

            ////Odliczanie czasu do przyjazdu
            //shardTimer.Elapsed += (sender, args) =>
            //{
            //    if (DateTime.Now == arrivalTime)
            //    {
            //        shardTimer.Stop();
            //        shardTimer.Dispose();
            //    }

            //    string secondsToShow = (arrivalTime - DateTime.Now).Seconds.ToString();
            //    if (secondsToShow.Length == 1) secondsToShow = "0" + secondsToShow;
            //    player.triggerEvent("BWTimerTick",
            //        $"{(arrivalTime - DateTime.Now).Minutes}:{secondsToShow}");
            //};
        }

        public void Dispose()
        {
            Shape.onEntityEnterColShape -= OnEntityEnterColshapeHandler;
            Shape.onEntityExitColShape -= OnEntityExitColshapeHandler;
            Api.deleteColShape(Shape);
        }
    }
}
