/* Copyright (C) Przemys³aw Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemys³aw Postrach <toyerek@gmail.com> July 2017
 */

var menuPool = null;

let vehicleBlip = null;

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "OnPlayerShowVehicles") {
        menuPool = API.getMenuPool();
        var vehiclesMenu = API.createMenu("", 0, 0, 6);
        API.setMenuTitle(vehiclesMenu, "Pojazdy");
        API.setMenuBannerRectangle(vehiclesMenu, 100, 106, 154, 40);

        var vehicleInfoMenu = API.createMenu("", 0, 0, 6);
        API.setMenuTitle(vehicleInfoMenu, "Pojazdy");
        API.setMenuBannerRectangle(vehicleInfoMenu, 100, 106, 154, 40);

        var vehicles = JSON.parse(args[0]);

        for (var i = 0; i < vehicles.length; i++) {
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

        vehiclesMenu.OnItemSelect.connect(function (sender, item, index) {
            API.triggerServerEvent("OnPlayerSelectedVehicle", vehicles[index]["Id"]);
        });

        vehicleInfoMenu.OnItemSelect.connect(function (sender, item, index) {
            if (index == 0) {
                API.triggerServerEvent("OnPlayerSpawnVehicle");
            }
            else if (index == 1) {
                API.triggerServerEvent("OnPlayerInformationsVehicle");
            }
            vehicleInfoMenu.Visible = false;
        });
    }
    else if (eventName === "OnPlayerManageVehicle") {
        menuPool = API.getMenuPool();   
        var vehicleMenu = API.createMenu("", 0, 0, 6);
        API.setMenuTitle(vehicleMenu, "Pojazdy");
        API.setMenuBannerRectangle(vehicleMenu, 100, 106, 154, 40);

        var tuningMenu = API.createMenu("Tuning w pojeŸdzie", 0, 0, 6);
        API.setMenuTitle(tuningMenu, "Tuning");
        API.setMenuBannerRectangle(tuningMenu, 100, 106, 154, 40);

        var itemsMenu = API.createMenu("Przedmioty w pojeŸdzie", 0, 0, 6);
        API.setMenuTitle(tuningMenu, "Przemioty");
        API.setMenuBannerRectangle(tuningMenu, 100, 106, 154, 40);

        var groupsMenu = API.createMenu("Przypisz pod grupê", 0, 0, 6);
        API.setMenuTitle(tuningMenu, "Grupy");
        API.setMenuBannerRectangle(tuningMenu, 100, 106, 154, 40);


        var tuningList = JSON.parse(args[0]);
        var itemsList = JSON.parse(args[1]);
        var groupList = JSON.parse(args[2]);

        for (var j = 0; j < tuningList.length; j++) {
            var tuningListMenuItem = API.createMenuItem(tuningList[j]["Name"], "Kliknij enter aby wymontowaæ tuning.");
            tuningMenu.AddItem(tuningListMenuItem);
        }

        for (var k = 0; k < itemsList.length; k++) {
            var itemsListMenuItem = API.createMenuItem(itemsList[k]["Name"], "Kliknij enter aby wymontowaæ tuning.");
            itemsMenu.AddItem(itemsListMenuItem);
        }

        for (var i = 0; i < groupList.length; i++) {
            var groupListMenuItem = API.createMenuItem(groupList[i]["Name"], "Kliknij enter aby wymontowaæ tuning.");
            groupsMenu.AddItem(groupListMenuItem);
        }

        var engineStateMenuItem = API.createMenuItem("Zgas/Odpal silnik", "");
        vehicleMenu.AddItem(engineStateMenuItem);
        var changeLockMenuItem = API.createMenuItem("Otworz/Zamknij", "");
        vehicleMenu.AddItem(changeLockMenuItem);
        var infoMenuItem = API.createMenuItem("Informacje", "");
        vehicleMenu.AddItem(infoMenuItem);
        var parkMenuItem = API.createMenuItem("Zaparkuj tutaj", "");
        vehicleMenu.AddItem(parkMenuItem);
        var itemsMenuItem = API.createMenuItem("Przedmioty w pojeŸdzie", "");
        vehicleMenu.AddItem(itemsMenuItem);
        var groupsMenuItem = API.createMenuItem("Przypisz pod grupê", "");
        vehicleMenu.AddItem(groupsMenuItem);
        var sellMenuItem = API.createMenuItem("Sprzedaj", "");
        vehicleMenu.AddItem(sellMenuItem);
        var tuningMenuItem = API.createMenuItem("Tuning", "");
        vehicleMenu.AddItem(tuningMenuItem);

        vehicleMenu.BindMenuToItem(tuningMenu, tuningMenuItem);
        vehicleMenu.BindMenuToItem(itemsMenu, itemsMenuItem);
        vehicleMenu.BindMenuToItem(groupsMenu, groupsMenuItem);


        menuPool.Add(vehicleMenu);
        menuPool.Add(tuningMenu);
        vehicleMenu.Visible = true;

        vehicleMenu.OnItemSelect.connect(function (sender, item, index) {
            if (index == 0) {
                API.triggerServerEvent("OnPlayerEngineStateChangeVehicle");
            }
            else if (index == 1) {
                API.triggerServerEvent("OnPlayerChangeLockVehicle");
            }
            else if (index == 2) {
                API.triggerServerEvent("OnPlayerInformationsInVehicle");
            }
            else if (index == 3) {
                API.triggerServerEvent("ParkVehicle");
            }
            else if (index == 4) {
                API.triggerServerEvent("ParkVehicle");
            }
            vehicleMenu.Visible = false;
        });

        tuningMenu.OnItemSelect.connect(function (sender, item, index) {
            API.sendNotification(`Wymontowano ${tuningList[index].Name} z pojazdu.`);
            tuningMenu.Visible = false;
        });

        itemsMenu.OnItemSelect.connect(function (sender, item, index) {
            API.sendNotification(`Wyjêto ${itemsList[index].Name} z pojazdu.`);
            tuningMenu.Visible = false;
        });

        groupsMenu.OnItemSelect.connect(function (sender, item, index) {
            
            tuningMenu.Visible = false;
        });
    }
    else if (eventName == "DrawVehicleComponents") {
        vehicleBlip = API.createBlip(args[0]);
        API.setBlipSprite(vehicleBlip, args[1]);
        API.setBlipColor(vehicleBlip, args[2]);
    }
    else if (eventName === "DisposeVehicleComponents") {
        if (vehicleBlip == null) return;
        API.deleteEntity(vehicleBlip);
    }
});

API.onUpdate.connect(function () {
    if (menuPool != null) {
        menuPool.ProcessMenus();
    }
});
