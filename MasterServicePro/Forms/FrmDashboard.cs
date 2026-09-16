using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.IO;
using Newtonsoft.Json;
using MasterServicePro.DAL;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public partial class FrmDashboard : Form
    {
        private Form activedForm = null;
        private WebView2 webViewSidebar;
        private WebView2 webViewHeader;

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        public FrmDashboard()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            try { this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
            ApplyTheme();

            // Check Permissions logic handled in JS now, we just pass the info
            if (!AuthSession.IsAdmin)
            {
                // Just hide the sidebar physically if the user doesn't even have access
                // But wait, the user needs the sidebar to click PDV, OS, etc.
                // So we just let JS handle the hiding of Admin items.
            }

            // Events
            this.DoubleBuffered = true;
            this.MaximizedBounds = Screen.PrimaryScreen.WorkingArea;
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1024, 768);

            // Start collapsed
            tableLayoutPanelMain.ColumnStyles[0].Width = 80;

            BindEvents();
            InitWebViewSidebar();
            InitWebViewHeader();
        }

        private async void InitWebViewHeader()
        {
            try
            {
                webViewHeader = new WebView2();
                webViewHeader.DefaultBackgroundColor = System.Drawing.Color.FromArgb(26, 29, 36);
                webViewHeader.Dock = DockStyle.Fill;
                
                // Remove os controles antigos
                pnlHeader.Controls.Clear();
                pnlHeader.Controls.Add(webViewHeader);

                await webViewHeader.EnsureCoreWebView2Async(null);

                string htmlPath = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"Header\index.html");
                webViewHeader.CoreWebView2.Navigate(Path.GetFullPath(htmlPath));

                webViewHeader.WebMessageReceived += (s, e) =>
                {
                    try
                    {
                        var msg = JsonConvert.DeserializeObject<dynamic>(e.WebMessageAsJson);
                        string action = msg.action;

                        if (action == "drag")
                        {
                            ReleaseCapture();
                            SendMessage(this.Handle, 0x112, 0xf012, 0);
                        }
                        else if (action == "close")
                        {
                            this.Close();
                        }
                        else if (action == "minimize")
                        {
                            this.WindowState = FormWindowState.Minimized;
                        }
                    }
                    catch { }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar o header: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void InitWebViewSidebar()
        {
            try
            {
                webViewSidebar = new WebView2();
                webViewSidebar.DefaultBackgroundColor = System.Drawing.Color.FromArgb(26, 29, 36);
                webViewSidebar.Dock = DockStyle.Fill;
                pnlSidebar.Controls.Add(webViewSidebar);

                await webViewSidebar.EnsureCoreWebView2Async(null);

                string htmlPath = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"Sidebar\index.html");
                htmlPath = Path.GetFullPath(htmlPath);
                webViewSidebar.CoreWebView2.Navigate(htmlPath);

                webViewSidebar.WebMessageReceived += WebViewSidebar_WebMessageReceived;
                webViewSidebar.CoreWebView2.NavigationCompleted += (s, e) =>
                {
                    var msg = new {
                        action = "init_user",
                        user = AuthSession.Usuario,
                        role = AuthSession.Nivel,
                        isAdmin = AuthSession.IsAdmin
                    };
                    webViewSidebar.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(msg));
                    
                    // Set default page
                    string defaultTarget = AuthSession.IsAdmin ? "btnDashboard" : "btnPDV";
                    var setmsg = new { action = "set_active", target = defaultTarget };
                    webViewSidebar.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(setmsg));
                    
                    if (AuthSession.IsAdmin) OpenChildForm(new FrmHomeAdminWeb());
                    else OpenChildForm(new FrmPDVWeb());
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar a barra lateral: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WebViewSidebar_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var msg = JsonConvert.DeserializeObject<dynamic>(e.WebMessageAsJson);
                string action = msg.action;

                if (action == "toggle_sidebar")
                {
                    bool expanded = msg.expanded;
                    tableLayoutPanelMain.ColumnStyles[0].Width = expanded ? 250 : 80;
                }
                else if (action == "open_form")
                {
                    string target = msg.target;
                    this.BeginInvoke(new Action(() => {
                        switch (target)
                        {
                            case "btnDashboard": OpenChildForm(new FrmHomeAdminWeb()); break;
                            case "btnPDV": OpenChildForm(new FrmPDVWeb()); break;
                            case "btnOS": OpenChildForm(new FrmOSWeb()); break;
                            case "btnProdutos": OpenChildForm(new FrmProdutosWeb()); break;
                            case "btnEstoqueBaixo": OpenChildForm(new FrmEstoqueBaixoWeb()); break;
                            case "btnNovosPedidos": OpenChildForm(new FrmNovosPedidosWeb()); break;
                            case "btnListaPrecos": OpenChildForm(new FrmListaPrecosWeb()); break;
                            case "btnClientes": OpenChildForm(new FrmClientesWeb()); break;
                            case "btnFinanceiro": OpenChildForm(new FrmFinanceiroWeb()); break;
                            case "btnRelatorios": OpenChildForm(new FrmRelatoriosWeb()); break;
                            case "btnConfiguracoes": OpenChildForm(new FrmConfiguracoesWeb()); break;
                        }
                    }));
                }
            }
            catch { }
        }

        private void BindEvents()
        {
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void ApplyTheme()
        {
            // Fundo principal
            this.BackColor = UITheme.BgApp;
            
            // Sidebar
            pnlSidebar.BackColor = UITheme.BgSidebar;
            
            // Top Header
            // Top Header
            pnlHeader.BackColor = UITheme.BgApp;
            // Native labels no longer used, handled by Webview

            // Area Principal
            pnlMain.BackColor = UITheme.BgApp;
        }

        public void OpenChildForm(Form childForm)
        {
            if (activedForm != null)
            {
                activedForm.Close();
                activedForm.Dispose();
            }

            pnlMain.Controls.Clear();
            activedForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            pnlMain.Controls.Add(childForm);
            pnlMain.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }
    }
}
