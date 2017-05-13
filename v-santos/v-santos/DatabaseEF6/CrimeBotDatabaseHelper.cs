using Serverside.DatabaseEF6.Models;
using System.Collections.Generic;
using System.Linq;

namespace Serverside.DatabaseEF6
{
    public class CrimeBotDatabaseHelper
    {
        public List<CrimeBot> SelectCrimeBots(long gid)
        {
            return RoleplayConnection.Instance.CrimeBots.Where(x => x.GroupId == gid).ToList();
        }

        public CrimeBot SelectCrimeBot(long bid)
        {
            return RoleplayConnection.Instance.CrimeBots.Where(x => x.BotId == bid).FirstOrDefault();
        }

        public void AddCrimeBot(CrimeBot editor)
        {
            RoleplayConnection.Instance.CrimeBots.Add(editor);
            RoleplayConnection.Instance.SaveChanges();
        }

        public void UpdateCrimeBot(CrimeBot editor)
        {
            RoleplayConnection.Instance.CrimeBots.Attach(editor);
            RoleplayConnection.Instance.Entry(editor).State = System.Data.Entity.EntityState.Modified;
            RoleplayConnection.Instance.SaveChanges();
        }

        public void DeleteCrimeBot(long bid)
        {
            CrimeBot delobj = RoleplayConnection.Instance.CrimeBots.Where(x => x.BotId == bid).FirstOrDefault();
            RoleplayConnection.Instance.CrimeBots.Remove(delobj);
            RoleplayConnection.Instance.SaveChanges();
        }
    }
}
