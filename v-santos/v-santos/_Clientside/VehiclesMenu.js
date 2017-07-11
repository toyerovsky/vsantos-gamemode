var menuPool = null;

let vehicleBlip = null;

API.onServerEventTrigger.connect(function (eventName, args)
{
    if (eventName === "OnPlayerShowVehicles")
    {
        menuPool = API.getMenuPool();
        var vehiclesMenu = API.createMenu("", 0, 0, 6);
        API.setMenuTitle(vehiclesMenu, "Pojazdy");
        API.setMenuBannerRectangle(vehiclesMenu, 100, 106, 154, 40);

        var vehicleInfoMenu = API.createMenu("", 0, 0, 6);
        API.setMenuTitle(vehicleInfoMenu, "Pojazdy");
        API.setMenuBannerRectangle(vehicleInfoMenu, 100, 0, 255, 0);

        var vehicles = JSON.parse(args[0]);
        
        for (var i = 0; i < vehicles.length; i++)
        {
            var vehicleMenuItem = API.createMenuItem(vehicles[i]["Name"], "");
            vehiclesMenu.AddItem(vehicleMenuItem);
            vehiclesMenu.BindMenuToItem(vehicleInfoMenu, vehicleMenuItem);
        }
        
        var spawnMenuItem = API.createMenuItem("Spawn/Unspawn", "");
        vehicleInfoMenu.AddItem(spawnMenuItem);
        var infoVehicleMenuItem = API.createMenuItem("Informacje", "");
        vehicleInfoMenu.AddItem(infoVehicleMenuItem);

        menuPool.Add(vehiclesMenu);
        menuPool.Add(vehicleInfoMenu);

        vehiclesMenu.Visible = true;

        vehiclesMenu.OnItemSelect.connect(function (sender, item, index)
        {
            API.triggerServerEvent("OnPlayerSelectedVehicle", vehicles[index]["Id"]);
        });

        vehicleInfoMenu.OnItemSelect.connect(function (sender, item, index)
        {
            if (index == 0)
            {
                API.triggerServerEvent("OnPlayerSpawnVehicle");
            }
            else if (index == 1)
            {
                API.triggerServerEvent("OnPlayerInformationsVehicle");
            }
            vehicleInfoMenu.Visible = false;
         });
    }
    else if (eventName === "OnPlayerManageVehicle")
    {
        menuPool = API.getMenuPool();
        var vehicleMenu = API.createMenu("", 0, 0, 6);
        API.setMenuTitle(vehicleMenu, "Pojazdy");
        API.setMenuBannerRectangle(vehicleMenu, 100, 0, 255, 0);

        var tuningMenu = API.createMenu("", 0, 0, 6);
        API.setMenuTitle(tuningMenu, "Tuning");
        API.setMenuBannerRectangle(tuningMenu, 100, 0, 255, 0);

        var tuningList = JSON.parse(args[0]);

            for (var j = 0; j < tuningList.length; j++)
            {
                var tuningListMenuItem = API.createMenuItem(tuningList[j]["Name"], "Kliknij enter aby wymontowa� tuning.");
                tuningMenu.AddItem(tuningListMenuItem);
            }
        
        var engineStateMenuItem = API.createMenuItem("Zgas/Odpal silnik", "");
        vehicleMenu.AddItem(engineStateMenuItem);
        var changeLockMenuItem = API.createMenuItem("Otworz/Zamknij", "");
        vehicleMenu.AddItem(changeLockMenuItem);
        var infoMenuItem = API.createMenuItem("Informacje", "");
        vehicleMenu.AddItem(infoMenuItem);
        var parkMenuItem = API.createMenuItem("Zaparkuj tutaj", "");
        vehicleMenu.AddItem(parkMenuItem);
        var tuningMenuItem = API.createMenuItem("Tuning", "");
        vehicleMenu.AddItem(tuningMenuItem);

        vehicleMenu.BindMenuToItem(tuningMenu, tuningMenuItem);

        menuPool.Add(vehicleMenu);
        menuPool.Add(tuningMenu);
        vehicleMenu.Visible = true;

        vehicleMenu.OnItemSelect.connect(function (sender, item, index)
        {
            if (index == 0)
            {
                API.triggerServerEvent("OnPlayerEngineStateChangeVehicle");
            }
            else if (index == 1)
            {
                API.triggerServerEvent("OnPlayerChangeLockVehicle");
            }
            else if (index == 2)
            {
                API.triggerServerEvent("OnPlayerInformationsInVehicle");
            }
            else if(index == 3)
            {
                API.triggerServerEvent("ParkVehicle");
            }
            else if (index == 4)
            {
                API.triggerServerEvent("ParkVehicle");
            }
            vehicleMenu.Visible = false;
        });

        tuningMenu.OnItemSelect.connect(function (sender, item, index)
        {
            API.sendNotification("Tw�j tuning zosta� pomy�lnie wymontowany.");
            tuningMenu.Visible = false;
        });
    }
    else if (eventName == "DrawVehicleComponents")
    {
        vehicleBlip = API.createBlip(args[0]);
        API.setBlipSprite(vehicleBlip, args[1]);
        API.setBlipColor(vehicleBlip, args[2]);
    } 
    else if (eventName === "DisposeVehicleComponents")
    {
        if (vehicleBlip == null) return;
        API.deleteEntity(vehicleBlip);
    }
});

API.onUpdate.connect(function ()
{
    if (menuPool != null)
    {
        menuPool.ProcessMenus();
    }
});
