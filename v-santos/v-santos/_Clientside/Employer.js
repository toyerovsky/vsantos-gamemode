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

    Call(met, val)
    {
        if (this.open === true) {
            this.browser.call(met, val);
        }
    }

    eval(string)
    {
        this.browser.eval(string);
    }
}

var employerCef;

API.onResourceStart.connect(function ()
{
    employerCef = new CefHelper("_Clientside/Resources/Employer/index.html");
});

API.onServerEventTrigger.connect(function (eventName, args)
{
    if (eventName === "OnPlayerEnteredEmployer")
    {
        //args[0] to gotówka do odebrania 
        employerCef.Show();
        employerCef.browser.call("UpdateInfo", args[0].toString());
    }
    else if (eventName === "OnPlayerExitEmployer")
    {
        CloseCef();
    }
});

function CloseCef()
{
    if (employerCef.open === true)
    {
        employerCef.Destroy();
    }
}

function SelectJob(value)
{
    API.triggerServerEvent("OnPlayerSelectedJob", value);
}

function TakeMoneyJob()
{
    API.triggerServerEvent("OnPlayerTakeMoneyJob");
}
