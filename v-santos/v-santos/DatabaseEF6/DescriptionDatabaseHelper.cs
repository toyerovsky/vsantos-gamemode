﻿using Serverside.DatabaseEF6.Models;
using System.Collections.Generic;
using System.Linq;

namespace Serverside.DatabaseEF6
{
    public class DescriptionDatabaseHelper
    {
        public static List<Description> SelectDescriptionsList(Character cid)
        {
            return ContextFactory.Instance.Descriptions.Where(x => x.Character.CharacterId == cid.CharacterId).ToList();
        }

        public static Description SelectDescription(long did)
        {
            return ContextFactory.Instance.Descriptions.Where(x => x.DescriptionId == did).FirstOrDefault();
        }

        public static void AddDescription(Description description)
        {
            ContextFactory.Instance.Descriptions.Add(description);
            ContextFactory.Instance.SaveChanges();
        }

        public static void UpdateDescription(Description description)
        {
            ContextFactory.Instance.Descriptions.Attach(description);
            ContextFactory.Instance.Entry(description).State = System.Data.Entity.EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }

        public static void DeleteDescription(long did)
        {
            Description delobj = ContextFactory.Instance.Descriptions.Where(x => x.DescriptionId == did).FirstOrDefault();
            ContextFactory.Instance.Descriptions.Remove(delobj);
            ContextFactory.Instance.SaveChanges();
        }
    }
}
