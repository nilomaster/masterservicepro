using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public partial class FrmPDVBuscaCliente : Form
    {
        private ClienteRepository repository = new ClienteRepository();
        private List<Cliente> todosClientes;
        public Cliente ClienteSelecionado { get; private set; }

        public FrmPDVBuscaCliente()
        {
            InitializeComponent();
            ApplyTheme();
            
            this.Load += async (s, e) => {
                todosClientes = await repository.BuscarTodosAsync();
                AtualizarGrid(todosClientes);
            };

            txtBusca.TextChanged += TxtBusca_TextChanged;
            dgvClientes.KeyDown += DgvClientes_KeyDown;
            dgvClientes.CellDoubleClick += (s, e) => SelecionarCliente();
            
            this.Shown += (s, e) => {
                txtBusca.Focus();
            };
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModalWindowStyle(this);
            this.BackColor = UITheme.BgModal;
            pnlHeader.BackColor = UITheme.BgModalHeader;
            
            lblTitle.Font = UITheme.FontTitle;
            lblTitle.ForeColor = UITheme.TextTitle;

            txtBusca.BackColor = UITheme.BgSidebar;
            txtBusca.ForeColor = UITheme.TextTitle;
            txtBusca.BorderStyle = BorderStyle.FixedSingle;

            dgvClientes.BackgroundColor = UITheme.BgModal;
            dgvClientes.BorderStyle = BorderStyle.None;
            dgvClientes.ColumnHeadersDefaultCellStyle.BackColor = UITheme.BgSidebar;
            dgvClientes.ColumnHeadersDefaultCellStyle.ForeColor = UITheme.TextTitle;
            dgvClientes.ColumnHeadersDefaultCellStyle.Font = UITheme.FontBodyBold;
            dgvClientes.EnableHeadersVisualStyles = false;
            dgvClientes.DefaultCellStyle.BackColor = UITheme.BgModal;
            dgvClientes.DefaultCellStyle.ForeColor = UITheme.TextMuted;
            dgvClientes.DefaultCellStyle.SelectionBackColor = UITheme.PrimaryHover;
        }

        private void AtualizarGrid(List<Cliente> lista)
        {
            dgvClientes.DataSource = null;
            dgvClientes.DataSource = lista;

            // Formatting
            if (dgvClientes.Columns.Count > 0)
            {
                foreach (DataGridViewColumn col in dgvClientes.Columns) col.Visible = false;

                if (dgvClientes.Columns["Nome"] != null)
                {
                    dgvClientes.Columns["Nome"].Visible = true;
                    dgvClientes.Columns["Nome"].HeaderText = "Nome do Cliente";
                    dgvClientes.Columns["Nome"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                if (dgvClientes.Columns["CpfCnpj"] != null)
                {
                    dgvClientes.Columns["CpfCnpj"].Visible = true;
                    dgvClientes.Columns["CpfCnpj"].HeaderText = "CPF/CNPJ";
                    dgvClientes.Columns["CpfCnpj"].Width = 150;
                }

                if (dgvClientes.Columns["Telefone"] != null)
                {
                    dgvClientes.Columns["Telefone"].Visible = true;
                    dgvClientes.Columns["Telefone"].HeaderText = "Telefone";
                    dgvClientes.Columns["Telefone"].Width = 150;
                }
            }
        }

        private void TxtBusca_TextChanged(object sender, EventArgs e)
        {
            string filtro = txtBusca.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(filtro) || todosClientes == null)
            {
                AtualizarGrid(todosClientes);
                return;
            }

            var termos = filtro.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var filtrados = todosClientes.Where(c => 
                termos.All(t => c.Nome.ToLower().Contains(t)) || 
                (c.CpfCnpj != null && c.CpfCnpj.Contains(filtro)) ||
                (c.Telefone != null && c.Telefone.Contains(filtro))
            ).ToList();
            
            AtualizarGrid(filtrados);
        }

        private void SelecionarCliente()
        {
            if (dgvClientes.SelectedRows.Count > 0)
            {
                ClienteSelecionado = (Cliente)dgvClientes.SelectedRows[0].DataBoundItem;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void DgvClientes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SelecionarCliente();
                e.Handled = true;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            if (keyData == Keys.Down && txtBusca.Focused)
            {
                dgvClientes.Focus();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
