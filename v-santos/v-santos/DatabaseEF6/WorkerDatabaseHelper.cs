using Serverside.DatabaseEF6.Models;
using System.Collections.Generic;
using System.Linq;

namespace Serverside.DatabaseEF6
{
    public static class WorkerDatabaseHelper
    {
        public static List<Worker> SelectWorkersList(Group group)
        {
            return ContextFactory.Instance.Workers.Where(x => x.GroupId == group).ToList();
        }

        public static Worker SelectWorker(long workerId)
        {
            return ContextFactory.Instance.Workers.Where(x => x.WorkerId == workerId).FirstOrDefault();
        }

        public static void AddWorker(Worker worker)
        {
            ContextFactory.Instance.Workers.Add(worker);
            ContextFactory.Instance.SaveChanges();
        }

        public static void UpdateWorker(Worker worker)
        {
            ContextFactory.Instance.Workers.Attach(worker);
            ContextFactory.Instance.Entry(worker).State = System.Data.Entity.EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }

        public static void DeleteWorker(long wid)
        {
            Worker delobj = ContextFactory.Instance.Workers.Where(x => x.WorkerId == wid).FirstOrDefault();
            ContextFactory.Instance.Workers.Remove(delobj);
            ContextFactory.Instance.SaveChanges();
        }
    }
}
