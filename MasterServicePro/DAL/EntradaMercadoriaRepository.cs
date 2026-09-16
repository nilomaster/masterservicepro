using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.DAL
{
    public class EntradaMercadoriaRepository
    {
        private DbConnection db = new DbConnection();
        private static bool _tabelasCriadas = false;
        private static readonly object _lock = new object();

        public EntradaMercadoriaRepository()
        {
            try { CriarTabelasSeNaoExistirem(); } catch { }
        }

        private void CriarTabelasSeNaoExistirem()
        {
            if (_tabelasCriadas) return;
            lock (_lock)
            {
                if (_tabelasCriadas) return;

                var resultEntrada = db.ExecuteScalar("SELECT object_id FROM sys.objects WHERE object_id = OBJECT_ID('EntradasMercadoria') AND type in (N'U')");
                if (resultEntrada == null || resultEntrada == DBNull.Value)
                {
                    db.ExecuteNonQuery(@"
                        CREATE TABLE EntradasMercadoria (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            IdFornecedor INT,
                            NumeroNota VARCHAR(50),
                            DataEntrada DATETIME DEFAULT GETDATE(),
                            ValorTotal DECIMAL(18,2) DEFAULT 0,
                            Observacao VARCHAR(MAX)
                        )");
                }

                var resultItens = db.ExecuteScalar("SELECT object_id FROM sys.objects WHERE object_id = OBJECT_ID('EntradaMercadoriaItens') AND type in (N'U')");
                if (resultItens == null || resultItens == DBNull.Value)
                {
                    db.ExecuteNonQuery(@"
                        CREATE TABLE EntradaMercadoriaItens (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            IdEntrada INT NOT NULL,
                            IdProduto INT NOT NULL,
                            Quantidade INT NOT NULL,
                            PrecoCusto DECIMAL(18,2) NOT NULL,
                            SubTotal DECIMAL(18,2) NOT NULL
                        )");
                }
                _tabelasCriadas = true;
            }
        }

        public DataTable ListarTodas()
        {
            string query = @"
                SELECT E.Id, E.DataEntrada, F.Nome as Fornecedor, E.NumeroNota, E.ValorTotal, E.Observacao
                FROM EntradasMercadoria E
                LEFT JOIN Fornecedores F ON E.IdFornecedor = F.Id
                ORDER BY E.DataEntrada DESC";
            return db.ExecuteQuery(query);
        }

        public void RegistrarEntrada(EntradaMercadoria entrada)
        {
            using (var conn = db.GetConnection())
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Inserir cabeçalho
                        string insertCabecalho = @"
                            INSERT INTO EntradasMercadoria (IdFornecedor, NumeroNota, DataEntrada, ValorTotal, Observacao)
                            VALUES (@IdFornecedor, @NumeroNota, @DataEntrada, @ValorTotal, @Observacao);
                            SELECT SCOPE_IDENTITY();";
                        
                        int entradaId = Convert.ToInt32(conn.ExecuteScalar(insertCabecalho, entrada, trans));
                        entrada.Id = entradaId;

                        var prodRepo = new ProdutoRepository();

                        // 2. Inserir itens e atualizar estoque/custo de cada produto
                        foreach (var item in entrada.Itens)
                        {
                            item.IdEntrada = entradaId;
                            
                            string insertItem = @"
                                INSERT INTO EntradaMercadoriaItens (IdEntrada, IdProduto, Quantidade, PrecoCusto, SubTotal)
                                VALUES (@IdEntrada, @IdProduto, @Quantidade, @PrecoCusto, @SubTotal)";
                            
                            conn.Execute(insertItem, item, trans);

                            // Mapeia e atualiza o produto para recalcular o custo médio
                            var prod = prodRepo.BuscarPorId(item.IdProduto);
                            if (prod != null)
                            {
                                int estoqueAtual = prod.Estoque;
                                decimal custoAtual = prod.PrecoCusto;
                                int qtdNova = item.Quantidade;
                                decimal custoNovo = item.PrecoCusto;

                                // Recálculo do Custo Médio
                                decimal novoCusto = custoNovo;
                                if (estoqueAtual > 0)
                                {
                                    novoCusto = ((estoqueAtual * custoAtual) + (qtdNova * custoNovo)) / (estoqueAtual + qtdNova);
                                }

                                int novoEstoque = estoqueAtual + qtdNova;

                                // Grava novos dados do produto
                                string updateProd = "UPDATE Produtos SET Estoque = @Estoque, PrecoCusto = @PrecoCusto, DataAtualizacao = GETDATE() WHERE Id = @Id";
                                conn.Execute(updateProd, new { Estoque = novoEstoque, PrecoCusto = novoCusto, Id = prod.Id }, trans);
                            }
                        }

                        trans.Commit();
                        new AuditoriaRepository().Registrar("ENTRADA_ESTOQUE", "Produtos", $"Entrada de estoque registrada (ID: {entradaId}, Nota: {entrada.NumeroNota}, Valor Total: {entrada.ValorTotal:C2}, Itens: {entrada.Itens.Count}).");
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
