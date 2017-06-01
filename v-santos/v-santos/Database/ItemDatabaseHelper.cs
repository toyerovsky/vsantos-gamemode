using System.Collections.Generic;
using System.Linq;
using Serverside.Database.Models;

namespace Serverside.Database
{
    public class ItemDatabaseHelper
    {
        public static List<Item> SelectItemsList(Character character)
        {
            return ContextFactory.Instance.Items.Where(x => x.Character.Id == character.Id).ToList();
        }

        public static List<Item> SelectItemsList(Building building)
        {
            return ContextFactory.Instance.Items.Where(x => x.Building.Id == building.Id).ToList();
        }

        public static List<Item> SelectItemsList(Vehicle vehicle)
        {
            return ContextFactory.Instance.Items.Where(x => x.Vehicle.Id == vehicle.Id).ToList();
        }

        public static Item SelectItem(long itemId)
        {
            return ContextFactory.Instance.Items.FirstOrDefault(x => x.Id == itemId);
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
            Item delitem = ContextFactory.Instance.Items.FirstOrDefault(x => x.Id == itemId);
            ContextFactory.Instance.Items.Remove(delitem);
            ContextFactory.Instance.SaveChanges();
        }
    }
}
