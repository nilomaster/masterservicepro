using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.DAL
{
    public class VendaRepository
    {
        private DbConnection db = new DbConnection();

        private static bool _colunaStatusCriada = false;
        private static readonly object _lock = new object();

        public VendaRepository()
        {
            try { GarantirColunaStatus(); } catch { }
        }

        private void GarantirColunaStatus()
        {
            if (_colunaStatusCriada) return;
            lock (_lock)
            {
                if (_colunaStatusCriada) return;
                try
                {
                    db.ExecuteNonQuery("IF NOT EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Vendas') AND name = 'Status') " +
                                       "ALTER TABLE Vendas ADD Status VARCHAR(50) DEFAULT 'Concluída';");
                    db.ExecuteNonQuery("UPDATE Vendas SET Status = 'Concluída' WHERE Status IS NULL;");
                    _colunaStatusCriada = true;
                }
                catch { }
            }
        }

        public int SalvarVenda(int clienteId, int usuarioId, decimal subtotal, decimal desconto, decimal totalFinal, string formaPagamento, List<VendaItem> itens)
        {
            try
            {
                // 1. Salva o cabeçalho da venda
                string queryVenda = @"INSERT INTO Vendas (ClienteId, UsuarioId, DataVenda, Subtotal, Desconto, TotalFinal, FormaPagamento) 
                                      VALUES (@ClienteId, @UsuarioId, @DataVenda, @Subtotal, @Desconto, @TotalFinal, @FormaPagamento);
                                      SELECT SCOPE_IDENTITY();";

                SqlParameter[] pVenda = {
                    new SqlParameter("@ClienteId", clienteId == 0 ? (object)DBNull.Value : clienteId),
                    new SqlParameter("@UsuarioId", usuarioId),
                    new SqlParameter("@DataVenda", DateTime.Now),
                    new SqlParameter("@Subtotal", subtotal),
                    new SqlParameter("@Desconto", desconto),
                    new SqlParameter("@TotalFinal", totalFinal),
                    new SqlParameter("@FormaPagamento", formaPagamento)
                };

                int vendaId = Convert.ToInt32(db.ExecuteScalar(queryVenda, pVenda));

                // 2. Salva os itens da venda
                foreach (var item in itens)
                {
                    string queryItem = @"INSERT INTO ItensVenda (VendaId, ProdutoId, Quantidade, PrecoUnitario, Subtotal) 
                                         VALUES (@VendaId, @ProdutoId, @Quantidade, @PrecoUnitario, @Subtotal)";
                    
                    SqlParameter[] pItem = {
                        new SqlParameter("@VendaId", vendaId),
                        new SqlParameter("@ProdutoId", item.IdProduto),
                        new SqlParameter("@Quantidade", item.Quantidade),
                        new SqlParameter("@PrecoUnitario", item.PrecoUnitario),
                        new SqlParameter("@Subtotal", item.Total)
                    };

                    db.ExecuteNonQuery(queryItem, pItem);
                }

                new AuditoriaRepository().Registrar("FINALIZAR", "Vendas", $"Registrada venda #{vendaId} no valor total de {totalFinal:C2} via {formaPagamento} com {itens.Count} itens.");

                return vendaId;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao salvar histórico de venda: " + ex.Message);
            }
        }

        public void AtualizarFormaPagamentoVenda(int vendaId, string novaFormaPagamento, int usuarioId)
        {
            DataTable dtVenda = db.ExecuteQuery("SELECT DataVenda, TotalFinal, FormaPagamento FROM Vendas WHERE Id = @Id", new[] { new SqlParameter("@Id", vendaId) });
            if (dtVenda.Rows.Count == 0) throw new Exception("Venda não encontrada.");
            
            decimal totalFinal = Convert.ToDecimal(dtVenda.Rows[0]["TotalFinal"]);
            string formaAtual = dtVenda.Rows[0]["FormaPagamento"].ToString();
            DateTime dataVenda = Convert.ToDateTime(dtVenda.Rows[0]["DataVenda"]);

            if (formaAtual == novaFormaPagamento) return;

            db.ExecuteNonQuery("UPDATE Vendas SET FormaPagamento = @Forma WHERE Id = @Id", new[] {
                new SqlParameter("@Forma", novaFormaPagamento),
                new SqlParameter("@Id", vendaId)
            });

            var finRepo = new FinanceiroRepository();
            finRepo.CorrigirMovimentacaoPorDescricao("Venda finalizada no PDV", totalFinal, novaFormaPagamento, dataVenda);

            new AuditoriaRepository().Registrar("CORREÇÃO", "Vendas", $"Forma de pagamento da Venda #{vendaId} alterada de '{formaAtual}' para '{novaFormaPagamento}' pelo usuário {usuarioId}.");
        }

        public void AtualizarCliente(int vendaId, int novoClienteId, string nomeCliente, int usuarioId)
        {
            db.ExecuteNonQuery("UPDATE Vendas SET ClienteId = @ClienteId WHERE Id = @Id", new[] {
                new SqlParameter("@ClienteId", novoClienteId > 0 ? (object)novoClienteId : DBNull.Value),
                new SqlParameter("@Id", vendaId)
            });

            new AuditoriaRepository().Registrar("CORREÇÃO", "Vendas", $"Cliente da Venda #{vendaId} alterado para '{nomeCliente}' pelo usuário {usuarioId}.");
        }

        public void AtualizarFormaPagamentoVendaMista(int vendaId, string novaFormaPagamento, int usuarioId, decimal troco, decimal mistoDin, decimal mistoPix, decimal mistoCartao, decimal mistoSaldo)
        {
            DataTable dtVenda = db.ExecuteQuery("SELECT DataVenda, TotalFinal, FormaPagamento FROM Vendas WHERE Id = @Id", new[] { new SqlParameter("@Id", vendaId) });
            if (dtVenda.Rows.Count == 0) throw new Exception("Venda não encontrada.");
            
            decimal totalFinal = Convert.ToDecimal(dtVenda.Rows[0]["TotalFinal"]);
            string formaAtual = dtVenda.Rows[0]["FormaPagamento"].ToString();
            DateTime dataVenda = Convert.ToDateTime(dtVenda.Rows[0]["DataVenda"]);

            db.ExecuteNonQuery("UPDATE Vendas SET FormaPagamento = @Forma WHERE Id = @Id", new[] {
                new SqlParameter("@Forma", novaFormaPagamento),
                new SqlParameter("@Id", vendaId)
            });

            var finRepo = new FinanceiroRepository();
            if (novaFormaPagamento == "Misto" || troco > 0)
            {
                finRepo.RecriarMovimentacaoMistaPorDescricao("Venda finalizada no PDV", totalFinal, dataVenda, "Venda", "Venda de Produtos", mistoDin, mistoPix, mistoCartao, mistoSaldo, troco);
            }
            else
            {
                finRepo.CorrigirMovimentacaoPorDescricao("Venda finalizada no PDV", totalFinal, novaFormaPagamento, dataVenda);
            }

            new AuditoriaRepository().Registrar("CORREÇÃO", "Vendas", $"Forma de pagamento da Venda #{vendaId} alterada de '{formaAtual}' para '{novaFormaPagamento}' pelo usuário {usuarioId}. (Misto/Troco atualizado)");
        }

        public System.Data.DataTable GetVendasRelatorio(DateTime inicio, DateTime fim)
        {
            string query = @"
                SELECT V.Id, V.DataVenda, ISNULL(C.Nome, 'Consumidor') as Cliente, 
                       STUFF((SELECT ', ' + CAST(CAST(IV.Quantidade AS INT) AS VARCHAR) + 'x ' + P.Nome
                              FROM ItensVenda IV 
                              INNER JOIN Produtos P ON IV.ProdutoId = P.Id
                              WHERE IV.VendaId = V.Id
                              FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') as ItensVendidos,
                       V.FormaPagamento, V.TotalFinal, ISNULL(V.Status, 'Concluída') as Status
                FROM Vendas V
                LEFT JOIN Clientes C ON V.ClienteId = C.Id
                WHERE V.DataVenda BETWEEN @inicio AND @fim
                ORDER BY V.DataVenda DESC";
            
            SqlParameter[] p = {
                new SqlParameter("@inicio", inicio.Date),
                new SqlParameter("@fim", fim.Date.AddDays(1).AddSeconds(-1))
            };
            return db.ExecuteQuery(query, p);
        }

        public System.Data.DataTable BuscarPorId(int id)
        {
            string query = "SELECT * FROM Vendas WHERE Id = @Id";
            return db.ExecuteQuery(query, new[] { new SqlParameter("@Id", id) });
        }

        public System.Data.DataTable BuscarItensVenda(int vendaId)
        {
            string query = @"SELECT IV.Id, IV.ProdutoId, P.Nome as Produto, IV.Quantidade, IV.PrecoUnitario, IV.Subtotal 
                             FROM ItensVenda IV 
                             INNER JOIN Produtos P ON IV.ProdutoId = P.Id 
                             WHERE IV.VendaId = @Id";
            return db.ExecuteQuery(query, new[] { new SqlParameter("@Id", vendaId) });
        }

        public void TrocarItemVenda(int vendaId, int idItemDevolvido, int idNovoProduto, decimal precoNovo, string formaPagamentoDiferenca, decimal diferenca, bool adicionarSaldo, int caixaId, int usuarioId)
        {
            // 1. Obter informações da Venda Original
            var dtVenda = BuscarPorId(vendaId);
            if (dtVenda.Rows.Count == 0) throw new Exception("Venda não encontrada.");
            decimal totalAtual = Convert.ToDecimal(dtVenda.Rows[0]["TotalFinal"]);
            object clienteIdObj = dtVenda.Rows[0]["ClienteId"];
            
            // 2. Obter informações do Item Devolvido
            string qItemDev = "SELECT ProdutoId, Quantidade, PrecoUnitario, Subtotal FROM ItensVenda WHERE Id = @IdItem";
            var dtItemDev = db.ExecuteQuery(qItemDev, new[] { new SqlParameter("@IdItem", idItemDevolvido) });
            if (dtItemDev.Rows.Count == 0) throw new Exception("Item devolvido não encontrado na venda.");
            
            int produtoDevolvidoId = Convert.ToInt32(dtItemDev.Rows[0]["ProdutoId"]);
            decimal precoDevolvido = Convert.ToDecimal(dtItemDev.Rows[0]["PrecoUnitario"]);
            decimal qtdTotalDoItem = Convert.ToDecimal(dtItemDev.Rows[0]["Quantidade"]);

            // 3. Devolver Produto ao Estoque
            db.ExecuteNonQuery("UPDATE Produtos SET Estoque = ISNULL(Estoque, 0) + 1 WHERE Id = @Id", new[] { new SqlParameter("@Id", produtoDevolvidoId) });

            // 4. Atualizar o ItensVenda (remover 1 qtd ou deletar se qtd == 1)
            if (qtdTotalDoItem <= 1)
            {
                db.ExecuteNonQuery("DELETE FROM ItensVenda WHERE Id = @IdItem", new[] { new SqlParameter("@IdItem", idItemDevolvido) });
            }
            else
            {
                db.ExecuteNonQuery("UPDATE ItensVenda SET Quantidade = Quantidade - 1, Subtotal = Subtotal - PrecoUnitario WHERE Id = @IdItem", new[] { new SqlParameter("@IdItem", idItemDevolvido) });
            }

            // 5. Retirar Novo Produto do Estoque
            db.ExecuteNonQuery("UPDATE Produtos SET Estoque = ISNULL(Estoque, 0) - 1 WHERE Id = @Id", new[] { new SqlParameter("@Id", idNovoProduto) });

            // 6. Inserir Novo Produto no ItensVenda
            string insertItem = @"INSERT INTO ItensVenda (VendaId, ProdutoId, Quantidade, PrecoUnitario, Subtotal) 
                                  VALUES (@VendaId, @ProdutoId, 1, @PrecoUnitario, @PrecoUnitario)";
            db.ExecuteNonQuery(insertItem, new[] {
                new SqlParameter("@VendaId", vendaId),
                new SqlParameter("@ProdutoId", idNovoProduto),
                new SqlParameter("@PrecoUnitario", precoNovo)
            });

            // 7. Atualizar Venda Total
            decimal novoTotal = totalAtual + diferenca;
            db.ExecuteNonQuery("UPDATE Vendas SET TotalFinal = @Total WHERE Id = @IdVenda", new[] {
                new SqlParameter("@Total", novoTotal),
                new SqlParameter("@IdVenda", vendaId)
            });

            // 8. Financeiro
            var finRepo = new FinanceiroRepository();
            if (diferenca > 0)
            {
                // Cliente pagou a mais (Entrada)
                finRepo.LançarMovimentacao(new Movimentacao {
                    IdCaixa = caixaId,
                    Tipo = "Entrada",
                    Categoria = "Venda",
                    Subcategoria = "Complemento Troca",
                    Valor = diferenca,
                    Descricao = $"Complemento ref. Troca de Venda #{vendaId}",
                    FormaPagamento = formaPagamentoDiferenca
                });
            }
            else if (diferenca < 0)
            {
                decimal valorADevolver = Math.Abs(diferenca);
                if (adicionarSaldo && clienteIdObj != DBNull.Value && Convert.ToInt32(clienteIdObj) > 0)
                {
                    // Dar saldo ao cliente
                    var clienteRepo = new ClienteRepository();
                    clienteRepo.AdicionarSaldo(Convert.ToInt32(clienteIdObj), valorADevolver);
                }
                else
                {
                    // Dar troco (Sangria/Saída)
                    finRepo.LançarMovimentacao(new Movimentacao {
                        IdCaixa = caixaId,
                        Tipo = "Saída",
                        Categoria = "Devolução",
                        Subcategoria = "Troco Troca",
                        Valor = valorADevolver,
                        Descricao = $"Troco ref. Troca de Venda #{vendaId}",
                        FormaPagamento = "Dinheiro" // Troco físico normalmente
                    });
                }
            }

            new AuditoriaRepository().Registrar("CORREÇÃO", "Vendas", $"Troca realizada na Venda #{vendaId}: Produto {produtoDevolvidoId} trocado por {idNovoProduto}. Diferença: {diferenca:C2}. Usuário: {usuarioId}");
        }

        public void EstornarVenda(int vendaId, string motivo, int caixaId, string destinoEstorno = "SANGRIA", string formaPagamentoDevolucao = "Dinheiro")
        {
            // 1. Marcar como Cancelada
            string updateStatus = "UPDATE Vendas SET Status = 'Cancelada' WHERE Id = @Id AND Status <> 'Cancelada'";
            int affected = db.ExecuteNonQuery(updateStatus, new[] { new SqlParameter("@Id", vendaId) });
            if (affected == 0) throw new Exception("Venda já foi cancelada ou não encontrada.");

            // 2. Devolver Produtos ao Estoque
            System.Data.DataTable dtItens = db.ExecuteQuery("SELECT ProdutoId, Quantidade FROM ItensVenda WHERE VendaId = @Id", new[] { new SqlParameter("@Id", vendaId) });
            foreach (System.Data.DataRow row in dtItens.Rows)
            {
                int pId = Convert.ToInt32(row["ProdutoId"]);
                decimal qtd = Convert.ToDecimal(row["Quantidade"]);
                db.ExecuteNonQuery("UPDATE Produtos SET Estoque = ISNULL(Estoque, 0) + @Qtd WHERE Id = @Id", new[] { new SqlParameter("@Qtd", qtd), new SqlParameter("@Id", pId) });
            }

            // 3. Lidar com o Financeiro (Saldo do Cliente ou Sangria)
            System.Data.DataTable dtVenda = db.ExecuteQuery("SELECT TotalFinal, ClienteId FROM Vendas WHERE Id = @Id", new[] { new SqlParameter("@Id", vendaId) });
            if (dtVenda.Rows.Count == 0) return;
            
            decimal total = Convert.ToDecimal(dtVenda.Rows[0]["TotalFinal"]);
            object clienteIdObj = dtVenda.Rows[0]["ClienteId"];

            if (destinoEstorno == "SALDO" && clienteIdObj != null && clienteIdObj != DBNull.Value && Convert.ToInt32(clienteIdObj) > 0)
            {
                // Cliente cadastrado: O valor volta como SALDO
                int clienteId = Convert.ToInt32(clienteIdObj);
                var clienteRepo = new ClienteRepository();
                clienteRepo.AdicionarSaldo(clienteId, total);

                new AuditoriaRepository().Registrar("ESTORNO", "Vendas", $"A Venda #{vendaId} (Valor: {total:C2}) foi estornada. Valor convertido em SALDO para o cliente. Motivo: {motivo}");
            }
            else if (destinoEstorno == "ERRO_LANCAMENTO")
            {
                // Apenas cancelamento: não gera movimentação de saída financeira
                new AuditoriaRepository().Registrar("ESTORNO", "Vendas", $"A Venda #{vendaId} (Valor: {total:C2}) foi cancelada por erro de lançamento. Nenhum dinheiro foi retirado do caixa. Motivo: {motivo}");
            }
            else
            {
                // Cliente Avulso ou escolha manual de Sangria: O valor vira Sangria/Saída do Caixa
                var finRepo = new FinanceiroRepository();
                finRepo.LançarMovimentacao(new Movimentacao {
                    IdCaixa = caixaId,
                    Tipo = "Saída",
                    Categoria = "Estorno de Venda",
                    Subcategoria = "Devolução",
                    Valor = total,
                    Descricao = $"Estorno da Venda #{vendaId}. Motivo: {motivo}",
                    FormaPagamento = formaPagamentoDevolucao
                });

                new AuditoriaRepository().Registrar("ESTORNO", "Vendas", $"A Venda #{vendaId} (Valor: {total:C2}) foi estornada. Valor devolvido via sangria em {formaPagamentoDevolucao}. Motivo: {motivo}");
            }
        }
        public System.Data.DataTable GetFaturamentoPorCategoria(DateTime inicio, DateTime fim)
        {
            // O relatório agrupa as vendas por categoria de produto (Acessório, Peça, etc) e soma a Mão de Obra das OS.
            string query = @"
                -- 1. Faturamento do PDV (Vendas) Agrupado por Categoria do Produto
                SELECT 
                    ISNULL(C.Nome, 'Sem Categoria') AS Categoria,
                    SUM(IV.Subtotal) AS Total
                FROM ItensVenda IV
                INNER JOIN Vendas V ON IV.VendaId = V.Id
                INNER JOIN Produtos P ON IV.ProdutoId = P.Id
                LEFT JOIN Categorias C ON P.IdCategoria = C.Id
                WHERE V.Status = 'Concluída' 
                  AND V.DataVenda BETWEEN @inicio AND @fim
                GROUP BY C.Nome

                UNION ALL

                -- 2. Serviços / Mão de Obra de OS
                SELECT 
                    'Mão de Obra / Serviços (OS)' AS Categoria,
                    SUM(O.ValorServico) AS Total
                FROM OrdensServico O
                WHERE (O.Status = 'Entregue' OR O.Status = 'Finalizada' OR O.Faturado = 1)
                  AND O.DataAtualizacao BETWEEN @inicio AND @fim
                  AND O.ValorServico > 0

                UNION ALL

                -- 3. Peças usadas em OS (Caso as peças da OS não baixem pelo PDV, e sim pela própria OS)
                -- Se a loja usa ValorPecas na OS, podemos agrupar tudo como Peças (OS).
                SELECT 
                    'Peças (Usadas na OS)' AS Categoria,
                    SUM(O.ValorPecas) AS Total
                FROM OrdensServico O
                WHERE (O.Status = 'Entregue' OR O.Status = 'Finalizada' OR O.Faturado = 1)
                  AND O.DataAtualizacao BETWEEN @inicio AND @fim
                  AND O.ValorPecas > 0
            ";
            
            SqlParameter[] p = {
                new SqlParameter("@inicio", inicio.Date),
                new SqlParameter("@fim", fim.Date.AddDays(1).AddSeconds(-1))
            };
            
            // Como usamos UNION ALL, pode haver categorias repetidas se por algum motivo houver (embora aqui não ocorra)
            // Mas vamos encapsular em um SELECT final para ordenar
            string finalQuery = $@"
                SELECT Categoria, SUM(Total) AS Total 
                FROM ({query}) AS T 
                GROUP BY Categoria 
                ORDER BY Total DESC";

            return db.ExecuteQuery(finalQuery, p);
        }
        public System.Data.DataTable GetFaturamentoDetalhadoPorCategoria(DateTime inicio, DateTime fim)
        {
            string query = @"
                -- 1. Vendas do PDV (Produtos avulsos)
                SELECT 
                    V.DataVenda AS Data,
                    ISNULL(C.Nome, 'Sem Categoria') AS Categoria,
                    CAST(CAST(IV.Quantidade AS INT) AS VARCHAR) + 'x ' + P.Nome AS Descricao,
                    (ISNULL(P.PrecoCusto, 0) * IV.Quantidade) AS Custo,
                    IV.Subtotal AS Faturamento,
                    (IV.Subtotal - (ISNULL(P.PrecoCusto, 0) * IV.Quantidade)) AS Lucro
                FROM ItensVenda IV
                INNER JOIN Vendas V ON IV.VendaId = V.Id
                INNER JOIN Produtos P ON IV.ProdutoId = P.Id
                LEFT JOIN Categorias C ON P.IdCategoria = C.Id
                WHERE V.Status = 'Concluída' 
                  AND V.DataVenda BETWEEN @inicio AND @fim

                UNION ALL

                -- 2. Serviços / Mão de Obra em OS
                SELECT 
                    O.DataAtualizacao AS Data,
                    'Mão de Obra / Serviços (OS)' AS Categoria,
                    'OS #' + O.NumeroOS + ' - ' + ISNULL(NULLIF(O.Defeito, ''), 'Serviço') AS Descricao,
                    0 AS Custo,
                    O.ValorServico AS Faturamento,
                    O.ValorServico AS Lucro
                FROM OrdensServico O
                WHERE (O.Status = 'Finalizado' OR O.Status = 'Entregue' OR O.Faturado = 1)
                  AND O.DataAtualizacao BETWEEN @inicio AND @fim
                  AND ISNULL(O.ValorServico, 0) > 0

                UNION ALL

                -- 3. Peças usadas em OS (Detalhado item a item)
                SELECT 
                    O.DataAtualizacao AS Data,
                    ISNULL(C.Nome, 'Peças (Usadas na OS)') AS Categoria,
                    'OS #' + O.NumeroOS + ' | ' + CAST(CAST(OSI.Quantidade AS INT) AS VARCHAR) + 'x ' + ISNULL(P.Nome, OSI.NomeProduto) AS Descricao,
                    (ISNULL(P.PrecoCusto, 0) * OSI.Quantidade) AS Custo,
                    OSI.SubTotal AS Faturamento,
                    (OSI.SubTotal - (ISNULL(P.PrecoCusto, 0) * OSI.Quantidade)) AS Lucro
                FROM OrdemServicoItens OSI
                INNER JOIN OrdensServico O ON OSI.OrdemServicoId = O.Id
                LEFT JOIN Produtos P ON OSI.ProdutoId = P.Id
                LEFT JOIN Categorias C ON P.IdCategoria = C.Id
                WHERE (O.Status = 'Finalizado' OR O.Status = 'Entregue' OR O.Faturado = 1)
                  AND O.DataAtualizacao BETWEEN @inicio AND @fim
                  AND ISNULL(OSI.SubTotal, 0) > 0
            ";
            
            SqlParameter[] p = {
                new SqlParameter("@inicio", inicio.Date),
                new SqlParameter("@fim", fim.Date.AddDays(1).AddSeconds(-1))
            };
            
            string finalQuery = $@"
                SELECT *
                FROM ({query}) AS T 
                ORDER BY Data DESC";

            return db.ExecuteQuery(finalQuery, p);
        }
    }
}
