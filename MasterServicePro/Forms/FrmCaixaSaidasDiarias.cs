using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public class FrmCaixaSaidasDiarias : Form
    {
        private FinanceiroRepository repository = new FinanceiroRepository();

        private DataGridView dgvHistorico;

        public FrmCaixaSaidasDiarias()
        {
            InitializeComponent();
            ApplyTheme();
            CreateCustomTitleBar();
            LoadData();
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void CreateCustomTitleBar()
        {
            Panel pnlTitleBar = new Panel();
            Label lblTitleBar = new Label();
            Button btnClose = new Button();
            PictureBox picLogo = new PictureBox();

            pnlTitleBar.BackColor = System.Drawing.Color.FromArgb(26, 29, 36);
            pnlTitleBar.Dock = DockStyle.Top;
            pnlTitleBar.Height = 40;
            pnlTitleBar.Controls.Add(lblTitleBar);
            pnlTitleBar.Controls.Add(btnClose);
            pnlTitleBar.Controls.Add(picLogo);
            pnlTitleBar.MouseDown += TitleBar_MouseDown;

            btnClose.Dock = DockStyle.Right;
            btnClose.Width = 45;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.ForeColor = System.Drawing.Color.White;
            btnClose.Text = "X";
            btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => this.Close();
            btnClose.MouseEnter += (s, e) => btnClose.BackColor = System.Drawing.Color.FromArgb(232, 17, 35);
            btnClose.MouseLeave += (s, e) => btnClose.BackColor = System.Drawing.Color.Transparent;

            picLogo.Dock = DockStyle.Left;
            picLogo.Width = 40;
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.Padding = new Padding(8);
            try
            {
                var appIcon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                this.Icon = appIcon;
                picLogo.Image = appIcon.ToBitmap();
            }
            catch { }
            picLogo.MouseDown += TitleBar_MouseDown;

            lblTitleBar.Dock = DockStyle.Fill;
            lblTitleBar.ForeColor = System.Drawing.Color.White;
            lblTitleBar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            lblTitleBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lblTitleBar.Text = " Histórico de Movimentações (Hoje)";
            lblTitleBar.MouseDown += TitleBar_MouseDown;

            this.Controls.Add(pnlTitleBar);
        }

        private void InitializeComponent()
        {
            this.Size = new Size(800, 540);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(1);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Histórico de Movimentações (Hoje)";

            dgvHistorico = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BorderStyle = BorderStyle.None
            };
            dgvHistorico.CellFormatting += DgvHistorico_CellFormatting;

            this.Controls.Add(dgvHistorico);
        }

        private void ApplyTheme()
        {
            this.BackColor = UITheme.BgApp;
            this.Paint += (sender, e) =>
            {
                using (Pen pen = new Pen(UITheme.Primary, 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
                }
            };
            
            UITheme.FormatGrid(dgvHistorico);
        }

        private void LoadData()
        {
            try
            {
                dgvHistorico.DataSource = repository.GetMovimentacoesDiarias(DateTime.Today);
                if (dgvHistorico.Columns.Count > 0)
                {
                    if (dgvHistorico.Columns["Id"] != null) dgvHistorico.Columns["Id"].Visible = false;
                    
                    if (dgvHistorico.Columns["Tipo"] != null) dgvHistorico.Columns["Tipo"].FillWeight = 80;
                    if (dgvHistorico.Columns["Categoria"] != null) dgvHistorico.Columns["Categoria"].FillWeight = 100;
                    if (dgvHistorico.Columns["Subcategoria"] != null) dgvHistorico.Columns["Subcategoria"].FillWeight = 120;
                    if (dgvHistorico.Columns["FormaPagamento"] != null) dgvHistorico.Columns["FormaPagamento"].HeaderText = "Forma Pgto";
                    if (dgvHistorico.Columns["Descricao"] != null) dgvHistorico.Columns["Descricao"].HeaderText = "Motivo/Descrição";
                    
                    if (dgvHistorico.Columns["Valor"] != null)
                    {
                        dgvHistorico.Columns["Valor"].DefaultCellStyle.Format = "C2";
                        dgvHistorico.Columns["Valor"].FillWeight = 80;
                    }
                    if (dgvHistorico.Columns["Data"] != null)
                    {
                        dgvHistorico.Columns["Data"].DefaultCellStyle.Format = "HH:mm";
                        dgvHistorico.Columns["Data"].HeaderText = "Hora";
                        dgvHistorico.Columns["Data"].FillWeight = 60;
                    }
                }
            }
            catch { }
        }

        private void DgvHistorico_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvHistorico.Columns[e.ColumnIndex].Name == "Valor")
            {
                var tipo = dgvHistorico.Rows[e.RowIndex].Cells["Tipo"].Value?.ToString();
                if (tipo == "Entrada")
                {
                    e.CellStyle.ForeColor = UITheme.Success;
                }
                else if (tipo == "Saída")
                {
                    e.CellStyle.ForeColor = UITheme.Danger;
                }
            }
        }
    }
}
