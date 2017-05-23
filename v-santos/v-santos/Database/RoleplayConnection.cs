using GTANetworkServer;
using MySql.Data.Entity;
using MySql.Data.MySqlClient;
using Serverside.Core.Extenstions;
using Serverside.DatabaseEF6.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Serverside.Database
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

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Ban> Bans { get; set; }
        public virtual DbSet<Character> Characters { get; set; }
        public virtual DbSet<DatabaseEF6.Models.Vehicle> Vehicles { get; set; }
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
            if (string.IsNullOrEmpty(ConnectionString))
            {
                APIExtensions.ConsoleOutput("[RPCore] Brak danych wymaganych do połączenia z bazą danych!", ConsoleColor.DarkRed);
                API.shared.stopResource("vsantos");
                //throw new Exception();
            }
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(ConnectionString);
                conn.Open();
                conn.Ping();
                conn.Close();
            }
            catch (MySqlException a_ex)
            {
                APIExtensions.ConsoleOutput("[RPCore] Błąd połączenia z bazą danych, sprawdź konfiguracje!", ConsoleColor.DarkRed);
                APIExtensions.ConsoleOutput(a_ex.Message, ConsoleColor.DarkRed);
                API.shared.stopResource("vsantos");
                //APIExtensions.ConsoleOutput(a_ex.ToString(), ConsoleColor.DarkRed);
                //throw new Exception();
            }
            APIExtensions.ConsoleOutput("[RPCore] Połączono z bazą danych!", ConsoleColor.DarkGreen);
            //return new RoleplayConnection("server=localhost;user id=root;password=;database=rp;port=3306"); // TYLKO DO GENEROWANIA BAZY DANYCH
            return new RoleplayConnection(ConnectionString);
        }
    }
}

