using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public partial class FrmCorrecoesAdmin : Form
    {
        private VendaRepository vendaRepo = new VendaRepository();
        private OrdemServicoRepository osRepo = new OrdemServicoRepository();

        public FrmCorrecoesAdmin()
        {
            InitializeComponent();
            this.Load += FrmCorrecoesAdmin_Load;
        }

        private void FrmCorrecoesAdmin_Load(object sender, EventArgs e)
        {
            CarregarVendas();
            CarregarOS();
            cbNovaForma.Items.AddRange(new string[] { "dinheiro", "pix", "credito", "debito", "misto" });
            cbNovaForma.SelectedIndex = 0;
        }

        private void CarregarVendas()
        {
            try
            {
                var dt = vendaRepo.GetVendasRelatorio(DateTime.Today.AddDays(-30), DateTime.Today);
                dgvVendas.DataSource = dt;
                dgvVendas.Columns["Id"].Width = 50;
                dgvVendas.Columns["DataVenda"].DefaultCellStyle.Format = "dd/MM/yy HH:mm";
                dgvVendas.Columns["TotalFinal"].DefaultCellStyle.Format = "C2";
            }
            catch { }
        }

        private void CarregarOS()
        {
            try
            {
                var dt = osRepo.BuscarTudoResumido();
                var view = new DataView(dt);
                view.RowFilter = "Status = 'Finalizado' OR Status = 'Entregue'";
                dgvOS.DataSource = view;
                dgvOS.Columns["Id"].Width = 50;
                dgvOS.Columns["ValorTotal"].DefaultCellStyle.Format = "C2";
            }
            catch { }
        }

        private void btnCorrigirVenda_Click(object sender, EventArgs e)
        {
            if (dgvVendas.CurrentRow == null) return;
            int id = Convert.ToInt32(dgvVendas.CurrentRow.Cells["Id"].Value);
            string forma = cbNovaForma.Text;

            if (MessageBox.Show($"Deseja alterar a forma de pagamento da Venda #{id} para {forma}?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    vendaRepo.AtualizarFormaPagamentoVenda(id, forma, AuthSession.Id);
                    FrmNotification.ShowSuccess("Venda corrigida com sucesso!");
                    CarregarVendas();
                }
                catch (Exception ex)
                {
                    FrmNotification.ShowError("Erro: " + ex.Message);
                }
            }
        }

        private void btnCorrigirOS_Click(object sender, EventArgs e)
        {
            if (dgvOS.CurrentRow == null) return;
            int id = Convert.ToInt32(dgvOS.CurrentRow.Cells["Id"].Value);
            string forma = cbNovaForma.Text;

            if (MessageBox.Show($"Deseja alterar a forma de pagamento da OS #{id} para {forma}?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    osRepo.AtualizarFormaPagamentoOS(id, forma, AuthSession.Id);
                    FrmNotification.ShowSuccess("OS corrigida com sucesso!");
                    CarregarOS();
                }
                catch (Exception ex)
                {
                    FrmNotification.ShowError("Erro: " + ex.Message);
                }
            }
        }
    }
}
