using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using MasterServicePro.DAL;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public class FrmEntradaMercadoriaWeb : Form
    {
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private readonly EntradaMercadoriaRepository _repository = new EntradaMercadoriaRepository();

        public FrmEntradaMercadoriaWeb()
        {
            InitializeComponent();
            this.Load += FrmEntradaMercadoriaWeb_Load;
        }

        private void FrmEntradaMercadoriaWeb_Load(object sender, EventArgs e)
        {
            InitializeAsync();
        }

        private void InitializeComponent()
        {
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();
            
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.FromArgb(26, 29, 36);
            this.webView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView.Location = new System.Drawing.Point(0, 0);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(900, 600);
            this.webView.TabIndex = 0;
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(26, 29, 36);
            this.ClientSize = new System.Drawing.Size(1200, 750);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(1);
            try { this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
            this.StartPosition = FormStartPosition.CenterParent;
            this.Controls.Add(this.webView);
            this.Name = "FrmEntradaMercadoriaWeb";
            this.Text = "Entradas de Mercadorias";
            
            this.Paint += (s, e) => {
                using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(51, 65, 85), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
                }
            };
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"EntradaMercadoria\index.html");
                path = Path.GetFullPath(path);
                
                if (!File.Exists(path))
                {
                    MessageBox.Show("Erro: Arquivo HTML não encontrado em " + path, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                webView.CoreWebView2.Navigate(path);
                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            }
            catch (System.Runtime.InteropServices.COMException ex) when ((uint)ex.ErrorCode == 0x80004004 || (uint)ex.ErrorCode == 0x80040154) { }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inicializar o WebView2: " + ex.Message);
            }
        }

        private async void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string jsonString = e.WebMessageAsJson;
                dynamic message = JsonConvert.DeserializeObject(jsonString);
                string actionName = message.action.ToString();

                if (actionName == "fechar")
                {
                    this.Close();
                }
                else if (actionName == "request_data")
                {
                    await EnviarDadosAoWebAsync();
                }
                else if (actionName == "nova_entrada")
                {
                    this.BeginInvoke((Action)(async () => {
                        using (var frm = new FrmEntradaMercadoriaAddWeb())
                        {
                            if (frm.ShowDialog() == DialogResult.OK)
                            {
                                await EnviarDadosAoWebAsync();
                            }
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao processar mensagem do Web: " + ex.Message);
            }
        }
        
        private async System.Threading.Tasks.Task EnviarDadosAoWebAsync()
        {
            try
            {
                DataTable dt = _repository.ListarTodas();
                var msg = new
                {
                    action = "load_data",
                    entradas = dt
                };
                
                string jsonResponse = JsonConvert.SerializeObject(msg);
                webView.CoreWebView2.PostWebMessageAsString(jsonResponse);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar os dados: " + ex.Message);
            }
        }
    }
}

