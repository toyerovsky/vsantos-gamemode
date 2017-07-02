using System;
using Serverside.Interfaces;

namespace Serverside.Core.Animations.Models
{
    [Serializable]
    public class Animation : IXmlObject
    {
        public string Name { get; set; }
        public string AnimDictionary { get; set; }
        public string AnimName { get; set; }
        public string FilePath { get; set; }
        public string CreatorForumName { get; set; }
    }
}