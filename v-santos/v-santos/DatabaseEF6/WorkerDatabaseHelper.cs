using Serverside.DatabaseEF6.Models;
using System.Collections.Generic;
using System.Linq;

namespace Serverside.DatabaseEF6
{
    public class WorkerDatabaseHelper
    {
        public List<Worker> SelectWorkersList(long groupUid)
        {
            return RoleplayConnection.Instance.Workers.Where(x => x.GID == groupUid).ToList();
        }

        public Worker SelectWorker(long workerId)
        {
            return RoleplayConnection.Instance.Workers.Where(x => x.WID == workerId).FirstOrDefault();
        }

        public void AddWorker(Worker worker)
        {
            RoleplayConnection.Instance.Workers.Add(worker);
            RoleplayConnection.Instance.SaveChanges();
        }

        public void UpdateWorker(Worker worker)
        {
            RoleplayConnection.Instance.Workers.Attach(worker);
            RoleplayConnection.Instance.Entry(worker).State = System.Data.Entity.EntityState.Modified;
            RoleplayConnection.Instance.SaveChanges();
        }

        public void DeleteWorker(long wid)
        {
            Worker delobj = RoleplayConnection.Instance.Workers.Where(x => x.WID == wid).FirstOrDefault();
            RoleplayConnection.Instance.Workers.Remove(delobj);
            RoleplayConnection.Instance.SaveChanges();
        }
    }
}
