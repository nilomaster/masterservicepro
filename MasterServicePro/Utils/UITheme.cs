using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MasterServicePro.Utils
{
    public static class UITheme
    {
        // Paleta Dark Mode (SaaS)
        public static Color BgApp = Color.FromArgb(15, 23, 42);       // Fundo principal do app (slate-900)
        public static Color BgSidebar = Color.FromArgb(30, 41, 59);   // Menu lateral (slate-800)
        public static Color BgCard = Color.FromArgb(30, 41, 59);      // Fundo dos cards e forms (slate-800)
        public static Color BgInput = Color.FromArgb(15, 23, 42);     // Fundo para inputs (slate-900) para dar contraste com o Card
        public static Color Primary = Color.FromArgb(99, 102, 241);   // Indigo-500 (Botões primários)
        public static Color PrimaryHover = Color.FromArgb(79, 70, 229);// Indigo-600
        public static Color TextTitle = Color.FromArgb(248, 250, 252); // Slate-50 (títulos)
        public static Color TextMuted = Color.FromArgb(148, 163, 184); // Slate-400 (descrições)
        public static Color ElementBorder = Color.FromArgb(51, 65, 85);// Slate-700
        public static Color Success = Color.FromArgb(34, 197, 94);     // Green-500
        public static Color Danger = Color.FromArgb(239, 68, 68);      // Red-500
        public static Color Warning = Color.FromArgb(245, 158, 11);     // Amber-500
        public static Color BgModal = Color.FromArgb(25, 34, 49);       // Fundo das janelas modais (diferente do app)
        public static Color BgModalHeader = Color.FromArgb(18, 25, 38); // Fundo do header das modais
        
        // Fontes modernas
        public static Font FontTitle = new Font("Segoe UI", 16F, FontStyle.Bold);
        public static Font FontSubtitle = new Font("Segoe UI", 12F, FontStyle.Regular);
        public static Font FontBody = new Font("Segoe UI", 10F, FontStyle.Regular);
        public static Font FontBodyBold = new Font("Segoe UI", 10F, FontStyle.Bold);
        public static Font FontSmall = new Font("Segoe UI", 8F, FontStyle.Regular);
        public static Font FontSmallBold = new Font("Segoe UI", 8F, FontStyle.Bold);

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        /// <summary>
        /// Aplica o estilo de modal (sem borda, cor de fundo, barra customizada)
        /// </summary>
        public static void ApplyModalWindowStyle(Form form)
        {
            form.FormBorderStyle = FormBorderStyle.None;
            form.BackColor = BgModal;
            form.StartPosition = FormStartPosition.CenterParent;
            form.Padding = new Padding(1);
            
            try { form.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }

            form.Height += 40;
            foreach (Control c in form.Controls)
            {
                c.Top += 40;
            }

            // Draw modern accent border
            form.Paint += (sender, e) =>
            {
                using (Pen pen = new Pen(Primary, 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, form.Width - 1, form.Height - 1);
                }
            };

            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = BgModalHeader
            };

            Button btnClose = new Button
            {
                Text = "X",
                Width = 40,
                Height = 40,
                Dock = DockStyle.Right,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.LightGray,
                Font = new Font("Arial", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => form.Close();

            Label lblHeaderTitle = new Label
            {
                Text = form.Text,
                ForeColor = TextTitle,
                Font = FontBodyBold,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Padding = new Padding(10, 0, 0, 0)
            };

            MouseEventHandler dragEvent = (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(form.Handle, 0x112, 0xf012, 0);
                }
            };

            pnlHeader.MouseDown += dragEvent;
            lblHeaderTitle.MouseDown += dragEvent;
            form.MouseDown += dragEvent;

            pnlHeader.Controls.Add(btnClose);
            pnlHeader.Controls.Add(lblHeaderTitle);
            
            form.Controls.Add(pnlHeader);
        }

        /// <summary>
        /// Aplica arredondamento em um controle customizado ou Painel, usando o Paint
        /// Exemplo: panel.Paint += (s, e) => UITheme.DrawRoundedPanel(e.Graphics, panel.ClientRectangle, UITheme.BgCard, 10);
        /// </summary>
        public static void DrawRoundedPanel(Graphics g, Rectangle rect, Color bgColor, int radius = 10, Color? borderColor = null)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                path.AddArc(rect.Right - radius - 1, rect.Y, radius, radius, 270, 90);
                path.AddArc(rect.Right - radius - 1, rect.Bottom - radius - 1, radius, radius, 0, 90);
                path.AddArc(rect.X, rect.Bottom - radius - 1, radius, radius, 90, 90);
                path.CloseFigure();

                using (SolidBrush brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

                if (borderColor.HasValue)
                {
                    using (Pen pen = new Pen(borderColor.Value, 1))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }
        
        /// <summary>
        /// Ajusta um botão plano padrão para ter aparência SaaS
        /// </summary>
        public static void FormatSaaSButton(Button btn, bool isPrimary = true)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = isPrimary ? Primary : ElementBorder;
            btn.ForeColor = Color.White;
            btn.Font = FontBodyBold;
            btn.Cursor = Cursors.Hand;
            
            // Hover logic could be attached here or via derived class
        }

        /// <summary>
        /// Aplica o estilo premium e plano em tabelas (DataGridView)
        /// </summary>
        public static void FormatGrid(DataGridView dgv)
        {
            dgv.BackgroundColor = BgCard; // Fundo da grid em formato de card para não parecer solta
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = ElementBorder; // Cor das linhas divisórias horizontais
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Esticar colunas para preencher o espaço
            
            // Estilo do Cabeçalho
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = BgSidebar;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextTitle;
            dgv.ColumnHeadersDefaultCellStyle.Font = FontBodyBold;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = BgSidebar;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(15, 0, 15, 0); // Alinha com o texto dos cards
            dgv.ColumnHeadersHeight = 45; // Mais respiro no cabeçalho

            // Estilo das Células (Conteúdo)
            dgv.DefaultCellStyle.BackColor = BgCard;
            dgv.DefaultCellStyle.ForeColor = TextTitle;
            dgv.DefaultCellStyle.Font = FontBody;
            dgv.DefaultCellStyle.Padding = new Padding(15, 0, 15, 0); // Alinha com o texto dos cards
            
            // COR DA SELEÇÃO (Fim do Azulão do Windows)
            // Usamos uma cor Indigo um pouco mais escura/transparente, ou o ElementBorder para um tom chumbo elegante.
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(49, 46, 129); // Indigo-900 (Roxo super elegante escuro)
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;

            // Remove a coluna da setinha na esquerda
            dgv.RowHeadersVisible = false;
            
            // Aumenta a altura das linhas para não ficar espremido
            dgv.RowTemplate.Height = 40;
            
            // Configurações de comportamento
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.ReadOnly = true;
        }

        /// <summary>
        /// Aplica um estilo moderno para TextBox, removendo a borda 3D nativa do Windows
        /// </summary>
        public static void FormatModernTextBox(TextBox txt)
        {
            txt.BorderStyle = BorderStyle.None;
            txt.BackColor = BgInput;
            txt.ForeColor = TextTitle;
            txt.Font = FontBody;
            
            // Padding nativo do TextBox não funciona bem, mas remover a borda ajuda muito.
            // Para adicionar uma borda customizada, precisamos desenhar em volta ou colocar dentro de um Panel.
        }

        /// <summary>
        /// Aplica um estilo moderno para ComboBox
        /// </summary>
        public static void FormatModernComboBox(ComboBox cb)
        {
            cb.FlatStyle = FlatStyle.Flat;
            cb.BackColor = BgInput;
            cb.ForeColor = TextTitle;
            cb.Font = FontBody;
        }

        /// <summary>
        /// Aplica um estilo moderno para TabControl, desenhando as abas manualmente para remover o visual antigo
        /// </summary>
        public static void FormatModernTabControl(TabControl tc)
        {
            tc.DrawMode = TabDrawMode.OwnerDrawFixed;
            tc.SizeMode = TabSizeMode.Fixed;
            tc.ItemSize = new Size(150, 40);

            tc.DrawItem += (s, e) =>
            {
                Graphics g = e.Graphics;
                TabPage tp = tc.TabPages[e.Index];
                Rectangle tabRect = tc.GetTabRect(e.Index);

                bool isSelected = (tc.SelectedIndex == e.Index);
                Color bgColor = isSelected ? BgApp : BgSidebar;
                Color textColor = isSelected ? Primary : TextMuted;

                using (SolidBrush brush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(brush, tabRect);
                }

                // Linha inferior de destaque para a aba selecionada
                if (isSelected)
                {
                    using (SolidBrush accentBrush = new SolidBrush(Primary))
                    {
                        g.FillRectangle(accentBrush, tabRect.X, tabRect.Bottom - 3, tabRect.Width, 3);
                    }
                }

                TextRenderer.DrawText(g, tp.Text, FontBodyBold, tabRect, textColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
        }
    }

}
