/// <reference path="../../types-gtanetwork/index.d.ts" />
class BuildingInfo {
}
let drawScaleform = false;
let scaleForm = null;
let buildingMenu = null;
var menuPool = null;
let buildings = null;
let buildingName;
let buildingDescription;
let buildingEnterCharge;
API.onKeyDown.connect((sender, e) => {
    if (e.KeyCode === Keys.E && API.hasEntitySyncedData(API.getLocalPlayer(), "DoorsTarget")) {
        API.triggerServerEvent("PassDoors");
    }
    else if (e.KeyCode === Keys.K && API.hasEntitySyncedData(API.getLocalPlayer(), "DoorsTarget")) {
        API.triggerServerEvent("KnockDoors");
    }
});
function getBuilding(name) {
    for (var i = 0; i < buildings.length; i++) {
        if (buildings[i].name == name) {
            return buildings[i];
        }
    }
    return null;
}
API.onServerEventTrigger.connect((eventName, args) => {
    if (eventName == "DrawBuildingComponents") {
        scaleForm = API.requestScaleform('instructional_buttons');
        scaleForm.CallFunction('SET_DATA_SLOT_EMPTY');
        if (args[0]) {
            scaleForm.CallFunction('SET_DATA_SLOT', 0, 'E', 'Wejdź');
            scaleForm.CallFunction('SET_DATA_SLOT', 1, 'K', 'Zapukaj');
            showBuildingPanel(args[1]);
        }
        else {
            scaleForm.CallFunction('SET_DATA_SLOT', 0, 'E', 'Wyjdź');
            scaleForm.CallFunction('SET_DATA_SLOT', 1, 'K', 'Zapukaj');
        }
        scaleForm.CallFunction('DRAW_INSTRUCTIONAL_BUTTONS', -1);
        drawScaleform = true;
    }
    else if (eventName == "DisposeBuildingComponents") {
        drawScaleform = false;
        if (buildingMenu != null)
            buildingMenu.killMenu();
    }
    else if (eventName == "ShowBuildingManagePanel") {
        var buildingInfo = new BuildingInfo();
        var info = args[0];
        buildingInfo.name = info[0];
        buildingInfo.description = info[1];
        buildingInfo.enterCharge = info[2];
        showBuildingManagePanel(buildingInfo);
    }
    else if (eventName == "ShowAdminBuildingMenu") {
        menuPool = API.getMenuPool();
        var menu = API.createMenu("Dodaj budynek", 0, 0, 6);
        buildings = [];
        var newBuilding;
        var list = JSON.parse(args[0]);
        for (var i = 0; i < list.length; i++) {
            newBuilding = new BuildingInfo();
            newBuilding.name = list[i].Name;
            newBuilding.internalPosition = list[i].InternalPostion;
            buildings.push(newBuilding);
        }
        var interiorNames = new List(String);
        for (var j = 0; j < buildings.length; j++) {
            interiorNames.Add(buildings[j].name);
        }
        var nameItem = API.createMenuItem("Nazwa", "Ustal nazwe budynku.");
        menu.AddItem(nameItem);
        var descriptionItem = API.createMenuItem("Opis", "Ustal opis budynku. (opcjonalne)");
        menu.AddItem(descriptionItem);
        var interiorsItem = API.createListItem("Interior", "Ustal interior budynku", interiorNames, 0);
        menu.AddItem(interiorsItem);
        var costItem = API.createMenuItem("Koszt", "Ustal cenę budynku");
        menu.AddItem(costItem);
        var saveItem = API.createMenuItem("Zapisz", "");
        menu.AddItem(saveItem);
        menuPool.Add(menu);
        menu.Visible = true;
        var nameResult = null;
        var descriptionResult = "";
        var interiorResult = null;
        var costResult = null;
        menu.OnItemSelect.connect((sender, item, index) => {
            if (item === nameItem) {
                nameResult = API.getUserInput("Nazwa budynku", 30);
            }
            else if (item === costItem) {
                costResult = API.getUserInput("Cena", 10);
                if (costResult.trim() == "" || isNaN(costResult)) {
                    API.sendNotification("Wprowadzono nieporawny koszt.");
                    costResult = null;
                }
            }
            else if (item == descriptionItem) {
                descriptionResult = API.getUserInput("Opis budynku", 100);
            }
            else if (item == saveItem) {
                if (nameResult != null &&
                    costResult != null &&
                    interiorResult != null) {
                    API.triggerServerEvent("AddBuilding", nameResult, costResult, interiorResult, descriptionResult);
                    menu.Visible = false;
                }
                else {
                    API.sendNotification("Dane nie zostały zaincjalizowane.");
                }
            }
        });
        interiorsItem.OnListChanged.connect((sender, newIndex) => {
            interiorResult = sender.IndexToItem(sender.Index);
            let building = getBuilding(interiorResult);
            if (building != null) {
                API.triggerServerEvent("ChangePosition", building.internalPosition);
            }
        });
    }
});
API.onUpdate.connect(() => {
    if (drawScaleform)
        API.renderScaleform(scaleForm, 0, 0, 1280, 720);
});
function showBuildingPanel(buildingInfo) {
    if (buildingMenu != null)
        buildingMenu.killMenu();
    buildingMenu = createMenu(1);
    let panel;
    let textElement;
    //Panel budynku
    //Header z nazwą budynku
    panel = buildingMenu.createPanel(0, 12, 4, 7, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText(buildingInfo[0]);
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.Offset = 18;
    //Guzik zamykania
    panel = buildingMenu.createPanel(0, 19, 4, 1, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    panel.HoverBackgroundColor(150, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = () => buildingMenu.killMenu();
    textElement = panel.addText("X");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.Offset = 18;
    //Opis budynku
    panel = buildingMenu.createPanel(0, 12, 5, 8, 7);
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText(buildingInfo[1]);
    textElement.Color(255, 255, 255, 255);
    panel.addText("");
    //Opłata za przejście
    if (buildingInfo[2] !== "") {
        textElement = panel.addText(`Opłata za przejście $${buildingInfo[2]}`);
        textElement.Color(255, 255, 255, 255);
        panel.addText("");
    }
    //Guzik kupowania 
    if (buildingInfo[3] !== "") {
        textElement = panel.addText(`Cena $${buildingInfo[3]}`);
        textElement.Color(255, 255, 255, 255);
        panel = buildingMenu.createPanel(0, 12, 12, 8, 1);
        panel.MainBackgroundColor(0, 0, 0, 160);
        panel.HoverBackgroundColor(25, 25, 25, 160);
        panel.Hoverable = true;
        panel.Function = () => { API.triggerServerEvent("BuyBuilding"); };
        panel.Tooltip = "Kup ten budynek";
        textElement = panel.addText("Kup");
        textElement.Color(255, 255, 255, 255);
        textElement.HoverColor(0, 180, 255, 255);
        textElement.Centered = true;
        textElement.VerticalCentered = true;
    }
    buildingMenu.Ready = true;
}
function showBuildingManagePanel(buildingInfo) {
    if (buildingMenu != null)
        buildingMenu.killMenu();
    buildingMenu = createMenu(1);
    let panel;
    let textElement;
    //Panel budynku
    //Header z nazwą budynku
    panel = buildingMenu.createPanel(0, 12, 4, 7, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText("Zarządzanie budynkiem");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.Offset = 18;
    //Guzik zamykania
    panel = buildingMenu.createPanel(0, 19, 4, 1, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    panel.HoverBackgroundColor(150, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = () => buildingMenu.killMenu();
    textElement = panel.addText("X");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.Offset = 18;
    //Nazwa budynku
    panel = buildingMenu.createPanel(0, 12, 5, 8, 7);
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText("Nazwa:");
    textElement.Color(255, 255, 255, 255);
    panel.addText("");
    //Opis budynku
    textElement = panel.addText("Opis:");
    textElement.Color(255, 255, 255, 255);
    panel.addText("");
    panel.addText("");
    panel.addText("");
    //Opłata za przejście
    textElement = panel.addText("Opłata za przejście:");
    textElement.Color(255, 255, 255, 255);
    panel.addText("");
    buildingName = panel.addInput(0, 1, 8, 1);
    buildingName.Input = buildingInfo.name;
    buildingDescription = panel.addInput(0, 3, 8, 1);
    buildingDescription.Input = buildingInfo.description;
    buildingEnterCharge = panel.addInput(0, 7, 8, 1);
    buildingEnterCharge.Input = buildingInfo.enterCharge.toString();
    buildingEnterCharge.NumericOnly = true;
    //Guzik zapisywania
    panel = buildingMenu.createPanel(0, 12, 12, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 160);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = () => { API.triggerServerEvent("EdingBuildingInfo", buildingName.Input, buildingDescription.Input, buildingEnterCharge.Input); };
    panel.Tooltip = "Zapisz bieżące informacje";
    textElement = panel.addText("Zapisz");
    textElement.Color(255, 255, 255, 255);
    textElement.HoverColor(0, 180, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    buildingMenu.Ready = true;
}
//Zmienne panelu logowania
var loginMenu;
var loginUsername;
var loginPassword;
var regUsername;
var regPassword;
var regConfirmPassword;
var loginNotification;
var index = 0;
var characters;
let fullnameTextElement;
let moneyTextElement;
let bankTextElement;
let selectFlag = false;
function menuLoginPanel() {
    loginMenu = createMenu(2);
    let panel;
    let textElement;
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
    }
    else {
        loginPassword.isError = false;
        API.triggerServerEvent("OnPlayerEnteredLoginData", user, pass);
        return;
    }
}
API.onServerEventTrigger.connect((eventName, args) => {
    if (eventName == "ShowLoginMenu") {
        var loginCamera = API.createCamera(new Vector3(-1650, -1030, 50), new Vector3(0, 0, 180));
        API.setActiveCamera(loginCamera);
        menuLoginPanel();
    }
    else if (eventName == "ShowNotification") {
        createNotification(0, args[0].toString(), parseInt(args[1]));
    }
    else if (eventName == "ShowCharacterSelectMenu") {
        //args[0] json postaci
        createNotification(0, "Używaj strzałek, aby przewijać swoje postacie.", 3000);
        characters = JSON.parse(args[0].toString());
        fillCharacterSelect();
        loginMenu.nextPage();
        selectFlag = true;
    }
});
API.onKeyDown.connect((sender, e) => {
    if ((e.KeyCode == Keys.Left || e.KeyCode == Keys.A) && selectFlag) {
        //API.playSoundFrontEnd("GOLF_NEW_RECORD", "HUD_AWARDS");
        //API.playSoundFrontEnd("Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS");
        decrementIndex();
    }
    else if ((e.KeyCode == Keys.Right || e.KeyCode == Keys.D) && selectFlag) {
        incrementIndex();
    }
    else if (e.KeyCode == Keys.Enter && selectFlag) {
        API.playSoundFrontEnd("Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS");
        selectCharacter();
    }
});
function selectCharacter() {
    if (index >= 0 && index <= characters.length - 1) {
        API.triggerServerEvent("OnPlayerSelectedCharacter", index);
        loginMenu.Blur = false;
        loginMenu.DisableOverlays(false);
        loginMenu.killMenu();
        selectFlag = false;
        API.setActiveCamera(null);
    }
    else {
        createNotification(0, "Wybrano nieprawidłową postać!", 2000);
    }
}
function incrementIndex() {
    if (index >= characters.length - 1)
        return;
    API.playSoundFrontEnd("Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS");
    index++;
    fillCharacterSelect();
}
function decrementIndex() {
    if (index <= 0)
        return;
    API.playSoundFrontEnd("Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS");
    index--;
    fillCharacterSelect();
}
function fillCharacterSelect() {
    fullnameTextElement.Text = characters[index].Name + " " + characters[index].Surname;
    moneyTextElement.Text = "Gotówka: $" + characters[index].Money;
    bankTextElement.Text = "Bank: $" + characters[index].BankMoney;
}
/// <reference path="../../types-gtanetwork/index.d.ts" />
var screenX = API.getScreenResolutionMantainRatio().Width;
var screenY = API.getScreenResolutionMantainRatio().Height;
var panelMinX = (screenX / 32);
var panelMinY = (screenY / 18);
var button = null;
var panel = null;
var image = null;
var notification = null;
var notifications = [];
var textnotification = null;
var textnotifications = [];
var padding = 10;
var selectedInput = null;
// Menu Properties
var tabIndex = [];
var tab = 0;
var menuElements = [];
var isReady = false;
var currentPage = 0;
var clickDelay = new Date().getTime();
// Current Menu
var currentMenu = null;
// On-Update Event -- Draws all of our stuff.
API.onUpdate.connect(() => {
    // Notifications can be global.
    drawNotification();
    drawTextNotification();
    if (!isReady) {
        return;
    }
    for (var i = 0; i < menuElements[currentPage].length; i++) {
        if (!isReady) {
            break;
        }
        menuElements[currentPage][i].draw();
        menuElements[currentPage][i].isHovered();
        menuElements[currentPage][i].isClicked();
    }
});
/**
 * Initialize how many pages our menu is going to have.
 * @param pages - Number of pages.
 */
function createMenu(pages) {
    if (currentMenu !== null) {
        isReady = false;
        currentPage = 0;
        selectedInput = null;
        tabIndex = [];
        menuElements = [];
    }
    currentMenu = new Menu(pages);
    return currentMenu;
}
class Menu {
    constructor(pages) {
        if (Array.isArray(menuElements[0])) {
            return;
        }
        for (var i = 0; i < pages; i++) {
            let emptyArray = [];
            menuElements.push(emptyArray);
        }
        this._blur = false;
    }
    /** Start drawing our menu. */
    set Ready(value) {
        isReady = value;
        currentPage = 0;
    }
    get Ready() {
        return isReady;
    }
    /** Used to blur the background behind the menu. */
    set Blur(value) {
        this._blur = value;
        if (value) {
            API.callNative("_TRANSITION_TO_BLURRED", 3000);
        }
        else {
            API.callNative("_TRANSITION_FROM_BLURRED", 3000);
        }
    }
    get Blur() {
        return this._blur;
    }
    /** Get the current menu page number or set the current menu page number */
    set Page(value) {
        currentPage = value;
    }
    get Page() {
        return currentPage;
    }
    DisableOverlays(value) {
        this._overlays = value;
        if (value) {
            API.setHudVisible(false);
            API.setChatVisible(false);
            API.setCanOpenChat(false);
            API.showCursor(true);
            return;
        }
        else {
            API.setHudVisible(true);
            API.setChatVisible(true);
            API.setCanOpenChat(true);
            API.showCursor(false);
            return;
        }
    }
    /**
     *  Delete any open menu instances.
     */
    killMenu() {
        isReady = false;
        API.showCursor(false);
        API.setHudVisible(true);
        API.setChatVisible(true);
        API.setCanOpenChat(true);
        API.callNative("_TRANSITION_FROM_BLURRED", 3000);
    }
    nextPage() {
        if (currentPage + 1 > menuElements.length - 1) {
            currentPage = 0;
            return;
        }
        currentPage++;
    }
    prevPage() {
        if (currentPage - 1 < 0) {
            currentPage = menuElements.length - 1;
            return;
        }
        currentPage--;
    }
    /**
     * Create a new panel.
     * @param page - The page you would like to add panels to.
     * @param xStart
     * @param yStart
     * @param xGridWidth
     * @param yGridHeight
     */
    createPanel(page, xStart, yStart, xGridWidth, yGridHeight) {
        let newPanel = new Panel(page, xStart, yStart, xGridWidth, yGridHeight);
        menuElements[page].push(newPanel);
        return newPanel;
    }
}
class PlayerTextNotification {
    constructor(text) {
        let playerPos = API.getEntityPosition(API.getLocalPlayer()).Add(new Vector3(0, 0, 1));
        let point = API.worldToScreenMantainRatio(playerPos);
        this._xPos = Point.Round(point).X;
        this._yPos = Point.Round(point).Y;
        this._drawing = true;
        this._alpha = 255;
        this._text = text;
        this._increment = -1;
        this._lastUpdateAlpha = new Date().getTime();
        this._lastUpdateTextPosition = new Date().getTime();
        this._r = 0;
        this._g = 0;
        this._b = 0;
    }
    draw() {
        if (!this._drawing) {
            return;
        }
        if (new Date().getTime() > this._lastUpdateAlpha + 35) {
            this._lastUpdateAlpha = new Date().getTime();
            this._alpha -= 5;
        }
        if (new Date().getTime() > this._lastUpdateTextPosition + 100) {
            this._yPos -= 0.3;
        }
        API.drawText(this._text, this._xPos, this._yPos, 0.4, this._r, this._g, this._b, this._alpha, 4, 1, true, true, 500);
        if (this._alpha <= 0) {
            this.cleanUpNotification();
        }
    }
    setColor(r, g, b) {
        this._r = r;
        this._g = g;
        this._b = b;
    }
    cleanUpNotification() {
        this._drawing = false;
        textnotification = null;
    }
    returnType() {
        return "PlayerTextNotification";
    }
}
class ProgressBar {
    constructor(x, y, width, height, currentProgress) {
        this._xPos = x * panelMinX;
        this._yPos = y * panelMinY;
        this._width = width * panelMinX - 10;
        this._height = height * panelMinY - 10;
        this._currentProgress = currentProgress;
        this._r = 0;
        this._g = 0;
        this._b = 0;
        this._alpha = 255;
        this._drawText = true;
        API.sendChatMessage("Created");
    }
    draw() {
        API.drawRectangle(this._xPos + 5, this._yPos + 5, ((Math.round(this._width) / 100) * this._currentProgress), this._height, this._r, this._g, this._b, this._alpha);
        if (this._drawText) {
            API.drawText("" + Math.round(this._currentProgress), this._xPos + (((Math.round(this._width) / 100) * this._currentProgress) / 2), this._yPos, 0.5, 255, 255, 255, 255, 4, 1, false, true, 100);
        }
    }
    setColor(r, g, b) {
        this._r = r;
        this._g = g;
        this._b = b;
    }
    /** The alpha property for the bar. */
    set Alpha(value) {
        this._alpha = value;
    }
    /** Draw any text? */
    set DrawText(value) {
        this._drawText = value;
    }
    addProgress(value) {
        if (this._currentProgress + value > 100) {
            this._currentProgress = 100;
            return;
        }
        this._currentProgress += value;
    }
    subtractProgress(value) {
        if (this._currentProgress - value < 0) {
            this._currentProgress = 0;
            return;
        }
        this._currentProgress -= value;
    }
    setProgressAmount(value) {
        if (value >= 100) {
            this._currentProgress = 100;
            return;
        }
        if (value <= 0) {
            this._currentProgress = 0;
            return;
        }
        this._currentProgress = value;
        return;
    }
    returnProgressAmount() {
        return this._currentProgress;
    }
    returnType() {
        return "ProgressBar";
    }
}
class Notification {
    constructor(text, displayTime) {
        this._currentPosX = 26 * panelMinX; // Starting Position
        this._currentPosY = screenY; // Starting Position Y
        this._targetX = 26 * panelMinX; // Ending Position
        this._targetY = 15 * panelMinY; // Ending Position Y
        this._width = panelMinX * 5;
        this._height = panelMinY * 3;
        // Text Settings
        this._text = text;
        this._r = 255;
        this._g = 165;
        this._b = 0;
        this._offset = 0;
        this._textScale = 0.5;
        // Animation Settings
        this._lastUpdateTime = new Date().getTime(); //ms
        this._alpha = 255;
        this._displayTime = displayTime;
        this._incrementer = 0;
        // Sound Settings
        this._sound = true;
    }
    draw() {
        if (notification !== this) {
            return;
        }
        if (this._sound) {
            this._sound = false;
            API.playSoundFrontEnd("GOLF_NEW_RECORD", "HUD_AWARDS");
        }
        // Starts below max screen.
        API.drawRectangle(this._currentPosX, this._currentPosY - 5, Math.round(this._width), 5, this._r, this._g, this._b, this._alpha - 30);
        API.drawRectangle(this._currentPosX, this._currentPosY, Math.round(this._width), this._height, 0, 0, 0, this._alpha - 30);
        API.drawText(this._text, this._offset + this._currentPosX + (Math.round(this._width) / 2), this._currentPosY + (this._height / 4), this._textScale, 255, 255, 255, this._alpha, 4, 1, false, false, Math.round(this._width) - padding);
        this.animate();
    }
    animate() {
        // Did we reach our goal?
        if (this._currentPosY <= this._targetY) {
            this._currentPosY = this._targetY;
            // Ready to fade?
            if (new Date().getTime() > this._lastUpdateTime + this._displayTime) {
                this.fade();
                return;
            }
            return;
        }
        this._lastUpdateTime = new Date().getTime();
        // If not let's reach our goal.
        if (this._currentPosY <= this._targetY + (this._height / 6)) {
            this._currentPosY -= 3;
            return;
        }
        else {
            this._currentPosY -= 5;
            return;
        }
    }
    fade() {
        if (this._alpha <= 0) {
            this.cleanUpNotification();
            return;
        }
        this._alpha -= 5;
        return;
    }
    cleanUpNotification() {
        notification = null;
    }
    setText(value) {
        this._text = value;
    }
    setColor(r, g, b) {
        this._r = r;
        this._g = g;
        this._b = b;
    }
    setTextScale(value) {
        this._textScale = value;
    }
    isHovered() {
        return;
    }
    isClicked() {
        return;
    }
    returnType() {
        return "Notification";
    }
}
class TextElement {
    // Constructor
    constructor(text, x, y, width, height, line) {
        this._xPos = x;
        this._yPos = y + (panelMinY * line);
        this._width = width;
        this._height = height;
        this._text = text;
        this._fontScale = 0.6;
        this._centered = false;
        this._centeredVertically = false;
        this._font = 4;
        this._fontR = 255;
        this._fontG = 255;
        this._fontB = 255;
        this._fontAlpha = 255;
        this._hoverTextAlpha = 255;
        this._hoverTextR = 255;
        this._hoverTextG = 255;
        this._hoverTextB = 255;
        this._offset = 0;
        this._padding = 10;
        this._hovered = false;
        this._shadow = false;
        this._outline = false;
    }
    draw() {
        if (this._centered && this._centeredVertically) {
            this.drawAsCenteredAll();
            return;
        }
        if (this._centered) {
            this.drawAsCentered();
            return;
        }
        if (this._centeredVertically) {
            this.drawAsCenteredVertically();
            return;
        }
        this.drawAsNormal();
    }
    //** Sets the text */
    set Text(value) {
        this._text = value;
    }
    //** Is this text element in a hover state? */
    set Hovered(value) {
        this._hovered = value;
    }
    get Hovered() {
        return this._hovered;
    }
    //** Sets the color of the main text. A = Alpha */
    Color(r, g, b, a) {
        this._fontR = r;
        this._fontG = g;
        this._fontB = b;
        this._fontAlpha = a;
    }
    HoverColor(r, g, b, a) {
        this._hoverTextR = r;
        this._hoverTextG = g;
        this._hoverTextB = b;
        this._hoverTextAlpha = a;
    }
    /** Sets the color for RGB of R type. Max of 255 */
    set R(value) {
        this._fontR = value;
    }
    /** Gets the color for RGB of R type. */
    get R() {
        return this._fontR;
    }
    /** Sets the color for RGB of G type. Max of 255 */
    set G(value) {
        this._fontG = value;
    }
    /** Gets the color for RGB of G type. */
    get G() {
        return this._fontG;
    }
    /** Sets the color for RGB of B type. Max of 255 */
    set B(value) {
        this._fontB = value;
    }
    /** Gets the color for RGB of B type. */
    get B() {
        return this._fontB;
    }
    /** Sets the font Alpha property */
    set Alpha(value) {
        this._fontAlpha = value;
    }
    get Alpha() {
        return this._fontAlpha;
    }
    /** Sets the font Alpha property */
    set HoverAlpha(value) {
        this._hoverTextAlpha = value;
    }
    get HoverAlpha() {
        return this._hoverTextAlpha;
    }
    /** Sets the hover color for the text RGB of R type. */
    set HoverR(value) {
        this._hoverTextR = value;
    }
    get HoverR() {
        return this._hoverTextR;
    }
    /** Sets the hover color for the text RGB of G type. */
    set HoverG(value) {
        this._hoverTextG = value;
    }
    get HoverG() {
        return this._hoverTextG;
    }
    /** Sets the hover color for the text RGB of B type. */
    set HoverB(value) {
        this._hoverTextB = value;
    }
    get HoverB() {
        return this._hoverTextB;
    }
    /** Set your font type. 0 - 7
    * 0 Normal
    * 1 Cursive
    * 2 All Caps
    * 3 Squares / Arrows / Etc.
    * 4 Condensed Normal
    * 5 Garbage
    * 6 Condensed Normal
    * 7 Bold GTA Style
    */
    set Font(value) {
        this._font = value;
    }
    get Font() {
        return this._font;
    }
    /** Sets the size of the text. 0.6 is pretty normal. 1 is quite large. */
    set FontScale(value) {
        this._fontScale = value;
    }
    get FontScale() {
        return this._fontScale;
    }
    /** Centers the content vertically. Do not use if your box is not very high to begin with */
    set VerticalCentered(value) {
        this._centeredVertically = value;
    }
    get VerticalCentered() {
        return this._centeredVertically;
    }
    /** Use this if you want centered content. */
    set Centered(value) {
        this._centered = value;
    }
    get Centered() {
        return this._centered;
    }
    /**
     *  Set Offset
     */
    set Offset(value) {
        this._offset = value;
    }
    drawAsCenteredAll() {
        if (this._hovered) {
            API.drawText(this._text, this._offset + this._xPos + (Math.round(this._width) / 2), this._yPos + (this._height / 2) - 20, this._fontScale, this._hoverTextR, this._hoverTextG, this._hoverTextB, this._hoverTextAlpha, this._font, 1, this._shadow, this._outline, Math.round(this._width));
            return;
        }
        API.drawText(this._text, this._offset + this._xPos + (Math.round(this._width) / 2), this._yPos + (this._height / 2) - 20, this._fontScale, this._fontR, this._fontG, this._fontB, this._fontAlpha, this._font, 1, this._shadow, this._outline, Math.round(this._width));
    }
    drawAsCenteredVertically() {
        if (this._hovered) {
            API.drawText(this._text, this._offset + this._xPos, this._yPos + (this._height / 2) - 20, this._fontScale, this._hoverTextR, this._hoverTextG, this._hoverTextB, this._hoverTextAlpha, this._font, 0, this._shadow, this._outline, Math.round(this._width));
            return;
        }
        API.drawText(this._text, this._offset + this._xPos, this._yPos + (this._height / 2) - 20, this._fontScale, this._fontR, this._fontG, this._fontB, this._fontAlpha, this._font, 0, this._shadow, this._outline, Math.round(this._width));
    }
    drawAsCentered() {
        if (this._hovered) {
            API.drawText(this._text, this._offset + this._xPos + (Math.round(this._width) / 2), this._padding + this._yPos, this._fontScale, this._hoverTextR, this._hoverTextG, this._hoverTextB, this._hoverTextAlpha, this._font, 1, this._shadow, this._outline, Math.round(this._width));
            return;
        }
        API.drawText(this._text, this._offset + this._xPos + (Math.round(this._width) / 2), this._padding + this._yPos, this._fontScale, this._fontR, this._fontG, this._fontB, this._fontAlpha, this._font, 1, this._shadow, this._outline, Math.round(this._width));
    }
    drawAsNormal() {
        if (this._hovered) {
            API.drawText(this._text, this._offset + this._xPos + this._padding, this._yPos + this._padding, this._fontScale, this._hoverTextR, this._hoverTextG, this._hoverTextB, this._hoverTextAlpha, this._font, 0, this._shadow, this._outline, Math.round(this._width) - this._padding);
            return;
        }
        API.drawText(this._text, this._offset + this._xPos + this._padding, this._yPos + this._padding, this._fontScale, this._fontR, this._fontG, this._fontB, this._fontAlpha, this._font, 0, this._shadow, this._outline, Math.round(this._width) - this._padding);
    }
}
class Panel {
    /**
     *
     * @param x - Max of 31. Starts on left side.
     * @param y - Max of 17. Starts at the top.
     * @param width - Max of 31. Each number fills a square.
     * @param height - Max of 17. Each number fills a square.
     */
    constructor(page, x, y, width, height) {
        this._page = page;
        this._padding = 10;
        this._xPos = x * panelMinX;
        this._yPos = y * panelMinY;
        this._width = width * panelMinX;
        this._height = height * panelMinY;
        this._alpha = 225;
        this._header = false;
        this._offset = 0;
        this._r = 0;
        this._g = 0;
        this._b = 0;
        this._textLines = [];
        this._inputPanels = [];
        this._progressBars = [];
        this._currentLine = 0;
        this._shadow = false;
        this._outline = false;
        this._tooltip = null;
        this._hovered = false;
        this._hoverTime = 0;
        this._hoverR = 0;
        this._hoverG = 0;
        this._hoverB = 0;
        this._hoverAlpha = 200;
        this._backgroundImage = null;
        this._backgroundImagePadding = 0;
        this._function = null;
        this._functionArgs = [];
        this._functionAudioLib = "Click";
        this._functionAudioName = "DLC_HEIST_HACKING_SNAKE_SOUNDS";
        this._hoverAudioLib = "Cycle_Item";
        this._hoverAudioName = "DLC_Dmod_Prop_Editor_Sounds";
        this._hoverAudio = true;
        this._functionClickAudio = true;
        this._hoverable = false;
        this._line = 0;
    }
    /**
     * Do not call this. It's specifically used for the menu builder file.
     */
    draw() {
        if (this._page !== currentPage) {
            return;
        }
        this.drawRectangles();
        if (!isReady) {
            return;
        }
        // Only used if using text lines.
        if (this._textLines.length > 0) {
            for (var i = 0; i < this._textLines.length; i++) {
                this._textLines[i].draw();
            }
        }
        // Only used if using input panels.
        if (this._inputPanels.length > 0) {
            for (var i = 0; i < this._inputPanels.length; i++) {
                this._inputPanels[i].draw();
            }
        }
        // Only used if using progress bars.
        if (this._progressBars.length > 0) {
            for (var i = 0; i < this._progressBars.length; i++) {
                this._progressBars[i].draw();
            }
        }
    }
    // Normal Versions
    drawRectangles() {
        if (this._backgroundImage !== null) {
            this.drawBackgroundImage();
            return;
        }
        if (this._hovered) {
            API.drawRectangle(this._xPos, this._yPos, Math.round(this._width), this._height, this._hoverR, this._hoverG, this._hoverB, this._hoverAlpha);
            if (this._header) {
                API.drawRectangle(this._xPos, this._yPos + this._height - 5, Math.round(this._width), 5, 255, 255, 255, 50);
            }
            return;
        }
        API.drawRectangle(this._xPos, this._yPos, Math.round(this._width), this._height, this._r, this._g, this._b, this._alpha);
        if (this._header) {
            API.drawRectangle(this._xPos, this._yPos + this._height - 5, Math.round(this._width), 5, 255, 255, 255, 50);
        }
    }
    drawBackgroundImage() {
        if (this._backgroundImagePadding > 1) {
            API.dxDrawTexture(this._backgroundImage, new Point(this._xPos + this._backgroundImagePadding, this._yPos + this._backgroundImagePadding), new Size(Math.round(this._width) - (this._backgroundImagePadding * 2), this._height - (this._backgroundImagePadding * 2)), 0);
            API.drawRectangle(this._xPos, this._yPos, Math.round(this._width), this._height, this._r, this._g, this._b, this._alpha);
            return;
        }
        API.dxDrawTexture(this._backgroundImage, new Point(this._xPos, this._yPos), new Size(Math.round(this._width), this._height), 0);
    }
    // Function Settings
    set Function(value) {
        this._function = value;
    }
    /** Add an array or a single value as a function. IMPORTANT! Any function you write must be able to take an array of arguments. */
    addFunctionArgs(value) {
        this._functionArgs = value;
    }
    // HOVER AUDIO
    /** Sets the hover audio library. Ex: "Cycle_Item" */
    set HoverAudioLib(value) {
        this._hoverAudioLib = value;
    }
    get HoverAudioLib() {
        return this._hoverAudioLib;
    }
    /** Sets the hover audio name. Ex: "DLC_Dmod_Prop_Editor_Sounds" */
    set HoverAudioName(value) {
        this._hoverAudioName = value;
    }
    get HoverAudioName() {
        return this._hoverAudioName;
    }
    // FUNCTION AUDIO
    /** Sets the function audio library. Ex: "Cycle_Item" */
    set FunctionAudioLib(value) {
        this._functionAudioLib = value;
    }
    get FunctionAudioLib() {
        return this._functionAudioLib;
    }
    /** Sets the function audio name. Ex: "DLC_Dmod_Prop_Editor_Sounds" */
    set FunctionAudioName(value) {
        this._functionAudioName = value;
    }
    get FunctionAudioName() {
        return this._functionAudioName;
    }
    /** Sets if the function audio plays. */
    set FunctionAudio(value) {
        this._functionClickAudio = value;
    }
    get FunctionAudio() {
        return this._functionClickAudio;
    }
    // Background Alpha
    /** Sets the background alpha property */
    set MainAlpha(value) {
        this._alpha = value;
    }
    get MainAlpha() {
        return this._alpha;
    }
    /** Sets the background image padding property */
    set MainBackgroundImagePadding(value) {
        this._backgroundImagePadding = value;
    }
    get MainBackgroundImagePadding() {
        return this._backgroundImagePadding;
    }
    /** Uses a custom image for your panel background. Must include extension. EX. 'clientside/image.jpg' */
    set MainBackgroundImage(value) {
        this._backgroundImage = value;
    }
    get MainBackgroundImage() {
        return this._backgroundImage;
    }
    /** Sets the color for RGB of R type. Max of 255 */
    set MainColorR(value) {
        this._r = value;
    }
    /** Gets the color for RGB of R type. */
    get MainColorR() {
        return this._r;
    }
    /** Sets the color for RGB of G type. Max of 255 */
    set MainColorG(value) {
        this._g = value;
    }
    /** Gets the color for RGB of G type. */
    get MainColorG() {
        return this._g;
    }
    /** Sets the color for RGB of B type. Max of 255 */
    set MainColorB(value) {
        this._b = value;
    }
    /** Gets the color for RGB of B type. */
    get MainColorB() {
        return this._b;
    }
    /** Sets RGB Color of Main */
    MainBackgroundColor(r, g, b, alpha) {
        this._r = r;
        this._g = g;
        this._b = b;
        this._alpha = alpha;
    }
    /** Sets the RGB Color of Hover */
    HoverBackgroundColor(r, g, b, alpha) {
        this._hoverR = r;
        this._hoverG = g;
        this._hoverB = b;
        this._hoverAlpha = alpha;
    }
    /** Is there a hover state? */
    set Hoverable(value) {
        this._hoverable = value;
    }
    get Hoverable() {
        return this._hoverable;
    }
    /** Sets the hover alpha */
    set HoverAlpha(value) {
        this._hoverAlpha = value;
    }
    get HoverAlpha() {
        return this._hoverAlpha;
    }
    /** Sets the hover color for RGB of R type. */
    set HoverR(value) {
        this._hoverR = value;
    }
    get HoverR() {
        return this._hoverR;
    }
    /** Sets the hover color for RGB of G type. */
    set HoverG(value) {
        this._hoverG = value;
    }
    get HoverG() {
        return this._hoverG;
    }
    /** Sets the hover color for RGB of B type. */
    set HoverB(value) {
        this._hoverB = value;
    }
    get HoverB() {
        return this._hoverB;
    }
    /** Sets the font Outline property */
    set FontOutline(value) {
        this._outline = value;
    }
    get FontOutline() {
        return this._outline;
    }
    /** Sets the font Shadow property */
    set FontShadow(value) {
        this._shadow = value;
    }
    get FontShadow() {
        return this._shadow;
    }
    /** Sets the Tooltip text for your element. */
    set Tooltip(value) {
        this._tooltip = value;
    }
    get Tooltip() {
        return this._tooltip;
    }
    /** Adds a stylized line under your your box. */
    set Header(value) {
        this._header = value;
    }
    get Header() {
        return this._header;
    }
    /** If your text needs to be pushed in a certain direction either add or remove pixels here. */
    set Offset(value) {
        this._offset = value;
    }
    get Offset() {
        return this._offset;
    }
    /**
     *  Used to add text elements to your panels.
     * @param value
     */
    addText(value) {
        let textElement = new TextElement(value, this._xPos, this._yPos, Math.round(this._width), this._height, this._line);
        this._textLines.push(textElement);
        this._line += 1;
        return textElement;
    }
    /**
     *
     * @param x - Start position of X inside the panel.
     * @param y - Start Position of Y inside the panel.
     * @param width - How wide. Generally the width of your panel.
     * @param height - How tall. 1 or 2 is pretty normal.
     */
    addInput(x, y, width, height) {
        let inputPanel = new InputPanel(this._page, (x * panelMinX) + this._xPos, (y * panelMinY) + this._yPos, width, height);
        this._inputPanels.push(inputPanel);
        return inputPanel;
    }
    /**
     *
     * @param x - Start position of X inside the panel.
     * @param y - Start Position of Y inside the panel.
     * @param width
     * @param height
     * @param currentProgress - 0 to 100
     */
    addProgressBar(x, y, width, height, currentProgress) {
        let progressBar = new ProgressBar((x * panelMinX) + this._xPos, (y * panelMinY) + this._yPos, width, height, currentProgress);
        this._progressBars.push(progressBar);
        return progressBar;
    }
    // Hover Action
    isHovered() {
        if (!API.isCursorShown()) {
            return;
        }
        if (!this._hoverable) {
            return;
        }
        let cursorPos = API.getCursorPositionMantainRatio();
        if (cursorPos.X > this._xPos && cursorPos.X < (this._xPos + Math.round(this._width)) && cursorPos.Y > this._yPos && cursorPos.Y < this._yPos + this._height) {
            if (!this._hovered) {
                this._hovered = true;
                this.setTextHoverState(true);
                if (this._hoverAudio) {
                    API.playSoundFrontEnd(this._hoverAudioLib, this._hoverAudioName);
                }
            }
            this._hoverTime += 1;
            if (this._hoverTime > 50) {
                if (this._tooltip === null) {
                    return;
                }
                if (this._tooltip.length > 1) {
                    API.drawText(this._tooltip, cursorPos.X + 25, cursorPos.Y, 0.4, 255, 255, 255, 255, 4, 0, true, true, 200);
                }
            }
            return;
        }
        this._hovered = false;
        this._hoverTime = 0;
        this.setTextHoverState(false);
    }
    // Click Action
    isClicked() {
        // Is there even a cursor?
        if (!API.isCursorShown()) {
            return;
        }
        // Is there a function if they click it?
        if (this._function === null) {
            return;
        }
        // Are they even left clicking?
        if (API.isControlJustPressed(237 /* CursorAccept */)) {
            let cursorPos = API.getCursorPositionMantainRatio();
            if (cursorPos.X > this._xPos && cursorPos.X < (this._xPos + Math.round(this._width)) && cursorPos.Y > this._yPos && cursorPos.Y < this._yPos + this._height) {
                if (new Date().getTime() < clickDelay + 200) {
                    return;
                }
                clickDelay = new Date().getTime();
                if (this._functionClickAudio) {
                    API.playSoundFrontEnd(this._functionAudioLib, this._functionAudioName);
                }
                if (this._functionArgs !== null || this._functionArgs.length > 1) {
                    this._function(this._functionArgs);
                }
                else {
                    this._function();
                }
                return;
            }
        }
    }
    setTextHoverState(value) {
        for (var i = 0; i < this._textLines.length; i++) {
            this._textLines[i].Hovered = value;
        }
    }
    // Type
    returnType() {
        return "Panel";
    }
}
class InputPanel {
    constructor(page, x, y, width, height) {
        this._xPos = x;
        this._yPos = y;
        this._width = width * panelMinX;
        this._height = height * panelMinY;
        this._protected = false;
        this._input = "";
        this._hovered = false;
        this._selected = false;
        this._numeric = false;
        this._isError = false;
        this._isTransparent = false;
        this._r = 255;
        this._g = 255;
        this._b = 255;
        this._alpha = 100;
        this._hoverR = 255;
        this._hoverG = 255;
        this._hoverB = 255;
        this._hoverAlpha = 125;
        this._selectR = 255;
        this._selectG = 255;
        this._selectB = 255;
        this._inputTextR = 0;
        this._inputTextG = 0;
        this._inputTextB = 0;
        this._inputTextAlpha = 255;
        this._selectAlpha = 125;
        this._inputAudioLib = "Click";
        this._inputAudioName = "DLC_HEIST_HACKING_SNAKE_SOUNDS";
        this._page = page;
        this._inputTextScale = 0.45;
        tabIndex.push(this);
    }
    /** Sets whether or not there is an error. */
    set isError(value) {
        this._isError = value;
    }
    /** Sets whether or not this input is selected. */
    set selected(value) {
        this._selected = value;
        if (value) {
            selectedInput = this;
        }
        else {
            selectedInput = null;
        }
    }
    get selected() {
        return this._selected;
    }
    // Hover BACKGROUND PARAMETERS
    /** Set R of RGB on hover background. */
    set HoverR(value) {
        this._hoverR = value;
    }
    get HoverR() {
        return this._hoverR;
    }
    /** Set G of RGB on hover background. */
    set HoverG(value) {
        this._hoverG = value;
    }
    get HoverG() {
        return this._hoverG;
    }
    /** Set B of RGB on hover background. */
    set HoverB(value) {
        this._hoverB = value;
    }
    get HoverB() {
        return this._hoverB;
    }
    /** Set Alpha of RGB on hover background. */
    set HoverAlpha(value) {
        this._hoverAlpha = value;
    }
    get HoverAlpha() {
        return this._hoverAlpha;
    }
    // Main BACKGROUND PARAMETERS
    MainBackgroundColor(r, g, b, alpha) {
        this._r = r;
        this._g = g;
        this._b = b;
        this._alpha = alpha;
    }
    HoverBackgroundColor(r, g, b, alpha) {
        this._hoverR = r;
        this._hoverG = g;
        this._hoverB = b;
        this._hoverAlpha = alpha;
    }
    SelectBackgroundColor(r, g, b, alpha) {
        this._selectR = r;
        this._selectG = g;
        this._selectB = b;
        this._selectAlpha = alpha;
    }
    /** Set R of RGB on main background. */
    set R(value) {
        this._r = value;
    }
    get R() {
        return this._r;
    }
    /** Set G of RGB on main background. */
    set G(value) {
        this._g = value;
    }
    get G() {
        return this._g;
    }
    /** Set B of RGB on main background. */
    set B(value) {
        this._b = value;
    }
    get B() {
        return this._b;
    }
    /** Set Alpha of RGB on main background. */
    set Alpha(value) {
        this._alpha = value;
    }
    get Alpha() {
        return this._alpha;
    }
    // SELECTION BACKGROUND PARAMETERS
    /** Set R of RGB on main background. */
    set SelectR(value) {
        this._selectR = value;
    }
    get SelectR() {
        return this._selectR;
    }
    /** Set G of RGB on main background. */
    set SelectG(value) {
        this._selectG = value;
    }
    get SelectG() {
        return this._selectG;
    }
    /** Set B of RGB on main background. */
    set SelectB(value) {
        this._selectB = value;
    }
    get SelectB() {
        return this._selectB;
    }
    /** Set Alpha of RGB on main background. */
    set SelectAlpha(value) {
        this._selectAlpha = value;
    }
    get SelectAlpha() {
        return this._selectAlpha;
    }
    /**
     *  Sets the RGB Parameter for the INPUT Text;
     * @param r
     * @param g
     * @param b
     * @param alpha
     */
    InputTextColor(r, g, b, alpha) {
        this._inputTextR = r;
        this._inputTextG = g;
        this._inputTextB = b;
        this._inputTextAlpha = alpha;
    }
    /** Set R of RGB on input text. */
    set TextR(value) {
        this._inputTextR = value;
    }
    get TextR() {
        return this._inputTextR;
    }
    /** Set G of RGB on input text. */
    set TextG(value) {
        this._inputTextG = value;
    }
    get TextG() {
        return this._inputTextG;
    }
    /** Set B of RGB on input text. */
    set TextB(value) {
        this._inputTextB = value;
    }
    get InputB() {
        return this._inputTextB;
    }
    /** Set Alpha of RGB on input text. */
    set InputAlpha(value) {
        this._inputTextAlpha = value;
    }
    get InputAlpha() {
        return this._inputTextAlpha;
    }
    /** Input Text Size */
    set InputTextScale(value) {
        this._inputTextScale = value;
    }
    get InputTextScale() {
        return this._inputTextScale;
    }
    /** Sets the input text. */
    set Input(value) {
        this._input = value;
    }
    /** Returns whatever the current input is. */
    get Input() {
        return this._input;
    }
    /** Removes the last character from the input. */
    removeFromInput() {
        this._input = this._input.substring(0, this._input.length - 1);
    }
    /** Set whether the input should be numeric only. */
    set NumericOnly(value) {
        this._numeric = value;
    }
    get NumericOnly() {
        return this._numeric;
    }
    /**
     *  Sets whether the input should be protected or not. */
    set Protected(value) {
        this._protected = value;
    }
    // Draw what we need to draw.
    draw() {
        if (this._selected) {
            this.selectedDraw();
        }
        if (this._hovered) {
            this.hoveredDraw();
        }
        if (!this._hovered && !this._selected) {
            this.normalDraw();
        }
        this.isHovered();
        this.isClicked();
    }
    //DEBUG
    normalDraw() {
        if (this._isError) {
            API.drawRectangle(this._xPos + 10, this._yPos + 10, Math.round(this._width) - 20, this._height - 20, 255, 0, 0, 100);
        }
        else {
            API.drawRectangle(this._xPos + 10, this._yPos + 10, Math.round(this._width) - 20, this._height - 20, this._r, this._g, this._b, this._alpha);
        }
        if (this._protected) {
            if (this._input.length < 1) {
                return;
            }
            API.drawText("*".repeat(this._input.length), this._xPos + (Math.round(this._width) / 2), this._yPos + (this._height / 2) - 14, this._inputTextScale, this._inputTextR, this._inputTextG, this._inputTextB, this._inputTextAlpha, 4, 1, false, false, (panelMinX * Math.round(this._width)));
        }
        else {
            API.drawText(this._input, this._xPos + (Math.round(this._width) / 2), this._yPos + (this._height / 2) - 14, this._inputTextScale, this._inputTextR, this._inputTextG, this._inputTextB, this._inputTextAlpha, 4, 1, false, false, (panelMinX * Math.round(this._width)));
        }
    }
    selectedDraw() {
        API.drawRectangle(this._xPos + 10, this._yPos + 10, Math.round(this._width) - 20, this._height - 20, this._selectR, this._selectG, this._selectB, this._selectAlpha);
        if (this._protected) {
            if (this._input.length < 1) {
                return;
            }
            API.drawText("*".repeat(this._input.length), this._xPos + (Math.round(this._width) / 2), this._yPos + (this._height / 2) - 14, this._inputTextScale, this._inputTextR, this._inputTextG, this._inputTextB, this._inputTextAlpha, 4, 1, false, false, (panelMinX * Math.round(this._width)));
        }
        else {
            API.drawText(this._input, this._xPos + (Math.round(this._width) / 2), this._yPos + (this._height / 2) - 14, this._inputTextScale, this._inputTextR, this._inputTextG, this._inputTextB, this._inputTextAlpha, 4, 1, false, false, (panelMinX * Math.round(this._width)));
        }
        return;
    }
    hoveredDraw() {
        if (this._isError) {
            API.drawRectangle(this._xPos + 10, this._yPos + 10, Math.round(this._width) - 20, this._height - 20, 255, 0, 0, 100);
        }
        else {
            API.drawRectangle(this._xPos + 10, this._yPos + 10, Math.round(this._width) - 20, this._height - 20, this._hoverR, this._hoverG, this._hoverB, this._hoverAlpha);
        }
        if (this._protected) {
            if (this._input.length < 1) {
                return;
            }
            API.drawText("*".repeat(this._input.length), this._xPos + (Math.round(this._width) / 2), this._yPos + (this._height / 2) - 14, this._inputTextScale, this._inputTextR, this._inputTextG, this._inputTextB, this._inputTextAlpha, 4, 1, false, false, (panelMinX * Math.round(this._width)));
        }
        else {
            API.drawText(this._input, this._xPos + (Math.round(this._width) / 2), this._yPos + (this._height / 2) - 14, this._inputTextScale, this._inputTextR, this._inputTextG, this._inputTextB, this._inputTextAlpha, 4, 1, false, false, (panelMinX * Math.round(this._width)));
        }
    }
    isHovered() {
        if (!API.isCursorShown()) {
            return;
        }
        let cursorPos = API.getCursorPositionMantainRatio();
        if (cursorPos.X > this._xPos && cursorPos.X < (this._xPos + Math.round(this._width)) && cursorPos.Y > this._yPos && cursorPos.Y < (this._yPos + this._height)) {
            if (this._selected) {
                this._hovered = false;
                return;
            }
            this._hovered = true;
        }
        else {
            this._hovered = false;
        }
    }
    isClicked() {
        if (API.isControlJustPressed(237 /* CursorAccept */)) {
            if (new Date().getTime() < clickDelay + 200) {
                return;
            }
            let cursorPos = API.getCursorPositionMantainRatio();
            if (cursorPos.X > this._xPos && cursorPos.X < (this._xPos + Math.round(this._width)) && cursorPos.Y > this._yPos && cursorPos.Y < (this._yPos + this._height)) {
                if (!this._selected) {
                    API.playSoundFrontEnd(this._inputAudioLib, this._inputAudioName);
                    this._selected = true;
                }
                selectedInput = this;
            }
            else {
                this._selected = false;
            }
        }
    }
    addToInput(text) {
        if (this._input.length > 2147483647) {
            return;
        }
        if (!this._numeric) {
            this._input += text;
            return this._input;
        }
        else {
            if (Number.isInteger(+text)) {
                this._input += text;
                return this._input;
            }
        }
    }
    returnType() {
        return "InputPanel";
    }
}
function drawTextNotification() {
    if (textnotification !== null) {
        textnotification.draw();
        return;
    }
    if (textnotifications.length <= 0) {
        return;
    }
    textnotification = textnotifications.shift();
    return;
}
function drawNotification() {
    if (notification !== null) {
        notification.draw();
        return;
    }
    if (notifications.length <= 0) {
        return;
    }
    notification = notifications.shift();
    return;
}
function createNotification(page, text, displayTime) {
    // Add to queue.
    let notify = new Notification(text, displayTime);
    notifications.push(notify);
    return notify;
}
function createPlayerTextNotification(text) {
    let notify = new PlayerTextNotification(text);
    textnotifications.push(notify);
    return notify;
}
/**
 * Set the page number for whatever current menu is open.
 * @param value
 */
function setPage(value) {
    currentPage = value;
}
function killMenu() {
    isReady = false;
    currentPage = -1;
    selectedInput = null;
    tabIndex = [];
    API.showCursor(false);
    API.setHudVisible(true);
    API.setChatVisible(true);
    API.setCanOpenChat(true);
    //API.callNative("_TRANSITION_FROM_BLURRED", 3000);
    menuElements = [];
}
// On-Keydown Event
API.onKeyDown.connect((sender, e) => {
    if (!isReady) {
        return;
    }
    // Shift between Input Boxes.
    if (e.KeyCode == Keys.Tab) {
        if (tabIndex[0].Selected) {
            tabIndex[0].Selected = false;
        }
        else {
            tabIndex[0].Selected = true;
            return;
        }
        let removeItem = tabIndex.shift();
        tabIndex.push(removeItem);
        tabIndex[0].Selected = true;
    }
    if (selectedInput === null) {
        return;
    }
    if (e.KeyCode === Keys.Back) {
        selectedInput.removeFromInput();
        return;
    }
    let shiftOn = false;
    if (e.Shift) {
        shiftOn = true;
    }
    let keypress = "";
    switch (e.KeyCode) {
        case Keys.Space:
            keypress = " ";
            break;
        case Keys.A:
            keypress = "a";
            if (shiftOn) {
                keypress = "A";
            }
            break;
        case Keys.B:
            keypress = "b";
            if (shiftOn) {
                keypress = "B";
            }
            break;
        case Keys.C:
            keypress = "c";
            if (shiftOn) {
                keypress = "C";
            }
            break;
        case Keys.D:
            keypress = "d";
            if (shiftOn) {
                keypress = "D";
            }
            break;
        case Keys.E:
            keypress = "e";
            if (shiftOn) {
                keypress = "E";
            }
            break;
        case Keys.F:
            keypress = "f";
            if (shiftOn) {
                keypress = "F";
            }
            break;
        case Keys.G:
            keypress = "g";
            if (shiftOn) {
                keypress = "G";
            }
            break;
        case Keys.H:
            keypress = "h";
            if (shiftOn) {
                keypress = "H";
            }
            break;
        case Keys.I:
            keypress = "i";
            if (shiftOn) {
                keypress = "I";
            }
            break;
        case Keys.J:
            keypress = "j";
            if (shiftOn) {
                keypress = "J";
            }
            break;
        case Keys.K:
            keypress = "k";
            if (shiftOn) {
                keypress = "K";
            }
            break;
        case Keys.L:
            keypress = "l";
            if (shiftOn) {
                keypress = "L";
            }
            break;
        case Keys.M:
            keypress = "m";
            if (shiftOn) {
                keypress = "M";
            }
            break;
        case Keys.N:
            keypress = "n";
            if (shiftOn) {
                keypress = "N";
            }
            break;
        case Keys.O:
            keypress = "o";
            if (shiftOn) {
                keypress = "O";
            }
            break;
        case Keys.P:
            keypress = "p";
            if (shiftOn) {
                keypress = "P";
            }
            break;
        case Keys.Q:
            keypress = "q";
            if (shiftOn) {
                keypress = "Q";
            }
            break;
        case Keys.R:
            keypress = "r";
            if (shiftOn) {
                keypress = "R";
            }
            break;
        case Keys.S:
            keypress = "s";
            if (shiftOn) {
                keypress = "S";
            }
            break;
        case Keys.T:
            keypress = "t";
            if (shiftOn) {
                keypress = "T";
            }
            break;
        case Keys.U:
            keypress = "u";
            if (shiftOn) {
                keypress = "U";
            }
            break;
        case Keys.V:
            keypress = "v";
            if (shiftOn) {
                keypress = "V";
            }
            break;
        case Keys.W:
            keypress = "w";
            if (shiftOn) {
                keypress = "W";
            }
            break;
        case Keys.X:
            keypress = "x";
            if (shiftOn) {
                keypress = "X";
            }
            break;
        case Keys.Y:
            keypress = "y";
            if (shiftOn) {
                keypress = "Y";
            }
            break;
        case Keys.Z:
            keypress = "z";
            if (shiftOn) {
                keypress = "Z";
            }
            break;
        case Keys.D0:
            keypress = "0";
            if (shiftOn) {
                keypress = ")";
            }
            break;
        case Keys.D1:
            keypress = "1";
            if (shiftOn) {
                keypress = "!";
            }
            break;
        case Keys.D2:
            keypress = "2";
            if (shiftOn) {
                keypress = "@";
            }
            break;
        case Keys.D3:
            keypress = "3";
            if (shiftOn) {
                keypress = "#";
            }
            break;
        case Keys.D4:
            keypress = "4";
            if (shiftOn) {
                keypress = "$";
            }
            break;
        case Keys.D5:
            keypress = "5";
            if (shiftOn) {
                keypress = "%";
            }
            break;
        case Keys.D6:
            keypress = "6";
            if (shiftOn) {
                keypress = "^";
            }
            break;
        case Keys.D7:
            keypress = "7";
            if (shiftOn) {
                keypress = "&";
            }
            break;
        case Keys.D8:
            keypress = "8";
            if (shiftOn) {
                keypress = "*";
            }
            break;
        case Keys.D9:
            keypress = "9";
            if (shiftOn) {
                keypress = "(";
            }
            break;
        case Keys.OemMinus:
            keypress = "-";
            if (shiftOn) {
                keypress = "_";
            }
            break;
        case Keys.OemQuestion:
            keypress = "/";
            if (shiftOn) {
                keypress = "?";
            }
            break;
        case Keys.OemPeriod:
            keypress = ".";
            if (shiftOn) {
                keypress = ">";
            }
            break;
        case Keys.OemSemicolon:
            keypress = ";";
            if (shiftOn) {
                keypress = ":";
            }
            break;
        case Keys.OemOpenBrackets:
            keypress = "[";
            if (shiftOn) {
                keypress = "{";
            }
            break;
        case Keys.OemCloseBrackets:
            keypress = "]";
            if (shiftOn) {
                keypress = "}";
            }
            break;
        case Keys.NumPad0:
            keypress = "0";
            break;
        case Keys.NumPad1:
            keypress = "1";
            break;
        case Keys.NumPad2:
            keypress = "2";
            break;
        case Keys.NumPad3:
            keypress = "3";
            break;
        case Keys.NumPad4:
            keypress = "4";
            break;
        case Keys.NumPad5:
            keypress = "5";
            break;
        case Keys.NumPad6:
            keypress = "6";
            break;
        case Keys.NumPad7:
            keypress = "7";
            break;
        case Keys.NumPad8:
            keypress = "8";
            break;
        case Keys.NumPad9:
            keypress = "9";
            break;
    }
    if (keypress === "") {
        return;
    }
    if (keypress.length > 0) {
        if (selectedInput === null) {
            return;
        }
        selectedInput.addToInput(keypress);
    }
    else {
        return;
    }
});
/// <reference path="../../types-gtanetwork/index.d.ts" />
class PlayerInfo {
    constructor() {
        this.socialClubName = "";
        this.name = "";
        this.ping = 0;
        this.color = [255, 255, 255];
    }
}
var gCurrentState = 0;
var gCurrentPage = 0;
var gStateSet = 0;
var gPlayers = [];
function getPlayer(socialClubName) {
    for (var i = 0; i < gPlayers.length; i++) {
        if (gPlayers[i].socialClubName == socialClubName) {
            return gPlayers[i];
        }
    }
    return null;
}
API.onServerEventTrigger.connect((name, args) => {
    var newPlayer;
    if (name == "playerlist") {
        gPlayers = [];
        var list = args[0];
        for (var i = 0; i < list.Count; i++) {
            var obj = JSON.parse(list[i]);
            newPlayer = new PlayerInfo();
            newPlayer.socialClubName = obj.socialClubName;
            newPlayer.name = obj.name;
            newPlayer.color = [255, 255, 255];
            gPlayers.push(newPlayer);
        }
    }
    else if (name == "playerlist_join") {
        // This can happen in certain situations, so we handle this as an update
        var existingPlayer = getPlayer(args[0]);
        if (existingPlayer != null) {
            existingPlayer.socialClubName = args[0];
            existingPlayer.name = args[1];
            existingPlayer.color = [255, 255, 255];
        }
        else {
            newPlayer = new PlayerInfo();
            newPlayer.socialClubName = args[0];
            newPlayer.name = args[1];
            newPlayer.color = [255, 255, 255];
            gPlayers.push(newPlayer);
        }
    }
    else if (name == "playerlist_leave") {
        for (var i = 0; i < gPlayers.length; i++) {
            if (gPlayers[i].socialClubName == args[0]) {
                gPlayers.splice(i, 1);
            }
        }
    }
    else if (name == "playerlist_pings") {
        var list = args[0];
        for (var i = 0; i < list.Count; i++) {
            var obj = JSON.parse(list[i]);
            var player = getPlayer(obj.socialClubName);
            if (player != null) {
                player.ping = obj.ping;
            }
        }
    }
    else if (name == "playerlist_changednames") {
        var list = args[0];
        for (var i = 0; i < list.Count; i++) {
            var obj = JSON.parse(list[i]);
            var player = getPlayer(obj.socialClubName);
            if (player != null) {
                player.name = obj.newName;
            }
        }
    }
});
API.onUpdate.connect(() => {
    // MultiplayerInfo
    if (!API.isChatOpen() && API.isControlJustPressed(20 /* MultiplayerInfo */)) {
        gCurrentState++;
        gStateSet = API.getGameTime();
        API.triggerServerEvent("playerlist_pings");
        if (gCurrentState == 1) {
            gCurrentPage = 0;
            return;
        }
        if (gCurrentState == 2) {
            API.callNative("_SET_RADAR_BIGMAP_ENABLED", true, false);
        }
        if (gCurrentState == 3) {
            gCurrentState = 0;
            API.callNative("_SET_RADAR_BIGMAP_ENABLED", false, false);
        }
    }
    if (gCurrentState > 0 && API.getGameTime() - gStateSet > 3000) {
        gCurrentState = 0;
        API.callNative("_SET_RADAR_BIGMAP_ENABLED", false, false);
    }
    if (gCurrentState == 1) {
        // Get list data
        var resolution = API.getScreenResolutionMantainRatio();
        var listPadding = 4;
        var listLine = 28;
        var listBorder = 2;
        var listItemsPerPage = Math.floor((resolution.Height * 0.8) / listLine);
        var listPages = Math.ceil(gPlayers.length / listItemsPerPage);
        // Page navigation is here for now because the onKeyDown method doesn't work
        if (listPages > 1) {
            if (API.isControlJustPressed(172 /* PhoneUp */)) {
                gStateSet = API.getGameTime();
                if (--gCurrentPage < 0) {
                    gCurrentPage = listPages - 1;
                }
            }
            else if (API.isControlJustPressed(173 /* PhoneDown */)) {
                gStateSet = API.getGameTime();
                if (++gCurrentPage >= listPages) {
                    gCurrentPage = 0;
                }
            }
        }
        var listPageStart = gCurrentPage * listItemsPerPage;
        var listPageCount = Math.min(gPlayers.length - listPageStart, listItemsPerPage);
        var listWidth = resolution.Width * 0.4;
        var listHeight = (Math.min(gPlayers.length, listPageCount) + 1) * 28 + listPadding * 2;
        var listX = resolution.Width / 2 - listWidth / 2;
        var listY = Math.max(30, resolution.Height * 0.3 - listHeight / 2);
        // Fill
        API.drawRectangle(listX, listY, listWidth, listHeight, 0, 0, 0, 220);
        // Separator
        API.drawRectangle(listX, listY + listLine, listWidth, listBorder, 100, 100, 100, 220);
        // Left
        API.drawRectangle(listX - listBorder, listY - listBorder, listBorder, listHeight + listBorder * 2, 91, 131, 40, 220);
        // Right
        API.drawRectangle(listX + listWidth, listY - listBorder, listBorder, listHeight + listBorder * 2, 91, 131, 40, 220);
        // Top
        API.drawRectangle(listX, listY - listBorder, listWidth, listBorder, 91, 131, 40, 220);
        // Bottom
        API.drawRectangle(listX, listY + listHeight, listWidth, listBorder, 91, 131, 40, 220);
        // Page indicators
        if (gCurrentPage > 0) {
            API.drawText("3", listX + listWidth / 2, listY + listLine, 0.5, 255, 255, 255, 255, 3, 1, true, true, 0);
        }
        if (gCurrentPage < listPages - 1) {
            API.drawText("4", listX + listWidth / 2, listY + listHeight - listLine * 2, 0.5, 255, 255, 255, 255, 3, 1, true, true, 0);
        }
        // Header
        API.drawText("ID Nazwa", listX + listPadding, listY - listPadding / 2, 0.4, 91, 131, 40, 255, 0, 0, true, true, 0);
        API.drawText("Ping", listX + listWidth - listPadding, listY - listPadding / 2, 0.4, 91, 131, 40, 255, 0, 2, true, true, 0);
        // Players
        for (var i = 0; i < listPageCount; i++) {
            var player = gPlayers[listPageStart + i];
            var color = player.color;
            API.drawText(player.name, listX + listPadding, listY + listPadding / 2 + listLine * (i + 1), 0.45, color[0], color[1], color[2], 255, 4, 0, true, true, 0);
            API.drawText("" + player.ping, listX + listWidth - listPadding, listY + listPadding / 2 + listLine * (i + 1), 0.45, 100, 100, 100, 255, 4, 2, true, true, 0);
        }
    }
});
var freeCam = null;
var normalspeed = 0.5;
var highspeed = 1.5;
API.onServerEventTrigger.connect((eventName, args) => {
    if (eventName == "FreeCamStart") {
        freeCam = API.createCamera(args[0], new Vector3(0, 0, 0));
        API.setActiveCamera(freeCam);
    }
    if (eventName == "FreeCamStop") {
        var cameraPos = API.getCameraPosition(freeCam);
        API.triggerServerEvent("ChangePosition", new Vector3(cameraPos.X, cameraPos.Y, cameraPos.Z + 1));
        API.setActiveCamera(null);
        freeCam = null;
    }
});
API.onUpdate.connect(() => {
    if (freeCam != null) {
        API.setCameraRotation(freeCam, API.getGameplayCamRot());
        var cameraRot = API.getCameraRotation(freeCam);
        var cameraPos = API.getCameraPosition(freeCam);
        API.setEntityPosition(API.getLocalPlayer(), new Vector3(cameraPos.X, cameraPos.Y, cameraPos.Z - 2.5));
        API.setEntityRotation(API.getLocalPlayer(), new Vector3(0, 0, cameraRot.Z));
        var pi = Math.PI;
        var xradian = ((cameraRot.X * pi) / 180);
        var yradian = ((cameraRot.Z * pi) / 180);
        var zradian = ((cameraRot.Z * pi) / 180);
        if (API.isControlPressed(87)) {
            API.enableControlThisFrame(87);
            var OldPos = API.getCameraPosition(freeCam);
            var newx = -(Math.sin(yradian) * normalspeed);
            var newy = Math.cos(yradian) * normalspeed;
            var newz = Math.sin(xradian) * normalspeed; // up or down					
            API.setCameraPosition(freeCam, new Vector3(OldPos.X + newx, OldPos.Y + newy, OldPos.Z + newz));
        }
        if (!API.isChatOpen()) {
            if (API.isControlPressed(21)) {
                if (API.isControlPressed(87)) {
                    API.enableControlThisFrame(21);
                    var OldPos = API.getCameraPosition(freeCam);
                    var newx = -(Math.sin(yradian) * highspeed);
                    var newy = Math.cos(yradian) * highspeed;
                    var newz = Math.sin(xradian) * highspeed;
                    API.setCameraPosition(freeCam, new Vector3(OldPos.X + newx, OldPos.Y + newy, OldPos.Z + newz));
                }
                if (API.isControlPressed(268)) {
                    API.enableControlThisFrame(87);
                    var OldPos = API.getCameraPosition(freeCam);
                    var newx = Math.sin(yradian) * highspeed;
                    var newy = -(Math.cos(yradian) * highspeed);
                    var newz = -(Math.sin(xradian) * highspeed); // up or down					
                    API.setCameraPosition(freeCam, new Vector3(OldPos.X + newx, OldPos.Y + newy, OldPos.Z + newz));
                }
                if (API.isControlPressed(35)) {
                    API.enableControlThisFrame(35);
                    var OldPos = API.getCameraPosition(freeCam);
                    var newx = Math.cos(yradian) * highspeed;
                    var newy = Math.sin(yradian) * highspeed;
                    var newz = -(Math.sin(xradian) * highspeed); // up or down					
                    API.setCameraPosition(freeCam, new Vector3(OldPos.X + newx, OldPos.Y + newy, OldPos.Z + newz));
                }
                if (API.isControlPressed(34)) {
                    API.enableControlThisFrame(34);
                    var OldPos = API.getCameraPosition(freeCam);
                    var newx = -(Math.cos(yradian) * highspeed);
                    var newy = -(Math.sin(yradian) * highspeed);
                    var newz = Math.sin(xradian) * highspeed; // up or down					
                    API.setCameraPosition(freeCam, new Vector3(OldPos.X + newx, OldPos.Y + newy, OldPos.Z + newz));
                }
            }
        }
        if (API.isControlPressed(268)) {
            API.enableControlThisFrame(87);
            var OldPos = API.getCameraPosition(freeCam);
            var newx = Math.sin(yradian) * normalspeed;
            var newy = -(Math.cos(yradian) * normalspeed);
            var newz = -(Math.sin(xradian) * normalspeed); // up or down					
            API.setCameraPosition(freeCam, new Vector3(OldPos.X + newx, OldPos.Y + newy, OldPos.Z + newz));
        }
        if (API.isControlPressed(35)) {
            API.enableControlThisFrame(35);
            var OldPos = API.getCameraPosition(freeCam);
            var newx = Math.cos(yradian) * normalspeed;
            var newy = Math.sin(yradian) * normalspeed;
            var newz = -(Math.sin(xradian) * normalspeed); // up or down					
            API.setCameraPosition(freeCam, new Vector3(OldPos.X + newx, OldPos.Y + newy, OldPos.Z + newz));
        }
        if (API.isControlPressed(34)) {
            API.enableControlThisFrame(34);
            var OldPos = API.getCameraPosition(freeCam);
            var newx = -(Math.cos(yradian) * normalspeed);
            var newy = -(Math.sin(yradian) * normalspeed);
            var newz = Math.sin(xradian) * normalspeed; // up or down					
            API.setCameraPosition(freeCam, new Vector3(OldPos.X + newx, OldPos.Y + newy, OldPos.Z + newz));
        }
    }
});
//# sourceMappingURL=Compiled.js.map