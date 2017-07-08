let reportMenu: Menu = null;

class ReportInfo
{
    public name: string;
    public showId: boolean;
    public contentPanel: InputPanel;
    public idPanel: InputPanel;

    constructor(name: string, showId: boolean) {
        this.name = name;
        this.showId = showId;
    }
} 

let reports: ReportInfo[] = [new ReportInfo("Pomoc", false),
    new ReportInfo("Cheater", true),
    new ReportInfo("BUG", false),
    new ReportInfo("Naruszenie zasad", true)];

function showReportMenu()
{
    reportMenu = createMenu(4);
    let panel: Panel;
    let textElement: TextElement;

    for (var i = 0; i < 3; i++)
    {
        panel = reportMenu.createPanel(i, 12, 4, 7, 1);
        panel.MainBackgroundColor(0, 0, 0, 175);
        panel.Header = true;
        textElement = panel.addText("Zgłoszenie");
        textElement.Color(255, 255, 255, 255);
        textElement.Centered = true;
        textElement.FontScale = 0.6;
        textElement.Offset = 18;

        panel = reportMenu.createPanel(i, 19, 4, 1, 1);
        panel.MainBackgroundColor(0, 0, 0, 175);
        panel.Header = true;
        panel.HoverBackgroundColor(150, 25, 25, 160);
        panel.Hoverable = true;
        panel.Function = () => reportMenu.killMenu();
        textElement = panel.addText("X");
        textElement.Color(255, 255, 255, 255);
        textElement.Centered = true;
        textElement.VerticalCentered = true;
        textElement.FontScale = 0.6;

        //Strzałka w lewo
        panel = loginMenu.createPanel(i, 12, 4, 1, 1);
        panel.MainBackgroundColor(0, 0, 0, 175);
        panel.Function = reportMenu.prevPage;
        panel.HoverBackgroundColor(25, 25, 25, 160);
        panel.Hoverable = true;
        textElement = panel.addText("<");
        textElement.Color(255, 255, 255, 255);
        textElement.Centered = true;
        textElement.VerticalCentered = true;
        textElement.FontScale = 0.6;
        textElement.HoverColor(0, 180, 255, 255);

        //Wyświetlanie obecnego typu
        panel = loginMenu.createPanel(i, 13, 4, 6, 1);
        panel.MainBackgroundColor(0, 0, 0, 175);
        panel.HoverBackgroundColor(25, 25, 25, 160);
        textElement = panel.addText(reports[i].name);
        textElement.Color(255, 255, 255, 255);
        textElement.Centered = true;
        textElement.VerticalCentered = true;
        textElement.FontScale = 0.6;

        //Strzałka w prawo
        panel = loginMenu.createPanel(i, 19, 4, 1, 1);
        panel.MainBackgroundColor(0, 0, 0, 175);
        panel.Function = reportMenu.nextPage;
        panel.HoverBackgroundColor(25, 25, 25, 160);
        panel.Hoverable = true;
        textElement = panel.addText(">");
        textElement.Color(255, 255, 255, 255);
        textElement.Centered = true;
        textElement.VerticalCentered = true;
        textElement.FontScale = 0.6;
        textElement.HoverColor(0, 180, 255, 255);

        //Treść raportu

        //Pole id tworzymy w zależności od typu raportu
        if (reports[i].showId)
        {
            panel = reportMenu.createPanel(i, 12, 5, 8, 4);
            panel.MainBackgroundColor(0, 0, 0, 160);
            textElement = panel.addText("ID zgłaszanej osoby:");
            textElement.Color(255, 255, 255, 255);
            textElement.FontScale = 0.5;
            panel.addText("");
            reports[i].idPanel = panel.addInput(0, 1, 8, 1);
            reports[i].idPanel.NumericOnly = true;
        }
        
        panel = reportMenu.createPanel(i, 12, reports[i].showId ? 5 : 0, 8, 4);
        panel.MainBackgroundColor(0, 0, 0, 160);
        textElement = panel.addText("Treść zgłoszenia:");
        textElement.Color(255, 255, 255, 255);
        textElement.FontScale = 0.5;
        panel.addText("");

        reports[i].contentPanel = panel.addInput(0, 1, 8, 1);

        //Guzik wyślij
        panel = reportMenu.createPanel(i, 12, reports[i].showId ? 10 : 0, 8, 1);
        panel.MainBackgroundColor(0, 0, 0, 160);
        panel.HoverBackgroundColor(25, 25, 25, 160);
        panel.Hoverable = true;
        panel.Function = () => {
            sendReport(reports[i].name, reports[i].contentPanel.Input, reports[i].idPanel.Input);
            reportMenu.killMenu();
            API.sendNotification("Zgłoszenie zostało wysłane.");
        };
        panel.Tooltip = "Wyślij zgłoszenie";
        textElement = panel.addText("Wyślij");
        textElement.Color(255, 255, 255, 255);
        textElement.HoverColor(0, 180, 255, 255);
        textElement.Centered = true;
        textElement.VerticalCentered = true;    
    }

    reportMenu.DisableOverlays(true);
    reportMenu.Ready = true;
}

function sendReport(type: string, content: string, id: string = "")
{
    API.triggerServerEvent("OnPlayerSendReport", content, id);
}

API.onServerEventTrigger.connect((eventName: string, args: any[]) => {
    if (eventName == "ShowReportMenu") {
        showReportMenu();
        API.sendNotification("Wiadomość w zgłoszeniu powinna być krótka i na temat...");
        API.sendNotification("...powielanie raportów wpływa negatywnie na czas rozpatrzenia...");
        API.sendNotification("...i może zakończyć się karą.");
    }
});