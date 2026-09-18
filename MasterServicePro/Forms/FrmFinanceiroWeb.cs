using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.IO;
using Newtonsoft.Json;
using MasterServicePro.DAL;
using MasterServicePro.Models;

namespace MasterServicePro.Forms
{
    public class FrmFinanceiroWeb : Form
    {
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private readonly FinanceiroRepository _repo = new FinanceiroRepository();

        public FrmFinanceiroWeb()
        {
            InitializeComponent();
            this.Load += FrmFinanceiroWeb_Load;
        }

        private void FrmFinanceiroWeb_Load(object sender, EventArgs e)
        {
            InitializeAsync();
        }

        private void InitializeComponent()
        {
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();
            
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.FromArgb(26, 29, 36);
            this.webView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView.Location = new System.Drawing.Point(0, 0);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(1200, 750);
            this.webView.TabIndex = 0;
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(26, 29, 36);
            this.ClientSize = new System.Drawing.Size(1200, 750);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Controls.Add(this.webView);
            this.Name = "FrmFinanceiroWeb";
            this.Text = "Financeiro (Web)";
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"Financeiro\index.html");
                path = Path.GetFullPath(path);
                
                if (!File.Exists(path))
                {
                    MessageBox.Show("Erro: Arquivo HTML não encontrado em " + path, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                webView.CoreWebView2.Navigate(path);
                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                webView.NavigationCompleted += (s, e) => { RefreshAllData(); };
            }
            catch (System.Runtime.InteropServices.COMException ex) when ((uint)ex.ErrorCode == 0x80004004 || (uint)ex.ErrorCode == 0x80040154)
            {
                // Ignore errors if form closed or WebView2 destroyed prematurely
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inicializar WebView2: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var json = e.WebMessageAsJson;
                dynamic msg = JsonConvert.DeserializeObject(json);
                string action = msg.action;

                if (action == "add_dinheiro") BtnAddDinheiro_Click();
                else if (action == "retirar_dinheiro") BtnRetirarDinheiro_Click();
                else if (action == "historico") BtnHistorico_Click();
                else if (action == "abrir_caixa") BtnAbrirCaixa_Click();
                else if (action == "fechar_caixa") BtnFecharCaixa_Click();
                else if (action == "filtrar_extrato")
                {
                    string dateStr = msg.payload.ToString();
                    if (DateTime.TryParseExact(dateStr, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dtFiltro))
                    {
                        LoadExtrato(dtFiltro);
                    }
                    else if (DateTime.TryParse(dateStr, out DateTime dtFiltroFallback))
                    {
                        LoadExtrato(dtFiltroFallback);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao processar mensagem web: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void RefreshAllData()
        {
            LoadStats();
            LoadExtrato(DateTime.Today);
            LoadCharts();
        }

        private void LoadStats()
        {
            try
            {
                var caixa = _repo.ObterCaixaAberto();
                bool isOpen = caixa != null;
                int id = isOpen ? caixa.Id : 0;
                
                decimal saldoInicial = isOpen ? caixa.ValorAbertura : 0;
                decimal entradas = 0;
                decimal saidas = 0;
                decimal dinheiroEmCaixa = 0;
                decimal pix = 0;
                decimal din = 0;
                decimal cred = 0;
                decimal deb = 0;

                if (isOpen)
                {
                    DateTime dataAbertura = caixa.DataAbertura;
                    DateTime agora = DateTime.Now;

                    entradas = _repo.GetTotalPorPeriodo(dataAbertura, agora, "Entrada");
                    saidas = _repo.GetTotalPorPeriodo(dataAbertura, agora, "Saída");
                    
                    pix = _repo.GetEntradasPorFormaPagamento(dataAbertura, agora, "Pix");
                    din = _repo.GetEntradasPorFormaPagamento(dataAbertura, agora, "Dinheiro");
                    cred = _repo.GetEntradasPorFormaPagamento(dataAbertura, agora, "Cartão de Crédito");
                    deb = _repo.GetEntradasPorFormaPagamento(dataAbertura, agora, "Cartão de Débito");

                    // Physical cash in drawer = Opening balance + Cash in - Cash out
                    decimal saidasDinheiro = _repo.GetSaidasPorFormaPagamento(dataAbertura, agora, "Dinheiro");
                    dinheiroEmCaixa = saldoInicial + din - saidasDinheiro;
                }

                var dynStats = new {
                    caixaAberto = isOpen,
                    caixaId = id,
                    saldoInicial = saldoInicial,
                    totalEntradas = entradas,
                    totalSaidas = saidas,
                    dinheiroEmCaixa = dinheiroEmCaixa,
                    entradasPix = pix,
                    entradasDinheiro = din,
                    entradasCredito = cred,
                    entradasDebito = deb,
                    isAdmin = MasterServicePro.DAL.AuthSession.IsAdmin
                };

                var msg = new { action = "load_stats", data = dynStats };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(msg));
            }
            catch { }
        }

        private void LoadExtrato(DateTime date)
        {
            try
            {
                DataTable dt = _repo.GetFluxoCaixa(date, date);
                var list = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    decimal valorVal = 0m;
                    if (row.Table.Columns.Contains("Valor") && row["Valor"] != DBNull.Value)
                    {
                        decimal.TryParse(row["Valor"].ToString(), out valorVal);
                    }

                    string dataStr = null;
                    if (row.Table.Columns.Contains("Data") && row["Data"] != DBNull.Value)
                    {
                        if (DateTime.TryParse(row["Data"].ToString(), out DateTime dtVal))
                            dataStr = dtVal.ToString("yyyy-MM-dd HH:mm:ss");
                    }

                    list.Add(new {
                        Tipo = row.Table.Columns.Contains("Tipo") && row["Tipo"] != DBNull.Value ? row["Tipo"].ToString() : "",
                        Categoria = row.Table.Columns.Contains("Categoria") && row["Categoria"] != DBNull.Value ? row["Categoria"].ToString() : "",
                        Valor = valorVal,
                        FormaPagamento = row.Table.Columns.Contains("FormaPagamento") && row["FormaPagamento"] != DBNull.Value ? row["FormaPagamento"].ToString() : "",
                        Descricao = row.Table.Columns.Contains("Descricao") && row["Descricao"] != DBNull.Value ? row["Descricao"].ToString() : "",
                        Data = dataStr
                    });
                }
                var msg = new { 
                    action = "load_extrato", 
                    data = list,
                    filterDate = date.ToString("yyyy-MM-dd"),
                    title = $"Extrato Financeiro ({date:dd/MM/yyyy})",
                    count = list.Count
                };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(msg));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar extrato: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void LoadCharts()
        {
            try
            {
                DateTime inicio = DateTime.Today.AddDays(-7);
                DateTime fim = DateTime.Today;

                var dadosGrafico = _repo.GetVendasUltimos7Dias();
                
                List<string> labels = new List<string>();
                List<decimal> data = new List<decimal>();
                foreach(var item in dadosGrafico)
                {
                    labels.Add(item.Key.ToString("dd/MM"));
                    data.Add(item.Value);
                }

                decimal pmDinheiro = _repo.GetEntradasPorFormaPagamento(inicio, fim, "Dinheiro");
                decimal pmPix = _repo.GetEntradasPorFormaPagamento(inicio, fim, "Pix");
                decimal pmCredito = _repo.GetEntradasPorFormaPagamento(inicio, fim, "Cartão de Crédito");
                decimal pmDebito = _repo.GetEntradasPorFormaPagamento(inicio, fim, "Cartão de Débito");

                var msg = new {
                    action = "load_charts",
                    data = new {
                        lineLabels = labels,
                        lineData = data,
                        doughnutData = new decimal[] { pmPix, pmDinheiro, pmCredito, pmDebito }
                    }
                };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(msg));
            }
            catch { }
        }

        private void BtnAddDinheiro_Click()
        {
            var caixa = _repo.ObterCaixaAberto();
            if (caixa == null) { MessageBox.Show("Você precisa ABRIR o caixa antes de fazer um lançamento.", "Caixa Fechado", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            this.BeginInvoke(new Action(() => {
                using (var frm = new FrmCaixaMovimentacaoWeb("Suprimento"))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        var mov = frm.Movimentacao;
                        mov.IdCaixa = caixa.Id;
                        _repo.LançarMovimentacao(mov);
                        RefreshAllData();
                    }
                }
            }));
        }

        private void BtnRetirarDinheiro_Click()
        {
            var caixa = _repo.ObterCaixaAberto();
            if (caixa == null) { MessageBox.Show("Você precisa ABRIR o caixa antes de fazer um lançamento.", "Caixa Fechado", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            this.BeginInvoke(new Action(() => {
                using (var frm = new FrmCaixaMovimentacaoWeb("Sangria"))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        var mov = frm.Movimentacao;
                        mov.IdCaixa = caixa.Id;
                        _repo.LançarMovimentacao(mov);
                        RefreshAllData();
                    }
                }
            }));
        }

        private void BtnAbrirCaixa_Click()
        {
            var caixaAtivo = _repo.ObterCaixaAberto();
            if (caixaAtivo != null) { MessageBox.Show("Já existe um caixa aberto no momento.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            this.BeginInvoke(new Action(() => {
                using (var frm = new FrmCaixaAberturaFechamento(true))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        _repo.AbrirCaixa(frm.Valor);
                        RefreshAllData();
                    }
                }
            }));
        }

