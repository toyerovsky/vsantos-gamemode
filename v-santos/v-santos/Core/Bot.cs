using System;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;


namespace Serverside.Core
{
    public class Bot : IDisposable
    {
        public string Name { get; set; }
        public PedHash PedHash { get; set; }
        private API Api { get; }

        protected TextLabel NameLabel { get; set; }
        //Jest to tylko i wyłącznie pozycja spawnu, potem korzystamy z BotHandle.position
        protected FullPosition SpawnPosition { get; set; }
        protected Ped BotHandle { get; set; }

        public Bot(API api, string name, PedHash pedHash, FullPosition spawnPosition)
        {
            Api = api;
            Name = name;
            PedHash = pedHash;
            SpawnPosition = spawnPosition;
        }

        public void GoToPoint(Vector3 position)
        {
            API.shared.sendNativeToPlayersInRange(BotHandle.position, 100, Hash.TASK_GO_STRAIGHT_TO_COORD, BotHandle.handle, position.X, position.Y, position.Z, 2, -1);

            //Zakładamy, że bot idzie z predkoscia 1.20 j/s
            BotHandle.movePosition(position, (int)(BotHandle.position.DistanceTo(position)/1.20) * 1000);
        }

        /// <summary>
        /// Metoda do incjalizacji bota w grze
        /// </summary>
        public virtual void Intialize()
        {
            BotHandle = Api.createPed(PedHash, SpawnPosition.Position, 1f);
            Api.setEntityRotation(BotHandle, SpawnPosition.Rotation);
            //Api.setEntityInvincible(BotHandle, true);
            //BotHandle.freezePosition = false;
            NameLabel = Api.createTextLabel(Name,
                new Vector3(SpawnPosition.Position.X, SpawnPosition.Position.Y, SpawnPosition.Position.Z + 1), 10f, 0.7f, true);
            NameLabel.attachTo(BotHandle, "SKEL_Head", new Vector3(0f, 0f, 1f), BotHandle.rotation);
        }

        protected void SendMessageToNerbyPlayers(string message, ChatMessageType chatMessageType)
        {
            var players = Api.getPlayersInRadiusOfPosition((int)chatMessageType, BotHandle.position);

            string color = null;

            if (chatMessageType == ChatMessageType.Normal)
            {
                message = Name + " mówi: " + message;
                color = "~#FFFFFF~";
            }
            else if (chatMessageType == ChatMessageType.Quiet)
            {
                message = Name + " szepcze: " + message;
                color = "~#FFFFFF~";
            }
            else if (chatMessageType == ChatMessageType.Loud)
            {
                message = Name + " krzyczy: " + message + "!";
                color = "~#FFFFFF~";
            }
            else if (chatMessageType == ChatMessageType.Me)
            {
                message = "** " + Name + " " + message;
                color = "~#C2A2DA~";
            }
            else if (chatMessageType == ChatMessageType.ServerMe)
            {
                message = "* " + Name + " " + message;
                color = "~#C2A2DA~";
            }
            else if (chatMessageType == ChatMessageType.Do)
            {
                message = "** " + message + " (( " + Name + " )) **";
                color = "~#847DB7~";
            }

            foreach (var player in players)
            {
                Api.sendChatMessageToPlayer(player, color, message);
            }
        }

        public virtual void Dispose()
        {
            Api.deleteEntity(BotHandle);
            Api.deleteEntity(NameLabel);
        }
    }
}
