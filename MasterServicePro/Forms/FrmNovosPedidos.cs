using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public partial class FrmNovosPedidos : Form
    {
        private ProdutoRepository repository = new ProdutoRepository();
        private List<ProdutoPedidoViewModel> viewModels = new List<ProdutoPedidoViewModel>();

        public FrmNovosPedidos()
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

            // Permitir edição na grid (para digitar a quantidade a pedir)
            dgvProdutos.ReadOnly = false;
        }

        private void LoadData(string filtro = "")
        {
            try
            {
                var produtos = repository.BuscarTodos();

                // Filtrar apenas produtos com estoque <= 1 ou <= estoque mínimo
                var produtosFiltrados = produtos.FindAll(p => p.Estoque <= 1 || p.Estoque <= p.EstoqueMinimo);

                // Mapear para view models
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

                ExibirLista(filtro);
            }
            catch (Exception ex)
            {
                FrmNotification.ShowError("Erro ao carregar lista de reposição: " + ex.Message, "Erro");
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
                    (c.CodigoInterno != null && c.CodigoInterno.ToLower().Contains(filtro))
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
                    col.ReadOnly = true; // somente leitura por padrão
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
                    dgvProdutos.Columns["Estoque"].HeaderText = "Estoque Atual";
                    dgvProdutos.Columns["Estoque"].Width = 90;
                }

                if (dgvProdutos.Columns["EstoqueMinimo"] != null)
                {
                    dgvProdutos.Columns["EstoqueMinimo"].Visible = true;
                    dgvProdutos.Columns["EstoqueMinimo"].HeaderText = "Mínimo";
                    dgvProdutos.Columns["EstoqueMinimo"].Width = 80;
                }

                if (dgvProdutos.Columns["QtdPedir"] != null)
                {
                    dgvProdutos.Columns["QtdPedir"].Visible = true;
                    dgvProdutos.Columns["QtdPedir"].HeaderText = "Qtd. Pedir";
                    dgvProdutos.Columns["QtdPedir"].Width = 100;
                    dgvProdutos.Columns["QtdPedir"].ReadOnly = false; // Permitir edição apenas desta coluna
                    dgvProdutos.Columns["QtdPedir"].DefaultCellStyle.BackColor = Color.FromArgb(47, 54, 76);
                    dgvProdutos.Columns["QtdPedir"].DefaultCellStyle.ForeColor = Color.White;
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
            // Forçar commit de edição ativa na grid para obter valores atualizados
            dgvProdutos.EndEdit();

            if (viewModels.Count == 0)
            {
                FrmNotification.ShowError("Não há produtos listados para reposição.", "Aviso");
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("*📋 LISTA DE PEDIDOS - REPOSIÇÃO DE ESTOQUE*");
            sb.AppendLine($"Gerada em: {DateTime.Now:dd/MM/yyyy HH:mm}");
            sb.AppendLine("--------------------------------------------------");

            int count = 0;
            foreach (var vm in viewModels)
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
                FrmNotification.ShowError("Nenhum produto possui quantidade para pedir maior que zero.", "Aviso");
                return;
            }

            try
            {
                Clipboard.SetText(sb.ToString());
                FrmNotification.ShowSuccess("Lista de pedidos copiada com sucesso para a área de transferência! Pronta para colar no WhatsApp.", "Sucesso");
            }
            catch (Exception ex)
            {
                FrmNotification.ShowError("Erro ao copiar para área de transferência: " + ex.Message, "Erro");
            }
        }
    }

    public class ProdutoPedidoViewModel
    {
        public int Id { get; set; }
        public string CodigoInterno { get; set; }
        public string Nome { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int Estoque { get; set; }
        public int EstoqueMinimo { get; set; }
        public int QtdPedir { get; set; }
    }
}
