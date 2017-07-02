let groupMenu: Menu;
let idWorkerText: TextElement;
let nameWorkerText: TextElement;
let salaryWorkerText: TextElement;
let dutyTimeWorkerText: TextElement;
let onDutyWorkerText: TextElement;

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

    getOnDutyWorkers() {
        var w: WorkerInfo[] = [];
        for (var i = 0; i < this.workers.length; i++) {
            if (this.workers[i].onDuty) w.push(this.workers[i]);
        }
        return w;
    }
}

function showGroupMenu(info: GroupInfo, manager: boolean) {
    groupMenu = createMenu(4);
    let panel: Panel;
    let textElement: TextElement;

    for (var i = 0; i < 3; i++) {
        panel = groupMenu.createPanel(i, 12, 4, 3, 1);
        panel.MainBackgroundColor(0, 0, 0, 175);
        panel.Header = true;
        panel.HoverBackgroundColor(25, 25, 25, 160);
        panel.Hoverable = true;
        panel.Function = () => groupMenu.Page = 0;
        textElement = panel.addText("Informacje");
        textElement.Color(255, 255, 255, 255);
        textElement.Centered = true;
        textElement.FontScale = 0.6;

        panel = groupMenu.createPanel(i, 15, 4, manager ? 2 : 4, 1);
        panel.MainBackgroundColor(0, 0, 0, 175);
        panel.Header = true;
        panel.HoverBackgroundColor(25, 25, 25, 160);
        panel.Hoverable = true;
        panel.Function = () => groupMenu.Page = 2;
        textElement = panel.addText("Służba");
        textElement.Color(255, 255, 255, 255);
        textElement.Centered = true;
        textElement.FontScale = 0.6;

        if (manager) {
            panel = groupMenu.createPanel(i, 17, 4, 2, 1);
            panel.MainBackgroundColor(0, 0, 0, 175);
            panel.Header = true;
            panel.HoverBackgroundColor(25, 25, 25, 160);
            panel.Hoverable = true;
            panel.Function = () => {

                groupMenu.Page = 1;
            };
            textElement = panel.addText("Zarządzaj");
            textElement.Color(255, 255, 255, 255);
            textElement.Centered = true;
            textElement.FontScale = 0.6;
        }

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

    //Strona 0
    //Informacje
    panel = groupMenu.createPanel(0, 12, 5, 8, 5);
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText(`Nazwa: ${info.name}`);
    textElement.Color(255, 255, 255, 255);
    textElement.FontScale = 0.5;
    textElement.Centered = true;

    textElement = panel.addText(`Tag: ${info.tag}`);
    textElement.Color(255, 255, 255, 255);
    textElement.FontScale = 0.5;
    textElement.Centered = true;

    textElement = panel.addText(`Gotówka: ${info.money.toString()}`);
    textElement.Color(255, 255, 255, 255);
    textElement.FontScale = 0.5;
    textElement.Centered = true;

    textElement = panel.addText(`Kolor: ${info.color.toString()}`);
    textElement.Color(255, 255, 255, 255);
    textElement.FontScale = 0.5;
    textElement.Centered = true;

    textElement = panel.addText(`Na służbie: ${info.getOnDutyWorkers().length}`);
    textElement.Color(255, 255, 255, 255);
    textElement.FontScale = 0.5;
    textElement.Centered = true;

    //Strona 1
    //Zarządzanie poszczególnymi pracownikami

    panel = groupMenu.createPanel(1, 12, 5, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText(`On-Line: ${info.workers.length}`);
    textElement.Color(255, 255, 255, 255);
    textElement.FontScale = 0.5;
    textElement.Centered = true;

    var start = 6;
    for (var j = 0; j < info.workers.length; j++) {
        var workerInfo = info.workers[j];
        panel = groupMenu.createPanel(1, 12, start + j, 8, 1);
        panel.MainBackgroundColor(0, 0, 0, 160);
        panel.HoverBackgroundColor(25, 25, 25, 160);
        panel.Hoverable = true;
        panel.Function = () => {

            var worker = Object.assign({}, workerInfo);

            idWorkerText.Text = `Id: ${worker.serverId.toString()}`;
            nameWorkerText.Text = worker.name;
            salaryWorkerText.Text = `Wypłata: $${worker.salary.toString()}`;
            dutyTimeWorkerText.Text = `Służba: ${worker.dutyTime.toString()} min`;
            var duty = worker.onDuty ? "Tak" : "Nie";
            onDutyWorkerText.Text = `Obecnie na służbie: ${duty}`;
            groupMenu.Page = 3;
        };
        panel.Tooltip = "Zarządzaj pracownikiem";
        textElement = panel.addText(info.workers[j].name);
        textElement.Color(255, 255, 255, 255);
        textElement.HoverColor(75, 109, 35, 255);
        textElement.FontScale = 0.5;
        textElement.Centered = true;
        textElement.VerticalCentered = true;
    }

    //Strona 2
    //Spis pracowników na służbie

    panel = groupMenu.createPanel(2, 12, 5, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText(`Na służbie: ${info.getOnDutyWorkers().length}`);
    textElement.Color(255, 255, 255, 255);
    textElement.FontScale = 0.5;
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    panel.addText("");

    for (var k = 0; k < info.getOnDutyWorkers().length; k++) {
        panel = groupMenu.createPanel(2, 12, start + k, 8, 1);
        panel.MainBackgroundColor(0, 0, 0, 160);
        panel.Hoverable = true;
        panel.HoverBackgroundColor(25, 25, 25, 160);
        textElement = panel.addText(info.getOnDutyWorkers()[k].name);
        textElement.Color(255, 255, 255, 255);
        textElement.HoverColor(75, 109, 35, 255);
        textElement.Centered = true;
        textElement.VerticalCentered = true;
        textElement.FontScale = 0.5;
    }

    //Strona 3 
    //panel zarządzania wybranym pracownikiem
    //Dostępne tylko dla uprawnień rekrutacyjnych
    if (manager) {

        panel = groupMenu.createPanel(3, 12, 4, 1, 1);
        panel.MainBackgroundColor(0, 0, 0, 175);
        panel.Header = true;
        panel.HoverBackgroundColor(25, 25, 25, 160);
        panel.Hoverable = true;
        panel.Function = () => groupMenu.Page = 1;
        textElement = panel.addText("<");
        textElement.Color(255, 255, 255, 255);
        textElement.Centered = true;
        textElement.VerticalCentered = true;
        textElement.FontScale = 0.6; 

        panel = groupMenu.createPanel(3, 13, 4, 6, 1);
        panel.MainBackgroundColor(0, 0, 0, 175);
        panel.Header = true;
        textElement = panel.addText("Zarządzanie");
        textElement.Color(255, 255, 255, 255);
        textElement.Centered = true;
        textElement.VerticalCentered = true;
        textElement.FontScale = 0.6; 

        panel = groupMenu.createPanel(3, 19, 4, 1, 1);
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

        panel = groupMenu.createPanel(3, 12, 5, 8, 5);
        panel.MainBackgroundColor(0, 0, 0, 160);

        idWorkerText = panel.addText("");
        idWorkerText.Color(255, 255, 255, 255);
        idWorkerText.FontScale = 0.5;
        idWorkerText.Centered = true;

        nameWorkerText = panel.addText("");
        nameWorkerText.Color(255, 255, 255, 255);
        nameWorkerText.FontScale = 0.5;
        nameWorkerText.Centered = true;

        salaryWorkerText = panel.addText("");
        salaryWorkerText.Color(255, 255, 255, 255);
        salaryWorkerText.FontScale = 0.5;
        salaryWorkerText.Centered = true;

        dutyTimeWorkerText = panel.addText("");
        dutyTimeWorkerText.Color(255, 255, 255, 255);
        dutyTimeWorkerText.FontScale = 0.5;
        dutyTimeWorkerText.Centered = true;

        onDutyWorkerText = panel.addText("");
        onDutyWorkerText.Color(255, 255, 255, 255);
        onDutyWorkerText.FontScale = 0.5;
        onDutyWorkerText.Centered = true;
    }

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

        showGroupMenu(new GroupInfo(group.Name, group.Tag, group.Money, group.Color, workers), args[1]);
    }
});