using System.Collections.Generic;
using System.Linq;
using Serverside.Database.Models;

namespace Serverside.Database
{
    public static class VehicleDatabaseHelper
    {
        public static List<Vehicle> SelectVehiclesList(Character character)
        {
            return ContextFactory.Instance.Vehicles.Where(x => x.Character.Id == character.Id).ToList();
        }

        public static List<Vehicle> SelectVehiclesList(Group group)
        {
            return ContextFactory.Instance.Vehicles.Where(x => x.Group.Id == group.Id).ToList();
        }

        public static Vehicle SelectVehicle(long vid)
        {
            return ContextFactory.Instance.Vehicles.FirstOrDefault(x => x.Id == vid);
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
            Vehicle delobj = ContextFactory.Instance.Vehicles.FirstOrDefault(x => x.Id == vid);
            ContextFactory.Instance.Vehicles.Remove(delobj);
            ContextFactory.Instance.SaveChanges();
        }
    }
}
