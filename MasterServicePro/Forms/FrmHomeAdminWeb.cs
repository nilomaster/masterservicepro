using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.IO;
using Newtonsoft.Json;
using MasterServicePro.DAL;
using MasterServicePro.Utils;

using System.Collections.Generic;

namespace MasterServicePro.Forms
{
    public class FrmHomeAdminWeb : Form
    {
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private VendaRepository _vendaRepo = new VendaRepository();
        private OrdemServicoRepository _osRepo = new OrdemServicoRepository();

        public FrmHomeAdminWeb()
        {
            InitializeComponent();
            this.Load += FrmHomeAdminWeb_Load;
        }

        private void FrmHomeAdminWeb_Load(object sender, EventArgs e)
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
            this.webView.ZoomFactor = 1D;
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(26, 29, 36);
            this.ClientSize = new System.Drawing.Size(1200, 750);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Controls.Add(this.webView);
            this.Name = "FrmHomeAdminWeb";
            this.Text = "Dashboard Admin";
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"HomeAdmin\index.html");
                path = Path.GetFullPath(path);
                
                if (!File.Exists(path))
                {
                    MessageBox.Show("Erro: Arquivo HTML não encontrado em " + path, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                webView.CoreWebView2.Navigate(path);
                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                webView.NavigationCompleted += (s, e) => { LoadMetrics(); };
            }
            catch (Exception)
            {
                // Ignorar erros de COM Exception se o form fechar antes de carregar
            }
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var json = e.WebMessageAsJson;
                dynamic msg = JsonConvert.DeserializeObject(json);
                string action = msg.action;

                if (action == "refresh_data")
                {
                    LoadMetrics();
                }
                else if (action == "abrir_correcoes")
                {
                    this.BeginInvoke((Action)(() => {
                        using (var frm = new FrmCorrecoesAdminWeb())
                        {
                            frm.ShowDialog();
                        }
                    }));
                }
            }
            catch { }
        }

        private void LoadMetrics()
        {
            try
            {
                DateTime hoje = DateTime.Now.Date;
                
                int diaSemana = (int)hoje.DayOfWeek;
                if (diaSemana == 0) diaSemana = 7;
                DateTime inicioSemana = hoje.AddDays(-(diaSemana - 1));
                
                DateTime inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
                
                decimal vendasDia = CalcularTotal(hoje, hoje);
                
                DateTime ontem = hoje.AddDays(-1);
                decimal vendasOntem = CalcularTotal(ontem, ontem);
                decimal percDiaria = vendasOntem > 0 ? ((vendasDia - vendasOntem) / vendasOntem) * 100 : (vendasDia > 0 ? 100 : 0);

                decimal vendasSemana = CalcularTotal(inicioSemana, hoje);
                
                DateTime inicioSemanaPassada = inicioSemana.AddDays(-7);
                DateTime fimSemanaPassada = inicioSemana.AddDays(-1); // Ultimo dia da semana passada
                decimal vendasSemanaPassada = CalcularTotal(inicioSemanaPassada, fimSemanaPassada);
                decimal percSemanal = vendasSemanaPassada > 0 ? ((vendasSemana - vendasSemanaPassada) / vendasSemanaPassada) * 100 : (vendasSemana > 0 ? 100 : 0);

                decimal vendasMes = CalcularTotal(inicioMes, hoje);
                
                DateTime inicioMesPassado = inicioMes.AddMonths(-1);
                DateTime fimMesPassado = inicioMes.AddDays(-1); // Ultimo dia do mes passado
                decimal vendasMesPassado = CalcularTotal(inicioMesPassado, fimMesPassado);
                decimal percMensal = vendasMesPassado > 0 ? ((vendasMes - vendasMesPassado) / vendasMesPassado) * 100 : (vendasMes > 0 ? 100 : 0);

                var chartLabels = new List<string>();
                var chartData = new List<decimal>();
                
                for(int i = 6; i >= 0; i--)
                {
                    DateTime dia = hoje.AddDays(-i);
                    chartLabels.Add(dia.ToString("dd/MM"));
                    chartData.Add(CalcularTotal(dia, dia));
                }
                
                DataTable dtVendasM = _vendaRepo.GetVendasRelatorio(inicioMes, hoje);
                DataTable dtOsM = _osRepo.GetCustosLucrosRelatorio(inicioMes, hoje, null);
                
                decimal totVendasDir = 0;
                foreach(DataRow r in dtVendasM.Rows)
                    totVendasDir += r["TotalFinal"] != DBNull.Value ? Convert.ToDecimal(r["TotalFinal"]) : 0;
                    
                decimal totOsDir = 0;
                foreach(DataRow r in dtOsM.Rows)
                    totOsDir += r["LucroLiquido"] != DBNull.Value ? Convert.ToDecimal(r["LucroLiquido"]) : 0; 

                int estoqueBaixo = 0;
                int osAtraso = 0;
                int contasHoje = 0;
                
                try
                {
                    DbConnection db = new DbConnection();
                    object r1 = db.ExecuteScalar("SELECT COUNT(Id) FROM Produtos WHERE Estoque <= EstoqueMinimo AND Ativo = 1");
                    object r2 = db.ExecuteScalar("SELECT COUNT(Id) FROM OrdensServico WHERE Status IN ('Aberta', 'Em Andamento')");
                    object r3 = db.ExecuteScalar("SELECT COUNT(Id) FROM ContasReceber WHERE CONVERT(date, DataVencimento) <= CONVERT(date, GETDATE()) AND Status = 'Pendente'");
                    
                    if (r1 != null && r1 != DBNull.Value) estoqueBaixo = Convert.ToInt32(r1);
                    if (r2 != null && r2 != DBNull.Value) osAtraso = Convert.ToInt32(r2);
                    if (r3 != null && r3 != DBNull.Value) contasHoje = Convert.ToInt32(r3);
                }
                catch { }

                var msg = new {
                    action = "load_metrics",
                    diaria = vendasDia,
                    semanal = vendasSemana,
                    mensal = vendasMes,
                    percDiaria = percDiaria,
                    percSemanal = percSemanal,
                    percMensal = percMensal,
                    barLabels = chartLabels,
                    barData = chartData,
                    pieVendas = totVendasDir,
                    pieOs = totOsDir,
                    estoqueBaixo = estoqueBaixo,
                    osAtraso = osAtraso,
                    contasHoje = contasHoje
                };
                
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(msg));
            }
            catch { }
        }

        private decimal CalcularTotal(DateTime inicio, DateTime fim)
        {
            decimal total = 0;
            DataTable dtVendas = _vendaRepo.GetVendasRelatorio(inicio, fim);
            foreach(DataRow row in dtVendas.Rows)
            {
                total += row["TotalFinal"] != DBNull.Value ? Convert.ToDecimal(row["TotalFinal"]) : 0;
            }
            
            DataTable dtOS = _osRepo.GetCustosLucrosRelatorio(inicio, fim, null);
            foreach(DataRow row in dtOS.Rows)
            {
                total += row["LucroLiquido"] != DBNull.Value ? Convert.ToDecimal(row["LucroLiquido"]) : 0;
            }
            
            return total;
        }
    }
}
