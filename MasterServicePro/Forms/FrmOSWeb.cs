using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.IO;
using Newtonsoft.Json;
using MasterServicePro.DAL;
using MasterServicePro.Models;
using System.Collections.Generic;

namespace MasterServicePro.Forms
{
    public class FrmOSWeb : Form
    {
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private Panel pnlTitleBar;
        private Label lblTitle;
        private Button btnClose;
        private PictureBox picLogo;
        private OrdemServicoRepository repository = new OrdemServicoRepository();

        public FrmOSWeb()
        {
            InitializeComponent();
            this.Load += FrmOSWeb_Load;
        }

        private void FrmOSWeb_Load(object sender, EventArgs e)
        {
            InitializeAsync();
        }

        private void InitializeComponent()
        {
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.pnlTitleBar = new Panel();
            this.lblTitle = new Label();
            this.btnClose = new Button();
            this.picLogo = new PictureBox();
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            
            // Title Bar
            this.pnlTitleBar.BackColor = System.Drawing.Color.FromArgb(26, 29, 36);
            this.pnlTitleBar.Dock = DockStyle.Top;
            this.pnlTitleBar.Height = 40;
            this.pnlTitleBar.Controls.Add(this.lblTitle);
            this.pnlTitleBar.Controls.Add(this.btnClose);
            this.pnlTitleBar.Controls.Add(this.picLogo);
            this.pnlTitleBar.MouseDown += TitleBar_MouseDown;
            
            // Close Button
            this.btnClose.Dock = DockStyle.Right;
            this.btnClose.Width = 45;
            this.btnClose.FlatStyle = FlatStyle.Flat;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Text = "X";
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.Cursor = Cursors.Hand;
            this.btnClose.Click += (s, e) => this.Close();
            this.btnClose.MouseEnter += (s, e) => this.btnClose.BackColor = System.Drawing.Color.FromArgb(232, 17, 35);
            this.btnClose.MouseLeave += (s, e) => this.btnClose.BackColor = System.Drawing.Color.Transparent;

            // Logo
            this.picLogo.Dock = DockStyle.Left;
            this.picLogo.Width = 40;
            this.picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            this.picLogo.Padding = new Padding(8);
            try 
            { 
                var appIcon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                this.Icon = appIcon;
                this.picLogo.Image = appIcon.ToBitmap(); 
            } 
            catch { }
            this.picLogo.MouseDown += TitleBar_MouseDown;

            // Title Label
            this.lblTitle.Dock = DockStyle.Fill;
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTitle.Text = "Ordens de Serviço (Web)";
            this.lblTitle.MouseDown += TitleBar_MouseDown;

            // WebView
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.FromArgb(26, 29, 36);
            this.webView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView.Location = new System.Drawing.Point(0, 40);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(1200, 710);
            this.webView.TabIndex = 0;
            this.webView.ZoomFactor = 1D;
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(26, 29, 36);
            this.ClientSize = new System.Drawing.Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            
            // Adding a 1px border effect by padding the form
            this.Padding = new Padding(1);
            
            this.Controls.Add(this.webView);
            this.Controls.Add(this.pnlTitleBar);
            this.Name = "FrmOSWeb";
            this.Text = "Ordens de Serviço (Web)";
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);
        }

