class CefHelper {
    constructor() {
        this.open = false;
        this.inUse = false;
    }

    Show()
    {
        if (this.open === false) {
            this.open = true;
            var resolution = API.getScreenResolution();
            this.browser = API.createCefBrowser(resolution.Width, resolution.Height, true);
            API.waitUntilCefBrowserInit(this.browser);
            API.setCefBrowserPosition(this.browser, 0, 0);
            //API.loadPageCefBrowser(this.browser, this.path);
            API.showCursor(true);
            API.setCefBrowserHeadless(this.browser, true);
        }
    }

    Hide()
    {
        if (this.inUse === true)
        {
            this.inUse = false;
            API.loadPageCefBrowser(this.browser, "");
            API.setCefBrowserHeadless(this.browser, true);
            API.setCanOpenChat(true);
            API.showCursor(false);
        }
    }

    LoadPage(path)
    {
        if (this.inUse === false)
        {
            API.setCanOpenChat(false);
            API.loadPageCefBrowser(this.browser, path);
            API.setCefBrowserHeadless(this.browser, false);
            this.inUse = true;
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

var charactersList;

var offer;

var descriptions;

//CrimeBot
var gunsCost;
var gunsCount;
var ammoCost;
var ammoCount;
var drugsCost;
var drugsCount;

var cef = new CefHelper();

API.onResourceStart.connect(function ()
{    
    cef.Show();
});

API.onServerEventTrigger.connect(function (eventName, args)
{
    if (eventName == "ShowLoginCef")
    {
        if (args[0])
        {
            cef.LoadPage("_Clientside/Resources/Login/index.html");
            var loginCamera = API.createCamera(args[1], args[2]);
            API.setActiveCamera(loginCamera);
        }
        else
        {
            cef.Hide();
        }
    }
    else if (eventName == "ShowCharacterSelectCef")
    {
        if (args[0])
        {
            charactersList = args[1];   
            cef.LoadPage("_Clientside/Resources/Characters/index.html");
        }
        else
        {
            cef.Hide();
            API.setActiveCamera(null);
        }
    }
    else if (eventName == "ShowOfferCef")
    {
        if (args[0])
        {
            offer = args[1];
            cef.LoadPage("_Clientside/Resources/Offer/index.html");
        }
        else
        {
            cef.Hide();
        }
    }
    else if (eventName == "ShowDescriptionsCef")
    {
        descriptions = args[0];           
        cef.LoadPage("_Clientside/Resources/Descriptions/index.html");
    }
    else if (eventName == "ShowCrimeBotCef")
    {
        gunsCost = args[0];
        gunsCount = args[1];

        ammoCost = args[2];
        ammoCount = args[3];

        drugsCost = args[4];
        drugsCount = args[5];

        cef.LoadPage("_Clientside/Resources/CrimeBot/index.html");    
    }
});

function Login(login, password)
{
    API.triggerServerEvent("OnPlayerEnteredLoginData", login, password);
}

function GetPlayerCharacters()
{
    return charactersList;
}

function SelectCharacter(uid)
{
    API.triggerServerEvent("OnPlayerSelectedCharacter", uid);
}

function GetOffer()
{
    return offer;
}

function PayOffer(bank)
{
    API.triggerServerEvent("OnPlayerOfferPay", bank);
}

function CancelOffer()
{
    API.triggerServerEvent("OnPlayerCancelOffer");
}

function GetDescriptions()
{
    return descriptions;
}

function AddDescription(title, description)
{
    API.triggerServerEvent("OnPlayerAddDescription", title, description);
}

function EditDescription(index, title, description)
{
    API.triggerServerEvent("OnPlayerEditDescription", index, title, description);
}

function DeleteDescription(index)
{
    API.triggerServerEvent("OnPlayerDeleteDescription", index);  
}

function SetDescription(description) {
    API.triggerServerEvent("OnPlayerSetDescription", description);
}

function GetGunsCost() {
    return gunsCost;
}

function GetGunsCount() {
    return gunsCount;
}

function GetAmmoCost() {
    return ammoCost;
}

function GetAmmoCount() {
    return ammoCount;
}

function GetDrugsCost() {
    return drugsCost;
}

function GetDrugsCount() {
    return drugsCount;
}

function CrimeBotBuy(i) {
    API.triggerServerEvent("OnCrimeBotBought", i.toString());
}

function CloseDescriptions()
{
    cef.Hide();
}

function CloseBot()
{
    cef.Hide();
}