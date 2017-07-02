using System;
using System.Linq;
using GTANetworkServer;
using GTANetworkServer.Constant;
using GTANetworkShared;
using Serverside.Controllers;
using Serverside.Core;
using Serverside.Core.Extensions;
using Serverside.Corners.Models;
using Serverside.CrimeBot.Models;
using Serverside.Database;
using Serverside.Groups.Base;

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
            //if (sender.GetAccountController().AccountData.ServerRank < ServerRank.GameMaster)
            //{
            //    sender.Notify("Nie posiadasz uprawnień do tworzenia grupy.");
            //    return;
            //}

            sender.Notify("Ustaw się w wybranej pozycji, a następnie wpisz /tu.");
            sender.Notify("...użyj /diag aby poznać swoją obecną pozycję.");

            FullPosition botPosition = null;
            VehicleController botVehicle = null;

            API.onChatCommand += Handler;

            void Handler(Client o, string command, CancelEventArgs cancel)
            {
                if (o == sender && command == "/tu" && botPosition == null)
                {
                    cancel.Cancel = true;
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
                        },

                        Direction = new Vector3(0f, 0f, 0f)
                    };

                    API.shared.triggerClientEvent(sender, "DrawAddingCrimeBotComponents", new Vector3(botPosition.Position.X, botPosition.Position.Y, botPosition.Position.Z - 1));
                    sender.Notify("Ustaw pojazd w wybranej pozycji następnie wpisz /tu.");

                    botVehicle = new VehicleController(new FullPosition(new Vector3(sender.position.X + 2, sender.position.Y + 2, sender.position.Z), sender.rotation), VehicleHash.Sentinel, "Admin", 0, 0, new Color(0, 0, 0), new Color(0, 0, 0));

                    API.shared.setPlayerIntoVehicle(sender, botVehicle.Vehicle, -1);

                }
                else if (o == sender && command == "/tu" && botPosition != null && botVehicle != null)
                {
                    cancel.Cancel = true;

                    var botVehiclePosition = new FullPosition
                    {
                        Position = new Vector3
                        {
                            X = botVehicle.Vehicle.position.X,
                            Y = botVehicle.Vehicle.position.Y,
                            Z = botVehicle.Vehicle.position.Z
                        },

                        Rotation = new Vector3
                        {
                            X = botVehicle.Vehicle.rotation.X,
                            Y = botVehicle.Vehicle.rotation.Y,
                            Z = botVehicle.Vehicle.rotation.Z
                        },

                        Direction = new Vector3(0f, 0f, 0f)
                    };

                    API.shared.triggerClientEvent(sender, "DisposeAddingCrimeBotComponents");

                    XmlHelper.AddXmlObject(new CrimeBotPosition
                    {
                        CreatorForumName = o.GetAccountController().AccountData.Name,
                        Name = name,
                        BotPosition = botPosition,
                        VehiclePosition = botVehiclePosition
                    }, $@"{Constant.ConstantAssemblyInfo.XmlDirectory}CrimeBotPositions\");

                    sender.Notify("Dodawanie pozycji bota zakończyło się pomyślnie!");
                    API.shared.warpPlayerOutOfVehicle(sender);
                    botVehicle.Dispose();
                    API.onChatCommand -= Handler;
                }
            };
        }

        [Command("usunbotp", "~y~ UŻYJ ~w~ /usunbotp (nazwa)", GreedyArg = true)]
        public void DeleteCrimeBotPosition(Client sender, string name = "")
        {
            CrimeBotPosition position = null;
            var positions = XmlHelper
                .GetXmlObjects<CrimeBotPosition>($@"{Constant.ConstantAssemblyInfo.XmlDirectory}CrimeBotPositions\");

            if (name != "")
            {
                position = positions.OrderBy(a => a.BotPosition.Position.DistanceTo(sender.position)).First();
            }
            else
            {
                if (positions.Any(f => f.Name == name))
                {
                    position = positions.First(x => x.Name == name);
                }
            }
            
            if (position != null && XmlHelper.TryDeleteXmlObject(position.FilePath))
            {
                sender.Notify("Usuwanie pozycji bota zakończyło się pomyślnie.");
            }
            else
            {
                sender.Notify("Usuwanie pozycji bota zakończyło się niepomyślnie.");
            }
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
                    var position = XmlHelper.GetXmlObjects<CrimeBotPosition>($@"{Constant.ConstantAssemblyInfo.XmlDirectory}CrimeBotPositions\")[Convert.ToInt32(arguments[0])];

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