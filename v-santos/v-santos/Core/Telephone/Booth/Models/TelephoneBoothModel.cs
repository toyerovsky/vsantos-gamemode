using System;

namespace Serverside.Core.Telephone.Booth.Models
{
    [Serializable]
    public class TelephoneBoothModel
    {
        public int Number { get; set; }
        public decimal Cost { get; set; }
        public FullPosition Position { get; set; }
        public string CreatorForumName { get; set; }
    }
}