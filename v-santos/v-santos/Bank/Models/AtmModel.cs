using System;
using Serverside.Core;

namespace Serverside.Bank.Models
{
    [Serializable]
    public class AtmModel
    {
        public FullPosition Position { get; set; }
        public string CreatorForumName { get; set; }
    }
}