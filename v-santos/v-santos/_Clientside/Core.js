API.onKeyDown.connect(function (sender, e)
{
    if (e.KeyCode === Keys.F12)
    {
        API.setHudVisible(!API.getHudVisible());
    }
    else if (e.KeyCode === Keys.F11)
    {
        API.setChatVisible(!API.getChatVisible());
    }
});

API.onServerEventTrigger.connect(function (eventName, args)
{
    //args[0] czy schowac czy pokazac
    if (eventName === "ToggleHud")
    {
        API.setHudVisible(args[0]);
    }
    else if (eventName === "ToggleChat")
    {
        API.setChatVisible(args[0]);
    }
    else if (eventName === "GetStreetName")
    {
        API.setEntitySyncedData(API.getLocalPlayer(), "StreetName", API.getStreetName(args[0]));
    }
});