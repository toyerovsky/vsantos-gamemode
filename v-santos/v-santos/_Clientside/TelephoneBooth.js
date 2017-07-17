/* Copyright (C) Przemys³aw Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemys³aw Postrach <toyerek@gmail.com> July 2017
 */

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