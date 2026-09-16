using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public partial class FrmHome : Form
    {
        private ProdutoRepository produtoRepo = new ProdutoRepository();
        private FinanceiroRepository financeiroRepo = new FinanceiroRepository();

        public FrmHome()
        {
            InitializeComponent();
            ApplyTheme();
            LoadDashboardData();
        }

        private void ApplyTheme()
        {
            this.BackColor = UITheme.BgApp;
            
            pnlAlerts.BackColor = UITheme.BgCard;
            lblAlertTitle.Font = UITheme.FontSubtitle;
            lblAlertTitle.ForeColor = UITheme.Danger;

            lblStatsTitle.Font = UITheme.FontSubtitle;
            lblStatsTitle.ForeColor = UITheme.TextTitle;

            dgvAlertas.BackgroundColor = UITheme.BgCard;
            dgvAlertas.BorderStyle = BorderStyle.None;
            dgvAlertas.ColumnHeadersDefaultCellStyle.BackColor = UITheme.BgSidebar;
            dgvAlertas.ColumnHeadersDefaultCellStyle.ForeColor = UITheme.TextTitle;
            dgvAlertas.EnableHeadersVisualStyles = false;
        }

        private void LoadDashboardData()
        {
            try
            {
                // Carrega alertas de estoque
                DataTable dtAlertas = produtoRepo.BuscarAlertasEstoque();
                dgvAlertas.DataSource = dtAlertas;

                if (dgvAlertas.Columns.Count > 0)
                {
                    // Verifica se a coluna existe antes de formatar para evitar erro
                    if (dgvAlertas.Columns["Id"] != null) dgvAlertas.Columns["Id"].Visible = false;
                    if (dgvAlertas.Columns["Nome"] != null) dgvAlertas.Columns["Nome"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    if (dgvAlertas.Columns["Estoque"] != null) dgvAlertas.Columns["Estoque"].HeaderText = "Atual";
                    if (dgvAlertas.Columns["EstoqueMinimo"] != null) dgvAlertas.Columns["EstoqueMinimo"].HeaderText = "Mínimo";
                }

                // Totais rápidos
                decimal vendasHoje = financeiroRepo.GetTotalPorPeriodo(DateTime.Now, DateTime.Now, "Entrada");
                lblVendasHoje.Text = vendasHoje.ToString("C2");
            }
            catch (Exception ex)
            {
                // Se der erro, apenas ignora para o programa não fechar
                Console.WriteLine("Erro ao carregar Dashboard Home: " + ex.Message);
            }
        }
    }
}