        // Win32 API para arrastar a janela
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"OS\index.html");
                path = Path.GetFullPath(path);
                
                if (!File.Exists(path))
                {
                    MessageBox.Show("Erro: Arquivo HTML não encontrado em " + path, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                webView.CoreWebView2.Navigate(path);
                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                webView.CoreWebView2.NavigationCompleted += (s, ev) => {
                    try {
                        bool isAdmin = MasterServicePro.DAL.AuthSession.IsAdmin;
                        webView.CoreWebView2.ExecuteScriptAsync($"window.isAdmin = {isAdmin.ToString().ToLower()}; if(typeof initDates === 'function') initDates();");
                    } catch {}
                };
            }
            catch (System.Runtime.InteropServices.COMException ex) when ((uint)ex.ErrorCode == 0x80004004 || (uint)ex.ErrorCode == 0x80040154)
            {
                // E_ABORT: Formulário fechado antes de inicializar o WebView2. Ignorar.
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
                    case "LOAD_DATA":
                        string filter = message.filter != null ? message.filter.ToString() : "";
                        string query = message.query != null ? message.query.ToString() : "";
                        string dtInicio = message.dtInicio != null ? message.dtInicio.ToString() : "";
                        string dtFim = message.dtFim != null ? message.dtFim.ToString() : "";
                        LoadData(filter, query, dtInicio, dtFim);
                        break;
                        
                    case "NOVA_OS":
                        this.BeginInvoke(new Action(() => 
                        {
                            using (var form = new FrmOSAddEditWeb())
                            {
                                if (form.ShowDialog() == DialogResult.OK) LoadData("Todas", "");
                            }
                        }));
                        break;
                        
                    case "EDITAR_OS":
                        int editId = (int)message.id;
                        this.BeginInvoke(new Action(() => 
                        {
                            using (var form = new FrmOSAddEditWeb(editId))
                            {
                                if (form.ShowDialog() == DialogResult.OK) LoadData("Todas", "");
                            }
                        }));
                        break;
                        
                    case "IMPRIMIR_OS":
                        int printId = (int)message.id;
                        new MasterServicePro.Utils.OSPrinter().Imprimir(printId);
                        break;

                    case "IMPRIMIR_RECIBO":
                        int reciboId = (int)message.id;
                        new MasterServicePro.Utils.OSPrinter().ImprimirReciboEntrega(reciboId);
                        break;

                    case "IMPRIMIR_ETIQUETA":
                        int etiquetaId = (int)message.id;
                        new MasterServicePro.Utils.OSPrinter().ImprimirEtiquetaAparelho(etiquetaId);
                        break;

                    case "EXCLUIR_OS":
                        int excluirId = (int)message.id;
                        if (MessageBox.Show("Tem certeza que deseja cancelar/excluir esta OS?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            repository.Excluir(excluirId);
                            LoadData("Todas", "");
                        }
                        break;

                    case "MOVER_ABERTO":
                        MoverStatus((int)message.id, "Aberto");
                        break;
                    case "MOVER_ANDAMENTO":
                        MoverStatus((int)message.id, "Em Andamento");
                        break;
                    case "MOVER_FINALIZADO":
                        MoverStatus((int)message.id, "Finalizado");
                        break;
                    case "MOVER_CANCELADO":
                        MoverStatus((int)message.id, "Cancelado");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao processar mensagem do JS: " + ex.Message);
            }
        }

        private async void LoadData(string filtroStatus, string busca, string dtInicio = "", string dtFim = "")
        {
            DataTable dt = repository.BuscarTudoResumido();

            string rowFilter = "";
            if (filtroStatus == "Pendente") rowFilter = "Status = 'Pendente' OR Status = 'Aberto' OR Status = 'Em Andamento'";
            else if (filtroStatus == "Finalizado") rowFilter = "Status = 'Finalizado' OR Status = 'Entregue'";
            else if (filtroStatus == "Cancelado") rowFilter = "Status = 'Cancelado' OR Status = 'Cancelada'";

            if (!string.IsNullOrWhiteSpace(busca))
            {
                string b = busca.Replace("'", "''");
                string bFilter = $"(Cliente LIKE '%{b}%' OR Marca LIKE '%{b}%' OR Modelo LIKE '%{b}%' OR Defeito LIKE '%{b}%')";
                rowFilter = string.IsNullOrEmpty(rowFilter) ? bFilter : $"{rowFilter} AND {bFilter}";
            }

            if (!string.IsNullOrEmpty(dtInicio) && DateTime.TryParse(dtInicio, out DateTime dIni))
            {
                string dateFilter = $"DataAbertura >= #{dIni:MM/dd/yyyy}#";
                rowFilter = string.IsNullOrEmpty(rowFilter) ? dateFilter : $"{rowFilter} AND {dateFilter}";
            }

            if (!string.IsNullOrEmpty(dtFim) && DateTime.TryParse(dtFim, out DateTime dFim))
            {
                dFim = dFim.AddDays(1).AddSeconds(-1);
                string dateFilter = $"DataAbertura <= #{dFim:MM/dd/yyyy HH:mm:ss}#";
                rowFilter = string.IsNullOrEmpty(rowFilter) ? dateFilter : $"{rowFilter} AND {dateFilter}";
            }

            DataView dv = dt.DefaultView;
            if (!string.IsNullOrEmpty(rowFilter)) dv.RowFilter = rowFilter;
            
            var list = new List<object>();
            foreach (DataRowView rowView in dv)
            {
                DataRow row = rowView.Row;
                list.Add(new {
                    Id = row["Id"],
                    Cliente = row["Cliente"],
                    Marca = row["Marca"],
                    Modelo = row["Modelo"],
                    Defeito = row["Defeito"],
                    ValorTotal = row["ValorTotal"],
                    Lucro = row["Lucro"],
                    Status = row["Status"],
                    DataAbertura = row["DataAbertura"],
                    DataAtualizacao = row["DataAtualizacao"]
                });
            }

            string jsonResult = JsonConvert.SerializeObject(list);
            string safeJsString = JsonConvert.SerializeObject(jsonResult);
            await webView.CoreWebView2.ExecuteScriptAsync($"renderData({safeJsString})");
        }

        private void MoverStatus(int idOS, string novoStatus)
        {
            var os = repository.BuscarPorId(idOS);
            if (os != null)
            {
                os.Status = novoStatus;

                if (novoStatus.Equals("Finalizado", StringComparison.OrdinalIgnoreCase) && !os.Faturado)
                {
                    using (var frm = new FrmPromptPagamentoOSWeb(os.ValorTotal)) // Pass total value
                    {
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            os.Faturado = true;
                            
                            if (frm.JaRecebido)
                            {
                                var finRepo = new MasterServicePro.DAL.FinanceiroRepository();
                                var caixa = finRepo.ObterCaixaAberto();
                                int idCaixa = caixa != null ? caixa.Id : 0;
                                
                                if (frm.FormaPagamento.ToLower() == "misto")
                                {
                                    decimal dinheiroNet = frm.MistoDinheiro;
                                    if (frm.Troco > 0)
                                    {
                                        dinheiroNet -= frm.Troco;
                                    }

                                    if (dinheiroNet > 0)
                                        finRepo.LançarMovimentacao(new MasterServicePro.Models.Movimentacao { Tipo = "Entrada", Categoria = "Serviços", Subcategoria = "Ordem de Serviço", Valor = dinheiroNet, FormaPagamento = "Dinheiro", Descricao = $"Recebimento OS #{os.NumeroOS} (Misto)", IdCaixa = idCaixa });
                                    if (frm.MistoPix > 0)
                                        finRepo.LançarMovimentacao(new MasterServicePro.Models.Movimentacao { Tipo = "Entrada", Categoria = "Serviços", Subcategoria = "Ordem de Serviço", Valor = frm.MistoPix, FormaPagamento = "Pix", Descricao = $"Recebimento OS #{os.NumeroOS} (Misto)", IdCaixa = idCaixa });
                                    if (frm.MistoCartao > 0)
                                        finRepo.LançarMovimentacao(new MasterServicePro.Models.Movimentacao { Tipo = "Entrada", Categoria = "Serviços", Subcategoria = "Ordem de Serviço", Valor = frm.MistoCartao, FormaPagamento = "Cartão", Descricao = $"Recebimento OS #{os.NumeroOS} (Misto)", IdCaixa = idCaixa });
                                    if (frm.MistoSaldo > 0)
                                    {
                                        var repoCli = new MasterServicePro.DAL.ClienteRepository();
                                        repoCli.DescontarSaldo(os.IdCliente, frm.MistoSaldo);
                                    }
                                }
                                else
                                {
                                    finRepo.LançarMovimentacao(new MasterServicePro.Models.Movimentacao
                                    {
                                        Tipo = "Entrada",
                                        Categoria = "Serviços",
                                        Subcategoria = "Ordem de Serviço",
                                        Valor = os.ValorTotal,
                                        FormaPagamento = frm.FormaPagamento,
                                        Descricao = $"Recebimento OS #{os.NumeroOS}",
                                        IdCaixa = idCaixa
                                    });
                                }
                            }
                            else
                            {
                                var crRepo = new MasterServicePro.DAL.ContasReceberRepository();
                                crRepo.Inserir(new MasterServicePro.Models.ContaReceber
                                {
                                    IdCliente = os.IdCliente,
                                    Descricao = $"Serviço OS #{os.NumeroOS}",
                                    ValorTotal = os.ValorTotal,
                                    ValorPago = 0,
                                    DataLancamento = DateTime.Now,
                                    DataVencimento = DateTime.Now.AddDays(30),
                                    Status = "Pendente"
                                });
                            }
                        }
                    }
                }

                repository.Atualizar(os);
                LoadData("Todas", "");
                
                System.Threading.Tasks.Task.Run(async () => {
                    var ws = new MasterServicePro.Services.WhatsAppService();
                    string tipo = os.Status.Equals("Finalizado", StringComparison.OrdinalIgnoreCase) ? "Finalizado" : "Atualizacao";
                    await ws.EnviarNotificacaoOSAsync(os, tipo);
                });
            }
        }
    }
}
