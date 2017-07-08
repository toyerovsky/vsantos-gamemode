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
    loginMenu = createMenu(4);
    let panel: Panel;
    let textElement: TextElement;

    //Login Screen - Strona 0
    //Login Header
    panel = loginMenu.createPanel(0, 12, 5, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText("V-Santos.pl - Logowanie");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;

    //Formatka logowania
    panel = loginMenu.createPanel(0, 12, 6, 8, 7);
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText("Twój e-mail:");
    textElement.Color(255, 255, 255, 255);
    textElement.FontScale = 0.5;
    panel.addText("");
    textElement = panel.addText("Twoje hasło:");
    textElement.Color(255, 255, 255, 255);
    textElement.FontScale = 0.5;
    loginUsername = panel.addInput(0, 1, 8, 1);
    loginPassword = panel.addInput(0, 3, 8, 1);
    loginPassword.Protected = true;

    //Podpowiedź odnośnie nie posiadania konta
    //Guzik rejestracja
    panel = loginMenu.createPanel(0, 12, 13, 4, 1);
    panel.MainBackgroundColor(0, 0, 0, 160);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = () => loginMenu.Page = 2;
    textElement = panel.addText("Rejestracja");
    textElement.Color(255, 255, 255, 255);
    textElement.HoverColor(75, 109, 35, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;

    //Guzik logowania
    panel = loginMenu.createPanel(0, 16, 13, 4, 1);
    panel.MainBackgroundColor(0, 0, 0, 160);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = attemptLogin;
    textElement = panel.addText("Login");
    textElement.Color(255, 255, 255, 255);
    textElement.HoverColor(75, 109, 35, 255);
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
    textElement.HoverColor(75, 109, 35, 255);

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
    textElement.HoverColor(75, 109, 35, 255);

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
    textElement.HoverColor(75, 109, 35, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;

    //Strona 2
    //Rejestracja
    panel = loginMenu.createPanel(2, 12, 4, 1, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Tooltip = "Wróć";
    panel.Function = () => loginMenu.Page = 0;
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Header = true;
    textElement = panel.addText("<");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.HoverColor(75, 109, 35, 255);

    panel = loginMenu.createPanel(2, 13, 4, 7, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText("V-Santos.pl - Rejestracja");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;

    panel = loginMenu.createPanel(2, 12, 5, 8, 4);
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText("Aby rozpocząć rozgrywkę dokonaj rejestracji na naszym forum. https://forum.v-santos.pl Następnie załóż swoją postać w panelu użytkownika. https://cp.v-santos.pl");
    textElement.Color(255, 255, 255, 255);
    textElement.FontScale = 0.5;
    textElement.Centered = true;
    textElement.VerticalCentered = true;

    //Strona 3
    //Brak postaci
    
    panel = loginMenu.createPanel(3, 12, 3, 8, 8);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText("V-Santos.pl - Brak postaci");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;

    panel = loginMenu.createPanel(3, 12, 6, 8, 4);
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText("Nie mogliśmy odnaleźć żadnych postaci powiązanych z Twoim kontem. https://cp.v-santos.pl Aby rozpocząć rozgrywkę załóż postać w naszym panelu użytkownika. Jeżeli już to zrobiłeś a mimo to widzisz ten komunikat, skontaktuj się z administratorem.");
    textElement.Color(255, 255, 255, 255);
    textElement.FontScale = 0.5;
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
        characters = JSON.parse(args[0].toString());

        if (characters == null && characters.length == 0) {
            loginMenu.Page = 3;
            return;
        }
        createNotification(0, "Używaj strzałek, aby przewijać swoje postacie.", 3000);
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
        decrementIndex();    
    }
    else if ((e.KeyCode == Keys.Right || e.KeyCode == Keys.D) && selectFlag)
    {
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
    API.playSoundFrontEnd("Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS");
    index++;
    fillCharacterSelect();
}

function decrementIndex()
{
    if (index <= 0) return;
    API.playSoundFrontEnd("Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS");
    index--;
    fillCharacterSelect();
}

function fillCharacterSelect()
{
    fullnameTextElement.Text = characters[index].Name + " " + characters[index].Surname;
    moneyTextElement.Text = "Gotówka: $" + characters[index].Money;
    bankTextElement.Text = "Bank: $" + characters[index].BankMoney;
}