using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.IO;
using Newtonsoft.Json;
using MasterServicePro.DAL;
using MasterServicePro.Models;
using System.Text;

namespace MasterServicePro.Forms
{
    public class FrmNovosPedidosWeb : Form
    {
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private ProdutoRepository repository = new ProdutoRepository();
        private List<ProdutoPedidoViewModel> viewModels = new List<ProdutoPedidoViewModel>();

        public FrmNovosPedidosWeb()
        {
            InitializeComponent();
            this.Load += FrmNovosPedidosWeb_Load;
        }

        private void FrmNovosPedidosWeb_Load(object sender, EventArgs e)
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
            this.Name = "FrmNovosPedidosWeb";
            this.Text = "Novos Pedidos (Web)";
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"NovosPedidos\index.html");
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
                else if (action == "copiar_whatsapp")
                {
                    var updatedDataJson = JsonConvert.SerializeObject(msg.data);
                    var updatedData = JsonConvert.DeserializeObject<List<ProdutoPedidoViewModel>>(updatedDataJson);
                    CopiarWhatsApp(updatedData);
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
                var produtosFiltrados = produtos.FindAll(p => p.Estoque <= 1 || p.Estoque <= p.EstoqueMinimo);

                viewModels = produtosFiltrados.Select(p => new ProdutoPedidoViewModel
                {
                    Id = p.Id,
                    CodigoInterno = p.CodigoInterno,
                    Nome = p.Nome,
                    Marca = p.Marca,
                    Modelo = p.Modelo,
                    Estoque = p.Estoque,
                    EstoqueMinimo = p.EstoqueMinimo,
                    QtdPedir = Math.Max(1, p.EstoqueMinimo > p.Estoque ? (p.EstoqueMinimo - p.Estoque) : 5)
                }).ToList();

                var msg = new
                {
                    action = "load_data",
                    data = viewModels
                };
                
                string jsonResponse = JsonConvert.SerializeObject(msg);
                webView.CoreWebView2.PostWebMessageAsString(jsonResponse);
            }
            catch (Exception ex)
            {
                MessageBox.Show(new Form { TopMost = true }, "Erro ao enviar dados para a web: " + ex.Message);
            }
        }

        private void CopiarWhatsApp(List<ProdutoPedidoViewModel> currentData)
        {
            if (currentData == null || currentData.Count == 0)
            {
                MessageBox.Show("Não há produtos listados para reposição.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("*📋 LISTA DE PEDIDOS - REPOSIÇÃO DE ESTOQUE*");
            sb.AppendLine($"Gerada em: {DateTime.Now:dd/MM/yyyy HH:mm}");
            sb.AppendLine("--------------------------------------------------");

            int count = 0;
            foreach (var vm in currentData)
            {
                if (vm.QtdPedir > 0)
                {
                    string infoExtra = !string.IsNullOrEmpty(vm.Marca) || !string.IsNullOrEmpty(vm.Modelo)
                        ? $" ({vm.Marca} {vm.Modelo})".Replace("  ", " ").Trim()
                        : "";
                    sb.AppendLine($"• {vm.Nome}{infoExtra} - *Qtd: {vm.QtdPedir}* (Estoque atual: {vm.Estoque})");
                    count++;
                }
            }

            if (count == 0)
            {
                MessageBox.Show("Nenhum produto possui quantidade para pedir maior que zero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Clipboard.SetText(sb.ToString());
                MessageBox.Show("Lista de pedidos copiada com sucesso para a área de transferência! Pronta para colar no WhatsApp.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao copiar para área de transferência: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
