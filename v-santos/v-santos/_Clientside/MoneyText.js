var drawMoney = false;
var moneyString;

var resX = API.getScreenResolutionMantainRatio().Width;
var resY = API.getScreenResolutionMantainRatio().Height;

API.onServerEventTrigger.connect(function (eventName, args)
{
    if (eventName === "Money_Changed")
    {
        drawMoney = true;
        moneyString = args[0];
    }
});

API.onUpdate.connect(function (sender, args)
{
    if (drawMoney)
    {
        //public void drawText(string caption, double xPos, double yPos, double scale, int r, int g, int b, int alpha, int font, int justify, bool shadow, bool outline, int wordWrap)
        API.drawText(moneyString, resX - 15, 10 + 40, 1, 115, 186, 131, 255, 4, 2, false, true, 0);
    }
});

