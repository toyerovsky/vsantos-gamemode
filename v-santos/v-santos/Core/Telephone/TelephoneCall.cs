using System;
using System.Linq;
using System.Timers;
using GTANetworkServer;
using Serverside.Core.Extensions;

namespace Serverside.Core.Telephone
{
    public class TelephoneCall : IDisposable
    {
        private readonly API _api = new API();

        public Client Sender { get; set; }
        public Client Getter { get; set; }

        public int BoothNumber { get; }

        public bool Accepted { get; set; }

        public Timer Timer { get; }

        private bool _currentlyTalking;
        public bool CurrentlyTalking
        {
            get => _currentlyTalking;
            set
            {
                _currentlyTalking = value;
                if (value)
                {
                    Sender.ResetSyncedData("CellphoneRinging");
                    Getter.ResetSyncedData("CellphoneRinging");
                    Sender.SetSyncedData("CellphoneTalking", true);
                    Getter.SetSyncedData("CellphoneTalking", true);
                }
                else
                {
                    Sender.ResetSyncedData("CellphoneTalking");
                    Getter.ResetSyncedData("CellphoneTalking");
                    Sender.ResetSyncedData("CellphoneRinging");
                    Getter.ResetSyncedData("CellphoneRinging");
                }
            }
        }

        //Połączenie z komórki
        public TelephoneCall(Client sender, Client getter)
        {
            //gracz który dokonuje połączenia
            Sender = sender;
            //Gracz który odbiera połączenie
            Getter = getter;

            Accepted = false;

            //W tym miejscu jeśli gracz nie odbierze telefonu w 10 sekund to przerywa rozmowę.
            Timer = new Timer(10000);
            Timer.Start();

            RPChat.OnPlayerSaid += RPChat_PlayerSaid;
            Getter.SetSyncedData("CellphoneRinging", true);
            RPChat.SendMessageToNearbyPlayers(Getter, $"dzwoni telefon {Getter.name}", ChatMessageType.Do);

            var senderController = Sender.GetAccountController().CharacterController.CellphoneController;
            var getterController = getter.GetAccountController().CharacterController.CellphoneController;
            var contacts = getterController.Contacts;

            _api.sendChatMessageToPlayer(Getter, "~#ffdb00~", contacts.Any(c => c.Number == senderController.Number) ? $"Połączenie przychodzące od: {contacts.First(c => c.Number == senderController.Number).Name}, naciśnij klawisz END aby, akceptować połączenie."
                : $"Połączenie przychodzące od: {senderController.Number}, naciśnij klawisz END aby, akceptować połączenie.");
        }

        //Połączenie z budki
        public TelephoneCall(Client sender, Client getter, int number)
        {
            //gracz który dokonuje połączenia
            Sender = sender;
            //Gracz który odbiera połączenie
            Getter = getter;

            BoothNumber = number;
            Accepted = false;

            Timer = new Timer(20000);
            Timer.Start();

            RPChat.OnPlayerSaid += RPChat_PlayerSaid;
            Getter.SetSyncedData("CellphoneRinging", true);
            RPChat.SendMessageToNearbyPlayers(Getter, $"dzwoni telefon {Getter.name}", ChatMessageType.Do);

            var telephone = Getter.GetAccountController().CharacterController.CellphoneController;
            var contacts = telephone.Contacts;

            _api.sendChatMessageToPlayer(Getter, "~#ffdb00~", contacts.Any(c => c.Number == number) ? $"Połączenie przychodzące od: {contacts.First(c => c.Number == number).Name}, naciśnij klawisz END aby, akceptować połączenie."
                : $"Połączenie przychodzące od: {number}, naciśnij klawisz END aby, akceptować połączenie.");

        }

        public void Open()
        {
            Accepted = true;
            CurrentlyTalking = true;
            Timer.Stop();
        }

        public void Dispose()
        {
            Accepted = false;
            CurrentlyTalking = false;

            RPChat.OnPlayerSaid -= RPChat_PlayerSaid;
            Timer.Dispose();

            API.shared.stopPlayerAnimation(Sender);
            API.shared.stopPlayerAnimation(Getter);
        }

        private void RPChat_PlayerSaid(object s, SaidEventArgs e)
        {
            if (Accepted && e.Player == Sender)
            {
                RPChat.SendMessageToPlayer(Getter, e.Message, ChatMessageType.Phone);
            }
            else if (Accepted && e.Player == Getter)
            {
                RPChat.SendMessageToPlayer(Sender, e.Message, ChatMessageType.Phone);
            }
        }
    }
}