/* Copyright (C) Przemys³aw Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemys³aw Postrach <toyerek@gmail.com> July 2017
 */

API.onServerEventTrigger.connect(function (eventName, args)
{
    if (eventName === "BWTimerTick")
    {
        API.showShard(args[0], 1000);
    }
});
