using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using Serverside.Core.Telephone.Booth.Models;
using Serverside.Core.Extensions;

namespace Serverside.Core.Telephone.Booth
{
    public class RPTelephoneBooth : Script
    {
        private List<TelephoneBooth> Booths { get; set; } = new List<TelephoneBooth>();

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
                Booths.Add(new TelephoneBooth(API, booth));
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
        public void CreateAtm(Client sender, decimal cost, string number)
        {
            sender.Notify("Ustaw się w wybranej pozycji, a następnie wpisz /tu.");
            sender.Notify("...użyj /diag aby poznać swoją obecną pozycję.");

            if (!Validator.IsMoneyValid(cost) || !Validator.IsCellphoneNumberValid(number))
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
                        CreatorForumName = o.GetAccountController().AccountData.Name,
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
                        Cost = cost,
                        Number = int.Parse(number)
                    };

                    XmlHelper.AddXmlObject(booth, Constant.ConstantAssemblyInfo.XmlDirectory + @"Booths\");
                    Booths.Add(new TelephoneBooth(API, booth));
                    sender.Notify("Dodawanie budki zakończyło się pomyślnie.");
                    API.onChatCommand -= Handler;
                }
            }
        }

        [Command("usunbudke")]
        public void DeleteBusStop(Client sender)
        {
            //if (sender.GetAccountController().AccountData.ServerRank < ServerRank.GameMaster)
            //{
            //    sender.Notify("Nie posiadasz uprawnień do usuwania bankomatu.");
            //    return;
            //}

            var telephoneBooth = Booths.OrderBy(a => a.Data.Position.Position.DistanceTo(sender.position)).First();
            if (XmlHelper.TryDeleteXmlObject(telephoneBooth.Data.FilePath))
            {
                sender.Notify("Usuwanie budki telefonicznej zakończyło się pomyślnie.");
                Booths.Remove(telephoneBooth);
                telephoneBooth.Dispose();
            }
            else
            {
                sender.Notify("Usuwanie budki telefonicznej zakończyło się niepomyślnie.");
            }          
        }
        #endregion
    }
}
