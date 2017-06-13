//Zmienne panelu logowania

var loginMenu: Menu;
var loginUsername: InputPanel;
var loginPassword: InputPanel;
var regUsername: InputPanel;
var regPassword: InputPanel;
var regConfirmPassword: InputPanel;
var loginNotification: any;

var index: number = 0;
var characters: any[];
let fullnameTextElement: TextElement;
let moneyTextElement: TextElement;
let bankTextElement: TextElement;
let selectFlag: boolean = false;

function menuLoginPanel()
{
    loginMenu = createMenu(2);
    let panel: Panel;
    let textElement: TextElement;

    //Login Screen - Strona 0
    //Login Header
    panel = loginMenu.createPanel(0, 12, 3, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText("Logowanie");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.Offset = 18;

    // Second Page Input Panel Example / Image
    panel = loginMenu.createPanel(0, 12, 4, 8, 2);
    panel.MainBackgroundColor(0, 0, 0, 255);
    panel.MainBackgroundImagePadding = 5;
    panel.MainBackgroundImage = "_Clientside/TypeScript/logo.png";

    //Formatka logowania
    panel = loginMenu.createPanel(0, 12, 6, 8, 7);
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText("Twój e-mail:");
    textElement.Color(255, 255, 255, 255);
    panel.addText("");
    textElement = panel.addText("Twoje hasło:");
    textElement.Color(255, 255, 255, 255);
    loginUsername = panel.addInput(0, 1, 8, 1);
    loginPassword = panel.addInput(0, 3, 8, 1);
    loginPassword.Protected = true;

    //Podpowiedź odnośnie nie posiadania konta

    //Guzik logowania
    panel = loginMenu.createPanel(0, 12, 13, 8, 1);
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

    //Wybór postaci - Strona 1
    //Header wyboru postaci
    panel = loginMenu.createPanel(1, 13, 4, 6, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText("V-Santos - Wybór postaci");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.Offset = 18;

    //Strzałka w lewo
    panel = loginMenu.createPanel(1, 12, 4, 1, 1);
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
    panel = loginMenu.createPanel(1, 19, 4, 1, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Tooltip = "Wybierz następną postać";
    panel.Function = incrementIndex;
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Header = true;
    textElement = panel.addText(">");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.HoverColor(0, 180, 255, 255);

    //Wyświetlanie obecnej postaci
    panel = loginMenu.createPanel(1, 12, 5, 8, 7);
    panel.MainBackgroundColor(0, 0, 0, 160);
    panel.addText("");
    fullnameTextElement = panel.addText("");
    fullnameTextElement.Color(255, 255, 255, 255);
    fullnameTextElement.Centered = true;
    moneyTextElement = panel.addText("");
    moneyTextElement.Color(255, 255, 255, 255);
    moneyTextElement.Centered = true;
    bankTextElement = panel.addText("");
    bankTextElement.Color(255, 255, 255, 255);
    bankTextElement.Centered = true;

    //Przycisk wyboru postaci
    panel = loginMenu.createPanel(1, 12, 12, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 160);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = selectCharacter;
    panel.Tooltip = "Wybierz postać jaką chcesz grać";
    textElement = panel.addText("Wybierz postać");
    textElement.Color(255, 255, 255, 255);
    textElement.HoverColor(0, 180, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;

    loginMenu.Blur = true;
    loginMenu.DisableOverlays(true);
    loginMenu.Ready = true;
}

function attemptLogin()
{
    let user = loginUsername.Input;
    let pass = loginPassword.Input;
    if (user.length < 5)
    {
        loginUsername.isError = true;
        return;
    }
    loginUsername.isError = false;
    if (pass.length < 5)
    {
        loginPassword.isError = true;
        return;
    }
    else
    {
        loginPassword.isError = false;
        API.triggerServerEvent("OnPlayerEnteredLoginData", user, pass);
        return;
    }
}

API.onServerEventTrigger.connect((eventName: string, args: System.Array<any>) => {
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
        characters = JSON.parse(args[0].toString());
        fillCharacterSelect();
        loginMenu.nextPage();
        selectFlag = true;
    }
});

API.onKeyDown.connect((sender: any, e: System.Windows.Forms.KeyEventArgs) =>
{
    if ((e.KeyCode == Keys.Left || e.KeyCode == Keys.A) && selectFlag)
    {
        //API.playSoundFrontEnd("GOLF_NEW_RECORD", "HUD_AWARDS");
        //API.playSoundFrontEnd("Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS");
        API.playSoundFrontEnd("Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS");
        decrementIndex();    
    }
    else if ((e.KeyCode == Keys.Right || e.KeyCode == Keys.D) && selectFlag)
    {
        API.playSoundFrontEnd("Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS");
        incrementIndex();
    }
    else if (e.KeyCode == Keys.Enter && selectFlag)
    {
        API.playSoundFrontEnd("Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS");
        selectCharacter();
    }
});

function selectCharacter()
{
    if (index >= 0 && index <= characters.length - 1)
    {
        API.triggerServerEvent("OnPlayerSelectedCharacter", index);
        loginMenu.Blur = false;
        loginMenu.DisableOverlays(false);
        loginMenu.killMenu();
        selectFlag = false;
        API.setActiveCamera(null);
    }
    else
    {
        createNotification(0, "Wybrano nieprawidłową postać!", 2000);
    }
}

function incrementIndex()
{
    if (index >= characters.length - 1) return;
    index++;
    fillCharacterSelect();
}

function decrementIndex()
{
    if (index <= 0) return;
    index--;
    fillCharacterSelect();
}

function fillCharacterSelect()
{
    fullnameTextElement.Text = characters[index].Name + " " + characters[index].Surname;
    moneyTextElement.Text = "Gotówka: $" + characters[index].Money;
    bankTextElement.Text = "Bank: $" + characters[index].BankMoney;
}