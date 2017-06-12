var menuPool = null;

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "ShowCornerBotMenu")
    {
        menuPool = API.getMenuPool();
        var menu = API.createMenu("Dodaj NPC", 0, 0, 6);
        
        menu.AddItem(API.createMenuItem("Nazwa", "Ustal nazwe NPC."));

        var temp = args[0];
        var skinDs = new List(String);
        for (var i = 0; i < temp.Count; i++) {
            skinDs.Add(temp[i].toString());
        }

        var skins = API.createListItem("Skin", "Ustal skin NPC", skinDs, 0);

        menu.AddItem(skins);
        menu.AddItem(API.createMenuItem("Koszt", "Ustal cene za jaka NPC kupuje narkotyk."));

        var dsType = new List(String);
        dsType.Add("Marihuana");
        dsType.Add("Lsd");
        dsType.Add("Ekstazy");
        dsType.Add("Amfetamina");
        dsType.Add("Metaamfetamina");
        dsType.Add("Crack");
        dsType.Add("Kokaina");
        dsType.Add("Haszysz");
        dsType.Add("Heroina");

        var types = API.createListItem("Typ narkotyku", "Ustal typ narkotyku jaki bedzie skupowal NPC", dsType, 0);

        menu.AddItem(types);
        menu.AddItem(API.createMenuItem("Przywitanie", "Ustal standardowe przywitanie gracza."));
        menu.AddItem(API.createMenuItem("Pozegnanie", "Ustal pozegnanie gracza."));
        menu.AddItem(API.createMenuItem("Pozegnanie zle", "Ustal pozegnanie kiedy transakcja przebiegla niepomyslnie."));

        menu.AddItem(API.createMenuItem("Zapisz", ""));

        menuPool.Add(menu);
        menu.Visible = true;

        var nameResult = null;
        var skinResult = null;
        var costResult = null;
        var typeResult = null;
        var greetingResult = null;
        var farewellResult = null;
        var badFarewellResult = null;

        menu.OnItemSelect.connect(function (sender, item, index)
        {
            if (index == 0) {
                nameResult = API.getUserInput("Imie Nazwisko", 30);
            }
            else if (index == 2) {
                costResult = API.getUserInput("Cena", 4);
                if (costResult.trim() == "" || isNaN(costResult)) {
                    API.sendNotification("Wprowadzono nieporawny koszt.");
                    costResult = null;
                }
            }
            else if (index == 4) {
                greetingResult = API.getUserInput("Przywitanie", 60);
            }
            else if (index == 5) {
                farewellResult = API.getUserInput("Pozegnanie przy udanej transakcji", 70);
            }
            else if (index == 6) {
                badFarewellResult = API.getUserInput("Pozegnanie przy nieudanej transakcji", 70);
            }
            else if (index == 7)
            {
                if (nameResult != null &&
                    skinResult != null &&
                    costResult != null &&
                    typeResult != null &&
                    greetingResult != null &&
                    farewellResult != null &&
                    badFarewellResult != null) {
                    API.triggerServerEvent("AddCornerBot", nameResult, skinResult, costResult, typeResult, greetingResult, farewellResult, badFarewellResult);
                    menu.Visible = false;
                }
                else
                {
                    API.sendNotification("Dane nie zosta³y zaincjalizowane.");
                }
                
            }
        });

        skins.OnListChanged.connect(function (sender, newIndex) {
            API.setPlayerSkin(parseInt(sender.IndexToItem(sender.Index)));
            skinResult = parseInt(sender.IndexToItem(sender.Index));
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

