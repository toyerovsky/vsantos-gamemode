using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using GTANetworkServer;
using MySql.Data.Entity;
using MySql.Data.MySqlClient;
using Serverside.Database.Models;
using Vehicle = Serverside.Database.Models.Vehicle;
using Serverside.Core.Extensions;

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
        private static string _connectionString;

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

            _connectionString = connectionStringBuilder.ToString();
        }

        private static RoleplayConnection _instance;

        public static RoleplayConnection Instance
        {
            get
            {
                if (_instance != null) return _instance;
                return _instance = new ContextFactory().Create();
            }
            private set { }
        }
        public RoleplayConnection Create()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                APIExtensions.ConsoleOutput("[RPCore] Brak danych wymaganych do połączenia z bazą danych!", ConsoleColor.DarkRed);
                API.shared.stopResource("vsantos");
                //throw new Exception();
            }
            try
            {
                var conn = new MySqlConnection(_connectionString);
                conn.Open();
                conn.Ping();
                conn.Close();
            }
            catch (MySqlException aEx)
            {
                APIExtensions.ConsoleOutput("[RPCore] Błąd połączenia z bazą danych, sprawdź konfiguracje!", ConsoleColor.DarkRed);
                APIExtensions.ConsoleOutput(aEx.Message, ConsoleColor.DarkRed);
                API.shared.stopResource("vsantos");
                //APIExtensions.ConsoleOutput(a_ex.ToString(), ConsoleColor.DarkRed);
                //throw new Exception();
            }
            APIExtensions.ConsoleOutput("[RPCore] Połączono z bazą danych!", ConsoleColor.DarkGreen);
            return new RoleplayConnection(_connectionString);
            //return new RoleplayConnection("server=v-santos.pl;uid=srv;pwd=WL8oTnufAAEFgoIt;database=rp"); // TYLKO DO GENEROWANIA BAZY DANYCH 
        }

        public static void Destroy()
        {
            _instance = null;
        }
    }
}

