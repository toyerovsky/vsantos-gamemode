using System;

namespace Serverside.Database
{
    [Serializable]
    public class GroupList
    {
        public long GID { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public int GroupType { get; set; }
    }

    [Serializable]
    public class GroupEditor
    {
        public long GID { get; set; }

        public string Name { get; set; }
        public string Tag { get; set; }

        public int Dotation { get; set; }
        public int MaxPayday { get; set; }
      
        public decimal Money { get; set; }

        public int GroupType { get; set; }

        //TODO Dodać do helpera
        public string Color { get; set; }
    }
}
