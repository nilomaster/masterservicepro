using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace MasterServicePro.Utils
{
    public class DbConnection
    {
        private readonly string connectionString;

        public DbConnection()
        {
            try
            {
                string configDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System", "Config");
                string configPath = Path.Combine(configDir, "db.config");

                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }

                if (File.Exists(configPath))
                {
                    connectionString = File.ReadAllText(configPath).Trim();
                }
                else
                {
                    // Fallback to App.config
                    var settings = ConfigurationManager.ConnectionStrings["MasterUnlockerDb"];
                    connectionString = settings != null ? settings.ConnectionString : null;

                    if (string.IsNullOrEmpty(connectionString))
                    {
                        // Default installation fallback pointing to MasterPDV on local SQLEXPRESS
                        connectionString = @"Server=.\SQLEXPRESS;Database=MasterPDV;Integrated Security=True;";
                    }

                    File.WriteAllText(configPath, connectionString);
                }
            }
            catch
            {
                connectionString = @"Server=.\SQLEXPRESS;Database=MasterPDV;Integrated Security=True;";
            }
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        // Helper para executar Queries (SELECT)
        public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        // Helper para executar NonQueries (INSERT, UPDATE, DELETE)
        public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // Helper para retornar objeto escalar (COUNT, MAX, ID identity)
        public object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}
