using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using MasterServicePro.Services;
using MasterServicePro.Utils;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;

namespace MasterServicePro.Forms
{
    // Form with embedded WebView2 to handle software license lock and Pix payment
    // All comments must use ASCII characters only.
    public class FrmLicencaWeb : Form
    {
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private string initialKey = string.Empty;
        private bool isExpiredMode = false;
        private string expirationDate = string.Empty;

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        public FrmLicencaWeb(string key = "", bool isExpired = false, string vencimento = "")
        {
            this.initialKey = key;
            this.isExpiredMode = isExpired;
            this.expirationDate = vencimento;

            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                // Close modal on Escape key
                if (e.KeyCode == Keys.Escape)
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            };
            try { this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
            this.Load += FrmLicencaWeb_Load;
        }

        private void InitializeComponent()
        {
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();

            // WebView
            this.webView.DefaultBackgroundColor = Color.FromArgb(10, 13, 20);
            this.webView.Dock = DockStyle.Fill;
            this.webView.Location = new Point(0, 0);
            this.webView.Name = "webView";
            this.webView.Size = new Size(680, 560);
            this.webView.TabIndex = 0;
            this.webView.ZoomFactor = 1D;

            this.AutoScaleMode = AutoScaleMode.None;
            this.BackColor = Color.FromArgb(10, 13, 20);
            this.ClientSize = new Size(680, 560);
            this.MinimumSize = new Size(560, 480);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(1);
            this.Controls.Add(this.webView);
            this.Name = "FrmLicencaWeb";
            this.Text = "Licenciamento - MasterServicePro";

            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);
        }

        protected override void WndProc(ref Message m)
        {
            // Allow border resizing on borderless form
            const int WM_NCHITTEST = 0x84;
            const int HTLEFT = 10;
            const int HTRIGHT = 11;
            const int HTTOP = 12;
            const int HTTOPLEFT = 13;
            const int HTTOPRIGHT = 14;
            const int HTBOTTOM = 15;
            const int HTBOTTOMLEFT = 16;
            const int HTBOTTOMRIGHT = 17;

            if (m.Msg == WM_NCHITTEST)
            {
                base.WndProc(ref m);
                Point pt = this.PointToClient(new Point(m.LParam.ToInt32()));
                int grip = 8;

                if (pt.X <= grip && pt.Y <= grip) { m.Result = (IntPtr)HTTOPLEFT; return; }
                if (pt.X >= this.ClientSize.Width - grip && pt.Y <= grip) { m.Result = (IntPtr)HTTOPRIGHT; return; }
                if (pt.X <= grip && pt.Y >= this.ClientSize.Height - grip) { m.Result = (IntPtr)HTBOTTOMLEFT; return; }
                if (pt.X >= this.ClientSize.Width - grip && pt.Y >= this.ClientSize.Height - grip) { m.Result = (IntPtr)HTBOTTOMRIGHT; return; }
                if (pt.X <= grip) { m.Result = (IntPtr)HTLEFT; return; }
                if (pt.X >= this.ClientSize.Width - grip) { m.Result = (IntPtr)HTRIGHT; return; }
                if (pt.Y <= grip) { m.Result = (IntPtr)HTTOP; return; }
                if (pt.Y >= this.ClientSize.Height - grip) { m.Result = (IntPtr)HTBOTTOM; return; }
                return;
            }
            base.WndProc(ref m);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (var pen = new Pen(Color.FromArgb(40, 255, 255, 255), 1))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }

