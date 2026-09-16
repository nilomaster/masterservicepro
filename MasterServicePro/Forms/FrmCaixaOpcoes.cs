using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public class FrmCaixaOpcoes : Form
    {
        private Panel pnlHeader;
        private Label lblTitle;
        private Button btnClose;
        private FlowLayoutPanel pnlButtons;

        private Button btnAbrirCaixa;
        private Button btnHistorico;
        private Button btnAddDinheiro;
        private Button btnRetirarDinheiro;
        private Button btnContasReceber;

        // Variáveis para permitir arrastar a janela
        private bool isDragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        public FrmCaixaOpcoes()
        {
            InitializeComponent();
            ApplyTheme();
            BindEvents();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(400, 550);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = UITheme.BgModal;

            // Header (Barra de Título customizada)
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = UITheme.BgModalHeader
            };

            lblTitle = new Label
            {
                Text = "💵 Controle de Caixa",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = UITheme.TextTitle,
                Location = new Point(20, 15),
                AutoSize = true
            };

            btnClose = new Button
            {
                Text = "X",
                Size = new Size(40, 40),
                Location = new Point(this.Width - 50, 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = UITheme.TextMuted,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold)
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 255, 0, 0);

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(btnClose);

            // Container dos Botões
            pnlButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(40, 30, 40, 30),
                BackColor = UITheme.BgModal
            };

            // Inicializando Botões
            btnAbrirCaixa = CreateMenuButton("🟢 Abrir Caixa");
            btnHistorico = CreateMenuButton("📜 Histórico Diário (E/S)");
            btnAddDinheiro = CreateMenuButton("➕ Suprimento (Adicionar Dinheiro)");
            btnRetirarDinheiro = CreateMenuButton("➖ Sangria (Retirar Dinheiro)");
            btnContasReceber = CreateMenuButton("🧾 Contas a Receber");

            pnlButtons.Controls.Add(btnAbrirCaixa);
            pnlButtons.Controls.Add(btnHistorico);
            pnlButtons.Controls.Add(btnAddDinheiro);
            pnlButtons.Controls.Add(btnRetirarDinheiro);
            pnlButtons.Controls.Add(btnContasReceber);

            this.Controls.Add(pnlButtons);
            this.Controls.Add(pnlHeader);
        }

        private Button CreateMenuButton(string text)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(320, 60),
                Margin = new Padding(0, 0, 0, 15),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            UITheme.FormatSaaSButton(btn); // Aplica nosso estilo padrão
            btn.BackColor = UITheme.BgCard; // Sobrescreve para usar o fundo Card para dar relevo no fundo da Modal
            btn.ForeColor = UITheme.TextTitle;
            
            // Efeito Hover customizado para botões de menu
            btn.MouseEnter += (s, e) => { btn.BackColor = UITheme.Primary; btn.ForeColor = Color.White; };
            btn.MouseLeave += (s, e) => { btn.BackColor = UITheme.BgCard; btn.ForeColor = UITheme.TextTitle; };

            return btn;
        }

        private void ApplyTheme()
        {
            // Borda customizada da janela modal
            this.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(UITheme.ElementBorder, 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
                }
            };
        }

        private void BindEvents()
        {
            btnClose.Click += (s, e) => this.Close();

            // Drag Window logic
            pnlHeader.MouseDown += (s, e) =>
            {
                isDragging = true;
                dragCursorPoint = Cursor.Position;
                dragFormPoint = this.Location;
            };
            pnlHeader.MouseMove += (s, e) =>
            {
                if (isDragging)
                {
                    Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                    this.Location = Point.Add(dragFormPoint, new Size(dif));
                }
            };
            pnlHeader.MouseUp += (s, e) => isDragging = false;
            
            // Clicks reais para as janelas do sistema
            btnAbrirCaixa.Click += (s, e) =>
            {
                var repo = new MasterServicePro.DAL.FinanceiroRepository();
                if (repo.ObterCaixaAberto() != null)
                {
                    MessageBox.Show("Atenção: Já existe um caixa aberto no momento!\n\nUm administrador precisa fechar o caixa atual antes que um novo possa ser aberto.", "Caixa Aberto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                using (var frm = new FrmCaixaAbertura()) { frm.ShowDialog(); }
            };
            
            btnHistorico.Click += (s, e) =>
            {
                using (var frm = new FrmCaixaSaidasDiarias()) { frm.ShowDialog(); }
            };
            
            btnAddDinheiro.Click += (s, e) =>
            {
                var repo = new MasterServicePro.DAL.FinanceiroRepository();
                var caixa = repo.ObterCaixaAberto();
                if (caixa == null)
                {
                    MessageBox.Show("O caixa precisa estar aberto para registrar movimentações.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var frm = new FrmCaixaMovimentacao("Suprimento"))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        var mov = frm.Movimentacao;
                        mov.IdCaixa = caixa.Id;
                        repo.LançarMovimentacao(mov);
                        MessageBox.Show("Movimentação registrada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            };
            
            btnRetirarDinheiro.Click += (s, e) =>
            {
                var repo = new MasterServicePro.DAL.FinanceiroRepository();
                var caixa = repo.ObterCaixaAberto();
                if (caixa == null)
                {
                    MessageBox.Show("O caixa precisa estar aberto para registrar movimentações.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var frm = new FrmCaixaMovimentacao("Sangria"))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        var mov = frm.Movimentacao;
                        mov.IdCaixa = caixa.Id;
                        repo.LançarMovimentacao(mov);
                        MessageBox.Show("Movimentação registrada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            };
            
            btnContasReceber.Click += (s, e) =>
            {
                using (var frm = new FrmContasReceberWeb()) { frm.ShowDialog(); }
            };
        }
    }
}
