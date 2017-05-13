using Serverside.DatabaseEF6.Models;
using System.Data.Entity;

namespace Serverside.DatabaseEF6
{
    public class RoleplayConnection : DbContext
    {
        public RoleplayConnection() : base("name=RoleplayConnection")
        {
            Configuration.LazyLoadingEnabled = false;
            //Database.SetInitializer(new CreateDatabaseIfNotExists<DbContext>());
            Database.CreateIfNotExists();
            Database.Initialize(true);
        }

        private static RoleplayConnection _instance;

        public static RoleplayConnection Instance
        {
            get
            {
                if (_instance != null) return _instance;
                return _instance = new RoleplayConnection();
            }
            private set { }
        }

        public virtual DbSet<Character> Characters { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Building> Buildings { get; set; }
        public virtual DbSet<CrimeBot> CrimeBots { get; set; }
        public virtual DbSet<Description> Descriptions { get; set; }
        public virtual DbSet<TelephoneContact> TelephoneContacts { get; set; }
        public virtual DbSet<TelephoneMessage> TelephoneMessages { get; set; }
        public virtual DbSet<Worker> Workers { get; set; }
    }
}
