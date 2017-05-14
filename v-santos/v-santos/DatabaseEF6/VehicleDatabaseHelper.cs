using Serverside.DatabaseEF6.Models;
using System.Collections.Generic;
using System.Linq;

namespace Serverside.DatabaseEF6
{
    public static class VehicleDatabaseHelper
    {
        public static List<Vehicle> SelectVehiclesList(Character character)
        {
            return ContextFactory.Instance.Vehicles.Where(x => x.CharacterId == character).ToList();
        }

        public static List<Vehicle> SelectVehiclesList(Group group)
        {
            return ContextFactory.Instance.Vehicles.Where(x => x.GroupId == group).ToList();
        }

        public static Vehicle SelectVehicle(long vid)
        {
            return ContextFactory.Instance.Vehicles.Where(x => x.VehicleId == vid).FirstOrDefault();
        }

        public static void AddVehicle(Vehicle vehicle)
        {
            ContextFactory.Instance.Vehicles.Add(vehicle);
            ContextFactory.Instance.SaveChanges();
        }

        public static void UpdateVehicle(Vehicle vehicle)
        {
            ContextFactory.Instance.Vehicles.Attach(vehicle);
            ContextFactory.Instance.Entry(vehicle).State = System.Data.Entity.EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }

        public static void DeleteVehicle(long vid)
        {
            Vehicle delobj = ContextFactory.Instance.Vehicles.Where(x => x.VehicleId == vid).FirstOrDefault();
            ContextFactory.Instance.Vehicles.Remove(delobj);
            ContextFactory.Instance.SaveChanges();
        }
    }
}
