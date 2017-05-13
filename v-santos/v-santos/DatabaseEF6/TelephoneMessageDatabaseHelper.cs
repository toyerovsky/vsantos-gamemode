using Serverside.DatabaseEF6.Models;
using System.Collections.Generic;
using System.Linq;

namespace Serverside.DatabaseEF6
{
    public class TelephoneMessageDatabaseHelper
    {
        public List<TelephoneMessage> SelectMessagesList(long tid)
        {
            return RoleplayConnection.Instance.TelephoneMessages.Where(x => x.TID == tid).ToList();
        }

        //public TelephoneContact SelectContact(long did)
        //{
        //    return RoleplayConnection.Instance.TelephoneContacts.Where(x => x.DID == did).FirstOrDefault();
        //}

        public void AddMessage(TelephoneMessage message)
        {
            RoleplayConnection.Instance.TelephoneMessages.Add(message);
            RoleplayConnection.Instance.SaveChanges();
        }

        public void UpdateMessage(TelephoneMessage message)
        {
            RoleplayConnection.Instance.TelephoneMessages.Attach(message);
            RoleplayConnection.Instance.Entry(message).State = System.Data.Entity.EntityState.Modified;
            RoleplayConnection.Instance.SaveChanges();
        }

        public void DeleteMessage(long mid)
        {
            TelephoneMessage delobj = RoleplayConnection.Instance.TelephoneMessages.Where(x => x.MID == mid).FirstOrDefault();
            RoleplayConnection.Instance.TelephoneMessages.Remove(delobj);
            RoleplayConnection.Instance.SaveChanges();
        }
    }
}
