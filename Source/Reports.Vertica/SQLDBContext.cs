using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Reports.SQLServer
{
    public class SQLDBContext : IDisposable, ISQLDbContext
    {
        private readonly Core.Utilities.ISettings Settings;
        private readonly Core.Utilities.IUtilities Utilities;
        private readonly string _connectionString;
        private SqlConnection _connection;

        public SQLDBContext(Core.Utilities.ISettings settings,
                                Core.Utilities.IUtilities utilities)
        {
            this.Settings = settings;
            this.Utilities = utilities;

            string connectString = System.Configuration.ConfigurationManager.ConnectionStrings["CacheSQLServerConnection"].ToString();
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectString);

            _connectionString = builder.ConnectionString;

            _connection = new SqlConnection(_connectionString);
            _connection.Open();
        }

        public SQLDBContext(Core.Utilities.ISettings settings,
                            Core.Utilities.IUtilities utilities,
                            string connectionStringName)
        {
            this.Settings = settings;
            this.Utilities = utilities;

            string connectString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName].ToString();
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectString);

            _connectionString = builder.ConnectionString;

            _connection = new SqlConnection(_connectionString);
            _connection.Open();
        }

        public SqlConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new System.Data.SqlClient.SqlConnection(_connectionString);
                }
                if (_connection.State != System.Data.ConnectionState.Open)
                {
                    _connection.Open();
                }
                return _connection;
            }
        }

        public void ExecuteNonQuery(string query, Int32 timeout)
        {
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandTimeout = timeout;
                cmd.CommandText = query;

                cmd.ExecuteNonQuery();
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

        public void ExecuteNonQuery(string query, List<SqlParameter> parameters)
        {
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = query;

                if (parameters != null && parameters.Count > 0)
                {
                    foreach (SqlParameter param in parameters)
                        cmd.Parameters.Add(param);
                }

                cmd.ExecuteNonQuery();
            }
        }

        public DataTable ExecuteDataAdapter(string query)
        {
            return this.ExecuteDataAdapter(query, null);
        }

        public DataTable ExecuteDataAdapter(string query, List<SqlParameter> parameters)
        {
            DataTable table = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = new SqlCommand(query, Connection);

            if (parameters != null && parameters.Count > 0)
            {
                foreach (SqlParameter param in parameters)
                    da.SelectCommand.Parameters.Add(param);
            }

            da.Fill(table);
            return table;
        }

        public SqlDataReader ExecuteReader(string query)
        {
            return this.ExecuteReader(query, null);
        }

        public SqlDataReader ExecuteReader(string query, List<SqlParameter> parameters)
        {
            DataTable table = new DataTable();
            SqlCommand sqlCommand = new SqlCommand(query, Connection);

            if (parameters != null && parameters.Count > 0)
            {
                foreach (SqlParameter param in parameters)
                    sqlCommand.Parameters.Add(param);
            }

            return sqlCommand.ExecuteReader();
        }

        public void Insert(String tableName, List<Dictionary<String, Object>> data)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();

                Int32 index = 1;

                String columns = String.Empty;
                String values = String.Empty;

                if (data != null)
                {
                    foreach (var item in data.FirstOrDefault())
                    {
                        columns = String.Format("{0}{1}[{2}]", columns, !String.IsNullOrEmpty(columns) ? "," : String.Empty, item.Key);
                    }

                    List<String> selects = new List<String>();

                    Int32 rowIndex = 0;
                    Int32 skip = 0;

                    while (rowIndex < data.Count())
                    {
                        selects = new List<String>();
                        skip = rowIndex;
                        parameters = new List<SqlParameter>();

                        foreach (var row in data.Skip(skip).Take(1000))
                        {
                            values = String.Empty;

                            foreach (var item in row)
                            {
                                //if (item.Value == null)
                                //    item.Value = String.Empty;

                                String value = String.Empty;

                                if (item.Value != null)
                                {                                     
                                    value = item.Value.ToString();
                                }

                                //String value = String.Empty;

                                //if (this.Utilities.IsNumeric(item.Value))
                                //{
                                //    value = item.Value.ToString();
                                //}
                                //else if (item.Value.GetType() == typeof(Boolean))
                                //{
                                //    value = item.Value.ToString().ToUpper();
                                //}
                                //else if (item.Value.GetType() == typeof(DateTime))
                                //{
                                //    value = String.Format("'{0:yyyy-MM-dd HH:mm:ss.fff}'::timestamp", item.Value);
                                //}
                                //else if (item.Value.GetType() == typeof(String))
                                //{
                                //    value = String.Format("'{0}'", item.Value.ToString().Replace("'", "''"));
                                //}
                                //else
                                //{
                                //    value = String.Format("'{0}'", item.Value.ToString().Replace("'", "''"));
                                //}

                                double doubleValue = 0;
                                Int32 Int32Value = 0;

                                if (String.IsNullOrEmpty(value))
                                {
                                    value = "NULL";
                                }
                                else
                                {
                                    if (!(Int32.TryParse(value, out Int32Value) || double.TryParse(value.ToString(), out doubleValue)) || String.IsNullOrEmpty(value))
                                        value = String.Format("'{0}'", item.Value.ToString().Replace("'", "''"));
                                }

                                values = String.Format("{0}{1}{2}", values, !String.IsNullOrEmpty(values) ? "," : String.Empty, value);
                                index++;
                            }

                            selects.Add(String.Format("({0})", values));
                            rowIndex++;
                        }

                        String insertQuery = String.Format("INSERT INTO {0} ({1}) {2} values {3}", tableName, columns, System.Environment.NewLine, String.Join(String.Format(", {0}", System.Environment.NewLine), selects));

                        this.ExecuteNonQuery(insertQuery);

                        insertQuery = String.Empty;
                        selects = null;
                        //columns = String.Empty;
                        values = String.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
