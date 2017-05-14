using MySql.Data.Entity;
using MySql.Data.MySqlClient;
using Serverside.DatabaseEF6.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Serverside.DatabaseEF6
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class RoleplayConnection : DbContext
    {
        public RoleplayConnection(string connectionString) : base(connectionString)
        {
            //Configuration.LazyLoadingEnabled = false;
            //Database.SetInitializer(new CreateDatabaseIfNotExists<DbContext>());
            //Database.CreateIfNotExists();
            //Database.Initialize(true);
        }

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            builder.Conventions.Remove<PluralizingTableNameConvention>();
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

    public class ContextFactory : IDbContextFactory<RoleplayConnection>
    {
        private static string ConnectionString;

        public static void SetConnectionParameters(string serverAddress, string username, string password, string database, uint port = 3306)
        {
            var connectionStringBuilder = new MySqlConnectionStringBuilder()
            {
                Server = serverAddress,
                UserID = username,
                Password = password,
                Database = database,
                Port = port
            };

            ConnectionString = connectionStringBuilder.ToString();
        }

        private static RoleplayConnection _instance;

        public static RoleplayConnection Instance
        {
            get
            {
                if (_instance != null) return _instance;
                Console.WriteLine(ConnectionString);
                return _instance = new ContextFactory().Create();
            }
            private set { }
        }
        public RoleplayConnection Create()
        {
            if (string.IsNullOrEmpty(ConnectionString)) throw new InvalidOperationException("Brak danych wymaganych do połączenia z bazą danych.");

            //return new RoleplayConnection("server=localhost;user id=root;password=;database=rp;port=3306"); // TYLKO DO GENEROWANIA BAZY DANYCH
            return new RoleplayConnection(ConnectionString);
        }
    }
}

