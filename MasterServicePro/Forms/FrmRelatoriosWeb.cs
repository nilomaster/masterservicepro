using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.IO;
using Newtonsoft.Json;
using MasterServicePro.DAL;

namespace MasterServicePro.Forms
{
    public class FrmRelatoriosWeb : Form
    {
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        
        private readonly VendaRepository _vendaRepo = new VendaRepository();
        private readonly OrdemServicoRepository _osRepo = new OrdemServicoRepository();
        private readonly FinanceiroRepository _financeiroRepo = new FinanceiroRepository();
        private readonly AuditoriaRepository _auditoriaRepo = new AuditoriaRepository();

        private DateTime dataInicio = DateTime.Today.AddDays(-30);
        private DateTime dataFim = DateTime.Today;

        public FrmRelatoriosWeb()
        {
            InitializeComponent();
            this.Load += FrmRelatoriosWeb_Load;
        }

        private void FrmRelatoriosWeb_Load(object sender, EventArgs e)
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
            this.Name = "FrmRelatoriosWeb";
            this.Text = "Relatórios (Web)";
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"Relatorios\index.html");
                path = Path.GetFullPath(path);
                
                if (!File.Exists(path))
                {
                    MessageBox.Show("Erro: Arquivo HTML não encontrado em " + path, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                webView.CoreWebView2.Navigate(path);
                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                webView.NavigationCompleted += (s, e) => { 
                    try {
                        var m = new { action = "init_relatorios", isAdmin = MasterServicePro.DAL.AuthSession.IsAdmin };
                        webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(m));
                    } catch { }
                };
            }
            catch (System.Runtime.InteropServices.COMException ex) when ((uint)ex.ErrorCode == 0x80004004 || (uint)ex.ErrorCode == 0x80040154)
            {
                // Ignorar erros de form fechado ou WebView2 destruído prematuramente
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

                if (action == "filtrar")
                {
                    if (DateTime.TryParse(msg.inicio.ToString(), out DateTime ini)) dataInicio = ini;
                    if (DateTime.TryParse(msg.fim.ToString(), out DateTime fim)) dataFim = fim;
                    
                    LoadAllData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao processar mensagem web: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAllData()
        {

            LoadVendas();
            LoadCategorias();
            LoadCustos();
            LoadFinanceiro();
            LoadAuditoria();
        }

        private List<Dictionary<string, object>> DataTableToList(DataTable dt)
        {
            var list = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                }
                list.Add(dict);
            }
            return list;
        }

        private void LoadVendas()
        {
            try
            {
                DataTable dt = _vendaRepo.GetVendasRelatorio(dataInicio, dataFim);
                decimal totalVendas = 0;
                foreach (DataRow row in dt.Rows)
                {
                    totalVendas += row["TotalFinal"] != DBNull.Value ? Convert.ToDecimal(row["TotalFinal"]) : 0;
                }
                int qtdVendas = dt.Rows.Count;
                decimal ticket = qtdVendas > 0 ? (totalVendas / qtdVendas) : 0;

                var msg = new {
                    action = "load_vendas",
                    data = DataTableToList(dt),
                    stats = new { total = totalVendas, qtd = qtdVendas, ticket = ticket }
                };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(msg));
            }
            catch { }
        }

        private void LoadCategorias()
        {
            try
            {
                DataTable dt = _vendaRepo.GetFaturamentoDetalhadoPorCategoria(dataInicio, dataFim);
                decimal totalCusto = 0;
                decimal totalFaturamento = 0;
                decimal totalLucro = 0;

                foreach (DataRow row in dt.Rows)
                {
                    totalCusto += row["Custo"] != DBNull.Value ? Convert.ToDecimal(row["Custo"]) : 0;
                    totalFaturamento += row["Faturamento"] != DBNull.Value ? Convert.ToDecimal(row["Faturamento"]) : 0;
                    totalLucro += row["Lucro"] != DBNull.Value ? Convert.ToDecimal(row["Lucro"]) : 0;
                }

                var msg = new {
                    action = "load_categorias",
                    data = DataTableToList(dt),
                    stats = new { totalCusto = totalCusto, totalFaturamento = totalFaturamento, totalLucro = totalLucro }
                };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(msg));
            }
            catch { }
        }

        private void LoadCustos()
        {
            try
            {
                DataTable dt = _osRepo.GetCustosLucrosRelatorio(dataInicio, dataFim, null);
                decimal totalCustoPecas = 0;
                decimal totalFat = 0;
                decimal totalLucro = 0;

                foreach (DataRow row in dt.Rows)
                {
                    totalCustoPecas += row["CustoPecas"] != DBNull.Value ? Convert.ToDecimal(row["CustoPecas"]) : 0;
                    totalFat += row["MaoObra"] != DBNull.Value ? Convert.ToDecimal(row["MaoObra"]) : 0;
                    totalLucro += row["LucroLiquido"] != DBNull.Value ? Convert.ToDecimal(row["LucroLiquido"]) : 0;
                }

                var msg = new {
                    action = "load_custos",
                    data = DataTableToList(dt),
                    stats = new { pecas = totalCustoPecas, maoObra = totalFat, lucro = totalLucro }
                };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(msg));
            }
            catch { }
        }

        private void LoadFinanceiro()
        {
            try
            {
                DataTable dt = _financeiroRepo.GetFluxoCaixa(dataInicio, dataFim);
                decimal entradas = 0;
                decimal saidas = 0;

                foreach (DataRow row in dt.Rows)
                {
                    decimal valor = row["Valor"] != DBNull.Value ? Convert.ToDecimal(row["Valor"]) : 0;
                    string tipo = row["Tipo"]?.ToString();

                    if (tipo == "Entrada") entradas += valor;
                    else if (tipo == "Saida") saidas += valor;
                }

                var msg = new {
                    action = "load_financeiro",
                    data = DataTableToList(dt),
                    stats = new { entradas = entradas, saidas = saidas, saldo = (entradas - saidas) }
                };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(msg));
            }
            catch { }
        }

        private void LoadAuditoria()
        {
            try
            {
                DataTable dt = _auditoriaRepo.Listar(dataInicio, dataFim, "");
                var msg = new {
                    action = "load_auditoria",
                    data = DataTableToList(dt),
                    stats = new { total = dt.Rows.Count }
                };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(msg));
            }
            catch { }
        }
    }
}
