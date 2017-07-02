using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Serverside.Core.Extensions;
using Serverside.Interfaces;

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
        public static void AddXmlObject<T>(T xmlObject, string path, string fileName = "") where T : IXmlObject
        {
            if (path.Last() != '\\')
            {
                APIExtensions.ConsoleOutput($"[XmlHelper] Podano nieprawidłową ścieżkę {path}", ConsoleColor.Blue);
                return;
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                APIExtensions.ConsoleOutput($"[XmlHelper] Utworzono ścieżkę {path}", ConsoleColor.Blue);
            }

            if (fileName == string.Empty)
            {
                var collection = GetXmlObjects<T>(path);
                int index = collection.Count;

                do ++index;
                while (File.Exists($"{path}{index}.xml"));

                fileName = index.ToString();
            }


            using (FileStream xmlStream = File.Create($"{path}{fileName}.xml"))
            {
                try
                {
                    xmlObject.FilePath = $"{path}{fileName}.xml";
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
        public static List<T> GetXmlObjects<T>(string path) where T : IXmlObject
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

        public static bool TryDeleteXmlObject(string filePath)
        {
            if (!File.Exists(filePath))
            {
                APIExtensions.ConsoleOutput($"[XmlHelper] Próbowano usunąć plik który nie istnieje {filePath}", ConsoleColor.Blue);
                return false;
            }

            File.Delete(filePath);
            return true;
        }
    }
}