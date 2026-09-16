using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.DAL
{
    public class TecnicoRepository
    {
        private DbConnection db = new DbConnection();
        private static bool _tabelaCriada = false;
        private static readonly object _lock = new object();

        public TecnicoRepository()
        {
            try { CriarTabelaSeNaoExistir(); } catch { }
        }

        private void CriarTabelaSeNaoExistir()
        {
            if (_tabelaCriada) return;
            lock (_lock)
            {
                if (_tabelaCriada) return;

                var result = db.ExecuteScalar("SELECT object_id FROM sys.objects WHERE object_id = OBJECT_ID('Tecnicos') AND type in (N'U')");
                if (result == null || result == DBNull.Value)
                {
                    db.ExecuteNonQuery(@"
                        CREATE TABLE Tecnicos (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            Nome VARCHAR(100) NOT NULL,
                            Telefone VARCHAR(20),
                            Especialidade VARCHAR(100),
                            Status VARCHAR(20) DEFAULT 'Disponível',
                            DataCadastro DATETIME DEFAULT GETDATE(),
                            DataAtualizacao DATETIME DEFAULT GETDATE(),
                            Ativo BIT DEFAULT 1
                        )");

                    // Inserir técnico padrão de teste
                    db.ExecuteNonQuery(@"
                        INSERT INTO Tecnicos (Nome, Telefone, Especialidade, Status, DataCadastro, DataAtualizacao, Ativo)
                        VALUES ('Técnico Geral', '99999-9999', 'Geral', 'Disponível', GETDATE(), GETDATE(), 1)");
                }
                _tabelaCriada = true;
            }
        }

        public async System.Threading.Tasks.Task<List<Tecnico>> BuscarTodosAsync()
        {
            using (var conn = db.GetConnection())
            {
                string query = "SELECT * FROM Tecnicos WHERE Ativo = 1 ORDER BY Nome ASC";
                var result = await conn.QueryAsync<Tecnico>(query);
                return result.ToList();
            }
        }

        public List<Tecnico> BuscarTodos()
        {
            using (var conn = db.GetConnection())
            {
                string query = "SELECT * FROM Tecnicos WHERE Ativo = 1 ORDER BY Nome ASC";
                return conn.Query<Tecnico>(query).ToList();
            }
        }

        public Tecnico BuscarPorId(int id)
        {
            using (var conn = db.GetConnection())
            {
                string query = "SELECT * FROM Tecnicos WHERE Id = @Id";
                return conn.QueryFirstOrDefault<Tecnico>(query, new { Id = id });
            }
        }

        public void Inserir(Tecnico t)
        {
            using (var conn = db.GetConnection())
            {
                string query = @"INSERT INTO Tecnicos (Nome, Telefone, Especialidade, Status, DataCadastro, DataAtualizacao, Ativo) 
                                 VALUES (@Nome, @Telefone, @Especialidade, @Status, GETDATE(), GETDATE(), 1)";
                conn.Execute(query, t);
            }
        }

        public void Atualizar(Tecnico t)
        {
            using (var conn = db.GetConnection())
            {
                string query = @"UPDATE Tecnicos SET Nome = @Nome, Telefone = @Telefone, Especialidade = @Especialidade, 
                                 Status = @Status, DataAtualizacao = GETDATE() WHERE Id = @Id";
                conn.Execute(query, t);
            }
        }

        public void Excluir(int id)
        {
            using (var conn = db.GetConnection())
            {
                string query = "UPDATE Tecnicos SET Ativo = 0, DataAtualizacao = GETDATE() WHERE Id = @Id";
                conn.Execute(query, new { Id = id });
            }
        }


    }
}
