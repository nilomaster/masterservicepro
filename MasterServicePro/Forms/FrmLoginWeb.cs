using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Utils;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using System.IO;

namespace MasterServicePro.Forms
{
    public class FrmLoginWeb : Form
    {
        private UsuarioRepository repository = new UsuarioRepository();
        private MasterServicePro.Services.LicenseCheckResult _licResult;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private System.Windows.Forms.PictureBox picSpinner;
        private System.Windows.Forms.Label lblLoadingText;
        private System.Windows.Forms.Timer tmrSpinner;
        private float spinnerAngle = 0;

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        public FrmLoginWeb(MasterServicePro.Services.LicenseCheckResult licResult = null)
        {
            _licResult = licResult;
            InitializeComponent();
            try { this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
            this.MouseDown += FrmLoginWeb_MouseDown;
            this.Load += FrmLoginWeb_Load;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Draw a subtle border so the form doesn't blend into a black background while WebView2 is loading
            using (var pen = new Pen(Color.FromArgb(15, 255, 255, 255), 1))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }

        private void InitializeComponent()
        {
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.picSpinner = new System.Windows.Forms.PictureBox();
            this.lblLoadingText = new System.Windows.Forms.Label();
            this.tmrSpinner = new System.Windows.Forms.Timer();
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSpinner)).BeginInit();
            this.SuspendLayout();
            
            // picSpinner
            this.picSpinner.Size = new System.Drawing.Size(50, 50);
            this.picSpinner.Location = new System.Drawing.Point(175, 235);
            this.picSpinner.Name = "picSpinner";
            this.picSpinner.TabIndex = 1;
            this.picSpinner.TabStop = false;
            this.picSpinner.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.picSpinner.Paint += new System.Windows.Forms.PaintEventHandler(this.PicSpinner_Paint);
            
            // lblLoadingText
            this.lblLoadingText.Text = "Carregando...";
            this.lblLoadingText.ForeColor = System.Drawing.Color.White;
            this.lblLoadingText.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.lblLoadingText.AutoSize = true;
            this.lblLoadingText.Location = new System.Drawing.Point(155, 295);
            this.lblLoadingText.Name = "lblLoadingText";
            this.lblLoadingText.TabIndex = 2;
            this.lblLoadingText.Anchor = System.Windows.Forms.AnchorStyles.None;
            
            // tmrSpinner
            this.tmrSpinner.Interval = 20;
            this.tmrSpinner.Tick += new System.EventHandler(this.TmrSpinner_Tick);
            
            // WebView
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.FromArgb(10, 13, 20); // Dark base
            this.webView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView.Location = new System.Drawing.Point(0, 0);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(400, 520);
            this.webView.TabIndex = 0;
            this.webView.ZoomFactor = 1D;
            this.webView.Visible = false; // Hide until HTML is fully loaded
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(10, 13, 20);
            this.ClientSize = new System.Drawing.Size(400, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new System.Windows.Forms.Padding(1);
            
            this.Controls.Add(this.lblLoadingText);
            this.Controls.Add(this.picSpinner);
            this.Controls.Add(this.webView);
            this.Name = "FrmLoginWeb";
            this.Text = "Login - MasterServicePro";
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSpinner)).EndInit();
            this.ResumeLayout(false);
            
            this.tmrSpinner.Start();
        }

        private void TmrSpinner_Tick(object sender, EventArgs e)
        {
            spinnerAngle += 15;
            if (spinnerAngle >= 360) spinnerAngle = 0;
            picSpinner.Invalidate();
        }

        private void PicSpinner_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            // Calculate dynamic size to prevent clipping regardless of DPI
            int padding = 6;
            int size = Math.Min(picSpinner.Width, picSpinner.Height) - (padding * 2);
            int x = (picSpinner.Width - size) / 2;
            int y = (picSpinner.Height - size) / 2;

            using (var penBg = new Pen(Color.FromArgb(50, 255, 255, 255), 4))
            {
                e.Graphics.DrawArc(penBg, x, y, size, size, 0, 360);
            }
            using (var penFg = new Pen(Color.FromArgb(67, 97, 238), 4))
            {
                penFg.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                penFg.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                e.Graphics.DrawArc(penFg, x, y, size, size, spinnerAngle, 90);
            }
        }

        private void FrmLoginWeb_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        }

        private void FrmLoginWeb_Load(object sender, EventArgs e)
        {
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"Login\index.html");
                path = Path.GetFullPath(path);
                
                if (!File.Exists(path))
                {
                    MessageBox.Show("Erro: Arquivo HTML não encontrado em " + path, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string webFolder = Path.GetDirectoryName(Path.GetDirectoryName(path));
                webView.CoreWebView2.SetVirtualHostNameToFolderMapping("app.local", webFolder, CoreWebView2HostResourceAccessKind.Allow);
                webView.CoreWebView2.Navigate("http://app.local/Login/index.html");
                
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
            // Do not hide spinner here. Wait for the 'ready' message from JS.
            // This prevents a black screen on slow internet connections while downloading web fonts/icons.
        }

        private async void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string jsonString = e.WebMessageAsJson;
                dynamic message = JsonConvert.DeserializeObject(jsonString);

                if (message.action == "ready")
                {
                    this.tmrSpinner.Stop();
                    this.picSpinner.Visible = false;
                    this.lblLoadingText.Visible = false;
                    this.webView.Visible = true;

                    // Inject Logo Base64
                    try
                    {
                        string logoPath = Path.Combine(Application.StartupPath, "LogoMasterServicePro.png");
                        if (File.Exists(logoPath))
                        {
                            byte[] imageBytes = File.ReadAllBytes(logoPath);
                            string base64String = Convert.ToBase64String(imageBytes);
                            
                            await webView.CoreWebView2.ExecuteScriptAsync($"loadLogo('{base64String}')");
                        }
                    }
                    catch { }

                    // Send license status to Web view
                    try
                    {
                        var licData = new
                        {
                            isLicensed = _licResult != null && _licResult.Success && _licResult.Status == "active",
                            cliente = _licResult?.Cliente ?? "",
                            vencimento = _licResult?.VencimentoBr ?? "",
                            isOffline = _licResult?.IsOffline ?? false
                        };
                        string jsonLic = JsonConvert.SerializeObject(licData);
                        await webView.CoreWebView2.ExecuteScriptAsync($"setLicenseStatus({jsonLic})");
                    }
                    catch { }

                    return;
                }

                string actionName = message.action.ToString();

                if (actionName == "CLOSE")
                {
                    await System.Threading.Tasks.Task.Delay(50);
                    Application.Exit();
                }
                else if (actionName == "MINIMIZE")
                {
                    this.WindowState = FormWindowState.Minimized;
                }
                else if (actionName == "LOGIN")
                {
                    string usr = message.data.usuario;
                    string pwd = message.data.senha;

                    var repository = new UsuarioRepository();
                    if (repository.Autenticar(usr, pwd))
                    {
                        _ = webView.CoreWebView2.ExecuteScriptAsync($"showSuccess('Autenticado. Abrindo sistema...')");
                        await System.Threading.Tasks.Task.Delay(1500);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        _ = webView.CoreWebView2.ExecuteScriptAsync($"showError('Usuário ou senha inválidos!')");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro JS: " + ex.Message);
            }
        }
    }
}
