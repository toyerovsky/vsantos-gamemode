using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Serverside.Core.Extensions;

namespace Serverside.Core
{
    public static class JsonHelper
    {
        public static List<T> GetJsonObjects<T>(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                APIExtensions.ConsoleOutput($"[JsonHelper] Utworzono ścieżkę {path}", ConsoleColor.Green);
            }

            return Directory.GetFiles(path).Select(JsonConvert.DeserializeObject<T>).ToList();
        }

        public static void AddJsonObject<T>(T value, string path, string fileName = "")
        {
            if (path.Last() != '\\')
            {
                APIExtensions.ConsoleOutput($"[JsonHelper] Podano nieprawidłową ścieżkę {path}", ConsoleColor.Green);
                return;
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                APIExtensions.ConsoleOutput($"[JsonHelper] Utworzono ścieżkę {path}", ConsoleColor.Green);
            }

            if (fileName == string.Empty)
            {
                var collection = GetJsonObjects<T>(path);
                int index = collection.Count;

                do ++index;
                while (File.Exists($"{path}{index}.json"));

                fileName = index.ToString();
            }

            var json = JsonConvert.SerializeObject(value);

            if (!File.Exists($"{path}{fileName}.json"))
                File.Create($"{path}{fileName}.json");

            File.AppendAllText($"{path}{fileName}.json", json);
            
        }
    }
}