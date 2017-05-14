using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Serverside.DatabaseEF6.Models
{
    public class CrimeBot
    {
        [Key]
        public long BotId { get; set; }
        public Group Group { get; set; }
        public string Name { get; set; }

        //public Dictionary<string, decimal?> GunsCost { get; set; }
        //public Dictionary<string, int?> GunsCount { get; set; }
        //public Dictionary<string, int?> GunsDefaultCount { get; set; }

        //public Dictionary<string, decimal?> AmmoCost { get; set; }
        //public Dictionary<string, int?> AmmoCount { get; set; }
        //public Dictionary<string, int?> AmmoDefaultCount { get; set; }

        //public Dictionary<string, decimal?> DrugsCost { get; set; }
        //public Dictionary<string, int?> DrugsCount { get; set; }
        //public Dictionary<string, int?> DrugsDefaultCount { get; set; }
    }
}