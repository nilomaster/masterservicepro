using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.DAL
{
    public class OrdemServicoRepository
    {
        private DbConnection db = new DbConnection();

        private static bool _tabelaCriada = false;
        private static readonly object _lock = new object();

        public OrdemServicoRepository()
        {
            try { CriarTabelaOS(); } catch { }
        }

        private void CriarTabelaOS()
        {
            if (_tabelaCriada) return;
            lock (_lock)
            {
                if (_tabelaCriada) return;

                // Se a tabela não existe, cria do zero
                if (db.ExecuteScalar("SELECT object_id FROM sys.objects WHERE object_id = OBJECT_ID('OrdensServico') AND type in (N'U')") == null)
                {
                    db.ExecuteNonQuery(@"
                        CREATE TABLE OrdensServico (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            NumeroOS VARCHAR(50),
                            ClienteId INT,
                            IdCliente INT NOT NULL,
                            Marca VARCHAR(100),
                            Modelo VARCHAR(100),
                            Cor VARCHAR(50),
                            IMEI VARCHAR(100),
                            ClienteFinal VARCHAR(100),
                            Defeito VARCHAR(MAX),
                            LaudoTecnico VARCHAR(MAX),
                            ValorTotal DECIMAL(18,2) DEFAULT 0,
                            Status VARCHAR(50) DEFAULT 'Pendente',
                            EstoqueBaixado BIT DEFAULT 0,
                            Faturado BIT DEFAULT 0,
                            DataAbertura DATETIME DEFAULT GETDATE(),
                            DataAtualizacao DATETIME DEFAULT GETDATE()
                        )");
                }
                else
                {
                    // Migração: Garante que todas as colunas existem
                    string[] colunas = { 
                        "NumeroOS VARCHAR(50)",
                        "ClienteId INT",
                        "IdCliente INT", 
                        "IdTecnico INT",
                        "ValorComissao DECIMAL(18,2)",
                        "ComissaoPaga BIT",
                        "Marca VARCHAR(100)", 
                        "Modelo VARCHAR(100)", 
                        "Cor VARCHAR(50)",
                        "IMEI VARCHAR(100)",
                        "ClienteFinal VARCHAR(100)",
                        "Defeito VARCHAR(MAX)", 
                        "LaudoTecnico VARCHAR(MAX)", 
                        "ValorPecas DECIMAL(18,2)",
                        "ValorServico DECIMAL(18,2)",
                        "ValorTotal DECIMAL(18,2)", 
                        "Status VARCHAR(50)", 
                        "EstoqueBaixado BIT",
                        "Faturado BIT",
                        "Checklist VARCHAR(MAX)",
                        "DataAbertura DATETIME", 
                        "DataAtualizacao DATETIME" 
                    };

                    foreach (var col in colunas)
                    {
                        string nomeCol = col.Split(' ')[0];
                        string checkCol = $"IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('OrdensServico') AND name = '{nomeCol}') " +
                                         $"ALTER TABLE OrdensServico ADD {col};";
                        
                        try { db.ExecuteNonQuery(checkCol); } catch { }
                    }
                }

                // Criação da tabela de itens
                if (db.ExecuteScalar("SELECT object_id FROM sys.objects WHERE object_id = OBJECT_ID('OrdemServicoItens') AND type in (N'U')") == null)
                {
                    db.ExecuteNonQuery(@"
                        CREATE TABLE OrdemServicoItens (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            OrdemServicoId INT NOT NULL,
                            ProdutoId INT NOT NULL,
                            NomeProduto VARCHAR(200),
                            Quantidade DECIMAL(18,2),
                            ValorUnitario DECIMAL(18,2),
                            SubTotal DECIMAL(18,2),
                            CustoAdicional DECIMAL(18,2) DEFAULT 0
                        )");
                }
                else
                {
                    string checkColCusto = $"IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('OrdemServicoItens') AND name = 'CustoAdicional') " +
                                         $"ALTER TABLE OrdemServicoItens ADD CustoAdicional DECIMAL(18,2) DEFAULT 0;";
                    try { db.ExecuteNonQuery(checkColCusto); } catch { }
                }

                _tabelaCriada = true;
            }
        }

        public DataTable BuscarTudoResumido()
        {
            string query = @"
                SELECT OS.Id, ISNULL(NULLIF(C.Nome, ''), ISNULL(OS.ClienteFinal, 'Cliente não encontrado')) as Cliente, 
                       ISNULL(OS.Marca, '') as Marca, ISNULL(OS.Modelo, '') as Modelo, 
                       ISNULL(OS.Defeito, '') as Defeito, OS.ValorTotal, 
                       OS.ValorServico as Lucro, 
                       OS.Status, OS.DataAbertura, OS.DataAtualizacao 
                FROM OrdensServico OS
                LEFT JOIN Clientes C ON (OS.IdCliente = C.Id OR OS.ClienteId = C.Id)
                ORDER BY OS.DataAbertura DESC";
                
            return db.ExecuteQuery(query);
        }



        public void Inserir(OrdemServico os)
        {
            if (string.IsNullOrEmpty(os.NumeroOS))
            {
                os.NumeroOS = new Random().Next(100000, 999999).ToString();
            }

            string query = @"INSERT INTO OrdensServico (NumeroOS, ClienteId, IdCliente, IdTecnico, Marca, Modelo, Cor, IMEI, ClienteFinal, Defeito, LaudoTecnico, ValorPecas, ValorServico, ValorTotal, Faturado, Status, Checklist, DataAbertura) 
                             VALUES (@NumeroOS, @IdCliente, @IdCliente, @IdTecnico, @Marca, @Modelo, @Cor, @IMEI, @ClienteFinal, @Defeito, @LaudoTecnico, @ValorPecas, @ValorServico, @ValorTotal, @Faturado, @Status, @Checklist, @DataAbertura);
                             SELECT SCOPE_IDENTITY();";
            
            SqlParameter[] param = {
                new SqlParameter("@NumeroOS", os.NumeroOS),
                new SqlParameter("@IdCliente", os.IdCliente),
                new SqlParameter("@IdTecnico", os.IdTecnico.HasValue ? (object)os.IdTecnico.Value : DBNull.Value),
                new SqlParameter("@Marca", os.Marca ?? (object)DBNull.Value),
                new SqlParameter("@Modelo", os.Modelo ?? (object)DBNull.Value),
                new SqlParameter("@Cor", os.Cor ?? (object)DBNull.Value),
                new SqlParameter("@IMEI", os.IMEI ?? (object)DBNull.Value),
                new SqlParameter("@ClienteFinal", os.ClienteFinal ?? (object)DBNull.Value),
                new SqlParameter("@Defeito", os.Defeito ?? (object)DBNull.Value),
                new SqlParameter("@LaudoTecnico", os.LaudoTecnico ?? (object)DBNull.Value),
                new SqlParameter("@ValorPecas", os.ValorPecas),
                new SqlParameter("@ValorServico", os.ValorServico),
                new SqlParameter("@ValorTotal", os.ValorTotal),

                new SqlParameter("@Faturado", os.Faturado ? 1 : 0),
                new SqlParameter("@Status", os.Status),
                new SqlParameter("@Checklist", os.Checklist ?? (object)DBNull.Value),
                new SqlParameter("@DataAbertura", os.DataAbertura)
            };
            
            int idOs = Convert.ToInt32(db.ExecuteScalar(query, param));
            os.Id = idOs;
            
            SalvarItens(os);
            ProcessarBaixaEstoque(os);
            new AuditoriaRepository().Registrar("CRIAR", "OrdensServico", $"Aberta O.S. #{os.NumeroOS} para o aparelho {os.Marca} {os.Modelo} (Status: {os.Status}).");
        }

        public void Atualizar(OrdemServico os)
        {
            string query = @"UPDATE OrdensServico SET 
                             ClienteId = @IdCliente,
                             IdCliente = @IdCliente, 
                             IdTecnico = @IdTecnico,
                             Marca = @Marca, 
                             Modelo = @Modelo, 
                             Cor = @Cor,
                             IMEI = @IMEI,
                             ClienteFinal = @ClienteFinal,
                             Defeito = @Defeito, 
                             LaudoTecnico = @LaudoTecnico, 
                             ValorPecas = @ValorPecas,
                             ValorServico = @ValorServico,
                             ValorTotal = @ValorTotal, 
                             Faturado = @Faturado,
                             Status = @Status, 
                             Checklist = @Checklist,
                             DataAtualizacao = GETDATE()
                             WHERE Id = @Id";
            
            SqlParameter[] param = {
                new SqlParameter("@Id", os.Id),
                new SqlParameter("@IdCliente", os.IdCliente),
                new SqlParameter("@IdTecnico", os.IdTecnico.HasValue ? (object)os.IdTecnico.Value : DBNull.Value),
                new SqlParameter("@Marca", os.Marca ?? (object)DBNull.Value),
                new SqlParameter("@Modelo", os.Modelo ?? (object)DBNull.Value),
                new SqlParameter("@Cor", os.Cor ?? (object)DBNull.Value),
                new SqlParameter("@IMEI", os.IMEI ?? (object)DBNull.Value),
                new SqlParameter("@ClienteFinal", os.ClienteFinal ?? (object)DBNull.Value),
                new SqlParameter("@Defeito", os.Defeito ?? (object)DBNull.Value),
                new SqlParameter("@LaudoTecnico", os.LaudoTecnico ?? (object)DBNull.Value),
                new SqlParameter("@ValorPecas", os.ValorPecas),
                new SqlParameter("@ValorServico", os.ValorServico),
                new SqlParameter("@ValorTotal", os.ValorTotal),

                new SqlParameter("@Faturado", os.Faturado ? 1 : 0),
                new SqlParameter("@Status", os.Status),
                new SqlParameter("@Checklist", os.Checklist ?? (object)DBNull.Value)
            };
            
            db.ExecuteNonQuery(query, param);
            
            RestaurarEstoque(os.Id);
            SalvarItens(os);
            ProcessarBaixaEstoque(os);
            new AuditoriaRepository().Registrar("ATUALIZAR", "OrdensServico", $"Atualizada O.S. #{os.NumeroOS} - Status: {os.Status}, Total: {os.ValorTotal:C2}.");
        }

        public void AtualizarFormaPagamentoOS(int osId, string novaFormaPagamento, int usuarioId)
        {
            var os = BuscarPorId(osId);
            if (os == null) throw new Exception("OS não encontrada.");
            if (!os.Faturado) throw new Exception("Esta O.S. ainda não foi faturada (paga).");

            var finRepo = new FinanceiroRepository();
            finRepo.CorrigirMovimentacaoPorDescricao($"Recebimento OS #{os.NumeroOS}", os.ValorTotal, novaFormaPagamento, os.DataAtualizacao);

            new AuditoriaRepository().Registrar("CORREÇÃO", "OrdensServico", $"Forma de pagamento da OS #{os.NumeroOS} alterada para '{novaFormaPagamento}' pelo usuário {usuarioId}.");
        }

        public void AtualizarFormaPagamentoOSMista(int osId, string novaFormaPagamento, int usuarioId, decimal troco, decimal mistoDin, decimal mistoPix, decimal mistoCartao, decimal mistoSaldo)
        {
            var os = BuscarPorId(osId);
            if (os == null) throw new Exception("OS não encontrada.");
            if (!os.Faturado) throw new Exception("Esta O.S. ainda não foi faturada (paga).");

            var finRepo = new FinanceiroRepository();
            
            if (novaFormaPagamento == "Misto" || troco > 0)
            {
                finRepo.RecriarMovimentacaoMistaPorDescricao($"Recebimento OS #{os.NumeroOS}", os.ValorTotal, os.DataAtualizacao, "Serviço", "Recebimento OS", mistoDin, mistoPix, mistoCartao, mistoSaldo, troco);
            }
            else
            {
                finRepo.CorrigirMovimentacaoPorDescricao($"Recebimento OS #{os.NumeroOS}", os.ValorTotal, novaFormaPagamento, os.DataAtualizacao);
            }

            new AuditoriaRepository().Registrar("CORREÇÃO", "OrdensServico", $"Forma de pagamento da OS #{os.NumeroOS} alterada para '{novaFormaPagamento}' pelo usuário {usuarioId}. (Misto/Troco atualizado)");
        }

        public void AtualizarCliente(int osId, int novoClienteId, string nomeCliente, int usuarioId)
        {
            var os = BuscarPorId(osId);
            if (os == null) throw new Exception("OS não encontrada.");

            db.ExecuteNonQuery("UPDATE OrdensServico SET ClienteId = @ClienteId, IdCliente = @ClienteId WHERE Id = @Id", new[] {
                new SqlParameter("@ClienteId", novoClienteId > 0 ? (object)novoClienteId : DBNull.Value),
                new SqlParameter("@Id", osId)
            });

            new AuditoriaRepository().Registrar("CORREÇÃO", "OrdensServico", $"Cliente da OS #{os.NumeroOS} alterado para '{nomeCliente}' pelo usuário {usuarioId}.");
        }

        private void SalvarItens(OrdemServico os)
        {
            if (os.Itens == null) return;

            db.ExecuteNonQuery("DELETE FROM OrdemServicoItens WHERE OrdemServicoId = @Id", new SqlParameter[] { new SqlParameter("@Id", os.Id) });

            foreach (var item in os.Itens)
            {
                string query = @"INSERT INTO OrdemServicoItens (OrdemServicoId, ProdutoId, NomeProduto, Quantidade, ValorUnitario, SubTotal, CustoAdicional)
                                 VALUES (@OrdemServicoId, @ProdutoId, @NomeProduto, @Quantidade, @ValorUnitario, @SubTotal, @CustoAdicional)";
                SqlParameter[] p = {
                    new SqlParameter("@OrdemServicoId", os.Id),
                    new SqlParameter("@ProdutoId", item.ProdutoId),
                    new SqlParameter("@NomeProduto", item.NomeProduto ?? ""),
                    new SqlParameter("@Quantidade", item.Quantidade),
                    new SqlParameter("@ValorUnitario", item.ValorUnitario),
                    new SqlParameter("@SubTotal", item.SubTotal),
                    new SqlParameter("@CustoAdicional", item.CustoAdicional)
                };
                db.ExecuteNonQuery(query, p);
            }
        }

        private void RestaurarEstoque(int osId)
        {
            bool jaBaixado = Convert.ToBoolean(db.ExecuteScalar("SELECT ISNULL(EstoqueBaixado, 0) FROM OrdensServico WHERE Id = @Id", new SqlParameter[] { new SqlParameter("@Id", osId) }));
            if (jaBaixado)
            {
                ProdutoRepository prodRepo = new ProdutoRepository();
                DataTable dtItens = db.ExecuteQuery("SELECT ProdutoId, Quantidade FROM OrdemServicoItens WHERE OrdemServicoId = @Id", new SqlParameter[] { new SqlParameter("@Id", osId) });
                foreach (DataRow r in dtItens.Rows)
                {
                    int produtoId = Convert.ToInt32(r["ProdutoId"]);
                    decimal qtd = Convert.ToDecimal(r["Quantidade"]);
                    if (produtoId > 0) prodRepo.DeduzirEstoque(produtoId, -qtd);
                }
                db.ExecuteNonQuery("UPDATE OrdensServico SET EstoqueBaixado = 0 WHERE Id = @Id", new SqlParameter[] { new SqlParameter("@Id", osId) });
            }
        }

        private void ProcessarBaixaEstoque(OrdemServico os)
        {
            bool deveBaixar = (os.Status != "Cancelado" && os.Status != "Cancelada");
            
            if (deveBaixar)
            {
                ProdutoRepository prodRepo = new ProdutoRepository();
                
                // Verifica estoque antes de baixar
                foreach (var item in os.Itens)
                {
                    if (item.ProdutoId > 0)
                    {
                        var produto = prodRepo.BuscarPorId(item.ProdutoId);
                        if (produto != null && produto.Estoque < item.Quantidade)
                        {
                            throw new Exception($"Estoque insuficiente para a peça/produto '{produto.Nome}'. Estoque atual: {produto.Estoque}, Quantidade necessária para a O.S.: {item.Quantidade}.");
                        }
                    }
                }

                // Baixa estoque
                foreach (var item in os.Itens)
                {
                    if (item.ProdutoId > 0) prodRepo.DeduzirEstoque(item.ProdutoId, item.Quantidade);
                }
                db.ExecuteNonQuery("UPDATE OrdensServico SET EstoqueBaixado = 1 WHERE Id = @Id", new SqlParameter[] { new SqlParameter("@Id", os.Id) });
            }
        }

        public OrdemServico BuscarPorId(int id)
        {
            string query = "SELECT * FROM OrdensServico WHERE Id = @Id";
            SqlParameter[] param = { new SqlParameter("@Id", id) };
            DataTable dt = db.ExecuteQuery(query, param);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                var os = new OrdemServico
                {
                    Id = Convert.ToInt32(row["Id"]),
                    NumeroOS = row.Table.Columns.Contains("NumeroOS") ? (row["NumeroOS"]?.ToString() ?? "") : "",
                    IdCliente = row.Table.Columns.Contains("ClienteId") && row["ClienteId"] != DBNull.Value 
                                ? Convert.ToInt32(row["ClienteId"]) 
                                : Convert.ToInt32(row["IdCliente"]),
                    IdTecnico = row.Table.Columns.Contains("IdTecnico") && row["IdTecnico"] != DBNull.Value 
                                ? Convert.ToInt32(row["IdTecnico"]) 
                                : (int?)null,

                    Faturado = row.Table.Columns.Contains("Faturado") && row["Faturado"] != DBNull.Value 
                                    ? Convert.ToBoolean(row["Faturado"]) 
                                    : false,
                    Marca = row["Marca"]?.ToString() ?? "",
                    Modelo = row["Modelo"]?.ToString() ?? "",
                    Cor = row.Table.Columns.Contains("Cor") ? (row["Cor"]?.ToString() ?? "") : "",
                    IMEI = row.Table.Columns.Contains("IMEI") ? (row["IMEI"]?.ToString() ?? "") : "",
                    ClienteFinal = row.Table.Columns.Contains("ClienteFinal") ? (row["ClienteFinal"]?.ToString() ?? "") : "",
                    Defeito = row["Defeito"]?.ToString() ?? "",
                    LaudoTecnico = row["LaudoTecnico"]?.ToString() ?? "",
                    ValorPecas = row.Table.Columns.Contains("ValorPecas") && row["ValorPecas"] != DBNull.Value ? Convert.ToDecimal(row["ValorPecas"]) : 0,
                    ValorServico = row.Table.Columns.Contains("ValorServico") && row["ValorServico"] != DBNull.Value ? Convert.ToDecimal(row["ValorServico"]) : 0,
                    ValorTotal = row["ValorTotal"] != DBNull.Value ? Convert.ToDecimal(row["ValorTotal"]) : 0,
                    Status = row["Status"]?.ToString() ?? "Pendente",
                    Checklist = row.Table.Columns.Contains("Checklist") ? (row["Checklist"]?.ToString() ?? "") : "",
                    DataAbertura = row["DataAbertura"] != DBNull.Value ? Convert.ToDateTime(row["DataAbertura"]) : DateTime.Now
                };

                // Busca itens
                DataTable dtItens = db.ExecuteQuery("SELECT * FROM OrdemServicoItens WHERE OrdemServicoId = @Id", new SqlParameter[] { new SqlParameter("@Id", id) });
                foreach (DataRow r in dtItens.Rows)
                {
                    os.Itens.Add(new OrdemServicoItem
                    {
                        Id = Convert.ToInt32(r["Id"]),
                        OrdemServicoId = id,
                        ProdutoId = Convert.ToInt32(r["ProdutoId"]),
                        NomeProduto = r["NomeProduto"].ToString(),
                        Quantidade = Convert.ToDecimal(r["Quantidade"]),
                        ValorUnitario = Convert.ToDecimal(r["ValorUnitario"]),
                        SubTotal = Convert.ToDecimal(r["SubTotal"]),
                        CustoAdicional = r.Table.Columns.Contains("CustoAdicional") && r["CustoAdicional"] != DBNull.Value ? Convert.ToDecimal(r["CustoAdicional"]) : 0
                    });
                }
                
                return os;
            }
            return null;
        }

        public void Excluir(int id)
        {
            var os = BuscarPorId(id);
            if (os != null)
            {
                string num = os.NumeroOS;
                os.Status = "Cancelada";
                Atualizar(os); // Isso processa automaticamente o estorno do estoque se já estivesse baixado
                new AuditoriaRepository().Registrar("CANCELAR", "OrdensServico", $"O.S. #{num} foi cancelada.");
            }
        }

        public void Deletar(int id)
        {
            var os = BuscarPorId(id);
            if (os != null)
            {
                string num = os.NumeroOS;
                
                // Antes de deletar fisicamente, precisamos estornar o estoque se ele estiver baixado
                bool jaBaixado = Convert.ToBoolean(db.ExecuteScalar("SELECT ISNULL(EstoqueBaixado, 0) FROM OrdensServico WHERE Id = @Id", new SqlParameter[] { new SqlParameter("@Id", id) }));
                if (jaBaixado)
                {
                    ProdutoRepository prodRepo = new ProdutoRepository();
                    foreach (var item in os.Itens)
                    {
                        if (item.ProdutoId > 0)
                            prodRepo.DeduzirEstoque(item.ProdutoId, -item.Quantidade); // negativo para devolver
                    }
                }
                
                // Apaga itens e a OS fisicamente do banco
                db.ExecuteNonQuery("DELETE FROM OrdemServicoItens WHERE OrdemServicoId = @Id", new SqlParameter[] { new SqlParameter("@Id", id) });
                db.ExecuteNonQuery("DELETE FROM OrdensServico WHERE Id = @Id", new SqlParameter[] { new SqlParameter("@Id", id) });
                new AuditoriaRepository().Registrar("DELETAR", "OrdensServico", $"O.S. #{num} foi excluída fisicamente do banco de dados.");
            }
        }

        public DataTable GetCustosLucrosRelatorio(DateTime inicio, DateTime fim, int? idTecnico = null)
        {
            string query = @"
                SELECT OS.NumeroOS, OS.DataAtualizacao as DataFechamento, T.Nome as Tecnico, 
                       ISNULL(STUFF((SELECT ', ' + CAST(CAST(OSI.Quantidade AS INT) AS VARCHAR) + 'x ' + OSI.NomeProduto
                                     FROM OrdemServicoItens OSI 
                                     WHERE OSI.OrdemServicoId = OS.Id
                                     FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, ''), 'Nenhuma') as PecasUsadas,
                       ISNULL(OS.ValorPecas, 0) as CustoPecas, 
                       ISNULL(OS.ValorTotal, 0) as MaoObra, 
                       ISNULL(OS.ValorServico, 0) as LucroLiquido
                FROM OrdensServico OS
                INNER JOIN Tecnicos T ON OS.IdTecnico = T.Id
                WHERE OS.Status = 'Finalizado' 
                  AND OS.DataAtualizacao BETWEEN @inicio AND @fim";
            
            var list = new List<SqlParameter>
            {
                new SqlParameter("@inicio", inicio.Date),
                new SqlParameter("@fim", fim.Date.AddDays(1).AddSeconds(-1))
            };

            if (idTecnico.HasValue && idTecnico.Value > 0)
            {
                query += " AND OS.IdTecnico = @IdTecnico";
                list.Add(new SqlParameter("@IdTecnico", idTecnico.Value));
            }

            query += " ORDER BY OS.DataAtualizacao DESC";

            return db.ExecuteQuery(query, list.ToArray());
        }

        // Retrieve distinct brands from OrdensServico and Produtos
        public async Task<List<string>> ObterMarcasDistintasAsync()
        {
            return await Task.Run(() =>
            {
                var marcas = new List<string>();
                try
                {
                    string query = @"
                        SELECT DISTINCT LTRIM(RTRIM(Marca)) AS Marca FROM OrdensServico WHERE Marca IS NOT NULL AND LTRIM(RTRIM(Marca)) <> ''
                        UNION
                        SELECT DISTINCT LTRIM(RTRIM(Marca)) AS Marca FROM Produtos WHERE Marca IS NOT NULL AND LTRIM(RTRIM(Marca)) <> ''
                        ORDER BY Marca";
                    DataTable dt = db.ExecuteQuery(query);
                    if (dt != null)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            string m = row["Marca"]?.ToString()?.Trim();
                            if (!string.IsNullOrEmpty(m) && !marcas.Contains(m))
                                marcas.Add(m);
                        }
                    }
                }
                catch { }
                return marcas;
            });
        }

        // Retrieve distinct models from OrdensServico and Produtos
        public async Task<List<string>> ObterModelosDistintosAsync()
        {
            return await Task.Run(() =>
            {
                var modelos = new List<string>();
                try
                {
                    string query = @"
                        SELECT DISTINCT LTRIM(RTRIM(Modelo)) AS Modelo FROM OrdensServico WHERE Modelo IS NOT NULL AND LTRIM(RTRIM(Modelo)) <> ''
                        UNION
                        SELECT DISTINCT LTRIM(RTRIM(Modelo)) AS Modelo FROM Produtos WHERE Modelo IS NOT NULL AND LTRIM(RTRIM(Modelo)) <> ''
                        ORDER BY Modelo";
                    DataTable dt = db.ExecuteQuery(query);
                    if (dt != null)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            string m = row["Modelo"]?.ToString()?.Trim();
                            if (!string.IsNullOrEmpty(m) && !modelos.Contains(m))
                                modelos.Add(m);
                        }
                    }
                }
                catch { }
                return modelos;
            });
        }
    }
}
