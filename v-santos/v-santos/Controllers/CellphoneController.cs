using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Serverside.Core.Telephone;
using Serverside.Database;
using Serverside.Database.Models;
using Item = Serverside.Database.Models.Item;

namespace Serverside.Controllers
{
    public class CellphoneController
    {
        public Item Data { get; set; }
        public int? Number => Data.FirstParameter;
        public bool CurrentlyTalking => CurrentCall != null;

        public TelephoneCall CurrentCall { get; set; }

        private List<TelephoneContact> _contacts;
        public List<TelephoneContact> Contacts
        {
            get { return _contacts; }
            set { _contacts = value; }
        }

        private List<TelephoneMessage> _messages;
        public List<TelephoneMessage> Messages
        {
            get { return _messages; }
            set { _messages = value; }
        }

        public CellphoneController(Item item)
        {
            Data = item;
            Contacts = ContextFactory.Instance.TelephoneContacts.Where(x => x.PhoneNumber == item.FirstParameter)
                .ToList();
            Messages = ContextFactory.Instance.TelephoneMessages.Where(x => x.PhoneNumber == item.FirstParameter)
                .ToList();
        }

        public void Save()
        {
            ContextFactory.Instance.Items.Attach(Data);
            ContextFactory.Instance.Entry(Data).State = EntityState.Modified;
            
            //bezsens
            //ContextFactory.Instance.TelephoneContacts.AddRange(Contacts);
            //ContextFactory.Instance.Entry(Contacts).State = EntityState.Modified;

            //ContextFactory.Instance.TelephoneMessages.AddRange(Messages);
            //ContextFactory.Instance.Entry(Messages).State = EntityState.Modified;

            ContextFactory.Instance.SaveChanges();

        }
    }
}