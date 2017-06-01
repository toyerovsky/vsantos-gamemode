using System.Collections.Generic;
using System.Linq;
using Serverside.Database.Models;

namespace Serverside.Database
{
    public static class BuildingDatabaseHelper
    {
        public static List<Building> SelectBuildingList(Character character)
        {
            return ContextFactory.Instance.Buildings.Where(x => x.Character.Id == character.Id).ToList();
        }

        public static List<Building> SelectBuildingList(Group group)
        {
            return ContextFactory.Instance.Buildings.Where(x => x.Group.Id == group.Id).ToList();
        }

        public static Building SelectBuilding(long buildingId)
        {
            return ContextFactory.Instance.Buildings.FirstOrDefault(x => x.Id == buildingId);
        }

        public static void AddBuilding(Building buliding)
        {
            ContextFactory.Instance.Buildings.Add(buliding);
            ContextFactory.Instance.SaveChanges();
        }

        public static void UpdateBuilding(Building buliding)
        {
            ContextFactory.Instance.Buildings.Attach(buliding);
            ContextFactory.Instance.Entry(buliding).State = System.Data.Entity.EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }

        public static void DeleteBuilding(long buildingId)
        {
            Building delobj = ContextFactory.Instance.Buildings.FirstOrDefault(x => x.Id == buildingId);
            ContextFactory.Instance.Buildings.Remove(delobj);
            ContextFactory.Instance.SaveChanges();
        }
    }
}
