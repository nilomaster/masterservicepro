using System;
using System.Drawing;
using System.Windows.Forms;
using MasterServicePro.Utils;
using Microsoft.Web.WebView2.Core;

namespace MasterServicePro.Forms
{
    public class FrmPromptCategoria : Form
    {
        public string ValorInput { get; private set; }
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;

        public FrmPromptCategoria()
        {
            InitializeComponent();
            this.Shown += FrmPromptCategoria_Shown;
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
            this.Name = "FrmPromptCategoria";
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.ResumeLayout(false);
        }

        private void FrmPromptCategoria_Shown(object sender, EventArgs e)
        {
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"PromptText\index.html");
                path = System.IO.Path.GetFullPath(path);
                
                if (!System.IO.File.Exists(path))
                {
                    MessageBox.Show("Erro: Arquivo HTML não encontrado em " + path, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string webFolder = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(path));
                webView.CoreWebView2.SetVirtualHostNameToFolderMapping("app.local", webFolder, Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);
                webView.CoreWebView2.Navigate("http://app.local/PromptText/index.html");
                
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
                mode = "categoria",
                title = "Nome da Categoria",
                placeholder = "Digite o nome da nova categoria...",
                confirmText = "SALVAR",
                btnStyle = "success"
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
                    this.ValorInput = message.value.ToString();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
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
