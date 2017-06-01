using System.Collections.Generic;
using System.Linq;
using Serverside.Database.Models;

namespace Serverside.Database
{
    public static class GroupDatabaseHelper
    {
        public static List<Group> SelectAllGroups()
        {
            return ContextFactory.Instance.Groups.ToList();
        }

        public static Group SelectGroup(long gid)
        {
            return ContextFactory.Instance.Groups.FirstOrDefault(x => x.Id == gid);
        }

        public static void AddGroup(Group group)
        {
            ContextFactory.Instance.Groups.Add(group);
            ContextFactory.Instance.SaveChanges();
        }

        public static void UpdateGroup(Group group)
        {
            ContextFactory.Instance.Groups.Attach(group);
            ContextFactory.Instance.Entry(group).State = System.Data.Entity.EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }

        public static void DeleteGroup(long gid)
        {
            Group delobj = ContextFactory.Instance.Groups.FirstOrDefault(x => x.Id == gid);
            ContextFactory.Instance.Groups.Remove(delobj);
            ContextFactory.Instance.SaveChanges();
        }
    }
}
