/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

var menuPool = null;
let menu = null;

API.onServerEventTrigger.connect(function(eventName, args) {
    if (eventName === "ShowAdminMarketItemMenu") {
        menuPool = API.getMenuPool();

        menu = API.createMenu("Dodaj nowy przedmiot", 0, 0, 6);

        var nameItem = API.createMenuItem("Nazwa", "Ustal nazwe przedmiotu");
        menu.AddItem(nameItem);
        var firstParameterItem = API.createMenuItem("Pierwszy parametr", "Parametry to zmienne zależne od typu przedmiotu");
        menu.AddItem(nameItem);
        var secondParameterItem = API.createMenuItem("Drugi parametr", "Parametry to zmienne zależne od typu przedmiotu");
        menu.AddItem(nameItem);
        var thirdParameterItem = API.createMenuItem("Trzeci parametr", "Parametry to zmienne zależne od typu przedmiotu");
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
        var firstParameterResult = null;
        var secondParameterResult = null;
        var thirdParameterResult = null;

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
                    API.triggerServerEvent("AddMarketItem", nameResult, typeResult, costResult, names, firstParameterResult, secondParameterResult, thirdParameterResult);
                    menu.Visible = false;
                }
                else {
                    API.sendNotification("Dane nie zostały zaincjalizowane.");
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
        });

        types.OnListChanged.connect(function (sender, newIndex) {
            typeResult = sender.IndexToItem(sender.Index);
        });
    }
    else if (eventName == "ShowMarketMenu") {

        menuPool = API.getMenuPool();
        menu = API.createMenu("", 3, 3, 5);
        API.setMenuTitle(menu, "24/7");
        API.setMenuBannerRectangle(menu, 100, 106, 154, 40);

        var items = JSON.parse(args[0]);

        for (var i = 0; i < items.length; i++) {
            var itemMenuItem = API.createMenuItem(items[i]["Name"], "Koszt: " + items[i]["Cost"]);
            menu.AddItem(itemMenuItem);
        }
        menuPool.Add(menu);
        menu.Visible = true;

        menu.OnItemSelect.connect(function (sender, item, index) {
            API.triggerServerEvent("OnPlayerBoughtMarketItem", index);
            menu.Visible = false;
        });
    }
});

API.onUpdate.connect(function () {
    if (menuPool != null) {
        menuPool.ProcessMenus();
    }
});

