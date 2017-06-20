using System;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Bank.Models;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Core.Extenstions;

namespace Serverside.Bank
{
    public sealed class RPBank : Script
    {
        public RPBank()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPBank] Uruchomione pomyslnie.", ConsoleColor.DarkMagenta);

            foreach (var atm in XmlHelper.GetXmlObjects<AtmModel>(Constant.ConstantAssemblyInfo.XmlDirectory + @"Atms\"))
            {
                new Atm(API, atm); //Nowa instancja bankomatu spawnuje go w świecie gry
            }
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "OnPlayerAtmTake")
            {
                if (!Validator.IsMoneyStringValid(arguments[0].ToString())) return;
                RPChat.SendMessageToNearbyPlayers(sender,
                    $"wkłada {(Convert.ToDecimal(arguments[0]) > 3000 ? "gruby" : "chudy")} plik gotówki do bankomatu i po przetworzeniu operacji zabiera kartę.", ChatMessageType.ServerMe);
                BankHelper.TakeMoneyToBank(sender, Convert.ToDecimal(arguments[0]));
            }
            else if (eventName == "OnPlayerAtmGive")
            {
                if (!Validator.IsMoneyStringValid(arguments[0].ToString())) return;
                RPChat.SendMessageToNearbyPlayers(sender,
                    $"wyciąga z bankomatu {(Convert.ToDecimal(arguments[0]) > 3000 ? "gruby" : "chudy")} plik gotówki, oraz kartę.", ChatMessageType.ServerMe);
                BankHelper.GiveMoneyFromBank(sender, Convert.ToDecimal(arguments[0]));
            }
        }

        #region Komendy administracji

        [Command("dodajbankomat")]
        public void CreateAtm(Client sender)
        {
            sender.Notify("Ustaw się w wybranej pozycji, a następnie wpisz /tu.");
            sender.Notify("...użyj /diag aby poznać swoją obecną pozycję.");

            API.onChatCommand += Handler;

            void Handler(Client o, string command, CancelEventArgs cancel)
            {
                if (o == sender && command == "/tu")
                {
                    AtmModel atm = new AtmModel
                    {
                        Position = new FullPosition
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
                        }
                    };
                    XmlHelper.AddXmlObject(atm, Constant.ConstantAssemblyInfo.XmlDirectory + @"Atms\");
                    new Atm(API, atm); //Nowa instancja bankomatu spawnuje go w świecie gry
                    sender.Notify("Dodawanie bankomatu zakończyło się pomyślnie.");
                    API.onChatCommand -= Handler;
                }
            }
        }
        #endregion
    }
}
