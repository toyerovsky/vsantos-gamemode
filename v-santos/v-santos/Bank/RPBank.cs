/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using Serverside.Admin.Enums;
using Serverside.Bank.Models;
using Serverside.Core;
using Serverside.Core.Extensions;

namespace Serverside.Bank
{
    public sealed class RPBank : Script
    {
        private List<Atm> Atms { get; set; } = new List<Atm>();

        public RPBank()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPBank] Uruchomione pomyslnie.", ConsoleColor.DarkMagenta);

            foreach (var atm in XmlHelper.GetXmlObjects<AtmModel>($@"{Constant.ConstantAssemblyInfo.XmlDirectory}Atms\"))
            {
                Atms.Add(new Atm(API, atm));
            }
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "OnPlayerAtmTake")
            {
                if (decimal.TryParse(arguments[0].ToString(), out decimal money))
                {
                    RPChat.SendMessageToNearbyPlayers(sender,
                        $"wkłada {(money >= 3000 ? "gruby" : "chudy")} plik gotówki do bankomatu i po przetworzeniu operacji zabiera kartę.", ChatMessageType.ServerMe);
                    BankHelper.TakeMoneyToBank(sender, money);
                }
            }
            else if (eventName == "OnPlayerAtmGive")
            {
                if (decimal.TryParse(arguments[0].ToString(), out decimal money))
                {
                    RPChat.SendMessageToNearbyPlayers(sender,
                        $"wyciąga z bankomatu {(money >= 3000 ? "gruby" : "chudy")} plik gotówki, oraz kartę.", ChatMessageType.ServerMe);
                    BankHelper.GiveMoneyFromBank(sender, money);
                }
            }
        }

        #region ADMIN COMMANDS

        [Command("dodajbankomat")]
        public void CreateAtm(Client sender)
        {
            if (sender.GetAccountController().AccountData.ServerRank < ServerRank.GameMaster)
            {
                sender.Notify("Nie posiadasz uprawnień do tworzenia grupy.");
                return;
            }

            sender.Notify("Ustaw się w wybranej pozycji, a następnie wpisz /tu.");
            sender.Notify("...użyj /diag aby poznać swoją obecną pozycję.");

            API.onChatCommand += Handler;

            void Handler(Client o, string command, CancelEventArgs cancel)
            {
                if (o == sender && command == "/tu")
                {
                    AtmModel atm = new AtmModel
                    {
                        CreatorForumName = o.GetAccountController().AccountData.Name,
                        Position = new FullPosition
                        {
                            Position = new Vector3
                            {
                                X = o.position.X,
                                Y = o.position.Y,
                                Z = o.position.Z
                            },

                            Rotation = new Vector3
                            {
                                X = o.rotation.X,
                                Y = o.rotation.Y,
                                Z = o.rotation.Z
                            }
                        }
                    };
                    XmlHelper.AddXmlObject(atm, $@"{Constant.ConstantAssemblyInfo.XmlDirectory}Atms\");
                    Atms.Add(new Atm(API, atm)); //Nowa instancja bankomatu spawnuje go w świecie gry
                    sender.Notify("Dodawanie bankomatu zakończyło się ~h~~g~pomyślnie.");
                    API.onChatCommand -= Handler;
                }
            }
        }

        [Command("usunbankomat")]
        public void DeleteAtm(Client sender)
        {
            //if (sender.GetAccountController().AccountData.ServerRank < ServerRank.GameMaster)
            //{
            //    sender.Notify("Nie posiadasz uprawnień do usuwania bankomatu.");
            //    return;
            //}

            if (Atms.Count == 0)
            {
                sender.Notify("Nie znaleziono bankomatu który można usunąć.");
                return;
            }
            var atm = Atms.OrderBy(a => a.Data.Position.Position.DistanceTo(sender.position)).First();
            if (XmlHelper.TryDeleteXmlObject(atm.Data.FilePath))
            {
                sender.Notify("Usuwanie bankomatu zakończyło się ~h~~g~pomyślnie.");
                Atms.Remove(atm);
                atm.Dispose();
            }
            else
            {
                sender.Notify("Usuwanie bankomatu zakończyło się ~h~~r~niepomyślnie.");
            }
        }
        #endregion
    }
}
