using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Models;
using MasterServicePro.Utils;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;

namespace MasterServicePro.Forms
{
    public partial class FrmPDVBuscaProduto : Form
    {
        private ProdutoRepository repository = new ProdutoRepository();
        private List<Produto> todosProdutos;
        public Produto ProdutoSelecionado { get; private set; }
        private string filtroInicial;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;

        public FrmPDVBuscaProduto(string filtroInicial = "")
        {
            this.filtroInicial = filtroInicial;
            InitializeComponent();
            this.Shown += FrmPDVBuscaProduto_Shown;
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
            this.webView.Size = new System.Drawing.Size(700, 500);
            this.webView.TabIndex = 0;
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.ClientSize = new System.Drawing.Size(700, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(1);
            try { this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath); } catch { }
            
            this.Controls.Add(this.webView);
            this.Name = "FrmPDVBuscaProduto";
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.ResumeLayout(false);
        }

        private void FrmPDVBuscaProduto_Shown(object sender, EventArgs e)
        {
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"PromptBuscaProduto\index.html");
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
            todosProdutos = repository.BuscarTodos();
            var payload = new
            {
                action = "LOAD_DATA",
                filtroInicial = this.filtroInicial,
                produtos = todosProdutos
            };

            string json = JsonConvert.SerializeObject(payload);
            webView.CoreWebView2.PostWebMessageAsJson(json);
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string msg = e.WebMessageAsJson;
                dynamic data = JsonConvert.DeserializeObject(msg);
                string action = data.action;

                if (action == "CANCEL")
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
                else if (action == "SELECT")
                {
                    int id = data.id;
                    ProdutoSelecionado = todosProdutos.FirstOrDefault(p => p.Id == id);
                    if (ProdutoSelecionado != null)
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao processar retorno web: " + ex.Message);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
