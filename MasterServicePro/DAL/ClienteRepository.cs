using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.DAL
{
    public class ClienteRepository
    {
        private DbConnection db = new DbConnection();

        private static bool _tabelaCriada = false;
        private static readonly object _lock = new object();

        public ClienteRepository()
        {
            try { CriarTabelaSeNaoExistir(); } catch { }
        }

        private void CriarTabelaSeNaoExistir()
        {
            if (_tabelaCriada) return;
            lock (_lock)
            {
                if (_tabelaCriada) return;

                // Garante que a tabela Clientes existe com todas as colunas necessárias
                var result = db.ExecuteScalar("SELECT object_id FROM sys.objects WHERE object_id = OBJECT_ID('Clientes') AND type in (N'U')");
                 if (result == null || result == DBNull.Value)
                {
                    db.ExecuteNonQuery(@"
                        CREATE TABLE Clientes (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            Nome VARCHAR(100) NOT NULL,
                            CpfCnpj VARCHAR(20),
                            Telefone VARCHAR(20),
                            WhatsApp VARCHAR(20),
                            Email VARCHAR(100),
                            Endereco VARCHAR(MAX),
                            Historico VARCHAR(MAX),
                            TipoCliente VARCHAR(50),
                            DataCadastro DATETIME DEFAULT GETDATE(),
                            DataAtualizacao DATETIME DEFAULT GETDATE(),
                            Ativo BIT DEFAULT 1,
                            Saldo DECIMAL(18,2) DEFAULT 0
                        )");
                }
                else
                {
                    // Migração: Garante que as colunas novas existem
                    string[] colunas = { 
                        "WhatsApp VARCHAR(20)", 
                        "Email VARCHAR(100)", 
                        "Endereco VARCHAR(MAX)", 
                        "Historico VARCHAR(MAX)",
                        "TipoCliente VARCHAR(50)",
                        "DataCadastro DATETIME DEFAULT GETDATE()",
                        "DataAtualizacao DATETIME DEFAULT GETDATE()",
                        "Ativo BIT DEFAULT 1",
                        "Saldo DECIMAL(18,2) DEFAULT 0"
                    };

                    foreach (var col in colunas)
                    {
                        string nomeCol = col.Split(' ')[0];
                        string checkCol = $"IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Clientes') AND name = '{nomeCol}') " +
                                         $"ALTER TABLE Clientes ADD {col};";
                        db.ExecuteNonQuery(checkCol);
                    }
                }
                _tabelaCriada = true;
            }
        }

        public async System.Threading.Tasks.Task<List<Cliente>> BuscarTodosAsync()
        {
            using (var conn = db.GetConnection())
            {
                string query = "SELECT * FROM Clientes WHERE Ativo = 1 OR Ativo IS NULL ORDER BY Nome ASC";
                var result = await conn.QueryAsync<Cliente>(query);
                return result.ToList();
            }
        }

        public Cliente BuscarPorId(int id)
        {
            using (var conn = db.GetConnection())
            {
                string query = "SELECT * FROM Clientes WHERE Id = @Id";
                return conn.QueryFirstOrDefault<Cliente>(query, new { Id = id });
            }
        }

        public void Inserir(Cliente c)
        {
            using (var conn = db.GetConnection())
            {
                string query = @"INSERT INTO Clientes (Nome, CpfCnpj, Telefone, WhatsApp, Email, Endereco, Historico, TipoCliente, Saldo) 
                                 VALUES (@Nome, @CpfCnpj, @Telefone, @WhatsApp, @Email, @Endereco, @Historico, 'Fisica', @Saldo)";
                conn.Execute(query, c);
            }
        }

        public void Atualizar(Cliente c)
        {
            using (var conn = db.GetConnection())
            {
                string query = @"UPDATE Clientes SET Nome = @Nome, CpfCnpj = @CpfCnpj, Telefone = @Telefone, 
                                 WhatsApp = @WhatsApp, Email = @Email, Endereco = @Endereco, Historico = @Historico, 
                                 TipoCliente = ISNULL(TipoCliente, 'Fisica'), Saldo = @Saldo,
                                 DataAtualizacao = GETDATE() WHERE Id = @Id";
                conn.Execute(query, c);
            }
        }

        public void Excluir(int id)
        {
            using (var conn = db.GetConnection())
            {
                string query = "UPDATE Clientes SET Ativo = 0, DataAtualizacao = GETDATE() WHERE Id = @Id";
                conn.Execute(query, new { Id = id });
            }
        }

        public DataTable GetHistoricoOS(int idCliente)
        {
            string query = "SELECT Id, Marca, Modelo, Defeito, Status, ValorTotal, DataAbertura FROM OrdensServico WHERE IdCliente = @Id ORDER BY DataAbertura DESC";
            SqlParameter[] p = { new SqlParameter("@Id", idCliente) };
            try { return db.ExecuteQuery(query, p); } catch { return new DataTable(); }
        }

        public DataTable GetHistoricoVendas(int idCliente)
        {
            string query = "SELECT Id, DataVenda, TotalFinal, FormaPagamento FROM Vendas WHERE ClienteId = @Id ORDER BY DataVenda DESC";
            SqlParameter[] p = { new SqlParameter("@Id", idCliente) };
            try { return db.ExecuteQuery(query, p); } catch { return new DataTable(); }
        }

        public void AdicionarSaldo(int idCliente, decimal valor)
        {
            using (var conn = db.GetConnection())
            {
                string query = "UPDATE Clientes SET Saldo = ISNULL(Saldo, 0) + @Valor, DataAtualizacao = GETDATE() WHERE Id = @Id";
                conn.Execute(query, new { Id = idCliente, Valor = valor });
            }
        }

        public void DescontarSaldo(int idCliente, decimal valor)
        {
            using (var conn = db.GetConnection())
            {
                string query = "UPDATE Clientes SET Saldo = ISNULL(Saldo, 0) - @Valor, DataAtualizacao = GETDATE() WHERE Id = @Id";
                conn.Execute(query, new { Id = idCliente, Valor = valor });
            }
        }
    }
}
