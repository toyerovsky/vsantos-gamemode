API.onServerEventTrigger.connect(function (eventName, args)
{
    if (eventName === "BWTimerTick")
    {
        API.showShard(args[0], 1000);
    }
});
