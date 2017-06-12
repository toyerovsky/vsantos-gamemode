var menuPool = null;

API.onServerEventTrigger.connect(function(eventName, args) {
    if (eventName === "ShowAdminMarketItemMenu") {
        menuPool = API.getMenuPool();
        var menu = API.createMenu("Dodaj przedmiot do sklepu", 0, 0, 6);

        var nameItem = API.createMenuItem("Nazwa", "Ustal nazwe przedmiotu.");
        menu.AddItem(nameItem);
        var typesItem = API.createListItem("Typy", "Ustal typ przedmiotu", args[0], 0);
        menu.AddItem(typesItem);
        var costItem = API.createMenuItem("Koszt", "Ustal cenę przedmiotu");
        menu.AddItem(costItem);

        var names = new List(string);

        var list = args[1];
        for (var i = 0; i < list.Count; i++) {
            var item = API.createCheckboxItem(list[i], "Zaznacz sklep do ktorego chcesz dodac przedmiot", false);
            item.OnCheckboxChange.connect((sender, checkbox, checked) => { if (checked) names.Add(checkbox.Name); });
            menu.AddItem(item);    
        }

        var saveItem = API.createMenuItem("Zapisz", "");
        menu.AddItem(save);

        menuPool.Add(menu);
        menu.Visible = true;

        var nameResult = null;
        var typeResult = null;
        var costResult = null;

        menu.OnItemSelect.connect(function (sender, item, index) {
            if (item === nameItem) {
                nameResult = API.getUserInput("Nazwa przedmiotu", 30);
            }
            else if (item === costItem) {
                costResult = API.getUserInput("Cena", 5);
                if (costResult.trim() == "" || isNaN(costResult)) {
                    API.sendNotification("Wprowadzono nieporawny koszt.");
                    costResult = null;
                }
            }
            else if (item == saveItem) {
                if (nameResult != null &&
                    costResult != null &&
                    typeResult != null) {
                    API.triggerServerEvent("AddMarketItem", nameResult, typeResult, costResult, names);
                    menu.Visible = false;
                }
                else {
                    API.sendNotification("Dane nie zostały zaincjalizowane.");
                }
            }
        });

        types.OnListChanged.connect(function (sender, newIndex) {
            typeResult = sender.IndexToItem(sender.Index);
        });
    }
});

API.onUpdate.connect(function () {
    if (menuPool != null) {
        menuPool.ProcessMenus();
    }
});

