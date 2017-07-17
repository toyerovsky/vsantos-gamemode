/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

class AdminInfo {
    public id: number;
    public forumName: string;
    public rank: string;

    constructor(id: number, forumName: string, rank: string) {
        this.id = id;
        this.forumName = forumName;
        this.rank = rank;
    }
}
    
let adminListMenu: Menu;

function showAdminListMenu(admins: AdminInfo[]) {
    adminListMenu = createMenu(1);
    let panel: Panel;
    let textElement: TextElement;

    panel = adminListMenu.createPanel(0, 12, 4, 7, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText("Administratorzy Online");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.FontScale = 0.6;
    textElement.Offset = 18;

    panel = adminListMenu.createPanel(0, 19, 4, 1, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    panel.HoverBackgroundColor(150, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = () => adminListMenu.killMenu();
    textElement = panel.addText("X");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;

    var start = 5;
    for (var i = 0; i < admins.length; i++) {
        panel = adminListMenu.createPanel(0, 12, start + i, 8, 1);
        panel.MainBackgroundColor(0, 0, 0, 175);
        panel.Header = true;
        textElement = panel.addText(`${admins[i].id} ${admins[i].forumName} ${prepareRank(admins[i].rank)}`);
        textElement.Color(255, 255, 255, 255);
        textElement.Centered = true;
        textElement.FontScale = 0.6;
        textElement.Offset = 18;
    }

    adminListMenu.DisableOverlays(true);
    adminListMenu.Ready = true;
}

API.onServerEventTrigger.connect((eventName: string, args: any[]) => {
    if (eventName == "ShowAdminsOnDuty") {

        var list = args[0];
        var info: AdminInfo[] = [];

        for (var i = 0; i < list.length; i++) {
            info.push(new AdminInfo(list[i].ServerId, list[i].ForumName, list[i].Rank));
        }

        showAdminListMenu(info);
    }
});

function prepareRank(rank: string)
{
    return `${rank.charAt(0)}${rank.charAt(rank.length - 1)}`;
}