/* Copyright (C) Przemys³aw Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemys³aw Postrach <toyerek@gmail.com> July 2017
 */

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

