﻿using Serverside.DatabaseEF6.Models;
using System.Collections.Generic;
using System.Linq;

namespace Serverside.DatabaseEF6
{
    public static class TelephoneMessageDatabaseHelper
    {
        public static List<TelephoneMessage> SelectMessagesList(long phonenumber)
        {
            return ContextFactory.Instance.TelephoneMessages.Where(x => x.PhoneNumber == phonenumber).ToList();
        }

        //public TelephoneContact SelectContact(long did)
        //{
        //    return ContextFactory.Instance.TelephoneContacts.Where(x => x.DID == did).FirstOrDefault();
        //}

        public static void AddMessage(TelephoneMessage message)
        {
            ContextFactory.Instance.TelephoneMessages.Add(message);
            ContextFactory.Instance.SaveChanges();
        }

        public static void UpdateMessage(TelephoneMessage message)
        {
            ContextFactory.Instance.TelephoneMessages.Attach(message);
            ContextFactory.Instance.Entry(message).State = System.Data.Entity.EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }

        public static void DeleteMessage(long mid)
        {
            TelephoneMessage delobj = ContextFactory.Instance.TelephoneMessages.Where(x => x.MessageId == mid).FirstOrDefault();
            ContextFactory.Instance.TelephoneMessages.Remove(delobj);
            ContextFactory.Instance.SaveChanges();
        }
    }
}