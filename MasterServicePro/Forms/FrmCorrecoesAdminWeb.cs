using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using MasterServicePro.DAL;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public class FrmCorrecoesAdminWeb : Form
    {
        private WebView2 webView;
        private Panel pnlTitleBar;
        private Label lblTitle;
        private Button btnClose;
        
        private VendaRepository vendaRepo = new VendaRepository();
        private OrdemServicoRepository osRepo = new OrdemServicoRepository();

        public FrmCorrecoesAdminWeb()
        {
            this.Size = new Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = UITheme.BgApp;
            
            // Title Bar
            pnlTitleBar = new Panel
            {
                Height = 40,
                Dock = DockStyle.Top,
                BackColor = UITheme.BgCard
            };
            
            lblTitle = new Label
            {
                Text = "Correções Admin",
                ForeColor = UITheme.TextTitle,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 10)
            };
            
            btnClose = new Button
            {
                Text = "✕",
                Width = 40,
                Height = 40,
                Dock = DockStyle.Right,
                FlatStyle = FlatStyle.Flat,
                ForeColor = UITheme.TextMuted,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();
            btnClose.MouseEnter += (s, e) => { btnClose.BackColor = Color.FromArgb(239, 68, 68); btnClose.ForeColor = Color.White; };
            btnClose.MouseLeave += (s, e) => { btnClose.BackColor = UITheme.BgCard; btnClose.ForeColor = UITheme.TextMuted; };

            pnlTitleBar.Controls.Add(lblTitle);
            pnlTitleBar.Controls.Add(btnClose);
            
            // WebView
            webView = new WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();

            webView.CreationProperties = null;
            webView.DefaultBackgroundColor = UITheme.BgApp;
            webView.Dock = DockStyle.Fill;
            this.Controls.Add(webView);
            this.Controls.Add(pnlTitleBar);
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);
            
            // Border
            this.Paint += (s, e) =>
            {
                e.Graphics.DrawRectangle(new Pen(UITheme.ElementBorder, 1), 0, 0, this.Width - 1, this.Height - 1);
            };

            // Drag
            bool dragging = false;
            Point dragCursorPoint = Point.Empty;
            Point dragFormPoint = Point.Empty;
            pnlTitleBar.MouseDown += (s, e) => { dragging = true; dragCursorPoint = Cursor.Position; dragFormPoint = this.Location; };
            pnlTitleBar.MouseMove += (s, e) => 
            {
                if (dragging)
                {
                    Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                    this.Location = Point.Add(dragFormPoint, new Size(dif));
                }
            };
            pnlTitleBar.MouseUp += (s, e) => dragging = false;

            this.Load += FrmCorrecoesAdminWeb_Load;
        }

        private void FrmCorrecoesAdminWeb_Load(object sender, EventArgs e)
        {
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"CorrecoesAdmin\index.html");
                if (!File.Exists(path))
                {
                    MessageBox.Show("Erro: Arquivo HTML não encontrado em " + path, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                webView.CoreWebView2.Navigate(path);
                webView.CoreWebView2.WebMessageReceived += WebView_WebMessageReceived;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar WebView: " + ex.Message);
            }
        }

        private void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string msg = e.TryGetWebMessageAsString();
                dynamic data = JsonConvert.DeserializeObject(msg);
                string action = data.action;

                if (action == "READY")
                {
                    LoadData();
                }
                else if (action == "PROMPT_CORRECAO")
                {
                    string type = data.type;
                    int id = data.id;
                    decimal total = Convert.ToDecimal(data.total);

                    this.BeginInvoke(new Action(() => {
                        using (var frm = new FrmPromptPagamentoOSWeb(total))
                        {
                            if (frm.ShowDialog() == DialogResult.OK)
                            {
                                try
                                {
                                    if (type == "venda")
                                    {
                                        vendaRepo.AtualizarFormaPagamentoVendaMista(id, frm.FormaPagamento, AuthSession.Id, frm.Troco, frm.MistoDinheiro, frm.MistoPix, frm.MistoCartao, frm.MistoSaldo);
                                        FrmNotification.ShowSuccess("Forma de pagamento da Venda atualizada!");
                                    }
                                    else if (type == "os")
                                    {
                                        osRepo.AtualizarFormaPagamentoOSMista(id, frm.FormaPagamento, AuthSession.Id, frm.Troco, frm.MistoDinheiro, frm.MistoPix, frm.MistoCartao, frm.MistoSaldo);
                                        FrmNotification.ShowSuccess("Forma de pagamento da OS atualizada!");
                                    }
                                    
                                    LoadData(); // Reload tables
                                }
                                catch (Exception ex)
                                {
                                    FrmNotification.ShowError("Erro: " + ex.Message);
                                }
                            }
                        }
                    }));
                }
                else if (action == "CANCELAR_VENDA")
                {
                    int id = data.id;
                    this.BeginInvoke(new Action(() => {
                        var finRepo = new FinanceiroRepository();
                        var caixa = finRepo.ObterCaixaAberto();
                        if (caixa == null)
                        {
                            MessageBox.Show("Você precisa ter um Caixa Aberto para poder realizar o estorno de uma venda (o dinheiro sairá do caixa atual).", "Caixa Fechado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        var dtVenda = vendaRepo.BuscarPorId(id);
                        bool hasCliente = dtVenda != null && dtVenda.Rows.Count > 0 && dtVenda.Rows[0]["ClienteId"] != DBNull.Value && Convert.ToInt32(dtVenda.Rows[0]["ClienteId"]) > 0;

                        using (var frm = new FrmPromptMotivoEstorno(showDestino: hasCliente))
                        {
                            if (frm.ShowDialog() == DialogResult.OK)
                            {
                                try
                                {
                                    vendaRepo.EstornarVenda(id, frm.Motivo, caixa.Id, frm.DestinoEstorno, frm.FormaPagamentoDevolucao);
                                    FrmNotification.ShowSuccess("Venda cancelada com sucesso!");
                                    LoadData();
                                }
                                catch (Exception ex)
                                {
                                    FrmNotification.ShowError("Erro ao cancelar venda: " + ex.Message);
                                }
                            }
                        }
                    }));
                }
                else if (action == "EDITAR_CLIENTE")
                {
                    string type = data.type;
                    int id = data.id;

                    this.BeginInvoke(new Action(() => {
                        using (var frm = new FrmPDVBuscaCliente())
                        {
                            if (frm.ShowDialog() == DialogResult.OK && frm.ClienteSelecionado != null)
                            {
                                try
                                {
                                    if (type == "venda")
                                    {
                                        vendaRepo.AtualizarCliente(id, frm.ClienteSelecionado.Id, frm.ClienteSelecionado.Nome, AuthSession.Id);
                                        FrmNotification.ShowSuccess("Cliente da Venda atualizado!");
                                    }
                                    else if (type == "os")
                                    {
                                        osRepo.AtualizarCliente(id, frm.ClienteSelecionado.Id, frm.ClienteSelecionado.Nome, AuthSession.Id);
                                        FrmNotification.ShowSuccess("Cliente da OS atualizado!");
                                    }
                                    
                                    LoadData(); // Reload tables
                                }
                                catch (Exception ex)
                                {
                                    FrmNotification.ShowError("Erro ao alterar cliente: " + ex.Message);
                                }
                            }
                        }
                    }));
                }
                else if (action == "TROCAR_VENDA")
                {
                    int id = data.id;
                    this.BeginInvoke(new Action(() => {
                        using (var frm = new FrmTrocaVenda(id))
                        {
                            if (frm.ShowDialog() == DialogResult.OK)
                            {
                                LoadData();
                            }
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro na comunicação web: " + ex.Message);
            }
        }

        private void LoadData()
        {
            // VENDAS
            try {
                var dtVendas = vendaRepo.GetVendasRelatorio(DateTime.Today.AddDays(-30), DateTime.Today);
                string jsonVendas = JsonConvert.SerializeObject(dtVendas);
                string safeJsonVendas = JsonConvert.SerializeObject(jsonVendas); // Escape pra JS
                webView.CoreWebView2.ExecuteScriptAsync($"loadVendas({safeJsonVendas})");
            } catch { }

            // OS
            try {
                var dtOS = osRepo.BuscarTudoResumido();
                var viewOS = new DataView(dtOS);
                viewOS.RowFilter = "Status = 'Finalizado' OR Status = 'Entregue'";
                var dtFiltrado = viewOS.ToTable();
                string jsonOS = JsonConvert.SerializeObject(dtFiltrado);
                string safeJsonOS = JsonConvert.SerializeObject(jsonOS);
                webView.CoreWebView2.ExecuteScriptAsync($"loadOS({safeJsonOS})");
            } catch { }
        }
    }
}
