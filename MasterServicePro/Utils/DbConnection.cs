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
            connectionString = ResolveConnectionString();
        }

        private string ResolveConnectionString()
        {
            string configDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System", "Config");
            string configPath = Path.Combine(configDir, "db.config");

            if (!Directory.Exists(configDir))
            {
                try { Directory.CreateDirectory(configDir); } catch { }
            }

            string currentConfig = null;
            if (File.Exists(configPath))
            {
                try { currentConfig = File.ReadAllText(configPath).Trim(); } catch { }
            }

            if (string.IsNullOrEmpty(currentConfig))
            {
                try
                {
                    var settings = ConfigurationManager.ConnectionStrings["MasterUnlockerDb"];
                    currentConfig = settings != null ? settings.ConnectionString : null;
                }
                catch { }
            }

            // Test if currentConfig works
            if (!string.IsNullOrEmpty(currentConfig) && TestConnection(currentConfig))
            {
                return currentConfig;
            }

            // If currentConfig failed or is empty, try to find a working connection string
            string workingConn = FindWorkingConnectionString();
            if (!string.IsNullOrEmpty(workingConn))
            {
                try { File.WriteAllText(configPath, workingConn); } catch { }
                return workingConn;
            }

            // If all tests failed, return currentConfig or default fallback
            if (!string.IsNullOrEmpty(currentConfig))
            {
                return currentConfig;
            }

            string defaultFallback = @"Server=.\SQLEXPRESS;Database=MasterPDV;Integrated Security=True;TrustServerCertificate=True;";
            try { File.WriteAllText(configPath, defaultFallback); } catch { }
            return defaultFallback;
        }

        private bool TestConnection(string connStr)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(connStr);
                builder.ConnectTimeout = 3;
                using (var conn = new SqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private string FindWorkingConnectionString()
        {
            // 1. Check if production installation in C:\MasterServicePro has a valid db.config
            try
            {
                string prodConfig = @"C:\MasterServicePro\System\Config\db.config";
                if (File.Exists(prodConfig))
                {
                    string content = File.ReadAllText(prodConfig).Trim();
                    if (!string.IsNullOrEmpty(content) && TestConnection(content))
                    {
                        return content;
                    }
                }
            }
            catch { }

            // 2. Candidate connection strings for local instances
            string[] candidates = new string[]
            {
                @"Server=.\SQLEXPRESS;Database=MasterPDV;Integrated Security=True;TrustServerCertificate=True;",
                @"Server=.\SQLEXPRESS;Database=MasterPDV;User Id=sa;Password=!@#Senha2026#@!;TrustServerCertificate=True;",
                @"Server=localhost\SQLEXPRESS;Database=MasterPDV;Integrated Security=True;TrustServerCertificate=True;",
                @"Server=localhost\SQLEXPRESS;Database=MasterPDV;User Id=sa;Password=!@#Senha2026#@!;TrustServerCertificate=True;",
                @"Server=localhost;Database=MasterPDV;Integrated Security=True;TrustServerCertificate=True;",
                @"Server=localhost;Database=MasterPDV;User Id=sa;Password=!@#Senha2026#@!;TrustServerCertificate=True;",
                @"Server=(local);Database=MasterPDV;Integrated Security=True;TrustServerCertificate=True;"
            };

            foreach (var candidate in candidates)
            {
                if (TestConnection(candidate))
                {
                    return candidate;
                }
            }

            return null;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        // Helper to execute Queries (SELECT)
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

        // Helper to execute NonQueries (INSERT, UPDATE, DELETE)
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

        // Helper to return scalar object (COUNT, MAX, ID identity)
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
