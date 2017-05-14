using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Serverside.Items
{
    public class ServerItem
    {
        public string Name { get; set; }
        public ItemType ItemType { get; set; }
        public decimal Cost { get; set; }
        public int Weight { get; set; }

        public static List<ServerItem> LoadItemsFromPath(string path)
        {
            XmlSerializer readerSerializer = new XmlSerializer(typeof(ServerItem));
            string extension = ".xml";
            var itemDirs = Directory.GetFiles(path);

            var xmlItems = new List<ServerItem>();
            //Dla kazdej sciezki dodaj odpowiadajacy jej .xml do listy
            foreach (var item in itemDirs)
            {
                var stream = new FileStream(item, FileMode.Open);
                if (Path.GetExtension(item) == extension)
                {
                    var container = readerSerializer.Deserialize(stream) as ServerItem;
                    xmlItems.Add(container);
                }
                stream.Close();
            }
            return xmlItems;
        }
    }
}
