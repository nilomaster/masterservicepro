using System;
using System.Drawing;
using System.Windows.Forms;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public partial class FrmPDVSucesso : Form
    {
        private string pagamento;
        private decimal total;
        private decimal troco;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;

        public FrmPDVSucesso(string pagamento, decimal total, decimal troco)
        {
            this.pagamento = pagamento;
            this.total = total;
            this.troco = troco;
            
            InitializeComponentWeb();
            this.Shown += FrmPDVSucesso_Shown;
        }

        private void InitializeComponentWeb()
        {
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.SuspendLayout();
            
            // WebView
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.FromArgb(26, 29, 36);
            this.webView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView.Location = new System.Drawing.Point(0, 0);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(550, 450);
            this.webView.TabIndex = 0;
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.ClientSize = new System.Drawing.Size(550, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(1);
            try { this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath); } catch { }
            
            this.Controls.Add(this.webView);
            this.Name = "FrmPDVSucesso";
            
            this.ResumeLayout(false);
        }

        private void FrmPDVSucesso_Shown(object sender, EventArgs e)
        {
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Web\PDVSucesso\index.html");
                path = System.IO.Path.GetFullPath(path);
                
                webView.CoreWebView2.Navigate(path);
                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inicializar o WebView2: " + ex.Message);
            }
        }

        private void CoreWebView2_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            var payload = new
            {
                action = "LOAD_DATA",
                pagamento = this.pagamento,
                total = this.total.ToString("C2"),
                trocoValue = this.troco,
                trocoFormat = this.troco.ToString("C2")
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
            webView.CoreWebView2.PostWebMessageAsJson(json);
        }

        private void CoreWebView2_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string jsonString = e.WebMessageAsJson;
                dynamic message = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);
                string actionName = message.action.ToString();

                if (actionName == "OK")
                {
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
