using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public partial class FrmClientes : Form
    {
        private ClienteRepository repository = new ClienteRepository();

        public FrmClientes()
        {
            InitializeComponent();
            ApplyTheme();
            
            this.Load += async (s, e) => await LoadDataAsync();
            
            // Events
            btnNovo.Click += async (s, e) => await AbrirCadastroAsync();
            btnEditar.Click += async (s, e) => await AbrirCadastroAsync(GetClienteSelecionado());
            btnExcluir.Click += async (s, e) => await ExcluirClienteAsync();
            btnHistorico.Click += (s, e) => VerHistorico();
            txtBusca.TextChanged += async (s, e) => await LoadDataAsync(txtBusca.Text);
        }

        private void VerHistorico()
        {
            var cliente = GetClienteSelecionado();
            if (cliente != null)
            {
                using (var frm = new FrmClienteHistorico(cliente))
                {
                    frm.ShowDialog();
                }
            }
        }

        private Cliente GetClienteSelecionado()
        {
            if (dgvClientes.SelectedRows.Count > 0)
            {
                return (Cliente)dgvClientes.SelectedRows[0].DataBoundItem;
            }
            return null;
        }

        private async System.Threading.Tasks.Task AbrirCadastroAsync(Cliente cliente = null)
        {
            using (var form = cliente == null ? new FrmClienteAddEdit() : new FrmClienteAddEdit(cliente.Id))
            {
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK) await LoadDataAsync();
            }
        }

        private async System.Threading.Tasks.Task ExcluirClienteAsync()
        {
            var cliente = GetClienteSelecionado();
            if (cliente != null)
            {
                if (MessageBox.Show("Tem certeza que deseja excluir o cliente selecionado?", "Atencão", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    repository.Excluir(cliente.Id);
                    await LoadDataAsync();
                }
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

            UITheme.FormatSaaSButton(btnNovo, true);
            UITheme.FormatSaaSButton(btnEditar, false);
            UITheme.FormatSaaSButton(btnExcluir, false);
            UITheme.FormatSaaSButton(btnHistorico, false);
            btnExcluir.BackColor = UITheme.Danger;

            // Style DataGridView (Premium)
            UITheme.FormatGrid(dgvClientes);
        }

        private async System.Threading.Tasks.Task LoadDataAsync(string filtro = "")
        {
            var clientes = await repository.BuscarTodosAsync();

            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim().ToLower();
                var termos = filtro.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                clientes = clientes.FindAll(c => 
                    termos.All(t => c.Nome.ToLower().Contains(t)) || 
                    (c.CpfCnpj != null && c.CpfCnpj.Contains(filtro)) ||
                    (c.Telefone != null && c.Telefone.Contains(filtro))
                );
            }

            dgvClientes.DataSource = null;
            dgvClientes.DataSource = clientes;

            // Format columns safely
            if (dgvClientes.Columns.Count > 0)
            {
                dgvClientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                if (dgvClientes.Columns["Id"] != null) 
                {
                    dgvClientes.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    dgvClientes.Columns["Id"].Width = 50;
                }
                if (dgvClientes.Columns["Nome"] != null) dgvClientes.Columns["Nome"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                if (dgvClientes.Columns["CpfCnpj"] != null) dgvClientes.Columns["CpfCnpj"].HeaderText = "CPF/CNPJ";
                if (dgvClientes.Columns["Historico"] != null) dgvClientes.Columns["Historico"].Visible = false;
                if (dgvClientes.Columns["Ativo"] != null) dgvClientes.Columns["Ativo"].Visible = false;
                if (dgvClientes.Columns["DataAtualizacao"] != null) dgvClientes.Columns["DataAtualizacao"].Visible = false;
                if (dgvClientes.Columns["Saldo"] != null) 
                {
                    dgvClientes.Columns["Saldo"].HeaderText = "Saldo (R$)";
                    dgvClientes.Columns["Saldo"].DefaultCellStyle.Format = "N2";
                    dgvClientes.Columns["Saldo"].DefaultCellStyle.ForeColor = UITheme.Success;
                }

                dgvClientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        private async void TxtBusca_TextChanged(object sender, EventArgs e)
        {
            await LoadDataAsync(txtBusca.Text);
        }

        private async void BtnNovo_Click(object sender, EventArgs e)
        {
            using (var form = new FrmClienteAddEdit())
            {
                if (form.ShowDialog() == DialogResult.OK) await LoadDataAsync();
            }
        }

        private async void BtnEditar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvClientes.SelectedRows[0].Cells["Id"].Value);
                using (var form = new FrmClienteAddEdit(id))
                {
                    if (form.ShowDialog() == DialogResult.OK) await LoadDataAsync();
                }
            }
        }

        private async void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvClientes.SelectedRows[0].Cells["Id"].Value);
                if (MessageBox.Show("Tem certeza que deseja excluir o cliente selecionado?", "Atencão", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    repository.Excluir(id);
                    await LoadDataAsync();
                }
            }
        }
    }
}
