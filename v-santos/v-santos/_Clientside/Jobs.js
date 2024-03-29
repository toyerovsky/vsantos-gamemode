/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

var jobBlip;
var jobMarker;

var drawJobText = false;
var jobTextString;

var resX = API.getScreenResolutionMaintainRatio().Width;
var resY = API.getScreenResolutionMaintainRatio().Height;

API.onServerEventTrigger.connect(function (eventName, args)
{
    if (eventName === "DrawJobComponents")
    {
        jobMarker = API.createMarker(1, args[0], new Vector3(), new Vector3(), new Vector3(2, 2, 2), 255, 255, 100, 100);
        jobBlip = API.createBlip(args[0]);
        API.setBlipSprite(jobBlip, args[1]);
    }
    else if (eventName === "DisposeJobComponents")
    {
        API.deleteEntity(jobBlip);
        API.deleteEntity(jobMarker);
    }
    else if (eventName === "JobText_Changed")
    {
        jobTextString = args[0];
    }
    else if (eventName === "JobTextVisibility")
    {
        drawJobText = args[0];
    }
});

API.onUpdate.connect(function (sender, args)
{
    if (drawJobText)
    {
        //public void drawText(string caption, double xPos, double yPos, double scale, int r, int g, int b, int alpha, int font, int justify, bool shadow, bool outline, int wordWrap)
        API.drawText(jobTextString, resX - 30, resY - 100, 1, 191, 244, 66, 110, 4, 2, false, true, 0);
    }
});
