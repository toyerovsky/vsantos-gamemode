using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using Serverside.Admin.Structs;
using Serverside.Controllers;
using Serverside.Core.Extensions;

namespace Serverside.Admin
{
    public class RPAdminList : Script
    {
        private static List<AccountController> AdminsOnDuty { get; set; } = new List<AccountController>();
        private static List<ReportData> CurrentReports { get; set; } = new List<ReportData>();

        public RPAdminList()
        {
            API.onResourceStart += OnResourceStartHandler;
            API.onClientEventTrigger += OnClientEventTriggerHandler;
        }

        private void OnClientEventTriggerHandler(Client sender, string eventName, params object[] arguments)
        {
            /* Arguments
             * args[0] string reportType
             * args[1] string content
             * args[2] string accusedId = null
             */
            if (eventName == "OnPlayerSendReport")
            {
                ReportData r = new ReportData();
                
            }
        }

        private void OnResourceStartHandler()
        {
            APIExtensions.ConsoleOutput("[RPAdminList] Uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
        }

        [Command("a")]
        public void ShowAdministratorsList(Client sender)
        {
            if (AdminsOnDuty.Count == 0)
            {
                sender.Notify("Obecnie nie ma administratorów na służbie.");
                return;
            }

            sender.triggerEvent("ShowAdminsOnDuty", AdminsOnDuty.Select(x => new
            {
                x.ServerId,
                ForumName = x.AccountData.Name,
                Rank = x.AccountData.ServerRank.ToString(),
            }).OrderBy(x => x.Rank));
        }

        [Command("listreports", Alias = "rl")]
        public void ShowCurrentReports(Client sender)
        {
            
        }

        [Command("report")]
        public void SendReport(Client sender)
        {
            sender.triggerEvent("ShowReportMenu");
        }

        [Command("aduty")]
        public void EnterAdminDuty(Client sender)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.Support)
            {
                sender.Notify("Nie posiadasz uprawnień do służby administracyjnej.");
                return;
            }

            var player = sender.GetAccountController();

            if (AdminsOnDuty.Any(a => a.AccountId == player.AccountId))
            {
                AdminsOnDuty.Remove(player);
                sender.Notify($"Zszedłeś ze służby ~{APIExtensions.GetRankColor(player.AccountData.ServerRank).ToHex()}~ {player.AccountData.ServerRank.ToString().Where(char.IsLetter)} ~w~ życzymy miłej gry.");
            }
            else
            {
                AdminsOnDuty.Add(player);
                sender.Notify($"Wszedłeś na służbę ~{APIExtensions.GetRankColor(player.AccountData.ServerRank).ToHex()}~ {player.AccountData.ServerRank.ToString().Where(char.IsLetter)} ~w~ życzymy cierpliwości.");
            }

        }
    }
}