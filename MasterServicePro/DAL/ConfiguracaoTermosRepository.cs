using System;
using System.Data;
using Dapper;
using MasterServicePro.Utils;
using MasterServicePro.Models;

namespace MasterServicePro.DAL
{
    public class ConfiguracaoTermosRepository
    {
        private DbConnection db = new DbConnection();
        private static bool _tabelaCriada = false;
        private static readonly object _lock = new object();

        private const string DefaultTermos = 
            "TERMOS E CONDIÇÕES:\n" +
            "1. Garantia de 90 dias para os serviços prestados e peças trocadas, conforme o Código de Defesa do Consumidor.\n" +
            "2. Aparelhos não retirados após 90 dias da data de conclusão do serviço poderão ser vendidos para custear despesas da loja.\n" +
            "3. Não nos responsabilizamos por chips, cartões de memória ou capas deixadas no aparelho, a não ser que estejam descritos na O.S.";

        public ConfiguracaoTermosRepository()
        {
            try { CriarTabelaSeNaoExistir(); } catch { }
        }

        private void CriarTabelaSeNaoExistir()
        {
            if (_tabelaCriada) return;
            lock (_lock)
            {
                if (_tabelaCriada) return;

                var result = db.ExecuteScalar("SELECT object_id FROM sys.objects WHERE object_id = OBJECT_ID('ConfiguracoesTermos') AND type in (N'U')");
                if (result == null || result == DBNull.Value)
                {
                    using (var conn = db.GetConnection())
                    {
                        conn.Execute(@"
                            CREATE TABLE ConfiguracoesTermos (
                                Id INT PRIMARY KEY IDENTITY(1,1),
                                NomeLoja VARCHAR(150),
                                Cnpj VARCHAR(50),
                                Contato VARCHAR(100),
                                Endereco VARCHAR(255),
                                TermosOS VARCHAR(MAX)
                            )");

                        // Insere registro inicial padrão
                        conn.Execute(@"INSERT INTO ConfiguracoesTermos (NomeLoja, Contato, Endereco, TermosOS) 
                                       VALUES ('MASTER SERVICE PRO', '(11) 99999-9999', 'Seu endereço aqui, 123 - Centro - Cidade/UF', @Termos)", new { Termos = DefaultTermos });
                    }
                }
                else
                {
                    // Update table if missing columns
                    using (var conn = db.GetConnection())
                    {
                        try
                        {
                            var hasNome = conn.ExecuteScalar<int>("SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('ConfiguracoesTermos') AND name = 'NomeLoja'");
                            if (hasNome == 0)
                            {
                                conn.Execute("ALTER TABLE ConfiguracoesTermos ADD NomeLoja VARCHAR(150), Cnpj VARCHAR(50), Contato VARCHAR(100), Endereco VARCHAR(255)");
                                conn.Execute("UPDATE ConfiguracoesTermos SET NomeLoja = 'MASTER SERVICE PRO', Contato = '(11) 99999-9999', Endereco = 'Seu endereço aqui, 123 - Centro - Cidade/UF'");
                            }
                        }
                        catch { }
                    }
                }
                _tabelaCriada = true;
            }
        }

        public ConfiguracaoImpressao ObterConfiguracao()
        {
            try
            {
                using (var conn = db.GetConnection())
                {
                    var conf = conn.QueryFirstOrDefault<ConfiguracaoImpressao>("SELECT TOP 1 * FROM ConfiguracoesTermos");
                    if (conf == null)
                    {
                        return new ConfiguracaoImpressao
                        {
                            NomeLoja = "MASTER SERVICE PRO",
                            Contato = "(11) 99999-9999",
                            Endereco = "Seu endereço aqui, 123 - Centro - Cidade/UF",
                            TermosOS = DefaultTermos
                        };
                    }
                    if (string.IsNullOrEmpty(conf.TermosOS)) conf.TermosOS = DefaultTermos;
                    if (string.IsNullOrEmpty(conf.NomeLoja)) conf.NomeLoja = "MASTER SERVICE PRO";
                    return conf;
                }
            }
            catch
            {
                return new ConfiguracaoImpressao
                {
                    NomeLoja = "MASTER SERVICE PRO",
                    Contato = "(11) 99999-9999",
                    Endereco = "Seu endereço aqui, 123 - Centro - Cidade/UF",
                    TermosOS = DefaultTermos
                };
            }
        }

        public string ObterTermos()
        {
            return ObterConfiguracao().TermosOS;
        }

        public void Salvar(ConfiguracaoImpressao conf)
        {
            using (var conn = db.GetConnection())
            {
                var existe = conn.ExecuteScalar<int>("SELECT COUNT(1) FROM ConfiguracoesTermos");
                if (existe > 0)
                {
                    conn.Execute("UPDATE ConfiguracoesTermos SET NomeLoja = @NomeLoja, Cnpj = @Cnpj, Contato = @Contato, Endereco = @Endereco, TermosOS = @TermosOS", conf);
                }
                else
                {
                    conn.Execute("INSERT INTO ConfiguracoesTermos (NomeLoja, Cnpj, Contato, Endereco, TermosOS) VALUES (@NomeLoja, @Cnpj, @Contato, @Endereco, @TermosOS)", conf);
                }
            }
        }
    }
}
