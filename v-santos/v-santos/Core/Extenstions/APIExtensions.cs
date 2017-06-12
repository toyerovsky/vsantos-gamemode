using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using GTANetworkServer.Constant;
using Serverside.Core.Extenstions;

namespace Serverside.Core.Extensions
{
    public static class APIExtensions
    {
        public static List<Client> GetNearestPlayers(this Vector3 position)
        {
            //Stopwatch _debug = new Stopwatch();
            //_debug.Start();
            return API.shared.getAllPlayers().Select(n => n).OrderBy(n => new Vector3(n.position.X, n.position.Y, n.position.Z).DistanceTo(position)).ToList();
            //API.shared.sendNotificationToPlayer(player, _debug.ElapsedTicks.ToString(), true); // DEBUG
            //_debug.Reset();
        }

        public static Client GetNearestPlayer(this Vector3 position)
        {
            return position.GetNearestPlayers()[0];
        }

        public static string ToHex(this Color c)
        {
            return "#" + c.red.ToString("X2") + c.green.ToString("X2") + c.blue.ToString("X2") + c.alpha.ToString("X2");
        }

        public static string ToRGB(this Color c)
        {
            return "RGB(" + c.red.ToString() + "," + c.green.ToString() + "," + c.blue.ToString() + ")";
        }

        public static string ToRGBA(this Color c)
        {
            return "RGBA(" + c.red.ToString() + "," + c.green.ToString() + "," + c.blue.ToString() + "," + c.alpha.ToString() + ")";
        }

        public static Color GetRandomColor()
        {
            Random r = new Random();
            return new Color(r.Next(256), r.Next(256), r.Next(256), r.Next(256));
        }

        public static Color ToColor(this string hex)
        {
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            if (hex.Length != 8) throw new Exception("Color not valid");

            return new Color(
                int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                int.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber));
        }

        public static void ConsoleOutput(string message, ConsoleColor color)
        {
            Console.BackgroundColor = color;
            API.shared.consoleOutput(message);
            Console.ResetColor();
        }

        public static string GetColoredString(string color, string text)
        {
            return "~" + color + "~" + text;
        }

        private static readonly Random getrandom = new Random();
        private static readonly object syncLock = new object();
        public static int GetRandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return getrandom.Next(min, max);
            }
        }

        public static void PopRandomsTyres(Vehicle vehicle, int howmany = 0)
        {
            Client player;
            if (vehicle.occupants.Count() != 0)
            {
                player = vehicle.occupants[0];
            }
            else
            {
                player = GetNearestPlayer(vehicle.position);
            }
            bool vehhastyres = API.fetchNativeFromPlayer<bool>(player, Hash.GET_VEHICLE_TYRES_CAN_BURST, vehicle.handle);

            if (vehhastyres)
            {
                VehicleClass vehClass = (VehicleClass)vehicle.Class;
                if (vehClass == VehicleClass.Motorcycles && vehClass == VehicleClass.Cycle)
                {
                    if (howmany > 2)
                        howmany = 2;

                    for (int i = 1; i <= howmany; i++)
                    {
                        //API.shared.sendChatMessageToAll(i.ToString());
                        //    int tyre = 6;
                        //    do
                        //    {
                        List<Wheel> wheels = new List<Wheel>();
                        wheels.Add(Wheel.BikeFront);
                        wheels.Add(Wheel.BikeRear);
                        int r = GetRandomNumber(0, 1);
                        if (API.shared.isVehicleTyrePopped(vehicle, (int)wheels[r]))
                        {
                            //i = i - 1;
                            continue;
                        }

                        //    } while (API.shared.isVehicleTyrePopped(vehicle, tyre));

                        API.shared.sendNativeToPlayer(player, Hash.SET_VEHICLE_TYRE_BURST, vehicle.handle, (int)wheels[r], true, 1000.0);
                        //API.shared.sendChatMessageToPlayer(player, "Popped: " + wheels[r].ToString());
                    }
                }
                else
                {
                    if (howmany > 5)
                        howmany = 5;

                    for (int i = 1; i <= howmany; i++)
                    {
                        //API.shared.sendChatMessageToAll("I: " + i.ToString());
                        //    int tyre = 0;
                        //    do
                        //    {
                        List<Wheel> wheels = new List<Wheel>();
                        wheels.Add(Wheel.FrontLeft);
                        wheels.Add(Wheel.FrontRight);
                        wheels.Add(Wheel.RearLeft);
                        wheels.Add(Wheel.RearRight);
                        int r = GetRandomNumber(0, 3);
                        //API.shared.sendChatMessageToPlayer(player, "Random Tyre: " + wheels[r].ToString());
                        bool popped = API.fetchNativeFromPlayer<bool>(player, Hash.IS_VEHICLE_TYRE_BURST, vehicle.handle, (int)wheels[r], true);
                        if (popped)
                        {
                            //API.shared.sendChatMessageToPlayer(player, "Is Popped: " + wheels[r].ToString());
                            //i = i - 1;
                            continue;
                        }
                        //API.shared.sendChatMessageToPlayer(player, "Not Popped: " + wheels[r].ToString());
                        //    } while (API.shared.isVehicleTyrePopped(vehicle, tyre));
                        API.shared.sendNativeToPlayer(player, Hash.SET_VEHICLE_TYRE_BURST, vehicle.handle, (int)wheels[r], true, 1000.0);
                        //API.shared.sendChatMessageToPlayer(player, "Popping: " + wheels[r].ToString());
                    } // ZA SKOMPLIKOWANE XD
                }
            }
        }
    }
}
