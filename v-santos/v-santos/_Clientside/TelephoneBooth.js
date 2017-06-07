API.onServerEventTrigger.connect(function (eventName, args)
{
    if (eventName === "OnPlayerEnteredTelephoneBooth")
    {
        var number = API.getUserInput("", 10);

        if (number.trim() != "" && !isNaN(number))
        {
            API.triggerServerEvent("OnPlayerTelephoneBoothCall", number);
        }
        else
        {
            API.sendNotification("Wprowadzono nieporawny numer telefonu.");
        }
    }
});

API.onKeyDown.connect(function(sender, e) {
    if (e.KeyCode === Keys.End &&
        API.hasEntitySyncedData(API.getLocalPlayer(), "CellphoneID") &&
        API.hasEntitySyncedData(API.getLocalPlayer(), "CellphoneTalking")) {
        API.triggerServerEvent("OnPlayerTelephoneBoothEnd");
    }
});