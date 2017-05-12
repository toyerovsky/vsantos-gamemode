using System.Collections.Generic;

namespace Serverside.Database
{
    public class CrimeBotEditor
    {
        public long BotId { get; set; }
        public long GroupId { get; set; }
        public string Name { get; set; }

        public Dictionary<string, decimal?> GunsCost { get; set; }
        public Dictionary<string, int?> GunsCount { get; set; }
        public Dictionary<string, int?> GunsDefaultCount { get; set; }

        public Dictionary<string, decimal?> AmmoCost { get; set; }
        public Dictionary<string, int?> AmmoCount { get; set; }
        public Dictionary<string, int?> AmmoDefaultCount { get; set; }

        public Dictionary<string, decimal?> DrugsCost { get; set; }
        public Dictionary<string, int?> DrugsCount { get; set; }
        public Dictionary<string, int?> DrugsDefaultCount { get; set; }
    }

    public class CrimeBotList
    {
        public Dictionary<string, decimal?> ItemsCost { get; set; }
        public Dictionary<string, int?> ItemsCount { get; set; }
    }
}
