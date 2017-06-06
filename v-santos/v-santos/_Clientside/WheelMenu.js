let enableWheelMenu = true;

API.onKeyDown.connect(function (sender, e)
{
    if (e.KeyCode === Keys.F1 && enableWheelMenu)
    {
        var raycast = API.createRaycast(API.getEntityPosition(API.getLocalPlayer()), API.getPlayerAimCoords(API.getLocalPlayer()), 10 | 12, API.getLocalPlayer());
        if (raycast.didHitEntity)
        {
            API.triggerServerEvent("RequestWheelMenu", raycast.hitEntity);
        }
    }
});

