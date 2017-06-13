let wheelMenuFlag = false;

API.onKeyDown.connect(function (sender, e)
{
    if (e.KeyCode === Keys.F1 && wheelMenuFlag)
    {
        var raycast = API.createRaycast(API.getEntityPosition(API.getLocalPlayer()), API.getPlayerAimCoords(API.getLocalPlayer()), 10 | 12, API.getLocalPlayer());
        if (raycast.didHitEntity)
        {
            API.triggerServerEvent("RequestWheelMenu", raycast.hitEntity);
        }
    }
});

API.onServerEventTrigger.connect((eventName, args) => {
    if (eventName == "ToggleHud") wheelMenuFlag = !wheelMenuFlag;
});

