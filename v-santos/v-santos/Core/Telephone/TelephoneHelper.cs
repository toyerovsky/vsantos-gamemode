using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
//using Serverside.Bank;
//using Serverside.Constant;
using Serverside.Core.Telephone.Booth;
using Serverside.Database;
using Serverside.Items;

namespace Serverside.Core.Telephone
{
    public static class TelephoneHelper
    {
        public static int GetNextFreeTelephoneNumber()
        {
            var numbers = ContextFactory.Instance.Items.Where(i => i.ItemType == (int)ItemType.Cellphone);

            Random r = new Random();
            int number = r.Next(100000000);

            while (numbers.Any(t => t.ThirdParameter == number))
            {
                number = r.Next(100000000);
            }
            return number;
        }

        public static bool AddTelephoneBooth(TelephoneBooth atm)
        {
            //using (FileStream xmlStream = File.Create(ConstantAssemblyInfo.Instance.XmlDirectory + @"\Booths\" + (GetTelephoneBooths().Count + 1) + ".xml"))
            //{
            //    try
            //    {
            //        var writerSerializer = new XmlSerializer(typeof(Atm));
            //        writerSerializer.Serialize(xmlStream, atm);
            //        return true;
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception(ex.Message);
            //    }
            //    finally
            //    {
            //        xmlStream.Close();
            //    }
            //}
            return false;
        }

        public static List<TelephoneBooth> GetTelephoneBooths()
        {
            //var readerSerializer = new XmlSerializer(typeof(TelephoneBooth));
            //var extension = ".xml";
            //var posDirs = Directory.GetFiles(ConstantAssemblyInfo.Instance.XmlDirectory + @"\Booths\");

            var xmlPositions = new List<TelephoneBooth>();
            //Dla kazdej sciezki dodaj odpowiadajacy jej .xml do listy
            //foreach (var item in posDirs)
            //{
            //    var stream = new FileStream(item, FileMode.Open);
            //    if (Path.GetExtension(item) == extension)
            //    {
            //        TelephoneBooth container = readerSerializer.Deserialize(stream) as TelephoneBooth;
            //        xmlPositions.Add(container);
            //    }
            //    stream.Close();
            //    stream.Dispose();
            //}
            return xmlPositions;
        }
    }
}