        private async void FrmLicencaWeb_Load(object sender, EventArgs e)
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);

                string path = WebPathResolver.GetHtmlPath(@"Licenca\index.html");
                path = Path.GetFullPath(path);

                if (!File.Exists(path))
                {
                    MessageBox.Show("Erro: Arquivo HTML de licenca nao encontrado em " + path, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    return;
                }

                string webFolder = Path.GetDirectoryName(Path.GetDirectoryName(path));
                webView.CoreWebView2.SetVirtualHostNameToFolderMapping("app.local", webFolder, CoreWebView2HostResourceAccessKind.Allow);
                webView.CoreWebView2.Navigate("http://app.local/Licenca/index.html");

                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inicializar WebView2 de Licenciamento: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string jsonString = e.WebMessageAsJson;
                dynamic message = JsonConvert.DeserializeObject(jsonString);

                // Handle double encoded string if passed via stringify
                if (message is string strMsg)
                {
                    message = JsonConvert.DeserializeObject(strMsg);
                }

                string action = message?.action?.ToString() ?? string.Empty;

                switch (action)
                {
                    case "ready":
                        await HandleReadyAsync();
                        break;

                    case "drag":
                        ReleaseCapture();
                        SendMessage(this.Handle, 0x112, 0xf012, 0);
                        break;

                    case "minimize":
                        this.WindowState = FormWindowState.Minimized;
                        break;

                    case "maximize":
                        if (this.WindowState == FormWindowState.Maximized)
                            this.WindowState = FormWindowState.Normal;
                        else
                            this.WindowState = FormWindowState.Maximized;
                        break;

                    case "close":
                        this.DialogResult = DialogResult.Cancel;
                        this.Close();
                        break;

                    case "openUrl":
                        try
                        {
                            string targetUrl = message.url;
                            if (!string.IsNullOrEmpty(targetUrl))
                            {
                                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(targetUrl) { UseShellExecute = true });
                            }
                        }
                        catch { }
                        break;

                    case "activateKey":
                        string newKey = message.key;
                        await HandleActivateKeyAsync(newKey);
                        break;

                    case "checkPixStatus":
                        string paymentId = message.paymentId;
                        string key = message.key;
                        await HandleCheckPixStatusAsync(paymentId, key);
                        break;

                    case "unlockSuccess":
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error handling web message: " + ex.Message);
            }
        }

        private async Task HandleReadyAsync()
        {
            // Inject logo Base64 as guarantee
            try
            {
                string logoPath = Path.Combine(Application.StartupPath, "LogoMasterServicePro.png");
                if (!File.Exists(logoPath))
                {
                    logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\LogoMasterServicePro.png");
                }
                if (File.Exists(logoPath))
                {
                    byte[] imageBytes = File.ReadAllBytes(logoPath);
                    string base64String = Convert.ToBase64String(imageBytes);
                    await webView.CoreWebView2.ExecuteScriptAsync($"loadLogo('{base64String}')");
                }
            }
            catch { }

            string key = string.IsNullOrWhiteSpace(initialKey) ? LicenseService.GetStoredLicenseKey() : initialKey;

            if (string.IsNullOrWhiteSpace(key))
            {
                // No key installed: show serial input view
                SendJsonToWeb(new { type = "init", isMissingKey = true });
            }
            else
            {
                // Has key: show expired Pix view and load Pix
                SendJsonToWeb(new
                {
                    type = "init",
                    isMissingKey = false,
                    chave = key,
                    vencimento = expirationDate
                });

                await RequestAndSendPixAsync(key);
            }
        }

        private async Task RequestAndSendPixAsync(string key)
        {
            var pixResult = await LicenseService.GeneratePixAsync(key);
            if (pixResult.Success)
            {
                SendJsonToWeb(new
                {
                    type = "pixGenerated",
                    paymentId = pixResult.PaymentId,
                    valor = pixResult.Valor,
                    qrCodeBase64 = pixResult.QrCodeBase64,
                    copiaCola = pixResult.CopiaCola
                });
            }
            else
            {
                MessageBox.Show(pixResult.Message, "Aviso de Pagamento", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task HandleActivateKeyAsync(string key)
        {
            var result = await LicenseService.CheckLicenseAsync(key);

            if (result.Success && result.Status == "active")
            {
                LicenseService.SaveLicenseKey(key);
                SendJsonToWeb(new
                {
                    type = "activationResult",
                    success = true,
                    message = "Licença ativada com sucesso!"
                });
            }
            else if (result.Status == "expired")
            {
                LicenseService.SaveLicenseKey(key);
                initialKey = key;
                expirationDate = result.VencimentoBr;

                SendJsonToWeb(new
                {
                    type = "init",
                    isMissingKey = false,
                    chave = key,
                    vencimento = result.VencimentoBr
                });

                await RequestAndSendPixAsync(key);
            }
            else
            {
                SendJsonToWeb(new
                {
                    type = "activationResult",
                    success = false,
                    message = result.Message
                });
            }
        }

        private async Task HandleCheckPixStatusAsync(string paymentId, string key)
        {
            var result = await LicenseService.CheckPixStatusAsync(paymentId, key);
            SendJsonToWeb(new
            {
                type = "pixStatusUpdate",
                status = result.Status,
                vencimento = result.VencimentoBr,
                message = result.Message
            });
        }

        private void SendJsonToWeb(object data)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data);
                webView?.CoreWebView2?.PostWebMessageAsString(json);
            }
            catch { }
        }
    }
}
