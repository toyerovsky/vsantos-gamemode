var menuPool = null;
let menu = null;

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "ShowAdminWarehouseItemMenu") {
        menuPool = API.getMenuPool();

        menu = API.createMenu("Dodaj nowy przedmiot", 0, 0, 6);

        var nameItem = API.createMenuItem("Nazwa", "Ustal nazwe przedmiotu");
        menu.AddItem(nameItem);
        var firstParameterItem = API.createMenuItem("Pierwszy parametr", "Parametry to zmienne zale¿ne od typu przedmiotu");
        menu.AddItem(nameItem);
        var secondParameterItem = API.createMenuItem("Drugi parametr", "Parametry to zmienne zale¿ne od typu przedmiotu");
        menu.AddItem(nameItem);
        var thirdParameterItem = API.createMenuItem("Trzeci parametr", "Parametry to zmienne zale¿ne od typu przedmiotu");
        menu.AddItem(nameItem);
        var typesItem = API.createListItem("Typ przedmiotu", "Ustal typ przedmiotu", args[0], 0);
        menu.AddItem(typesItem);
        var groupTypesItem = API.createListItem("Typ grupy", "Ustal typ grupy dla jakiej dostêpny bêdzie przedmiot", args[1], 0);
        menu.AddItem(typesItem);
        var costItem = API.createMenuItem("Koszt", "Ustal koszt za jaki bêdzie mo¿na zamówiæ przedmiot");
        menu.AddItem(costItem);
        var minimalCostItem = API.createMenuItem("Minimalny koszt", "Ustal minimalny koszt sprzeda¿y");
        menu.AddItem(costItem);
        var weeklyCountItem = API.createMenuItem("Tygodniowa iloœæ", "Ustal iloœæ przedmiotu jaka bêdzie dostêpna co tydzieñ");
        menu.AddItem(costItem);

        var names = new List(string);

        var saveItem = API.createMenuItem("Zapisz", "");
        menu.AddItem(save);

        menuPool.Add(menu);
        menu.Visible = true;

        var nameResult = null;
        var typeResult = null;
        var costResult = null;
        var firstParameterResult = null;
        var secondParameterResult = null;
        var thirdParameterResult = null;
        var groupTypeResult = null;
        var minimalCostResult = null;
        var weeklyCountResult = null;

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
                    typeResult != null &&
                    weeklyCountResult != null &&
                    minimalCostResult != null) {
                    API.triggerServerEvent("AddMarketItem", nameResult, typeResult, costResult, groupTypeResult, minimalCostResult, weeklyCountResult, firstParameterResult, secondParameterResult, thirdParameterResult);
                    menu.Visible = false;
                }
                else {
                    API.sendNotification("Dane nie zosta³y zaincjalizowane.");
                }
            }
            else if (item == firstParameterItem) {
                firstParameterResult = API.getUserInput("Pierwszy parametr", 5);
                if (firstParameterResult.trim() == "" || isNaN(firstParameterResult)) {
                    API.sendNotification("Wprowadzono niepoprawny parametr");
                    firstParameterResult = null;
                }
            }
            else if (item == secondParameterItem) {
                secondParameterResult = API.getUserInput("Drugi parametr", 5);
                if (secondParameterResult.trim() == "" || isNaN(secondParameterResult)) {
                    API.sendNotification("Wprowadzono niepoprawny parametr");
                    secondParameterResult = null;
                }
            }
            else if (item == thirdParameterItem) {
                thirdParameterResult = API.getUserInput("Trzeci parametr", 5);
                if (thirdParameterResult.trim() == "" || isNaN(thirdParameterResult)) {
                    API.sendNotification("Wprowadzono niepoprawny parametr");
                    thirdParameterResult = null;
                }
            }
            else if (item == minimalCostItem) {
                minimalCostResult = API.getUserInput("Minimalny koszt", 6);
                if (minimalCostResult.trim() == "" || isNaN(minimalCostResult)) {
                    API.sendNotification("Wprowadzono niepoprawny koszt");
                    minimalCostResult = null;
                }
            }
            else if (item == weeklyCountItem) {
                weeklyCountResult = API.getUserInput("Tygodniowa iloœæ", 6);
                if (weeklyCountResult.trim() == "" || isNaN(weeklyCountResult)) {
                    API.sendNotification("Wprowadzono niepoprawn¹ iloœæ");
                    weeklyCountResult = null;
                }
            }
        });

        types.OnListChanged.connect(function (sender, newIndex) {
            typeResult = sender.IndexToItem(sender.Index);
        });

        groupTypesItem.OnListChanged.connect(function (sender, newIndex) {
            groupTypeResult = sender.IndexToItem(sender.Index);
        });
    }
});

API.onUpdate.connect(() => {
    if (menuPool != null) {
        menuPool.ProcessMenus();
    }
});

