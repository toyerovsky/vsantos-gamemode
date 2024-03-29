/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

var menuPool = null;

var crimeBotBlip;
var addingCrimeBotMarker;

API.onServerEventTrigger.connect(function (eventName, args)
{
    if (eventName === "OnPlayerCalledCrimeBot")
    {
        menuPool = API.getMenuPool();
        var menu = API.createMenu("", 0, 0, 6);
        API.setMenuTitle(menu, "Dzielnice");
        API.setMenuBannerRectangle(menu, 100, 0, 255, 0);

        var districts = args[0];

        var districtMenuItem = {};
        for (var i = 0; i < districts.Count; i++) 
        {
            districtMenuItem = API.createMenuItem(districts[i], "");
            menu.AddItem(districtMenuItem);
        }

        menuPool.Add(menu);
        menu.Visible = true;

        menu.OnItemSelect.connect(function (sender, item, index)
        {
            API.triggerServerEvent("OnPlayerSelectedCrimeBotDiscrict", index);
            menu.Visible = false;
        });
    }
    
    else if (eventName === "DrawCrimeBotComponents")
    {
        crimeBotBlip = API.createBlip(args[0]);
        API.setBlipSprite(crimeBotBlip, args[1]);
        API.setBlipColor(crimeBotBlip, args[2]);
    }
    else if (eventName === "DisposeCrimeBotComponents")
    {
        API.deleteEntity(crimeBotBlip);
        resource.Cef.CloseCef();
    }
    else if (eventName === "DrawAddingCrimeBotComponents") {
        addingCrimeBotMarker = API.createMarker(1,
            args[0],
            new Vector3(),
            new Vector3(),
            new Vector3(1, 1, 1),
            255,
            0,
            0,
            100);
    }
    else if (eventName === "DisposeAddingCrimeBotComponents")
    {
        API.deleteEntity(addingCrimeBotMarker);
    }
});

API.onUpdate.connect(function ()
{
    if (menuPool != null)
    {
        menuPool.ProcessMenus();
    }
});