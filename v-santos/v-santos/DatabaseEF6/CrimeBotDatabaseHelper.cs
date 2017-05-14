using Serverside.DatabaseEF6.Models;
using System.Collections.Generic;
using System.Linq;

namespace Serverside.DatabaseEF6
{
    public static class CrimeBotDatabaseHelper
    {
        public static List<CrimeBot> SelectCrimeBots(Group gid)
        {
            return ContextFactory.Instance.CrimeBots.Where(x => x.GroupId == gid).ToList();
        }

        public static CrimeBot SelectCrimeBot(long bid)
        {
            return ContextFactory.Instance.CrimeBots.Where(x => x.BotId == bid).FirstOrDefault();
        }

        public static void AddCrimeBot(CrimeBot editor)
        {
            ContextFactory.Instance.CrimeBots.Add(editor);
            ContextFactory.Instance.SaveChanges();
        }

        public static void UpdateCrimeBot(CrimeBot editor)
        {
            ContextFactory.Instance.CrimeBots.Attach(editor);
            ContextFactory.Instance.Entry(editor).State = System.Data.Entity.EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }

        public static void DeleteCrimeBot(long bid)
        {
            CrimeBot delobj = ContextFactory.Instance.CrimeBots.Where(x => x.BotId == bid).FirstOrDefault();
            ContextFactory.Instance.CrimeBots.Remove(delobj);
            ContextFactory.Instance.SaveChanges();
        }
    }
}
