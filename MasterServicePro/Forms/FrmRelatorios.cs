using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public class FrmRelatorios : Form
    {
        private readonly VendaRepository _vendaRepo = new VendaRepository();
        private readonly OrdemServicoRepository _osRepo = new OrdemServicoRepository();
        private readonly ProdutoRepository _produtoRepo = new ProdutoRepository();
        private readonly FinanceiroRepository _financeiroRepo = new FinanceiroRepository();
        private readonly TecnicoRepository _tecnicoRepo = new TecnicoRepository();
        private readonly AuditoriaRepository _auditoriaRepo = new AuditoriaRepository();

        // Printing helper state
        private int _currentRowIndex = 0;

        // UI Controls
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private Panel pnlDateFilter;
        private Label lblInicio;
        private DateTimePicker dtpInicio;
        private Label lblFim;
        private DateTimePicker dtpFim;
        private Button btnFiltrar;
        private Button btnExportar;
        private Button btnImprimir;

        private FlowLayoutPanel pnlTabButtons;
        private Panel pnlTabContent;
        private Panel activeTabPanel;
        private Button activeTabButton;

        // Tab: Vendas
        private Panel tpVendas;
        private TableLayoutPanel tlpVendasStats;
        private Panel pnlVendasTotal;
        private Label lblVendasTotalVal;
        private Label lblVendasTotalTitle;
        private Panel pnlVendasQtd;
        private Label lblVendasQtdVal;
        private Label lblVendasQtdTitle;
        private Panel pnlVendasTicket;
        private Label lblVendasTicketVal;
        private Label lblVendasTicketTitle;
        private DataGridView dgvVendas;

        // Tab: Categorias
        private Panel tpCategorias;
        private TableLayoutPanel tlpCategoriasStats;
        private Panel pnlCatTotal;
        private Label lblCatTotalVal;
        private Label lblCatTotalTitle;
        private DataGridView dgvCategorias;

        // Tab: Custos e Lucros
        private Panel tpCustosLucros;
        private Panel pnlCustosLucrosFilters;
        private Label lblTecnico;
        private ComboBox cboTecnicos;
        private TableLayoutPanel tlpCustosLucrosStats;
        private Panel pnlTotalCustoPecas;
        private Label lblTotalCustoPecasVal;
        private Label lblTotalCustoPecasTitle;
        private Panel pnlTotalFaturamento;
        private Label lblTotalFaturamentoVal;
        private Label lblTotalFaturamentoTitle;
        private Panel pnlLucroLiquido;
        private Label lblLucroLiquidoVal;
        private Label lblLucroLiquidoTitle;
        private DataGridView dgvCustosLucros;

        // Tab: Estoque
        private Panel tpEstoque;
        private TableLayoutPanel tlpEstoqueStats;
        private Panel pnlEstoqueQtd;
        private Label lblEstoqueQtdVal;
        private Label lblEstoqueQtdTitle;
        private Panel pnlEstoqueCusto;
        private Label lblEstoqueCustoVal;
        private Label lblEstoqueCustoTitle;
        private DataGridView dgvEstoque;

        // Tab: Financeiro
        private Panel tpFinanceiro;
        private Panel pnlFinanceiroFilters;
        private Label lblTipoFin;
        private ComboBox cboTipoFin;
        private Label lblSubcatFin;
        private ComboBox cboSubcatFin;
        private TableLayoutPanel tlpFinanceiroStats;
        private Panel pnlFinEntradas;
        private Label lblFinEntradasVal;
        private Label lblFinEntradasTitle;
        private Panel pnlFinSaidas;
        private Label lblFinSaidasVal;
        private Label lblFinSaidasTitle;
        private Panel pnlFinSaldo;
        private Label lblFinSaldoVal;
        private Label lblFinSaldoTitle;
        private DataGridView dgvFinanceiro;

        // Tab: Auditoria
        private Panel tpAuditoria;
        private Panel pnlAuditoriaFilters;
        private Label lblAuditoriaBusca;
        private TextBox txtAuditoriaBusca;
        private TableLayoutPanel tlpAuditoriaStats;
        private Panel pnlAuditTotal;
        private Label lblAuditTotalVal;
        private Label lblAuditTotalTitle;
        private Panel pnlAuditAdmins;
        private Label lblAuditAdminsVal;
        private Label lblAuditAdminsTitle;
        private DataGridView dgvAuditoria;

        public FrmRelatorios()
        {
            InitializeComponent();
            ApplyTheme();
            BindEvents();
            LoadDataAsync();
        }

        private void InitializeComponent()
        {
            this.Text = "Relatórios e Estatísticas Gerais";
            this.Size = new Size(960, 680);

            // 1. Header Panel
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = UITheme.BgCard
            };

            lblTitle = new Label
            {
                Text = "Relatórios Gerenciais",
                Location = new Point(25, 12),
                Size = new Size(350, 28),
                Font = new Font("Segoe UI", 16F, FontStyle.Bold)
            };

            lblSubtitle = new Label
            {
                Text = "Visualize, exporte e imprima resumos de vendas, comissões, estoque, caixa e logs de auditoria.",
                Location = new Point(25, 42),
                Size = new Size(700, 18),
                Font = new Font("Segoe UI", 9F)
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);

            // 2. Global Date Filter Panel
            pnlDateFilter = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = UITheme.BgCard
            };

            lblInicio = new Label { Text = "De:", Location = new Point(30, 18), Size = new Size(30, 20), TextAlign = ContentAlignment.MiddleLeft };
            dtpInicio = new DateTimePicker { Location = new Point(65, 16), Size = new Size(120, 25), Format = DateTimePickerFormat.Short };
            dtpInicio.Value = DateTime.Today.AddDays(-30);

            lblFim = new Label { Text = "Até:", Location = new Point(205, 18), Size = new Size(35, 20), TextAlign = ContentAlignment.MiddleLeft };
            dtpFim = new DateTimePicker { Location = new Point(245, 16), Size = new Size(120, 25), Format = DateTimePickerFormat.Short };

            btnFiltrar = new Button { Text = "🔍 Filtrar", Location = new Point(385, 13), Size = new Size(100, 32) };
            
            FlowLayoutPanel pnlExportPrint = new FlowLayoutPanel { Dock = DockStyle.Right, FlowDirection = FlowDirection.LeftToRight, Width = 350, Padding = new Padding(0, 13, 10, 0) };
            btnExportar = new Button { Text = "📥 Exportar Excel", Size = new Size(150, 32) };
            btnImprimir = new Button { Text = "🖨️ Imprimir", Size = new Size(150, 32) };
            pnlExportPrint.Controls.Add(btnExportar);
            pnlExportPrint.Controls.Add(btnImprimir);

            pnlDateFilter.Controls.Add(lblInicio);
            pnlDateFilter.Controls.Add(dtpInicio);
            pnlDateFilter.Controls.Add(lblFim);
            pnlDateFilter.Controls.Add(dtpFim);
            pnlDateFilter.Controls.Add(btnFiltrar);
            pnlDateFilter.Controls.Add(pnlExportPrint);

            // 3. Tab Buttons Panel
            pnlTabButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 45,
                BackColor = UITheme.BgApp,
                Padding = new Padding(30, 5, 0, 0)
            };

            // 4. Tab Content Panel
            pnlTabContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UITheme.BgApp
            };

            // Helper to create tab button
            Button CreateTabButton(string text)
            {
                var btn = new Button
                {
                    Text = text,
                    Height = 35,
                    AutoSize = true,
                    Padding = new Padding(10, 0, 10, 0),
                    FlatStyle = FlatStyle.Flat,
                    Font = UITheme.FontBodyBold,
                    Cursor = Cursors.Hand,
                    Margin = new Padding(0, 0, 5, 0)
                };
                btn.FlatAppearance.BorderSize = 0;
                return btn;
            }

            Button btnTabVendas = CreateTabButton("🛍️ Vendas por Período");
            Button btnTabCategorias = CreateTabButton("🏷️ Categorias");
            Button btnTabCustosLucros = CreateTabButton("⚙️ Custos e Lucros");
            Button btnTabEstoque = CreateTabButton("📦 Estoque Mínimo");
            Button btnTabFin = CreateTabButton("💰 Fluxo de Caixa");
            Button btnTabAuditoria = CreateTabButton("📋 Auditoria");

            pnlTabButtons.Controls.Add(btnTabVendas);
            pnlTabButtons.Controls.Add(btnTabCategorias);
            pnlTabButtons.Controls.Add(btnTabCustosLucros);
            pnlTabButtons.Controls.Add(btnTabEstoque);
            pnlTabButtons.Controls.Add(btnTabFin);
            pnlTabButtons.Controls.Add(btnTabAuditoria);

            // --- TAB: VENDAS ---
            tpVendas = new Panel { Dock = DockStyle.Fill, Visible = false, Padding = new Padding(20, 10, 20, 20) };
            tlpVendasStats = new TableLayoutPanel { Dock = DockStyle.Top, Height = 110, ColumnCount = 3, RowCount = 1 };
            tlpVendasStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tlpVendasStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tlpVendasStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

            pnlVendasTotal = new Panel { Dock = DockStyle.Fill, Margin = new Padding(10) };
            lblVendasTotalVal = new Label { Text = "R$ 0,00", Location = new Point(15, 10), Size = new Size(250, 25), Font = new Font("Segoe UI", 14F, FontStyle.Bold) };
            lblVendasTotalTitle = new Label { Text = "Total Vendas (Produtos)", Location = new Point(15, 38), Size = new Size(250, 18) };
            pnlVendasTotal.Controls.Add(lblVendasTotalVal);
            pnlVendasTotal.Controls.Add(lblVendasTotalTitle);

            pnlVendasQtd = new Panel { Dock = DockStyle.Fill, Margin = new Padding(10) };
            lblVendasQtdVal = new Label { Text = "0", Location = new Point(15, 10), Size = new Size(250, 25), Font = new Font("Segoe UI", 14F, FontStyle.Bold) };
            lblVendasQtdTitle = new Label { Text = "Quantidade Vendas", Location = new Point(15, 38), Size = new Size(250, 18) };
            pnlVendasQtd.Controls.Add(lblVendasQtdVal);
            pnlVendasQtd.Controls.Add(lblVendasQtdTitle);

            pnlVendasTicket = new Panel { Dock = DockStyle.Fill, Margin = new Padding(10) };
            lblVendasTicketVal = new Label { Text = "R$ 0,00", Location = new Point(15, 10), Size = new Size(250, 25), Font = new Font("Segoe UI", 14F, FontStyle.Bold) };
            lblVendasTicketTitle = new Label { Text = "Tíquete Médio", Location = new Point(15, 38), Size = new Size(250, 18) };
            pnlVendasTicket.Controls.Add(lblVendasTicketVal);
            pnlVendasTicket.Controls.Add(lblVendasTicketTitle);

            tlpVendasStats.Controls.Add(pnlVendasTotal, 0, 0);
            tlpVendasStats.Controls.Add(pnlVendasQtd, 1, 0);
            tlpVendasStats.Controls.Add(pnlVendasTicket, 2, 0);

            dgvVendas = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = UITheme.BgCard };
            Panel pnlWrapVendas = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 10, 10, 10) };
            pnlWrapVendas.Controls.Add(dgvVendas);

            Panel pnlVendasActions = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = UITheme.BgApp, Padding = new Padding(10, 5, 10, 5) };
            Button btnEstornarVenda = new Button { Text = "🔄 Cancelar / Estornar Venda", Size = new Size(250, 35), Dock = DockStyle.Right };
            UITheme.FormatSaaSButton(btnEstornarVenda, false);
            btnEstornarVenda.BackColor = UITheme.Danger;
            btnEstornarVenda.Click += BtnEstornarVenda_Click;
            pnlVendasActions.Controls.Add(btnEstornarVenda);

            tpVendas.Controls.Add(pnlWrapVendas);
            tpVendas.Controls.Add(pnlVendasActions);
            tpVendas.Controls.Add(tlpVendasStats);

            // --- TAB: CATEGORIAS ---
            tpCategorias = new Panel { Dock = DockStyle.Fill, Visible = false, Padding = new Padding(20, 10, 20, 20) };
            tlpCategoriasStats = new TableLayoutPanel { Dock = DockStyle.Top, Height = 110, ColumnCount = 1, RowCount = 1 };
            tlpCategoriasStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            pnlCatTotal = new Panel { Dock = DockStyle.Fill, Margin = new Padding(10) };
            lblCatTotalVal = new Label { Text = "R$ 0,00", Location = new Point(15, 10), Size = new Size(250, 25), Font = new Font("Segoe UI", 14F, FontStyle.Bold) };
            lblCatTotalTitle = new Label { Text = "Faturamento Total (Competência)", Location = new Point(15, 38), Size = new Size(250, 18) };
            pnlCatTotal.Controls.Add(lblCatTotalVal);
            pnlCatTotal.Controls.Add(lblCatTotalTitle);
            
            tlpCategoriasStats.Controls.Add(pnlCatTotal, 0, 0);

            dgvCategorias = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = UITheme.BgCard };
            Panel pnlWrapCategorias = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 10, 10, 10) };
            pnlWrapCategorias.Controls.Add(dgvCategorias);

            tpCategorias.Controls.Add(pnlWrapCategorias);
            tpCategorias.Controls.Add(tlpCategoriasStats);

            // --- TAB: COMISSÕES ---
            // --- TAB: CUSTOS E LUCROS ---
            tpCustosLucros = new Panel { Dock = DockStyle.Fill, Visible = false, Padding = new Padding(20, 10, 20, 20) };
            pnlCustosLucrosFilters = new Panel { Dock = DockStyle.Top, Height = 45, BackColor = UITheme.BgApp };
            lblTecnico = new Label { Text = "Técnico:", Location = new Point(30, 13), Size = new Size(60, 20), TextAlign = ContentAlignment.MiddleLeft };
            cboTecnicos = new ComboBox { Location = new Point(95, 10), Size = new Size(200, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            
            pnlCustosLucrosFilters.Controls.Add(lblTecnico);
            pnlCustosLucrosFilters.Controls.Add(cboTecnicos);

            tlpCustosLucrosStats = new TableLayoutPanel { Dock = DockStyle.Top, Height = 110, ColumnCount = 3, RowCount = 1 };
            tlpCustosLucrosStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tlpCustosLucrosStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tlpCustosLucrosStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

            pnlTotalCustoPecas = new Panel { Dock = DockStyle.Fill, Margin = new Padding(10) };
            lblTotalCustoPecasVal = new Label { Text = "R$ 0,00", Location = new Point(15, 10), Size = new Size(250, 25), Font = new Font("Segoe UI", 14F, FontStyle.Bold) };
            lblTotalCustoPecasTitle = new Label { Text = "Custo Total de Peças", Location = new Point(15, 38), Size = new Size(250, 18) };
            pnlTotalCustoPecas.Controls.Add(lblTotalCustoPecasVal);
            pnlTotalCustoPecas.Controls.Add(lblTotalCustoPecasTitle);

            pnlTotalFaturamento = new Panel { Dock = DockStyle.Fill, Margin = new Padding(10) };
            lblTotalFaturamentoVal = new Label { Text = "R$ 0,00", Location = new Point(15, 10), Size = new Size(250, 25), Font = new Font("Segoe UI", 14F, FontStyle.Bold) };
            lblTotalFaturamentoTitle = new Label { Text = "Faturamento (Mão de Obra)", Location = new Point(15, 38), Size = new Size(250, 18) };
            pnlTotalFaturamento.Controls.Add(lblTotalFaturamentoVal);
            pnlTotalFaturamento.Controls.Add(lblTotalFaturamentoTitle);

            pnlLucroLiquido = new Panel { Dock = DockStyle.Fill, Margin = new Padding(10) };
            lblLucroLiquidoVal = new Label { Text = "R$ 0,00", Location = new Point(15, 10), Size = new Size(250, 25), Font = new Font("Segoe UI", 14F, FontStyle.Bold) };
            lblLucroLiquidoTitle = new Label { Text = "Lucro Líquido Real", Location = new Point(15, 38), Size = new Size(250, 18) };
            pnlLucroLiquido.Controls.Add(lblLucroLiquidoVal);
            pnlLucroLiquido.Controls.Add(lblLucroLiquidoTitle);

            tlpCustosLucrosStats.Controls.Add(pnlTotalCustoPecas, 0, 0);
            tlpCustosLucrosStats.Controls.Add(pnlTotalFaturamento, 1, 0);
            tlpCustosLucrosStats.Controls.Add(pnlLucroLiquido, 2, 0);

            dgvCustosLucros = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = UITheme.BgCard };
            Panel pnlWrapCustosLucros = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 10, 10, 10) };
            pnlWrapCustosLucros.Controls.Add(dgvCustosLucros);
            tpCustosLucros.Controls.Add(pnlWrapCustosLucros);
            tpCustosLucros.Controls.Add(tlpCustosLucrosStats);
            tpCustosLucros.Controls.Add(pnlCustosLucrosFilters);

            // --- TAB: ESTOQUE ---
            tpEstoque = new Panel { Dock = DockStyle.Fill, Visible = false, Padding = new Padding(20, 10, 20, 20) };
            tlpEstoqueStats = new TableLayoutPanel { Dock = DockStyle.Top, Height = 110, ColumnCount = 2, RowCount = 1 };
            tlpEstoqueStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpEstoqueStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            pnlEstoqueQtd = new Panel { Dock = DockStyle.Fill, Margin = new Padding(10) };
            lblEstoqueQtdVal = new Label { Text = "0", Location = new Point(15, 10), Size = new Size(400, 25), Font = new Font("Segoe UI", 14F, FontStyle.Bold) };
            lblEstoqueQtdTitle = new Label { Text = "Produtos em Estoque Crítico", Location = new Point(15, 38), Size = new Size(400, 18) };
            pnlEstoqueQtd.Controls.Add(lblEstoqueQtdVal);
            pnlEstoqueQtd.Controls.Add(lblEstoqueQtdTitle);

            pnlEstoqueCusto = new Panel { Dock = DockStyle.Fill, Margin = new Padding(10) };
            lblEstoqueCustoVal = new Label { Text = "R$ 0,00", Location = new Point(15, 10), Size = new Size(400, 25), Font = new Font("Segoe UI", 14F, FontStyle.Bold) };
            lblEstoqueCustoTitle = new Label { Text = "Custo Estimado de Reposição", Location = new Point(15, 38), Size = new Size(400, 18) };
            pnlEstoqueCusto.Controls.Add(lblEstoqueCustoVal);
            pnlEstoqueCusto.Controls.Add(lblEstoqueCustoTitle);

            tlpEstoqueStats.Controls.Add(pnlEstoqueQtd, 0, 0);
            tlpEstoqueStats.Controls.Add(pnlEstoqueCusto, 1, 0);

            dgvEstoque = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = UITheme.BgCard };
            Panel pnlWrapEstoque = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 10, 10, 10) };
            pnlWrapEstoque.Controls.Add(dgvEstoque);
            tpEstoque.Controls.Add(pnlWrapEstoque);
            tpEstoque.Controls.Add(tlpEstoqueStats);

            // --- TAB: FINANCEIRO ---
            tpFinanceiro = new Panel { Dock = DockStyle.Fill, Visible = false, Padding = new Padding(20, 10, 20, 20) };
            pnlFinanceiroFilters = new Panel { Dock = DockStyle.Top, Height = 45, BackColor = UITheme.BgApp };
            lblTipoFin = new Label { Text = "Tipo:", Location = new Point(30, 13), Size = new Size(40, 20), TextAlign = ContentAlignment.MiddleLeft };
            cboTipoFin = new ComboBox { Location = new Point(75, 10), Size = new Size(150, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cboTipoFin.Items.AddRange(new object[] { "Todas", "Entrada", "Saida" });
            cboTipoFin.SelectedIndex = 0;

            lblSubcatFin = new Label { Text = "Subcategoria:", Location = new Point(245, 13), Size = new Size(90, 20), TextAlign = ContentAlignment.MiddleLeft };
            cboSubcatFin = new ComboBox { Location = new Point(340, 10), Size = new Size(220, 25), DropDownStyle = ComboBoxStyle.DropDownList };

            pnlFinanceiroFilters.Controls.Add(lblTipoFin);
            pnlFinanceiroFilters.Controls.Add(cboTipoFin);
            pnlFinanceiroFilters.Controls.Add(lblSubcatFin);
            pnlFinanceiroFilters.Controls.Add(cboSubcatFin);

            tlpFinanceiroStats = new TableLayoutPanel { Dock = DockStyle.Top, Height = 110, ColumnCount = 3, RowCount = 1 };
            tlpFinanceiroStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tlpFinanceiroStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tlpFinanceiroStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

            pnlFinEntradas = new Panel { Dock = DockStyle.Fill, Margin = new Padding(10) };
            lblFinEntradasVal = new Label { Text = "R$ 0,00", Location = new Point(15, 10), Size = new Size(250, 25), Font = new Font("Segoe UI", 14F, FontStyle.Bold) };
            lblFinEntradasTitle = new Label { Text = "Total de Entradas (+)", Location = new Point(15, 38), Size = new Size(250, 18) };
            pnlFinEntradas.Controls.Add(lblFinEntradasVal);
            pnlFinEntradas.Controls.Add(lblFinEntradasTitle);

            pnlFinSaidas = new Panel { Dock = DockStyle.Fill, Margin = new Padding(10) };
            lblFinSaidasVal = new Label { Text = "R$ 0,00", Location = new Point(15, 10), Size = new Size(250, 25), Font = new Font("Segoe UI", 14F, FontStyle.Bold) };
            lblFinSaidasTitle = new Label { Text = "Total de Saídas (-)", Location = new Point(15, 38), Size = new Size(250, 18) };
            pnlFinSaidas.Controls.Add(lblFinSaidasVal);
            pnlFinSaidas.Controls.Add(lblFinSaidasTitle);

            pnlFinSaldo = new Panel { Dock = DockStyle.Fill, Margin = new Padding(10) };
            lblFinSaldoVal = new Label { Text = "R$ 0,00", Location = new Point(15, 10), Size = new Size(250, 25), Font = new Font("Segoe UI", 14F, FontStyle.Bold) };
            lblFinSaldoTitle = new Label { Text = "Saldo Líquido", Location = new Point(15, 38), Size = new Size(250, 18) };
            pnlFinSaldo.Controls.Add(lblFinSaldoVal);
            pnlFinSaldo.Controls.Add(lblFinSaldoTitle);

            tlpFinanceiroStats.Controls.Add(pnlFinEntradas, 0, 0);
            tlpFinanceiroStats.Controls.Add(pnlFinSaidas, 1, 0);
            tlpFinanceiroStats.Controls.Add(pnlFinSaldo, 2, 0);

            dgvFinanceiro = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = UITheme.BgCard };
            Panel pnlWrapFinanceiro = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 10, 10, 10) };
            pnlWrapFinanceiro.Controls.Add(dgvFinanceiro);
            tpFinanceiro.Controls.Add(pnlWrapFinanceiro);
            tpFinanceiro.Controls.Add(tlpFinanceiroStats);
            tpFinanceiro.Controls.Add(pnlFinanceiroFilters);

            // --- TAB: AUDITORIA ---
            tpAuditoria = new Panel { Dock = DockStyle.Fill, Visible = false, Padding = new Padding(20, 10, 20, 20) };
            pnlAuditoriaFilters = new Panel { Dock = DockStyle.Top, Height = 45, BackColor = UITheme.BgApp };
            lblAuditoriaBusca = new Label { Text = "Buscar:", Location = new Point(30, 13), Size = new Size(50, 20), TextAlign = ContentAlignment.MiddleLeft };
            txtAuditoriaBusca = new TextBox { Location = new Point(85, 10), Size = new Size(300, 25), BorderStyle = BorderStyle.FixedSingle };
            pnlAuditoriaFilters.Controls.Add(lblAuditoriaBusca);
            pnlAuditoriaFilters.Controls.Add(txtAuditoriaBusca);

            tlpAuditoriaStats = new TableLayoutPanel { Dock = DockStyle.Top, Height = 110, ColumnCount = 2, RowCount = 1 };
            tlpAuditoriaStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpAuditoriaStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            pnlAuditTotal = new Panel { Dock = DockStyle.Fill, Margin = new Padding(10) };
            lblAuditTotalVal = new Label { Text = "0", Location = new Point(15, 10), Size = new Size(400, 25), Font = new Font("Segoe UI", 14F, FontStyle.Bold) };
            lblAuditTotalTitle = new Label { Text = "Ações Registradas no Período", Location = new Point(15, 38), Size = new Size(400, 18) };
            pnlAuditTotal.Controls.Add(lblAuditTotalVal);
            pnlAuditTotal.Controls.Add(lblAuditTotalTitle);

            pnlAuditAdmins = new Panel { Dock = DockStyle.Fill, Margin = new Padding(10) };
            lblAuditAdminsVal = new Label { Text = "0", Location = new Point(15, 10), Size = new Size(400, 25), Font = new Font("Segoe UI", 14F, FontStyle.Bold) };
            lblAuditAdminsTitle = new Label { Text = "Ações Executadas por Administradores", Location = new Point(15, 38), Size = new Size(400, 18) };
            pnlAuditAdmins.Controls.Add(lblAuditAdminsVal);
            pnlAuditAdmins.Controls.Add(lblAuditAdminsTitle);

            tlpAuditoriaStats.Controls.Add(pnlAuditTotal, 0, 0);
            tlpAuditoriaStats.Controls.Add(pnlAuditAdmins, 1, 0);

            dgvAuditoria = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = UITheme.BgCard };
            Panel pnlWrapAuditoria = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 10, 10, 10) };
            pnlWrapAuditoria.Controls.Add(dgvAuditoria);
            tpAuditoria.Controls.Add(pnlWrapAuditoria);
            tpAuditoria.Controls.Add(tlpAuditoriaStats);
            tpAuditoria.Controls.Add(pnlAuditoriaFilters);

            pnlTabContent.Controls.Add(tpVendas);
            pnlTabContent.Controls.Add(tpCategorias);
            pnlTabContent.Controls.Add(tpCustosLucros);
            pnlTabContent.Controls.Add(tpEstoque);
            pnlTabContent.Controls.Add(tpFinanceiro);
            pnlTabContent.Controls.Add(tpAuditoria);

            this.Controls.Add(pnlTabContent);
            this.Controls.Add(pnlTabButtons);
            this.Controls.Add(pnlDateFilter);
            this.Controls.Add(pnlHeader);

            // Tab Switch Logic
            void SwitchTab(Button btn, Panel pnl)
            {
                if (activeTabButton != null)
                {
                    activeTabButton.BackColor = UITheme.BgApp;
                    activeTabButton.ForeColor = UITheme.TextMuted;
                }
                activeTabButton = btn;
                activeTabButton.BackColor = UITheme.Primary;
                activeTabButton.ForeColor = Color.White;

                if (activeTabPanel != null) activeTabPanel.Visible = false;
                activeTabPanel = pnl;
                activeTabPanel.Visible = true;
                activeTabPanel.BringToFront();
            }

            btnTabVendas.Click += (s, e) => SwitchTab(btnTabVendas, tpVendas);
            btnTabCategorias.Click += (s, e) => SwitchTab(btnTabCategorias, tpCategorias);
            btnTabCustosLucros.Click += (s, e) => SwitchTab(btnTabCustosLucros, tpCustosLucros);
            btnTabEstoque.Click += (s, e) => SwitchTab(btnTabEstoque, tpEstoque);
            btnTabFin.Click += (s, e) => SwitchTab(btnTabFin, tpFinanceiro);
            btnTabAuditoria.Click += (s, e) => SwitchTab(btnTabAuditoria, tpAuditoria);

            // Select first tab by default
            SwitchTab(btnTabVendas, tpVendas);
        }

        private void ApplyTheme()
        {
            this.BackColor = UITheme.BgApp;

            pnlHeader.BackColor = UITheme.BgCard;
            lblTitle.ForeColor = UITheme.TextTitle;
            lblSubtitle.ForeColor = UITheme.TextMuted;

            pnlDateFilter.BackColor = UITheme.BgCard;
            lblInicio.ForeColor = UITheme.TextTitle;
            lblInicio.Font = UITheme.FontBodyBold;
            lblFim.ForeColor = UITheme.TextTitle;
            lblFim.Font = UITheme.FontBodyBold;

            dtpInicio.BackColor = UITheme.BgSidebar;
            dtpInicio.ForeColor = UITheme.TextTitle;
            dtpFim.BackColor = UITheme.BgSidebar;
            dtpFim.ForeColor = UITheme.TextTitle;

            UITheme.FormatSaaSButton(btnFiltrar, true);
            UITheme.FormatSaaSButton(btnExportar, true);
            btnExportar.BackColor = Color.FromArgb(79, 70, 229);
            UITheme.FormatSaaSButton(btnImprimir, false);

            tpVendas.BackColor = UITheme.BgApp;
            tpCategorias.BackColor = UITheme.BgApp;
            tpCustosLucros.BackColor = UITheme.BgApp;
            tpEstoque.BackColor = UITheme.BgApp;
            tpFinanceiro.BackColor = UITheme.BgApp;
            tpAuditoria.BackColor = UITheme.BgApp;

            // Apply card styling
            ConfigureStatsCard(pnlVendasTotal, UITheme.Success, lblVendasTotalVal, lblVendasTotalTitle);
            ConfigureStatsCard(pnlVendasQtd, UITheme.Primary, lblVendasQtdVal, lblVendasQtdTitle);
            ConfigureStatsCard(pnlVendasTicket, Color.DeepSkyBlue, lblVendasTicketVal, lblVendasTicketTitle);
            ConfigureStatsCard(pnlCatTotal, UITheme.Primary, lblCatTotalVal, lblCatTotalTitle);

            ConfigureStatsCard(pnlTotalCustoPecas, UITheme.Danger, lblTotalCustoPecasVal, lblTotalCustoPecasTitle);
            ConfigureStatsCard(pnlTotalFaturamento, UITheme.Success, lblTotalFaturamentoVal, lblTotalFaturamentoTitle);
            ConfigureStatsCard(pnlLucroLiquido, UITheme.Primary, lblLucroLiquidoVal, lblLucroLiquidoTitle);

            ConfigureStatsCard(pnlEstoqueQtd, UITheme.Danger, lblEstoqueQtdVal, lblEstoqueQtdTitle);
            ConfigureStatsCard(pnlEstoqueCusto, Color.Orange, lblEstoqueCustoVal, lblEstoqueCustoTitle);

            ConfigureStatsCard(pnlFinEntradas, UITheme.Success, lblFinEntradasVal, lblFinEntradasTitle);
            ConfigureStatsCard(pnlFinSaidas, UITheme.Danger, lblFinSaidasVal, lblFinSaidasTitle);
            ConfigureStatsCard(pnlFinSaldo, UITheme.Primary, lblFinSaldoVal, lblFinSaldoTitle);

            ConfigureStatsCard(pnlAuditTotal, UITheme.Primary, lblAuditTotalVal, lblAuditTotalTitle);
            ConfigureStatsCard(pnlAuditAdmins, Color.MediumOrchid, lblAuditAdminsVal, lblAuditAdminsTitle);

            // Apply grid styling
            UITheme.FormatGrid(dgvVendas);
            UITheme.FormatGrid(dgvCategorias);
            UITheme.FormatGrid(dgvCustosLucros);
            UITheme.FormatGrid(dgvEstoque);
            UITheme.FormatGrid(dgvFinanceiro);
            UITheme.FormatGrid(dgvAuditoria);

            lblTecnico.ForeColor = UITheme.TextTitle;
            lblTecnico.Font = UITheme.FontBodyBold;
            cboTecnicos.BackColor = UITheme.BgSidebar;
            cboTecnicos.ForeColor = UITheme.TextTitle;



            lblTipoFin.ForeColor = UITheme.TextTitle;
            lblTipoFin.Font = UITheme.FontBodyBold;
            cboTipoFin.BackColor = UITheme.BgSidebar;
            cboTipoFin.ForeColor = UITheme.TextTitle;

            lblSubcatFin.ForeColor = UITheme.TextTitle;
            lblSubcatFin.Font = UITheme.FontBodyBold;
            cboSubcatFin.BackColor = UITheme.BgSidebar;
            cboSubcatFin.ForeColor = UITheme.TextTitle;

            lblAuditoriaBusca.ForeColor = UITheme.TextTitle;
            lblAuditoriaBusca.Font = UITheme.FontBodyBold;
            txtAuditoriaBusca.BackColor = UITheme.BgSidebar;
            txtAuditoriaBusca.ForeColor = UITheme.TextTitle;
        }

        private void ConfigureStatsCard(Panel pnl, Color accentColor, Label val, Label title)
        {
            // The panel backcolor MUST match the parent's backcolor (BgApp) so the rounded corners blend in.
            // The DrawRoundedPanel will paint the inner area with BgCard.
            pnl.BackColor = UITheme.BgApp;
            pnl.BorderStyle = BorderStyle.None;
            val.ForeColor = UITheme.TextTitle;
            title.ForeColor = UITheme.TextMuted;
            title.Font = UITheme.FontSmallBold;

            pnl.Paint += (s, e) =>
            {
                UITheme.DrawRoundedPanel(e.Graphics, pnl.ClientRectangle, UITheme.BgCard, 10, UITheme.ElementBorder);
                using (var brush = new SolidBrush(accentColor))
                {
                    e.Graphics.FillRectangle(brush, 4, 10, 4, pnl.Height - 20);
                }
            };
        }

        private async System.Threading.Tasks.Task LoadTecnicosAsync()
        {
            try
            {
                var list = await _tecnicoRepo.BuscarTodosAsync();
                list.Insert(0, new Tecnico { Id = 0, Nome = "Todos os Técnicos" });
                cboTecnicos.DataSource = list;
                cboTecnicos.DisplayMember = "Nome";
                cboTecnicos.ValueMember = "Id";
                cboTecnicos.SelectedIndex = 0;
            }
            catch { }
        }

        private void LoadSubcategorias()
        {
            try
            {
                var list = _financeiroRepo.GetSubcategoriasDisponiveis();
                list.Insert(0, "-- Todas as Subcategorias --");
                
                cboSubcatFin.DataSource = null;
                cboSubcatFin.DataSource = list;
                cboSubcatFin.SelectedIndex = 0;
            }
            catch { }
        }

        private void LoadData()
        {
            LoadVendas();
            LoadCategorias();
            LoadCustosLucros();
            LoadEstoqueBaixo();
            LoadFinanceiro();
            LoadAuditoria();
        }

        private async void LoadDataAsync()
        {
            await LoadTecnicosAsync();
            LoadSubcategorias();
            LoadData();
        }

        private void LoadVendas()
        {
            try
            {
                DataTable dt = _vendaRepo.GetVendasRelatorio(dtpInicio.Value, dtpFim.Value);
                dgvVendas.DataSource = null;
                dgvVendas.DataSource = dt;

                // Format Grid Columns
                if (dgvVendas.Columns.Count > 0)
                {
                    if (dgvVendas.Columns["Id"] != null) dgvVendas.Columns["Id"].HeaderText = "Venda #";
                    if (dgvVendas.Columns["DataVenda"] != null)
                    {
                        dgvVendas.Columns["DataVenda"].HeaderText = "Data/Hora";
                        dgvVendas.Columns["DataVenda"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                    }
                    if (dgvVendas.Columns["Cliente"] != null) dgvVendas.Columns["Cliente"].HeaderText = "Cliente";
                    if (dgvVendas.Columns["ItensVendidos"] != null) 
                    {
                        dgvVendas.Columns["ItensVendidos"].HeaderText = "Itens Vendidos";
                        dgvVendas.Columns["ItensVendidos"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                    if (dgvVendas.Columns["FormaPagamento"] != null) dgvVendas.Columns["FormaPagamento"].HeaderText = "Pagamento";
                    if (dgvVendas.Columns["TotalFinal"] != null)
                    {
                        dgvVendas.Columns["TotalFinal"].HeaderText = "Total";
                        dgvVendas.Columns["TotalFinal"].DefaultCellStyle.Format = "C2";
                    }
                    if (dgvVendas.Columns["Status"] != null) dgvVendas.Columns["Status"].HeaderText = "Situação";
                }

                // KPIs
                decimal totalVendas = 0;
                int qtdVendas = dt.Rows.Count;

                foreach (DataRow row in dt.Rows)
                {
                    totalVendas += row["TotalFinal"] != DBNull.Value ? Convert.ToDecimal(row["TotalFinal"]) : 0;
                }

                lblVendasTotalVal.Text = totalVendas.ToString("C2");
                lblVendasQtdVal.Text = qtdVendas.ToString();
                lblVendasTicketVal.Text = qtdVendas > 0 ? (totalVendas / qtdVendas).ToString("C2") : "R$ 0,00";
            }
            catch { }
        }

        private void LoadCategorias()
        {
            try
            {
                DataTable dt = _vendaRepo.GetFaturamentoPorCategoria(dtpInicio.Value, dtpFim.Value);
                dgvCategorias.DataSource = null;
                dgvCategorias.DataSource = dt;

                if (dgvCategorias.Columns.Count > 0)
                {
                    dgvCategorias.Columns["Categoria"].HeaderText = "Categoria / Origem";
                    dgvCategorias.Columns["Categoria"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    
                    dgvCategorias.Columns["Total"].HeaderText = "Faturamento Bruto (R$)";
                    dgvCategorias.Columns["Total"].DefaultCellStyle.Format = "C2";
                    dgvCategorias.Columns["Total"].Width = 200;
                }

                decimal total = 0;
                foreach (DataRow row in dt.Rows)
                {
                    total += row["Total"] != DBNull.Value ? Convert.ToDecimal(row["Total"]) : 0;
                }
                lblCatTotalVal.Text = total.ToString("C2");
            }
            catch { }
        }

        private void BtnEstornarVenda_Click(object sender, EventArgs e)
        {
            if (dgvVendas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione uma venda na lista para cancelar/estornar.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = dgvVendas.SelectedRows[0];
            int vendaId = Convert.ToInt32(row.Cells["Id"].Value);
            string status = row.Cells["Status"].Value?.ToString();

            if (status == "Cancelada")
            {
                MessageBox.Show("Esta venda já foi cancelada anteriormente.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var finRepo = new FinanceiroRepository();
            var caixa = finRepo.ObterCaixaAberto();

            if (caixa == null)
            {
                MessageBox.Show("Você precisa ter um Caixa Aberto para poder realizar o estorno de uma venda (o dinheiro sairá do caixa atual).", "Caixa Fechado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dtVenda = _vendaRepo.BuscarPorId(vendaId);
            bool hasCliente = dtVenda != null && dtVenda.Rows.Count > 0 && dtVenda.Rows[0]["ClienteId"] != DBNull.Value && Convert.ToInt32(dtVenda.Rows[0]["ClienteId"]) > 0;

            using (var frm = new FrmPromptMotivoEstorno(showDestino: hasCliente))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _vendaRepo.EstornarVenda(vendaId, frm.Motivo, caixa.Id, frm.DestinoEstorno, frm.FormaPagamentoDevolucao);
                        MessageBox.Show("Venda cancelada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadVendas(); // Refresh
                        LoadFinanceiro(); // Atualiza painel caso impacte
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao estornar venda: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LoadCustosLucros()
        {
            try
            {
                int? idTecnico = null;
                if (cboTecnicos.SelectedValue != null)
                {
                    if (cboTecnicos.SelectedValue is int idInt && idInt > 0)
                        idTecnico = idInt;
                    else if (int.TryParse(cboTecnicos.SelectedValue.ToString(), out int idParsed) && idParsed > 0)
                        idTecnico = idParsed;
                }

                DataTable dt = _osRepo.GetCustosLucrosRelatorio(dtpInicio.Value, dtpFim.Value, idTecnico);
                dgvCustosLucros.DataSource = null;
                dgvCustosLucros.DataSource = dt;

                if (dgvCustosLucros.Columns.Count > 0)
                {
                    if (dgvCustosLucros.Columns["NumeroOS"] != null) dgvCustosLucros.Columns["NumeroOS"].HeaderText = "OS #";
                    if (dgvCustosLucros.Columns["DataFechamento"] != null)
                    {
                        dgvCustosLucros.Columns["DataFechamento"].HeaderText = "Data";
                        dgvCustosLucros.Columns["DataFechamento"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    }
                    if (dgvCustosLucros.Columns["PecasUsadas"] != null)
                    {
                        dgvCustosLucros.Columns["PecasUsadas"].HeaderText = "Peças / Produtos";
                        dgvCustosLucros.Columns["PecasUsadas"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                    if (dgvCustosLucros.Columns["ValorPecas"] != null)
                    {
                        dgvCustosLucros.Columns["ValorPecas"].HeaderText = "Custo Peças";
                        dgvCustosLucros.Columns["ValorPecas"].DefaultCellStyle.Format = "C2";
                    }
                    if (dgvCustosLucros.Columns["Faturamento"] != null)
                    {
                        dgvCustosLucros.Columns["Faturamento"].HeaderText = "Faturamento";
                        dgvCustosLucros.Columns["Faturamento"].DefaultCellStyle.Format = "C2";
                    }
                    if (dgvCustosLucros.Columns["Lucro"] != null)
                    {
                        dgvCustosLucros.Columns["Lucro"].HeaderText = "Lucro Líquido";
                        dgvCustosLucros.Columns["Lucro"].DefaultCellStyle.Format = "C2";
                    }
                }

                decimal totalCusto = 0;
                decimal totalFat = 0;
                decimal totalLucro = 0;

                foreach (DataRow row in dt.Rows)
                {
                    totalCusto += row["ValorPecas"] != DBNull.Value ? Convert.ToDecimal(row["ValorPecas"]) : 0;
                    totalFat += row["Faturamento"] != DBNull.Value ? Convert.ToDecimal(row["Faturamento"]) : 0;
                    totalLucro += row["Lucro"] != DBNull.Value ? Convert.ToDecimal(row["Lucro"]) : 0;
                }

                lblTotalCustoPecasVal.Text = totalCusto.ToString("C2");
                lblTotalFaturamentoVal.Text = totalFat.ToString("C2");
                lblLucroLiquidoVal.Text = totalLucro.ToString("C2");
            }
            catch { }
        }

        private void LoadEstoqueBaixo()
        {
            try
            {
                var produtos = _produtoRepo.BuscarTodos().Where(p => p.Ativo && (p.Estoque <= p.EstoqueMinimo || p.Estoque <= 0)).ToList();

                // Convert to datatable for grid binding
                DataTable dt = new DataTable();
                dt.Columns.Add("CodigoInterno", typeof(string));
                dt.Columns.Add("Nome", typeof(string));
                dt.Columns.Add("Estoque", typeof(int));
                dt.Columns.Add("EstoqueMinimo", typeof(int));
                dt.Columns.Add("PrecoCusto", typeof(decimal));
                dt.Columns.Add("CustoReposicao", typeof(decimal));

                decimal totalCusto = 0;

                foreach (var p in produtos)
                {
                    int falta = Math.Max(0, p.EstoqueMinimo - p.Estoque);
                    if (p.Estoque <= 0 && falta == 0) falta = 1; // Precisa de pelo menos 1 unidade se o estoque estiver zerado/negativo

                    decimal custoRep = falta * p.PrecoCusto;
                    totalCusto += custoRep;

                    dt.Rows.Add(p.CodigoInterno, p.Nome, p.Estoque, p.EstoqueMinimo, p.PrecoCusto, custoRep);
                }

                dgvEstoque.DataSource = null;
                dgvEstoque.DataSource = dt;

                if (dgvEstoque.Columns.Count > 0)
                {
                    if (dgvEstoque.Columns["CodigoInterno"] != null) dgvEstoque.Columns["CodigoInterno"].HeaderText = "Código";
                    if (dgvEstoque.Columns["Nome"] != null) dgvEstoque.Columns["Nome"].HeaderText = "Descrição do Produto";
                    if (dgvEstoque.Columns["Estoque"] != null) dgvEstoque.Columns["Estoque"].HeaderText = "Qtd Atual";
                    if (dgvEstoque.Columns["EstoqueMinimo"] != null) dgvEstoque.Columns["EstoqueMinimo"].HeaderText = "Mínimo";
                    if (dgvEstoque.Columns["PrecoCusto"] != null)
                    {
                        dgvEstoque.Columns["PrecoCusto"].HeaderText = "Custo Unit.";
                        dgvEstoque.Columns["PrecoCusto"].DefaultCellStyle.Format = "C2";
                    }
                    if (dgvEstoque.Columns["CustoReposicao"] != null)
                    {
                        dgvEstoque.Columns["CustoReposicao"].HeaderText = "Custo Repos.";
                        dgvEstoque.Columns["CustoReposicao"].DefaultCellStyle.Format = "C2";
                    }
                }

                lblEstoqueQtdVal.Text = produtos.Count.ToString();
                lblEstoqueCustoVal.Text = totalCusto.ToString("C2");
            }
            catch { }
        }

        private void LoadFinanceiro()
        {
            try
            {
                DataTable dt = _financeiroRepo.GetFluxoCaixa(dtpInicio.Value, dtpFim.Value);

                // Filtragem manual se selecionou tipo diferente
                if (cboTipoFin.SelectedIndex > 0)
                {
                    string tipoFilter = cboTipoFin.SelectedItem.ToString();
                    var rows = dt.AsEnumerable().Where(r => r.Field<string>("Tipo") == tipoFilter);
                    if (rows.Any()) dt = rows.CopyToDataTable();
                    else dt.Clear();
                }

                // Filtragem por Subcategoria
                if (cboSubcatFin.SelectedIndex > 0)
                {
                    string subcatFilter = cboSubcatFin.SelectedItem.ToString();
                    var rows = dt.AsEnumerable().Where(r => 
                    {
                        string sub = r.Table.Columns.Contains("Subcategoria") ? r.Field<string>("Subcategoria") : null;
                        if (string.IsNullOrEmpty(sub)) sub = r.Field<string>("Categoria");
                        return sub == subcatFilter;
                    });
                    if (rows.Any()) dt = rows.CopyToDataTable();
                    else dt.Clear();
                }

                dgvFinanceiro.DataSource = null;
                dgvFinanceiro.DataSource = dt;

                if (dgvFinanceiro.Columns.Count > 0)
                {
                    if (dgvFinanceiro.Columns["Id"] != null) dgvFinanceiro.Columns["Id"].Visible = false;
                    if (dgvFinanceiro.Columns["IdCaixa"] != null) dgvFinanceiro.Columns["IdCaixa"].Visible = false;
                    if (dgvFinanceiro.Columns["Data"] != null)
                    {
                        dgvFinanceiro.Columns["Data"].HeaderText = "Data/Hora";
                        dgvFinanceiro.Columns["Data"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                    }
                    if (dgvFinanceiro.Columns["Tipo"] != null) dgvFinanceiro.Columns["Tipo"].HeaderText = "Tipo";
                    if (dgvFinanceiro.Columns["Categoria"] != null) dgvFinanceiro.Columns["Categoria"].HeaderText = "Categoria";
                    if (dgvFinanceiro.Columns["Subcategoria"] != null)
                    {
                        dgvFinanceiro.Columns["Subcategoria"].HeaderText = "Subcategoria";
                        dgvFinanceiro.Columns["Subcategoria"].Visible = true;
                    }
                    if (dgvFinanceiro.Columns["FormaPagamento"] != null) dgvFinanceiro.Columns["FormaPagamento"].HeaderText = "Pagamento";
                    if (dgvFinanceiro.Columns["Descricao"] != null) dgvFinanceiro.Columns["Descricao"].HeaderText = "Descrição";
                    if (dgvFinanceiro.Columns["Valor"] != null)
                    {
                        dgvFinanceiro.Columns["Valor"].HeaderText = "Valor";
                        dgvFinanceiro.Columns["Valor"].DefaultCellStyle.Format = "C2";
                    }
                }

                // KPIs
                decimal entradas = 0;
                decimal saidas = 0;

                foreach (DataRow row in dt.Rows)
                {
                    decimal valor = row["Valor"] != DBNull.Value ? Convert.ToDecimal(row["Valor"]) : 0;
                    string tipo = row["Tipo"]?.ToString() ?? "";

                    if (tipo == "Entrada") entradas += valor;
                    else saidas += valor;
                }

                decimal saldo = entradas - saidas;

                lblFinEntradasVal.Text = entradas.ToString("C2");
                lblFinSaidasVal.Text = saidas.ToString("C2");
                lblFinSaldoVal.Text = saldo.ToString("C2");
                lblFinSaldoVal.ForeColor = saldo >= 0 ? UITheme.Success : UITheme.Danger;
            }
            catch { }
        }

        private void LoadAuditoria()
        {
            try
            {
                DataTable dt = _auditoriaRepo.Listar(dtpInicio.Value, dtpFim.Value, txtAuditoriaBusca.Text.Trim());
                dgvAuditoria.DataSource = null;
                dgvAuditoria.DataSource = dt;

                if (dgvAuditoria.Columns.Count > 0)
                {
                    if (dgvAuditoria.Columns["DataHora"] != null)
                    {
                        dgvAuditoria.Columns["DataHora"].HeaderText = "Data/Hora";
                        dgvAuditoria.Columns["DataHora"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
                        dgvAuditoria.Columns["DataHora"].Width = 140;
                    }
                    if (dgvAuditoria.Columns["Usuario"] != null)
                    {
                        dgvAuditoria.Columns["Usuario"].HeaderText = "Usuário";
                        dgvAuditoria.Columns["Usuario"].Width = 100;
                    }
                    if (dgvAuditoria.Columns["Acao"] != null)
                    {
                        dgvAuditoria.Columns["Acao"].HeaderText = "Ação";
                        dgvAuditoria.Columns["Acao"].Width = 120;
                    }
                    if (dgvAuditoria.Columns["Tabela"] != null)
                    {
                        dgvAuditoria.Columns["Tabela"].HeaderText = "Módulo";
                        dgvAuditoria.Columns["Tabela"].Width = 120;
                    }
                    if (dgvAuditoria.Columns["Descricao"] != null)
                    {
                        dgvAuditoria.Columns["Descricao"].HeaderText = "Detalhamento da Operação";
                        dgvAuditoria.Columns["Descricao"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                }

                // KPIs
                int totalAcoes = dt.Rows.Count;
                int acoesAdmin = 0;

                foreach (DataRow row in dt.Rows)
                {
                    string user = row["Usuario"]?.ToString() ?? "";
                    if (user.Equals("admin", StringComparison.OrdinalIgnoreCase))
                    {
                        acoesAdmin++;
                    }
                }

                lblAuditTotalVal.Text = totalAcoes.ToString();
                lblAuditAdminsVal.Text = acoesAdmin.ToString();
            }
            catch { }
        }

        private void BindEvents()
        {
            btnFiltrar.Click += (s, e) => LoadData();

            cboTecnicos.SelectedIndexChanged += (s, e) => LoadCustosLucros();
            cboTipoFin.SelectedIndexChanged += (s, e) => LoadFinanceiro();
            cboSubcatFin.SelectedIndexChanged += (s, e) => LoadFinanceiro();
            txtAuditoriaBusca.TextChanged += (s, e) => LoadAuditoria();

            btnExportar.Click += btnExportar_Click;
            btnImprimir.Click += BtnImprimir_Click;
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            string activeTabTitle = activeTabButton.Text.Substring(3); // Remove emoji
            

            DataGridView activeGrid = null;

            if (activeTabPanel == tpVendas) activeGrid = dgvVendas;
            if (activeTabPanel == tpCategorias) activeGrid = dgvCategorias;
            if (activeTabPanel == tpCustosLucros) activeGrid = dgvCustosLucros;
            if (activeTabPanel == tpEstoque) activeGrid = dgvEstoque;
            if (activeTabPanel == tpFinanceiro) activeGrid = dgvFinanceiro;
            if (activeTabPanel == tpAuditoria) activeGrid = dgvAuditoria;

            ExportToCsv(activeGrid, $"Relatorio_{activeTabTitle.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}");
        }

        private string GetActiveTitle()
        {
            return activeTabButton.Text.Substring(3); // Remove emoji
        }

        private DataGridView GetActiveGrid()
        {
            if (activeTabPanel == tpVendas)
                return dgvVendas;
            if (activeTabPanel == tpCustosLucros)
                return dgvCustosLucros;
            if (activeTabPanel == tpEstoque)
                return dgvEstoque;
            if (activeTabPanel == tpFinanceiro)
                return dgvFinanceiro;
            if (activeTabPanel == tpAuditoria)
                return dgvAuditoria;
            return null;
        }

        private string GetActiveReportTitle()
        {
            return activeTabButton.Text.Substring(3); // Remove emoji
        }

        private void SetPrintWidths(DataGridView grid, PrintPageEventArgs e, int startX)
        {
            if (activeTabPanel == tpVendas)
                grid.Columns[0].Width = 80;
            if (activeTabPanel == tpCustosLucros)
                grid.Columns[0].Width = 80;
            if (activeTabPanel == tpEstoque)
                grid.Columns[0].Width = 100;
            if (activeTabPanel == tpFinanceiro)
                grid.Columns[0].Width = 100;
        }

        private string GetActiveReportKpis()
        {
            if (activeTabPanel == tpVendas)
                return $"Faturamento Bruto: {lblVendasTotalVal.Text}  |  Quantidade: {lblVendasQtdVal.Text}  |  Tíquete Médio: {lblVendasTicketVal.Text}";
            if (activeTabPanel == tpCustosLucros)
                return $"Custo Total: {lblTotalCustoPecasVal.Text}  |  Faturamento: {lblTotalFaturamentoVal.Text}  |  Lucro: {lblLucroLiquidoVal.Text}";
            if (activeTabPanel == tpEstoque)
                return $"Estoque Crítico: {lblEstoqueQtdVal.Text} itens  |  Custo Reposição: {lblEstoqueCustoVal.Text}";
            if (activeTabPanel == tpFinanceiro)
                return $"Entradas (+): {lblFinEntradasVal.Text}  |  Saídas (-): {lblFinSaidasVal.Text}  |  Saldo Líquido: {lblFinSaldoVal.Text}";
            return $"Ações Totais: {lblAuditTotalVal.Text}  |  Por Admins: {lblAuditAdminsVal.Text}";
        }

        private void ExportToCsv(DataGridView dgv, string defaultFileName)
        {
            if (dgv.Rows.Count == 0)
            {
                MessageBox.Show("Não há dados na tabela para exportar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Arquivo CSV (*.csv)|*.csv";
                sfd.FileName = defaultFileName;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var sw = new System.IO.StreamWriter(sfd.FileName, false, System.Text.Encoding.GetEncoding("iso-8859-1")))
                        {
                            // Headers
                            var headers = new List<string>();
                            foreach (DataGridViewColumn col in dgv.Columns)
                            {
                                if (col.Visible)
                                {
                                    headers.Add($"\"{col.HeaderText.Replace("\"", "\"\"")}\"");
                                }
                            }
                            sw.WriteLine(string.Join(";", headers));

                            // Rows
                            foreach (DataGridViewRow row in dgv.Rows)
                            {
                                var cells = new List<string>();
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    if (dgv.Columns[cell.ColumnIndex].Visible)
                                    {
                                        string val = "";
                                        if (cell.Value is DateTime dtVal)
                                            val = dtVal.ToString("dd/MM/yyyy HH:mm:ss");
                                        else
                                            val = cell.Value?.ToString() ?? "";

                                        cells.Add($"\"{val.Replace("\"", "\"\"")}\"");
                                    }
                                }
                                sw.WriteLine(string.Join(";", cells));
                            }
                        }
                        MessageBox.Show("Dados exportados com sucesso!", "Exportação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao exportar dados: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            DataGridView dgv = GetActiveGrid();
            if (dgv.Rows.Count == 0)
            {
                MessageBox.Show("Não há dados na tabela para imprimir.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _currentRowIndex = 0;

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += (s, ev) => PrintReportPage(ev, dgv, GetActiveReportTitle(), GetActiveReportKpis());
            pd.DocumentName = $"Relatorio_{GetActiveReportTitle()}";

            using (var previewDialog = new PrintPreviewDialog())
            {
                previewDialog.Document = pd;
                previewDialog.WindowState = FormWindowState.Maximized;
                previewDialog.ShowIcon = false;
                previewDialog.Text = $"Impressão - Relatório de {GetActiveReportTitle()}";
                previewDialog.ShowDialog();
            }
        }

        private void PrintReportPage(PrintPageEventArgs e, DataGridView dgv, string title, string kpis)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Font fontTitle = new Font("Arial", 16, FontStyle.Bold);
            Font fontHeader = new Font("Arial", 10, FontStyle.Bold);
            Font fontRow = new Font("Arial", 9, FontStyle.Regular);
            Font fontKpis = new Font("Arial", 10, FontStyle.Bold);
            Font fontTime = new Font("Arial", 8, FontStyle.Regular);

            int startX = 40;
            int startY = 40;
            int y = startY;
            int width = e.PageBounds.Width - 80;

            // 1. Header
            var conf = new MasterServicePro.DAL.ConfiguracaoTermosRepository().ObterConfiguracao();
            string tituloBase = string.IsNullOrWhiteSpace(conf.NomeLoja) ? "MASTER SERVICE PRO" : conf.NomeLoja;
            g.DrawString(tituloBase + " - ERP/PDV", fontHeader, Brushes.Gray, startX, y);
            g.DrawString($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm:ss}", fontTime, Brushes.Gray, startX + width - 180, y);
            y += 20;
            g.DrawString(title.ToUpper(), fontTitle, Brushes.Black, startX, y);
            y += 30;
            g.DrawString($"Período: {dtpInicio.Value:dd/MM/yyyy} até {dtpFim.Value:dd/MM/yyyy}", fontRow, Brushes.Black, startX, y);
            y += 20;
            g.DrawString(kpis, fontKpis, Brushes.DarkBlue, startX, y);
            y += 30;

            // Draw line
            g.DrawLine(Pens.Black, startX, y, startX + width, y);
            y += 10;

            // Get columns to print
            var visibleCols = new List<DataGridViewColumn>();
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (col.Visible) visibleCols.Add(col);
            }

            int colWidth = width / Math.Max(1, visibleCols.Count);

            // Draw headers
            int x = startX;
            foreach (var col in visibleCols)
            {
                g.DrawString(col.HeaderText, fontHeader, Brushes.Black, x, y);
                x += colWidth;
            }
            y += 25;
            g.DrawLine(Pens.LightGray, startX, y, startX + width, y);
            y += 10;

            // Draw rows
            while (_currentRowIndex < dgv.Rows.Count)
            {
                var row = dgv.Rows[_currentRowIndex];
                x = startX;
                foreach (var col in visibleCols)
                {
                    string cellVal = "";
                    
                    // Tratamento específico de formatação para impressão
                    if (row.Cells[col.Index].Value is DateTime dt)
                        cellVal = dt.ToString("dd/MM/yyyy HH:mm");
                    else if (row.Cells[col.Index].Value is decimal dec)
                        cellVal = dec.ToString("C2");
                    else
                        cellVal = row.Cells[col.Index].Value?.ToString() ?? "";

                    if (cellVal.Length > 25) 
                        cellVal = cellVal.Substring(0, 22) + "...";

                    g.DrawString(cellVal, fontRow, Brushes.Black, x, y);
                    x += colWidth;
                }
                y += 20;
                _currentRowIndex++;

                // Page break check
                if (y > e.PageBounds.Height - 60)
                {
                    e.HasMorePages = true;
                    return;
                }
            }

            // Finished printing
            e.HasMorePages = false;
            _currentRowIndex = 0; // reset
        }
    }
}
