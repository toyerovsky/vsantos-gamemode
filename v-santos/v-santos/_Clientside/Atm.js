class CefHelper
{
    constructor(resourcePath)
    {
        this.path = resourcePath;
        this.open = false;
        this.browser = null;
    }

    Show()
    {
        if (this.open === false) {
            this.open = true;
            var resolution = API.getScreenResolution();
            this.browser = API.createCefBrowser(resolution.Width, resolution.Height, true);
            API.waitUntilCefBrowserInit(this.browser);
            API.setCefBrowserPosition(this.browser, 0, 0);
            API.loadPageCefBrowser(this.browser, this.path);
            API.showCursor(true);
        }
    }

    Destroy()
    {
        this.open = false;
        API.destroyCefBrowser(this.browser);
        API.showCursor(false);
    }

    eval(string)
    {
        this.browser.eval(string);
    }
}

var atmCef;
var atmName;
var atmCount;
var atmNumber;

API.onResourceStart.connect(function ()
{
    atmCef = new CefHelper("Clientside/Resources/Bank/Atm/atm.html");
});

API.onServerEventTrigger.connect(function (eventName, args)
{
    if (eventName == "OnPlayerEnteredAtm")
    {
        atmName = args[0].toString();
        atmCount = args[1].toString();
        atmNumber = args[2].toString();
        atmCef.Show();
    }
    else if (eventName == "OnPlayerExitAtm")
    {
        if (atmCef.open === true)
        {
            atmCef.Destroy();
        }
    }
});

function Close()
{
    if (atmCef.open === true)
    {
        atmCef.Destroy();
    }
}

function TakeAtm(count)
{
    API.triggerServerEvent("OnPlayerAtmTake", count);
}

function GiveAtm(count)
{
    API.triggerServerEvent("OnPlayerAtmGive", count);
}

function GetName()
{
    return atmName;
}

function GetCount()
{
    return atmCount;
}

function GetNumber()
{
    return atmNumber;
}
