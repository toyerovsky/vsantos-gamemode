using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using Serverside.Admin.Enums;
using Serverside.Core.Extensions;
using Serverside.Database;
using Serverside.Jobs.Dustman;
using Serverside.Jobs.Greenkeeper;
using Serverside.Core;
using Serverside.Jobs.Base;
using Serverside.Jobs.Dustman.Models;

namespace Serverside.Jobs
{
    public class RPJobs : Script
    {
        public static List<JobController> Jobs { get; set; } = new List<JobController>();
        public  static List<GarbageModel> Garbages { get; set; } = new List<GarbageModel>();

        private bool _resetFlag = true;

        public RPJobs()
        {
            Jobs.Add(new DustmanJob(API, "Śmieciarz", 500));
            Jobs.Add(new GreenkeeperJob(API, "Ogrodnik", 400));

            API.onResourceStart += API_onResourceStart;
            //API.onUpdate += OnUpdate;   
        }

        private void API_onResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPJobs] Uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
            Garbages = XmlHelper.GetXmlObjects<GarbageModel>($"{Constant.ConstantAssemblyInfo.XmlDirectory}\\JobGarbages\\");

            foreach (var garbage in Garbages)
            {
                if (garbage.GtaPropId != 0)
                {
                    API.createObject(garbage.GtaPropId, garbage.Position, garbage.Rotation);
                }
            }
        }

        //private void OnUpdate()
        //{
        //    if (_resetFlag && Math.Abs(ServerInfo.Instance.Data.JobsResetTime.CompareTo(DateTime.Now)) <= 1000)
        //    {
        //        _resetFlag = false;
        //        ResetJobLimit();
        //    }
        //}

        #region STATIC

        private static void ResetJobLimit()
        {
            foreach (var character in ContextFactory.Instance.Characters)
            {
                if (character.JobLimit == 0)
                    continue;

                character.JobLimit = 0;
            }

            ContextFactory.Instance.SaveChanges();
        }

        public static GarbageModel GetRandomGarbage() => Garbages[new Random().Next(Garbages.Count)];

        #endregion

        #region ADMIN COMMANDS

        [Command("usunsmietnik")]
        public void DeleteGarbage(Client sender)
        {
            //if (sender.GetAccountController().AccountData.ServerRank < ServerRank.GameMaster)
            //{
            //    sender.Notify("Nie posiadasz uprawnień do usuwania bankomatu.");
            //    return;
            //}

            var garbage = Garbages.OrderBy(a => a.Position.DistanceTo(sender.position)).First();
            if (XmlHelper.TryDeleteXmlObject(garbage.FilePath))
            {
                if (garbage.GtaPropId != 0) API.deleteObject(sender, garbage.Position, garbage.GtaPropId);
                Garbages.Remove(garbage);
                sender.Notify("Usuwanie bankomatu zakończyło się pomyślnie.");
            }
            else
            {
                sender.Notify("Usuwanie bankomatu zakończyło się niepomyślnie.");
            }
        }

        [Command("dodajsmietnik", "~y~ UŻYJ ~w~ /dodajsmietnik (id obiektu)")]
        public void AddGarbage(Client sender, int prop = 0)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.GameMaster)
            {
                sender.Notify("Nie posiadasz uprawnień do tworzenia śmietnika.");
                return;
            }

            sender.Notify("Ustaw się w wybranej pozycji, a następnie wpisz /tu.");
            sender.Notify("...użyj /diag aby poznać swoją obecną pozycję.");

            API.onChatCommand += Handler;

            void Handler(Client o, string command, CancelEventArgs cancel)
            {
                if (o == sender && command == "/tu")
                {

                    GarbageModel garbage = new GarbageModel
                    {
                        GtaPropId = prop,
                        CreatorForumName = sender.GetAccountController().AccountData.Name,
                        Position = sender.position,
                        Rotation = sender.rotation
                    };

                    XmlHelper.AddXmlObject(garbage, $@"{Constant.ConstantAssemblyInfo.XmlDirectory}JobGarbages\");
                    Garbages.Add(garbage);
                    sender.Notify("Dodawanie śmitnika zakończyło się pomyślnie.");
                    API.onChatCommand -= Handler;
                }
            }

        }

        #endregion
    }
}