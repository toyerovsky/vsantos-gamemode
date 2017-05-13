using Serverside.DatabaseEF6.Models;
using System.Collections.Generic;
using System.Linq;

namespace Serverside.DatabaseEF6
{
    public class ItemDatabaseHelper
    {
        public List<Item> SelectItemsList(long ownerId, int ownerType)
        {
            return RoleplayConnection.Instance.Items.Select(x => new Item { IID = x.IID, Name = x.Name, ItemType = x.ItemType } ).Where(x => x.OID == ownerId && x.OwnerType == ownerType).ToList();
        }

        public Item SelectItem(long itemId)
        {
            return RoleplayConnection.Instance.Items.Where(x => x.IID == itemId).FirstOrDefault();
        }

        public void AddItem(Item item)
        {
            RoleplayConnection.Instance.Items.Add(item);
            RoleplayConnection.Instance.SaveChanges();
        }

        public void UpdateItem(Item item)
        {
            RoleplayConnection.Instance.Items.Attach(item);
            RoleplayConnection.Instance.Entry(item).State = System.Data.Entity.EntityState.Modified;
            RoleplayConnection.Instance.SaveChanges();
        }

        public void DeleteItem(long itemId)
        {
            Item delitem = RoleplayConnection.Instance.Items.Where(x => x.IID == itemId).FirstOrDefault();
            RoleplayConnection.Instance.Items.Remove(delitem);
            RoleplayConnection.Instance.SaveChanges();
        }
    }
}
