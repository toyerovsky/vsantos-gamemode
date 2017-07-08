using System;
using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using Serverside.Controllers;

namespace Serverside.Core
{
    public class RPScoreBoard : Script
    {
        //By MissMelisa
        private DateTime _mLastTick = DateTime.Now;

        public RPScoreBoard()
        {
            API.onPlayerDisconnected += API_onPlayerDisconnected;
            API.onPlayerFinishedDownload += API_onPlayerFinishedDownload;
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onUpdate += API_onUpdate;
            CharacterController.OnPlayerCharacterLogin += RPLogin_OnPlayerLogin;
        }

        private void RPLogin_OnPlayerLogin(Client sender, CharacterController character)
        {
            API.triggerClientEventForAll("playerlist_join", sender.socialClubName, character.FormatName);
        }

        private void API_onPlayerDisconnected(Client player, string reason)
        {
            API.triggerClientEventForAll("playerlist_leave", player.socialClubName);
        }

        private void API_onPlayerFinishedDownload(Client player)
        {
            var list = new List<string>();
            foreach (var ply in RPEntityManager.GetAccounts())
            {
                var dic = new Dictionary<string, object>
                {
                    {"socialClubName", ply.Value.Client.socialClubName},
                    {"serverId", ply.Value.ServerId},
                    {"name", ply.Value.Client.name},
                    {"ping", ply.Value.Client.ping}
                };
                list.Add(API.toJson(dic));
            }

            API.triggerClientEvent(player, "playerlist", list);
        }


        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "playerlist_pings")
            {
                var list = new List<string>();
                foreach (var ply in RPEntityManager.GetAccounts())
                {
                    var dic = new Dictionary<string, object>
                    {
                        {"socialClubName", ply.Value.Client.socialClubName},
                        {"serverId", ply.Value.ServerId},
                        {"ping", ply.Value.Client.ping}
                    };
                    list.Add(API.toJson(dic));
                }
                API.triggerClientEvent(sender, "playerlist_pings", list);
            }
        }

        private void API_onUpdate()
        {
            if ((DateTime.Now - _mLastTick).TotalMilliseconds >= 1000)
            {
                _mLastTick = DateTime.Now;

                var changedNames = new List<string>();
                var players = API.getAllPlayers();
                foreach (var player in players)
                {
                    string lastName = player.getData("playerlist_lastname");

                    if (lastName == null)
                    {
                        player.setData("playerlist_lastname", player.name);
                        continue;
                    }

                    if (lastName != player.name)
                    {
                        player.setData("playerlist_lastname", player.name);

                        var dic = new Dictionary<string, object>
                        {
                            {"socialClubName", player.socialClubName},
                            {"newName", player.name}
                        };
                        changedNames.Add(API.toJson(dic));
                    }
                }

                if (changedNames.Count > 0)
                {
                    API.triggerClientEventForAll("playerlist_changednames", changedNames);
                }
            }
        }
    }
}
