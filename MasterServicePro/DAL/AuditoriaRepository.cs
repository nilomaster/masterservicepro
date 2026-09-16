using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.DAL
{
    public class AuditoriaRepository
    {
        private DbConnection db = new DbConnection();
        private static bool _tabelaCriada = false;
        private static readonly object _lock = new object();

        public AuditoriaRepository()
        {
            try { CriarTabelaSeNaoExistir(); } catch { }
        }

        private void CriarTabelaSeNaoExistir()
        {
            if (_tabelaCriada) return;
            lock (_lock)
            {
                if (_tabelaCriada) return;

                var result = db.ExecuteScalar("SELECT object_id FROM sys.objects WHERE object_id = OBJECT_ID('Auditoria') AND type in (N'U')");
                if (result == null || result == DBNull.Value)
                {
                    db.ExecuteNonQuery(@"
                        CREATE TABLE Auditoria (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            Usuario VARCHAR(50) NOT NULL,
                            Acao VARCHAR(50) NOT NULL,
                            Tabela VARCHAR(50) NOT NULL,
                            Descricao VARCHAR(MAX) NOT NULL,
                            DataHora DATETIME NOT NULL DEFAULT GETDATE()
                        )");
                }
                _tabelaCriada = true;
            }
        }

        public void Registrar(string acao, string tabela, string descricao)
        {
            try
            {
                string user = !string.IsNullOrEmpty(AuthSession.Usuario) ? AuthSession.Usuario : "Sistema";
                string query = "INSERT INTO Auditoria (Usuario, Acao, Tabela, Descricao, DataHora) VALUES (@Usuario, @Acao, @Tabela, @Descricao, GETDATE())";
                
                SqlParameter[] p = {
                    new SqlParameter("@Usuario", user),
                    new SqlParameter("@Acao", acao),
                    new SqlParameter("@Tabela", tabela),
                    new SqlParameter("@Descricao", descricao)
                };
                db.ExecuteNonQuery(query, p);
            }
            catch { }
        }

        public DataTable Listar(DateTime inicio, DateTime fim, string filtro = "")
        {
            string query = @"
                SELECT DataHora, Usuario, Acao, Tabela, Descricao 
                FROM Auditoria 
                WHERE DataHora BETWEEN @inicio AND @fim";
            
            var list = new List<SqlParameter>
            {
                new SqlParameter("@inicio", inicio.Date),
                new SqlParameter("@fim", fim.Date.AddDays(1).AddSeconds(-1))
            };

            if (!string.IsNullOrEmpty(filtro))
            {
                query += " AND (Usuario LIKE @filtro OR Acao LIKE @filtro OR Tabela LIKE @filtro OR Descricao LIKE @filtro)";
                list.Add(new SqlParameter("@filtro", "%" + filtro + "%"));
            }

            query += " ORDER BY DataHora DESC";

            return db.ExecuteQuery(query, list.ToArray());
        }
    }
}
