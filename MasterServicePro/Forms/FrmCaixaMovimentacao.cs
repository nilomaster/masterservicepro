using System;
using System.Drawing;
using System.Windows.Forms;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public class FrmCaixaMovimentacao : Form
    {
        public Movimentacao Movimentacao { get; private set; }
        private string tipoMovimentacao; // "Suprimento" ou "Sangria"
        
        private TextBox txtValor;
        private TextBox txtDescricao;
        private ComboBox cmbFormaPagamento;
        private ComboBox cmbSubcategoria;
        private Button btnConfirmar;
        private Button btnCancelar;

        public FrmCaixaMovimentacao(string tipo)
        {
            this.tipoMovimentacao = tipo;
            InitializeComponent();
            ApplyTheme();
            CreateCustomTitleBar();
        }

        // Win32 API para arrastar a janela
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
            string friendlyName = tipoMovimentacao == "Suprimento" ? "Adicionar Dinheiro" : "Retirar Dinheiro";
            lblTitleBar.Text = " Registrar " + friendlyName;
            lblTitleBar.MouseDown += TitleBar_MouseDown;

            this.Controls.Add(pnlTitleBar);
        }

        private void InitializeComponent()
        {
            this.Size = new Size(400, 450);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(1);
            this.StartPosition = FormStartPosition.CenterParent;
            
            Label lblValor = new Label { Text = "Valor (R$):", Location = new Point(30, 60), AutoSize = true };
            txtValor = new TextBox { Location = new Point(30, 85), Size = new Size(340, 36), Text = "0,00" };

            Label lblForma = new Label { Text = "Forma de Pagamento:", Location = new Point(30, 135), AutoSize = true };
            cmbFormaPagamento = new ComboBox { Location = new Point(30, 160), Size = new Size(340, 30), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbFormaPagamento.Items.AddRange(new string[] { "Dinheiro", "Cartão de Crédito", "Cartão de Débito", "PIX" });
            cmbFormaPagamento.SelectedIndex = 0;

            Label lblSubcat = new Label { Text = "Subcategoria:", Location = new Point(30, 200), AutoSize = true };
            cmbSubcategoria = new ComboBox { Location = new Point(30, 225), Size = new Size(340, 30), DropDownStyle = ComboBoxStyle.DropDownList };
            if (tipoMovimentacao == "Suprimento")
            {
                cmbSubcategoria.Items.AddRange(new string[] { "Aporte de Caixa", "Troco Inicial", "Venda Direta", "Outros" });
            }
            else
            {
                cmbSubcategoria.Items.AddRange(new string[] { 
                    "Despesa com Peças", 
                    "Aluguel / Condomínio", 
                    "Pró-labore / Salários", 
                    "Água / Luz / Internet", 
                    "Material de Escritório", 
                    "Ferramentas / Equipamentos", 
                    "Estorno/Devolução", 
                    "Outros" 
                });
            }
            cmbSubcategoria.SelectedIndex = 0;

            Label lblDesc = new Label { Text = "Motivo / Observação:", Location = new Point(30, 265), AutoSize = true };
            txtDescricao = new TextBox { Location = new Point(30, 290), Size = new Size(340, 30) };

            btnConfirmar = new Button { Text = "CONFIRMAR", Location = new Point(220, 340), Size = new Size(150, 45) };
            btnCancelar = new Button { Text = "CANCELAR", Location = new Point(60, 340), Size = new Size(150, 45) };

            btnConfirmar.Click += BtnConfirmar_Click;
            btnCancelar.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            
            this.Controls.Add(lblValor);
            this.Controls.Add(txtValor);
            this.Controls.Add(lblForma);
            this.Controls.Add(cmbFormaPagamento);
            this.Controls.Add(lblSubcat);
            this.Controls.Add(cmbSubcategoria);
            this.Controls.Add(lblDesc);
            this.Controls.Add(txtDescricao);
            this.Controls.Add(btnConfirmar);
            this.Controls.Add(btnCancelar);
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDescricao.Text))
            {
                MessageBox.Show("Informe o motivo/observação da movimentação.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (decimal.TryParse(txtValor.Text.Replace("R$", "").Trim(), out decimal val) && val > 0)
            {
                Movimentacao = new Movimentacao
                {
                    Tipo = tipoMovimentacao == "Suprimento" ? "Entrada" : "Saída",
                    Categoria = tipoMovimentacao,
                    Subcategoria = cmbSubcategoria.Text,
                    Valor = val,
                    FormaPagamento = cmbFormaPagamento.Text,
                    Descricao = txtDescricao.Text
                };
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Valor inválido! Digite um valor maior que zero.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            
            foreach (Control c in this.Controls)
            {
                if (c is Label lbl && lbl.Name != "lblHeaderTitle")
                {
                    lbl.ForeColor = UITheme.TextMuted;
                    lbl.Font = UITheme.FontBodyBold;
                }
            }

            txtValor.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            txtValor.BackColor = UITheme.BgSidebar;
            txtValor.ForeColor = UITheme.Primary;
            txtValor.BorderStyle = BorderStyle.FixedSingle;
            txtValor.TextAlign = HorizontalAlignment.Center;

            txtDescricao.Font = UITheme.FontBody;
            txtDescricao.BackColor = UITheme.BgSidebar;
            txtDescricao.ForeColor = UITheme.TextTitle;
            txtDescricao.BorderStyle = BorderStyle.FixedSingle;

            cmbFormaPagamento.Font = UITheme.FontBody;
            cmbFormaPagamento.BackColor = UITheme.BgSidebar;
            cmbFormaPagamento.ForeColor = UITheme.TextTitle;
            cmbFormaPagamento.FlatStyle = FlatStyle.Flat;

            cmbSubcategoria.Font = UITheme.FontBody;
            cmbSubcategoria.BackColor = UITheme.BgSidebar;
            cmbSubcategoria.ForeColor = UITheme.TextTitle;
            cmbSubcategoria.FlatStyle = FlatStyle.Flat;

            UITheme.FormatSaaSButton(btnConfirmar, true);
            UITheme.FormatSaaSButton(btnCancelar, false);
            btnConfirmar.BackColor = tipoMovimentacao == "Suprimento" ? UITheme.Success : UITheme.Warning;
        }
    }
}
