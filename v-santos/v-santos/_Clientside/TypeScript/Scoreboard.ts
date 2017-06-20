/// <reference path="../../types-gtanetwork/index.d.ts" />

class PlayerInfo {
    socialClubName: string = "";
    name: string = "";
    ping: number = 0;
    color: number[] = [255, 255, 255];
}
//
var gCurrentState = 0;
var gCurrentPage = 0;
var gStateSet = 0;
var gPlayers: PlayerInfo[] = [];

function getPlayer(socialClubName: string): PlayerInfo {
    for (var i = 0; i < gPlayers.length; i++) {
        if (gPlayers[i].socialClubName == socialClubName) {
            return gPlayers[i];
        }
    }
    return null;
}

API.onServerEventTrigger.connect((name, args) => {
    var newPlayer: PlayerInfo;
    if (name == "playerlist") {
        gPlayers = [];

        var list = args[0];
        for (var i = 0; i < list.Count; i++) {
            var obj = JSON.parse(list[i]);
            newPlayer = new PlayerInfo();
            newPlayer.socialClubName = obj.socialClubName;
            newPlayer.name = obj.name;
            newPlayer.color = [255, 255, 255];
            gPlayers.push(newPlayer);
        }

    } else if (name == "playerlist_join") {
        // This can happen in certain situations, so we handle this as an update
        var existingPlayer = getPlayer(args[0]);
        if (existingPlayer != null) {
            existingPlayer.socialClubName = args[0];
            existingPlayer.name = args[1];
            existingPlayer.color = [255, 255, 255];
        } else {
            newPlayer = new PlayerInfo();
            newPlayer.socialClubName = args[0];
            newPlayer.name = args[1];
            newPlayer.color = [255, 255, 255];
            gPlayers.push(newPlayer);
        }

    } else if (name == "playerlist_leave") {
        for (var i = 0; i < gPlayers.length; i++) {
            if (gPlayers[i].socialClubName == args[0]) {
                gPlayers.splice(i, 1);
            }
        }

    } else if (name == "playerlist_pings") {
        var list = args[0];
        for (var i = 0; i < list.Count; i++) {
            var obj = JSON.parse(list[i]);
            var player = getPlayer(obj.socialClubName);
            if (player != null) {
                player.ping = obj.ping;
            }
        }

    } else if (name == "playerlist_changednames") {
        var list = args[0];
        for (var i = 0; i < list.Count; i++) {
            var obj = JSON.parse(list[i]);
            var player = getPlayer(obj.socialClubName);
            if (player != null) {
                player.name = obj.newName;
            }
        }
    }
});

API.onUpdate.connect(() => {
    // MultiplayerInfo
    if (!API.isChatOpen() && API.isControlJustPressed(Enums.Controls.MultiplayerInfo)) {
        gCurrentState++;
        gStateSet = API.getGameTime();

        API.triggerServerEvent("playerlist_pings");

        if (gCurrentState == 1) {
            gCurrentPage = 0;
            return;
        }

        if (gCurrentState == 2) {
            API.callNative("_SET_RADAR_BIGMAP_ENABLED", true, false);
        }

        if (gCurrentState == 3) {
            gCurrentState = 0;
            API.callNative("_SET_RADAR_BIGMAP_ENABLED", false, false);
        }
    }

    if (gCurrentState > 0 && API.getGameTime() - gStateSet > 3000) {
        gCurrentState = 0;
        API.callNative("_SET_RADAR_BIGMAP_ENABLED", false, false);
    }

    if (gCurrentState == 1) {
        // Get list data
        var resolution = API.getScreenResolutionMantainRatio();

        var listPadding = 4;
        var listLine = 28;
        var listBorder = 2;
        var listItemsPerPage = Math.floor((resolution.Height * 0.8) / listLine);
        var listPages = Math.ceil(gPlayers.length / listItemsPerPage);

        // Page navigation is here for now because the onKeyDown method doesn't work
        if (listPages > 1) {
            if (API.isControlJustPressed(Enums.Controls.PhoneUp)) {
                gStateSet = API.getGameTime();
                if (--gCurrentPage < 0) {
                    gCurrentPage = listPages - 1;
                }
            } else if (API.isControlJustPressed(Enums.Controls.PhoneDown)) {
                gStateSet = API.getGameTime();
                if (++gCurrentPage >= listPages) {
                    gCurrentPage = 0;
                }
            }
        }

        var listPageStart = gCurrentPage * listItemsPerPage;
        var listPageCount = Math.min(gPlayers.length - listPageStart, listItemsPerPage);

        var listWidth = resolution.Width * 0.4;
        var listHeight = (Math.min(gPlayers.length, listPageCount) + 1) * 28 + listPadding * 2;
        var listX = resolution.Width / 2 - listWidth / 2;
        var listY = Math.max(30, resolution.Height * 0.3 - listHeight / 2);

        // Fill
        API.drawRectangle(listX, listY, listWidth, listHeight, 0, 0, 0, 220);

        // Separator
        API.drawRectangle(listX, listY + listLine, listWidth, listBorder, 100, 100, 100, 220);

        // Left
        API.drawRectangle(listX - listBorder, listY - listBorder, listBorder, listHeight + listBorder * 2, 91, 131, 40, 220);
        // Right
        API.drawRectangle(listX + listWidth, listY - listBorder, listBorder, listHeight + listBorder * 2, 91, 131, 40, 220);
        // Top
        API.drawRectangle(listX, listY - listBorder, listWidth, listBorder, 91, 131, 40, 220);
        // Bottom
        API.drawRectangle(listX, listY + listHeight, listWidth, listBorder, 91, 131, 40, 220);

        // Page indicators
        if (gCurrentPage > 0) {
            API.drawText("3", listX + listWidth / 2, listY + listLine, 0.5, 255, 255, 255, 255, 3, 1, true, true, 0);
        }
        if (gCurrentPage < listPages - 1) {
            API.drawText("4", listX + listWidth / 2, listY + listHeight - listLine * 2, 0.5, 255, 255, 255, 255, 3, 1, true, true, 0);
        }

        // Header
        API.drawText("ID Nazwa", listX + listPadding, listY - listPadding / 2, 0.4, 91, 131, 40, 255, 0, 0, true, true, 0);
        API.drawText("Ping", listX + listWidth - listPadding, listY - listPadding / 2, 0.4, 91, 131, 40, 255, 0, 2, true, true, 0);

        // Players
        for (var i = 0; i < listPageCount; i++) {
            var player = gPlayers[listPageStart + i];
            var color = player.color;
            API.drawText(player.name, listX + listPadding, listY + listPadding / 2 + listLine * (i + 1), 0.45, color[0], color[1], color[2], 255, 4, 0, true, true, 0);
            API.drawText("" + player.ping, listX + listWidth - listPadding, listY + listPadding / 2 + listLine * (i + 1), 0.45, 100, 100, 100, 255, 4, 2, true, true, 0);
        }
    }
});
