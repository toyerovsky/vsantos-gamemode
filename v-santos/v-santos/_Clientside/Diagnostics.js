/* Copyright (C) Przemys³aw Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemys³aw Postrach <toyerek@gmail.com> July 2017
 */

var drawCords = false;

var resX = API.getScreenResolutionMaintainRatio().Width;
var resY = API.getScreenResolutionMaintainRatio().Height;

API.onServerEventTrigger.connect(function (eventName, args) {
    //args[0] czy schowac czy pokazac
    if (eventName === "ShowDiagnostics") {
        drawCords = !drawCords;
    }
});

API.onUpdate.connect(function () {
    if (drawCords)
    {
        API.drawText("~o~Pozycja: ~w~ X: " + API.getEntityPosition(API.getLocalPlayer()).X + " Y:" + API.getEntityPosition(API.getLocalPlayer()).Y + " Z:" + API.getEntityPosition(API.getLocalPlayer()).Z, 27, resY - 320, 0.5, 50, 211, 82, 255, 4, 0, true, true, 0);
        API.drawText("~g~Rotacja: ~w~ X: " + API.getEntityRotation(API.getLocalPlayer()).X + " Y:" + API.getEntityRotation(API.getLocalPlayer()).Y + " Z:" + API.getEntityRotation(API.getLocalPlayer()).Z, 27, resY - 285, 0.5, 50, 211, 82, 255, 4, 0, true, true, 0);
    }
});