/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

/// <reference path="../../types-gt-mp/index.d.ts" />

class PackageInfo
{
    id: number;
    getter: string;

    constructor(id: number, getter: string)
    {
        this.id = id;
        this.getter = getter;
    }
}

var courierMenu: Menu;

API.onServerEventTrigger.connect((eventName: string, args: any[]) => {
    if (eventName == "ShowCourierMenu") {

        var json = JSON.parse(args[0]);
        var info: PackageInfo[] = [];

        for (var i = 0; i < json.length; i++) {
            info.push(new PackageInfo(json[i].Id, json[i].Getter));
        }
        showCourierMenuPanel(info);

    }
    else if (eventName == "DisposeCourierMenu") {

        if (courierMenu != null) courierMenu.killMenu();
    }
});

function showCourierMenuPanel(packagesInfo: PackageInfo[]) {
    courierMenu = createMenu(1);
    let panel: Panel;
    let textElement: TextElement;

    //Panel magazynu
    //Header
    panel = courierMenu.createPanel(0, 12, 4, 7, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText("Magazyn paczek");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.FontScale = 0.6;
    textElement.Offset = 18;

    //Guzik zamykania
    panel = courierMenu.createPanel(0, 19, 4, 1, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    panel.HoverBackgroundColor(150, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = () => courierMenu.killMenu();
    textElement = panel.addText("X");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;

    panel = groupMenu.createPanel(1, 12, 2, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText(`Paczek: ${packagesInfo.length}`);
    textElement.Color(255, 255, 255, 255);
    textElement.FontScale = 0.5;
    textElement.Centered = true;

    var start = 3;
    for (var j = 0; j < packagesInfo.length; j++) {

        var packageC = packagesInfo[j];
        panel = groupMenu.createPanel(1, 12, start + j, 8, 1);
        panel.MainBackgroundColor(0, 0, 0, 160);
        panel.HoverBackgroundColor(25, 25, 25, 160);
        panel.Hoverable = true;
        panel.Function = () => {

            var package = Object.assign({}, packageC);
            API.triggerServerEvent("OnPlayerTakePackage", package.id);
        };
        panel.Tooltip = "Podejmij dostawę";
        textElement = panel.addText(packagesInfo[j].getter);
        textElement.Color(255, 255, 255, 255);
        textElement.HoverColor(75, 109, 35, 255);
        textElement.FontScale = 0.5;
        textElement.Centered = true;
        textElement.VerticalCentered = true;
    }
    
    courierMenu.Ready = true;
}