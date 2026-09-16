using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MasterServicePro.Forms
{
    public partial class FrmPDVWeb : Form
    {
        // Mock data
        private double totalValue = 0.00;
        private double restValue = 0.00;
        private double changeValue = 0.00;
        private List<dynamic> items = new List<dynamic>();

        public FrmPDVWeb()
        {
            InitializeComponent();
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            try
            {
                // Ensure environment is initialized
                await webView.EnsureCoreWebView2Async(null);
                
                // Get absolute path to the HTML file
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"PDV\index.html");
                path = Path.GetFullPath(path);
                
                webView.CoreWebView2.Navigate(path);

                // Listen for messages from JavaScript
                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            }
            catch (System.Runtime.InteropServices.COMException ex) when ((uint)ex.ErrorCode == 0x80004004 || (uint)ex.ErrorCode == 0x80040154)
            {
                // Ignorar erros de form fechado ou WebView2 destruído prematuramente
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inicializar o WebView2: " + ex.Message);
            }
        }

        private async void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string jsonString = e.WebMessageAsJson;
                dynamic message = JsonConvert.DeserializeObject(jsonString);

                string actionName = message.action.ToString();

                switch (actionName)
                {
                    case "SEARCH_PRODUCTS":
                        try {
                            string query = message.query;
                            var repoBusca = new MasterServicePro.DAL.ProdutoRepository();
                            var todos = repoBusca.BuscarTodos();
                            
                            query = query ?? "";
                            var termos = query.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            var filtrados = string.IsNullOrWhiteSpace(query) 
                                ? new List<MasterServicePro.Models.Produto>() 
                                : todos.Where(p => 
                                    (p.Nome != null && termos.All(t => p.Nome.ToLower().Contains(t))) || 
                                    (p.CodigoBarras != null && p.CodigoBarras.ToLower().Contains(query.ToLower())) ||
                                    (p.CodigoInterno != null && p.CodigoInterno.ToLower().Contains(query.ToLower())) ||
                                    (p.CategoriaNome != null && p.CategoriaNome.ToLower().Contains(query.ToLower()))
                                ).Take(15).ToList();
                                                  
                            string jsonResult = JsonConvert.SerializeObject(filtrados);
                            string safeJson = jsonResult.Replace("\\", "\\\\").Replace("'", "\\'");
                            await webView.CoreWebView2.ExecuteScriptAsync($"renderSearchResults('{safeJson}')");
                        } catch (Exception) {
                            // Ignorar falhas de busca silenciosamente, pois o front-end apenas não atualizará
                        }
                        break;
                        
                    case "ADD_SELECTED_ITEM":
                        int productId = (int)message.id;
                        var repoAdd = new MasterServicePro.DAL.ProdutoRepository();
                        var produto = repoAdd.BuscarPorId(productId);
                        
                        if (produto != null)
                        {
                            var newItem = new {
                                id = produto.Id,
                                name = produto.Nome,
                                sub = produto.Marca,
                                qty = 1,
                                price = produto.PrecoVenda,
                                total = produto.PrecoVenda * 1
                            };
                            items.Add(newItem);
                            totalValue += Convert.ToDouble(produto.PrecoVenda);
                            UpdateWebUI();
                        }
                        break;
                        
                    case "UPDATE_QTY":
                        int idx = (int)message.index;
                        int newQty = (int)message.qty;
                        if (idx >= 0 && idx < items.Count && newQty > 0)
                        {
                            var oldItem = items[idx];
                            var repoQt = new MasterServicePro.DAL.ProdutoRepository();
                            var pQt = repoQt.BuscarPorId((int)oldItem.id);
                            
                            if (pQt != null && pQt.Estoque < newQty)
                            {
                                MessageBox.Show($"Estoque insuficiente. Estoque disponível: {pQt.Estoque}", "Estoque Insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                UpdateWebUI();
                                break;
                            }
                            
                            totalValue -= (double)oldItem.total;
                            var updatedItem = new {
                                id = oldItem.id,
                                name = oldItem.name,
                                sub = oldItem.sub,
                                qty = newQty,
                                price = oldItem.price,
                                total = (double)oldItem.price * newQty
                            };
                            items[idx] = updatedItem;
                            totalValue += (double)updatedItem.total;
                            UpdateWebUI();
                        }
                        break;
                        
                    case "FINALIZAR_VENDA":
                        if (items.Count == 0) return;
                        MessageBox.Show("Venda de R$ " + totalValue.ToString("F2") + " finalizada com sucesso!", "PDV", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Reset mock state
                        items.Clear();
                        totalValue = 0;
                        UpdateWebUI();
                        break;

                    case "CANCELAR_VENDA":
                        if (items.Count == 0) return;
                        if (MessageBox.Show("Deseja realmente cancelar esta venda?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            items.Clear();
                            totalValue = 0;
                            UpdateWebUI();
                        }
                        break;

                    case "REMOVER_ITEM":
                        if (items.Count > 0)
                        {
                            var lastItem = items[items.Count - 1];
                            items.RemoveAt(items.Count - 1);
                            totalValue -= (double)lastItem.total;
                            if (totalValue < 0) totalValue = 0;
                            UpdateWebUI();
                        }
                        break;

                    case "SEARCH_CLIENTS":
                        string clientQuery = message.query;
                        var repoCliBusca = new MasterServicePro.DAL.ClienteRepository();
                        var todosCli = await repoCliBusca.BuscarTodosAsync();
                        
                        var filtradosCli = string.IsNullOrWhiteSpace(clientQuery) 
                            ? todosCli.Take(15).ToList() 
                            : todosCli.Where(c => c.Nome.ToLower().Contains(clientQuery.ToLower()) || 
                                              (c.CpfCnpj != null && c.CpfCnpj.Contains(clientQuery))).Take(15).ToList();
                                              
                        string cliJsonResult = JsonConvert.SerializeObject(filtradosCli);
                        string safeCliJsString = JsonConvert.SerializeObject(cliJsonResult);
                        await webView.CoreWebView2.ExecuteScriptAsync($"renderClientSearchResults({safeCliJsString})");
                        break;

                    case "SELECT_CLIENT":
                        int cliId = (int)message.id;
                        var repoCliAdd = new MasterServicePro.DAL.ClienteRepository();
                        var cliente = repoCliAdd.BuscarPorId(cliId);
                        if (cliente != null)
                        {
                            var clientData = new {
                                id = cliente.Id,
                                name = cliente.Nome,
                                balance = cliente.Saldo
                            };
                            string clientJson = JsonConvert.SerializeObject(clientData);
                            string safeJsString = JsonConvert.SerializeObject(clientJson);
                            _ = webView.CoreWebView2.ExecuteScriptAsync($"updateClient({safeJsString})");
                        }
                        break;

                    case "REMOVER_CLIENTE":
                        var finalClientData = new {
                            id = 0,
                            name = "NENHUM CLIENTE",
                            balance = 0.0
                        };
                        string finalClientJson = JsonConvert.SerializeObject(finalClientData);
                        string finalSafeJsString = JsonConvert.SerializeObject(finalClientJson);
                        _ = webView.CoreWebView2.ExecuteScriptAsync($"updateClient({finalSafeJsString})");
                        break;

                    case "CONFIRMAR_DESCONTO":
                        double desc = (double)message.valor;
                        if (desc > 0 && desc <= totalValue)
                        {
                            totalValue -= desc;
                            UpdateWebUI();
                        }
                        break;

                    case "CONFIRMAR_FINALIZAR":
                        if (items.Count == 0) return;
                        
                        double recebido = Convert.ToDouble(message.recebido);
                        string metodo = message.metodo.ToString();
                        double saldoUsado = message.saldoUsado != null ? Convert.ToDouble(message.saldoUsado) : 0;
                        int clientId = message.clientId != null ? Convert.ToInt32(message.clientId) : 0;
                        
                        var financeiroRepository = new MasterServicePro.DAL.FinanceiroRepository();
                        var caixa = financeiroRepository.ObterCaixaAberto();
                        if (caixa == null)
                        {
                            MessageBox.Show("Atenção: O caixa não está aberto. Por favor, abra o caixa antes de realizar vendas.", "Caixa Fechado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        
                        // Verifica estoque antes de prosseguir
                        var repoProdutoEstoque = new MasterServicePro.DAL.ProdutoRepository();
                        foreach (var it in items)
                        {
                            var produtoCheck = repoProdutoEstoque.BuscarPorId((int)it.id);
                            if (produtoCheck != null && produtoCheck.Estoque < (decimal)it.qty)
                            {
                                MessageBox.Show($"Estoque insuficiente para o produto '{produtoCheck.Nome}'.\nEstoque atual: {produtoCheck.Estoque}\nQuantidade solicitada: {it.qty}", "Estoque Insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }

                        if (saldoUsado > 0 && clientId > 0)
                        {
                            var repoCli = new MasterServicePro.DAL.ClienteRepository();
                            repoCli.DescontarSaldo(clientId, (decimal)saldoUsado);
                        }
                        
                        double subtotal = 0;
                        var vendaItens = new List<MasterServicePro.Models.VendaItem>();
                        foreach (var it in items)
                        {
                            subtotal += (double)it.total;
                            vendaItens.Add(new MasterServicePro.Models.VendaItem {
                                IdProduto = (int)it.id,
                                Quantidade = (decimal)it.qty,
                                PrecoUnitario = (decimal)it.price
                            });
                        }
                        
                        double desconto = subtotal - totalValue;
                        if (desconto < 0) desconto = 0;
                        
                        
                        string metodoFormatado = metodo;
                        if (metodo.Equals("cartao_credito", StringComparison.OrdinalIgnoreCase)) metodoFormatado = "Cartão de Crédito";
                        else if (metodo.Equals("cartao_debito", StringComparison.OrdinalIgnoreCase)) metodoFormatado = "Cartão de Débito";
                        else if (metodo.Equals("dinheiro", StringComparison.OrdinalIgnoreCase)) metodoFormatado = "Dinheiro";
                        else if (metodo.Equals("pix", StringComparison.OrdinalIgnoreCase)) metodoFormatado = "Pix";
                        else if (metodo.Equals("saldo_cliente", StringComparison.OrdinalIgnoreCase)) metodoFormatado = "Saldo do Cliente";

                        var repoVenda = new MasterServicePro.DAL.VendaRepository();
                        int vendaIdGerada = 0;
                        try
                        {
                            vendaIdGerada = repoVenda.SalvarVenda(clientId, MasterServicePro.DAL.AuthSession.Id > 0 ? MasterServicePro.DAL.AuthSession.Id : 1, (decimal)subtotal, (decimal)desconto, (decimal)totalValue, metodoFormatado, vendaItens);
                        }
                        catch { } // Ignore if error logging history
                        
                        double valorRecebidoFinanceiro = totalValue - saldoUsado;
                        if (valorRecebidoFinanceiro > 0)
                        {
                            var nomesList = new List<string>();
                            foreach (var it in items) {
                                nomesList.Add((string)it.name);
                            }
                            string nomesProdutos = string.Join(", ", nomesList);
                            if (nomesProdutos.Length > 45) nomesProdutos = nomesProdutos.Substring(0, 45) + "...";

                            if (metodo.Equals("misto", StringComparison.OrdinalIgnoreCase))
                            {
                                double mistoDinheiro = message.mistoDinheiro != null ? Convert.ToDouble(message.mistoDinheiro) : 0;
                                double mistoPix = message.mistoPix != null ? Convert.ToDouble(message.mistoPix) : 0;
                                double mistoCartao = message.mistoCartao != null ? Convert.ToDouble(message.mistoCartao) : 0;
                                
                                double troco = recebido - totalValue;
                                if (troco > 0) {
                                    if (mistoDinheiro >= troco) {
                                        mistoDinheiro -= troco;
                                    } else {
                                        troco -= mistoDinheiro;
                                        mistoDinheiro = 0;
                                    }
                                }

                                if (mistoDinheiro > 0)
                                {
                                    financeiroRepository.LançarMovimentacao(new MasterServicePro.Models.Movimentacao
                                    {
                                        IdCaixa = caixa.Id,
                                        Tipo = "Entrada",
                                        Categoria = "Venda PDV",
                                        Subcategoria = nomesProdutos,
                                        Valor = (decimal)mistoDinheiro,
                                        FormaPagamento = "Dinheiro",
                                        Descricao = "Venda finalizada no PDV Web (Misto)"
                                    });
                                }
                                if (mistoPix > 0)
                                {
                                    financeiroRepository.LançarMovimentacao(new MasterServicePro.Models.Movimentacao
                                    {
                                        IdCaixa = caixa.Id,
                                        Tipo = "Entrada",
                                        Categoria = "Venda PDV",
                                        Subcategoria = nomesProdutos,
                                        Valor = (decimal)mistoPix,
                                        FormaPagamento = "Pix",
                                        Descricao = "Venda finalizada no PDV Web (Misto)"
                                    });
                                }
                                if (mistoCartao > 0)
                                {
                                    financeiroRepository.LançarMovimentacao(new MasterServicePro.Models.Movimentacao
                                    {
                                        IdCaixa = caixa.Id,
                                        Tipo = "Entrada",
                                        Categoria = "Venda PDV",
                                        Subcategoria = nomesProdutos,
                                        Valor = (decimal)mistoCartao,
                                        FormaPagamento = "Cartão de Crédito",
                                        Descricao = "Venda finalizada no PDV Web (Misto)"
                                    });
                                }
                            }
                            else if (metodo.IndexOf("Prazo", StringComparison.OrdinalIgnoreCase) >= 0 || metodo.IndexOf("Fiado", StringComparison.OrdinalIgnoreCase) >= 0 || metodo.IndexOf("Crediário", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                var crRepo = new MasterServicePro.DAL.ContasReceberRepository();
                                crRepo.Inserir(new MasterServicePro.Models.ContaReceber
                                {
                                    IdCliente = clientId,
                                    Descricao = nomesProdutos,
                                    ValorTotal = (decimal)valorRecebidoFinanceiro,
                                    ValorPago = 0,
                                    DataLancamento = DateTime.Now,
                                    DataVencimento = DateTime.Now.AddDays(30),
                                    Status = "Pendente"
                                });
                            }
                            else
                            {
                                financeiroRepository.LançarMovimentacao(new MasterServicePro.Models.Movimentacao
                                {
                                    IdCaixa = caixa.Id,
                                    Tipo = "Entrada",
                                    Categoria = "Venda PDV",
                                    Subcategoria = nomesProdutos,
                                    Valor = (decimal)valorRecebidoFinanceiro,
                                    FormaPagamento = metodoFormatado,
                                    Descricao = "Venda finalizada no PDV Web"
                                });
                            }
                        }
                        
                        var repoProduto = new MasterServicePro.DAL.ProdutoRepository();
                        foreach (var item in vendaItens)
                        {
                            repoProduto.DeduzirEstoque(item.IdProduto, item.Quantidade);
                        }
                        
                        double trocoValue = recebido - totalValue;
                        if (trocoValue < 0) trocoValue = 0;
                        
                        if (MessageBox.Show($"Venda de R$ {totalValue:F2} finalizada com sucesso!\n\nDeseja imprimir o comprovante de venda?", "Imprimir Comprovante", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            if (vendaIdGerada > 0)
                            {
                                this.BeginInvoke(new Action(() => 
                                {
                                    new MasterServicePro.Utils.PDVPrinter().ImprimirVenda(vendaIdGerada, (decimal)recebido, (decimal)trocoValue, metodoFormatado);
                                }));
                            }
                        }
                        
                        items.Clear();
                        totalValue = 0;
                        UpdateWebUI();
                        
                        var clearClientData = new { id = 0, name = "NENHUM CLIENTE", balance = 0.0 };
                        string clearClientJson = JsonConvert.SerializeObject(clearClientData);
                        string clearSafeJsString = JsonConvert.SerializeObject(clearClientJson);
                        _ = webView.CoreWebView2.ExecuteScriptAsync($"updateClient({clearSafeJsString})");
                        break;

                    case "MENU_OS":
                    {
                        this.BeginInvoke(new Action(() => 
                        {
                            using (var frm = new FrmOSWeb()) { frm.ShowDialog(); }
                        }));
                        break;
                    }
                    case "MENU_ABRIR_CAIXA":
                    {
                        var repCaixa = new MasterServicePro.DAL.FinanceiroRepository();
                        if (repCaixa.ObterCaixaAberto() != null)
                        {
                            MessageBox.Show("Atenção: Já existe um caixa aberto no momento!\n\nUm administrador precisa fechar o caixa atual antes que um novo possa ser aberto.", "Caixa Aberto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        using (var frm = new FrmCaixaAbertura()) { frm.ShowDialog(); }
                        break;
                    }
                    case "MENU_ADD_DINHEIRO":
                    {
                        this.BeginInvoke(new Action(() => 
                        {
                            var repCaixaAdd = new MasterServicePro.DAL.FinanceiroRepository();
                            var cAdd = repCaixaAdd.ObterCaixaAberto();
                            if (cAdd == null)
                            {
                                MessageBox.Show("O caixa precisa estar aberto para registrar movimentações.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            using (var frm = new FrmCaixaMovimentacaoWeb("Suprimento"))
                            {
                                if (frm.ShowDialog() == DialogResult.OK)
                                {
                                    var mov = frm.Movimentacao;
                                    mov.IdCaixa = cAdd.Id;
                                    repCaixaAdd.LançarMovimentacao(mov);
                                    MessageBox.Show("Movimentação registrada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }));
                        break;
                    }
                    case "MENU_RETIRAR_DINHEIRO":
                    {
                        this.BeginInvoke(new Action(() => 
                        {
                            var repCaixaRet = new MasterServicePro.DAL.FinanceiroRepository();
                            var cRet = repCaixaRet.ObterCaixaAberto();
                            if (cRet == null)
                            {
                                MessageBox.Show("O caixa precisa estar aberto para registrar movimentações.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            using (var frm = new FrmCaixaMovimentacaoWeb("Sangria"))
                            {
                                if (frm.ShowDialog() == DialogResult.OK)
                                {
                                    var mov = frm.Movimentacao;
                                    mov.IdCaixa = cRet.Id;
                                    repCaixaRet.LançarMovimentacao(mov);
                                    MessageBox.Show("Movimentação registrada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }));
                        break;
                    }
                    case "MENU_HISTORICO":
                    {
                        this.BeginInvoke(new Action(() => 
                        {
                            using (var frm = new FrmCaixaSaidasDiariasWeb()) { frm.ShowDialog(); }
                        }));
                        break;
                    }
                    case "MENU_CONTAS_RECEBER":
                    {
                        this.BeginInvoke(new Action(() => 
                        {
                            using (var frm = new FrmContasReceberWeb()) { frm.ShowDialog(); }
                        }));
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao processar mensagem do JS: " + ex.Message);
            }
        }

        private async void UpdateWebUI()
        {
            // Update items list
            string itemsJson = JsonConvert.SerializeObject(items);
            await webView.CoreWebView2.ExecuteScriptAsync($"updateItemsList('{itemsJson}')");

            // Update totals
            var totals = new {
                total = totalValue,
                rest = restValue,
                change = changeValue
            };
            string totalsJson = JsonConvert.SerializeObject(totals);
            await webView.CoreWebView2.ExecuteScriptAsync($"updateTotals('{totalsJson}')");
        }
    }
}
