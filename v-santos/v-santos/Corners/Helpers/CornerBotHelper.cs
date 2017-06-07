using System;
using System.Collections.Generic;
using System.Linq;
using Serverside.Core;
using Serverside.Corners.Models;

namespace Serverside.Corners.Helpers
{
    public class CornerBotHelper
    {
        public static bool TryGetCornerBotIds(List<string> botIds, out List<int> correctBotIds)
        {
            correctBotIds = new List<int>();
            List<int> ids = XmlHelper.GetXmlObjects<CornerBotModel>(Constant.ConstantAssemblyInfo.XmlDirectory +
                                                              @"CornerBots\").Select(x => x.BotId).ToList();            
            foreach (var id in botIds)
            {
                var correctId = Convert.ToInt32(id);
                if (!ids.Contains(correctId))
                {
                    return false;
                }
                correctBotIds.Add(correctId);
            }
            return true;
        }
    }
}