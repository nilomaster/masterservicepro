using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.IO;
using Newtonsoft.Json;
using MasterServicePro.Models;

namespace MasterServicePro.Forms
{
    public class FrmCaixaMovimentacaoWeb : Form
    {
        public Movimentacao Movimentacao { get; private set; }
        private string tipoMovimentacao;
        
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private Panel pnlTitleBar;
        private Label lblTitle;
        private Button btnClose;
        private PictureBox picLogo;

        public FrmCaixaMovimentacaoWeb(string tipo)
        {
            this.tipoMovimentacao = tipo;
            InitializeComponent();
            this.Load += FrmCaixaMovimentacaoWeb_Load;
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
            this.btnClose.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.btnClose.MouseEnter += (s, e) => this.btnClose.BackColor = System.Drawing.Color.FromArgb(232, 17, 35);
            this.btnClose.MouseLeave += (s, e) => this.btnClose.BackColor = System.Drawing.Color.Transparent;

            // Logo (Hidden for cleaner SaaS look)
            this.picLogo.Dock = DockStyle.Left;
            this.picLogo.Width = 40;
            this.picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            this.picLogo.Padding = new Padding(8);
            this.picLogo.Visible = false;
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
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            string friendlyName = tipoMovimentacao == "Suprimento" ? "Adicionar Dinheiro" : "Retirar Dinheiro";
            this.lblTitle.Text = friendlyName;
            this.lblTitle.MouseDown += TitleBar_MouseDown;

            // WebView
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.FromArgb(26, 29, 36);
            this.webView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView.Location = new System.Drawing.Point(0, 40);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(500, 560);
            this.webView.TabIndex = 0;
            this.webView.ZoomFactor = 1D;
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(26, 29, 36);
            this.ClientSize = new System.Drawing.Size(500, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(1);
            
            this.Controls.Add(this.webView);
            this.Controls.Add(this.pnlTitleBar);
            this.Name = "FrmCaixaMovimentacaoWeb";
            this.Text = "Registrar Movimentação";
            
            // Border
            this.Paint += (sender, e) =>
            {
                using (Pen pen = new Pen(System.Drawing.Color.FromArgb(51, 65, 85), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
                }
            };
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);
        }

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

        private void FrmCaixaMovimentacaoWeb_Load(object sender, EventArgs e)
        {
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"CaixaMovimentacao\index.html");
                path = Path.GetFullPath(path);
                
                if (!File.Exists(path))
                {
                    MessageBox.Show("Erro: Arquivo HTML não encontrado em " + path, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                webView.CoreWebView2.Navigate(path);
                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            }
            catch (System.Runtime.InteropServices.COMException ex) when ((uint)ex.ErrorCode == 0x80004004 || (uint)ex.ErrorCode == 0x80040154) { }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inicializar o WebView2: " + ex.Message);
            }
        }

        private async void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            string safeJsString = JsonConvert.SerializeObject(tipoMovimentacao);
            await webView.CoreWebView2.ExecuteScriptAsync($"loadData({safeJsString})");
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string jsonString = e.WebMessageAsJson;
                dynamic message = JsonConvert.DeserializeObject(jsonString);
                string actionName = message.action.ToString();

                if (actionName == "CANCEL")
                {
                    this.DialogResult = DialogResult.Cancel;
                }
                else if (actionName == "SAVE")
                {
                    dynamic data = message.data;
                    Movimentacao = new Movimentacao
                    {
                        Tipo = tipoMovimentacao == "Suprimento" ? "Entrada" : "Saída",
                        Categoria = tipoMovimentacao,
                        Subcategoria = data.Subcategoria,
                        Valor = (decimal)data.Valor,
                        FormaPagamento = data.FormaPagamento,
                        Descricao = data.Descricao
                    };
                    this.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao processar mensagem do JS: " + ex.Message);
            }
        }
    }
}

