using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using Newtonsoft.Json;
using Serverside.Admin.Structs;
using Serverside.Controllers;
using Serverside.Core;
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
             * args[2] string accusedId = ""
             */
            if (eventName == "OnPlayerSendReport")
            {
                ReportData data = new ReportData
                {
                    Type = (ReportType) Enum.Parse(typeof(ReportType), arguments[0].ToString().Replace(' ', '_')),
                    Content = arguments[1].ToString(),
                    Accused = arguments[2].ToString() != ""
                        ? RPEntityManager.GetAccountByServerId(Convert.ToInt32(arguments[2]))
                        : null,
                    Sender = sender.GetAccountController()
                };

                CurrentReports.Add(data);
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

        [Command("listreports", Alias = "lr")]
        public void ShowCurrentReports(Client sender)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.Support)
            {
                sender.Notify("Nie posiadasz uprawnień do przeglądania raportów.");
                return;
            }

            sender.triggerEvent("ShowAdminReportMenu", JsonConvert.SerializeObject(CurrentReports.Select(x => new
            {
                SenderName = x.Sender.CharacterController.FormatName,
                SenderId = x.Sender.ServerId.ToString(),
                AccusedName = x.Accused?.CharacterController.FormatName ?? "",
                AccusedId = x.Accused?.ServerId.ToString() ?? "",
                x.Content,
                ReportType = x.Type.ToString().Replace('_', ' ')
            })));
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
                API.sendChatMessageToPlayer(sender, $"Zszedłeś ze służby ~{APIExtensions.GetRankColor(player.AccountData.ServerRank).ToHex()}~ {player.AccountData.ServerRank.ToString().Where(char.IsLetter)} ~w~ życzymy miłej gry.");
            }
            else
            {
                AdminsOnDuty.Add(player);
                API.sendChatMessageToPlayer(sender, $"Wszedłeś na służbę ~{APIExtensions.GetRankColor(player.AccountData.ServerRank).ToHex()}~ {player.AccountData.ServerRank.ToString().Where(char.IsLetter)} ~w~ życzymy cierpliwości.");
            }

        }
    }
}