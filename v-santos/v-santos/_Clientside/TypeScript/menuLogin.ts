// Panel Variables
var loginMenu: Menu;
var loginUsername: InputPanel;
var loginPassword: InputPanel;
var regUsername: InputPanel;
var regPassword: InputPanel;
var regConfirmPassword: InputPanel;

// Main Menu Login Panel Function
function menuLoginPanel() {
    loginMenu = createMenu(4);
    let panel: Panel;
    let inputPanel: InputPanel;
    let textElement: TextElement;
    // EULA Screen - Page 0
    // EULA Header Panel
    panel = loginMenu.createPanel(0, 12, 4, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText("Warunki uzytkowania");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.Font = 1;
    // EULA Text
    panel = loginMenu.createPanel(0, 12, 5, 8, 7);
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText("Akceptujac niniejszy kontrakt zgadzasz sie z warunkami uzytkowania obowiazujacymi na serwerze. Pliki jakie otrzymaleś moga sluzyc tylko i wylacznie do gry na serwerze V-Santos.pl manipulacja nimi jest zabroniona. Zlamanie warunków kontraktu bedzie skutkowala permamentna banicja konta.");
    textElement.Color(255, 255, 255, 255);
    textElement.FontScale = 0.4;
    textElement.Font = 4;
    textElement.Centered = false;
    // EULA Button
    panel = loginMenu.createPanel(0, 12, 12, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 160);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = loginMenu.nextPage;
    panel.Tooltip = "Zaakceptuj warunki";
    textElement = panel.addText("Accept");
    textElement.Color(255, 255, 255, 255);
    textElement.HoverColor(0, 180, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    // Login Screen - Page 1
    // Login Header Panel
    panel = loginMenu.createPanel(1, 12, 4, 7, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText("Login");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = false;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.Offset = 18;
    // Go to Registration Button
    panel = loginMenu.createPanel(1, 19, 4, 1, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Tooltip = "Nowe konto?";
    panel.Function = loginMenu.nextPage;
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Header = true;
    textElement = panel.addText(">");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.HoverColor(0, 180, 255, 255);
    // Login Screen Login Forms and Such
    panel = loginMenu.createPanel(1, 12, 5, 8, 7);
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText("Twój e-mail:");
    textElement.Color(255, 255, 255, 255);
    panel.addText("");
    textElement = panel.addText("Twoje haslo:");
    textElement.Color(255, 255, 255, 255);
    loginUsername = panel.addInput(0, 1, 8, 1);
    loginPassword = panel.addInput(0, 3, 8, 1);
    loginPassword.Protected = true;
    // Login Screen Button
    panel = loginMenu.createPanel(1, 12, 12, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 160);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    //DEBUG
    //panel.Function = resource.menu_login.attemptLogin;
    panel.Tooltip = "Przejdź do logowania";
    textElement = panel.addText("Login");
    textElement.Color(255, 255, 255, 255);
    textElement.HoverColor(0, 180, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    // Login Screen - Page 2
    // Login Header Panel
    panel = loginMenu.createPanel(2, 12, 4, 7, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText("Rejestracja");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = false;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.Offset = 18;
    // Go to Login Button
    panel = loginMenu.createPanel(2, 19, 4, 1, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Tooltip = "Istniejace konto?";
    panel.Function = loginMenu.prevPage;
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Header = true;
    textElement = panel.addText("<");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.HoverColor(0, 180, 255, 255);
    // Registration Screen Login Forms and Such
    panel = loginMenu.createPanel(2, 12, 5, 8, 7);
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText("E-mail");
    textElement.Color(255, 255, 255, 255);
    panel.addText("");
    textElement = panel.addText("Haslo");
    textElement.Color(255, 255, 255, 255);
    panel.addText("");
    textElement = panel.addText("Password Again");
    textElement.Color(255, 255, 255, 255);
    regUsername = panel.addInput(0, 1, 8, 1);
    regPassword = panel.addInput(0, 3, 8, 1);
    regPassword.Protected = true;
    regConfirmPassword = panel.addInput(0, 5, 8, 1);
    regConfirmPassword.Protected = true;
    // Registration Screen Button
    panel = loginMenu.createPanel(2, 12, 12, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 160);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    //DEBUG
    //panel.Function = resource.menu_login.attemptRegister;
    panel.Tooltip = "Zarejestruj sie";
    textElement = panel.addText("Register");
    textElement.Color(255, 255, 255, 255);
    textElement.HoverColor(0, 180, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    // Menu is now ready, parse it and draw it.
    loginMenu.Blur = true;
    loginMenu.DisableOverlays(true);
    loginMenu.Ready = true;
}

//function attemptLogin() {
//    let user = loginUsername.Input;
//    let pass = loginPassword.Input;
//    if (user.length < 5) {
//        loginUsername.isError = true;
//        return;
//    }
//    loginUsername.isError = false;
//    if (pass.length < 5) {
//        loginPassword.isError = true;
//        return;
//    } else {
//        loginPassword.isError = false;
//        API.triggerServerEvent("clientLogin", user, pass);
//        loginMenu.Page = 3;
//        return;
//    }
//}

//function attemptRegister() {
//    let user = regUsername.Input;
//    let pass = regPassword.Input;
//    let pass_verify = regConfirmPassword.Input;
//    if (user.length < 5) {
//        regUsername.isError = true;
//        return;
//    }
//    regUsername.isError = false;

//    if (pass.length < 5 && pass_verify.length < 5) {
//        regPassword.isError = true;
//        regConfirmPassword.isError = true;
//        return;
//    }

//    regPassword.isError = false;
//    regConfirmPassword.isError = false;

//    if (pass === pass_verify) {
//        API.triggerServerEvent("clientRegistration", user, pass);
//        resource.MenuBuilder.setPage(3);
//        return;
//    } else {
//        regPassword.isError = true;
//        regConfirmPassword.isError = true;
//        return;
//    }
//}

API.onServerEventTrigger.connect((eventName: string, ...args: any[]) => {
    if (eventName == "ShowLoginCef")
    {
        if (args[0])
        {
            //CEF.load("_Clientside/Resources/Bootstrap/login.html");
            //var loginCamera = API.createCamera(args[1], args[2]);
            //API.setActiveCamera(loginCamera);
            API.showCursor(true);
            menuLoginPanel();
        }
        else {
            //resource.MenuBuilder.setPage(3);
        }
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