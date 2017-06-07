var menuPool = null;

API.onServerEventTrigger.connect(function (eventName, args)
{
    //args[0] to słownik animacji
    //args[1] to nazwa animacji
    if (eventName == "ShowAdminAnimMenu")
    {
        //Zmienne do przechowywania danych
        var _name = "";
        var _dict = "";
        var _anim = "";

        menuPool = API.getMenuPool();
        var menu = API.createMenu("Dodaj animację", 0, 0, 6);

        var name = API.createMenuItem("Ustal nazwę", "");
        menu.AddItem(name);

        var temp = args[0];
        var animsDataSource = new List(String);
        for (var k = 0; k < temp.Count; k++) {
            animsDataSource.Add(temp[k].toString());
        }
        var anims = API.createListItem("Animacje", "Animacje GTAN", animsDataSource, 0);

        var set = API.createMenuItem("Ustaw", "");
        menu.AddItem(set);

        var save = API.createMenuItem("Zapisz", ""); 
        menu.AddItem(save);

        menuPool.Add(menu);
        menu.Visible = true;

        anims.OnListChanged.connect(function (sender, newIndex)
        {
            //Dane są w formacie słownik,animacja dlatego należy je rozdzielić
            var a = sender.IndexToItem(sender.Index).toString().split(",");
            
            _dict = a[0];
            _anim = a[1];
        });

        menu.OnItemSelect.connect(function (sender, item, index)
        {
            if (item == name)
            {
                _name = API.getUserInput("Nazwa", 30);
                anims.Description = "Ostatnia animacja: " + _name;
            }
            else if (item == save) 
            {
                if (_name == "" || _anim == "" || _dict == "")
                {
                    API.sendNotification("Wypełnij wszystkie pola.");
                    return;
                }
                API.triggerServerEvent("OnPlayerAddAnim", _name, _dict, _anim);
            }
            else if (item == set)
            {
                if (_anim == "" || _dict == "") return;
                API.playPlayerAnimation(_dict, _name, 1, -1);
            }
        });
    }
});