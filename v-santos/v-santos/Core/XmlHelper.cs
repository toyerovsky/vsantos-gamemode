using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Serverside.Core.Extenstions;

namespace Serverside.Core
{
    public static class XmlHelper
    {
        /// <summary>
        /// Ścieżkę należy dostarczyć z \ na końcu
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlObject"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        public static void AddXmlObject<T>(T xmlObject, string path, string fileName = "")
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                APIExtensions.ConsoleOutput($"[XmlHelper] Utworzono ścieżkę {path}", ConsoleColor.Blue);
            }

            using (FileStream xmlStream = File.Create(path + (fileName != "" ? fileName : (GetXmlObjects<T>(path).Count + 1).ToString()) + ".xml"))
            {
                try
                {
                    var writerSerializer = new XmlSerializer(typeof(T));
                    writerSerializer.Serialize(xmlStream, xmlObject);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    xmlStream.Close();
                }
            }
        }

        /// <summary>
        /// Ścieżkę należy dostarczyć z \ na końcu
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<T> GetXmlObjects<T>(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                APIExtensions.ConsoleOutput($"[XmlHelper] Utworzono ścieżkę {path}", ConsoleColor.Blue);
            }

            var readerSerializer = new XmlSerializer(typeof(T));
            var posDirs = Directory.GetFiles(path);
            
            var xmlPositions = new List<T>();
            //Dla kazdej sciezki dodaj odpowiadajacy jej .xml do listy
            foreach (var item in posDirs)
            {
                var stream = new FileStream(item, FileMode.Open);
                if (Path.GetExtension(item) == ".xml")
                {
                    xmlPositions.Add((T)readerSerializer.Deserialize(stream));
                }
                stream.Close();
                stream.Dispose();
            }
            return xmlPositions;
        }
    }
}