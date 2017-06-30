using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.CrimeBot.Models;
using Serverside.Database;
using Serverside.Groups;
using Serverside.Groups.Base;
using Vehicle = GTANetworkServer.Vehicle;

namespace Serverside.CrimeBot
{
    public class RPCrimeBot : Script
    {
        public RPCrimeBot()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        [Command("dodajbotp")]
        public void AddCrimeBot(Client sender, string name)
        {
            sender.Notify("Ustaw się w wybranej pozycji, a następnie wpisz /tu.");
            sender.Notify("...użyj /diag aby poznać swoją obecną pozycję.");

            FullPosition botPosition = null;
            Vehicle botVehicle = API.shared.createVehicle(VehicleHash.Sentinel,
                        new Vector3(sender.position.X + 2, sender.position.Y + 2, sender.position.Z),
                sender.rotation, 0, 0, 2137);

            API.onChatCommand += Handler;

            void Handler(Client o, string command, CancelEventArgs cancel)
            {
                if (o == sender && command == "/tu" && botPosition == null)
                {
                    botPosition = new FullPosition
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
                    };

                    API.shared.triggerClientEvent(sender, "DrawAddingCrimeBotComponents", botPosition.Position);
                    sender.Notify("Ustaw pojazd w wybranej pozycji następnie wpisz /tu.");
                    botVehicle.dimension = 0;
                    API.shared.setPlayerIntoVehicle(sender, botVehicle, -1);

                }
                else if (o == sender && command == "/tu" && botPosition != null)
                {
                    var botVehiclePosition = new FullPosition
                    {
                        Position = new Vector3
                        {
                            X = botVehicle.position.X,
                            Y = botVehicle.position.Y,
                            Z = botVehicle.position.Z
                        },

                        Rotation = new Vector3
                        {
                            X = botVehicle.rotation.X,
                            Y = botVehicle.rotation.Y,
                            Z = botVehicle.rotation.Z
                        }
                    };

                    API.shared.triggerClientEvent(sender, "DisposeAddingCrimeBotComponents");

                    XmlHelper.AddXmlObject(new CrimeBotPosition
                    {
                        Name = name,
                        BotPosition = botPosition,
                        VehiclePosition = botVehiclePosition
                    }, Constant.ConstantAssemblyInfo.XmlDirectory + @"CrimeBotPositions\");

                    sender.Notify("Dodawanie bota zakończyło się pomyślnie!");
                    API.shared.warpPlayerOutOfVehicle(sender);
                    API.shared.deleteEntity(botVehicle);
                    API.onChatCommand -= Handler;
                }
            };
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            //args[0] to index na liście pozycji
            if (eventName == "OnPlayerSelectedCrimeBotDiscrict")
            {
                var player = sender.GetAccountController();
                if (player.CharacterController.OnDutyGroup is CrimeGroup)
                {
                    var group = (CrimeGroup)player.CharacterController.OnDutyGroup;
                    if (group.CrimeBot != null)
                    {
                        sender.sendChatMessage("~#ffdb00~",
                            "Wybrany abonent ma wyłączony telefon, bądź znajduje się poza zasięgiem, spróbuj później.");
                        return;
                    }

                    var crimeBotData = ContextFactory.Instance.CrimeBots.Single(b => b.Group.Id == group.Id);
                    var position = XmlHelper.GetXmlObjects<CrimeBotPosition>(@"CrimeBotPositions\")[(int)arguments[0]];

                    group.CrimeBot = new CrimeBot(player, group, position.VehiclePosition, API, crimeBotData.Name, crimeBotData.Model, position.BotPosition);
                    group.CrimeBot.Intialize();
                    API.triggerClientEvent(sender, "DrawCrimeBotComponents", position.BotPosition.Position, 500, 2);
                }
                else
                {
                    sender.Notify("Aby wezwać sprzedawcę musisz znajdować się na służbie grupy.");
                }
            }
        }
    }
}