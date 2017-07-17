/* Copyright (C) Przemys³aw Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemys³aw Postrach <toyerek@gmail.com> July 2017
 */

var menuPool = null;

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "OnPlayerEnteredCarshop") {
        var compactsList = JSON.parse(args[0]);
        var coupesList = JSON.parse(args[1]);
        var suvsList = JSON.parse(args[2]);
        var sedansList = JSON.parse(args[3]);
        var sportsList = JSON.parse(args[4]);
        var motorcyclesList = JSON.parse(args[6]);
        var bicyclesList = JSON.parse(args[5]);

        menuPool = API.getMenuPool();

        var mainMenu = API.createMenu("Kategorie", 0, 0, 6);
        API.setMenuBannerRectangle(mainMenu, 100, 0, 255, 0);
        API.setMenuTitle(mainMenu, "Salon");

        var compactsMainMenuItem = API.createMenuItem("Kompakty", "");
        mainMenu.AddItem(compactsMainMenuItem);
        var compactsMenu = API.createMenu("Kompakty", 0, 0, 6);
        API.setMenuBannerRectangle(compactsMenu, 100, 0, 255, 0);

        var coupesMainMenuItem = API.createMenuItem("Coupe", "");
        mainMenu.AddItem(coupesMainMenuItem);
        var coupesMenu = API.createMenu("Coupe", 0, 0, 6);
        API.setMenuBannerRectangle(coupesMenu, 100, 0, 255, 0);

        var suvsMainMenuItem = API.createMenuItem("SUV", "");
        mainMenu.AddItem(suvsMainMenuItem);
        var suvsMenu = API.createMenu("SUV", 0, 0, 6);
        API.setMenuBannerRectangle(suvsMenu, 100, 0, 255, 0);

        var sedansMainMenuItem = API.createMenuItem("Sedany", "");
        mainMenu.AddItem(sedansMainMenuItem);
        var sedansMenu = API.createMenu("Sedany", 0, 0, 6);
        API.setMenuBannerRectangle(sedansMenu, 100, 0, 255, 0);

        var sportsMainMenuItem = API.createMenuItem("Sportowe", "");
        mainMenu.AddItem(sportsMainMenuItem);
        var sportsMenu = API.createMenu("Sportowe", 0, 0, 6);
        API.setMenuBannerRectangle(sportsMenu, 100, 0, 255, 0);

        var bicyclesMainMenuItem = API.createMenuItem("Rowery", "");
        mainMenu.AddItem(bicyclesMainMenuItem);
        var bicyclesMenu = API.createMenu("Rowery", 0, 0, 6);
        API.setMenuBannerRectangle(bicyclesMenu, 100, 0, 255, 0);

        var motorcyclesMainMenuItem = API.createMenuItem("Motocykle", "");
        mainMenu.AddItem(motorcyclesMainMenuItem);
        var motorcyclesMenu = API.createMenu("Motocykle", 0, 0, 6);
        API.setMenuBannerRectangle(motorcyclesMenu, 100, 0, 255, 0);

        for (var i = 0; i < compactsList.length; i++) {
            var compactItem = API.createMenuItem(compactsList[i]["Name"], "Koszt: $" + compactsList[i]["Cost"]);
            compactsMenu.AddItem(compactItem);
        }

        for (var k = 0; k < coupesList.length; k++) {
            var coupeItem = API.createMenuItem(coupesList[k]["Name"], "Koszt: $" + coupesList[k]["Cost"]);
            coupesMenu.AddItem(coupeItem);
        }

        for (var l = 0; l < suvsList.length; l++) {
            var suvItem = API.createMenuItem(suvsList[l]["Name"], "Koszt: $" + suvsList[l]["Cost"]);
            suvsMenu.AddItem(suvItem);
        }

        for (var a = 0; a < sedansList.length; a++) {
            var sedanItem = API.createMenuItem(sedansList[a]["Name"], "Koszt: $" + sedansList[a]["Cost"]);
            sedansList.AddItem(sedanItem);
        }

        for (var b = 0; b < sportsList.length; b++) {
            var sportItem = API.createMenuItem(sportsList[b]["Name"], "Koszt: $" + sportsList[b]["Cost"]);
            suvsMenu.AddItem(sportItem);
        }

        for (var d = 0; d < bicyclesList.length; d++) {
            var bicycleItem = API.createMenuItem(bicyclesList[d]["Name"], "Koszt: $" + bicyclesList[d]["Cost"]);
            suvsMenu.AddItem(bicycleItem);
        }

        for (var c = 0; c < motorcyclesList.length; c++) {
            var motorcycleItem = API.createMenuItem(motorcyclesList[c]["Name"], "Koszt: $" + motorcyclesList[c]["Cost"]);
            suvsMenu.AddItem(motorcycleItem);
        }

        mainMenu.BindMenuToItem(compactsMenu, compactsMainMenuItem);
        mainMenu.BindMenuToItem(coupesMenu, coupesMainMenuItem);
        mainMenu.BindMenuToItem(suvsMenu, suvsMainMenuItem);
        mainMenu.BindMenuToItem(sedansMenu, sedansMainMenuItem);
        mainMenu.BindMenuToItem(sportsMenu, sportsMainMenuItem);
        mainMenu.BindMenuToItem(bicyclesMenu, bicyclesMainMenuItem);
        mainMenu.BindMenuToItem(motorcyclesMenu, motorcyclesMainMenuItem);

        menuPool.Add(mainMenu);
        menuPool.Add(compactsMenu);
        menuPool.Add(coupesMenu);
        menuPool.Add(sedansMenu);
        menuPool.Add(sportsMenu);
        menuPool.Add(motorcyclesMenu);
        menuPool.Add(bicyclesMenu);

        mainMenu.Visible = true;

        compactsMenu.OnItemSelect.connect(function (sender, item, index) {
            API.triggerServerEvent("OnPlayerBoughtVehicle", item.Text.toString());
            compactsMenu.Visible = false;
        });

        coupesMenu.OnItemSelect.connect(function (sender, item, index) {
            API.triggerServerEvent("OnPlayerBoughtVehicle", item.Text.toString());
            coupesMenu.Visible = false;
        });

        suvsMenu.OnItemSelect.connect(function (sender, item, index) {
            API.triggerServerEvent("OnPlayerBoughtVehicle", item.Text.toString());
            suvsMenu.Visible = false;
        });

        sedansMenu.OnItemSelect.connect(function (sender, item, index) {
            API.triggerServerEvent("OnPlayerBoughtVehicle", item.Text.toString());

            sedansMenu.Visible = false;
        });

        sportsMenu.OnItemSelect.connect(function (sender, item, index) {
            API.triggerServerEvent("OnPlayerBoughtVehicle", item.Text.toString());
            sportsMenu.Visible = false;
        });

        bicyclesMenu.OnItemSelect.connect(function (sender, item, index) {
            API.triggerServerEvent("OnPlayerBoughtVehicle", item.Text.toString());
            bicyclesMenu.Visible = false;
        });

        motorcyclesMenu.OnItemSelect.connect(function (sender, item, index) {
            API.triggerServerEvent("OnPlayerBoughtVehicle", item.Text.toString());
            motorcyclesMenu.Visible = false;
        });
    }
});


API.onUpdate.connect(function () {
    if (menuPool != null) {
        menuPool.ProcessMenus();
    }
});
