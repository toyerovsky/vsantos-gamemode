using Serverside.DatabaseEF6.Models;
using System.Collections.Generic;
using System.Linq;

namespace Serverside.DatabaseEF6
{
    public class DescriptionDatabaseHelper
    {
        public List<Description> SelectDescriptionsList(long cid)
        {
            return RoleplayConnection.Instance.Descriptions.Where(x => x.CID == cid).ToList();
        }

        public Description SelectDescription(long did)
        {
            return RoleplayConnection.Instance.Descriptions.Where(x => x.DID == did).FirstOrDefault();
        }

        public void AddDescription(Description description)
        {
            RoleplayConnection.Instance.Descriptions.Add(description);
            RoleplayConnection.Instance.SaveChanges();
        }

        public void UpdateDescription(Description description)
        {
            RoleplayConnection.Instance.Descriptions.Attach(description);
            RoleplayConnection.Instance.Entry(description).State = System.Data.Entity.EntityState.Modified;
            RoleplayConnection.Instance.SaveChanges();
        }

        public void DeleteDescription(long UID)
        {
            Description delobj = RoleplayConnection.Instance.Descriptions.Where(x => x.CID == UID).FirstOrDefault();
            RoleplayConnection.Instance.Descriptions.Remove(delobj);
            RoleplayConnection.Instance.SaveChanges();
        }
    }
}
