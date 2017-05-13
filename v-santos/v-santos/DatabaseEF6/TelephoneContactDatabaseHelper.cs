using Serverside.DatabaseEF6.Models;
using System.Collections.Generic;
using System.Linq;

namespace Serverside.DatabaseEF6
{
    public class TelephoneContactDatabaseHelper
    {
        public List<TelephoneContact> SelectContactsList(long tid)
        {
            return RoleplayConnection.Instance.TelephoneContacts.Where(x => x.TID == tid).ToList();
        }

        //public TelephoneContact SelectContact(long did)
        //{
        //    return RoleplayConnection.Instance.TelephoneContacts.Where(x => x.DID == did).FirstOrDefault();
        //}

        public void AddContact(TelephoneContact contact)
        {
            RoleplayConnection.Instance.TelephoneContacts.Add(contact);
            RoleplayConnection.Instance.SaveChanges();
        }

        public void UpdateContact(TelephoneContact contact)
        {
            RoleplayConnection.Instance.TelephoneContacts.Attach(contact);
            RoleplayConnection.Instance.Entry(contact).State = System.Data.Entity.EntityState.Modified;
            RoleplayConnection.Instance.SaveChanges();
        }

        public void DeleteContact(long coid)
        {
            TelephoneContact delobj = RoleplayConnection.Instance.TelephoneContacts.Where(x => x.COID == coid).FirstOrDefault();
            RoleplayConnection.Instance.TelephoneContacts.Remove(delobj);
            RoleplayConnection.Instance.SaveChanges();
        }
    }
}
