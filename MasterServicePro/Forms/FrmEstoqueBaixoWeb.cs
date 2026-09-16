using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.IO;
using Newtonsoft.Json;
using MasterServicePro.DAL;
using MasterServicePro.Models;

namespace MasterServicePro.Forms
{
    public class FrmEstoqueBaixoWeb : Form
    {
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private ProdutoRepository repository = new ProdutoRepository();

        public FrmEstoqueBaixoWeb()
        {
            InitializeComponent();
            this.Load += FrmEstoqueBaixoWeb_Load;
        }

        private void FrmEstoqueBaixoWeb_Load(object sender, EventArgs e)
        {
            InitializeAsync();
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
            this.webView.Size = new System.Drawing.Size(1200, 750);
            this.webView.TabIndex = 0;
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(26, 29, 36);
            this.ClientSize = new System.Drawing.Size(1200, 750);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Controls.Add(this.webView);
            this.Name = "FrmEstoqueBaixoWeb";
            this.Text = "Falta de Estoque (Web)";
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"EstoqueBaixo\index.html");
                path = Path.GetFullPath(path);
                
                if (!File.Exists(path))
                {
                    MessageBox.Show("Erro: Arquivo HTML não encontrado em " + path, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                webView.CoreWebView2.Navigate(path);
                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            }
            catch (System.Runtime.InteropServices.COMException ex) when ((uint)ex.ErrorCode == 0x80004004 || (uint)ex.ErrorCode == 0x80040154)
            {
                // Ignorar erros de form fechado ou WebView2 destruído prematuramente
            }
            catch (Exception ex)
            {
                MessageBox.Show(new Form { TopMost = true }, "Erro ao inicializar WebView2: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var json = e.WebMessageAsJson;
                dynamic msg = JsonConvert.DeserializeObject(json);
                string action = msg.action;

                if (action == "request_data" || action == "atualizar")
                {
                    await EnviarDadosAoWebAsync();
                }
                else if (action == "editar_selecionado")
                {
                    int id = msg.id;
                    this.BeginInvoke((Action)(async () => {
                        using (var frm = new FrmProdutoAddEdit(id))
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
                MessageBox.Show(new Form { TopMost = true }, "Erro ao processar mensagem web: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task EnviarDadosAoWebAsync()
        {
            try
            {
                var produtos = repository.BuscarTodos();
                var produtosBaixoEstoque = produtos.FindAll(p => p.Estoque <= p.EstoqueMinimo || p.Estoque <= 0);

                var msg = new
                {
                    action = "load_data",
                    data = produtosBaixoEstoque
                };
                
                string jsonResponse = JsonConvert.SerializeObject(msg);
                webView.CoreWebView2.PostWebMessageAsString(jsonResponse);
            }
            catch (Exception ex)
            {
                MessageBox.Show(new Form { TopMost = true }, "Erro ao enviar dados para a web: " + ex.Message);
            }
        }
    }
}
