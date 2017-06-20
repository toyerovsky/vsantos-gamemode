using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Core.Extenstions;
using Serverside.Corners.Helpers;
using Serverside.Corners.Models;

namespace Serverside.Corners
{
    public sealed class RPCorners : Script
    {
        public RPCorners()
        {
            API.onResourceStart += OnResourceStartHandler;
        }

        public void OnResourceStartHandler()
        {
            APIExtensions.ConsoleOutput("[RPCorners] uruchomione pomyślnie!", ConsoleColor.DarkMagenta);
            foreach (var corner in XmlHelper.GetXmlObjects<CornerModel>(Constant.ConstantAssemblyInfo.XmlDirectory + @"Corners\"))
            {
                new Corner(corner);
            }
        }

        #region Komendy administracji
        [Command("dodajrog", "~y~UŻYJ: ~w~ /dodajrog [id npc np: 1, 2, 8, 4, 5]", GreedyArg = true)]
        public void AddCorner(Client sender, string ids)
        {
            var botIds = ids.Split(',').ToList();
            List<int> correctBotIds;

            //Sprawdzamy czy gracz podał prawidłowe ID NPC
            if (!CornerBotHelper.TryGetCornerBotIds(botIds, out correctBotIds))
            {
                sender.Notify("Podano dane w nieprawidłowym formacie. Lub podany NPC nie istnieje.");
                return;
            }

            sender.Notify("Ustaw się w wybranej pozycji, a następnie wpisz /tu.");
            sender.Notify("...użyj /diag aby poznać swoją obecną pozycję.");

            FullPosition position = null;
            List<FullPosition> botPositions = new List<FullPosition>();

            API.onChatCommand += Handler;

            void Handler(Client o, string command, CancelEventArgs cancel)
            {
                if (position == null && o == sender && command == "/tu")
                {
                    cancel.Cancel = true;
                    position = new FullPosition
                    {
                        Position = new Vector3
                        {
                            X = sender.position.X,
                            Y = sender.position.Y,
                            Z = sender.position.Z - 0.5f
                        },

                        Rotation = new Vector3
                        {
                            X = sender.rotation.X,
                            Y = sender.rotation.Y,
                            Z = sender.rotation.Z
                        }
                    };

                    sender.Notify("Wyznacz trasę npc na rogu. Ustaw się w danym punkcie i wpisz /poz.");
                    sender.Notify("Aby zacząć od nowa wpisz /reset");
                    sender.Notify("Aby usunąć ostatnią pozycję wpisz /usun");
                }
                else if (position != null && o == sender && command == "/poz")
                {
                    cancel.Cancel = true;
                    botPositions.Add(new FullPosition
                    {
                        Position = new Vector3
                        {
                            X = sender.position.X,
                            Y = sender.position.Y,
                            Z = sender.position.Z
                        },

                        Rotation = new Vector3
                        {
                            X = sender.rotation.X,
                            Y = sender.rotation.Y,
                            Z = sender.rotation.Z
                        }
                    });

                    sender.Notify($"Obecna liczba punktów: {botPositions.Count}. Aby zakończyć wpisz /zakoncz");
                    sender.Notify("Aby zacząć od nowa wpisz /reset");
                }
                else if (position != null && botPositions.Count != 0 && o == sender && command == "/zakoncz")
                {
                    CornerModel corner = new CornerModel
                    {
                        Position = position,
                        CornerBots = XmlHelper.GetXmlObjects<CornerBotModel>(Constant.ConstantAssemblyInfo.XmlDirectory + @"CornerBots\").Where(e => correctBotIds.Contains(e.BotId)).ToList(),
                        BotPositions = botPositions
                    };
                    //Dodajemy nowy plik .xml
                    XmlHelper.AddXmlObject(corner, Constant.ConstantAssemblyInfo.XmlDirectory + @"Corners\");
                    new Corner(corner);

                    sender.Notify("Dodawanie rogu zakończyło się pomyślnie.");
                    API.onChatCommand -= Handler;
                }
                else if (botPositions.Count != 0 && position != null && o == sender && command == "/usun")
                {
                    botPositions.RemoveAt(botPositions.Count);
                }
                else if (botPositions.Count != 0 && position != null && o == sender && command == "/reset")
                {
                    position = null;
                    botPositions = new List<FullPosition>();
                    sender.Notify("Ustaw się w wybranej pozycji, a następnie wpisz /tu.");
                    sender.Notify("...użyj /diag aby poznać swoją obecną pozycję.");
                }
            };
        }
        #endregion
    }
}
