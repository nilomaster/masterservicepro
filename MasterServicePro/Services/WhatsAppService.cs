using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Models;

namespace MasterServicePro.Services
{
    public class WhatsAppService
    {
        private readonly ConfiguracaoWhatsAppRepository _configRepo = new ConfiguracaoWhatsAppRepository();
        private readonly ClienteRepository _clienteRepo = new ClienteRepository();

        public async Task EnviarNotificacaoOSAsync(OrdemServico os, string tipoTemplate)
        {
            try
            {
                var config = _configRepo.Buscar();
                if (config == null) return;

                // Obter cliente
                string nomeCliente = os.ClienteFinal;
                string telefone = "";

                if (os.IdCliente > 0)
                {
                    var cliente = _clienteRepo.BuscarPorId(os.IdCliente);
                    if (cliente != null)
                    {
                        nomeCliente = cliente.Nome;
                        telefone = !string.IsNullOrWhiteSpace(cliente.WhatsApp) ? cliente.WhatsApp : cliente.Telefone;
                    }
                }

                if (string.IsNullOrWhiteSpace(telefone))
                {
                    // Se não tiver telefone, não há como enviar
                    return;
                }

                // Escolher template
                string template = "";
                if (tipoTemplate.Equals("Abertura", StringComparison.OrdinalIgnoreCase))
                    template = config.TemplateAbertura;
                else if (tipoTemplate.Equals("Finalizado", StringComparison.OrdinalIgnoreCase))
                    template = config.TemplateFinalizado;
                else
                    template = config.TemplateAtualizacao;

                if (string.IsNullOrWhiteSpace(template)) return;

                // Substituir variáveis
                string mensagem = SubstituirVariaveis(template, nomeCliente, os);

                // Enviar
                await EnviarMensagemAsync(telefone, mensagem, config);
            }
            catch (Exception ex)
            {
                // Logar ou ignorar silenciosamente para não quebrar fluxo principal do ERP
                Debug.WriteLine("Erro ao enviar notificação de WhatsApp: " + ex.Message);
            }
        }

        public async Task EnviarRelatorioFechamentoCaixaAsync(Models.Caixa caixa, FinanceiroRepository repo)
        {
            try
            {
                var config = _configRepo.Buscar();
                if (config == null || string.IsNullOrWhiteSpace(config.TelefoneAdministrador))
                {
                    return;
                }

                // Compile cash data
                decimal totalVendas = repo.GetTotalPorPeriodo(caixa.DataAbertura, DateTime.Now, "Entrada");
                decimal totalSaidas = repo.GetTotalPorPeriodo(caixa.DataAbertura, DateTime.Now, "Saída");
                decimal saldoFinal = caixa.ValorAbertura + totalVendas - totalSaidas;

                decimal pmPix = repo.GetEntradasPorFormaPagamento(caixa.DataAbertura, DateTime.Now, "Pix");
                decimal pmDinheiro = repo.GetEntradasPorFormaPagamento(caixa.DataAbertura, DateTime.Now, "Dinheiro");
                decimal pmCredito = repo.GetEntradasPorFormaPagamento(caixa.DataAbertura, DateTime.Now, "Cartão de Crédito");
                decimal pmDebito = repo.GetEntradasPorFormaPagamento(caixa.DataAbertura, DateTime.Now, "Cartão de Débito");

                // Physical cash in drawer
                decimal saidasDinheiro = repo.GetSaidasPorFormaPagamento(caixa.DataAbertura, DateTime.Now, "Dinheiro");
                decimal dinheiroEmCaixa = caixa.ValorAbertura + pmDinheiro - saidasDinheiro;

                string mensagem = $"📊 *Fechamento de Caixa - MasterServicePro*\n\n" +
                                  $"Olá {config.NomeAdministrador},\n" +
                                  $"O caixa acaba de ser fechado. Segue o resumo financeiro desta sessão:\n\n" +
                                  $"*Resumo de Totais:*\n" +
                                  $"💵 Saldo Inicial: *{caixa.ValorAbertura:C2}*\n" +
                                  $"🟢 Entradas (Vendas/OS): *{totalVendas:C2}*\n" +
                                  $"🔴 Saídas (Despesas/Sangrias): *{totalSaidas:C2}*\n" +
                                  $"💰 Dinheiro Físico no Caixa: *{dinheiroEmCaixa:C2}*\n" +
                                  $"🏦 *Saldo Geral: {saldoFinal:C2}*\n\n" +
                                  $"*Entradas por Forma de Pagamento:*\n" +
                                  $"🔸 Pix: {pmPix:C2}\n" +
                                  $"🔸 Dinheiro: {pmDinheiro:C2}\n" +
                                  $"🔸 Crédito: {pmCredito:C2}\n" +
                                  $"🔸 Débito: {pmDebito:C2}\n\n" +
                                  $"🕒 *Abertura:* {caixa.DataAbertura:dd/MM/yyyy HH:mm}\n" +
                                  $"🕒 *Fechamento:* {DateTime.Now:dd/MM/yyyy HH:mm}";

                await EnviarMensagemAsync(config.TelefoneAdministrador, mensagem, config);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erro ao enviar relatório de caixa WhatsApp: " + ex.Message);
            }
        }

