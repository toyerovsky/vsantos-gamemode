using System;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Core.Telephone.Booth.Models;
using Serverside.Core.Extensions;
using Serverside.Core.Extenstions;

namespace Serverside.Core.Telephone.Booth
{
    public class RPTelephoneBooth : Script
    {
        public RPTelephoneBooth()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onResourceStart += OnResourceStartHandler;
        }

        private void OnResourceStartHandler()
        {
            APIExtensions.ConsoleOutput("[RPTelephoneBooth] uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
            foreach (var booth in XmlHelper.GetXmlObjects<TelephoneBoothModel>(Constant.ConstantAssemblyInfo.XmlDirectory + @"Booths\"))
            {
                //W konstruktorze spawnujemy budkę telefoniczną do gry
                new TelephoneBooth(API, booth);
            }
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] args)
        {
            //args[0] to numer na jaki dzwoni
            if (eventName == "OnPlayerTelephoneBoothCall" && sender.HasData("Booth"))
            {
                TelephoneBooth booth = sender.GetData("Booth");
                if (sender.HasMoney(booth.Data.Cost))
                {
                    sender.RemoveMoney(booth.Data.Cost);
                    RPChat.SendMessageToNearbyPlayers(sender, "wrzuca monetę do automatu i wybiera numer", ChatMessageType.ServerMe);

                    if (API.getAllPlayers().Any(t => t.GetAccountController().CharacterController.CellphoneController.Number == Convert.ToInt32(args[0])))
                    {
                        Client getterPlayer = API.getAllPlayers()
                            .Single(t => t.GetAccountController().CharacterController.CellphoneController.Number ==
                                      Convert.ToInt32(args[0]));

                        if (getterPlayer.HasData("CellphoneTalking"))
                        {
                            API.shared.sendChatMessageToPlayer(sender, "~#ffdb00~",
                                "Wybrany abonent prowadzi obecnie rozmowę, spróbuj później.");
                            return;
                        }

                        booth.CurrentCall = new TelephoneCall(sender, getterPlayer, booth.Data.Number);

                        booth.CurrentCall.Timer.Elapsed += (o, eventArgs) =>
                        {
                            API.shared.sendChatMessageToPlayer(sender, "~#ffdb00~",
                                "Wybrany abonent ma wyłączony telefon, bądź znajduje się poza zasięgiem, spróbuj później.");
                            booth.CurrentCall.Dispose();
                            booth.CurrentCall = null;
                        };
                    }
                    else
                    {
                        API.shared.sendChatMessageToPlayer(sender, "~#ffdb00~",
                            "Wybrany abonent ma wyłączony telefon, bądź znajduje się poza zasięgiem, spróbuj później.");
                    }
                }
                else
                {
                    sender.Notify("Nie posiadasz wystarczającej ilości gotówki.");
                }
            }
            else if (eventName == "OnPlayerTelephoneBoothEnd" && sender.HasData("Booth"))
            {
                TelephoneBooth booth = sender.GetData("Booth");

                if (booth.CurrentCall != null && booth.CurrentCall.CurrentlyTalking)
                {
                    API.shared.sendChatMessageToPlayer(booth.CurrentCall.Sender, "~#ffdb00~",
                        "Rozmowa zakończona.");
                    API.shared.sendChatMessageToPlayer(booth.CurrentCall.Getter, "~#ffdb00~",
                        "Rozmowa zakończona.");

                    booth.CurrentCall.Dispose();
                    booth.CurrentCall = null;
                }
            }
        }

        #region Komendy administracji
        [Command("dodajbudke")]
        public void CreateAtm(Client sender, string cost, string number)
        {
            sender.Notify("Ustaw się w wybranej pozycji, a następnie wpisz /tu.");
            sender.Notify("...użyj /diag aby poznać swoją obecną pozycję.");

            if (!Validator.IsMoneyStringValid(cost) || !Validator.IsCellphoneNumberValid(number))
            {
                sender.Notify("Wprowadzono dane w nieprawidłowym formacie.");
            }

            API.onChatCommand += Handler;

            void Handler(Client o, string command, CancelEventArgs cancel)
            {
                if (o == sender && command == "/tu")
                {
                    TelephoneBoothModel booth = new TelephoneBoothModel
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
                        },
                        Cost = int.Parse(cost),
                        Number = int.Parse(number)
                    };

                    XmlHelper.AddXmlObject(booth, Constant.ConstantAssemblyInfo.XmlDirectory + @"Booths\");
                    new TelephoneBooth(API, booth);
                    sender.Notify("Dodawanie budki zakończyło się pomyślnie.");
                    API.onChatCommand -= Handler;
                }
            }
        }
        #endregion
    }
}
