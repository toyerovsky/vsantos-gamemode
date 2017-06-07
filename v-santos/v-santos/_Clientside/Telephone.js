var menuPool = null;

API.onKeyDown.connect(function (sender, e)
{
    if (e.KeyCode === Keys.End && API.hasEntitySyncedData(API.getLocalPlayer(), "CellphoneID") && API.hasEntitySyncedData(API.getLocalPlayer(), "CellphoneTalking"))
    {
        API.triggerServerEvent("OnPlayerCellphoneEnd");
    }
    else if (e.KeyCode === Keys.End && API.hasEntitySyncedData(API.getLocalPlayer(), "CellphoneID") && API.hasEntitySyncedData(API.getLocalPlayer(), "CellphoneRinging"))
    {
        API.triggerServerEvent("OnPlayerCellphonePickUp");
    }
    else if (e.KeyCode === Keys.End && API.hasEntitySyncedData(API.getLocalPlayer(), "CellphoneID"))
    {
        API.triggerServerEvent("OnPlayerPullCellphoneRequest");
    }
});

API.onServerEventTrigger.connect(function (eventName, args)
{
    if (eventName === "OnPlayerPulledCellphone")
    {
        var number;
        var name;

        var contactsList = JSON.parse(args[1]);
        var messagesList = JSON.parse(args[2]);

        menuPool = API.getMenuPool();

        var telephoneMenu = API.createMenu(args[0].toString(), 0, 0, 3);
        var contactsMenu = API.createMenu("Kontakty", 0, 0, 3);
        var contactsSubmenu = API.createMenu("Kontakty", 0, 0, 3);
        var messagesMenu = API.createMenu("Wiadomosci", 0, 0, 3);
        var messagesSubmenu = API.createMenu("Wiadomosci", 0, 0, 3);
        var inboxMenu = API.createMenu("Skrzynka odbiorcza", 0, 0, 3);

        var callMenuItem = API.createMenuItem("Zadzwon", "");
        var messageMenuItem = API.createMenuItem("Wiadomosci", "");
        var contactsMenuItem = API.createMenuItem("Kontakty", "");
        var vcardMenuItem = API.createMenuItem("Wyslij v-card", "Wyslij swoj kontakt do innego telefonu");
        var muteMenuItem = API.createMenuItem("Wycisz", "");
        var addContactMenuItem = API.createMenuItem("Dodaj kontakt", "");


        var sendMessageMenuItem = API.createMenuItem("Wyslij", "Wyslij wiadomosc na podany numer telefonu");
        var inboxMenuItem = API.createMenuItem("Skrzynka odbiorcza", "");

        //Menu dla ka¿dego kontaktu
        var callContactMenuItem = API.createMenuItem("Zadzwon", "Zadzwon pod numer kontaktu");
        var sendMessageToContactMenuItem = API.createMenuItem("Wyslij wiadomosc", "Wyslij wiadomosc do kontaktu");
        var editContactMenuItem = API.createMenuItem("Edytuj", "Edytuj kontakt");
        var deleteContactMenuItem = API.createMenuItem("Usun", "Usun kontakt");

        var replyMessageMenuItem = API.createMenuItem("Odpowiedz", "Zadzwon pod numer kontaktu");
        var deleteMessageMenuItem = API.createMenuItem("Usun", "Zadzwon pod numer kontaktu");

        for (var a = 0; a < contactsList.length; a++)
        {
            var contactItem = API.createMenuItem(contactsList[a]["Name"], "Kontakt o numerze: " + contactsList[a]["Number"]);
            contactsSubmenu.AddItem(contactItem);
        }

        for (var k = 0; k < messagesList.length; k++)
        {
            var messageItem = API.createMenuItem(messagesList[k]["Content"], "Wiadomosc od numeru: " + messagesList[k]["SenderNumber"]);
            inboxMenu.AddItem(messageItem);
        }

        telephoneMenu.AddItem(callMenuItem);
        telephoneMenu.AddItem(messageMenuItem);
        telephoneMenu.AddItem(contactsMenuItem);
        telephoneMenu.AddItem(vcardMenuItem);
        telephoneMenu.AddItem(muteMenuItem);
        telephoneMenu.AddItem(addContactMenuItem);

        contactsSubmenu.AddItem(callContactMenuItem);
        contactsSubmenu.AddItem(editContactMenuItem);
        contactsSubmenu.AddItem(deleteContactMenuItem);

        messagesMenu.AddItem(sendMessageMenuItem);
        messagesMenu.AddItem(sendMessageToContactMenuItem);
        messagesMenu.AddItem(inboxMenuItem);

        telephoneMenu.BindMenuToItem(contactsMenu, contactsMenuItem);
        telephoneMenu.BindMenuToItem(messagesMenu, messageMenuItem);

        menuPool.Add(telephoneMenu);
        menuPool.Add(contactsMenu);
        menuPool.Add(contactsSubmenu);
        menuPool.Add(messagesMenu);
        menuPool.Add(messagesSubmenu);
        menuPool.Add(inboxMenu);

        telephoneMenu.Visible = true;

        telephoneMenu.OnItemSelect.connect(function (sender, item, index)
        {
            if (index == 0)
            {
                number = API.getUserInput("Numer", 10);

                if (number.trim() != "" && !isNaN(number))
                {
                    API.triggerServerEvent("OnPlayerTelephoneCall", number);
                }
                else
                {
                    API.sendNotification("Wprowadzono nieporawny numer telefonu.");
                }
            }
            else if (index == 3)
            {
                API.triggerServerEvent("OnPlayerTelephoneVcard");
            }
            else if (index == 4)
            {
                if (API.hasEntitySyncedData(API.getLocalPlayer(), "CellphoneMuted"))
                {
                    API.setEntitySyncedData(API.getLocalPlayer(), "CellphoneMuted", true);
                    API.sendNotification("Wyciszono telefon.");
                }
                else
                {
                    API.resetEntitySyncedData(API.getLocalPlayer(), "CellphoneMuted");
                    API.sendNotification("Odciszono telefon.");
                }

            }
            else if (index == 5)
            {
                number = API.getUserInput("Numer", 9);
                name = API.getUserInput("Nazwa", 15);

                if (number.trim() != "" && !isNaN(number))
                {
                    API.triggerServerEvent("OnPlayerTelephoneContactAdded", number, name);
                }
                else
                {
                    API.sendNotification("Wprowadzono nieporawny numer telefonu.");
                }
            }
            telephoneMenu.Visible = false;
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
