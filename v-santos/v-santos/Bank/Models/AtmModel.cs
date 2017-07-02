using System;
using Serverside.Core;
using Serverside.Interfaces;

namespace Serverside.Bank.Models
{
    [Serializable]
    public class AtmModel : IXmlObject
    {
        public FullPosition Position { get; set; }
        public string CreatorForumName { get; set; }
        public string FilePath { get; set; }
    }
}