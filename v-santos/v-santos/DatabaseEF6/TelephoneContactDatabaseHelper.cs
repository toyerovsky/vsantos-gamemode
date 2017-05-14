using Serverside.DatabaseEF6.Models;
using System.Collections.Generic;
using System.Linq;

namespace Serverside.DatabaseEF6
{
    public static class TelephoneContactDatabaseHelper
    {
        public static List<TelephoneContact> SelectContactsList(long phonenumber)
        {
            return ContextFactory.Instance.TelephoneContacts.Where(x => x.PhoneNumber == phonenumber).ToList();
        }

        //public TelephoneContact SelectContact(long did)
        //{
        //    return ContextFactory.Instance.TelephoneContacts.Where(x => x.DID == did).FirstOrDefault();
        //}

        public static void AddContact(TelephoneContact contact)
        {
            ContextFactory.Instance.TelephoneContacts.Add(contact);
            ContextFactory.Instance.SaveChanges();
        }

        public static void UpdateContact(TelephoneContact contact)
        {
            ContextFactory.Instance.TelephoneContacts.Attach(contact);
            ContextFactory.Instance.Entry(contact).State = System.Data.Entity.EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }

        public static void DeleteContact(long coid)
        {
            TelephoneContact delobj = ContextFactory.Instance.TelephoneContacts.Where(x => x.ContactId == coid).FirstOrDefault();
            ContextFactory.Instance.TelephoneContacts.Remove(delobj);
            ContextFactory.Instance.SaveChanges();
        }
    }
}
