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
    public class FrmListaPrecosWeb : Form
    {
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private ProdutoRepository repository = new ProdutoRepository();
        private CategoriaRepository categoriaRepository = new CategoriaRepository();
        private List<ProdutoPrecoViewModel> viewModels = new List<ProdutoPrecoViewModel>();

        public FrmListaPrecosWeb()
        {
            InitializeComponent();
            this.Load += FrmListaPrecosWeb_Load;
        }

        private void FrmListaPrecosWeb_Load(object sender, EventArgs e)
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
            this.Name = "FrmListaPrecosWeb";
            this.Text = "Lista de Preços (Web)";
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"ListaPrecos\index.html");
                path = Path.GetFullPath(path);
                
                if (!File.Exists(path))
                {
                    MessageBox.Show("Erro: Arquivo HTML não encontrado em " + path, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                webView.CoreWebView2.Navigate(path);
                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                webView.CoreWebView2.SetVirtualHostNameToFolderMapping("appassets", AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'), CoreWebView2HostResourceAccessKind.Allow);
            }
            catch (System.Runtime.InteropServices.COMException ex) when ((uint)ex.ErrorCode == 0x80004004 || (uint)ex.ErrorCode == 0x80040154)
            {
                // Ignorar erros de form fechado ou WebView2 destruído prematuramente
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inicializar WebView2: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    string categoria = msg.categoria != null ? (string)msg.categoria : "";
                    string marca = msg.marca != null ? (string)msg.marca : "";
                    CopiarWhatsApp(categoria, marca);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao processar mensagem web: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task EnviarDadosAoWebAsync()
        {
            try
            {
                var produtos = repository.BuscarTodos();
                var categorias = categoriaRepository.BuscarTodas();
                var dictCategorias = categorias.ToDictionary(c => c.Id, c => c.Nome);

                var produtosComEstoque = produtos.FindAll(p => p.Estoque > 0);

                viewModels = produtosComEstoque.Select(p => new ProdutoPrecoViewModel
                {
                    Id = p.Id,
                    CodigoInterno = p.CodigoInterno,
                    Nome = p.Nome,
                    Categoria = p.IdCategoria.HasValue && dictCategorias.ContainsKey(p.IdCategoria.Value) 
                        ? dictCategorias[p.IdCategoria.Value] 
                        : "Sem Categoria",
                    Marca = p.Marca,
                    Modelo = p.Modelo,
                    Estoque = p.Estoque,
                    PrecoVenda = p.PrecoVenda,
                    ImagemUrl = p.ImagemUrl != null ? "http://appassets/" + p.ImagemUrl.Replace('\\', '/') : null
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
                MessageBox.Show("Erro ao enviar dados para a web: " + ex.Message);
            }
        }

        private void CopiarWhatsApp(string filtroCategoria = "", string filtroMarca = "")
        {
            if (viewModels.Count == 0)
            {
                MessageBox.Show("Não há produtos em estoque para gerar a tabela de preços.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var itensParaCopiar = viewModels;
            if (!string.IsNullOrEmpty(filtroCategoria) && filtroCategoria != "Todas")
            {
                itensParaCopiar = itensParaCopiar.Where(v => v.Categoria == filtroCategoria).ToList();
            }

            if (!string.IsNullOrEmpty(filtroMarca) && filtroMarca != "Todas")
            {
                itensParaCopiar = itensParaCopiar.Where(v => v.Marca == filtroMarca).ToList();
            }

            if (itensParaCopiar.Count == 0)
            {
                MessageBox.Show("Nenhum produto em estoque com os filtros selecionados.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var grupoCategorias = itensParaCopiar.GroupBy(p => p.Categoria).OrderBy(g => g.Key);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("*GEEK CELL DISTRIBUIDORA*");
            sb.AppendLine("💵 TABELA DE PREÇOS - PEÇAS E PRODUTOS");
            sb.AppendLine($"Atualizada em: {DateTime.Now:dd/MM/yyyy HH:mm}");
            sb.AppendLine("--------------------------------------------------");
            sb.AppendLine();

            foreach (var grupo in grupoCategorias)
            {
                sb.AppendLine($"*📁 {grupo.Key.ToUpper()}*");
                
                var produtosPorMarca = grupo.GroupBy(DetectarSubgrupo)
                                            .OrderBy(g => ObterOrdemMarca(g.Key))
                                            .ThenBy(g => g.Key);

                foreach (var grupoMarca in produtosPorMarca)
                {
                    sb.AppendLine();
                    sb.AppendLine($"*_{grupoMarca.Key.ToUpper()}_*");
                    foreach (var item in grupoMarca.OrderBy(x => x.Nome))
                    {
                        // Exemplo pedido: TELA ORIGINAL (MODELO APARELHO) C/A (LZH)
                        // Vamos garantir que o modelo esteja entre parênteses logo após o nome, 
                        // e que a marca ou outras informações fiquem visíveis.
                        
                        string modeloPart = !string.IsNullOrEmpty(item.Modelo) ? $" ({item.Modelo.Trim()})" : "";
                        string marcaPart = !string.IsNullOrEmpty(item.Marca) ? $" ({item.Marca.Trim()})" : "";
                        
                        sb.AppendLine($"• 📱 {item.Nome}{modeloPart}{marcaPart} - *{item.PrecoVenda:C2}*");
                    }
                }
                sb.AppendLine();
            }

            try
            {
                Clipboard.SetText(sb.ToString());
                MessageBox.Show("Tabela de preços copiada com sucesso para a área de transferência! Pronta para colar no WhatsApp.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao copiar tabela de preços: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int ObterOrdemMarca(string marca)
        {
            string upper = marca?.ToUpper() ?? "";
            if (upper.Contains("IPHONE")) return 1;
            if (upper.Contains("SAMSUNG")) return 2;
            if (upper.Contains("MOTOROLA")) return 3;
            if (upper.Contains("XIAOMI") || upper.Contains("REDMI")) return 4;
            if (upper.Contains("REALME")) return 5;
            if (upper.Contains("CHINA")) return 6;
            if (upper.Contains("LG")) return 7;
            if (upper.StartsWith("OUTROS")) return 99;
            return 8;
        }

        private string DetectarSubgrupo(ProdutoPrecoViewModel p)
        {
            string marca = DetectarMarca(p);
            
            if (p.Categoria != null && p.Categoria.Equals("Tela", StringComparison.OrdinalIgnoreCase))
            {
                string nomeLower = p.Nome?.ToLower() ?? "";
                if (nomeLower.Contains("c/a") || nomeLower.Contains("com aro"))
                {
                    return marca + " Com Aro";
                }
                else if (nomeLower.Contains("s/a") || nomeLower.Contains("sem aro"))
                {
                    return marca + " Sem Aro";
                }
            }
            return marca;
        }

        private string DetectarMarca(ProdutoPrecoViewModel p)
        {
            string search = $"{p.Marca} {p.Modelo} {p.Nome}".ToLower();
            
            if (search.Contains("iphone") || search.Contains("apple")) return "iPhone";
            if (search.Contains("samsung") || search.Contains("galaxy")) return "Samsung";
            if (search.Contains("motorola") || search.Contains("moto ")) return "Motorola";
            if (search.Contains("xiaomi") || search.Contains("redmi") || search.Contains("poco")) return "Xiaomi / Redmi";
            if (search.Contains("realme")) return "Realme";
            if (search.Contains("china") || search.Contains("infinix")) return "China";
            if (search.Contains(" lg ") || search.StartsWith("lg ")) return "LG";
            
            if (!string.IsNullOrWhiteSpace(p.Marca)) return p.Marca.Trim();
            
            return "Outros";
        }
    }
}
