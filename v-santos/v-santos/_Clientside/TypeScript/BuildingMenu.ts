/// <reference path="../../types-gtanetwork/index.d.ts" />

let drawScaleform: boolean = false;
let scaleForm: any = null;
let buildingMenu: Menu = null;

API.onKeyDown.connect((sender: any, e: System.Windows.Forms.KeyEventArgs) => {
    if (e.KeyCode === Keys.E && API.hasEntitySyncedData(API.getLocalPlayer(), "DoorsTarget")) {
        API.triggerServerEvent("PassDoors");
    }
    else if (e.KeyCode === Keys.K && API.hasEntitySyncedData(API.getLocalPlayer(), "DoorsTarget")) {
        API.triggerServerEvent("KnockDoors");
    }
});

API.onServerEventTrigger.connect((eventName: string, args: System.Array<any>) => {
    if (eventName == "DrawBuildingComponents") {

        scaleForm = API.requestScaleform('instructional_buttons');
        scaleForm.CallFunction('SET_DATA_SLOT_EMPTY');
        if (args[0])
        {
            scaleForm.CallFunction('SET_DATA_SLOT', 0, 'E', 'Wejdź');
            scaleForm.CallFunction('SET_DATA_SLOT', 1, 'K', 'Zapukaj');
            showBuildingPanel(args[1]);
        }
        else
        {
            scaleForm.CallFunction('SET_DATA_SLOT', 0, 'E', 'Wyjdź');
            scaleForm.CallFunction('SET_DATA_SLOT', 1, 'K', 'Zapukaj');
        }
        scaleForm.CallFunction('DRAW_INSTRUCTIONAL_BUTTONS', -1);
        drawScaleform = true;
    }
    else if (eventName == "DisposeBuildingComponents") {
        drawScaleform = false;
        if (buildingMenu != null) buildingMenu.killMenu();
    }
});

API.onUpdate.connect(() => {
    if (drawScaleform) API.renderScaleform(scaleForm, 0, 0, 1280, 720);
});

function showBuildingPanel(buildingInfo: System.Array<string>)
{
    buildingMenu = createMenu(1);
    let panel: Panel;
    let textElement: TextElement;

    //Panel budynku
    //Header z nazwą budynku
    panel = loginMenu.createPanel(0, 12, 4, 7, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText(buildingInfo[0]);
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.Offset = 18;

    //Guzik zamykania
    panel = loginMenu.createPanel(0, 19, 4, 1, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    panel.HoverBackgroundColor(150, 25, 25, 160);
    panel.Hoverable = true;
    textElement = panel.addText("X");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.Offset = 18;

    //opis budynku
    panel = loginMenu.createPanel(0, 12, 5, 8, 7);
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText(buildingInfo[1]);
    textElement.Color(255, 255, 255, 255);
    panel.addText("");

    //Opłata za przejście
    if (buildingInfo[2] !== "")
    {
        textElement = panel.addText(`Opłata za przejście $${buildingInfo[2]}`);
        textElement.Color(255, 255, 255, 255);
        panel.addText("");
    }

    //Guzik kupowania 
    if (buildingInfo[3] !== "")
    {
        textElement = panel.addText(`Cena $${buildingInfo[3]}`);
        textElement.Color(255, 255, 255, 255);
        panel = loginMenu.createPanel(0, 12, 12, 8, 1);
        panel.MainBackgroundColor(0, 0, 0, 160);
        panel.HoverBackgroundColor(25, 25, 25, 160);
        panel.Hoverable = true;
        panel.Function = () => { API.triggerServerEvent("BuyBuilding") };
        panel.Tooltip = "Kup ten budynek";
        textElement = panel.addText("Kup");
        textElement.Color(255, 255, 255, 255);
        textElement.HoverColor(0, 180, 255, 255);
        textElement.Centered = true;
        textElement.VerticalCentered = true;
    }
}