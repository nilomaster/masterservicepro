using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.IO;
using Newtonsoft.Json;

namespace MasterServicePro.Forms
{
    public class FrmPromptPagamentoOSWeb : Form
    {
        public string FormaPagamento { get; private set; }
        public bool JaRecebido { get; private set; }
        
        public decimal ValorRecebido { get; private set; }
        public decimal Troco { get; private set; }
        public decimal MistoDinheiro { get; private set; }
        public decimal MistoPix { get; private set; }
        public decimal MistoCartao { get; private set; }
        public decimal MistoSaldo { get; private set; }
        
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private decimal valorCobrado;

        public FrmPromptPagamentoOSWeb(decimal valorCobrado)
        {
            this.valorCobrado = valorCobrado;
            InitializeComponent();
            this.Load += FrmPromptPagamentoOSWeb_Load;
        }

        private void InitializeComponent()
        {
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();
            
            // WebView
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.FromArgb(26, 29, 36);
            this.webView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView.Location = new System.Drawing.Point(0, 0);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(650, 600);
            this.webView.TabIndex = 0;
            this.webView.ZoomFactor = 1D;
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(26, 29, 36);
            this.ClientSize = new System.Drawing.Size(650, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(1);
            try { this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath); } catch { }
            
            this.Controls.Add(this.webView);
            this.Name = "FrmPromptPagamentoOSWeb";
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);
        }



        private void FrmPromptPagamentoOSWeb_Load(object sender, EventArgs e)
        {
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"PromptPagamento\index.html");
                path = Path.GetFullPath(path);
                
                if (!File.Exists(path))
                {
                    MessageBox.Show("Erro: Arquivo HTML não encontrado em " + path, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string webFolder = Path.GetDirectoryName(Path.GetDirectoryName(path));
                webView.CoreWebView2.SetVirtualHostNameToFolderMapping("app.local", webFolder, CoreWebView2HostResourceAccessKind.Allow);
                webView.CoreWebView2.Navigate("http://app.local/PromptPagamento/index.html");
                
                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inicializar o WebView2: " + ex.Message);
            }
        }

        private async void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            var payload = new
            {
                ValorCobrado = valorCobrado
            };

            string json = JsonConvert.SerializeObject(payload);
            string safeJsString = JsonConvert.SerializeObject(json);
            
            await webView.CoreWebView2.ExecuteScriptAsync($"loadFormData({safeJsString})");
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
                    case "CANCEL":
                        await System.Threading.Tasks.Task.Delay(50);
                        this.DialogResult = DialogResult.Cancel;
                        this.Close();
                        break;
                        
                    case "CONFIRMAR":
                        dynamic data = message.data;
                        this.FormaPagamento = data.FormaPagamento;
                        this.JaRecebido = (bool)data.JaRecebido;
                        
                        this.ValorRecebido = data.ValorRecebido != null ? Convert.ToDecimal(data.ValorRecebido) : 0;
                        this.Troco = data.Troco != null ? Convert.ToDecimal(data.Troco) : 0;
                        this.MistoDinheiro = data.MistoDinheiro != null ? Convert.ToDecimal(data.MistoDinheiro) : 0;
                        this.MistoPix = data.MistoPix != null ? Convert.ToDecimal(data.MistoPix) : 0;
                        this.MistoCartao = data.MistoCartao != null ? Convert.ToDecimal(data.MistoCartao) : 0;
                        this.MistoSaldo = data.MistoSaldo != null ? Convert.ToDecimal(data.MistoSaldo) : 0;
                        
                        await System.Threading.Tasks.Task.Delay(50);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                        break;
                }
            }
            catch (Exception ex)
            {
                this.BeginInvoke((Action)(() => {
                    FrmNotification.ShowError("Erro ao processar mensagem do JS: " + ex.Message, "Erro");
                }));
            }
        }
    }
}
