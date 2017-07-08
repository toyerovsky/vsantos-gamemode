var drawStreetHUD = false;
let player = API.getLocalPlayer();
let res = API.getScreenResolutionMaintainRatio();

API.onKeyDown.connect(function (sender, e)
{
    if (e.KeyCode === Keys.F12)
    {
        API.setHudVisible(!API.getHudVisible());
    }
    else if (e.KeyCode === Keys.F11)
    {
        API.setChatVisible(!API.getChatVisible());
    }
});

API.onServerEventTrigger.connect(function (eventName, args)
{
    //args[0] czy schowac czy pokazac
    if (eventName === "ToggleHud")
    {
        API.setHudVisible(args[0]);
        drawStreetHUD = args[0];
    }
    else if (eventName === "ToggleChat")
    {
        API.setChatVisible(args[0]);
    }
    else if (eventName === "ShowShard") {
        API.showShard(args[0], args[1]);
    }
    else if (eventName === "ResetCamera") {
        API.setActiveCamera(null);
    }
    else if (eventName === "GetWaypointVector") {
        if (API.isWaypointSet()) {
            var pos = API.getWaypointPosition();
            API.triggerServerEvent("InvokeWaypointVector", pos.X, pos.Y, API.getGroundHeight(pos));
        } 
    }
});

API.onUpdate.connect(() =>
{
    if (drawStreetHUD)
    {
        var pos = API.getEntityPosition(player);
        var streetname = API.getStreetName(pos);
        var zoneNameLabel = API.getZoneNameLabel(pos);
        var zoneName = API.getZoneName(pos);
        API.drawText(`~y~${streetname}`, (99 * res.Width) / 100, (99 * res.Height) / 100 - 80, 0.5, 255, 255, 255, 255, 1, 2, false, true, 0);
        API.drawText(`${zoneName} (${zoneNameLabel})`, (99 * res.Width) / 100, (99 * res.Height) / 100 - 40, 0.4, 255, 255, 255, 255, 1, 2, false, true, 0);
    }
});