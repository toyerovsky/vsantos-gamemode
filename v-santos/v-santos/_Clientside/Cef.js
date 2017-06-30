class WebBrowser {
    constructor(local) {
        var resolution = API.getScreenResolution();

        this.browser = API.createCefBrowser(resolution.Width, resolution.Height, local)
        API.waitUntilCefBrowserInit(this.browser)
        API.setCefBrowserPosition(this.browser, 0, 0)

        API.setCefDrawState(false);
    }

    load(path, local = true, chatstate = false, cursorstate = true) {
        if (!API.isCefDrawEnabled()) {
            API.setCefDrawState(true);
        }
        this.path = path
        API.loadPageCefBrowser(this.browser, this.path)
        API.setCanOpenChat(chatstate)
        API.showCursor(cursorstate)

    }

    show(chatstate = false) {
        if (API.isCefDrawEnabled()) {
            API.setCefDrawState(false)
            API.setCanOpenChat(chatstate)
            API.showCursor(true)
        }
    }

    hide() {
        if (API.isCefDrawEnabled()) {
            API.setCefDrawState(false)
            API.setCanOpenChat(true)
            API.showCursor(false)
        }
    }

    destroy() {
        API.setCefDrawState(false)
        API.destroyCefBrowser(this.browser)
        API.setCanOpenChat(true)
        API.showCursor(false)
    }

    call(func, args) {
        this.browser.call(func, args)
    }

    eval(string) {
        this.browser.eval(string)
    }
}

let CEF = new WebBrowser(true);

var offer;
var descriptions;

let wheelMenuDataSource = null;
let busMenuDataSource = null;
let crimeBotMenuDataSource = null;

API.onResourceStart.connect(function () {
    CEF.show();
});

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName == "ShowOfferCef")
    {
        if (args[0]) {
            offer = args[1];
            CEF.load("_Clientside/Resources/Offer/index.html");
        }
        else {
            CEF.hide();
        }
    }
    else if (eventName == "ShowDescriptionsCef") {
        descriptions = args[0];
        CEF.load("_Clientside/Resources/Descriptions/index.html");
    }
    else if (eventName == "ShowCrimeBotCef") {

        crimeBotMenuDataSource = args[0];
        CEF.load("_Clientside/Resources/CrimeBot/index.html");
    }
    else if (eventName == "ShowWheelMenu") {
        wheelMenuDataSource = args[0];
        CEF.load("_Clientside/Resources/WheelMenu/index.html");
    }
    else if (eventName == "ShowBusMenu") {
        CEF.load("_Clientside/Resources/Bus/bus.html");
        busMenuDataSource = args[0];
    }
});


function GetOffer() {
    return offer;
}

function PayOffer(bank) {
    API.triggerServerEvent("OnPlayerOfferPay", bank);
}

function CancelOffer() {
    API.triggerServerEvent("OnPlayerCancelOffer");
}

function GetDescriptions() {
    return descriptions;
}

function AddDescription(title, description) {
    API.triggerServerEvent("OnPlayerAddDescription", title, description);
}

function EditDescription(index, title, description) {
    API.triggerServerEvent("OnPlayerEditDescription", index, title, description);
}

function DeleteDescription(index) {
    API.triggerServerEvent("OnPlayerDeleteDescription", index);
}

function SetDescription(description) {
    API.triggerServerEvent("OnPlayerSetDescription", description);
}

function GetCrimeBotItems() {
    return crimeBotMenuDataSource;
}

function CrimeBotBuy(i) {
    API.triggerServerEvent("OnCrimeBotBought", i.toString());
}

function GetBusStops() {
    return busMenuDataSource;
}

function RequestBus(time, cost, index) {
    API.triggerServerEvent("RequestBus", time, cost, index);
    CloseCef();
}

function GetWheelMenuDataSource() {
    return wheelMenuDataSource;
}

function WheelMenuUseItem(name) {
    API.triggerServerEvent("UseWheelMenuItem", name);
    CloseCef();
}

function CloseCef() {
    CEF.hide();
}
