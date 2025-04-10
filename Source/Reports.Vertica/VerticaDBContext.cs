using System;
using System.Collections.Generic;
using Vertica.Data.VerticaClient;
using System.Data;

namespace Reports.Vertica
{
    public class VerticaDbContext : IDisposable, IVerticaDbContext
    {
        private readonly string _connectionString;
        private VerticaConnection _connection;

        private readonly Core.Utilities.ISettings Settings;
        private readonly Core.Utilities.IUtilities Utilities;

        public VerticaDbContext(Core.Utilities.ISettings settings,
                                Core.Utilities.IUtilities utilities)
        {
            this.Settings = settings;
            this.Utilities = utilities;

            //string host = this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.VerticaHost);
            //string database = this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.VerticaDB);
            //string username = this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.VerticaUsername);
            //string password = this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.VerticaPassword);

            //VerticaConnectionStringBuilder builder = new VerticaConnectionStringBuilder();
            //builder.Host = host;
            //builder.Database = database;
            //builder.User = username;
            //builder.Password = password;

            //_connectionString = builder.ToString();

            //_connection = new VerticaConnection(_connectionString);
            //_connection.Open();
        }

        public void Reconnect(Int32 delay)
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;

                System.Threading.Thread.Sleep(delay);
            }

            if (_connection == null)
            {
                _connection = new VerticaConnection(_connectionString);
            }

            _connection.Open();
        }

        public void Connect()
        {
            if (_connection == null)
            {
                _connection = new VerticaConnection(_connectionString);
            }

            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        public void Disconnect()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }

            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        public VerticaDbContext(Core.Utilities.ISettings settings,
                                Core.Utilities.IUtilities utilities,
                                String host, String database, String username, String password)
        {
            this.Settings = settings;
            this.Utilities = utilities;

            //string host = this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.VerticaHost);
            //string database = this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.VerticaDB);
            //string username = this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.VerticaUsername);
            //string password = this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.VerticaPassword);

            VerticaConnectionStringBuilder builder = new VerticaConnectionStringBuilder();
            builder.Host = host;
            builder.Database = database;
            builder.User = username;
            builder.Password = password;

            _connectionString = builder.ToString();

            _connection = new VerticaConnection(_connectionString);
            _connection.Open();
        }

        public VerticaConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new VerticaConnection(_connectionString);
                }
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }
                return _connection;
            }
        }

        public void ExecuteNonQuery(string query)
        {
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
            }
        }

        public void ExecuteNonQuery(string query, List<VerticaParameter> parameters)
        {
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = query;

                if (parameters != null && parameters.Count > 0)
                {
                    foreach (VerticaParameter param in parameters)
                        cmd.Parameters.Add(param);
                }

                cmd.ExecuteNonQuery();
            }
        }

        public DataTable ExecuteDataAdapter(string query, List<VerticaParameter> parameters)
        {
            DataTable table = new DataTable();
            VerticaDataAdapter da = new VerticaDataAdapter();
            da.SelectCommand = new VerticaCommand(query,Connection);
            da.SelectCommand.CommandTimeout = 7200;

            if (parameters != null && parameters.Count > 0)
            {
                foreach (VerticaParameter param in parameters)
                    da.SelectCommand.Parameters.Add(param);
            }
            
            da.Fill(table);
            return table;            
        }

        public VerticaDataReader ExecuteReader(String query)
        {
            return this.ExecuteReader(query, null);
        }

        public VerticaDataReader ExecuteReader(String query, List<VerticaParameter> parameters)
        {
            using (VerticaCommand dc = new VerticaCommand(query, this.Connection))
            {
                if (parameters != null && parameters.Count > 0)
                {
                    foreach (VerticaParameter param in parameters)
                        dc.Parameters.Add(param);
                }

                dc.CommandTimeout = 7200;
                VerticaDataReader reader = dc.ExecuteReader();

                return reader;
            }
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();

                this._connection.Dispose();
            }
        }
    }
}
