var jobBlip;
var jobMarker;

var drawJobText = false;
var jobTextString;

var resX = API.getScreenResolutionMantainRatio().Width;
var resY = API.getScreenResolutionMantainRatio().Height;

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
