using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using MasterServicePro.DAL;
using MasterServicePro.Models;

namespace MasterServicePro.Forms
{
    public class FrmEntradaMercadoriaAddWeb : Form
    {
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private readonly FornecedorRepository _fornecedorRepository = new FornecedorRepository();
        private readonly EntradaMercadoriaRepository _entradaRepository = new EntradaMercadoriaRepository();

        public FrmEntradaMercadoriaAddWeb()
        {
            InitializeComponent();
            this.Load += FrmEntradaMercadoriaAddWeb_Load;
        }

        private void FrmEntradaMercadoriaAddWeb_Load(object sender, EventArgs e)
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
            this.webView.Size = new System.Drawing.Size(1200, 750);
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
            this.Name = "FrmEntradaMercadoriaAddWeb";
            this.Text = "Registrar Entrada de Mercadoria";
            
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
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"EntradaMercadoriaAdd\index.html");
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
                else if (actionName == "request_init")
                {
                    await EnviarFornecedoresAsync();
                }
                else if (actionName == "buscar_produto")
                {
                    string query = message.query;
                    
                    var repoBusca = new MasterServicePro.DAL.ProdutoRepository();
                    var todos = repoBusca.BuscarTodos();
                    
                    query = query ?? "";
                    var termos = query.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var filtrados = string.IsNullOrWhiteSpace(query) 
                        ? todos.Take(20).ToList() 
                        : todos.Where(p => termos.All(t => p.Nome.ToLower().Contains(t)) || 
                                          (p.CodigoBarras != null && p.CodigoBarras.Contains(query))).Take(20).ToList();
                    
                    var msg = new { action = "resultados_busca", produtos = filtrados };
                    string jsonResponse = JsonConvert.SerializeObject(msg);
                    webView.CoreWebView2.PostWebMessageAsString(jsonResponse);
                }
                else if (actionName == "gravar")
                {
                    var payload = message.payload;
                    var entrada = new EntradaMercadoria
                    {
                        IdFornecedor = (int)payload.IdFornecedor,
                        NumeroNota = payload.NumeroNota != null ? payload.NumeroNota.ToString() : "",
                        Observacao = payload.Observacao != null ? payload.Observacao.ToString() : "",
                        ValorTotal = (decimal)payload.ValorTotal
                    };
                    
                    foreach (var item in payload.Itens)
                    {
                        entrada.Itens.Add(new EntradaMercadoriaItem
                        {
                            IdProduto = (int)item.IdProduto,
                            NomeProduto = item.NomeProduto.ToString(),
                            Quantidade = (int)item.Quantidade,
                            PrecoCusto = (decimal)item.PrecoCusto,
                            SubTotal = (decimal)item.SubTotal
                        });
                    }

                    this.BeginInvoke((Action)(() => {
                        try
                        {
                            _entradaRepository.RegistrarEntrada(entrada);
                            MessageBox.Show("Entrada registrada com sucesso e custo médio recalculado!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Erro ao gravar entrada: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao processar mensagem do Web: " + ex.Message);
            }
        }
        
        private async System.Threading.Tasks.Task EnviarFornecedoresAsync()
        {
            try
            {
                var list = await _fornecedorRepository.BuscarTodosAsync();
                var msg = new
                {
                    action = "load_fornecedores",
                    fornecedores = list
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

