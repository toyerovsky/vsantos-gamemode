using Serverside.DatabaseEF6.Models;
using System.Collections.Generic;
using System.Linq;

namespace Serverside.DatabaseEF6
{
    public class VehicleDatabaseHelper
    {
        public List<Vehicle> SelectCharactersList(long ownerUID, int ownerType)
        {
            return RoleplayConnection.Instance.Vehicles.Where(x => x.OwnerID == ownerUID && x.OwnerType == ownerType).ToList();
        }

        public Vehicle SelectVehicle(long vid)
        {
            return RoleplayConnection.Instance.Vehicles.Where(x => x.VID == vid).FirstOrDefault();
        }

        public void AddVehicle(Vehicle vehicle)
        {
            RoleplayConnection.Instance.Vehicles.Add(vehicle);
            RoleplayConnection.Instance.SaveChanges();
        }

        public void UpdateVehicle(Vehicle vehicle)
        {
            RoleplayConnection.Instance.Vehicles.Attach(vehicle);
            RoleplayConnection.Instance.Entry(vehicle).State = System.Data.Entity.EntityState.Modified;
            RoleplayConnection.Instance.SaveChanges();
        }

        public void DeleteVehicle(long vid)
        {
            Vehicle delobj = RoleplayConnection.Instance.Vehicles.Where(x => x.VID == vid).FirstOrDefault();
            RoleplayConnection.Instance.Vehicles.Remove(delobj);
            RoleplayConnection.Instance.SaveChanges();
        }
    }
}
