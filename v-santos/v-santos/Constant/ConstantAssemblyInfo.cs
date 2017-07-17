/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using System.IO;
using System.Reflection;
using GrandTheftMultiplayer.Server.API;

namespace Serverside.Constant
{
    public static class ConstantAssemblyInfo
    {
        public static string ListeningServer => API.shared.getSetting<string>("ListeningServer");
        
        public static int ListeningPort => API.shared.getSetting<int>("ListeningPort");

        public static string ListeningString => ListeningServer + ":" + ListeningPort + "/"; 
        
        public static string WebServerHost => API.shared.getSetting<string>("WebServerHost");

        public static int WebServerPort => API.shared.getSetting<int>("WebServerPort");
        
        public static string WebServerConnectionString => WebServerHost + ":" + WebServerPort + "/";

        public static string WorkingDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        /// <summary>
        /// Ścieżka z \ na końcu
        /// </summary>
        public static string XmlDirectory => WorkingDirectory + @"\Xml\";

        /// <summary>
        /// Ścieżka z \ na końcu
        /// </summary>
        public static string JsonDirectory => WorkingDirectory + @"\Json\";
    }
}
