using System.Collections.Generic;
using System.Linq;
using Serverside.Database.Models;

namespace Serverside.Database
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
            TelephoneContact delobj = ContextFactory.Instance.TelephoneContacts.FirstOrDefault(x => x.Id == coid);
            ContextFactory.Instance.TelephoneContacts.Remove(delobj);
            ContextFactory.Instance.SaveChanges();
        }
    }
}
