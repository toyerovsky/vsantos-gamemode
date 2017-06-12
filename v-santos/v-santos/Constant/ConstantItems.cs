using System.Collections.Generic;
using System.IO;
using System.Linq;
using GTANetworkShared;
using Serverside.Items;

namespace Serverside.Constant
{
    public static class ConstantItems
    {

        /// <summary>
        /// Klucz to słownik, wartosc to animacja
        /// </summary>
        public static Dictionary<string, string> Animations
        {
            get
            {
                return File.ReadLines(ConstantAssemblyInfo.WorkingDirectory + @"\Files\dict.txt").Select(x => x.Split(',')).ToDictionary(s => s[0], s => s[1]);
            }
        }



        public static Dictionary<int, int> GunDefaultAmmo = new Dictionary<int, int>
        {
            {-1045183535, 12},
            {-1074790547, 60},
            {-1076751822, 12},
            {-1716589765, 18},
            {-2084633992, 30},
            {-275439685, 4},
            {-619010992, 54},
            {-771403250, 14},
            {100416529, 10},
            {137902532, 20},
            {1593441988, 18},
            {1649403952, 60},
            {2017895192, 16},
            {324215364, 16},
            {453432689, 18},
            {487013001, 16},
            {-1121678507, 20}
        };

        public static Dictionary<int, string> GunNames = new Dictionary<int, string>
        {
            {-1045183535, "Colt Anaconda"},
            {-1074790547, "AK-47"},
            {-1076751822, "Colt Junior 25"},
            {-1716589765, "Colt M45A1"},
            {-2084633992, "Colt M4"},
            {-275439685, "AYA 12g"},
            {-619010992, "TEC-9"},
            {-771403250, "Desert Eagle"},
            {100416529, "Barrett M98B"},
            {137902532, "FN 1922"},
            {1593441988, "Walther P99"},
            {1649403952, "AKMSU"},
            {2017895192, "Mossberg 500"},
            {324215364, "Mini-UZI"},
            {453432689, "Colt M1911"},
            {487013001, "Mossberg 590"},
            {-1121678507, "Skorpion vz 61"}
        };

        public static List<ServerItem> Items
        {
            get { return new List<ServerItem>(); }
        }

        public static Dictionary<string, Vector3> DefaultInteriors = new Dictionary<string, Vector3>()
        {
            {"Garaż 2 auta", new Vector3(173.2903, -1003.6, -99.65707)},
            {"Garaż 6 aut",  new Vector3(197.8153, -1002.293, -99.65749)},
            {"Garaż 10 aut", new Vector3(229.9559, -981.7928, -99.66071)},
            {"Tanie mieszkanie", new Vector3(261.4586, -998.8196, -99.00863)},
            {"4 Integrity Way Apt 28", new Vector3(-18.07856, -583.6725, 79.46569)},
            {"4 Integrity Way, Apt 30", new Vector3(-35.31277, -580.4199, 88.71221)},
            {"Dell Perro Heights, Apt 4", new Vector3(-1468.14, -541.815, 73.4442)},
            {"Dell Perro Heights Apt 7", new Vector3(-1477.14, -538.7499, 55.5264)},
            {"Richard Majestic, Apt 2", new Vector3(-915.811, -379.432, 113.6748)},
            {"Tinsel Towers, Apt 42", new Vector3(-614.86, 40.6783, 97.60007)},
            {"EclipseTowers, Apt 3", new Vector3(-773.407, 341.766, 211.397)},
            {"3655 Wild Oats Drive", new Vector3(-169.286, 486.4938, 137.4436)},
            {"2044 North Conker Avenue", new Vector3(340.9412, 437.1798, 149.3925)},
            {"2045 North Conker Avenue", new Vector3(373.023, 416.105, 145.7006)},
            {"2862 Hillcrest Avenue", new Vector3(-676.127, 588.612, 145.1698)},
            {"2868 Hillcrest Avenue", new Vector3(-763.107, 615.906, 144.1401)},
            {"2874 Hillcrest Avenue", new Vector3(-857.798, 682.563, 152.6529)},
            {"2677 Whispymound Drive", new Vector3(120.5, 549.952, 184.097)},
            {"2133 Mad Wayne Thunder", new Vector3(-1288, 440.748, 97.69459) },
            {"CharCreator", new Vector3(402.5164, -1002.847, -99.2587)},
            {"Mission Carpark", new Vector3(405.9228, -954.1149, -99.6627)},
            {"Torture Room", new Vector3(136.5146, -2203.149, 7.30914)},
            {"Solomon's Office", new Vector3(-1005.84, -478.92, 50.02733)},
            {"Psychiatrist's Office", new Vector3(-1908.024, -573.4244, 19.09722)},
            {"Omega's Garage", new Vector3(2331.344, 2574.073, 46.68137)},
            {"Movie Theatre", new Vector3(-1427.299, -245.1012, 16.8039)},
            {"Motel", new Vector3(152.2605, -1004.471, -98.99999)},
            {"Mandrazos Ranch", new Vector3(152.2605, 1146.954, 114.337)},
            {"Life Invader Office", new Vector3(-1044.193, -236.9535, 37.96496)},
            {"Lester's House", new Vector3(1273.9, -1719.305, 54.77141)},
            {"FBI Top Floor", new Vector3(134.5835, -749.339, 258.152)},
            {"FBI Floor 47", new Vector3(134.5835, -766.486, 234.152)},
            {"FBI Floor 49", new Vector3(134.635, -765.831, 242.152)},
            {"IAA Office", new Vector3(117.22, -620.938, 206.1398)},
        };
    }
}