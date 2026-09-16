using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public class FrmEntradaMercadoria : Form
    {
        private readonly EntradaMercadoriaRepository _repository = new EntradaMercadoriaRepository();

        // UI Controls
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private DataGridView dgvEntradas;
        private Button btnNovaEntrada;
        private Button btnFechar;

        public FrmEntradaMercadoria()
        {
            InitializeComponent();
            ApplyTheme();
            LoadData();
            BindEvents();
        }

        private void InitializeComponent()
        {
            this.Text = "Entradas de Peças / Mercadorias (Estoque)";
            this.ClientSize = new Size(880, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Header Panel
            pnlHeader = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(880, 70),
                BackColor = UITheme.BgCard
            };

            lblTitle = new Label
            {
                Text = "Entradas de Mercadorias",
                Location = new Point(25, 12),
                Size = new Size(400, 28),
                Font = new Font("Segoe UI", 16F, FontStyle.Bold)
            };

            lblSubtitle = new Label
            {
                Text = "Gerencie as entradas de estoque e acompanhe o recálculo automático de custo médio.",
                Location = new Point(25, 42),
                Size = new Size(700, 18),
                Font = new Font("Segoe UI", 9F)
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);

            // DataGridView
            dgvEntradas = new DataGridView
            {
                Location = new Point(25, 95),
                Size = new Size(825, 395),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            // Buttons
            btnNovaEntrada = new Button
            {
                Text = "📥 Registrar Nova Entrada",
                Location = new Point(25, 510),
                Size = new Size(240, 40)
            };

            btnFechar = new Button
            {
                Text = "Fechar",
                Location = new Point(650, 510),
                Size = new Size(200, 40)
            };

            this.Controls.Add(pnlHeader);
            this.Controls.Add(dgvEntradas);
            this.Controls.Add(btnNovaEntrada);
            this.Controls.Add(btnFechar);
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModalWindowStyle(this);
            this.BackColor = UITheme.BgModal;

            pnlHeader.BackColor = UITheme.BgCard;
            lblTitle.ForeColor = UITheme.TextTitle;
            lblSubtitle.ForeColor = UITheme.TextMuted;

            UITheme.FormatGrid(dgvEntradas);

            UITheme.FormatSaaSButton(btnNovaEntrada, true);
            UITheme.FormatSaaSButton(btnFechar, false);
        }

        private void LoadData()
        {
            try
            {
                DataTable dt = _repository.ListarTodas();
                dgvEntradas.DataSource = null;
                dgvEntradas.DataSource = dt;

                // Format Grid Columns
                if (dgvEntradas.Columns.Count > 0)
                {
                    if (dgvEntradas.Columns["Id"] != null) dgvEntradas.Columns["Id"].HeaderText = "ID";
                    if (dgvEntradas.Columns["DataEntrada"] != null)
                    {
                        dgvEntradas.Columns["DataEntrada"].HeaderText = "Data/Hora";
                        dgvEntradas.Columns["DataEntrada"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                    }
                    if (dgvEntradas.Columns["Fornecedor"] != null) dgvEntradas.Columns["Fornecedor"].HeaderText = "Fornecedor";
                    if (dgvEntradas.Columns["NumeroNota"] != null) dgvEntradas.Columns["NumeroNota"].HeaderText = "Nota Fiscal";
                    if (dgvEntradas.Columns["ValorTotal"] != null)
                    {
                        dgvEntradas.Columns["ValorTotal"].HeaderText = "Valor Total";
                        dgvEntradas.Columns["ValorTotal"].DefaultCellStyle.Format = "C2";
                    }
                    if (dgvEntradas.Columns["Observacao"] != null) dgvEntradas.Columns["Observacao"].HeaderText = "Obs";

                    foreach (DataGridViewColumn col in dgvEntradas.Columns)
                    {
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar entradas: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindEvents()
        {
            btnFechar.Click += (s, e) => this.Close();
            btnNovaEntrada.Click += BtnNovaEntrada_Click;
        }

        private void BtnNovaEntrada_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmEntradaMercadoriaAdd())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }
    }
}
