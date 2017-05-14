using Serverside.DatabaseEF6.Models;
using System.Collections.Generic;
using System.Linq;

namespace Serverside.DatabaseEF6
{
    public class ItemDatabaseHelper
    {
        public static List<Item> SelectItemsList(Character character)
        {
            return ContextFactory.Instance.Items.Where(x => x.Character.CharacterId == character.CharacterId).ToList();
        }

        public static List<Item> SelectItemsList(Building building)
        {
            return ContextFactory.Instance.Items.Where(x => x.Building.BuildingId == building.BuildingId).ToList();
        }

        public static Item SelectItem(long itemId)
        {
            return ContextFactory.Instance.Items.Where(x => x.ItemId == itemId).FirstOrDefault();
        }

        public static void AddItem(Item item)
        {
            ContextFactory.Instance.Items.Add(item);
            ContextFactory.Instance.SaveChanges();
        }

        public static void UpdateItem(Item item)
        {
            ContextFactory.Instance.Items.Attach(item);
            ContextFactory.Instance.Entry(item).State = System.Data.Entity.EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }

        public static void DeleteItem(long itemId)
        {
            Item delitem = ContextFactory.Instance.Items.Where(x => x.ItemId == itemId).FirstOrDefault();
            ContextFactory.Instance.Items.Remove(delitem);
            ContextFactory.Instance.SaveChanges();
        }
    }
}
