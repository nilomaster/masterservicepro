using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.DAL
{
    public class FinanceiroRepository
    {
        private DbConnection db = new DbConnection();

        private static bool _tabelasCriadas = false;
        private static readonly object _lock = new object();

        public FinanceiroRepository()
        {
            try { CriarTabelasSeNaoExistirem(); } catch { }
        }

        private void CriarTabelasSeNaoExistirem()
        {
            if (_tabelasCriadas) return;
            lock (_lock)
            {
                if (_tabelasCriadas) return;

                try
                {
                    // Garante que a tabela Financeiro existe
                    var resultFinanceiro = db.ExecuteScalar("SELECT object_id FROM sys.objects WHERE object_id = OBJECT_ID('Financeiro') AND type in (N'U')");
                    if (resultFinanceiro == null || resultFinanceiro == DBNull.Value)
                    {
                        db.ExecuteNonQuery(@"
                            CREATE TABLE Financeiro (
                                Id INT PRIMARY KEY IDENTITY(1,1),
                                IdCaixa INT,
                                Tipo VARCHAR(20),
                                Categoria VARCHAR(50),
                                Subcategoria VARCHAR(50) NULL,
                                Valor DECIMAL(18,2) NOT NULL,
                                FormaPagamento VARCHAR(30),
                                Descricao VARCHAR(MAX),
                                Data DATETIME DEFAULT GETDATE()
                            )");
                    }
                    else
                    {
                        string checkSubcol = "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Financeiro') AND name = 'Subcategoria') " +
                                             "ALTER TABLE Financeiro ADD Subcategoria VARCHAR(50) NULL;";
                        db.ExecuteNonQuery(checkSubcol);
                    }

                    // Garante que a tabela Caixa existe
                    var resultCaixa = db.ExecuteScalar("SELECT object_id FROM sys.objects WHERE object_id = OBJECT_ID('Caixa') AND type in (N'U')");
                    if (resultCaixa == null || resultCaixa == DBNull.Value)
                    {
                        db.ExecuteNonQuery(@"
                            CREATE TABLE Caixa (
                                Id INT PRIMARY KEY IDENTITY(1,1),
                                DataAbertura DATETIME NOT NULL DEFAULT GETDATE(),
                                DataFechamento DATETIME NULL,
                                ValorAbertura DECIMAL(18,2) NOT NULL DEFAULT 0,
                                ValorFechamento DECIMAL(18,2) NULL,
                                Status VARCHAR(20) DEFAULT 'Aberto',
                                Observacao VARCHAR(MAX)
                            )");
                    }
                    else
                    {
                        // Se a tabela existe, garante que as colunas novas também existem (Migração)
                        string[] colunas = { 
                            "DataAbertura DATETIME NOT NULL DEFAULT GETDATE()",
                            "ValorAbertura DECIMAL(18,2) NOT NULL DEFAULT 0",
                            "DataFechamento DATETIME NULL",
                            "ValorFechamento DECIMAL(18,2) NULL",
                            "Status VARCHAR(20) DEFAULT 'Aberto'",
                            "Observacao VARCHAR(MAX)"
                        };

                        foreach (var col in colunas)
                        {
                            string nomeCol = col.Split(' ')[0];
                            string checkCol = $"IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Caixa') AND name = '{nomeCol}') " +
                                             $"ALTER TABLE Caixa ADD {col};";
                            db.ExecuteNonQuery(checkCol);
                        }

                        // Correção específica: Se existirem colunas extras (como 'Tipo', 'Valor', 'UsuarioAberturaId') na tabela Caixa
                        // que não deveriam estar lá, vamos torná-las NULLABLE para não quebrar o INSERT.
                        string[] colunasExtras = { "Tipo", "Valor", "Descricao", "Data", "Categoria", "UsuarioAberturaId", "UsuarioFechamentoId" };
                        foreach (var extra in colunasExtras)
                        {
                            string fixExtra = $"IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Caixa') AND name = '{extra}') " +
                                              $"ALTER TABLE Caixa ALTER COLUMN {extra} VARCHAR(MAX) NULL;";
                            
                            if (extra == "Valor")
                            {
                                fixExtra = "IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Caixa') AND name = 'Valor') " +
                                           "ALTER TABLE Caixa ALTER COLUMN Valor DECIMAL(18,2) NULL;";
                            }
                            else if (extra == "UsuarioAberturaId" || extra == "UsuarioFechamentoId")
                            {
                                fixExtra = $"IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Caixa') AND name = '{extra}') " +
                                           $"ALTER TABLE Caixa ALTER COLUMN {extra} INT NULL;";
                            }
                            
                            db.ExecuteNonQuery(fixExtra);
                        }
                    }

                    // Correção de dados antigos: Padroniza as strings de forma de pagamento
                    string fixDataVendas = @"
                        UPDATE Vendas SET FormaPagamento = 'Cartão de Crédito' WHERE FormaPagamento = 'cartao_credito';
                        UPDATE Vendas SET FormaPagamento = 'Cartão de Débito' WHERE FormaPagamento = 'cartao_debito';
                        UPDATE Vendas SET FormaPagamento = 'Dinheiro' WHERE FormaPagamento = 'dinheiro';
                        UPDATE Vendas SET FormaPagamento = 'Pix' WHERE FormaPagamento = 'pix';
                    ";
                    db.ExecuteNonQuery(fixDataVendas);

                    string fixDataFinanceiro = @"
                        UPDATE Financeiro SET FormaPagamento = 'Cartão de Crédito' WHERE FormaPagamento = 'cartao_credito';
                        UPDATE Financeiro SET FormaPagamento = 'Cartão de Débito' WHERE FormaPagamento = 'cartao_debito';
                        UPDATE Financeiro SET FormaPagamento = 'Dinheiro' WHERE FormaPagamento = 'dinheiro';
                        UPDATE Financeiro SET FormaPagamento = 'Pix' WHERE FormaPagamento = 'pix';
                    ";
                    db.ExecuteNonQuery(fixDataFinanceiro);

                    _tabelasCriadas = true;
                }
                catch { }
            }
        }

        // --- Gestão de Caixa ---

        public Caixa ObterCaixaAberto()
        {
            string query = "SELECT TOP 1 * FROM Caixa WHERE Status = 'Aberto' ORDER BY Id DESC";
            DataTable dt = db.ExecuteQuery(query);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new Caixa {
                    Id = (int)row["Id"],
                    DataAbertura = (DateTime)row["DataAbertura"],
                    ValorAbertura = (decimal)row["ValorAbertura"],
                    Status = row["Status"].ToString()
                };
            }
            return null;
        }

        public void AbrirCaixa(decimal valor)
        {
            string query = "INSERT INTO Caixa (DataAbertura, ValorAbertura, Status) VALUES (GETDATE(), @Valor, 'Aberto')";
            SqlParameter[] p = { new SqlParameter("@Valor", valor) };
            db.ExecuteNonQuery(query, p);
            new AuditoriaRepository().Registrar("ABRIR_CAIXA", "Financeiro", $"Caixa aberto com saldo inicial de {valor:C2}.");
        }

        public void FecharCaixa(int id, decimal valorFechamento)
        {
            string query = "UPDATE Caixa SET DataFechamento = GETDATE(), ValorFechamento = @Valor, Status = 'Fechado' WHERE Id = @Id";
            SqlParameter[] p = { 
                new SqlParameter("@Valor", valorFechamento),
                new SqlParameter("@Id", id)
            };
            db.ExecuteNonQuery(query, p);
            new AuditoriaRepository().Registrar("FECHAR_CAIXA", "Financeiro", $"Caixa #{id} fechado com saldo final de {valorFechamento:C2}.");
        }

        public DataTable ListarCaixasFechados()
        {
            string query = "SELECT Id, DataAbertura, DataFechamento, ValorAbertura, ValorFechamento FROM Caixa WHERE Status = 'Fechado' ORDER BY Id DESC";
            return db.ExecuteQuery(query);
        }

        // --- Movimentações ---

        public void LançarMovimentacao(Movimentacao m)
        {
            string query = @"INSERT INTO Financeiro (IdCaixa, Tipo, Categoria, Valor, FormaPagamento, Descricao, Data, Subcategoria) 
                              VALUES (@IdCaixa, @Tipo, @Categoria, @Valor, @FormaPagamento, @Descricao, GETDATE(), @Subcategoria)";
            
            SqlParameter[] p = {
                new SqlParameter("@IdCaixa", m.IdCaixa > 0 ? (object)m.IdCaixa : DBNull.Value),
                new SqlParameter("@Tipo", m.Tipo),
                new SqlParameter("@Categoria", m.Categoria),
                new SqlParameter("@Valor", m.Valor),
                new SqlParameter("@FormaPagamento", m.FormaPagamento ?? (object)DBNull.Value),
                new SqlParameter("@Descricao", m.Descricao ?? (object)DBNull.Value),
                new SqlParameter("@Subcategoria", m.Subcategoria ?? (object)DBNull.Value)
            };
            db.ExecuteNonQuery(query, p);
            new AuditoriaRepository().Registrar(m.Tipo.ToUpper(), "Financeiro", $"Lançamento financeiro do tipo {m.Tipo} na categoria '{m.Categoria}' [Subcategoria: '{m.Subcategoria}'] no valor de {m.Valor:C2} ({m.Descricao}).");
        }

        public void AtualizarFormaPagamento(int financeiroId, string novaFormaPagamento, int usuarioId)
        {
            db.ExecuteNonQuery("UPDATE Financeiro SET FormaPagamento = @NovaForma, Descricao = Descricao + ' [Corrigido]' WHERE Id = @Id", new[] {
                new SqlParameter("@NovaForma", novaFormaPagamento),
                new SqlParameter("@Id", financeiroId)
            });
            new AuditoriaRepository().Registrar("CORREÇÃO", "Financeiro", $"Forma de pagamento do lançamento #{financeiroId} alterada para '{novaFormaPagamento}' pelo usuário {usuarioId}.");
        }

        public void CorrigirMovimentacaoPorDescricao(string descricaoBusca, decimal valor, string novaFormaPagamento, DateTime dataAproximada)
        {
            string updateFin = @"
                UPDATE Financeiro SET FormaPagamento = @NovaForma, Descricao = Descricao + ' [Corrigido]'
                WHERE Id IN (
                    SELECT TOP 1 Id FROM Financeiro 
                    WHERE Tipo = 'Entrada' AND Descricao LIKE '%' + @DescBusca + '%'
                    AND Valor = @Valor AND Data >= @DataInicio AND Data <= @DataFim
                    ORDER BY Data DESC
                )";

            db.ExecuteNonQuery(updateFin, new[] {
                new SqlParameter("@NovaForma", novaFormaPagamento),
                new SqlParameter("@DescBusca", descricaoBusca),
                new SqlParameter("@Valor", valor),
                new SqlParameter("@DataInicio", dataAproximada.AddMinutes(-10)),
                new SqlParameter("@DataFim", dataAproximada.AddMinutes(10))
            });
        }

        public void RecriarMovimentacaoMistaPorDescricao(string descricaoBusca, decimal valorAntigo, DateTime dataAproximada, string cat, string subcat, decimal mistoDin, decimal mistoPix, decimal mistoCartao, decimal mistoSaldo, decimal troco)
        {
            // 1. Acha as movimentações antigas relacionadas (podem ser varias se já foi Misto antes, ou 1 se foi normal)
            string selectFin = @"
                SELECT Id, IdCaixa FROM Financeiro 
                WHERE Tipo = 'Entrada' AND Descricao LIKE '%' + @DescBusca + '%'
                AND Data >= @DataInicio AND Data <= @DataFim
            ";

            DataTable dt = db.ExecuteQuery(selectFin, new[] {
                new SqlParameter("@DescBusca", descricaoBusca),
                new SqlParameter("@DataInicio", dataAproximada.AddMinutes(-10)),
                new SqlParameter("@DataFim", dataAproximada.AddMinutes(10))
            });

            if (dt.Rows.Count == 0) return; // Não encontrou

            int idCaixa = dt.Rows[0]["IdCaixa"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["IdCaixa"]) : 0;

            // 2. Apaga as movimentacoes antigas relacionadas à entrada
            string deleteFin = @"
                DELETE FROM Financeiro 
                WHERE Tipo = 'Entrada' AND Descricao LIKE '%' + @DescBusca + '%'
                AND Data >= @DataInicio AND Data <= @DataFim
            ";
            db.ExecuteNonQuery(deleteFin, new[] {
                new SqlParameter("@DescBusca", descricaoBusca),
                new SqlParameter("@DataInicio", dataAproximada.AddMinutes(-10)),
                new SqlParameter("@DataFim", dataAproximada.AddMinutes(10))
            });

            // 3. E também apaga possível troco antigo
            string deleteTroco = @"
                DELETE FROM Financeiro 
                WHERE Tipo = 'Saída' AND Subcategoria = 'Troco' AND Descricao LIKE '%' + @DescBusca + '%'
                AND Data >= @DataInicio AND Data <= @DataFim
            ";
            db.ExecuteNonQuery(deleteTroco, new[] {
                new SqlParameter("@DescBusca", descricaoBusca),
                new SqlParameter("@DataInicio", dataAproximada.AddMinutes(-10)),
                new SqlParameter("@DataFim", dataAproximada.AddMinutes(10))
            });

            // 4. Lança os novos valores
            if (mistoDin > 0)
                LançarMovimentacao(new Movimentacao { IdCaixa = idCaixa, Tipo = "Entrada", Categoria = cat, Subcategoria = subcat, Valor = mistoDin, FormaPagamento = "Dinheiro", Descricao = descricaoBusca });
            if (mistoPix > 0)
                LançarMovimentacao(new Movimentacao { IdCaixa = idCaixa, Tipo = "Entrada", Categoria = cat, Subcategoria = subcat, Valor = mistoPix, FormaPagamento = "Pix", Descricao = descricaoBusca });
            if (mistoCartao > 0)
                LançarMovimentacao(new Movimentacao { IdCaixa = idCaixa, Tipo = "Entrada", Categoria = cat, Subcategoria = subcat, Valor = mistoCartao, FormaPagamento = "Cartão", Descricao = descricaoBusca });
            if (mistoSaldo > 0)
                LançarMovimentacao(new Movimentacao { IdCaixa = idCaixa, Tipo = "Entrada", Categoria = cat, Subcategoria = subcat, Valor = mistoSaldo, FormaPagamento = "Saldo Cliente", Descricao = descricaoBusca });

            if (troco > 0)
            {
                LançarMovimentacao(new Movimentacao { IdCaixa = idCaixa, Tipo = "Saída", Categoria = cat, Subcategoria = "Troco", Valor = troco, FormaPagamento = "Dinheiro", Descricao = "Troco - " + descricaoBusca });
            }
        }

        public Dictionary<string, decimal> GetTotaisPorSubcategoria(DateTime inicio, DateTime fim, string tipo)
        {
            Dictionary<string, decimal> dados = new Dictionary<string, decimal>();
            string query = @"SELECT ISNULL(Subcategoria, Categoria) as Subcat, SUM(Valor) as Total 
                             FROM Financeiro 
                             WHERE Tipo = @Tipo AND Data BETWEEN @inicio AND @fim 
                             GROUP BY ISNULL(Subcategoria, Categoria)";
            SqlParameter[] p = {
                new SqlParameter("@Tipo", tipo),
                new SqlParameter("@inicio", inicio.Date),
                new SqlParameter("@fim", fim.Date.AddDays(1).AddSeconds(-1))
            };
            DataTable dt = db.ExecuteQuery(query, p);
            foreach (DataRow row in dt.Rows)
            {
                string key = row["Subcat"]?.ToString() ?? "Outros";
                decimal val = Convert.ToDecimal(row["Total"]);
                if (dados.ContainsKey(key)) dados[key] += val;
                else dados.Add(key, val);
            }
            return dados;
        }

        public DataTable GetFluxoCaixa(DateTime inicio, DateTime fim)
        {
            string query = "SELECT * FROM Financeiro WHERE Data BETWEEN @inicio AND @fim ORDER BY Data DESC";
            SqlParameter[] p = {
                new SqlParameter("@inicio", inicio.Date),
                new SqlParameter("@fim", fim.Date.AddDays(1).AddSeconds(-1))
            };
            return db.ExecuteQuery(query, p);
        }

        public decimal GetTotalPorPeriodo(DateTime inicio, DateTime fim, string tipo)
        {
            string query = "SELECT SUM(Valor) FROM Financeiro WHERE Tipo = @Tipo AND Data BETWEEN @inicio AND @fim";
            SqlParameter[] p = {
                new SqlParameter("@Tipo", tipo),
                new SqlParameter("@inicio", inicio.Date),
                new SqlParameter("@fim", fim.Date.AddDays(1).AddSeconds(-1))
            };
            object res = db.ExecuteScalar(query, p);
            return res != DBNull.Value ? Convert.ToDecimal(res) : 0;
        }

        // Calculate gross entradas by payment method
        public decimal GetEntradasPorFormaPagamento(DateTime inicio, DateTime fim, string forma)
        {
            string query = "SELECT SUM(Valor) FROM Financeiro WHERE Tipo = 'Entrada' AND FormaPagamento = @Forma AND Data BETWEEN @inicio AND @fim";
            SqlParameter[] p = {
                new SqlParameter("@Forma", forma),
                new SqlParameter("@inicio", inicio.Date),
                new SqlParameter("@fim", fim.Date.AddDays(1).AddSeconds(-1))
            };
            object res = db.ExecuteScalar(query, p);
            return res != DBNull.Value ? Convert.ToDecimal(res) : 0;
        }

        // Calculate gross saidas by payment method
        public decimal GetSaidasPorFormaPagamento(DateTime inicio, DateTime fim, string forma)
        {
            string query = "SELECT SUM(Valor) FROM Financeiro WHERE Tipo = 'Saída' AND FormaPagamento = @Forma AND Data BETWEEN @inicio AND @fim";
            SqlParameter[] p = {
                new SqlParameter("@Forma", forma),
                new SqlParameter("@inicio", inicio.Date),
                new SqlParameter("@fim", fim.Date.AddDays(1).AddSeconds(-1))
            };
            object res = db.ExecuteScalar(query, p);
            return res != DBNull.Value ? Convert.ToDecimal(res) : 0;
        }

        // Calculate net balance by payment method (entradas - saidas)
        public decimal GetTotalPorFormaPagamento(DateTime inicio, DateTime fim, string forma)
        {
            return GetEntradasPorFormaPagamento(inicio, fim, forma) - GetSaidasPorFormaPagamento(inicio, fim, forma);
        }
        public Dictionary<DateTime, decimal> GetVendasUltimos7Dias()
        {
            Dictionary<DateTime, decimal> dados = new Dictionary<DateTime, decimal>();
            DateTime hoje = DateTime.Today;

            string query = @"SELECT CAST(DataVenda AS DATE) as Dia, SUM(TotalFinal) as Total 
                             FROM Vendas 
                             WHERE Status = 'Concluída' AND DataVenda >= @inicio 
                             GROUP BY CAST(DataVenda AS DATE)";
            SqlParameter[] p = { new SqlParameter("@inicio", hoje.AddDays(-6)) };
            DataTable dt = db.ExecuteQuery(query, p);
            
            // preencher dados
            for (int i = 6; i >= 0; i--) dados.Add(hoje.AddDays(-i), 0);
            foreach(DataRow row in dt.Rows) {
                dados[Convert.ToDateTime(row["Dia"])] = Convert.ToDecimal(row["Total"]);
            }
            return dados;
        }

        public List<string> GetSubcategoriasDisponiveis()
        {
            List<string> lista = new List<string>();
            string query = "SELECT DISTINCT ISNULL(Subcategoria, Categoria) as Sub FROM Financeiro ORDER BY Sub ASC";
            DataTable dt = db.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                string s = row["Sub"]?.ToString();
                if (!string.IsNullOrWhiteSpace(s)) lista.Add(s);
            }
            return lista;
        }

        public DataTable GetMovimentacoesDiarias(DateTime data)
        {
            string query = "SELECT Id, Tipo, Categoria, ISNULL(Subcategoria, '') as Subcategoria, FormaPagamento, Descricao, Valor, Data " +
                           "FROM Financeiro WHERE Data >= @inicio AND Data <= @fim ORDER BY Data DESC";
            SqlParameter[] p = {
                new SqlParameter("@inicio", data.Date),
                new SqlParameter("@fim", data.Date.AddDays(1).AddSeconds(-1))
            };
            return db.ExecuteQuery(query, p);
        }
    }
}
