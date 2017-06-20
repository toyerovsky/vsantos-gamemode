var menuPool = null;

API.onServerEventTrigger.connect(function (eventName, args)
{
 if (eventName === "ShowItems")
    {
        menuPool = API.getMenuPool();
        var menu = API.createMenu("", 0, 0, 6);
        API.setMenuTitle(menu, "Przedmioty");
        API.setMenuBannerRectangle(menu, 100, 106, 154, 40);

        var items = JSON.parse(args[0]);

        var itemMenuItem = {};
        for (var i = 0; i < items.length; i++)
        {
            itemMenuItem = API.createMenuItem(items[i]["Name"], "");
            menu.AddItem(itemMenuItem);
        }
        menuPool.Add(menu);
        menu.Visible = true;

        menu.OnItemSelect.connect(function (sender, item, index)
        {
            API.triggerServerEvent("SelectedItem", index);
            menu.Visible = false;
        });
    }
    else if (eventName === "SelectOptionItem")
    {
        menuPool = API.getMenuPool();
        var menu = API.createMenu("", 0, 0, 6);
        API.setMenuTitle(menu, "Przedmioty");
        API.setMenuBannerRectangle(menu, 100, 0, 255, 0);

        var itemMenuItem = {};
        itemMenuItem = API.createMenuItem("Uzyj", "");
        menu.AddItem(itemMenuItem);
        itemMenuItem = API.createMenuItem("Informacje", "");
        menu.AddItem(itemMenuItem);
        itemMenuItem = API.createMenuItem("Co daje ten przedmiot?", "");
        menu.AddItem(itemMenuItem);
        itemMenuItem = API.createMenuItem("Twoje inne przedmioty...", "");
        menu.AddItem(itemMenuItem);

        menuPool.Add(menu);
        menu.Visible = true;

        menu.OnItemSelect.connect(function (sender, item, index)
        {
            if (index == 0)
            {
                API.triggerServerEvent("UseItem");
            }
            else if (index == 1)
            {
                API.triggerServerEvent("InformationsItem");
            }
            else if (index == 2)
            {
                API.triggerServerEvent("UsingInformationsItem");
            }
            else if (index == 3)
            {
                API.triggerServerEvent("BackToItemsList");
            }
            menu.Visible = false;
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

