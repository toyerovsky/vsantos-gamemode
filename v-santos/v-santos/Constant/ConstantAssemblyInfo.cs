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

        public static string XmlDirectory => WorkingDirectory + @"\Xml\";
    }
}
