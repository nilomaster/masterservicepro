using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public partial class FrmListaPrecos : Form
    {
        private ProdutoRepository repository = new ProdutoRepository();
        private CategoriaRepository categoriaRepository = new CategoriaRepository();
        private List<ProdutoPrecoViewModel> viewModels = new List<ProdutoPrecoViewModel>();

        public FrmListaPrecos()
        {
            InitializeComponent();
            ApplyTheme();
            LoadData();

            // Events
            btnCopiarWhatsApp.Click += BtnCopiarWhatsApp_Click;
            btnAtualizar.Click += BtnAtualizar_Click;
            txtBusca.TextChanged += TxtBusca_TextChanged;
        }

        private void ApplyTheme()
        {
            this.BackColor = UITheme.BgCard;

            pnlHeader.BackColor = UITheme.BgCard;
            pnlSearch.BackColor = UITheme.BgCard;
            tlpMain.BackColor = UITheme.BgCard;

            lblTitle.Font = UITheme.FontTitle;
            lblTitle.ForeColor = UITheme.TextTitle;

            lblBusca.Font = UITheme.FontBody;
            lblBusca.ForeColor = UITheme.TextMuted;

            txtBusca.Font = UITheme.FontBody;
            txtBusca.BackColor = UITheme.BgSidebar;
            txtBusca.ForeColor = UITheme.TextTitle;
            txtBusca.BorderStyle = BorderStyle.FixedSingle;

            UITheme.FormatSaaSButton(btnCopiarWhatsApp, true);
            UITheme.FormatSaaSButton(btnAtualizar, false);

            // Style DataGridView (Premium)
            UITheme.FormatGrid(dgvProdutos);
        }

        private void LoadData(string filtro = "")
        {
            try
            {
                var produtos = repository.BuscarTodos();
                var categorias = categoriaRepository.BuscarTodas();

                // Dicionário de Categorias para busca rápida
                var dictCategorias = categorias.ToDictionary(c => c.Id, c => c.Nome);

                // Filtrar apenas produtos ATIVOS com estoque > 0
                var produtosComEstoque = produtos.FindAll(p => p.Estoque > 0);

                // Mapear para view models
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
                    ImagemUrl = p.ImagemUrl
                }).ToList();

                ExibirLista(filtro);
            }
            catch (Exception ex)
            {
                FrmNotification.ShowError("Erro ao carregar lista de preços: " + ex.Message, "Erro");
            }
        }

        private void ExibirLista(string filtro)
        {
            var listaExibicao = viewModels;

            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim().ToLower();
                var termos = filtro.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                listaExibicao = viewModels.FindAll(c =>
                    termos.All(t => c.Nome.ToLower().Contains(t)) ||
                    (c.CodigoInterno != null && c.CodigoInterno.ToLower().Contains(filtro)) ||
                    (c.Categoria != null && c.Categoria.ToLower().Contains(filtro))
                );
            }

            dgvProdutos.DataSource = null;
            dgvProdutos.DataSource = listaExibicao;

            // Formatar colunas
            if (dgvProdutos.Columns.Count > 0)
            {
                dgvProdutos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                foreach (DataGridViewColumn col in dgvProdutos.Columns)
                {
                    col.Visible = false; // ocultar tudo primeiro
                }

                if (dgvProdutos.Columns["Id"] != null)
                {
                    dgvProdutos.Columns["Id"].Visible = true;
                    dgvProdutos.Columns["Id"].Width = 50;
                }

                if (dgvProdutos.Columns["CodigoInterno"] != null)
                {
                    dgvProdutos.Columns["CodigoInterno"].Visible = true;
                    dgvProdutos.Columns["CodigoInterno"].HeaderText = "Cód.";
                    dgvProdutos.Columns["CodigoInterno"].Width = 80;
                }

                if (dgvProdutos.Columns["Nome"] != null)
                {
                    dgvProdutos.Columns["Nome"].Visible = true;
                    dgvProdutos.Columns["Nome"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                if (dgvProdutos.Columns["Categoria"] != null)
                {
                    dgvProdutos.Columns["Categoria"].Visible = true;
                    dgvProdutos.Columns["Categoria"].HeaderText = "Categoria";
                    dgvProdutos.Columns["Categoria"].Width = 130;
                }

                if (dgvProdutos.Columns["Marca"] != null)
                {
                    dgvProdutos.Columns["Marca"].Visible = true;
                    dgvProdutos.Columns["Marca"].HeaderText = "Marca";
                    dgvProdutos.Columns["Marca"].Width = 110;
                }

                if (dgvProdutos.Columns["Modelo"] != null)
                {
                    dgvProdutos.Columns["Modelo"].Visible = true;
                    dgvProdutos.Columns["Modelo"].HeaderText = "Modelo";
                    dgvProdutos.Columns["Modelo"].Width = 110;
                }

                if (dgvProdutos.Columns["Estoque"] != null)
                {
                    dgvProdutos.Columns["Estoque"].Visible = true;
                    dgvProdutos.Columns["Estoque"].HeaderText = "Disponível";
                    dgvProdutos.Columns["Estoque"].Width = 80;
                }

                if (dgvProdutos.Columns["PrecoVenda"] != null)
                {
                    dgvProdutos.Columns["PrecoVenda"].Visible = true;
                    dgvProdutos.Columns["PrecoVenda"].HeaderText = "Preço Venda";
                    dgvProdutos.Columns["PrecoVenda"].DefaultCellStyle.Format = "C2";
                    dgvProdutos.Columns["PrecoVenda"].Width = 120;
                }

                dgvProdutos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        private void TxtBusca_TextChanged(object sender, EventArgs e)
        {
            ExibirLista(txtBusca.Text);
        }

        private void BtnAtualizar_Click(object sender, EventArgs e)
        {
            LoadData(txtBusca.Text);
        }

        private void BtnCopiarWhatsApp_Click(object sender, EventArgs e)
        {
            if (viewModels.Count == 0)
            {
                FrmNotification.ShowError("Não há produtos em estoque para gerar a tabela de preços.", "Aviso");
                return;
            }

            // Agrupar produtos por Categoria
            var grupoCategorias = viewModels.GroupBy(p => p.Categoria).OrderBy(g => g.Key);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("*💵 TABELA DE PREÇOS - PEÇAS E PRODUTOS*");
            sb.AppendLine($"Atualizada em: {DateTime.Now:dd/MM/yyyy HH:mm}");
            sb.AppendLine("--------------------------------------------------");
            sb.AppendLine();

            foreach (var grupo in grupoCategorias)
            {
                sb.AppendLine($"*📁 {grupo.Key.ToUpper()}*");
                foreach (var item in grupo.OrderBy(x => x.Nome))
                {
                    string infoExtra = !string.IsNullOrEmpty(item.Marca) || !string.IsNullOrEmpty(item.Modelo)
                        ? $" ({item.Marca} {item.Modelo})".Replace("  ", " ").Trim()
                        : "";
                    sb.AppendLine($"• {item.Nome}{infoExtra} - *{item.PrecoVenda:C2}*");
                }
                sb.AppendLine();
            }

            try
            {
                Clipboard.SetText(sb.ToString());
                FrmNotification.ShowSuccess("Tabela de preços copiada com sucesso para a área de transferência! Pronta para colar no WhatsApp.", "Sucesso");
            }
            catch (Exception ex)
            {
                FrmNotification.ShowError("Erro ao copiar tabela de preços: " + ex.Message, "Erro");
            }
        }
    }

    public class ProdutoPrecoViewModel
    {
        public int Id { get; set; }
        public string CodigoInterno { get; set; }
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int Estoque { get; set; }
        public decimal PrecoVenda { get; set; }
        public string ImagemUrl { get; set; }
    }
}
