using Serverside.DatabaseEF6.Models;
using System.Collections.Generic;
using System.Linq;

namespace Serverside.DatabaseEF6
{
    public static class BuildingDatabaseHelper
    {
        public static List<Building> SelectBuildingList(Character character)
        {
            return ContextFactory.Instance.Buildings.Where(x => x.Character.CharacterId == character.CharacterId).ToList();
        }

        public static List<Building> SelectBuildingList(Group group)
        {
            return ContextFactory.Instance.Buildings.Where(x => x.Group.GroupId == group.GroupId).ToList();
        }

        public static Building SelectBuilding(long buildingId)
        {
            return ContextFactory.Instance.Buildings.Where(x => x.BuildingId == buildingId).FirstOrDefault();
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
            Building delobj = ContextFactory.Instance.Buildings.Where(x => x.BuildingId == buildingId).FirstOrDefault();
            ContextFactory.Instance.Buildings.Remove(delobj);
            ContextFactory.Instance.SaveChanges();
        }
    }
}
