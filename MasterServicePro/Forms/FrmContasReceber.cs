using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public class FrmContasReceber : Form
    {
        private ContasReceberRepository _repository = new ContasReceberRepository();
        private ClienteRepository _clienteRepo = new ClienteRepository();
        private FinanceiroRepository _financeiroRepo = new FinanceiroRepository();

        // KPI values for chart
        private decimal _totalPendente;
        private decimal _totalAtrasado;
        private decimal _totalRecebido;

        // UI Components
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;

        private TableLayoutPanel tlpStats;
        private Panel pnlPendente;
        private Label lblPendenteVal;
        private Label lblPendenteTitle;
        private Panel pnlAtrasado;
        private Label lblAtrasadoVal;
        private Label lblAtrasadoTitle;
        private Panel pnlRecebido;
        private Label lblRecebidoVal;
        private Label lblRecebidoTitle;

        private Panel pnlFilters;
        private Label lblCliente;
        private ComboBox cboCliente;
        private Label lblStatus;
        private ComboBox cboStatus;

        private DataGridView dgvContas;
        private Button btnReceber;
        private Button btnNovoLancamento;
        private Button btnFechar;
        private Panel pnlChart;

        public FrmContasReceber()
        {
            InitializeComponent();
            ApplyTheme();
            LoadClientes();
            LoadData();
            BindEvents();
        }

        private void InitializeComponent()
        {
            this.Text = "Controle de Contas a Receber";
            this.Size = new Size(1120, 680);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Header Panel
            pnlHeader = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(1120, 70),
                BackColor = UITheme.BgCard
            };

            lblTitle = new Label
            {
                Text = "Contas a Receber (Fiado / Crediário)",
                Location = new Point(25, 12),
                Size = new Size(450, 28),
                Font = new Font("Segoe UI", 16F, FontStyle.Bold)
            };

            lblSubtitle = new Label
            {
                Text = "Monitore as pendências dos clientes, filtre inadimplência e receba pagamentos vinculados ao caixa.",
                Location = new Point(25, 42),
                Size = new Size(1000, 18),
                Font = new Font("Segoe UI", 9F)
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);

            // Stats Panels (Consolidated Metrics)
            tlpStats = new TableLayoutPanel
            {
                Location = new Point(25, 85),
                Size = new Size(790, 90),
                ColumnCount = 3,
                RowCount = 1
            };
            tlpStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tlpStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tlpStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

            pnlPendente = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 10, 0) };
            lblPendenteVal = new Label { Text = "R$ 0,00", Location = new Point(15, 15), Size = new Size(220, 30), Font = new Font("Segoe UI", 18F, FontStyle.Bold) };
            lblPendenteTitle = new Label { Text = "A Receber (Pendente)", Location = new Point(15, 50), Size = new Size(220, 20), Font = UITheme.FontBody };
            pnlPendente.Controls.Add(lblPendenteVal);
            pnlPendente.Controls.Add(lblPendenteTitle);

            pnlAtrasado = new Panel { Dock = DockStyle.Fill, Margin = new Padding(5, 0, 5, 0) };
            lblAtrasadoVal = new Label { Text = "R$ 0,00", Location = new Point(15, 15), Size = new Size(220, 30), Font = new Font("Segoe UI", 18F, FontStyle.Bold) };
            lblAtrasadoTitle = new Label { Text = "Vencidas (Inadimplência)", Location = new Point(15, 50), Size = new Size(220, 20), Font = UITheme.FontBody };
            pnlAtrasado.Controls.Add(lblAtrasadoVal);
            pnlAtrasado.Controls.Add(lblAtrasadoTitle);

            pnlRecebido = new Panel { Dock = DockStyle.Fill, Margin = new Padding(10, 0, 0, 0) };
            lblRecebidoVal = new Label { Text = "R$ 0,00", Location = new Point(15, 15), Size = new Size(220, 30), Font = new Font("Segoe UI", 18F, FontStyle.Bold) };
            lblRecebidoTitle = new Label { Text = "Total Recebido (Baixado)", Location = new Point(15, 50), Size = new Size(220, 20), Font = UITheme.FontBody };
            pnlRecebido.Controls.Add(lblRecebidoVal);
            pnlRecebido.Controls.Add(lblRecebidoTitle);

            tlpStats.Controls.Add(pnlPendente, 0, 0);
            tlpStats.Controls.Add(pnlAtrasado, 1, 0);
            tlpStats.Controls.Add(pnlRecebido, 2, 0);

            // Filters Panel
            pnlFilters = new Panel
            {
                Location = new Point(25, 190),
                Size = new Size(790, 60),
                BackColor = UITheme.BgCard
            };

            lblCliente = new Label { Text = "Filtrar por Cliente:", Location = new Point(15, 20), Size = new Size(120, 20), Font = UITheme.FontBodyBold, TextAlign = ContentAlignment.MiddleLeft };
            cboCliente = new ComboBox { Location = new Point(140, 17), Size = new Size(250, 25), DropDownStyle = ComboBoxStyle.DropDownList };

            lblStatus = new Label { Text = "Status:", Location = new Point(410, 20), Size = new Size(60, 20), Font = UITheme.FontBodyBold, TextAlign = ContentAlignment.MiddleLeft };
            cboStatus = new ComboBox { Location = new Point(480, 17), Size = new Size(150, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cboStatus.Items.AddRange(new object[] { "Todos", "Pendente", "Atrasado", "Pago" });
            cboStatus.SelectedIndex = 0;

            pnlFilters.Controls.Add(lblCliente);
            pnlFilters.Controls.Add(cboCliente);
            pnlFilters.Controls.Add(lblStatus);
            pnlFilters.Controls.Add(cboStatus);

            // Grid
            dgvContas = new DataGridView
            {
                Location = new Point(25, 265),
                Size = new Size(790, 290)
            };

            // Actions Buttons
            btnReceber = new Button { Text = "💰 REGISTRAR RECEBIMENTO", Location = new Point(25, 575), Size = new Size(230, 45) };
            btnNovoLancamento = new Button { Text = "➕ NOVO LANÇAMENTO (DÉBITO)", Location = new Point(270, 575), Size = new Size(240, 45) };
            btnFechar = new Button { Text = "FECHAR", Location = new Point(665, 575), Size = new Size(150, 45) };

            // Chart Panel
            pnlChart = new Panel
            {
                Location = new Point(830, 85),
                Size = new Size(260, 470)
            };

            this.Controls.Add(pnlHeader);
            this.Controls.Add(tlpStats);
            this.Controls.Add(pnlFilters);
            this.Controls.Add(dgvContas);
            this.Controls.Add(btnReceber);
            this.Controls.Add(btnNovoLancamento);
            this.Controls.Add(btnFechar);
            this.Controls.Add(pnlChart);
        }

        private void ApplyTheme()
        {
            this.BackColor = UITheme.BgApp;

            lblTitle.ForeColor = UITheme.TextTitle;
            lblSubtitle.ForeColor = UITheme.TextMuted;

            pnlPendente.BackColor = UITheme.BgCard;
            lblPendenteVal.ForeColor = UITheme.Warning;
            lblPendenteTitle.ForeColor = UITheme.TextMuted;

            pnlAtrasado.BackColor = UITheme.BgCard;
            lblAtrasadoVal.ForeColor = UITheme.Danger;
            lblAtrasadoTitle.ForeColor = UITheme.TextMuted;

            pnlRecebido.BackColor = UITheme.BgCard;
            lblRecebidoVal.ForeColor = UITheme.Success;
            lblRecebidoTitle.ForeColor = UITheme.TextMuted;

            // Arredondamento dos cards de estatísticas
            pnlPendente.Paint += (s, e) => UITheme.DrawRoundedPanel(e.Graphics, pnlPendente.ClientRectangle, UITheme.BgCard, 10, UITheme.ElementBorder);
            pnlAtrasado.Paint += (s, e) => UITheme.DrawRoundedPanel(e.Graphics, pnlAtrasado.ClientRectangle, UITheme.BgCard, 10, UITheme.ElementBorder);
            pnlRecebido.Paint += (s, e) => UITheme.DrawRoundedPanel(e.Graphics, pnlRecebido.ClientRectangle, UITheme.BgCard, 10, UITheme.ElementBorder);

            pnlFilters.BackColor = UITheme.BgCard;
            lblCliente.ForeColor = UITheme.TextMuted;
            cboCliente.BackColor = UITheme.BgSidebar;
            cboCliente.ForeColor = UITheme.TextTitle;

            lblStatus.ForeColor = UITheme.TextMuted;
            cboStatus.BackColor = UITheme.BgSidebar;
            cboStatus.ForeColor = UITheme.TextTitle;

            pnlFilters.Paint += (s, e) => UITheme.DrawRoundedPanel(e.Graphics, pnlFilters.ClientRectangle, UITheme.BgCard, 10, UITheme.ElementBorder);

            UITheme.FormatGrid(dgvContas);
            UITheme.FormatSaaSButton(btnReceber, true);
            btnReceber.BackColor = UITheme.Success;
            
            UITheme.FormatSaaSButton(btnNovoLancamento, true);
            btnNovoLancamento.BackColor = UITheme.Primary;

            UITheme.FormatSaaSButton(btnFechar, false);
            btnFechar.BackColor = UITheme.ElementBorder;

            pnlChart.BackColor = UITheme.BgCard;
            pnlChart.Paint += PnlChart_Paint;
        }

        private async void LoadClientes()
        {
            try
            {
                var list = await _clienteRepo.BuscarTodosAsync();
                var listCbo = new List<Cliente>();
                listCbo.Add(new Cliente { Id = 0, Nome = "-- Todos os Clientes --" });
                listCbo.AddRange(list);

                cboCliente.DataSource = listCbo;
                cboCliente.DisplayMember = "Nome";
                cboCliente.ValueMember = "Id";
                cboCliente.SelectedIndex = 0;
            }
            catch { }
        }

        private void LoadData()
        {
            int? idCliente = null;
            if (cboCliente.SelectedValue != null)
            {
                if (cboCliente.SelectedValue is int val && val > 0)
                {
                    idCliente = val;
                }
                else if (cboCliente.SelectedValue is MasterServicePro.Models.Cliente c && c.Id > 0)
                {
                    idCliente = c.Id;
                }
                else if (int.TryParse(cboCliente.SelectedValue.ToString(), out int parsed) && parsed > 0)
                {
                    idCliente = parsed;
                }
            }

            string status = cboStatus.Text;
            var list = _repository.Listar(idCliente, status);

            dgvContas.DataSource = list;
            FormatGridColumns();
            CalculateKPIs(list);
        }

        private void FormatGridColumns()
        {
            if (dgvContas.Columns.Count > 0)
            {
                foreach (DataGridViewColumn c in dgvContas.Columns) c.Visible = false;

                dgvContas.Columns["ClienteNome"].Visible = true;
                dgvContas.Columns["ClienteNome"].HeaderText = "Cliente";
                dgvContas.Columns["ClienteNome"].FillWeight = 160;

                dgvContas.Columns["Descricao"].Visible = true;
                dgvContas.Columns["Descricao"].HeaderText = "Descrição";
                dgvContas.Columns["Descricao"].FillWeight = 200;

                dgvContas.Columns["ValorTotal"].Visible = true;
                dgvContas.Columns["ValorTotal"].HeaderText = "Total (R$)";
                dgvContas.Columns["ValorTotal"].DefaultCellStyle.Format = "C2";
                dgvContas.Columns["ValorTotal"].FillWeight = 100;

                dgvContas.Columns["ValorPago"].Visible = true;
                dgvContas.Columns["ValorPago"].HeaderText = "Pago (R$)";
                dgvContas.Columns["ValorPago"].DefaultCellStyle.Format = "C2";
                dgvContas.Columns["ValorPago"].FillWeight = 100;

                dgvContas.Columns["ValorRestante"].Visible = true;
                dgvContas.Columns["ValorRestante"].HeaderText = "Restante (R$)";
                dgvContas.Columns["ValorRestante"].DefaultCellStyle.Format = "C2";
                dgvContas.Columns["ValorRestante"].FillWeight = 100;

                dgvContas.Columns["DataVencimento"].Visible = true;
                dgvContas.Columns["DataVencimento"].HeaderText = "Vencimento";
                dgvContas.Columns["DataVencimento"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvContas.Columns["DataVencimento"].FillWeight = 110;

                dgvContas.Columns["Status"].Visible = true;
                dgvContas.Columns["Status"].HeaderText = "Status";
                dgvContas.Columns["Status"].FillWeight = 100;
            }
        }

        private void CalculateKPIs(List<ContaReceber> list)
        {
            _totalPendente = 0;
            _totalAtrasado = 0;
            _totalRecebido = 0;

            // Para os KPIs calculamos sobre TODAS as contas filtradas
            foreach (var item in list)
            {
                if (item.Status == "Pendente")
                    _totalPendente += item.ValorRestante;
                else if (item.Status == "Atrasado")
                    _totalAtrasado += item.ValorRestante;
                
                _totalRecebido += item.ValorPago;
            }

            lblPendenteVal.Text = _totalPendente.ToString("C2");
            lblAtrasadoVal.Text = _totalAtrasado.ToString("C2");
            lblRecebidoVal.Text = _totalRecebido.ToString("C2");

            pnlChart.Invalidate();
        }

        private void PnlChart_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Draw Card background & border
            UITheme.DrawRoundedPanel(g, pnlChart.ClientRectangle, UITheme.BgCard, 10, UITheme.ElementBorder);

            // Chart Title
            Font fontTitle = new Font("Segoe UI", 10F, FontStyle.Bold);
            Font fontText = new Font("Segoe UI", 9F, FontStyle.Regular);
            Font fontTextBold = new Font("Segoe UI", 9F, FontStyle.Bold);
            Font fontCenter = new Font("Segoe UI", 8F, FontStyle.Bold);

            string title = "COMPOSIÇÃO DE CONTAS";
            SizeF titleSize = g.MeasureString(title, fontTitle);
            g.DrawString(title, fontTitle, new SolidBrush(UITheme.TextTitle), (pnlChart.Width - titleSize.Width) / 2, 18);

            // Calculate dimensions
            int centerX = pnlChart.Width / 2;
            int centerY = 160;
            int outerRadius = 75;
            int innerRadius = 45;

            Rectangle rectOuter = new Rectangle(centerX - outerRadius, centerY - outerRadius, outerRadius * 2, outerRadius * 2);

            decimal total = _totalRecebido + _totalPendente + _totalAtrasado;

            if (total == 0)
            {
                // Draw a beautiful placeholder gray donut
                using (var brush = new SolidBrush(Color.FromArgb(60, 60, 65)))
                {
                    g.FillPie(brush, rectOuter, 0, 360);
                }
                
                // Draw Inner Circle to clear center
                using (var brush = new SolidBrush(UITheme.BgCard))
                {
                    g.FillEllipse(brush, centerX - innerRadius, centerY - innerRadius, innerRadius * 2, innerRadius * 2);
                }

                // Draw center text
                string emptyText1 = "100%";
                string emptyText2 = "Sem Pendências";
                SizeF s1 = g.MeasureString(emptyText1, fontTitle);
                SizeF s2 = g.MeasureString(emptyText2, fontCenter);
                g.DrawString(emptyText1, fontTitle, Brushes.Gray, centerX - s1.Width / 2, centerY - 15);
                g.DrawString(emptyText2, fontCenter, Brushes.Gray, centerX - s2.Width / 2, centerY + 3);
            }
            else
            {
                float angleRecebido = (float)(360 * (_totalRecebido / total));
                float anglePendente = (float)(360 * (_totalPendente / total));
                float angleAtrasado = (float)(360 * (_totalAtrasado / total));

                float startAngle = -90f; // Start at top

                // Recebido (Success / Green)
                if (angleRecebido > 0)
                {
                    using (var brush = new SolidBrush(UITheme.Success))
                    {
                        g.FillPie(brush, rectOuter, startAngle, angleRecebido);
                    }
                    startAngle += angleRecebido;
                }

                // Pendente (Warning / Yellow)
                if (anglePendente > 0)
                {
                    using (var brush = new SolidBrush(UITheme.Warning))
                    {
                        g.FillPie(brush, rectOuter, startAngle, anglePendente);
                    }
                    startAngle += anglePendente;
                }

                // Atrasado (Danger / Red)
                if (angleAtrasado > 0)
                {
                    using (var brush = new SolidBrush(UITheme.Danger))
                    {
                        g.FillPie(brush, rectOuter, startAngle, angleAtrasado);
                    }
                }

                // Inner circle
                using (var brush = new SolidBrush(UITheme.BgCard))
                {
                    g.FillEllipse(brush, centerX - innerRadius, centerY - innerRadius, innerRadius * 2, innerRadius * 2);
                }

                // Center info (Percentage of bad debt)
                decimal inadinRate = total > 0 ? (_totalAtrasado / total) * 100 : 0;
                string centerText1 = $"{inadinRate:N0}%";
                string centerText2 = "Inadimplente";
                SizeF s1 = g.MeasureString(centerText1, fontTitle);
                SizeF s2 = g.MeasureString(centerText2, fontCenter);
                g.DrawString(centerText1, fontTitle, new SolidBrush(UITheme.Danger), centerX - s1.Width / 2, centerY - 15);
                g.DrawString(centerText2, fontCenter, new SolidBrush(UITheme.TextMuted), centerX - s2.Width / 2, centerY + 3);
            }

            // Legend listing below the donut
            int legendY = 275;
            int legendX = 20;

            DrawLegendItem(g, legendX, legendY, UITheme.Success, "Recebido (Baixado)", _totalRecebido, total, fontText, fontTextBold);
            legendY += 55;
            DrawLegendItem(g, legendX, legendY, UITheme.Warning, "A Receber (Em dia)", _totalPendente, total, fontText, fontTextBold);
            legendY += 55;
            DrawLegendItem(g, legendX, legendY, UITheme.Danger, "Inadimplência (Atrasadas)", _totalAtrasado, total, fontText, fontTextBold);
        }

        private void DrawLegendItem(Graphics g, int x, int y, Color color, string label, decimal val, decimal total, Font fontText, Font fontTextBold)
        {
            // Indicator colored dot
            using (var brush = new SolidBrush(color))
            {
                g.FillEllipse(brush, x, y + 3, 10, 10);
            }

            // Text Label
            g.DrawString(label, fontText, new SolidBrush(UITheme.TextMuted), x + 18, y);

            // Value and percentage
            decimal pct = total > 0 ? (val / total) * 100 : 0;
            string valStr = $"{val:C2} ({pct:N1}%)";
            g.DrawString(valStr, fontTextBold, new SolidBrush(UITheme.TextTitle), x + 18, y + 18);
        }

        private void BindEvents()
        {
            cboCliente.SelectedIndexChanged += (s, e) => LoadData();
            cboStatus.SelectedIndexChanged += (s, e) => LoadData();

            btnFechar.Click += (s, e) => this.Close();
            btnReceber.Click += BtnReceber_Click;
            btnNovoLancamento.Click += BtnNovoLancamento_Click;
        }

        private void BtnReceber_Click(object sender, EventArgs e)
        {
            if (dgvContas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um lançamento para registrar recebimento.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var conta = (ContaReceber)dgvContas.SelectedRows[0].DataBoundItem;
            if (conta.Status == "Pago")
            {
                MessageBox.Show("Esta conta já está totalmente paga.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var caixa = _financeiroRepo.ObterCaixaAberto();
            if (caixa == null)
            {
                MessageBox.Show("O caixa está fechado! Abra o caixa antes de receber valores.", "Caixa Fechado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Abre janela de recebimento
            using (var prompt = new FrmPromptRecebimento(conta.ValorRestante))
            {
                if (prompt.ShowDialog() == DialogResult.OK)
                {
                    decimal valor = prompt.ValorInformado;
                    string metodo = prompt.MetodoPagamento;

                    try
                    {
                        _repository.DarBaixa(conta.Id, valor, metodo, caixa.Id);
                        FrmNotification.ShowSuccess("Recebimento lançado com sucesso!", "Sucesso");
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        FrmNotification.ShowError("Erro ao registrar baixa: " + ex.Message, "Erro");
                    }
                }
            }
        }

        private void BtnNovoLancamento_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmNovoLancamentoReceber())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _repository.Inserir(frm.NovoLancamento);
                        FrmNotification.ShowSuccess("Lançamento de débito efetuado com sucesso!", "Lançado");
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        FrmNotification.ShowError("Erro ao lançar conta: " + ex.Message, "Erro");
                    }
                }
            }
        }
    }

    // Modal auxiliar para entrada de valor recebido
    public class FrmPromptRecebimento : Form
    {
        public decimal ValorInformado { get; private set; }
        public string MetodoPagamento { get; private set; }

        private TextBox txtValor;
        private ComboBox cboMetodo;
        private Button btnOk;
        private Button btnCancel;

        public FrmPromptRecebimento(decimal valorMaximo)
        {
            this.Text = "Receber Valor";
            this.Size = new Size(350, 240);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            UITheme.ApplyModalWindowStyle(this);
            this.BackColor = UITheme.BgModal;

            Panel pnlMain = new Panel { Location = new Point(20, 20), Size = new Size(310, 200), BackColor = UITheme.BgCard };
            this.Controls.Add(pnlMain);
            pnlMain.BringToFront();

            Label lblTitle = new Label { Text = "Registrar Recebimento", Location = new Point(15, 15), Size = new Size(280, 20), Font = UITheme.FontBodyBold, ForeColor = UITheme.TextTitle };
            pnlMain.Controls.Add(lblTitle);

            Label lblValor = new Label { Text = $"Valor a Pagar (Máx: {valorMaximo:C2}):", Location = new Point(15, 45), Size = new Size(280, 20), Font = UITheme.FontBody, ForeColor = UITheme.TextMuted };
            txtValor = new TextBox { Location = new Point(15, 65), Size = new Size(280, 30), Font = new Font("Segoe UI", 12F), Text = valorMaximo.ToString("N2"), BackColor = UITheme.BgSidebar, ForeColor = UITheme.TextTitle, BorderStyle = BorderStyle.FixedSingle };
            pnlMain.Controls.Add(lblValor);
            pnlMain.Controls.Add(txtValor);

            Label lblMetodo = new Label { Text = "Método de Recebimento:", Location = new Point(15, 105), Size = new Size(280, 20), Font = UITheme.FontBody, ForeColor = UITheme.TextMuted };
            cboMetodo = new ComboBox { Location = new Point(15, 125), Size = new Size(280, 30), DropDownStyle = ComboBoxStyle.DropDownList, Font = UITheme.FontBody, BackColor = UITheme.BgSidebar, ForeColor = UITheme.TextTitle };
            cboMetodo.Items.AddRange(new object[] { "Dinheiro", "Pix", "Cartão de Crédito", "Cartão de Débito" });
            cboMetodo.SelectedIndex = 0;
            pnlMain.Controls.Add(lblMetodo);
            pnlMain.Controls.Add(cboMetodo);

            btnOk = new Button { Text = "CONFIRMAR", Location = new Point(165, 165), Size = new Size(130, 32) };
            UITheme.FormatSaaSButton(btnOk, true);
            btnOk.BackColor = UITheme.Success;
            btnOk.Click += (s, e) =>
            {
                if (decimal.TryParse(txtValor.Text, out decimal val) && val > 0)
                {
                    if (val > valorMaximo)
                    {
                        MessageBox.Show("O valor pago não pode ser maior que o saldo restante da conta.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    ValorInformado = val;
                    MetodoPagamento = cboMetodo.Text;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Digite um valor válido.");
                }
            };

            btnCancel = new Button { Text = "CANCELAR", Location = new Point(15, 165), Size = new Size(130, 32) };
            UITheme.FormatSaaSButton(btnCancel, false);
            btnCancel.BackColor = UITheme.Danger;
            btnCancel.Click += (s, e) => { this.Close(); };

            pnlMain.Controls.Add(btnOk);
            pnlMain.Controls.Add(btnCancel);
        }
    }

    // Modal auxiliar para criar lançamento de débito fiado manualmente
    public class FrmNovoLancamentoReceber : Form
    {
        public ContaReceber NovoLancamento { get; private set; }

        private ComboBox cboCliente;
        private TextBox txtDescricao;
        private TextBox txtValor;
        private DateTimePicker dtpVencimento;
        private Button btnOk;
        private Button btnCancel;

        public FrmNovoLancamentoReceber()
        {
            this.Text = "Novo Lançamento";
            this.Size = new Size(420, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            UITheme.ApplyModalWindowStyle(this);
            this.BackColor = UITheme.BgModal;

            Panel pnlMain = new Panel { Location = new Point(20, 20), Size = new Size(380, 340), BackColor = UITheme.BgCard };
            this.Controls.Add(pnlMain);
            pnlMain.BringToFront();

            Label lblTitle = new Label { Text = "Novo Débito Fiado", Location = new Point(15, 15), Size = new Size(350, 20), Font = UITheme.FontTitle, ForeColor = UITheme.TextTitle };
            pnlMain.Controls.Add(lblTitle);

            Label lblCliente = new Label { Text = "Cliente Devedor:", Location = new Point(15, 55), Size = new Size(350, 20), Font = UITheme.FontBody, ForeColor = UITheme.TextMuted };
            cboCliente = new ComboBox { Location = new Point(15, 75), Size = new Size(350, 30), DropDownStyle = ComboBoxStyle.DropDownList, Font = UITheme.FontBody, BackColor = UITheme.BgSidebar, ForeColor = UITheme.TextTitle };
            pnlMain.Controls.Add(lblCliente);
            pnlMain.Controls.Add(cboCliente);

            Label lblDesc = new Label { Text = "Descrição / Motivo do Débito:", Location = new Point(15, 115), Size = new Size(350, 20), Font = UITheme.FontBody, ForeColor = UITheme.TextMuted };
            txtDescricao = new TextBox { Location = new Point(15, 135), Size = new Size(350, 30), Font = UITheme.FontBody, BackColor = UITheme.BgSidebar, ForeColor = UITheme.TextTitle, BorderStyle = BorderStyle.FixedSingle };
            pnlMain.Controls.Add(lblDesc);
            pnlMain.Controls.Add(txtDescricao);

            Label lblValor = new Label { Text = "Valor Total do Débito (R$):", Location = new Point(15, 175), Size = new Size(160, 20), Font = UITheme.FontBody, ForeColor = UITheme.TextMuted };
            txtValor = new TextBox { Location = new Point(15, 195), Size = new Size(160, 30), Font = new Font("Segoe UI", 12F), BackColor = UITheme.BgSidebar, ForeColor = UITheme.TextTitle, BorderStyle = BorderStyle.FixedSingle };
            pnlMain.Controls.Add(lblValor);
            pnlMain.Controls.Add(txtValor);

            Label lblVenc = new Label { Text = "Data de Vencimento:", Location = new Point(200, 175), Size = new Size(165, 20), Font = UITheme.FontBody, ForeColor = UITheme.TextMuted };
            dtpVencimento = new DateTimePicker { Location = new Point(200, 195), Size = new Size(165, 30), Format = DateTimePickerFormat.Short, Font = UITheme.FontBody };
            pnlMain.Controls.Add(lblVenc);
            pnlMain.Controls.Add(dtpVencimento);

            btnOk = new Button { Text = "LANÇAR DÉBITO", Location = new Point(200, 260), Size = new Size(165, 45) };
            UITheme.FormatSaaSButton(btnOk, true);
            btnOk.Click += BtnOk_Click;

            btnCancel = new Button { Text = "CANCELAR", Location = new Point(15, 260), Size = new Size(165, 45) };
            UITheme.FormatSaaSButton(btnCancel, false);
            btnCancel.BackColor = UITheme.Danger;
            btnCancel.Click += (s, e) => { this.Close(); };

            pnlMain.Controls.Add(btnOk);
            pnlMain.Controls.Add(btnCancel);

            LoadClientesList();
        }

        private async void LoadClientesList()
        {
            try
            {
                var list = await new ClienteRepository().BuscarTodosAsync();
                cboCliente.DataSource = list;
                cboCliente.DisplayMember = "Nome";
                cboCliente.ValueMember = "Id";
                cboCliente.SelectedIndex = -1;
            }
            catch { }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (cboCliente.SelectedValue == null)
            {
                MessageBox.Show("Selecione um cliente.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescricao.Text))
            {
                MessageBox.Show("Informe uma descrição para o débito.");
                return;
            }

            if (!decimal.TryParse(txtValor.Text, out decimal val) || val <= 0)
            {
                MessageBox.Show("Informe um valor de débito válido.");
                return;
            }

            NovoLancamento = new ContaReceber
            {
                IdCliente = (int)cboCliente.SelectedValue,
                Descricao = txtDescricao.Text.Trim(),
                ValorTotal = val,
                ValorPago = 0,
                DataLancamento = DateTime.Now,
                DataVencimento = dtpVencimento.Value,
                Status = dtpVencimento.Value < DateTime.Today ? "Atrasado" : "Pendente"
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
