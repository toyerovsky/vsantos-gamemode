var menuPool = null;

API.onServerEventTrigger.connect(function (eventName, args)
{
    if (eventName === "OnPlayerEnteredCarshop")
    {
        var compactsList = JSON.parse(args[0]);
        var coupesList = JSON.parse(args[1]);
        var suvsList = JSON.parse(args[2]);

        menuPool = API.getMenuPool();

        var mainMenu = API.createMenu("Kategorie", 0, 0, 6);
        API.setMenuBannerRectangle(mainMenu, 100, 0, 255, 0);
        API.setMenuTitle(mainMenu, "Salon");
        
        var compactsMainMenuItem = API.createMenuItem("Kompakty", "");
        mainMenu.AddItem(compactsMainMenuItem);

        var coupesMainMenuItem = API.createMenuItem("Coupe", "");
        mainMenu.AddItem(coupesMainMenuItem);

        var suvsMainMenuItem = API.createMenuItem("SUV", "");
        mainMenu.AddItem(suvsMainMenuItem);

        var sedansMainMenuItem = API.createMenuItem("Sedany", "");
        mainMenu.AddItem(sedansMainMenuItem);

		var sportsMainMenuItem = API.createMenuItem("Sportowe", "");
        mainMenu.AddItem(sportsMainMenuItem);

        var cyclesMainMenuItem = API.createMenuItem("Motocykle", "");
        mainMenu.AddItem(cyclesMainMenuItem);	

        var motorsMainMenuItem = API.createMenuItem("Rowery", "");
        mainMenu.AddItem(motorsMainMenuItem);	

        var compactsMenu = API.createMenu("Kompakty", 0, 0, 6);
        API.setMenuBannerRectangle(compactsMenu, 100, 0, 255, 0);

        var coupesMenu = API.createMenu("Coupe", 0, 0, 6);
        API.setMenuBannerRectangle(coupesMenu, 100, 0, 255, 0);

        var suvsMenu = API.createMenu("SUV", 0, 0, 6);
        API.setMenuBannerRectangle(suvsMenu, 100, 0, 255, 0);

        for (var i = 0; i < compactsList.length; i++)
        {
            var compactItem = API.createMenuItem(compactsList[i]["Name"], "Koszt: $" + compactsList[i]["Cost"]);
            compactsMenu.AddItem(compactItem);
        }
             
        for (var k = 0; k < coupesList.length; k++)
        {
            var coupeItem = API.createMenuItem(coupesList[k]["Name"], "Koszt: $" + coupesList[k]["Cost"]);
            coupesMenu.AddItem(coupeItem);
        }

        for (var l = 0; l < suvsList.length; l++)
        {
            var suvItem = API.createMenuItem(suvsList[l]["Name"], "Koszt: $" + suvsList[l]["Cost"]);
            suvsMenu.AddItem(suvItem);
        }

        mainMenu.BindMenuToItem(compactsMenu, compactsMainMenuItem);
        mainMenu.BindMenuToItem(coupesMenu, coupesMainMenuItem);
        mainMenu.BindMenuToItem(suvsMenu, suvsMainMenuItem);

        menuPool.Add(mainMenu);
        menuPool.Add(compactsMenu);
        menuPool.Add(coupesMenu);
        menuPool.Add(suvsMenu);

        mainMenu.Visible = true;

        compactsMenu.OnItemSelect.connect(function (sender, item, index)
        {
            API.triggerServerEvent("OnPlayerBoughtVehicle", item.Text.toString());
            
            compactsMenu.Visible = false;
        });

        coupesMenu.OnItemSelect.connect(function (sender, item, index)
        {
            API.triggerServerEvent("OnPlayerBoughtVehicle", item.Text.toString());
            coupesMenu.Visible = false;
        });

        suvsMenu.OnItemSelect.connect(function (sender, item, index)
        {
            API.triggerServerEvent("OnPlayerBoughtVehicle", item.Text.toString());
            suvsMenu.Visible = false;
        });
    }
});


API.onUpdate.connect(function ()
{
    if (menuPool != null)
    {
        menuPool.ProcessMenus();
    }
});
