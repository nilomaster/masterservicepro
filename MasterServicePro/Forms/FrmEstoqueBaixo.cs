using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public partial class FrmEstoqueBaixo : Form
    {
        private ProdutoRepository repository = new ProdutoRepository();

        public FrmEstoqueBaixo()
        {
            InitializeComponent();
            ApplyTheme();
            LoadData();

            // Events
            btnEditar.Click += BtnEditar_Click;
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

            UITheme.FormatSaaSButton(btnEditar, true);
            UITheme.FormatSaaSButton(btnAtualizar, false);

            // Style DataGridView (Premium)
            UITheme.FormatGrid(dgvProdutos);
        }

        private void LoadData(string filtro = "")
        {
            try
            {
                var produtos = repository.BuscarTodos();

                // Filtrar apenas produtos com estoque igual ou abaixo do mínimo
                produtos = produtos.FindAll(p => p.Estoque <= p.EstoqueMinimo || p.Estoque <= 0);

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

                // Format columns
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

                    if (dgvProdutos.Columns["Marca"] != null)
                    {
                        dgvProdutos.Columns["Marca"].Visible = true;
                        dgvProdutos.Columns["Marca"].HeaderText = "Marca";
                        dgvProdutos.Columns["Marca"].Width = 120;
                    }

                    if (dgvProdutos.Columns["Modelo"] != null)
                    {
                        dgvProdutos.Columns["Modelo"].Visible = true;
                        dgvProdutos.Columns["Modelo"].HeaderText = "Modelo";
                        dgvProdutos.Columns["Modelo"].Width = 120;
                    }

                    if (dgvProdutos.Columns["Estoque"] != null)
                    {
                        dgvProdutos.Columns["Estoque"].Visible = true;
                        dgvProdutos.Columns["Estoque"].HeaderText = "Qtd. Atual";
                        dgvProdutos.Columns["Estoque"].Width = 90;
                        dgvProdutos.Columns["Estoque"].DefaultCellStyle.ForeColor = UITheme.Danger;
                    }

                    if (dgvProdutos.Columns["EstoqueMinimo"] != null)
                    {
                        dgvProdutos.Columns["EstoqueMinimo"].Visible = true;
                        dgvProdutos.Columns["EstoqueMinimo"].HeaderText = "Qtd. Mínima";
                        dgvProdutos.Columns["EstoqueMinimo"].Width = 90;
                    }

                    if (dgvProdutos.Columns["PrecoVenda"] != null)
                    {
                        dgvProdutos.Columns["PrecoVenda"].Visible = true;
                        dgvProdutos.Columns["PrecoVenda"].HeaderText = "Preço Venda";
                        dgvProdutos.Columns["PrecoVenda"].DefaultCellStyle.Format = "C2";
                        dgvProdutos.Columns["PrecoVenda"].Width = 110;
                    }

                    dgvProdutos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                FrmNotification.ShowError("Erro ao carregar lista de alertas de estoque: " + ex.Message, "Erro");
            }
        }

        private void TxtBusca_TextChanged(object sender, EventArgs e)
        {
            LoadData(txtBusca.Text);
        }

        private void BtnAtualizar_Click(object sender, EventArgs e)
        {
            LoadData(txtBusca.Text);
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
                        LoadData(txtBusca.Text);
                    }
                }
            }
            else
            {
                FrmNotification.ShowError("Selecione um produto para editar/abastecer.", "Aviso");
            }
        }
    }
}
