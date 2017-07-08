/// <reference path="../../types-gt-mp/index.d.ts" />

class BuildingInfo
{
    public name: string;
    public internalPosition: Vector3;
    public enterCharge: number;
    public description: string;
}

let drawScaleform: boolean = false;
let scaleForm: any = null;
let buildingMenu: Menu = null;
let buildings: BuildingInfo[] = [];
let buildingName: InputPanel;
let buildingDescription: InputPanel;
let buildingEnterCharge: InputPanel;

var menuPool: any = null;

API.onKeyDown.connect((sender: any, e: System.Windows.Forms.KeyEventArgs) =>
{
    if (e.KeyCode === Keys.E && API.hasEntitySyncedData(API.getLocalPlayer(), "DoorsTarget"))
    {
        API.triggerServerEvent("PassDoors");
    }
    else if (e.KeyCode === Keys.K && API.hasEntitySyncedData(API.getLocalPlayer(), "DoorsTarget"))
    {
        API.triggerServerEvent("KnockDoors");
    }
});

function getBuilding(name: string): BuildingInfo
{
    for (var i = 0; i < buildings.length; i++)
    {
        if (buildings[i].name == name)
        {
            return buildings[i];
        }
    }
    return null;
}

API.onServerEventTrigger.connect((eventName: string, args: System.Array<any>) =>
{
    if (eventName == "DrawBuildingComponents") {
        resource.Core.drawStreetHUD = false;
        scaleForm = API.requestScaleform('instructional_buttons');
        scaleForm.CallFunction('SET_DATA_SLOT_EMPTY');
        if (args[0])
        {
            scaleForm.CallFunction('SET_DATA_SLOT', 0, 't_E', 'Wejdź');
            scaleForm.CallFunction('SET_DATA_SLOT', 1, 't_K', 'Zapukaj');
            showBuildingPanel(args[1]);
        }
        else
        {
            scaleForm.CallFunction('SET_DATA_SLOT', 0, 't_E', 'Wyjdź');
            scaleForm.CallFunction('SET_DATA_SLOT', 1, 't_K', 'Zapukaj');
        }
        scaleForm.CallFunction('DRAW_INSTRUCTIONAL_BUTTONS', -1);
        drawScaleform = true;
    }
    else if (eventName == "DisposeBuildingComponents")
    {
        drawScaleform = false;
        resource.Core.drawStreetHUD = true;
        if (buildingMenu != null) buildingMenu.killMenu();
    }
    else if (eventName == "ShowBuildingManagePanel")
    {
        var buildingInfo = new BuildingInfo();
        var info = args[0];
        buildingInfo.name = info[0];
        buildingInfo.description = info[1];
        buildingInfo.enterCharge = info[2];
        showBuildingManagePanel(buildingInfo);    
    }
    else if (eventName == "ShowAdminBuildingMenu")
    {
        menuPool = API.getMenuPool();
        var menu = API.createMenu("Dodaj budynek", 0, 0, 6);

        var newBuilding: BuildingInfo;

        var list = JSON.parse(args[0]);
        for (var i = 0; i < list.length; i++)
        {
            newBuilding = new BuildingInfo();
            newBuilding.name = list[i].Name;
            newBuilding.internalPosition = list[i].InternalPosition;
            buildings.push(newBuilding);
        }

        var interiorNames = new List(String);
        for (var j = 0; j < buildings.length; j++)
        {
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

        var nameResult: string = null;
        var descriptionResult: string = "";
        var interiorResult: string = null;
        var costResult: string = null;

        menu.OnItemSelect.connect((sender, item, index) =>
        {
            if (item === nameItem)
            {
                nameResult = API.getUserInput("Nazwa budynku", 30);
            }
            else if (item === costItem)
            {
                costResult = API.getUserInput("Cena", 10);
                if (costResult.trim() == "" || isNaN(parseInt(costResult))) {
                    API.sendNotification("Wprowadzono nieporawny koszt.");
                    costResult = null;
                }
            }
            else if (item == descriptionItem)
            {
                descriptionResult = API.getUserInput("Opis budynku", 100);
            }
            else if (item == saveItem)
            {
                if (nameResult != null &&
                    costResult != null &&
                    interiorResult != null)
                {
                    API.triggerServerEvent("AddBuilding", nameResult, costResult, interiorResult, descriptionResult);
                    menu.Visible = false;
                }
                else
                {
                    API.sendNotification("Dane nie zostały zaincjalizowane.");
                }
            }
        });

        interiorsItem.OnListChanged.connect((sender, newIndex) =>
        {
            interiorResult = sender.IndexToItem(sender.Index);
            let building = getBuilding(interiorResult);
            if (building != null)
            {
                //Jak przesyłamy Vector3 to nie działa
                API.triggerServerEvent("ChangePosition", building.internalPosition.X, building.internalPosition.Y, building.internalPosition.Z);
            }
        });
    }
});

API.onUpdate.connect(() =>
{
    if (drawScaleform) API.renderScaleform(scaleForm, 0, 0, 1280, 720);
    if (menuPool != null) {
        menuPool.ProcessMenus();
    }
});

function showBuildingPanel(buildingInfo: System.Array<string>)
{
    buildingMenu = createMenu(1);
    let panel: Panel;
    let textElement: TextElement;

    //Panel budynku
    //Header z nazwą budynku
    panel = buildingMenu.createPanel(0, 12, 4, buildingInfo[3] !== "" ? 7 : 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText(buildingInfo[0]);
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.FontScale = 0.6;
    textElement.Offset = 18;

    //Guzik zamykania pokazujemy tylko kiedy jest włączony guzik kupowania
    if (buildingInfo[3] !== "") {
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
    }

    //Opis budynku
    panel = buildingMenu.createPanel(0, 12, 5, 8, 4);
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText(buildingInfo[1]);
    textElement.Color(255, 255, 255, 255);
    textElement.FontScale = 0.5;
    panel.addText("");

    //Opłata za przejście
    if (buildingInfo[2] !== "")
    {
        textElement = panel.addText(`Opłata za przejście: $${buildingInfo[2]}`);
        textElement.Color(255, 255, 255, 255);
        textElement.FontScale = 0.5;
    }

    //Guzik kupowania 
    if (buildingInfo[3] !== "") {
        //panel ceny
        panel = buildingMenu.createPanel(0, 12, 9, 8, 1);
        panel.MainBackgroundColor(0, 0, 0, 160);
        textElement = panel.addText(`Cena $${buildingInfo[3]}`);
        textElement.Color(255, 255, 255, 255);
        textElement.FontScale = 0.5;

        panel = buildingMenu.createPanel(0, 12, 10, 8, 1);
        panel.MainBackgroundColor(0, 0, 0, 160);
        panel.HoverBackgroundColor(25, 25, 25, 160);
        panel.Hoverable = true;
        panel.Function = () => {
            API.triggerServerEvent("BuyBuilding");
            buildingMenu.killMenu();
        };
        panel.Tooltip = "Kup ten budynek";
        textElement = panel.addText("Kup");
        textElement.Color(255, 255, 255, 255);
        textElement.HoverColor(0, 180, 255, 255);
        textElement.Centered = true;
        textElement.VerticalCentered = true;
    }

    buildingMenu.DisableOverlays(buildingInfo[3] != "");
    buildingMenu.Ready = true;
}

function showBuildingManagePanel(buildingInfo: BuildingInfo)
{
    buildingMenu = createMenu(1);
    let panel: Panel;
    let textElement: TextElement;

    //Panel budynku
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

    //Opłata za przejście
    textElement = panel.addText("Opłata za przejście:");
    textElement.Color(255, 255, 255, 255);
    panel.addText("");

    buildingName = panel.addInput(0, 1, 8, 1);
    buildingName.Input = buildingInfo.name;
    buildingDescription = panel.addInput(0, 3, 8, 2);
    buildingDescription.Input = buildingInfo.description;
    buildingEnterCharge = panel.addInput(0, 6, 8, 1);
    buildingEnterCharge.Input = buildingInfo.enterCharge.toString();
    buildingEnterCharge.NumericOnly = true;

    //Guzik zapisywania
    panel = buildingMenu.createPanel(0, 12, 12, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 160);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = () =>
    {
        if (buildingName.Input.trim() == "")
        {
            buildingName.isError = true;
        } 
        if (buildingDescription.Input.trim() == "")
        {
            buildingDescription.isError = true;
        }

        if (buildingName.isError || buildingDescription.isError) {
            return;
        }

        API.triggerServerEvent("EdingBuildingInfo",
            buildingName.Input,
            buildingDescription.Input,
            buildingEnterCharge.Input);
        buildingMenu.killMenu();  
    };

    panel.Tooltip = "Zapisz bieżące informacje";
    textElement = panel.addText("Zapisz");
    textElement.Color(255, 255, 255, 255);
    textElement.HoverColor(0, 180, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;

    buildingMenu.DisableOverlays(true);
    buildingMenu.Ready = true;
}
