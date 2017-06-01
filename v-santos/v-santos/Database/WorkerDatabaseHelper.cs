using System.Collections.Generic;
using System.Linq;
using Serverside.Database.Models;

namespace Serverside.Database
{
    public static class WorkerDatabaseHelper
    {
        public static List<Worker> SelectWorkersList(Group group)
        {
            return ContextFactory.Instance.Workers.Where(x => x.Group.Id == group.Id).ToList();
        }

        public static Worker SelectWorker(long workerId)
        {
            return ContextFactory.Instance.Workers.FirstOrDefault(x => x.Id == workerId);
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
            Worker delobj = ContextFactory.Instance.Workers.FirstOrDefault(x => x.Id == wid);
            ContextFactory.Instance.Workers.Remove(delobj);
            ContextFactory.Instance.SaveChanges();
        }
    }
}
