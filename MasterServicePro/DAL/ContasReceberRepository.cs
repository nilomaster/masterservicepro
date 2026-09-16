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
    public class ContasReceberRepository
    {
        private DbConnection db = new DbConnection();
        private static bool _tabelaCriada = false;
        private static readonly object _lock = new object();

        public ContasReceberRepository()
        {
            try { CriarTabelaSeNaoExistir(); } catch { }
        }

        private void CriarTabelaSeNaoExistir()
        {
            if (_tabelaCriada) return;
            lock (_lock)
            {
                if (_tabelaCriada) return;

                var result = db.ExecuteScalar("SELECT object_id FROM sys.objects WHERE object_id = OBJECT_ID('ContasReceber') AND type in (N'U')");
                if (result == null || result == DBNull.Value)
                {
                    db.ExecuteNonQuery(@"
                        CREATE TABLE ContasReceber (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            IdCliente INT NOT NULL,
                            Descricao VARCHAR(255),
                            ValorTotal DECIMAL(18,2) NOT NULL,
                            ValorPago DECIMAL(18,2) DEFAULT 0,
                            DataLancamento DATETIME DEFAULT GETDATE(),
                            DataVencimento DATETIME,
                            Status VARCHAR(50) DEFAULT 'Pendente'
                        )");
                }
                _tabelaCriada = true;
            }
        }

        public void Inserir(ContaReceber conta)
        {
            string query = @"INSERT INTO ContasReceber (IdCliente, Descricao, ValorTotal, ValorPago, DataLancamento, DataVencimento, Status)
                             VALUES (@IdCliente, @Descricao, @ValorTotal, @ValorPago, @DataLancamento, @DataVencimento, @Status);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";
            
            using (var conn = db.GetConnection())
            {
                int id = conn.QuerySingle<int>(query, conta);
                conta.Id = id;
            }
            new AuditoriaRepository().Registrar("CRIAR", "ContasReceber", $"Lançada conta a receber para o cliente ID {conta.IdCliente}. Valor: {conta.ValorTotal:C2}.");
        }

        public void AtualizarStatusAtrasados()
        {
            string query = "UPDATE ContasReceber SET Status = 'Atrasado' WHERE Status = 'Pendente' AND DataVencimento < GETDATE()";
            db.ExecuteNonQuery(query);
        }

        public List<ContaReceber> Listar(int? idCliente = null, string status = null)
        {
            AtualizarStatusAtrasados();

            string query = @"
                SELECT CR.*, C.Nome as ClienteNome, (CR.ValorTotal - CR.ValorPago) as ValorRestante
                FROM ContasReceber CR
                INNER JOIN Clientes C ON CR.IdCliente = C.Id
                WHERE 1=1";

            var param = new DynamicParameters();

            if (idCliente.HasValue && idCliente.Value > 0)
            {
                query += " AND CR.IdCliente = @IdCliente";
                param.Add("@IdCliente", idCliente.Value);
            }

            if (!string.IsNullOrEmpty(status) && status != "Todos")
            {
                query += " AND CR.Status = @Status";
                param.Add("@Status", status);
            }

            query += " ORDER BY CR.DataVencimento ASC";

            using (var conn = db.GetConnection())
            {
                return conn.Query<ContaReceber>(query, param).ToList();
            }
        }

        public void DarBaixa(int idConta, decimal valorPago, string formaPagamento, int idCaixa)
        {
            using (var conn = db.GetConnection())
            {
                var conta = conn.QueryFirstOrDefault<ContaReceber>("SELECT * FROM ContasReceber WHERE Id = @Id", new { Id = idConta });
                if (conta == null) return;

                decimal novoPago = conta.ValorPago + valorPago;
                string novoStatus = "Pendente";

                if (novoPago >= conta.ValorTotal)
                {
                    novoPago = conta.ValorTotal;
                    novoStatus = "Pago";
                }
                else if (conta.DataVencimento < DateTime.Now)
                {
                    novoStatus = "Atrasado";
                }

                conn.Execute("UPDATE ContasReceber SET ValorPago = @ValorPago, Status = @Status WHERE Id = @Id", 
                    new { ValorPago = novoPago, Status = novoStatus, Id = idConta });

                // Lança entrada financeira no caixa
                var finRepo = new FinanceiroRepository();
                finRepo.LançarMovimentacao(new Movimentacao
                {
                    IdCaixa = idCaixa,
                    Tipo = "Entrada",
                    Categoria = "Recebimento Fiado",
                    Subcategoria = "Recebimento Fiado",
                    Valor = valorPago,
                    FormaPagamento = formaPagamento,
                    Descricao = $"Recebimento parcial/total de débito. Ref ID #{idConta}"
                });

                new AuditoriaRepository().Registrar("ATUALIZAR", "ContasReceber", $"Recebimento de {valorPago:C2} registrado para a conta #{idConta} (Novo Saldo Pago: {novoPago:C2}).");
            }
        }

        public ContaReceber BuscarPorId(int id)
        {
            string query = @"
                SELECT CR.*, C.Nome as ClienteNome, (CR.ValorTotal - CR.ValorPago) as ValorRestante
                FROM ContasReceber CR
                INNER JOIN Clientes C ON CR.IdCliente = C.Id
                WHERE CR.Id = @Id";

            using (var conn = db.GetConnection())
            {
                return conn.QueryFirstOrDefault<ContaReceber>(query, new { Id = id });
            }
        }
    }
}
