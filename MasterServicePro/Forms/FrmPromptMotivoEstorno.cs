using System;
using System.Drawing;
using System.Windows.Forms;
using MasterServicePro.Utils;
using Microsoft.Web.WebView2.Core;

namespace MasterServicePro.Forms
{
    public class FrmPromptMotivoEstorno : Form
    {
        public string Motivo { get; private set; }
        public string DestinoEstorno { get; private set; } // SALDO ou SANGRIA
        public string FormaPagamentoDevolucao { get; private set; }
        private bool _showDestino;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;

        public FrmPromptMotivoEstorno(bool showDestino = false)
        {
            _showDestino = showDestino;
            InitializeComponent();
            this.Shown += FrmPromptMotivoEstorno_Shown;
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
            this.webView.Size = new System.Drawing.Size(500, 320);
            this.webView.TabIndex = 0;
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.ClientSize = new System.Drawing.Size(500, 320);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(1);
            try { this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath); } catch { }
            
            this.Controls.Add(this.webView);
            this.Name = "FrmPromptMotivoEstorno";
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.ResumeLayout(false);
        }

        private void FrmPromptMotivoEstorno_Shown(object sender, EventArgs e)
        {
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"PromptEstorno\index.html");
                if (!System.IO.File.Exists(path))
                {
                    MessageBox.Show("Arquivo não encontrado: " + path);
                    return;
                }
                
                webView.CoreWebView2.Navigate(path);
                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inicializar o WebView2: " + ex.Message);
            }
        }

        private void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            var payload = new
            {
                action = "LOAD_DATA",
                title = "Por que está cancelando essa venda?",
                placeholder = "Digite o motivo do estorno...",
                showDestino = _showDestino
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
            webView.CoreWebView2.PostWebMessageAsJson(json);
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string jsonString = e.WebMessageAsJson;
                dynamic message = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);
                string actionName = message.action.ToString();

                if (actionName == "CANCEL")
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
                else if (actionName == "CONFIRMAR")
                {
                    this.Motivo = message.value.ToString();
                    if (message.destino != null)
                    {
                        this.DestinoEstorno = message.destino.ToString();
                    }
                    else
                    {
                        this.DestinoEstorno = "ERRO_LANCAMENTO"; // Default fallback
                    }
                    this.FormaPagamentoDevolucao = message.formaPagamento?.ToString() ?? "Dinheiro";
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao processar mensagem do JS: " + ex.Message);
            }
        }
    }
}
