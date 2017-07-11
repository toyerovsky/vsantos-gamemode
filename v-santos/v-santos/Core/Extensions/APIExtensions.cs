using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;


using Serverside.Admin;
using Serverside.Admin.Enums;

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

        public static string ToGameColor(this Color c) => $"~{c.ToHex()}~";

        public static string ToHex(this Color c)
        {
            if (c.alpha == 0)
                return $"#{c.red:X2}{c.green:X2}{c.blue:X2}";

            return $"#{c.red:X2}{c.green:X2}{c.blue:X2}{c.alpha:X2}";
        }

        public static string ToRGB(this Color c) => $"RGB({c.red},{c.green},{c.blue})";

        public static string ToRGBA(this Color c) => $"RGBA({c.red},{c.green},{c.blue},{c.alpha})";

        public static Color GetRankColor(ServerRank group)
        {
            if (group >= ServerRank.Support && group <= ServerRank.Support6) return new Color(51, 143, 255);
            if (group >= ServerRank.GameMaster && group <= ServerRank.GameMaster5) return new Color(0, 109, 15);
            if (group >= ServerRank.Administrator && group <= ServerRank.Adminadministrator3) new Color(117, 13, 18);
            if (group >= ServerRank.Zarzad && group <= ServerRank.Zarzad2) return new Color(255, 0, 0);
            return new Color(255, 255, 255);
        }

        private static Dictionary<string, string> RocstarColors = new Dictionary<string, string>()
        {
            {"~r~", "DE3232"},
            {"~g~", "71CA71"},
            {"~b~", "5CB4EB"},
            {"~y~", "EEC650"},
            {"~p~", "AD65E5"},
            {"~q~", "EB4F80"},
            {"~o~", "FE8455"},
            {"~m~", "636378"},
            {"~u~", "252525"}
        };

        public static string ToRocstar(this Color color)
        {
            foreach (var c in RocstarColors)
            {
                //ConsoleOutput(c.Key.Substring(1, 2).ToString(), ConsoleColor.Red);
                //ConsoleOutput((Convert.ToInt32(c.Key.Substring(1, 2)) < 20).ToString(), ConsoleColor.Green);
                //ConsoleOutput((Math.Abs(color.red - Convert.ToInt32(c.Key.Substring(1, 2))) < 20).ToString(), ConsoleColor.Blue);
                if (Math.Abs(color.red - int.Parse(c.Value.Substring(0, 2), System.Globalization.NumberStyles.HexNumber)) < 20 ||
                    Math.Abs(color.green - int.Parse(c.Value.Substring(2, 2), System.Globalization.NumberStyles.HexNumber)) < 20 ||
                    Math.Abs(color.red - int.Parse(c.Value.Substring(4, 2), System.Globalization.NumberStyles.HexNumber)) < 20)
                    return c.Key;
            }
            return "~w~";
        }

        public static Color GetRandomColor(this Color color)
        {
            Random r = new Random();
            return new Color(r.Next(256), r.Next(256), r.Next(256), r.Next(256));
        }

        public static Color ToColor(this string hex)
        {
            if (hex == null) return new Color(255, 255, 255);
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            if (hex.Length != 6 || hex.Length != 8) return new Color(255, 0, 0);

            if (hex.Length == 6)
            {
                return new Color(
                    int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                    int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                    int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
            }
            else
            {
                return new Color(
                    int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                    int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                    int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                    int.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber));
            }

        }

        public static void ConsoleOutput(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            API.shared.consoleOutput(message);
            Console.ResetColor();
        }

        public static string GetColoredString(string color, string text) => $"~{color}~{text}";

        private static readonly Random GetRandom = new Random();
        private static readonly object syncLock = new object();
        public static int GetRandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return GetRandom.Next(min, max);
            }
        }

        public static void PopRandomsTyres(Vehicle vehicle, int howmany = 0)
        {
            Client player;
            if (vehicle.occupants.Length != 0)
            {
                player = vehicle.occupants[0];
            }
            else
            {
                player = GetNearestPlayer(vehicle.position);
            }
            bool vehhastyres = API.shared.fetchNativeFromPlayer<bool>(player, Hash.GET_VEHICLE_TYRES_CAN_BURST, vehicle.handle);

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
                        List<Wheel> wheels = new List<Wheel> {Wheel.BikeFront, Wheel.BikeRear};
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
                        List<Wheel> wheels = new List<Wheel> { Wheel.FrontLeft, Wheel.FrontRight, Wheel.RearLeft, Wheel.RearRight };
                        int r = GetRandomNumber(0, 3);
                        //API.shared.sendChatMessageToPlayer(player, "Random Tyre: " + wheels[r].ToString());
                        bool popped = API.shared.fetchNativeFromPlayer<bool>(player, Hash.IS_VEHICLE_TYRE_BURST, vehicle.handle, (int)wheels[r], true);
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
