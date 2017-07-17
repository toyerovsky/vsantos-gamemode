/* Copyright (C) Przemys³aw Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemys³aw Postrach <toyerek@gmail.com> July 2017
 */

class WebBrowser {
    constructor(local) {
        var resolution = API.getScreenResolution();

        this.browser = API.createCefBrowser(resolution.Width, resolution.Height, local);
        API.waitUntilCefBrowserInit(this.browser);
        API.setCefBrowserPosition(this.browser, 0, 0);
    }

    load(path, local = true, chatstate = false, cursorstate = true) {
        if (API.isCefBrowserInitialized(this.browser)) {
            API.setCefBrowserHeadless(false);
        }

        this.path = path;
        API.loadPageCefBrowser(this.browser, this.path);
        API.setCanOpenChat(chatstate);
        API.showCursor(cursorstate);

    }

    show(chatstate = false) {
        if (API.isCefBrowserInitialized(this.browser)) {
            API.setCefBrowserHeadless(true);
            API.setCanOpenChat(chatstate);
            API.showCursor(true);
        }
    }

    hide() {
        if (API.isCefBrowserInitialized(this.browser)) {
            API.setCefBrowserHeadless(true);
            API.setCanOpenChat(true);
            API.showCursor(false);
        }
    }

    destroy() {
        API.setCefDrawState(false);
        API.destroyCefBrowser(this.browser);
        API.setCanOpenChat(true);
        API.showCursor(false);
    }

    call(func, args) {
        this.browser.call(func, args);
    }

    eval(string) {
        this.browser.eval(string);
    }
}

let employerCef = new WebBrowser(true);
var t = null;

API.onResourceStart.connect(function () {
    employerCef.show();
});

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "OnPlayerEnteredEmployer") {
        //args[0] to got?wka do odebrania 
        t = args[0].toString();
        employerCef.load("_Clientside/Resources/Employer/index.html");
    }
    else if (eventName === "OnPlayerExitEmployer") {
        employerCef.hide();
    }
});

function CloseEmployerCef() {

    employerCef.hide();

}

function SelectJob(value) {
    API.triggerServerEvent("OnPlayerSelectedJob", value);
}

function TakeMoneyJob() {
    API.triggerServerEvent("OnPlayerTakeMoneyJob");
}

function getInfo() {
    return t;
}