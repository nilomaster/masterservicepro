using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public partial class FrmProdutos : Form
    {
        private ProdutoRepository repository = new ProdutoRepository();
        private CategoriaRepository categoriaRepository = new CategoriaRepository();
        private Button btnEntradaEstoque;

        public FrmProdutos()
        {
            InitializeComponent();
            InicializarComponentesEstoque();
            ApplyTheme();
            LoadCategorias();
            LoadData();
            
            // Events 
            btnNovo.Click += BtnNovo_Click;
            btnEditar.Click += BtnEditar_Click;
            btnExcluir.Click += BtnExcluir_Click;
            txtBusca.TextChanged += TxtBusca_TextChanged;
            lstCategorias.SelectedIndexChanged += LstCategorias_SelectedIndexChanged;
            lstCategorias.DrawItem += LstCategorias_DrawItem;
        }

        private void InicializarComponentesEstoque()
        {
            btnEntradaEstoque = new Button
            {
                Text = "📥 Entrada Estoque",
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(btnNovo.Left - 155, btnNovo.Top),
                Size = new Size(145, btnNovo.Height),
                UseVisualStyleBackColor = true
            };
            btnEntradaEstoque.Click += BtnEntradaEstoque_Click;
            pnlHeader.Controls.Add(btnEntradaEstoque);
        }

        private void BtnEntradaEstoque_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmEntradaMercadoria())
            {
                frm.ShowDialog();
                LoadData();
            }
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

            // Estilo do Menu Lateral de Categorias
            pnlCategorias.BackColor = UITheme.BgSidebar;
            lblCategoriasTitle.Font = UITheme.FontBodyBold;
            lblCategoriasTitle.ForeColor = UITheme.TextMuted;
            
            lstCategorias.BackColor = UITheme.BgSidebar;
            lstCategorias.ForeColor = UITheme.TextTitle;
            lstCategorias.BorderStyle = BorderStyle.None;

            UITheme.FormatSaaSButton(btnNovo, true);
            if (btnEntradaEstoque != null)
            {
                UITheme.FormatSaaSButton(btnEntradaEstoque, true);
                btnEntradaEstoque.BackColor = Color.FromArgb(79, 70, 229);
            }
            UITheme.FormatSaaSButton(btnEditar, false);
            UITheme.FormatSaaSButton(btnExcluir, false);
            btnExcluir.BackColor = UITheme.Danger;

            // Botão Nova Categoria (Menu Esquerdo)
            Button btnNovaCategoria = new Button
            {
                Text = "➕ Nova Categoria",
                Dock = DockStyle.Bottom,
                Height = 45,
                FlatStyle = FlatStyle.Flat,
                ForeColor = UITheme.TextTitle,
                BackColor = UITheme.BgCard,
                Font = UITheme.FontBodyBold,
                Cursor = Cursors.Hand
            };
            btnNovaCategoria.FlatAppearance.BorderSize = 0;
            btnNovaCategoria.Click += (s, e) =>
            {
                using (var frm = new FrmPromptCategoria())
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        categoriaRepository.Inserir(frm.ValorInput);
                        LoadCategorias();

                        // Selecionar a recém-criada
                        foreach (var item in lstCategorias.Items)
                        {
                            if (item is Categoria c && c.Nome == frm.ValorInput)
                            {
                                lstCategorias.SelectedItem = item;
                                break;
                            }
                        }
                    }
                }
            };
            pnlCategorias.Controls.Add(btnNovaCategoria);
            btnNovaCategoria.BringToFront();

            // Style DataGridView (Premium)
            UITheme.FormatGrid(dgvProdutos);
        }

        private void LoadCategorias()
        {
            try
            {
                var categorias = categoriaRepository.BuscarTodas();
                categorias.Insert(0, new Categoria { Id = 0, Nome = "Todos os Produtos" });
                
                lstCategorias.DataSource = null;
                lstCategorias.DataSource = categorias;
                lstCategorias.DisplayMember = "Nome";
                lstCategorias.ValueMember = "Id";
                lstCategorias.ItemHeight = 40;
            }
            catch { }
        }

        private void LoadData(string filtro = "")
        {
            var produtos = repository.BuscarTodos();

            // Filtrar por Categoria Selecionada (apenas se a busca estiver vazia)
            if (string.IsNullOrEmpty(filtro) && lstCategorias.SelectedValue != null && lstCategorias.SelectedValue is int catId && catId > 0)
            {
                produtos = produtos.FindAll(p => p.IdCategoria == catId);
            }

            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim().ToLower();
                var termos = filtro.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                produtos = produtos.FindAll(c => 
                    termos.All(t => c.Nome.ToLower().Contains(t)) || 
                    (c.CodigoBarras != null && c.CodigoBarras.ToLower().Contains(filtro)) ||
                    (c.CodigoInterno != null && c.CodigoInterno.ToLower().Contains(filtro))
                );
            }

            dgvProdutos.DataSource = null;
            dgvProdutos.DataSource = produtos;

            // Limit and format columns to look nice safely
            if (dgvProdutos.Columns.Count > 0)
            {
                dgvProdutos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                
                foreach(DataGridViewColumn col in dgvProdutos.Columns)
                {
                    col.Visible = false; // hide all first
                }

                if (dgvProdutos.Columns["Id"] != null)
                {
                    dgvProdutos.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    dgvProdutos.Columns["Id"].Visible = true;
                    dgvProdutos.Columns["Id"].Width = 50;
                }

                if (dgvProdutos.Columns["CodigoInterno"] != null)
                {
                    dgvProdutos.Columns["CodigoInterno"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    dgvProdutos.Columns["CodigoInterno"].Visible = true;
                    dgvProdutos.Columns["CodigoInterno"].HeaderText = "Cód.";
                    dgvProdutos.Columns["CodigoInterno"].Width = 80;
                }

                if (dgvProdutos.Columns["Nome"] != null)
                {
                    dgvProdutos.Columns["Nome"].Visible = true;
                    dgvProdutos.Columns["Nome"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                if (dgvProdutos.Columns["Modelo"] != null)
                {
                    dgvProdutos.Columns["Modelo"].Visible = true;
                    dgvProdutos.Columns["Modelo"].HeaderText = "Modelo";
                    dgvProdutos.Columns["Modelo"].Width = 150;
                }

                if (dgvProdutos.Columns["Marca"] != null)
                {
                    dgvProdutos.Columns["Marca"].Visible = true;
                    dgvProdutos.Columns["Marca"].HeaderText = "Marca / Qualidade";
                    dgvProdutos.Columns["Marca"].Width = 150;
                }

                if (dgvProdutos.Columns["PrecoVenda"] != null)
                {
                    dgvProdutos.Columns["PrecoVenda"].Visible = true;
                    dgvProdutos.Columns["PrecoVenda"].HeaderText = "Venda (R$)";
                    dgvProdutos.Columns["PrecoVenda"].DefaultCellStyle.Format = "C2";
                }

                if (dgvProdutos.Columns["Estoque"] != null)
                {
                    dgvProdutos.Columns["Estoque"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    dgvProdutos.Columns["Estoque"].Visible = true;
                    dgvProdutos.Columns["Estoque"].Width = 80;
                }

                dgvProdutos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        private void LstCategorias_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(txtBusca.Text);
        }

        private void LstCategorias_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            
            ListBox list = (ListBox)sender;
            string text = "";
            var item = list.Items[e.Index];
            if (item is Categoria cat)
            {
                text = cat.Nome;
            }
            else
            {
                text = item.ToString();
            }

            // Limpa o fundo do item com a cor do sidebar
            using (var bgBrush = new SolidBrush(UITheme.BgSidebar))
            {
                e.Graphics.FillRectangle(bgBrush, e.Bounds);
            }

            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Rectangle rect = new Rectangle(e.Bounds.X + 10, e.Bounds.Y + 4, e.Bounds.Width - 20, e.Bounds.Height - 8);

            if (isSelected)
            {
                // Desenha fundo arredondado no item selecionado usando cor Indigo
                UITheme.DrawRoundedPanel(e.Graphics, rect, UITheme.Primary, 6);
                
                // Texto em Branco
                TextRenderer.DrawText(e.Graphics, text, UITheme.FontBodyBold, rect, Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            }
            else
            {
                // Texto normal em cinza claro/título
                TextRenderer.DrawText(e.Graphics, text, UITheme.FontBody, rect, UITheme.TextTitle, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            }
        }

        private void TxtBusca_TextChanged(object sender, EventArgs e)
        {
            LoadData(txtBusca.Text);
        }

        private void BtnNovo_Click(object sender, EventArgs e)
        {
            using (var form = new FrmProdutoAddEdit())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    RefreshAndHighlightProduct(form.SavedProdutoId);
                }
            }
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (dgvProdutos.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvProdutos.SelectedRows[0].Cells["Id"].Value);
                using (var form = new FrmProdutoAddEdit(id))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        RefreshAndHighlightProduct(form.SavedProdutoId);
                    }
                }
            }
        }

        private void RefreshAndHighlightProduct(int id)
        {
            txtBusca.Text = string.Empty;
            if (lstCategorias.Items.Count > 0)
            {
                lstCategorias.SelectedIndex = 0;
            }
            
            LoadData();

            try
            {
                foreach (DataGridViewRow row in dgvProdutos.Rows)
                {
                    if (row.Cells["Id"].Value != null && Convert.ToInt32(row.Cells["Id"].Value) == id)
                    {
                        dgvProdutos.ClearSelection();
                        row.Selected = true;
                        if (row.Cells["Nome"].Visible)
                        {
                            dgvProdutos.CurrentCell = row.Cells["Nome"];
                        }
                        dgvProdutos.FirstDisplayedScrollingRowIndex = row.Index;
                        break;
                    }
                }
            }
            catch { }
        }

        private void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (dgvProdutos.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvProdutos.SelectedRows[0].Cells["Id"].Value);
                bool confirm = FrmNotification.ShowConfirm("Tem certeza que deseja desativar o produto selecionado?", "Atenção");
                if (confirm)
                {
                    try
                    {
                        repository.Excluir(id);
                        FrmNotification.ShowSuccess("Produto desativado com sucesso!", "Sucesso");
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        FrmNotification.ShowError("Erro ao desativar produto: " + ex.Message, "Erro");
                    }
                }
            }
        }
    }
}
