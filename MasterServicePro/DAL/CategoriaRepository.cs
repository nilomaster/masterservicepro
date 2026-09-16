using System;
using System.Collections.Generic;
using System.Data;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.DAL
{
    public class CategoriaRepository
    {
        private DbConnection db = new DbConnection();

        private static bool _tabelaCriada = false;
        private static readonly object _lock = new object();

        public CategoriaRepository()
        {
            try { CriarTabelaCategorias(); } catch { }
        }

        private void CriarTabelaCategorias()
        {
            if (_tabelaCriada) return;
            lock (_lock)
            {
                if (_tabelaCriada) return;

                var result = db.ExecuteScalar("SELECT object_id FROM sys.objects WHERE object_id = OBJECT_ID('Categorias') AND type in (N'U')");
                if (result == null || result == DBNull.Value)
                {
                    db.ExecuteNonQuery(@"
                        CREATE TABLE Categorias (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            Nome VARCHAR(100) NOT NULL,
                            Ativo BIT DEFAULT 1
                        )");

                    // Inserir categorias padrões
                    db.ExecuteNonQuery("INSERT INTO Categorias (Nome) VALUES ('Geral')");
                    db.ExecuteNonQuery("INSERT INTO Categorias (Nome) VALUES ('Peças')");
                    db.ExecuteNonQuery("INSERT INTO Categorias (Nome) VALUES ('Acessórios')");
                    db.ExecuteNonQuery("INSERT INTO Categorias (Nome) VALUES ('Serviços')");
                }
                _tabelaCriada = true;
            }
        }

        public List<Categoria> BuscarTodas()
        {
            List<Categoria> lista = new List<Categoria>();
            string query = "SELECT * FROM Categorias WHERE Ativo = 1 ORDER BY Nome ASC";
            
            DataTable dt = db.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new Categoria
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Nome = row["Nome"].ToString(),
                    Ativo = Convert.ToBoolean(row["Ativo"])
                });
            }
            return lista;
        }
        public void Inserir(string nome)
        {
            var parameters = new System.Data.SqlClient.SqlParameter[]
            {
                new System.Data.SqlClient.SqlParameter("@Nome", nome)
            };
            // Adicionado 'Grupo' para evitar erro de coluna NOT NULL que existe no banco real
            db.ExecuteNonQuery("INSERT INTO Categorias (Nome, Grupo) VALUES (@Nome, 'Geral')", parameters);
        }
    }
}
