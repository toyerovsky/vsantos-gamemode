using System;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using Serverside.Core;
using Serverside.Core.Extensions;


namespace Serverside.Employer
{
    public class RPEmployer : Script
    {
        private FullPosition EmployerPosition => new FullPosition(new Vector3(1750f, -1580f, 113f), new Vector3(1f, 1f, 1f));
        private EmployerBot Employer { get; }

        public RPEmployer()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onResourceStart += API_onResourceStart;
            Employer = new EmployerBot(API, "Pracodawca", PedHash.Business01AMM, EmployerPosition);
            Employer.Intialize();
        }
        
        private void API_onResourceStart()
        {
            API.consoleOutput("RPEmployer zostało uruchomione pomyślnie!");
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            //args[0] to numerek pracy
            //0 Śmieciarz
            //1 Ogrodnik
            //2 Złodziej
            //3 Magazynier
            if (eventName == "OnPlayerSelectedJob")
            {
                var player = sender.GetAccountController();
                player.CharacterController.Character.Job = Convert.ToInt32(arguments[0]);
                player.Save();

                switch (Convert.ToInt32(arguments[0]))
                {
                    case 0:
                        sender.Notify("Podjąłeś się pracy: Operator śmieciarki. Udaj się na wysypisko i wsiądź do śmieciarki.");
                        break;
                    case 1:
                        sender.Notify("Podjąłeś się pracy: Ogrodnik. Udaj się na pole golfowe i wsiądź do kosiarki.");
                        break;
                    case 2:
                        sender.Notify("Podjąłeś się pracy: Złodziej. Udaj się do portu i wsiądź do jednej z ciężarówek.");
                        break;
                    case 3:
                        sender.Notify("Podjąłeś się pracy: Magazynier. Udaj się do magazynu, jest on oznaczony na mAPIe ikoną TU WPISAC.");
                        break;
                }
            }
            else if (eventName == "OnPlayerTakeMoneyJob")
            {
                var player = sender.GetAccountController();
                if (player.CharacterController.Character.MoneyJob != null)
                {
                    sender.AddMoney((decimal)player.CharacterController.Character.MoneyJob);
                    player.CharacterController.Character.MoneyJob = 0;
                    player.CharacterController.Save();
                }
            }
        }
    }
}