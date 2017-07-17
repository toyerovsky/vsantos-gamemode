/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

API.onServerEventTrigger.connect(function (eventName, args)
{
    if (eventName === "BWTimerTick")
    {
        API.showShard(args[0], 1000);
    }
});
