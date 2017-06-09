// Panel Variables
var loginMenu: Menu;
var loginUsername: InputPanel;
var loginPassword: InputPanel;
var regUsername: InputPanel;
var regPassword: InputPanel;
var regConfirmPassword: InputPanel;
var loginNotification: any;

let index: number;
let characters: any[];
let characterIdTextElement: TextElement;
let fullnameTextElement: TextElement;
let moneyTextElement: TextElement;
let bankTextElement: TextElement;

// Main Menu Login Panel Function
function menuLoginPanel()
{
    loginMenu = createMenu(4);
    let panel: Panel;
    let inputPanel: InputPanel;
    let textElement: TextElement;

    //Warunki użytkowania - Strona 0
    //Warunki header
    panel = loginMenu.createPanel(0, 12, 4, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText("Warunki uzytkowania");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.Font = 1;

    //Warunki użytkowania tekst
    panel = loginMenu.createPanel(0, 12, 5, 8, 7);
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText("Akceptując niniejszy kontrakt zgadzasz się z warunkami użytkowania obowiązującymi na serwerze. Pliki jakie otrzymałeś moga służyć tylko i wyłącznie do rozgrywki na serwerze V-Santos.pl manipulacja nimi jest ściśle zabroniona. Złamanie warunków kontraktu będzie skutkowało permamentną banicją konta.");
    textElement.Color(255, 255, 255, 255);
    textElement.FontScale = 0.4;
    textElement.Font = 4;
    textElement.Centered = false;

    //Guzik warunków użytkowania
    panel = loginMenu.createPanel(0, 12, 12, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 160);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = loginMenu.nextPage;
    panel.Tooltip = "Zaakceptuj warunki użytkowania";
    textElement = panel.addText("OK");
    textElement.Color(255, 255, 255, 255);
    textElement.HoverColor(0, 180, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;

    //Login Screen - Strona 1
    //Login Header
    panel = loginMenu.createPanel(1, 12, 4, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText("Login");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = false;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.Offset = 18;

    //Formatka logowania
    panel = loginMenu.createPanel(1, 12, 5, 8, 7);
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText("Twój e-mail:");
    textElement.Color(255, 255, 255, 255);
    panel.addText("");
    textElement = panel.addText("Twoje hasło:");
    textElement.Color(255, 255, 255, 255);
    loginUsername = panel.addInput(0, 1, 8, 1);
    loginPassword = panel.addInput(0, 3, 8, 1);
    loginPassword.Protected = true;

    //Guzik logowania
    panel = loginMenu.createPanel(1, 12, 12, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 160);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = attemptLogin;
    panel.Tooltip = "Zaloguj się";
    textElement = panel.addText("Login");
    textElement.Color(255, 255, 255, 255);
    textElement.HoverColor(0, 180, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;

    //Wybór postaci - Strona 2
    //Strzałka w lewo
    panel = loginMenu.createPanel(2, 19, 4, 1, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Tooltip = "Wybierz poprzednią postać";
    panel.Function = decrementIndex;
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Header = true;
    textElement = panel.addText("<");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.HoverColor(0, 180, 255, 255);

    //Strzałka w prawo
    panel = loginMenu.createPanel(2, 19, 4, 1, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Tooltip = "Wybierz następną postać";
    panel.Function = decrementIndex;
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Header = true;
    textElement = panel.addText(">");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.HoverColor(0, 180, 255, 255);

    //Header
    panel = loginMenu.createPanel(2, 12, 4, 7, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText("Wybór postaci");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = false;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.Offset = 18;

    //Wyświetlanie obecnej postaci
    //panel = loginMenu.createPanel(2, 12, 5, 8, 7);
    //panel.MainBackgroundColor(0, 0, 0, 160);
    //characterIdTextElement = panel.addText("");
    //fullnameTextElement.Color(255, 255, 255, 255);
    //panel.addText("");
    //fullnameTextElement = panel.addText("");
    //fullnameTextElement.Color(255, 255, 255, 255);
    //panel.addText("");
    //moneyTextElement = panel.addText("");
    //moneyTextElement.Color(255, 255, 255, 255);
    //panel.addText("");
    //bankTextElement = panel.addText("");
    //bankTextElement.Color(255, 255, 255, 255);
    //panel.addText("");

    ////Przycisk wyboru postaci
    //panel = loginMenu.createPanel(2, 12, 12, 8, 1);
    //panel.MainBackgroundColor(0, 0, 0, 160);
    //panel.HoverBackgroundColor(25, 25, 25, 160);
    //panel.Hoverable = true;
    //panel.Function = selectCharacter;
    //panel.Tooltip = "Wybierz postać";
    //textElement = panel.addText("Wybierz postać");
    //textElement.Color(255, 255, 255, 255);
    //textElement.HoverColor(0, 180, 255, 255);
    //textElement.Centered = true;
    //textElement.VerticalCentered = true;

    loginMenu.Blur = true;
    loginMenu.DisableOverlays(true);
    loginMenu.Ready = true;
}

function attemptLogin() {
    let user = loginUsername.Input;
    let pass = loginPassword.Input;
    if (user.length < 5) {
        loginUsername.isError = true;
        return;
    }
    loginUsername.isError = false;
    if (pass.length < 5) {
        loginPassword.isError = true;
        return;
    } else {
        loginPassword.isError = false;
        API.triggerServerEvent("OnPlayerEnteredLoginData", user, pass);
        return;
    }
}

API.onServerEventTrigger.connect(function (eventName: string, ...args: Array<any>)
{
    if (eventName == "ShowLoginMenu")
    {
        var loginCamera = API.createCamera(new Vector3(-1650, -1030, 50), new Vector3(0, 0, 180));
        API.setActiveCamera(loginCamera);
        menuLoginPanel();
    }
    else if (eventName == "ShowNotification")
    {
        createNotification(0, args[0].toString(), parseInt(args[1]));
    }
    else if (eventName == "ShowCharacterSelectMenu")
    {
        //args[0] json postaci
        createNotification(0, "Używaj strzałek, aby przewijać swoje postacie.", 3000);
        characters = JSON.parse(args[0]);
        loginMenu.Page = 2;
    }
    //else if (eventName == "ShowCharacterSelectCef") {
    //    if (args[0]) {
    //        charactersList = args[1];
    //        CEF.load("_Clientside/Resources/Characters/index.html");
    //    }
    //    else {
    //        CEF.hide();
    //        API.setActiveCamera(null);
    //    }
    //}
});

function selectCharacter()
{
    if (index >= 0 && index <= characters.length - 1) {
        API.triggerServerEvent("OnPlayerSelectedCharacter", index);
    } else {
        createNotification(0, "Wybrano nieprawidłową postać!", 2000);
    }
}

function incrementIndex()
{
    if (index >= characters.length - 1) return;
    index++;
}

function decrementIndex()
{
    if (index <= 0) return;
    index--;
}