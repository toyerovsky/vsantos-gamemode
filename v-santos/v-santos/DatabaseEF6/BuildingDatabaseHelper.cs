using Serverside.DatabaseEF6.Models;
using System.Collections.Generic;
using System.Linq;

namespace Serverside.DatabaseEF6
{
    public class BuildingDatabaseHelper
    {
        public List<Building> SelectBuildingList(long ownerId, int ownerType)
        {
            return RoleplayConnection.Instance.Buildings.Where(x => x.OwnerUID == ownerId && x.OwnerType == ownerType).ToList();
        }

        public Building SelectBuilding(long buildingId)
        {
            return RoleplayConnection.Instance.Buildings.Where(x => x.BID == buildingId).FirstOrDefault();
        }

        public void AddBuilding(Building buliding)
        {
            RoleplayConnection.Instance.Buildings.Add(buliding);
            RoleplayConnection.Instance.SaveChanges();
        }

        public void UpdateBuilding(Building buliding)
        {
            RoleplayConnection.Instance.Buildings.Attach(buliding);
            RoleplayConnection.Instance.Entry(buliding).State = System.Data.Entity.EntityState.Modified;
            RoleplayConnection.Instance.SaveChanges();
        }

        public void DeleteBuilding(long buildingId)
        {
            Building delobj = RoleplayConnection.Instance.Buildings.Where(x => x.BID == buildingId).FirstOrDefault();
            RoleplayConnection.Instance.Buildings.Remove(delobj);
            RoleplayConnection.Instance.SaveChanges();
        }
    }
}
