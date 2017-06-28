let groupMenu: Menu;

class WorkerInfo {
    public serverId: string;
    public name: string;
    public salary: number;
    public dutyTime: number;
    public onDuty: boolean;

    constructor(serverId: string, name: string, salary: number, dutyTime: number, onDuty: boolean) {
        this.serverId = serverId;
        this.name = name;
        this.salary = salary;
        this.dutyTime = dutyTime;
        this.onDuty = onDuty;
    }
}

class GroupInfo {
    public name: string;
    public tag: string;
    public money: number;
    public color: string;
    public workers: WorkerInfo[];

    constructor(name: string, tag: string, money: number, color: string, workers: WorkerInfo[]) {
        this.name = name;
        this.tag = tag;
        this.money = money;
        this.color = color;
        this.workers = workers;
    }
}

function showGroupMenu(info: GroupInfo) {
    groupMenu = createMenu(3);
    let panel: Panel;
    let textElement: TextElement;

    for (var i = 0; i < 3; i++) {
        panel = groupMenu.createPanel(i, 12, 4, 3, 1);
        panel.MainBackgroundColor(0, 0, 0, 175);
        panel.Header = true;
        panel.HoverBackgroundColor(150, 25, 25, 160);
        panel.Hoverable = true;
        panel.Function = () => groupMenu.Page = 0;
        textElement = panel.addText("Informacje");
        textElement.Color(255, 255, 255, 255);
        textElement.Centered = true;
        textElement.FontScale = 0.6;
        textElement.Offset = 18;

        panel = groupMenu.createPanel(i, 15, 4, 3, 1);
        panel.MainBackgroundColor(0, 0, 0, 175);
        panel.Header = true;
        panel.HoverBackgroundColor(150, 25, 25, 160);
        panel.Hoverable = true;
        panel.Function = () => groupMenu.Page = 1;
        textElement = panel.addText("Zarządzaj");
        textElement.Color(255, 255, 255, 255);
        textElement.Centered = true;
        textElement.FontScale = 0.6;
        textElement.Offset = 18;

        panel = groupMenu.createPanel(i, 18, 4, 3, 1);
        panel.MainBackgroundColor(0, 0, 0, 175);
        panel.Header = true;
        panel.HoverBackgroundColor(150, 25, 25, 160);
        panel.Hoverable = true;
        panel.Function = () => groupMenu.Page = 2;
        textElement = panel.addText("Służba");
        textElement.Color(255, 255, 255, 255);
        textElement.Centered = true;
        textElement.FontScale = 0.6;
        textElement.Offset = 18;

        panel = groupMenu.createPanel(i, 19, 4, 1, 1);
        panel.MainBackgroundColor(0, 0, 0, 175);
        panel.Header = true;
        panel.HoverBackgroundColor(150, 25, 25, 160);
        panel.Hoverable = true;
        panel.Function = () => groupMenu.killMenu();
        textElement = panel.addText("X");
        textElement.Color(255, 255, 255, 255);
        textElement.Centered = true;
        textElement.VerticalCentered = true;
        textElement.FontScale = 0.6;
    }

    //Pole id tworzymy w zależności od typu raportu
    if (reports[i].showId) {
        panel = groupMenu.createPanel(i, 12, 5, 8, 4);
        panel.MainBackgroundColor(0, 0, 0, 160);
        textElement = panel.addText("ID zgłaszanej osoby:");
        textElement.Color(255, 255, 255, 255);
        textElement.FontScale = 0.5;
        panel.addText("");
        reports[i].idPanel = panel.addInput(0, 1, 8, 1);
        reports[i].idPanel.NumericOnly = true;
    }

    panel = groupMenu.createPanel(i, 12, reports[i].showId ? 5 : 0, 8, 4);
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText("Treść zgłoszenia:");
    textElement.Color(255, 255, 255, 255);
    textElement.FontScale = 0.5;
    panel.addText("");

    reports[i].contentPanel = panel.addInput(0, 1, 8, 1);

    //Guzik wyślij
    panel = groupMenu.createPanel(i, 12, reports[i].showId ? 10 : 0, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 160);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = () => {
        sendReport(reports[i].name, reports[i].contentPanel.Input, reports[i].idPanel.Input);
        groupMenu.killMenu();
        API.sendNotification("Zgłoszenie zostało wysłane.");
    };
    panel.Tooltip = "Wyślij zgłoszenie";
    textElement = panel.addText("Wyślij");
    textElement.Color(255, 255, 255, 255);
    textElement.HoverColor(0, 180, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;    


    groupMenu.DisableOverlays(true);
    groupMenu.Ready = true;
}

API.onServerEventTrigger.connect((eventName: string, args: System.Array<any>) => {
    if (eventName == "ShowGroupMenu") {

        var group = JSON.parse(args[0]);
        var players = JSON.parse(group.PlayerOnLine);

        var workers: WorkerInfo[] = [];

        for (var i = 0; i < players.length; i++) {
            workers.push(new WorkerInfo(players[i].ServerId,
                players[i].Name,
                players[i].Salary,
                players[i].DutyTime,
                players[i].OnDuty));
        }

        showGroupMenu(new GroupInfo(group.Name, group.Tag, group.Money, group.Color, workers));
    }
});