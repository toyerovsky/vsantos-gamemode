using Serverside.DatabaseEF6.Models;
using System.Collections.Generic;
using System.Linq;

namespace Serverside.DatabaseEF6
{
    public class DescriptionDatabaseHelper
    {
        public static List<Description> SelectDescriptionsList(Character cid)
        {
            return ContextFactory.Instance.Descriptions.Where(x => x.CharacterId == cid).ToList();
        }

        public static Description SelectDescription(long did)
        {
            return ContextFactory.Instance.Descriptions.Where(x => x.DID == did).FirstOrDefault();
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

        public static void DeleteDescription(Character UID)
        {
            Description delobj = ContextFactory.Instance.Descriptions.Where(x => x.CharacterId == UID).FirstOrDefault();
            ContextFactory.Instance.Descriptions.Remove(delobj);
            ContextFactory.Instance.SaveChanges();
        }
    }
}