        public string SubstituirVariaveis(string template, string nomeCliente, OrdemServico os)
        {
            if (string.IsNullOrEmpty(template)) return "";

            string aparelho = $"{os.Marca} {os.Modelo}".Trim();
            string valorTotalStr = os.ValorTotal.ToString("F2");

            string msg = template;
            msg = msg.Replace("{Cliente}", nomeCliente ?? "");
            msg = msg.Replace("{OS}", os.Id.ToString());
            msg = msg.Replace("{Aparelho}", aparelho);
            msg = msg.Replace("{Valor}", valorTotalStr);
            msg = msg.Replace("{Status}", os.Status ?? "");
            msg = msg.Replace("{Defeito}", os.Defeito ?? "");
            msg = msg.Replace("{Laudo}", os.LaudoTecnico ?? "");

            return msg;
        }

        public async Task EnviarMensagemAsync(string telefone, string mensagem, ConfiguracaoWhatsApp config = null)
        {
            if (config == null)
            {
                config = _configRepo.Buscar();
            }

            string foneLimpo = LimparTelefone(telefone);
            if (string.IsNullOrEmpty(foneLimpo)) return;

            if (config.UsarAPI && !string.IsNullOrWhiteSpace(config.ApiUrl))
            {
                await EnviarViaAPIAsync(foneLimpo, mensagem, config);
            }
            else
            {
                EnviarViaBrowser(foneLimpo, mensagem);
            }
        }

        private async Task EnviarViaAPIAsync(string telefone, string mensagem, ConfiguracaoWhatsApp config)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(15);

                    string url = config.ApiUrl;
                    if (!string.IsNullOrEmpty(config.Instancia))
                    {
                        url = url.Replace("{instance}", config.Instancia)
                                 .Replace("{instancia}", config.Instancia);
                    }

                    // Construir payload compatível com múltiplos gateways
                    // Evolution API: {"number": "...", "text": "..."}
                    // Z-API: {"phone": "...", "message": "..."}
                    // Outros: {"to": "...", "body": "..."}
                    string jsonPayload = $@"{{
                        ""number"": ""{telefone}"",
                        ""phone"": ""{telefone}"",
                        ""to"": ""{telefone}"",
                        ""text"": {HttpUtilityJavaScriptStringEncode(mensagem)},
                        ""message"": {HttpUtilityJavaScriptStringEncode(mensagem)},
                        ""body"": {HttpUtilityJavaScriptStringEncode(mensagem)}
                    }}";

                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    // Adicionar headers de autorização comuns
                    if (!string.IsNullOrWhiteSpace(config.ApiToken))
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation("apikey", config.ApiToken);
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {config.ApiToken}");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Client-Token", config.ApiToken);
                        client.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", config.ApiToken);
                    }

                    var response = await client.PostAsync(url, content);
                    if (!response.IsSuccessStatusCode)
                    {
                        string respText = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine($"API de WhatsApp retornou erro: {response.StatusCode} - {respText}");
                        // Em caso de erro na API, podemos tentar fallback via browser
                        EnviarViaBrowser(telefone, mensagem);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erro de conexão na API do WhatsApp: " + ex.Message);
                // Fallback imediato
                EnviarViaBrowser(telefone, mensagem);
            }
        }

        private void EnviarViaBrowser(string telefone, string mensagem)
        {
            try
            {
                string msgEscaped = Uri.EscapeDataString(mensagem);
                string url = $"https://api.whatsapp.com/send?phone={telefone}&text={msgEscaped}";
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível abrir o navegador: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string LimparTelefone(string tel)
        {
            if (string.IsNullOrEmpty(tel)) return "";
            
            // Apenas números
            var sb = new StringBuilder();
            foreach (char c in tel)
            {
                if (char.IsDigit(c)) sb.Append(c);
            }

            string num = sb.ToString();
            
            // Garantir DDI (Brasil = 55) se tiver 10 ou 11 dígitos
            if (num.Length == 10 || num.Length == 11)
            {
                num = "55" + num;
            }

            return num;
        }

        private string HttpUtilityJavaScriptStringEncode(string value)
        {
            if (string.IsNullOrEmpty(value)) return "\"\"";
            
            var sb = new StringBuilder();
            sb.Append("\"");
            foreach (char c in value)
            {
                switch (c)
                {
                    case '\"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        if (c < ' ')
                        {
                            sb.Append(string.Format("\\u{0:x4}", (int)c));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            sb.Append("\"");
            return sb.ToString();
        }
    }
}
