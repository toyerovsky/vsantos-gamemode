using System;
using Serverside.Interfaces;

namespace Serverside.Core.Telephone.Booth.Models
{
    [Serializable]
    public class TelephoneBoothModel : IXmlObject
    {
        public int Number { get; set; }
        public decimal Cost { get; set; }
        public FullPosition Position { get; set; }
        public string CreatorForumName { get; set; }
        public string FilePath { get; set; }
    }
}