        private void BtnFecharCaixa_Click()
        {
            if (!MasterServicePro.DAL.AuthSession.IsAdmin) { MessageBox.Show("Apenas administradores podem fechar o caixa.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var caixaAtivo = _repo.ObterCaixaAberto();
            if (caixaAtivo == null) { MessageBox.Show("Não há caixa aberto para fechar.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            this.BeginInvoke(new Action(() => {
                using (var frm = new FrmCaixaAberturaFechamento(false))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        _repo.FecharCaixa(caixaAtivo.Id, frm.Valor);
                        RefreshAllData();
                    }
                }
            }));
        }

        private void BtnHistorico_Click()
        {
            this.BeginInvoke(new Action(() => {
                using (var frm = new FrmCaixaHistorico())
                {
                    if (frm.ShowDialog() == DialogResult.OK && frm.Selecionou)
                    {
                        LoadHistoricCaixa(frm.DataInicioSelecionada, frm.DataFimSelecionada, frm.ValorAberturaSelecionado);
                    }
                }
            }));
        }

        private void LoadHistoricCaixa(DateTime inicio, DateTime fim, decimal saldoInicial)
        {
            try
            {
                decimal entradas = _repo.GetTotalPorPeriodo(inicio, fim, "Entrada");
                decimal saidas = _repo.GetTotalPorPeriodo(inicio, fim, "Saída");

                decimal pix = _repo.GetEntradasPorFormaPagamento(inicio, fim, "Pix");
                decimal din = _repo.GetEntradasPorFormaPagamento(inicio, fim, "Dinheiro");
                decimal cred = _repo.GetEntradasPorFormaPagamento(inicio, fim, "Cartão de Crédito");
                decimal deb = _repo.GetEntradasPorFormaPagamento(inicio, fim, "Cartão de Débito");

                // Physical cash in drawer = Opening balance + Cash in - Cash out
                decimal saidasDinheiro = _repo.GetSaidasPorFormaPagamento(inicio, fim, "Dinheiro");
                decimal dinheiroEmCaixa = saldoInicial + din - saidasDinheiro;

                var dynStats = new {
                    caixaAberto = false, 
                    caixaId = -1,
                    saldoInicial = saldoInicial,
                    totalEntradas = entradas,
                    totalSaidas = saidas,
                    dinheiroEmCaixa = dinheiroEmCaixa,
                    entradasPix = pix,
                    entradasDinheiro = din,
                    entradasCredito = cred,
                    entradasDebito = deb,
                    isAdmin = MasterServicePro.DAL.AuthSession.IsAdmin
                };

                var msgStats = new { action = "load_stats", data = dynStats };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(msgStats));

                DataTable dt = _repo.GetFluxoCaixa(inicio, fim);
                var list = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    decimal valorVal = 0m;
                    if (row.Table.Columns.Contains("Valor") && row["Valor"] != DBNull.Value)
                    {
                        decimal.TryParse(row["Valor"].ToString(), out valorVal);
                    }

                    string dataStr = null;
                    if (row.Table.Columns.Contains("Data") && row["Data"] != DBNull.Value)
                    {
                        if (DateTime.TryParse(row["Data"].ToString(), out DateTime dtVal))
                            dataStr = dtVal.ToString("yyyy-MM-dd HH:mm:ss");
                    }

                    list.Add(new {
                        Tipo = row.Table.Columns.Contains("Tipo") && row["Tipo"] != DBNull.Value ? row["Tipo"].ToString() : "",
                        Categoria = row.Table.Columns.Contains("Categoria") && row["Categoria"] != DBNull.Value ? row["Categoria"].ToString() : "",
                        Valor = valorVal,
                        FormaPagamento = row.Table.Columns.Contains("FormaPagamento") && row["FormaPagamento"] != DBNull.Value ? row["FormaPagamento"].ToString() : "",
                        Descricao = row.Table.Columns.Contains("Descricao") && row["Descricao"] != DBNull.Value ? row["Descricao"].ToString() : "",
                        Data = dataStr
                    });
                }

                // Send informative title and filterDate for historic session
                string sessionTitle = inicio.Date == fim.Date 
                    ? $"Extrato da Sessao ({inicio:dd/MM/yyyy})" 
                    : $"Extrato da Sessao ({inicio:dd/MM/yyyy} a {fim:dd/MM/yyyy})";

                var msgExtrato = new { 
                    action = "load_extrato", 
                    data = list,
                    filterDate = inicio.ToString("yyyy-MM-dd"),
                    title = sessionTitle,
                    count = list.Count
                };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(msgExtrato));
            }
            catch { }
        }
    }
}
