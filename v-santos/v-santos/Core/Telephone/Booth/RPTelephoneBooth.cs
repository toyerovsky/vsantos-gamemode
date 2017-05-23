using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Core.Extensions;

namespace Serverside.Core.Telephone.Booth
{
    public class RPTelephoneBooth : Script
    {
        public static List<TelephoneCall> CurrentBoothCalls = new List<TelephoneCall>();

        private List<TelephoneBooth> booths;
        public List<TelephoneBooth> Booths
        {
            get { return booths; }
            set
            {
                booths = value;
                foreach (TelephoneBooth booth in value)
                {
                    booth.Intialize(API);
                }
            }
        }

        public RPTelephoneBooth()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onResourceStart += OnResourceStartHandler;
        }

        private void OnResourceStartHandler()
        {
            APIExtensions.ConsoleOutput("[RPTelephoneBooth] uruchomione pomyślnie.", ConsoleColor.DarkMagenta);
            Booths = TelephoneHelper.GetTelephoneBooths();
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] args)
        {
            //args[0] to numer na jaki dzwoni
            if (eventName == "OnPlayerTelephoneBoothCall")
            {
                var booth = Booths.First(b => b.Number == sender.getData("BoothNumber"));
                if (sender.HasMoney(booth.Cost))
                {
                    sender.RemoveMoney(booth.Cost);
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

                        TelephoneCall telephoneCall = new TelephoneCall(sender, getterPlayer, booth.Number);

                        telephoneCall.Timer.Elapsed += (o, eventArgs) =>
                        {
                            API.shared.sendChatMessageToPlayer(sender, "~#ffdb00~",
                                "Wybrany abonent ma wyłączony telefon, bądź znajduje się poza zasięgiem, spróbuj później.");
                            telephoneCall.Dispose();
                            CurrentBoothCalls.Remove(telephoneCall);
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
            else if (eventName == "OnPlayerTelephoneBoothEnd")
            {
                TelephoneCall telephoneCall = CurrentBoothCalls.First(s => s.Sender == sender || s.Getter == sender);

                if (telephoneCall != null && telephoneCall.CurrentlyTalking)
                {
                    telephoneCall.Dispose();

                    API.shared.sendChatMessageToPlayer(telephoneCall.Sender, "~#ffdb00~",
                        "Rozmowa zakończona.");
                    API.shared.sendChatMessageToPlayer(telephoneCall.Getter, "~#ffdb00~",
                        "Rozmowa zakończona.");
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

            API.onChatCommand += (o, command, cancel) =>
            {
                if (o == sender && command == "/tu")
                {
                    TelephoneBooth booth = new TelephoneBooth
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
                    TelephoneHelper.AddTelephoneBooth(booth);
                    booth.Intialize(API);
                    sender.Notify("Dodawanie budki zakończyło się pomyślnie.");
                }
            };
        }
        #endregion
    }
}
