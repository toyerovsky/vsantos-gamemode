/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using MySql.Data.Entity;
using MySql.Data.MySqlClient;
using Serverside.Database.Models;
using Vehicle = Serverside.Database.Models.Vehicle;
using Serverside.Core.Extensions;
using Serverside.Database.Exceptions;

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
        public virtual DbSet<Models.CrimeBot> CrimeBots { get; set; }
        public virtual DbSet<Description> Descriptions { get; set; }
        public virtual DbSet<TelephoneContact> TelephoneContacts { get; set; }
        public virtual DbSet<TelephoneMessage> TelephoneMessages { get; set; }
        public virtual DbSet<Worker> Workers { get; set; }
        public virtual DbSet<GroupWarehouseItem> GroupWarehouseItems { get; set; }
        public virtual DbSet<GroupWarehouseOrder> GroupWarehouseOrders { get; set; }
    }

    public class ContextFactory : IDbContextFactory<RoleplayConnection>
    {
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
            var connectionStringBuilder = new MySqlConnectionStringBuilder()
            {
                Server = "137.74.4.8",
                UserID = "chat",
                Password = "%$*!H#%NFK{!EFjmcr903umn1CM[4RJX913RY8V1[M!$!vASFFG35215",
                Database = "rpchat",
                Port = 3306,
                ConvertZeroDateTime = true,
                AllowZeroDateTime = true
            };

            string connectionString = connectionStringBuilder.ToString();

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new RoleplayConnectionException("Podano pusty connection string.");
            }

            try
            {
                var conn = new MySqlConnection(connectionString);
                conn.Open();
                conn.Ping();
                conn.Close();
            }
            catch (MySqlException e)
            {
                throw new RoleplayConnectionException(e.Message);
            }
            APIExtensions.ConsoleOutput("[RPCore] Połączono z bazą danych!", ConsoleColor.DarkGreen);
            return new RoleplayConnection(connectionString);
            //return new RoleplayConnection("Server=137.74.4.8;Database=rpchat;Uid=chat;Pwd=%$*!H#%NFK{!EFjmcr903umn1CM[4RJX913RY8V1[M!$!vASFFG35215;");
        }

        public static void Destroy()
        {
            _instance = null;
        }
    }
}

