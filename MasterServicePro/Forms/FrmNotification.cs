using System;
using System.Drawing;
using System.Windows.Forms;
using MasterServicePro.Utils;
using Microsoft.Web.WebView2.Core;

namespace MasterServicePro.Forms
{
    public partial class FrmNotification : Form
    {
        public enum NotificationType
        {
            Success,
            Error,
            Confirm
        }

        private NotificationType currentType;
        private string mensagem;
        private string titulo;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;

        public FrmNotification(NotificationType type, string mensagem, string titulo)
        {
            this.currentType = type;
            this.mensagem = mensagem;
            this.titulo = titulo;
            
            InitializeComponentWeb();
            this.Shown += FrmNotification_Shown;
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
            this.Name = "FrmNotification";
            
            this.ResumeLayout(false);
        }

        private void FrmNotification_Shown(object sender, EventArgs e)
        {
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"Notification\index.html");
                path = System.IO.Path.GetFullPath(path);
                
                if (!System.IO.File.Exists(path))
                {
                    MessageBox.Show("Erro: Arquivo HTML não encontrado em " + path, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string webFolder = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(path));
                webView.CoreWebView2.SetVirtualHostNameToFolderMapping("app.local", webFolder, CoreWebView2HostResourceAccessKind.Allow);
                webView.CoreWebView2.Navigate("http://app.local/Notification/index.html");
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
                type = currentType.ToString(),
                title = titulo.ToUpper(),
                message = mensagem
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

                if (actionName == "CANCEL")
                {
                    this.DialogResult = DialogResult.No;
                    this.Close();
                }
                else if (actionName == "OK")
                {
                    this.DialogResult = currentType == NotificationType.Confirm ? DialogResult.Yes : DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao processar mensagem do JS: " + ex.Message);
            }
        }

        /// <summary>
        /// Exibe uma mensagem global de erro com estilo SaaS.
        /// </summary>
        public static void ShowError(string mensagem, string titulo = "Erro")
        {
            using (var frm = new FrmNotification(NotificationType.Error, mensagem, titulo))
            {
                frm.ShowDialog();
            }
        }

        /// <summary>
        /// Exibe uma mensagem global de sucesso com estilo SaaS.
        /// </summary>
        public static void ShowSuccess(string mensagem, string titulo = "Sucesso")
        {
            using (var frm = new FrmNotification(NotificationType.Success, mensagem, titulo))
            {
                frm.ShowDialog();
            }
        }

        /// <summary>
        /// Exibe uma mensagem de confirmação Sim/Não com estilo SaaS.
        /// Retorna true se o usuário clicar em SIM, falso caso contrário.
        /// </summary>
        public static bool ShowConfirm(string mensagem, string titulo = "Confirmação")
        {
            using (var frm = new FrmNotification(NotificationType.Confirm, mensagem, titulo))
            {
                var result = frm.ShowDialog();
                return result == DialogResult.Yes;
            }
        }
    }
}
