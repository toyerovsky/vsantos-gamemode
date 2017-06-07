using System;

namespace Serverside.Core.Animations.Models
{
    [Serializable]
    public class Animation
    {
        public string Name { get; set; }
        public string AnimDictionary { get; set; }
        public string AnimName { get; set; }
    }
}