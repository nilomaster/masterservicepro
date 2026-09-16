using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.DAL
{
    public class ProdutoRepository
    {
        private DbConnection db = new DbConnection();

        private static bool _tabelaCriada = false;
        private static readonly object _lock = new object();

        public ProdutoRepository()
        {
            try { CriarTabelaProdutos(); } catch { }
        }

        private void CriarTabelaProdutos()
        {
            if (_tabelaCriada) return;
            lock (_lock)
            {
                if (_tabelaCriada) return;

                try
                {
                    var result = db.ExecuteScalar("SELECT object_id FROM sys.objects WHERE object_id = OBJECT_ID('Produtos') AND type in (N'U')");
                    if (result == null || result == DBNull.Value)
                    {
                        db.ExecuteNonQuery(@"
                            CREATE TABLE Produtos (
                                Id INT PRIMARY KEY IDENTITY(1,1),
                                IdCategoria INT NULL,
                                IdFornecedor INT NULL,
                                CodigoInterno VARCHAR(50),
                                CodigoBarras VARCHAR(50),
                                Nome VARCHAR(100) NOT NULL,
                                Marca VARCHAR(50),
                                Modelo VARCHAR(50),
                                Descricao VARCHAR(MAX),
                                PrecoCusto DECIMAL(18,2) DEFAULT 0,
                                PrecoVenda DECIMAL(18,2) DEFAULT 0,
                                Margem DECIMAL(18,2) DEFAULT 0,
                                Estoque DECIMAL(18,2) DEFAULT 0,
                                EstoqueMinimo DECIMAL(18,2) DEFAULT 0,
                                Garantia VARCHAR(100),
                                Status VARCHAR(20) DEFAULT 'Ativo',
                                Ativo BIT DEFAULT 1,
                                DataCadastro DATETIME DEFAULT GETDATE(),
                                DataAtualizacao DATETIME DEFAULT GETDATE()
                            )");
                    }
                    else
                    {
                        string[] colunas = { 
                            "IdCategoria INT",
                            "IdFornecedor INT",
                            "CodigoInterno VARCHAR(50)", 
                            "CodigoBarras VARCHAR(50)", 
                            "Marca VARCHAR(50)", 
                            "Modelo VARCHAR(50)", 
                            "Descricao VARCHAR(MAX)", 
                            "PrecoCusto DECIMAL(18,2)", 
                            "PrecoVenda DECIMAL(18,2)", 
                            "Margem DECIMAL(18,2)", 
                            "Estoque DECIMAL(18,2)", 
                            "EstoqueMinimo DECIMAL(18,2)", 
                            "Garantia VARCHAR(100)", 
                            "ImagemUrl VARCHAR(MAX)",
                            "Status VARCHAR(20)", 
                            "Ativo BIT", 
                            "DataCadastro DATETIME", 
                            "DataAtualizacao DATETIME" 
                        };

                        foreach (var col in colunas)
                        {
                            string nomeCol = col.Split(' ')[0];
                            string checkCol = $"IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Produtos') AND name = '{nomeCol}') " +
                                             $"ALTER TABLE Produtos ADD {col};";
                            
                            try { db.ExecuteNonQuery(checkCol); } catch { }
                        }

                        // Executa updates apenas uma vez no startup
                        try { db.ExecuteNonQuery("UPDATE Produtos SET Ativo = 1 WHERE Ativo IS NULL"); } catch { }
                        try { db.ExecuteNonQuery("UPDATE Produtos SET Status = 'Ativo' WHERE Status IS NULL"); } catch { }
                    }
                    _tabelaCriada = true;
                }
                catch { }
            }
        }

        public List<Produto> BuscarTodos()
        {
            List<Produto> lista = new List<Produto>();
            string query = "SELECT P.*, C.Nome as CategoriaNome, F.Nome as FornecedorNome FROM Produtos P LEFT JOIN Categorias C ON P.IdCategoria = C.Id LEFT JOIN Fornecedores F ON P.IdFornecedor = F.Id WHERE P.Ativo = 1 ORDER BY P.Nome ASC";
            
            DataTable dt = db.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(MapRowToProduto(row));
            }
            return lista;
        }

        public void Inserir(Produto p)
        {
            string query = @"INSERT INTO Produtos (IdCategoria, IdFornecedor, CodigoInterno, CodigoBarras, Nome, Marca, Modelo, Descricao, PrecoCusto, PrecoVenda, Margem, Estoque, EstoqueMinimo, Garantia, ImagemUrl, Ativo, Status) 
                              VALUES (@IdCategoria, @IdFornecedor, @CodigoInterno, @CodigoBarras, @Nome, @Marca, @Modelo, @Descricao, @PrecoCusto, @PrecoVenda, @Margem, @Estoque, @EstoqueMinimo, @Garantia, @ImagemUrl, @Ativo, @Status);
                              SELECT SCOPE_IDENTITY();";
            
            SqlParameter[] param = {
                new SqlParameter("@IdCategoria", p.IdCategoria ?? (object)DBNull.Value),
                new SqlParameter("@IdFornecedor", p.IdFornecedor ?? (object)DBNull.Value),
                new SqlParameter("@CodigoInterno", p.CodigoInterno ?? (object)DBNull.Value),
                new SqlParameter("@CodigoBarras", p.CodigoBarras ?? (object)DBNull.Value),
                new SqlParameter("@Nome", p.Nome),
                new SqlParameter("@Marca", p.Marca ?? (object)DBNull.Value),
                new SqlParameter("@Modelo", p.Modelo ?? (object)DBNull.Value),
                new SqlParameter("@Descricao", p.Descricao ?? (object)DBNull.Value),
                new SqlParameter("@PrecoCusto", p.PrecoCusto),
                new SqlParameter("@PrecoVenda", p.PrecoVenda),
                new SqlParameter("@Margem", p.Margem),
                new SqlParameter("@Estoque", p.Estoque),
                new SqlParameter("@EstoqueMinimo", p.EstoqueMinimo),
                new SqlParameter("@Garantia", p.Garantia ?? (object)DBNull.Value),
                new SqlParameter("@ImagemUrl", p.ImagemUrl ?? (object)DBNull.Value),
                new SqlParameter("@Ativo", p.Ativo),
                new SqlParameter("@Status", string.IsNullOrEmpty(p.Status) ? "Ativo" : p.Status)
            };
            
            object res = db.ExecuteScalar(query, param);
            if (res != null && res != DBNull.Value)
            {
                p.Id = Convert.ToInt32(res);
            }
            new AuditoriaRepository().Registrar("CRIAR", "Produtos", $"Cadastrado produto '{p.Nome}' com estoque inicial {p.Estoque}.");
        }

        public void Excluir(int id)
        {
            var pObj = BuscarPorId(id);
            string nome = pObj != null ? pObj.Nome : id.ToString();
            string query = "UPDATE Produtos SET Ativo = 0, DataAtualizacao = GETDATE() WHERE Id = @Id";
            SqlParameter[] p = { new SqlParameter("@Id", id) };
            db.ExecuteNonQuery(query, p);
            new AuditoriaRepository().Registrar("DESATIVAR", "Produtos", $"Produto '{nome}' foi desativado/inativado.");
        }

        public void Atualizar(Produto p)
        {
            string query = @"UPDATE Produtos SET 
                             IdCategoria = @IdCategoria,
                             IdFornecedor = @IdFornecedor,
                             CodigoInterno = @CodigoInterno, 
                             CodigoBarras = @CodigoBarras, 
                             Nome = @Nome, 
                             Marca = @Marca, 
                             Modelo = @Modelo, 
                             Descricao = @Descricao, 
                             PrecoCusto = @PrecoCusto, 
                             PrecoVenda = @PrecoVenda, 
                             Margem = @Margem, 
                             Estoque = @Estoque, 
                             EstoqueMinimo = @EstoqueMinimo,
                             Garantia = @Garantia,
                             ImagemUrl = @ImagemUrl,
                             DataAtualizacao = GETDATE()
                             WHERE Id = @Id";
            
            SqlParameter[] param = {
                new SqlParameter("@Id", p.Id),
                new SqlParameter("@IdCategoria", p.IdCategoria ?? (object)DBNull.Value),
                new SqlParameter("@IdFornecedor", p.IdFornecedor ?? (object)DBNull.Value),
                new SqlParameter("@CodigoInterno", p.CodigoInterno ?? (object)DBNull.Value),
                new SqlParameter("@CodigoBarras", p.CodigoBarras ?? (object)DBNull.Value),
                new SqlParameter("@Nome", p.Nome),
                new SqlParameter("@Marca", p.Marca ?? (object)DBNull.Value),
                new SqlParameter("@Modelo", p.Modelo ?? (object)DBNull.Value),
                new SqlParameter("@Descricao", p.Descricao ?? (object)DBNull.Value),
                new SqlParameter("@PrecoCusto", p.PrecoCusto),
                new SqlParameter("@PrecoVenda", p.PrecoVenda),
                new SqlParameter("@Margem", p.Margem),
                new SqlParameter("@Estoque", p.Estoque),
                new SqlParameter("@EstoqueMinimo", p.EstoqueMinimo),
                new SqlParameter("@Garantia", p.Garantia ?? (object)DBNull.Value),
                new SqlParameter("@ImagemUrl", p.ImagemUrl ?? (object)DBNull.Value)
            };
            
            db.ExecuteNonQuery(query, param);
            new AuditoriaRepository().Registrar("ATUALIZAR", "Produtos", $"Atualizado produto '{p.Nome}' (Custo: {p.PrecoCusto:C2}, Venda: {p.PrecoVenda:C2}, Estoque: {p.Estoque}).");
        }

        public void DeduzirEstoque(int idProduto, decimal quantidade)
        {
            string query = "UPDATE Produtos SET Estoque = ISNULL(Estoque, 0) - @Qtd WHERE Id = @Id";
            SqlParameter[] p = {
                new SqlParameter("@Qtd", quantidade),
                new SqlParameter("@Id", idProduto)
            };
            db.ExecuteNonQuery(query, p);
        }

        public DataTable BuscarAlertasEstoque()
        {
            string query = "SELECT Id, Nome, Estoque, EstoqueMinimo FROM Produtos WHERE Estoque <= EstoqueMinimo ORDER BY Estoque ASC";
            return db.ExecuteQuery(query);
        }

        public Produto BuscarPorId(int id)
        {
            string query = "SELECT * FROM Produtos WHERE Id = @Id";
            SqlParameter[] param = { new SqlParameter("@Id", id) };
            DataTable dt = db.ExecuteQuery(query, param);
            if (dt.Rows.Count > 0) return MapRowToProduto(dt.Rows[0]);
            return null;
        }

        public Produto BuscarPorCodigo(string codigo)
        {
            string query = "SELECT * FROM Produtos WHERE (CodigoBarras = @Codigo OR CodigoInterno = @Codigo) AND Ativo = 1";
            SqlParameter[] param = { new SqlParameter("@Codigo", codigo) };
            
            DataTable dt = db.ExecuteQuery(query, param);
            if (dt.Rows.Count > 0)
            {
                return MapRowToProduto(dt.Rows[0]);
            }
            return null;
        }

        private Produto MapRowToProduto(DataRow row)
        {
            return new Produto
            {
                Id = Convert.ToInt32(row["Id"]),
                IdCategoria = row["IdCategoria"] != DBNull.Value ? Convert.ToInt32(row["IdCategoria"]) : (int?)null,
                IdFornecedor = row.Table.Columns.Contains("IdFornecedor") && row["IdFornecedor"] != DBNull.Value ? Convert.ToInt32(row["IdFornecedor"]) : (int?)null,
                CodigoInterno = row["CodigoInterno"] != DBNull.Value ? row["CodigoInterno"].ToString() : null,
                CodigoBarras = row["CodigoBarras"] != DBNull.Value ? row["CodigoBarras"].ToString() : null,
                Nome = row["Nome"].ToString(),
                Marca = row["Marca"] != DBNull.Value ? row["Marca"].ToString() : null,
                Modelo = row["Modelo"] != DBNull.Value ? row["Modelo"].ToString() : null,
                Descricao = row["Descricao"] != DBNull.Value ? row["Descricao"].ToString() : null,
                PrecoCusto = row["PrecoCusto"] != DBNull.Value ? Convert.ToDecimal(row["PrecoCusto"]) : 0,
                PrecoVenda = row["PrecoVenda"] != DBNull.Value ? Convert.ToDecimal(row["PrecoVenda"]) : 0,
                Margem = row["Margem"] != DBNull.Value ? Convert.ToDecimal(row["Margem"]) : 0,
                Estoque = row["Estoque"] != DBNull.Value ? Convert.ToInt32(row["Estoque"]) : 0,
                EstoqueMinimo = row["EstoqueMinimo"] != DBNull.Value ? Convert.ToInt32(row["EstoqueMinimo"]) : 0,
                Garantia = row["Garantia"] != DBNull.Value ? row["Garantia"].ToString() : null,
                ImagemUrl = row.Table.Columns.Contains("ImagemUrl") && row["ImagemUrl"] != DBNull.Value ? row["ImagemUrl"].ToString() : null,
                Status = row["Status"] != DBNull.Value ? row["Status"].ToString() : "Ativo",
                Ativo = row["Ativo"] != DBNull.Value ? Convert.ToBoolean(row["Ativo"]) : true,
                CategoriaNome = row.Table.Columns.Contains("CategoriaNome") && row["CategoriaNome"] != DBNull.Value ? row["CategoriaNome"].ToString() : null,
                FornecedorNome = row.Table.Columns.Contains("FornecedorNome") && row["FornecedorNome"] != DBNull.Value ? row["FornecedorNome"].ToString() : null
            };
        }
    }
}
