using System.Collections.Generic;
using Serverside.Items;

namespace Serverside.Constant
{
    public static class ConstantItems
    {

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
    }
}