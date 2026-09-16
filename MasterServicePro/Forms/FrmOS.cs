using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public partial class FrmOS : Form
    {
        private OrdemServicoRepository repository = new OrdemServicoRepository();

        private Button btnToggleView;
        private Panel pnlKanban;
        private TableLayoutPanel tlpKanban;
        private FlowLayoutPanel flpAberto;
        private FlowLayoutPanel flpProgresso;
        private FlowLayoutPanel flpPronto;
        private FlowLayoutPanel flpCancelado;
        private bool isKanbanView = false;

        public FrmOS()
        {
            InitializeComponent();
            ApplyTheme();
            InitializeKanban();
            LoadData();
            
            // Events 
            btnNovo.Click += BtnNovo_Click;
            btnEditar.Click += BtnEditar_Click;
            btnImprimir.Click += BtnImprimir_Click;
            btnExcluir.Click += BtnExcluir_Click;
            txtBusca.TextChanged += TxtBusca_TextChanged;
            dgvOS.CellClick += DgvOS_CellClick;
            dgvOS.CellFormatting += DgvOS_CellFormatting;

            rdoTodas.CheckedChanged += RdoFiltro_CheckedChanged;
            rdoPendentes.CheckedChanged += RdoFiltro_CheckedChanged;
            rdoProntas.CheckedChanged += RdoFiltro_CheckedChanged;
            rdoEntregues.CheckedChanged += RdoFiltro_CheckedChanged;

            SetupContextMenu();
        }

        private void ApplyTheme()
        {
            this.BackColor = UITheme.BgModal;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(2);
            this.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(UITheme.Primary, 2))
                {
                    e.Graphics.DrawRectangle(pen, 1, 1, this.Width - 2, this.Height - 2);
                }
            };
            
            FlowLayoutPanel flpControls = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Right,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Padding = new Padding(0, 10, 10, 0),
                BackColor = Color.Transparent
            };

            Button btnClose = new Button { Text = "X", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = UITheme.TextMuted, BackColor = Color.Transparent, FlatStyle = FlatStyle.Flat, Size = new Size(40, 30), Cursor = Cursors.Hand, Margin = new Padding(0) };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = UITheme.Danger;
            btnClose.Click += (s, e) => this.Close();

            Button btnMax = new Button { Text = "🗖", Font = new Font("Segoe UI", 10F), ForeColor = UITheme.TextMuted, BackColor = Color.Transparent, FlatStyle = FlatStyle.Flat, Size = new Size(40, 30), Cursor = Cursors.Hand, Margin = new Padding(0) };
            btnMax.FlatAppearance.BorderSize = 0;
            btnMax.Click += (s, e) => 
            {
                if (this.WindowState == FormWindowState.Normal) 
                {
                    this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
                    this.WindowState = FormWindowState.Maximized;
                }
                else 
                {
                    this.WindowState = FormWindowState.Normal;
                }
            };

            Button btnMin = new Button { Text = "—", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = UITheme.TextMuted, BackColor = Color.Transparent, FlatStyle = FlatStyle.Flat, Size = new Size(40, 30), Cursor = Cursors.Hand, Margin = new Padding(0) };
            btnMin.FlatAppearance.BorderSize = 0;
            btnMin.Click += (s, e) => this.WindowState = FormWindowState.Minimized;

            flpControls.Controls.Add(btnClose);
            flpControls.Controls.Add(btnMax);
            flpControls.Controls.Add(btnMin);

            pnlHeader.Controls.Add(flpControls);
            btnNovo.Left -= 130; // Evita sobreposição com os botões da janela
            
            // Permite mover a janela arrastando o cabeçalho
            bool dragging = false;
            Point dragCursorPoint = Point.Empty;
            Point dragFormPoint = Point.Empty;

            MouseEventHandler mouseDown = (s, e) => { if (e.Button == MouseButtons.Left) { dragging = true; dragCursorPoint = Cursor.Position; dragFormPoint = this.Location; } };
            MouseEventHandler mouseMove = (s, e) => { if (dragging) { Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint)); this.Location = Point.Add(dragFormPoint, new Size(dif)); } };
            MouseEventHandler mouseUp = (s, e) => { dragging = false; };

            pnlHeader.MouseDown += mouseDown;
            pnlHeader.MouseMove += mouseMove;
            pnlHeader.MouseUp += mouseUp;
            
            lblTitle.MouseDown += mouseDown;
            lblTitle.MouseMove += mouseMove;
            lblTitle.MouseUp += mouseUp;
            
            pnlHeader.BackColor = UITheme.BgModalHeader;
            pnlSearch.BackColor = UITheme.BgModal;
            tlpMain.BackColor = UITheme.BgModal;

            lblTitle.Font = UITheme.FontTitle;
            lblTitle.ForeColor = UITheme.TextTitle;

            lblBusca.Font = UITheme.FontBody;
            lblBusca.ForeColor = UITheme.TextMuted;

            txtBusca.Font = UITheme.FontBody;
            txtBusca.BackColor = UITheme.BgSidebar;
            txtBusca.ForeColor = UITheme.TextTitle;
            txtBusca.BorderStyle = BorderStyle.FixedSingle;

            UITheme.FormatSaaSButton(btnNovo, true);
            btnNovo.Font = new Font(UITheme.FontBodyBold.FontFamily, 11F, FontStyle.Bold);
            UITheme.FormatSaaSButton(btnEditar, false);
            btnEditar.Text = "Editar O.S."; // Corrigindo o texto cortado
            UITheme.FormatSaaSButton(btnImprimir, false);
            UITheme.FormatSaaSButton(btnExcluir, false);
            btnExcluir.BackColor = UITheme.Danger;
            
            // Ocultar os botões do topo pois agora estão na tabela!
            btnEditar.Visible = false;
            btnImprimir.Visible = false;
            btnExcluir.Visible = false;

            // Tabs styling
            flpFiltros.BackColor = UITheme.BgCard;
            rdoPendentes.Text = "Em Aberto";
            rdoProntas.Text = "Prontas";
            rdoEntregues.Text = "Canceladas";
            
            foreach (var btn in new[] { rdoTodas, rdoPendentes, rdoProntas, rdoEntregues })
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.CheckedBackColor = UITheme.Primary;
                btn.Cursor = Cursors.Hand;
                btn.Font = UITheme.FontBodyBold;
                btn.Padding = new Padding(10, 5, 10, 5);
            }
            StyleFilterButtons();

            // Style DataGridView (Substituído pelo novo método global premium)
            UITheme.FormatGrid(dgvOS);
            dgvOS.CellPainting += DgvOS_CellPainting;
        }

        private void DgvOS_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvOS.Columns[e.ColumnIndex].Name == "Status")
            {
                e.PaintBackground(e.CellBounds, true);
                
                string status = e.Value?.ToString() ?? "";
                Color bgColor = UITheme.BgSidebar;
                Color txtColor = UITheme.TextMuted;

                if (status.Equals("Aberto", StringComparison.OrdinalIgnoreCase))
                {
                    bgColor = UITheme.Warning; // Yellow
                    txtColor = Color.Black;
                }
                else if (status.Equals("Em Andamento", StringComparison.OrdinalIgnoreCase))
                {
                    bgColor = UITheme.Primary; // Blue
                    txtColor = Color.White;
                }
                else if (status.Equals("Finalizado", StringComparison.OrdinalIgnoreCase))
                {
                    bgColor = UITheme.Success; // Green
                    txtColor = Color.White;
                }
                else if (status.Equals("Cancelado", StringComparison.OrdinalIgnoreCase))
                {
                    bgColor = UITheme.Danger; // Red
                    txtColor = Color.White;
                }

                // Draw the badge
                int padding = 8;
                int height = e.CellBounds.Height - (padding * 2);
                
                SizeF textSize = e.Graphics.MeasureString(status, UITheme.FontBodyBold);
                int width = (int)textSize.Width + 24;

                Rectangle badgeRect = new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + (e.CellBounds.Height - height) / 2, width, height);
                
                using (Brush brush = new SolidBrush(bgColor))
                {
                    System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                    int radius = 10;
                    path.AddArc(badgeRect.X, badgeRect.Y, radius, radius, 180, 90);
                    path.AddArc(badgeRect.X + badgeRect.Width - radius, badgeRect.Y, radius, radius, 270, 90);
                    path.AddArc(badgeRect.X + badgeRect.Width - radius, badgeRect.Y + badgeRect.Height - radius, radius, radius, 0, 90);
                    path.AddArc(badgeRect.X, badgeRect.Y + badgeRect.Height - radius, radius, radius, 90, 90);
                    path.CloseFigure();

                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    e.Graphics.FillPath(brush, path);
                }

                using (Brush textBrush = new SolidBrush(txtColor))
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    e.Graphics.DrawString(status, UITheme.FontBodyBold, textBrush, badgeRect, sf);
                }

                e.Handled = true;
            }
        }

        private void DgvOS_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvOS.Columns[e.ColumnIndex].Name == "ColCancelar")
            {
                var status = dgvOS.Rows[e.RowIndex].Cells["Status"].Value?.ToString();
                if (status != null && status.Equals("Cancelada", StringComparison.OrdinalIgnoreCase))
                {
                    e.Value = "🗑️";
                }
                else
                {
                    e.Value = "❌";
                }
            }
        }

        private void StyleFilterButtons()
        {
            var buttons = new[] { rdoTodas, rdoPendentes, rdoProntas, rdoEntregues };
            foreach(var btn in buttons)
            {
                if (btn.Checked)
                {
                    btn.BackColor = UITheme.Primary;
                    btn.ForeColor = Color.White;
                }
                else
                {
                    btn.BackColor = UITheme.BgCard;
                    btn.ForeColor = UITheme.TextMuted;
                }
            }
        }

        private void RdoFiltro_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rdo = sender as RadioButton;
            if (rdo != null && rdo.Checked)
            {
                StyleFilterButtons();
                LoadData(txtBusca.Text);
            }
        }

        private void LoadData(string filtro = "")
        {
            DataTable dt = repository.BuscarTudoResumido();

            string rowFilter = "";
            
            if (rdoPendentes.Checked) 
                rowFilter = "Status = 'Pendente' OR Status = 'Aberto' OR Status = 'Em Andamento'";
            else if (rdoProntas.Checked) 
                rowFilter = "Status = 'Finalizado'";
            else if (rdoEntregues.Checked) 
                rowFilter = "Status = 'Cancelado'";

            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.ToLower();
                string searchFilter = string.Format("(Cliente LIKE '%{0}%' OR Marca LIKE '%{0}%' OR Modelo LIKE '%{0}%')", filtro);
                if (!string.IsNullOrEmpty(rowFilter))
                    rowFilter += " AND " + searchFilter;
                else
                    rowFilter = searchFilter;
            }

            DataView dv = dt.DefaultView;
            if (!string.IsNullOrEmpty(rowFilter))
                dv.RowFilter = rowFilter;
                
            dt = dv.ToTable();

            if (isKanbanView)
            {
                RenderKanban(dt);
                return;
            }

            dgvOS.DataSource = null;
            dgvOS.DataSource = dt;

            // Formatting safely
            if (dgvOS.Columns.Count > 0)
            {
                if (dgvOS.Columns["Id"] != null) 
                {
                    dgvOS.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    dgvOS.Columns["Id"].Width = 50;
                }

                if (dgvOS.Columns["Cliente"] != null) dgvOS.Columns["Cliente"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                if (dgvOS.Columns["Defeito"] != null) dgvOS.Columns["Defeito"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                
                if (dgvOS.Columns["ValorTotal"] != null) 
                {
                    dgvOS.Columns["ValorTotal"].HeaderText = "Lucro (R$)";
                    dgvOS.Columns["ValorTotal"].DefaultCellStyle.Format = "C2";
                }

                // --- Adicionando Botões de Ação na Tabela ---
                if (dgvOS.Columns["ColEditar"] == null)
                {
                    DataGridViewButtonColumn btnEdit = new DataGridViewButtonColumn();
                    btnEdit.Name = "ColEditar";
                    btnEdit.HeaderText = "";
                    btnEdit.Text = "✏️";
                    btnEdit.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    btnEdit.Width = 45;
                    btnEdit.UseColumnTextForButtonValue = true;
                    btnEdit.FlatStyle = FlatStyle.Flat;
                    btnEdit.DefaultCellStyle.BackColor = UITheme.BgCard;
                    btnEdit.DefaultCellStyle.ForeColor = UITheme.Primary;
                    dgvOS.Columns.Add(btnEdit);
                }
                
                if (dgvOS.Columns["ColImprimir"] == null)
                {
                    DataGridViewButtonColumn btnPrint = new DataGridViewButtonColumn();
                    btnPrint.Name = "ColImprimir";
                    btnPrint.HeaderText = "";
                    btnPrint.Text = "🖨️";
                    btnPrint.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    btnPrint.Width = 45;
                    btnPrint.UseColumnTextForButtonValue = true;
                    btnPrint.FlatStyle = FlatStyle.Flat;
                    btnPrint.DefaultCellStyle.BackColor = UITheme.BgCard;
                    btnPrint.DefaultCellStyle.ForeColor = UITheme.TextTitle;
                    dgvOS.Columns.Add(btnPrint);
                }

                if (dgvOS.Columns["ColRecibo"] == null)
                {
                    DataGridViewButtonColumn btnRecibo = new DataGridViewButtonColumn();
                    btnRecibo.Name = "ColRecibo";
                    btnRecibo.HeaderText = "";
                    btnRecibo.Text = "📄";
                    btnRecibo.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    btnRecibo.Width = 45;
                    btnRecibo.UseColumnTextForButtonValue = true;
                    btnRecibo.FlatStyle = FlatStyle.Flat;
                    btnRecibo.DefaultCellStyle.BackColor = UITheme.BgCard;
                    btnRecibo.DefaultCellStyle.ForeColor = UITheme.Success;
                    dgvOS.Columns.Add(btnRecibo);
                }

                if (dgvOS.Columns["ColEtiqueta"] == null)
                {
                    DataGridViewButtonColumn btnEtiqueta = new DataGridViewButtonColumn();
                    btnEtiqueta.Name = "ColEtiqueta";
                    btnEtiqueta.HeaderText = "";
                    btnEtiqueta.Text = "🏷️";
                    btnEtiqueta.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    btnEtiqueta.Width = 45;
                    btnEtiqueta.UseColumnTextForButtonValue = true;
                    btnEtiqueta.FlatStyle = FlatStyle.Flat;
                    btnEtiqueta.DefaultCellStyle.BackColor = UITheme.BgCard;
                    btnEtiqueta.DefaultCellStyle.ForeColor = UITheme.Success; 
                    dgvOS.Columns.Add(btnEtiqueta);
                }

                if (dgvOS.Columns["ColCancelar"] == null)
                {
                    DataGridViewButtonColumn btnCancel = new DataGridViewButtonColumn();
                    btnCancel.Name = "ColCancelar";
                    btnCancel.HeaderText = "";
                    btnCancel.Text = "❌";
                    btnCancel.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    btnCancel.Width = 45;
                    btnCancel.UseColumnTextForButtonValue = false;
                    btnCancel.FlatStyle = FlatStyle.Flat;
                    btnCancel.DefaultCellStyle.BackColor = UITheme.BgCard;
                    btnCancel.DefaultCellStyle.ForeColor = UITheme.Danger;
                    dgvOS.Columns.Add(btnCancel);
                }

                // Corrige o posicionamento (bug do winforms ao recarregar datasource)
                int lastIdx = dgvOS.Columns.Count - 1;
                if (dgvOS.Columns["ColCancelar"] != null) dgvOS.Columns["ColCancelar"].DisplayIndex = lastIdx;
                if (dgvOS.Columns["ColEtiqueta"] != null) dgvOS.Columns["ColEtiqueta"].DisplayIndex = lastIdx - 1;
                if (dgvOS.Columns["ColRecibo"] != null) dgvOS.Columns["ColRecibo"].DisplayIndex = lastIdx - 2;
                if (dgvOS.Columns["ColImprimir"] != null) dgvOS.Columns["ColImprimir"].DisplayIndex = lastIdx - 3;
                if (dgvOS.Columns["ColEditar"] != null) dgvOS.Columns["ColEditar"].DisplayIndex = lastIdx - 4;
            }
        }

        private void DgvOS_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == dgvOS.Columns["ColEditar"]?.Index)
                {
                    int id = Convert.ToInt32(dgvOS.Rows[e.RowIndex].Cells["Id"].Value);
                    using (var form = new FrmOSAddEdit(id))
                    {
                        if (form.ShowDialog() == DialogResult.OK) dgvOS.DataSource = repository.BuscarTudoResumido();
                    }
                }
                else if (e.ColumnIndex == dgvOS.Columns["ColImprimir"]?.Index)
                {
                    int id = Convert.ToInt32(dgvOS.Rows[e.RowIndex].Cells["Id"].Value);
                    MasterServicePro.Utils.OSPrinter printer = new MasterServicePro.Utils.OSPrinter();
                    printer.Imprimir(id);
                }
                else if (e.ColumnIndex == dgvOS.Columns["ColRecibo"]?.Index)
                {
                    int id = Convert.ToInt32(dgvOS.Rows[e.RowIndex].Cells["Id"].Value);
                    MasterServicePro.Utils.OSPrinter printer = new MasterServicePro.Utils.OSPrinter();
                    printer.ImprimirReciboEntrega(id);
                }
                else if (e.ColumnIndex == dgvOS.Columns["ColEtiqueta"]?.Index)
                {
                    int id = Convert.ToInt32(dgvOS.Rows[e.RowIndex].Cells["Id"].Value);
                    MasterServicePro.Utils.OSPrinter printer = new MasterServicePro.Utils.OSPrinter();
                    printer.ImprimirEtiquetaAparelho(id);
                }
                else if (e.ColumnIndex == dgvOS.Columns["ColCancelar"]?.Index)
                {
                    int id = Convert.ToInt32(dgvOS.Rows[e.RowIndex].Cells["Id"].Value);
                    var status = dgvOS.Rows[e.RowIndex].Cells["Status"].Value?.ToString();
                    
                    if (status != null && status.Equals("Cancelada", StringComparison.OrdinalIgnoreCase))
                    {
                        if (MessageBox.Show("Tem certeza que deseja DELETAR PERMANENTEMENTE esta OS do sistema?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            repository.Deletar(id);
                            LoadData();
                        }
                    }
                    else
                    {
                        if (MessageBox.Show("Tem certeza que deseja cancelar esta OS?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            repository.Excluir(id);
                            LoadData();
                        }
                    }
                }
            }
        }

        private void SetupContextMenu()
        {
            ContextMenuStrip cms = new ContextMenuStrip();
            cms.Items.Add("✏️ Editar OS", null, (s, e) => BtnEditar_Click(null, null));
            cms.Items.Add("🖨️ Imprimir OS", null, (s, e) => BtnImprimir_Click(null, null));
            cms.Items.Add("📄 Imprimir Recibo", null, (s, e) => 
            {
                if (dgvOS.SelectedRows.Count > 0)
                {
                    int id = Convert.ToInt32(dgvOS.SelectedRows[0].Cells["Id"].Value);
                    MasterServicePro.Utils.OSPrinter printer = new MasterServicePro.Utils.OSPrinter();
                    printer.ImprimirReciboEntrega(id);
                }
            });
            cms.Items.Add("🏷️ Imprimir Etiqueta", null, (s, e) => 
            {
                if (dgvOS.SelectedRows.Count > 0)
                {
                    int id = Convert.ToInt32(dgvOS.SelectedRows[0].Cells["Id"].Value);
                    MasterServicePro.Utils.OSPrinter printer = new MasterServicePro.Utils.OSPrinter();
                    printer.ImprimirEtiquetaAparelho(id);
                }
            });
            cms.Items.Add(new ToolStripSeparator());
            cms.Items.Add("❌ Cancelar / Excluir OS", null, (s, e) => BtnExcluir_Click(null, null));
            
            dgvOS.ContextMenuStrip = cms;

            dgvOS.CellMouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
                {
                    dgvOS.ClearSelection();
                    dgvOS.Rows[e.RowIndex].Selected = true;
                }
            };
        }

        private void TxtBusca_TextChanged(object sender, EventArgs e)
        {
            LoadData(txtBusca.Text);
        }

        private void BtnNovo_Click(object sender, EventArgs e)
        {
            using (var form = new FrmOSAddEdit())
            {
                if (form.ShowDialog() == DialogResult.OK) LoadData();
            }
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (dgvOS.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvOS.SelectedRows[0].Cells["Id"].Value);
                using (var form = new FrmOSAddEdit(id))
                {
                    if (form.ShowDialog() == DialogResult.OK) LoadData();
                }
            }
        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            if (dgvOS.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvOS.SelectedRows[0].Cells["Id"].Value);
                MasterServicePro.Utils.OSPrinter printer = new MasterServicePro.Utils.OSPrinter();
                printer.Imprimir(id);
            }
            else
            {
                MessageBox.Show("Selecione uma ordem de serviço para imprimir.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (dgvOS.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvOS.SelectedRows[0].Cells["Id"].Value);
                if (MessageBox.Show("Tem certeza que cancelar esta OS? Ela ficará com status Cancelada.", "Atencão", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    repository.Excluir(id);
                    LoadData();
                }
            }
        }

        private void InitializeKanban()
        {
            btnToggleView = new Button 
            { 
                Text = "📋 Modo Kanban", 
                Size = new Size(150, 35), 
                Location = new Point(btnNovo.Left - 165, 12), 
                Anchor = AnchorStyles.Top | AnchorStyles.Right 
            };
            UITheme.FormatSaaSButton(btnToggleView, false);
            btnToggleView.Click += BtnToggleView_Click;
            pnlHeader.Controls.Add(btnToggleView);

            pnlKanban = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = false,
                Padding = new Padding(20, 0, 20, 20),
                BackColor = UITheme.BgCard
            };
            
            tlpMain.Controls.Add(pnlKanban, 0, 2);

            tlpKanban = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1
            };
            tlpKanban.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpKanban.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpKanban.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpKanban.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            pnlKanban.Controls.Add(tlpKanban);

            flpAberto = CreateKanbanColumn("EM ABERTO", UITheme.Warning, tlpKanban, 0);
            flpProgresso = CreateKanbanColumn("EM ANDAMENTO", UITheme.Primary, tlpKanban, 1);
            flpPronto = CreateKanbanColumn("FINALIZADO", UITheme.Success, tlpKanban, 2);
            flpCancelado = CreateKanbanColumn("CANCELADO", UITheme.Danger, tlpKanban, 3);
        }

        private FlowLayoutPanel CreateKanbanColumn(string title, Color accentColor, TableLayoutPanel parent, int colIndex)
        {
            Panel pnlCol = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5),
                BackColor = UITheme.BgCard
            };
            pnlCol.Paint += (s, e) => {
                UITheme.DrawRoundedPanel(e.Graphics, pnlCol.ClientRectangle, UITheme.BgSidebar, 8, UITheme.ElementBorder);
            };

            Label lblTitle = new Label
            {
                Text = title,
                Font = UITheme.FontBodyBold,
                ForeColor = UITheme.TextTitle,
                Dock = DockStyle.Top,
                Height = 35,
                TextAlign = ContentAlignment.MiddleCenter
            };
            
            Panel line = new Panel
            {
                BackColor = accentColor,
                Dock = DockStyle.Top,
                Height = 3,
                Margin = new Padding(0, 0, 0, 10)
            };

            FlowLayoutPanel flp = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(3),
                BackColor = Color.Transparent
            };
            flp.SizeChanged += (s, e) => {
                flp.FlowDirection = FlowDirection.TopDown;
            };

            pnlCol.Controls.Add(flp);
            pnlCol.Controls.Add(line);
            pnlCol.Controls.Add(lblTitle);

            parent.Controls.Add(pnlCol, colIndex, 0);
            return flp;
        }

        private void BtnToggleView_Click(object sender, EventArgs e)
        {
            isKanbanView = !isKanbanView;
            if (isKanbanView)
            {
                btnToggleView.Text = "📋 Modo Tabela";
                dgvOS.Visible = false;
                pnlKanban.Visible = true;
                LoadData(txtBusca.Text);
            }
            else
            {
                btnToggleView.Text = "📋 Modo Kanban";
                pnlKanban.Visible = false;
                dgvOS.Visible = true;
                LoadData(txtBusca.Text);
            }
        }

        private void RenderKanban(DataTable dt)
        {
            flpAberto.Controls.Clear();
            flpProgresso.Controls.Clear();
            flpPronto.Controls.Clear();
            flpCancelado.Controls.Clear();

            // Determinar largura para caber no flow sem quebrar
            int cardWidth = Math.Max(150, flpAberto.Width - 15);

            foreach (DataRow row in dt.Rows)
            {
                int id = Convert.ToInt32(row["Id"]);
                string cliente = row["Cliente"]?.ToString() ?? "Sem Cliente";
                string marca = row["Marca"]?.ToString() ?? "";
                string modelo = row["Modelo"]?.ToString() ?? "";
                string defeito = row["Defeito"]?.ToString() ?? "";
                decimal valorTotal = row["ValorTotal"] != DBNull.Value ? Convert.ToDecimal(row["ValorTotal"]) : 0;
                string status = row["Status"]?.ToString() ?? "Aberto";

                Panel card = new Panel
                {
                    Width = cardWidth,
                    Height = 110,
                    Margin = new Padding(0, 0, 0, 10),
                    Cursor = Cursors.Hand,
                    Tag = id
                };

                card.Paint += (s, e) => {
                    UITheme.DrawRoundedPanel(e.Graphics, card.ClientRectangle, UITheme.BgCard, 8, UITheme.ElementBorder);
                    
                    Color color = UITheme.Warning;
                    if (status.Equals("Em Andamento", StringComparison.OrdinalIgnoreCase)) color = UITheme.Primary;
                    else if (status.Equals("Finalizado", StringComparison.OrdinalIgnoreCase)) color = UITheme.Success;
                    else if (status.Equals("Cancelado", StringComparison.OrdinalIgnoreCase) || status.Equals("Cancelada", StringComparison.OrdinalIgnoreCase)) color = UITheme.Danger;

                    using (var brush = new SolidBrush(color))
                    {
                        e.Graphics.FillRectangle(brush, 4, 10, 4, card.Height - 20);
                    }
                };

                Label lblOS = new Label
                {
                    Text = $"O.S. #{id} - Lucro: {valorTotal:C2}",
                    Font = UITheme.FontBodyBold,
                    ForeColor = UITheme.TextTitle,
                    Location = new Point(15, 10),
                    AutoSize = true
                };

                Label lblClienteDisp = new Label
                {
                    Text = $"{cliente}\n{marca} {modelo}".Replace("  ", " ").Trim(),
                    Font = UITheme.FontBody,
                    ForeColor = UITheme.TextMuted,
                    Location = new Point(15, 30),
                    Size = new Size(cardWidth - 25, 35),
                    AutoEllipsis = true
                };

                Label lblDefeito = new Label
                {
                    Text = defeito,
                    Font = UITheme.FontSmall,
                    ForeColor = UITheme.Danger,
                    Location = new Point(15, 68),
                    Size = new Size(cardWidth - 55, 35),
                    AutoEllipsis = true
                };

                Button btnEdit = new Button
                {
                    Text = "✏️",
                    Size = new Size(30, 25),
                    Location = new Point(cardWidth - 40, 75),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnEdit.FlatAppearance.BorderSize = 0;
                btnEdit.Click += (s, e) => {
                    using (var form = new FrmOSAddEdit(id))
                    {
                        if (form.ShowDialog() == DialogResult.OK) LoadData(txtBusca.Text);
                    }
                };

                card.DoubleClick += (s, e) => {
                    using (var form = new FrmOSAddEdit(id))
                    {
                        if (form.ShowDialog() == DialogResult.OK) LoadData(txtBusca.Text);
                    }
                };

                ContextMenuStrip cms = new ContextMenuStrip();
                cms.Items.Add("Mover para: EM ABERTO", null, (s, e) => MoverStatus(id, "Aberto"));
                cms.Items.Add("Mover para: EM ANDAMENTO", null, (s, e) => MoverStatus(id, "Em Andamento"));
                cms.Items.Add("Mover para: FINALIZADO", null, (s, e) => MoverStatus(id, "Finalizado"));
                cms.Items.Add("Mover para: CANCELADO", null, (s, e) => MoverStatus(id, "Cancelado"));
                cms.Items.Add(new ToolStripSeparator());
                cms.Items.Add("💬 Enviar p/ WhatsApp", null, (s, e) => EnviarWhatsAppManual(id));
                card.ContextMenuStrip = cms;

                card.Controls.Add(lblOS);
                card.Controls.Add(lblClienteDisp);
                card.Controls.Add(lblDefeito);
                card.Controls.Add(btnEdit);

                if (status.Equals("Em Andamento", StringComparison.OrdinalIgnoreCase))
                    flpProgresso.Controls.Add(card);
                else if (status.Equals("Finalizado", StringComparison.OrdinalIgnoreCase))
                    flpPronto.Controls.Add(card);
                else if (status.Equals("Cancelado", StringComparison.OrdinalIgnoreCase) || status.Equals("Cancelada", StringComparison.OrdinalIgnoreCase))
                    flpCancelado.Controls.Add(card);
                else
                    flpAberto.Controls.Add(card);
            }
        }

        private void EnviarWhatsAppManual(int idOS)
        {
            try
            {
                var os = repository.BuscarPorId(idOS);
                if (os != null)
                {
                    System.Threading.Tasks.Task.Run(async () => {
                        var ws = new Services.WhatsAppService();
                        string tipo = os.Status.Equals("Finalizado", StringComparison.OrdinalIgnoreCase) ? "Finalizado" : "Atualizacao";
                        await ws.EnviarNotificacaoOSAsync(os, tipo);
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao enviar notificação manual: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MoverStatus(int idOS, string novoStatus)
        {
            try
            {
                var os = repository.BuscarPorId(idOS);
                if (os != null)
                {
                    os.Status = novoStatus;
                    
                    if (novoStatus.Equals("Finalizado", StringComparison.OrdinalIgnoreCase) && !os.Faturado)
                    {
                        using (var frm = new FrmPromptPagamentoOS(os.ValorServico))
                        {
                            if (frm.ShowDialog() == DialogResult.OK)
                            {
                                os.Faturado = true;
                                
                                if (frm.JaRecebido)
                                {
                                    var finRepo = new MasterServicePro.DAL.FinanceiroRepository();
                                    var caixa = finRepo.ObterCaixaAberto();
                                    finRepo.LançarMovimentacao(new MasterServicePro.Models.Movimentacao
                                    {
                                        Tipo = "Entrada",
                                        Categoria = "Serviços",
                                        Subcategoria = "Ordem de Serviço",
                                        Valor = os.ValorServico,
                                        FormaPagamento = frm.FormaPagamento,
                                        Descricao = $"Recebimento OS #{os.NumeroOS}",
                                        IdCaixa = caixa != null ? caixa.Id : 0
                                    });
                                }
                                else
                                {
                                    var crRepo = new MasterServicePro.DAL.ContasReceberRepository();
                                    crRepo.Inserir(new MasterServicePro.Models.ContaReceber
                                    {
                                        IdCliente = os.IdCliente,
                                        Descricao = $"Serviço OS #{os.NumeroOS}",
                                        ValorTotal = os.ValorServico,
                                        ValorPago = 0,
                                        DataLancamento = DateTime.Now,
                                        DataVencimento = DateTime.Now.AddDays(30),
                                        Status = "Pendente"
                                    });
                                }
                            }
                        }
                    }
                    
                    repository.Atualizar(os);
                    LoadData(txtBusca.Text);

                    // Enviar notificação de alteração / conclusão
                    System.Threading.Tasks.Task.Run(async () => {
                        var ws = new Services.WhatsAppService();
                        string tipo = os.Status.Equals("Finalizado", StringComparison.OrdinalIgnoreCase) ? "Finalizado" : "Atualizacao";
                        await ws.EnviarNotificacaoOSAsync(os, tipo);
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao mover status da O.S.: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
