/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using System.IO;
using System.Linq;
using Serverside.Core.ServerInfo.Models;

namespace Serverside.Core.ServerInfo
{
    [Serializable]
    public class ServerInfo
    {
        public ServerInfoData Data { get; set; }

        private static ServerInfo _instance;
        public static ServerInfo Instance => _instance = _instance ?? new ServerInfo();

        public ServerInfo()
        {
            if (!File.Exists($"{Constant.ConstantAssemblyInfo.XmlDirectory}ServerInfo\\ServerInfo.xml"))
                XmlHelper.AddXmlObject(new ServerInfoData(), $"{Constant.ConstantAssemblyInfo.XmlDirectory}ServerInfo\\", "ServerInfo");

            Data = XmlHelper.GetXmlObjects<ServerInfoData>(
                $"{Constant.ConstantAssemblyInfo.XmlDirectory}ServerInfo\\").First();
        }

        public void Save()
        {
            
        }
    }
}