using System;

namespace Serverside.Database
{
    [Serializable]
    public class DescriptionList
    {
        public long DID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    [Serializable]
    public class DescriptionEditor
    {
        public long DID { get; set; }
        public long CID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}