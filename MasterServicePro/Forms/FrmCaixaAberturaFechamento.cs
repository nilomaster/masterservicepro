using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.IO;
using Newtonsoft.Json;

namespace MasterServicePro.Forms
{
    public partial class FrmCaixaAberturaFechamento : Form
    {
        public decimal Valor { get; private set; }
        private bool isAbertura;
        
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;

        public FrmCaixaAberturaFechamento(bool abertura)
        {
            this.isAbertura = abertura;
            InitializeComponent();
            this.Shown += FrmCaixaAberturaFechamento_Shown;
        }

        private void InitializeComponent()
        {
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            // WebView
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.FromArgb(26, 29, 36);
            this.webView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView.Location = new System.Drawing.Point(0, 0);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(600, 500);
            this.webView.TabIndex = 0;
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.ClientSize = new System.Drawing.Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(1);
            try { this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath); } catch { }
            
            this.Controls.Add(this.webView);
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();
            this.ResumeLayout(false);
        }



        private void FrmCaixaAberturaFechamento_Shown(object sender, EventArgs e)
        {
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"CaixaAberturaFechamento\index.html");
                path = Path.GetFullPath(path);
                
                string webFolder = Path.GetDirectoryName(Path.GetDirectoryName(path));
                webView.CoreWebView2.SetVirtualHostNameToFolderMapping("app.local", webFolder, CoreWebView2HostResourceAccessKind.Allow);
                webView.CoreWebView2.Navigate("http://app.local/CaixaAberturaFechamento/index.html");
                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            }
            catch (System.Runtime.InteropServices.COMException ex) when ((uint)ex.ErrorCode == 0x80004004) { }
            catch { }
        }

        private async void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            string safeJsString = JsonConvert.SerializeObject(isAbertura);
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
                    decimal val = (decimal)message.valor;
                    
                    if (isAbertura && val < 10m)
                    {
                        this.BeginInvoke((Action)(() => {
                            FrmNotification.ShowError("O valor de fundo de troco para abertura do caixa deve ser de no mínimo R$ 10,00.", "Atenção");
                        }));
                        return;
                    }

                    Valor = val;
                    this.DialogResult = DialogResult.OK;
                }
            }
            catch { }
        }
    }
}

