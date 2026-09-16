using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.IO;
using Newtonsoft.Json;
using MasterServicePro.Models;
using MasterServicePro.DAL;

namespace MasterServicePro.Forms
{
    public partial class FrmProdutoAddEdit : Form
    {
        private int produtoId = 0;
        private ProdutoRepository repository = new ProdutoRepository();
        private CategoriaRepository categoriaRepository = new CategoriaRepository();
        private FornecedorRepository fornecedorRepository = new FornecedorRepository();
        public int SavedProdutoId { get; private set; }
        
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;

        public FrmProdutoAddEdit(int id = 0)
        {
            this.produtoId = id;
            InitializeComponent();
            this.Shown += FrmProdutoAddEdit_Shown;
        }

        private void InitializeComponent()
        {
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            // WebView
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.FromArgb(26, 29, 36);
            this.webView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView.Location = new System.Drawing.Point(0, 0);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(800, 700);
            this.webView.TabIndex = 0;
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.ClientSize = new System.Drawing.Size(800, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(1);
            try { this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath); } catch { }
            
            this.Controls.Add(this.webView);
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();
            this.ResumeLayout(false);
        }



        private void FrmProdutoAddEdit_Shown(object sender, EventArgs e)
        {
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"ProdutoAddEdit\index.html");
                path = Path.GetFullPath(path);
                
                webView.CoreWebView2.Navigate(path);
                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            }
            catch (System.Runtime.InteropServices.COMException ex) when ((uint)ex.ErrorCode == 0x80004004) { }
            catch { }
        }

        private async void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            var categorias = categoriaRepository.BuscarTodas();
            var fornecedores = await fornecedorRepository.BuscarTodosAsync();
            Produto p = null;
            string imagemBase64 = null;
            if (produtoId > 0)
            {
                p = repository.BuscarPorId(produtoId);
                if (p != null && !string.IsNullOrEmpty(p.ImagemUrl))
                {
                    string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, p.ImagemUrl);
                    if (File.Exists(fullPath))
                    {
                        try
                        {
                            byte[] imgBytes = File.ReadAllBytes(fullPath);
                            string ext = Path.GetExtension(fullPath).ToLower();
                            string mime = "image/png";
                            if (ext == ".jpg" || ext == ".jpeg") mime = "image/jpeg";
                            else if (ext == ".webp") mime = "image/webp";
                            else if (ext == ".gif") mime = "image/gif";
                            
                            imagemBase64 = $"data:{mime};base64,{Convert.ToBase64String(imgBytes)}";
                        }
                        catch { }
                    }
                }
            }

            var data = new {
                id = produtoId,
                categorias = categorias,
                fornecedores = fornecedores,
                produto = p != null ? new {
                    p.Id, p.IdCategoria, p.IdFornecedor, p.Nome, p.CodigoBarras, p.Marca, p.Modelo,
                    p.Garantia, p.PrecoCusto, p.PrecoVenda, p.Estoque, p.EstoqueMinimo,
                    ImagemUrlBase64 = imagemBase64
                } : null
            };

            string safeJsString = JsonConvert.SerializeObject(data);
            await webView.CoreWebView2.ExecuteScriptAsync($"loadData({safeJsString})");
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string jsonString = e.WebMessageAsJson;
                dynamic message = JsonConvert.DeserializeObject(jsonString);
                string actionName = message.action.ToString();

                if (actionName == "CANCEL")
                {
                    this.DialogResult = DialogResult.Cancel;
                }
                else if (actionName == "SAVE")
                {
                    dynamic pd = message.produto;
                    
                    string codigoBarras = (string)pd.CodigoBarras;
                    if (string.IsNullOrEmpty(codigoBarras))
                    {
                        Random rnd = new Random();
                        codigoBarras = "789" + rnd.Next(100000000, 999999999).ToString();
                    }

                    int idCategoria = (int)pd.IdCategoria;
                    int idFornecedor = pd.IdFornecedor != null ? (int)pd.IdFornecedor : 0;

                    var p = new Produto
                    {
                        Id = produtoId,
                        IdCategoria = idCategoria > 0 ? (int?)idCategoria : null,
                        IdFornecedor = idFornecedor > 0 ? (int?)idFornecedor : null,
                        Nome = pd.Nome,
                        CodigoBarras = codigoBarras,
                        Marca = pd.Marca,
                        Modelo = pd.Modelo,
                        Garantia = pd.Garantia,
                        PrecoCusto = (decimal)pd.PrecoCusto,
                        PrecoVenda = (decimal)pd.PrecoVenda,
                        Estoque = (int)pd.Estoque,
                        EstoqueMinimo = (int)pd.EstoqueMinimo,
                        DataAtualizacao = DateTime.Now
                    };

                    string imagemBase64 = pd.ImagemBase64 != null ? (string)pd.ImagemBase64 : null;
                    string imagemFileName = pd.ImagemFileName != null ? (string)pd.ImagemFileName : null;

                    if (!string.IsNullOrEmpty(imagemBase64) && !string.IsNullOrEmpty(imagemFileName))
                    {
                        try
                        {
                            string[] split = imagemBase64.Split(',');
                            if (split.Length == 2)
                            {
                                byte[] imgBytes = Convert.FromBase64String(split[1]);
                                string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uploads", "Produtos");
                                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                                
                                string ext = Path.GetExtension(imagemFileName);
                                string uniqueName = Guid.NewGuid().ToString() + ext;
                                string savePath = Path.Combine(dir, uniqueName);
                                
                                File.WriteAllBytes(savePath, imgBytes);
                                p.ImagemUrl = Path.Combine("Uploads", "Produtos", uniqueName);
                            }
                        }
                        catch { }
                    }
                    else if (produtoId > 0)
                    {
                        var pExistente = repository.BuscarPorId(produtoId);
                        if (pExistente != null) p.ImagemUrl = pExistente.ImagemUrl;
                    }

                    if (produtoId == 0)
                    {
                        p.Ativo = true;
                        repository.Inserir(p);
                    }
                    else
                    {
                        repository.Atualizar(p);
                    }

                    this.SavedProdutoId = p.Id;
                    this.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                this.BeginInvoke((Action)(() => {
                    FrmNotification.ShowError("Erro ao salvar produto: " + ex.Message, "Erro");
                }));
            }
        }
    }
}

