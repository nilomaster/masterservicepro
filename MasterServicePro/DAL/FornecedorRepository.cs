using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.DAL
{
    public class FornecedorRepository
    {
        private DbConnection db = new DbConnection();
        private static bool _tabelaCriada = false;
        private static readonly object _lock = new object();

        public FornecedorRepository()
        {
            try { CriarTabelaSeNaoExistir(); } catch { }
        }

        private void CriarTabelaSeNaoExistir()
        {
            if (_tabelaCriada) return;
            lock (_lock)
            {
                if (_tabelaCriada) return;

                var result = db.ExecuteScalar("SELECT object_id FROM sys.objects WHERE object_id = OBJECT_ID('Fornecedores') AND type in (N'U')");
                if (result == null || result == DBNull.Value)
                {
                    db.ExecuteNonQuery(@"
                        CREATE TABLE Fornecedores (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            Nome VARCHAR(100) NOT NULL,
                            Cnpj VARCHAR(20),
                            Telefone VARCHAR(20),
                            Email VARCHAR(100),
                            Endereco VARCHAR(MAX),
                            NomeLoja VARCHAR(150),
                            Ativo BIT DEFAULT 1
                        )");

                    // Inserir fornecedor padrão
                    db.ExecuteNonQuery(@"
                        INSERT INTO Fornecedores (Nome, Cnpj, Telefone, Email, Endereco, NomeLoja, Ativo)
                        VALUES ('Fornecedor Geral/Padrão', '00.000.000/0001-00', '(11) 99999-9999', 'contato@fornecedor.com', 'Rua Central, 100', '', 1)");
                }
                else
                {
                    // Tenta adicionar a coluna NomeLoja caso não exista (banco legado)
                    try
                    {
                        db.ExecuteNonQuery("IF COL_LENGTH('Fornecedores', 'NomeLoja') IS NULL ALTER TABLE Fornecedores ADD NomeLoja VARCHAR(150)");
                    }
                    catch { }
                }
                _tabelaCriada = true;
            }
        }

        public async System.Threading.Tasks.Task<List<Fornecedor>> BuscarTodosAsync()
        {
            using (var conn = db.GetConnection())
            {
                string query = "SELECT * FROM Fornecedores WHERE Ativo = 1 ORDER BY Nome ASC";
                var result = await conn.QueryAsync<Fornecedor>(query);
                return result.ToList();
            }
        }

        public Fornecedor BuscarPorId(int id)
        {
            using (var conn = db.GetConnection())
            {
                string query = "SELECT * FROM Fornecedores WHERE Id = @Id";
                return conn.QueryFirstOrDefault<Fornecedor>(query, new { Id = id });
            }
        }

        public void Inserir(Fornecedor f)
        {
            using (var conn = db.GetConnection())
            {
                string query = @"INSERT INTO Fornecedores (Nome, Cnpj, Telefone, Email, Endereco, NomeLoja, Ativo) 
                                 VALUES (@Nome, @Cnpj, @Telefone, @Email, @Endereco, @NomeLoja, 1)";
                conn.Execute(query, f);
            }
        }

        public void Atualizar(Fornecedor f)
        {
            using (var conn = db.GetConnection())
            {
                string query = @"UPDATE Fornecedores SET Nome = @Nome, Cnpj = @Cnpj, Telefone = @Telefone, 
                                 Email = @Email, Endereco = @Endereco, NomeLoja = @NomeLoja WHERE Id = @Id";
                conn.Execute(query, f);
            }
        }

        public void Excluir(int id)
        {
            using (var conn = db.GetConnection())
            {
                string query = "UPDATE Fornecedores SET Ativo = 0 WHERE Id = @Id";
                conn.Execute(query, new { Id = id });
            }
        }
    }
}
