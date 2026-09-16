using System;
using System.Data;
using System.Linq;
using Dapper;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.DAL
{
    public class ConfiguracaoWhatsAppRepository
    {
        private DbConnection db = new DbConnection();
        private static bool _tabelaCriada = false;
        private static readonly object _lock = new object();

        public ConfiguracaoWhatsAppRepository()
        {
            try { CriarTabelaSeNaoExistir(); } catch { }
        }

        private void CriarTabelaSeNaoExistir()
        {
            if (_tabelaCriada) return;
            lock (_lock)
            {
                if (_tabelaCriada) return;

                var result = db.ExecuteScalar("SELECT object_id FROM sys.objects WHERE object_id = OBJECT_ID('ConfiguracoesWhatsApp') AND type in (N'U')");
                if (result == null || result == DBNull.Value)
                {
                    db.ExecuteNonQuery(@"
                        CREATE TABLE ConfiguracoesWhatsApp (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            UsarAPI BIT DEFAULT 0,
                            ApiUrl VARCHAR(MAX),
                            ApiToken VARCHAR(MAX),
                            Instancia VARCHAR(100),
                            TemplateAbertura VARCHAR(MAX),
                            TemplateFinalizado VARCHAR(MAX),
                            TemplateAtualizacao VARCHAR(MAX),
                            NomeAdministrador VARCHAR(100),
                            TelefoneAdministrador VARCHAR(50)
                        )");

                    // Insere registro inicial padrão
                    db.ExecuteNonQuery(@"
                        INSERT INTO ConfiguracoesWhatsApp (UsarAPI, ApiUrl, ApiToken, Instancia, TemplateAbertura, TemplateFinalizado, TemplateAtualizacao, NomeAdministrador, TelefoneAdministrador)
                        VALUES (0, '', '', '', 
                        'Olá {Cliente}, sua O.S. #{OS} foi aberta com sucesso para o aparelho {Aparelho}. Defeito relatado: {Defeito}. Acompanhe o status com a gente!', 
                        'Olá {Cliente}, boas notícias! O reparo do seu aparelho {Aparelho} (O.S. #{OS}) foi concluído com sucesso. Valor total: R$ {Valor}. Já está disponível para retirada!', 
                        'Olá {Cliente}, o status da sua O.S. #{OS} ({Aparelho}) foi atualizado para: {Status}.', '', '')");
                }
                else
                {
                    // Migração de colunas novas
                    string[] colunasNovas = { 
                        "NomeAdministrador VARCHAR(100)", 
                        "TelefoneAdministrador VARCHAR(50)" 
                    };

                    foreach (var col in colunasNovas)
                    {
                        string nomeCol = col.Split(' ')[0];
                        string checkCol = $"IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ConfiguracoesWhatsApp') AND name = '{nomeCol}') " +
                                         $"ALTER TABLE ConfiguracoesWhatsApp ADD {col};";
                        try { db.ExecuteNonQuery(checkCol); } catch { }
                    }
                }
                _tabelaCriada = true;
            }
        }

        public ConfiguracaoWhatsApp Buscar()
        {
            using (var conn = db.GetConnection())
            {
                var config = conn.QueryFirstOrDefault<ConfiguracaoWhatsApp>("SELECT TOP 1 * FROM ConfiguracoesWhatsApp");
                if (config == null)
                {
                    // Fallback de segurança se deletarem
                    config = new ConfiguracaoWhatsApp
                    {
                        UsarAPI = false,
                        TemplateAbertura = "Olá {Cliente}, sua O.S. #{OS} foi aberta com sucesso para o aparelho {Aparelho}. Defeito relatado: {Defeito}. Acompanhe o status com a gente!",
                        TemplateFinalizado = "Olá {Cliente}, boas notícias! O reparo do seu aparelho {Aparelho} (O.S. #{OS}) foi concluído com sucesso. Valor total: R$ {Valor}. Já está disponível para retirada!",
                        TemplateAtualizacao = "Olá {Cliente}, o status da sua O.S. #{OS} ({Aparelho}) foi atualizado para: {Status}.",
                        NomeAdministrador = "",
                        TelefoneAdministrador = ""
                    };
                }
                return config;
            }
        }

        public void Salvar(ConfiguracaoWhatsApp config)
        {
            using (var conn = db.GetConnection())
            {
                var existe = conn.ExecuteScalar<int>("SELECT COUNT(1) FROM ConfiguracoesWhatsApp");
                if (existe > 0)
                {
                    string query = @"UPDATE ConfiguracoesWhatsApp 
                                     SET UsarAPI = @UsarAPI, ApiUrl = @ApiUrl, ApiToken = @ApiToken, Instancia = @Instancia,
                                         TemplateAbertura = @TemplateAbertura, TemplateFinalizado = @TemplateFinalizado, 
                                         TemplateAtualizacao = @TemplateAtualizacao,
                                         NomeAdministrador = @NomeAdministrador, TelefoneAdministrador = @TelefoneAdministrador";
                    conn.Execute(query, config);
                }
                else
                {
                    string query = @"INSERT INTO ConfiguracoesWhatsApp (UsarAPI, ApiUrl, ApiToken, Instancia, TemplateAbertura, TemplateFinalizado, TemplateAtualizacao, NomeAdministrador, TelefoneAdministrador)
                                     VALUES (@UsarAPI, @ApiUrl, @ApiToken, @Instancia, @TemplateAbertura, @TemplateFinalizado, @TemplateAtualizacao, @NomeAdministrador, @TelefoneAdministrador)";
                    conn.Execute(query, config);
                }
            }
        }
    }
}
