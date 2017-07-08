class AtmInfo {
    public money: string;
    public name: string;
    public accountNumber: string;

    constructor(money: string, name: string, accountNumber: string) {
        this.money = money;
        this.name = name;
        this.accountNumber = accountNumber;
    }
}

let atmMenu: Menu;
let customMoneyInput: InputPanel;

let prevPage: number;

function showAtmMenu(info: AtmInfo) {
    atmMenu = createMenu(4);
    let panel: Panel;
    let textElement: TextElement;

    for (var i = 0; i < 3; i++) {
        panel = atmMenu.createPanel(i, 12, 4, 3, 1);
        panel.MainBackgroundColor(0, 0, 0, 175);
        panel.HoverBackgroundColor(25, 25, 25, 160);
        panel.Hoverable = true;
        panel.Header = true;
        panel.Function = () => atmMenu.Page = 0;
        textElement = panel.addText("Informacje");
        textElement.Color(255, 255, 255, 255);
        textElement.Centered = true;
        textElement.FontScale = 0.6;

        panel = atmMenu.createPanel(i, 15, 4, 2, 1);
        panel.MainBackgroundColor(0, 0, 0, 175);
        panel.HoverBackgroundColor(25, 25, 25, 160);
        panel.Hoverable = true;
        panel.Header = true;
        panel.Function = () => atmMenu.Page = 1;
        textElement = panel.addText("Wpłata");
        textElement.Color(255, 255, 255, 255);
        textElement.Centered = true;
        textElement.FontScale = 0.6;

        panel = atmMenu.createPanel(i, 17, 4, 2, 1);
        panel.MainBackgroundColor(0, 0, 0, 175);
        panel.Header = true;
        panel.HoverBackgroundColor(25, 25, 25, 160);
        panel.Hoverable = true;
        panel.Function = () => atmMenu.Page = 2;
        textElement = panel.addText("Wypłata");
        textElement.Color(255, 255, 255, 255);
        textElement.Centered = true;
        textElement.FontScale = 0.6;


        panel = atmMenu.createPanel(i, 19, 4, 1, 1);
        panel.MainBackgroundColor(0, 0, 0, 175);
        panel.Header = true;
        panel.HoverBackgroundColor(150, 25, 25, 160);
        panel.Hoverable = true;
        panel.Function = () => atmMenu.killMenu();
        textElement = panel.addText("X");
        textElement.Color(255, 255, 255, 255);
        textElement.Centered = true;
        textElement.VerticalCentered = true;
        textElement.FontScale = 0.6;
    }

    //Strona 0
    //Informacje
    panel = atmMenu.createPanel(0, 12, 5, 8, 3);
    panel.MainBackgroundColor(0, 0, 0, 160);
    panel.Header = true;
    textElement = panel.addText(`Właściciel konta: ${info.name}`);
    textElement.Color(255, 255, 255, 255);
    textElement.FontScale = 0.5;
    textElement.Centered = true;

    textElement = panel.addText(`Numer konta: ${info.accountNumber}`);
    textElement.Color(255, 255, 255, 255);
    textElement.FontScale = 0.5;
    textElement.Centered = true;

    textElement = panel.addText(`Środki: $${info.money.toString()}`);
    textElement.Color(255, 255, 255, 255);
    textElement.FontScale = 0.5;
    textElement.Centered = true;

    //Strona 1
    //Wpłata
    panel = atmMenu.createPanel(1, 12, 5, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = () => {
        API.triggerServerEvent("OnPlayerAtmGive", 50);
        atmMenu.killMenu();
    };
    textElement = panel.addText("$50");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.FontScale = 0.6;

    panel = atmMenu.createPanel(1, 12, 6, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = () => {
        API.triggerServerEvent("OnPlayerAtmGive", 100);
        atmMenu.killMenu();
    };
    textElement = panel.addText("$100");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.FontScale = 0.6;

    panel = atmMenu.createPanel(1, 12, 7, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = () => {
        API.triggerServerEvent("OnPlayerAtmGive", 500);
        atmMenu.killMenu();
    };
    textElement = panel.addText("$500");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.FontScale = 0.6;

    panel = atmMenu.createPanel(1, 12, 8, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Header = true;
    panel.Function = () => {
        prevPage = 2;
        atmMenu.Page = 3;
    };
    textElement = panel.addText("Inne kwoty");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.FontScale = 0.6;

    //Strona 2
    //Wypłata
    panel = atmMenu.createPanel(2, 12, 5, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = () => {
        API.triggerServerEvent("OnPlayerAtmTake", 50);
        atmMenu.killMenu();
    };
    textElement = panel.addText("$50");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.FontScale = 0.6;

    panel = atmMenu.createPanel(2, 12, 6, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = () => {
        API.triggerServerEvent("OnPlayerAtmTake", 100);
        atmMenu.killMenu();
    };
    textElement = panel.addText("$100");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.FontScale = 0.6;

    panel = atmMenu.createPanel(2, 12, 7, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = () => {
        API.triggerServerEvent("OnPlayerAtmTake", 500);
        atmMenu.killMenu();
    };
    textElement = panel.addText("$500");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.FontScale = 0.6;

    panel = atmMenu.createPanel(2, 12, 8, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Header = true;
    panel.Function = () => {
        prevPage = 2;
        atmMenu.Page = 3;
    };
    textElement = panel.addText("Inne kwoty");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.FontScale = 0.6;

    //Strona 3
    //Custom kwota
    panel = atmMenu.createPanel(3, 13, 5, 1, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = () => {
        atmMenu.Page = prevPage;
    };
    textElement = panel.addText("<");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;

    panel = atmMenu.createPanel(3, 14, 5, 7, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText("Inne kwoty");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;

    panel = atmMenu.createPanel(3, 13, 6, 8, 2);
    panel.MainBackgroundColor(0, 0, 0, 175);
    textElement = panel.addText("Kwota:");
    textElement.Color(255, 255, 255, 255);
    textElement.FontScale = 0.6;
    textElement = panel.addText("");

    customMoneyInput = panel.addInput(0, 1, 8, 1);
    customMoneyInput.NumericOnly = true;

    panel = atmMenu.createPanel(3, 13, 8, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = () => {
        if (isNaN(parseInt(customMoneyInput.Input))) {
            customMoneyInput.isError = true;
            return;
        }        

        if (prevPage == 2 && parseInt(customMoneyInput.Input) > 5000) {
            API.sendNotification("Aby obsłużyć tak dużą kwotę musisz udać się do banku.");
            return;
        }

        //Wpłata
        if (prevPage == 1) {
            API.triggerServerEvent("OnPlayerAtmGive", customMoneyInput.Input);
            atmMenu.killMenu();
        }
        //Wypłata
        else if (prevPage == 2) {
            API.triggerServerEvent("OnPlayerAtmTake", customMoneyInput.Input);
            atmMenu.killMenu();
        }
    };
    textElement = panel.addText("OK");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.FontScale = 0.6;

    atmMenu.DisableOverlays(true);
    atmMenu.Ready = true;
}

API.onServerEventTrigger.connect((eventName: string, args: any[]) => {
    if (eventName == "OnPlayerEnteredAtm") {

        var info = JSON.parse(args[0]);
        showAtmMenu(new AtmInfo(info.BankMoney, info.FormatName, info.BankAccountNumber));
    }
    else if (eventName == "OnPlayerExitAtm") {
        if (atmMenu != null) {
            atmMenu.killMenu();
        }
    }
});