using Serverside.DatabaseEF6.Models;
using System.Collections.Generic;
using System.Linq;

namespace Serverside.DatabaseEF6
{
    public class GroupDatabaseHelper
    {
        public List<Group> SelectAllGroups()
        {
            return RoleplayConnection.Instance.Groups.ToList();
        }

        public Group SelectGroup(long gid)
        {
            return RoleplayConnection.Instance.Groups.Where(x => x.GID == gid).FirstOrDefault();
        }

        public void AddGroup(Group group)
        {
            RoleplayConnection.Instance.Groups.Add(group);
            RoleplayConnection.Instance.SaveChanges();
        }

        public void UpdateGroup(Group group)
        {
            RoleplayConnection.Instance.Groups.Attach(group);
            RoleplayConnection.Instance.Entry(group).State = System.Data.Entity.EntityState.Modified;
            RoleplayConnection.Instance.SaveChanges();
        }

        public void DeleteGroup(long gid)
        {
            Group delobj = RoleplayConnection.Instance.Groups.Where(x => x.GID == gid).FirstOrDefault();
            RoleplayConnection.Instance.Groups.Remove(delobj);
            RoleplayConnection.Instance.SaveChanges();
        }
    }
}
