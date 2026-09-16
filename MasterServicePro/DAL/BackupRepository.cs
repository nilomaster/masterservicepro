using System;
using System.IO;
using MasterServicePro.Utils;

namespace MasterServicePro.DAL
{
    public class BackupRepository
    {
        private DbConnection db = new DbConnection();

        public string RealizarBackup()
        {
            try
            {
                // Define a pasta de backup (Raiz do projeto \ Backups)
                string pastaBackup = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
                
                if (!Directory.Exists(pastaBackup))
                    Directory.CreateDirectory(pastaBackup);

                string dbName = "MasterPDV";
                using (var conn = db.GetConnection())
                {
                    dbName = conn.Database;
                }

                string nomeArquivo = $"Backup_{dbName}_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                string caminhoCompleto = Path.Combine(pastaBackup, nomeArquivo);

                // Comando SQL para Backup
                // Nota: O SQL Server precisa de permissão de escrita na pasta destino
                string query = $@"BACKUP DATABASE [{dbName}] TO DISK = '{caminhoCompleto}' WITH FORMAT, MEDIANAME = '{dbName}_Backup', NAME = 'Full Backup of {dbName}';";

                db.ExecuteNonQuery(query);

                return caminhoCompleto;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao realizar backup: " + ex.Message);
            }
        }

        public void ZerarProdutos()
        {
            try
            {
                string query = @"
                    IF OBJECT_ID('dbo.ItensVenda', 'U') IS NOT NULL DELETE FROM [dbo].[ItensVenda];
                    IF OBJECT_ID('dbo.OrdemServicoItens', 'U') IS NOT NULL DELETE FROM [dbo].[OrdemServicoItens];
                    IF OBJECT_ID('dbo.Produtos', 'U') IS NOT NULL DELETE FROM [dbo].[Produtos];";
                db.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao zerar produtos: " + ex.Message);
            }
        }

        public void ZerarClientes()
        {
            try
            {
                string query = @"
                    IF OBJECT_ID('dbo.Vendas', 'U') IS NOT NULL UPDATE [dbo].[Vendas] SET [ClienteId] = NULL;
                    IF OBJECT_ID('dbo.OrdemServicoItens', 'U') IS NOT NULL DELETE FROM [dbo].[OrdemServicoItens];
                    IF OBJECT_ID('dbo.OrdensServico', 'U') IS NOT NULL DELETE FROM [dbo].[OrdensServico];
                    IF OBJECT_ID('dbo.ContasReceber', 'U') IS NOT NULL DELETE FROM [dbo].[ContasReceber];
                    IF OBJECT_ID('dbo.Clientes', 'U') IS NOT NULL DELETE FROM [dbo].[Clientes];";
                db.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao zerar clientes: " + ex.Message);
            }
        }

        public void ZerarFinanceiro()
        {
            try
            {
                string query = @"
                    IF OBJECT_ID('dbo.Financeiro', 'U') IS NOT NULL DELETE FROM [dbo].[Financeiro];
                    IF OBJECT_ID('dbo.ContasReceber', 'U') IS NOT NULL DELETE FROM [dbo].[ContasReceber];
                    IF OBJECT_ID('dbo.Caixa', 'U') IS NOT NULL DELETE FROM [dbo].[Caixa];
                    IF OBJECT_ID('dbo.ItensVenda', 'U') IS NOT NULL DELETE FROM [dbo].[ItensVenda];
                    IF OBJECT_ID('dbo.Vendas', 'U') IS NOT NULL DELETE FROM [dbo].[Vendas];";
                db.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao zerar financeiro: " + ex.Message);
            }
        }

        public void ZerarAuditoria()
        {
            try
            {
                string query = @"IF OBJECT_ID('dbo.Auditoria', 'U') IS NOT NULL DELETE FROM [dbo].[Auditoria];";
                db.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao zerar relatórios/auditoria: " + ex.Message);
            }
        }
    }
}
