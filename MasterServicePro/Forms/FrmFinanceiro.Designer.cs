namespace MasterServicePro.Forms
{
    partial class FrmFinanceiro
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnAbrirCaixa = new System.Windows.Forms.Button();
            this.btnFecharCaixa = new System.Windows.Forms.Button();
            this.btnHistorico = new System.Windows.Forms.Button();
            this.pnlChart = new System.Windows.Forms.Panel();
            this.lblChartTitle = new System.Windows.Forms.Label();
            this.tlpStats = new System.Windows.Forms.TableLayoutPanel();
            this.pnlAbertura = new System.Windows.Forms.Panel();
            this.lblAberturaValue = new System.Windows.Forms.Label();
            this.lblAberturaTitle = new System.Windows.Forms.Label();
            this.pnlPix = new System.Windows.Forms.Panel();
            this.lblPixValue = new System.Windows.Forms.Label();
            this.lblPixTitle = new System.Windows.Forms.Label();
            this.pnlDinheiro = new System.Windows.Forms.Panel();
            this.lblDinheiroValue = new System.Windows.Forms.Label();
            this.lblDinheiroTitle = new System.Windows.Forms.Label();
            this.pnlCredito = new System.Windows.Forms.Panel();
            this.lblCreditoValue = new System.Windows.Forms.Label();
            this.lblCreditoTitle = new System.Windows.Forms.Label();
            this.pnlDebito = new System.Windows.Forms.Panel();
            this.lblDebitoValue = new System.Windows.Forms.Label();
            this.lblDebitoTitle = new System.Windows.Forms.Label();
            this.pnlEntradas = new System.Windows.Forms.Panel();
            this.lblEntradasValue = new System.Windows.Forms.Label();
            this.lblEntradasTitle = new System.Windows.Forms.Label();
            this.pnlSaidas = new System.Windows.Forms.Panel();
            this.lblSaidasValue = new System.Windows.Forms.Label();
            this.lblSaidasTitle = new System.Windows.Forms.Label();
            this.pnlSaldo = new System.Windows.Forms.Panel();
            this.lblSaldoValue = new System.Windows.Forms.Label();
            this.lblSaldoTitle = new System.Windows.Forms.Label();
            this.pnlFilters = new System.Windows.Forms.Panel();
            this.dtpFim = new System.Windows.Forms.DateTimePicker();
            this.dtpInicio = new System.Windows.Forms.DateTimePicker();
            this.lblFiltro = new System.Windows.Forms.Label();
            this.dgvFluxo = new System.Windows.Forms.DataGridView();
            this.tlpMain.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlChart.SuspendLayout();
            this.tlpStats.SuspendLayout();
            this.pnlAbertura.SuspendLayout();
            this.pnlPix.SuspendLayout();
            this.pnlDinheiro.SuspendLayout();
            this.pnlCredito.SuspendLayout();
            this.pnlDebito.SuspendLayout();
            this.pnlEntradas.SuspendLayout();
            this.pnlSaidas.SuspendLayout();
            this.pnlSaldo.SuspendLayout();
            this.pnlFilters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFluxo)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.Controls.Add(this.pnlFilters, 0, 3);
            this.tlpMain.Controls.Add(this.dgvFluxo, 0, 4);
            this.tlpMain.Controls.Add(this.pnlHeader, 0, 0);
            this.tlpMain.Controls.Add(this.tlpStats, 0, 1);
            this.tlpMain.Controls.Add(this.pnlChart, 0, 2);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 5;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 210F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Size = new System.Drawing.Size(900, 750);
            this.tlpMain.TabIndex = 0;
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.btnHistorico);
            this.pnlHeader.Controls.Add(this.btnFecharCaixa);
            this.pnlHeader.Controls.Add(this.btnAbrirCaixa);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(900, 60);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.lblTitle.Size = new System.Drawing.Size(400, 60);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Fluxo de Caixa / Financeiro";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnHistorico
            // 
            this.btnHistorico.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHistorico.Location = new System.Drawing.Point(440, 12);
            this.btnHistorico.Name = "btnHistorico";
            this.btnHistorico.Size = new System.Drawing.Size(140, 35);
            this.btnHistorico.TabIndex = 3;
            this.btnHistorico.Text = "HISTÓRICO";
            this.btnHistorico.UseVisualStyleBackColor = true;
            // 
            // btnAbrirCaixa
            // 
            this.btnAbrirCaixa.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbrirCaixa.Location = new System.Drawing.Point(590, 12);
            this.btnAbrirCaixa.Name = "btnAbrirCaixa";
            this.btnAbrirCaixa.Size = new System.Drawing.Size(140, 35);
            this.btnAbrirCaixa.TabIndex = 1;
            this.btnAbrirCaixa.Text = "ABRIR CAIXA";
            this.btnAbrirCaixa.UseVisualStyleBackColor = true;
            // 
            // btnFecharCaixa
            // 
            this.btnFecharCaixa.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFecharCaixa.Location = new System.Drawing.Point(740, 12);
            this.btnFecharCaixa.Name = "btnFecharCaixa";
            this.btnFecharCaixa.Size = new System.Drawing.Size(140, 35);
            this.btnFecharCaixa.TabIndex = 2;
            this.btnFecharCaixa.Text = "FECHAR CAIXA";
            this.btnFecharCaixa.UseVisualStyleBackColor = true;
            // 
            // pnlChart
            // 
            this.pnlChart.Controls.Add(this.lblChartTitle);
            this.pnlChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlChart.Location = new System.Drawing.Point(10, 280);
            this.pnlChart.Margin = new System.Windows.Forms.Padding(10);
            this.pnlChart.Name = "pnlChart";
            this.pnlChart.Size = new System.Drawing.Size(880, 160);
            this.pnlChart.TabIndex = 4;
            // 
            // lblChartTitle
            // 
            this.lblChartTitle.AutoSize = true;
            this.lblChartTitle.Location = new System.Drawing.Point(10, 5);
            this.lblChartTitle.Name = "lblChartTitle";
            this.lblChartTitle.Size = new System.Drawing.Size(180, 15);
            this.lblChartTitle.TabIndex = 0;
            this.lblChartTitle.Text = "Faturamento - Últimos 7 Dias";
            // 
            // tlpStats
            // 
            this.tlpStats.ColumnCount = 4;
            this.tlpStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpStats.Controls.Add(this.pnlSaldo, 3, 0);
            this.tlpStats.Controls.Add(this.pnlSaidas, 2, 0);
            this.tlpStats.Controls.Add(this.pnlEntradas, 1, 0);
            this.tlpStats.Controls.Add(this.pnlAbertura, 0, 0);
            this.tlpStats.Controls.Add(this.pnlPix, 0, 1);
            this.tlpStats.Controls.Add(this.pnlDinheiro, 1, 1);
            this.tlpStats.Controls.Add(this.pnlCredito, 2, 1);
            this.tlpStats.Controls.Add(this.pnlDebito, 3, 1);
            this.tlpStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpStats.Location = new System.Drawing.Point(20, 70);
            this.tlpStats.Margin = new System.Windows.Forms.Padding(20, 10, 20, 10);
            this.tlpStats.Name = "tlpStats";
            this.tlpStats.RowCount = 2;
            this.tlpStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpStats.Size = new System.Drawing.Size(860, 200);
            this.tlpStats.TabIndex = 1;
            // 
            // pnlAbertura
            // 
            this.pnlAbertura.Controls.Add(this.lblAberturaValue);
            this.pnlAbertura.Controls.Add(this.lblAberturaTitle);
            this.pnlAbertura.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAbertura.Location = new System.Drawing.Point(5, 5);
            this.pnlAbertura.Margin = new System.Windows.Forms.Padding(5);
            this.pnlAbertura.Name = "pnlAbertura";
            this.pnlAbertura.Padding = new System.Windows.Forms.Padding(10);
            this.pnlAbertura.Size = new System.Drawing.Size(205, 90);
            this.pnlAbertura.TabIndex = 3;
            // 
            // lblAberturaTitle
            // 
            this.lblAberturaTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAberturaTitle.Location = new System.Drawing.Point(10, 10);
            this.lblAberturaTitle.Name = "lblAberturaTitle";
            this.lblAberturaTitle.Size = new System.Drawing.Size(185, 20);
            this.lblAberturaTitle.TabIndex = 0;
            this.lblAberturaTitle.Text = "SALDO INICIAL (CAIXA)";
            // 
            // lblAberturaValue
            // 
            this.lblAberturaValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAberturaValue.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblAberturaValue.Location = new System.Drawing.Point(10, 30);
            this.lblAberturaValue.Name = "lblAberturaValue";
            this.lblAberturaValue.Size = new System.Drawing.Size(185, 50);
            this.lblAberturaValue.TabIndex = 1;
            this.lblAberturaValue.Text = "R$ 0,00";
            this.lblAberturaValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlEntradas
            // 
            this.pnlEntradas.Controls.Add(this.lblEntradasValue);
            this.pnlEntradas.Controls.Add(this.lblEntradasTitle);
            this.pnlEntradas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEntradas.Location = new System.Drawing.Point(5, 5);
            this.pnlEntradas.Margin = new System.Windows.Forms.Padding(5);
            this.pnlEntradas.Name = "pnlEntradas";
            this.pnlEntradas.Padding = new System.Windows.Forms.Padding(10);
            this.pnlEntradas.Size = new System.Drawing.Size(276, 90);
            this.pnlEntradas.TabIndex = 0;
            // 
            // lblEntradasTitle
            // 
            this.lblEntradasTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblEntradasTitle.Location = new System.Drawing.Point(10, 10);
            this.lblEntradasTitle.Name = "lblEntradasTitle";
            this.lblEntradasTitle.Size = new System.Drawing.Size(256, 20);
            this.lblEntradasTitle.TabIndex = 0;
            this.lblEntradasTitle.Text = "ENTRADAS (VENDAS/OS)";
            // 
            // lblEntradasValue
            // 
            this.lblEntradasValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblEntradasValue.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblEntradasValue.Location = new System.Drawing.Point(10, 30);
            this.lblEntradasValue.Name = "lblEntradasValue";
            this.lblEntradasValue.Size = new System.Drawing.Size(256, 50);
            this.lblEntradasValue.TabIndex = 1;
            this.lblEntradasValue.Text = "R$ 0,00";
            this.lblEntradasValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlSaidas
            // 
            this.pnlSaidas.Controls.Add(this.lblSaidasValue);
            this.pnlSaidas.Controls.Add(this.lblSaidasTitle);
            this.pnlSaidas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSaidas.Location = new System.Drawing.Point(291, 5);
            this.pnlSaidas.Margin = new System.Windows.Forms.Padding(5);
            this.pnlSaidas.Name = "pnlSaidas";
            this.pnlSaidas.Padding = new System.Windows.Forms.Padding(10);
            this.pnlSaidas.Size = new System.Drawing.Size(276, 90);
            this.pnlSaidas.TabIndex = 1;
            // 
            // lblSaidasTitle
            // 
            this.lblSaidasTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSaidasTitle.Location = new System.Drawing.Point(10, 10);
            this.lblSaidasTitle.Name = "lblSaidasTitle";
            this.lblSaidasTitle.Size = new System.Drawing.Size(256, 20);
            this.lblSaidasTitle.TabIndex = 0;
            this.lblSaidasTitle.Text = "SAÍDAS (SANGRIAS/DESPESAS)";
            // 
            // lblSaidasValue
            // 
            this.lblSaidasValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSaidasValue.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSaidasValue.Location = new System.Drawing.Point(10, 30);
            this.lblSaidasValue.Name = "lblSaidasValue";
            this.lblSaidasValue.Size = new System.Drawing.Size(256, 50);
            this.lblSaidasValue.TabIndex = 1;
            this.lblSaidasValue.Text = "R$ 0,00";
            this.lblSaidasValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlSaldo
            // 
            this.pnlSaldo.Controls.Add(this.lblSaldoValue);
            this.pnlSaldo.Controls.Add(this.lblSaldoTitle);
            this.pnlSaldo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSaldo.Location = new System.Drawing.Point(577, 5);
            this.pnlSaldo.Margin = new System.Windows.Forms.Padding(5);
            this.pnlSaldo.Name = "pnlSaldo";
            this.pnlSaldo.Padding = new System.Windows.Forms.Padding(10);
            this.pnlSaldo.Size = new System.Drawing.Size(278, 90);
            this.pnlSaldo.TabIndex = 2;
            // 
            // lblSaldoTitle
            // 
            this.lblSaldoTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSaldoTitle.Location = new System.Drawing.Point(10, 10);
            this.lblSaldoTitle.Name = "lblSaldoTitle";
            this.lblSaldoTitle.Size = new System.Drawing.Size(258, 20);
            this.lblSaldoTitle.TabIndex = 0;
            this.lblSaldoTitle.Text = "SALDO LÍQUIDO";
            // 
            // lblSaldoValue
            // 
            this.lblSaldoValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSaldoValue.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSaldoValue.Location = new System.Drawing.Point(10, 30);
            this.lblSaldoValue.Name = "lblSaldoValue";
            this.lblSaldoValue.Size = new System.Drawing.Size(258, 50);
            this.lblSaldoValue.TabIndex = 1;
            this.lblSaldoValue.Text = "R$ 0,00";
            this.lblSaldoValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlPix
            // 
            this.pnlPix.Controls.Add(this.lblPixValue);
            this.pnlPix.Controls.Add(this.lblPixTitle);
            this.pnlPix.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPix.Location = new System.Drawing.Point(5, 105);
            this.pnlPix.Margin = new System.Windows.Forms.Padding(5);
            this.pnlPix.Name = "pnlPix";
            this.pnlPix.Padding = new System.Windows.Forms.Padding(10);
            this.pnlPix.Size = new System.Drawing.Size(205, 90);
            this.pnlPix.TabIndex = 4;
            // 
            // lblPixTitle
            // 
            this.lblPixTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPixTitle.Location = new System.Drawing.Point(10, 10);
            this.lblPixTitle.Name = "lblPixTitle";
            this.lblPixTitle.Size = new System.Drawing.Size(185, 20);
            this.lblPixTitle.TabIndex = 0;
            this.lblPixTitle.Text = "ENTRADAS PIX";
            // 
            // lblPixValue
            // 
            this.lblPixValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPixValue.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblPixValue.Location = new System.Drawing.Point(10, 30);
            this.lblPixValue.Name = "lblPixValue";
            this.lblPixValue.Size = new System.Drawing.Size(185, 50);
            this.lblPixValue.TabIndex = 1;
            this.lblPixValue.Text = "R$ 0,00";
            this.lblPixValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlDinheiro
            // 
            this.pnlDinheiro.Controls.Add(this.lblDinheiroValue);
            this.pnlDinheiro.Controls.Add(this.lblDinheiroTitle);
            this.pnlDinheiro.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDinheiro.Location = new System.Drawing.Point(220, 105);
            this.pnlDinheiro.Margin = new System.Windows.Forms.Padding(5);
            this.pnlDinheiro.Name = "pnlDinheiro";
            this.pnlDinheiro.Padding = new System.Windows.Forms.Padding(10);
            this.pnlDinheiro.Size = new System.Drawing.Size(205, 90);
            this.pnlDinheiro.TabIndex = 5;
            // 
            // lblDinheiroTitle
            // 
            this.lblDinheiroTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDinheiroTitle.Location = new System.Drawing.Point(10, 10);
            this.lblDinheiroTitle.Name = "lblDinheiroTitle";
            this.lblDinheiroTitle.Size = new System.Drawing.Size(185, 20);
            this.lblDinheiroTitle.TabIndex = 0;
            this.lblDinheiroTitle.Text = "ENTRADAS DINHEIRO";
            // 
            // lblDinheiroValue
            // 
            this.lblDinheiroValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDinheiroValue.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblDinheiroValue.Location = new System.Drawing.Point(10, 30);
            this.lblDinheiroValue.Name = "lblDinheiroValue";
            this.lblDinheiroValue.Size = new System.Drawing.Size(185, 50);
            this.lblDinheiroValue.TabIndex = 1;
            this.lblDinheiroValue.Text = "R$ 0,00";
            this.lblDinheiroValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlCredito
            // 
            this.pnlCredito.Controls.Add(this.lblCreditoValue);
            this.pnlCredito.Controls.Add(this.lblCreditoTitle);
            this.pnlCredito.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCredito.Location = new System.Drawing.Point(435, 105);
            this.pnlCredito.Margin = new System.Windows.Forms.Padding(5);
            this.pnlCredito.Name = "pnlCredito";
            this.pnlCredito.Padding = new System.Windows.Forms.Padding(10);
            this.pnlCredito.Size = new System.Drawing.Size(205, 90);
            this.pnlCredito.TabIndex = 6;
            // 
            // lblCreditoTitle
            // 
            this.lblCreditoTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCreditoTitle.Location = new System.Drawing.Point(10, 10);
            this.lblCreditoTitle.Name = "lblCreditoTitle";
            this.lblCreditoTitle.Size = new System.Drawing.Size(185, 20);
            this.lblCreditoTitle.TabIndex = 0;
            this.lblCreditoTitle.Text = "ENTRADAS CRÉDITO";
            // 
            // lblCreditoValue
            // 
            this.lblCreditoValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCreditoValue.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCreditoValue.Location = new System.Drawing.Point(10, 30);
            this.lblCreditoValue.Name = "lblCreditoValue";
            this.lblCreditoValue.Size = new System.Drawing.Size(185, 50);
            this.lblCreditoValue.TabIndex = 1;
            this.lblCreditoValue.Text = "R$ 0,00";
            this.lblCreditoValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlDebito
            // 
            this.pnlDebito.Controls.Add(this.lblDebitoValue);
            this.pnlDebito.Controls.Add(this.lblDebitoTitle);
            this.pnlDebito.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDebito.Location = new System.Drawing.Point(650, 105);
            this.pnlDebito.Margin = new System.Windows.Forms.Padding(5);
            this.pnlDebito.Name = "pnlDebito";
            this.pnlDebito.Padding = new System.Windows.Forms.Padding(10);
            this.pnlDebito.Size = new System.Drawing.Size(205, 90);
            this.pnlDebito.TabIndex = 7;
            // 
            // lblDebitoTitle
            // 
            this.lblDebitoTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDebitoTitle.Location = new System.Drawing.Point(10, 10);
            this.lblDebitoTitle.Name = "lblDebitoTitle";
            this.lblDebitoTitle.Size = new System.Drawing.Size(185, 20);
            this.lblDebitoTitle.TabIndex = 0;
            this.lblDebitoTitle.Text = "ENTRADAS DÉBITO";
            // 
            // lblDebitoValue
            // 
            this.lblDebitoValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDebitoValue.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblDebitoValue.Location = new System.Drawing.Point(10, 30);
            this.lblDebitoValue.Name = "lblDebitoValue";
            this.lblDebitoValue.Size = new System.Drawing.Size(185, 50);
            this.lblDebitoValue.TabIndex = 1;
            this.lblDebitoValue.Text = "R$ 0,00";
            this.lblDebitoValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlFilters
            // 
            this.pnlFilters.Controls.Add(this.dtpFim);
            this.pnlFilters.Controls.Add(this.dtpInicio);
            this.pnlFilters.Controls.Add(this.lblFiltro);
            this.pnlFilters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFilters.Location = new System.Drawing.Point(0, 280);
            this.pnlFilters.Margin = new System.Windows.Forms.Padding(0);
            this.pnlFilters.Name = "pnlFilters";
            this.pnlFilters.Size = new System.Drawing.Size(900, 50);
            this.pnlFilters.TabIndex = 2;
            // 
            // lblFiltro
            // 
            this.lblFiltro.AutoSize = true;
            this.lblFiltro.Location = new System.Drawing.Point(20, 18);
            this.lblFiltro.Name = "lblFiltro";
            this.lblFiltro.Size = new System.Drawing.Size(126, 15);
            this.lblFiltro.TabIndex = 0;
            this.lblFiltro.Text = "Filtrar por Período:";
            // 
            // dtpInicio
            // 
            this.dtpInicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpInicio.Location = new System.Drawing.Point(155, 14);
            this.dtpInicio.Name = "dtpInicio";
            this.dtpInicio.Size = new System.Drawing.Size(120, 23);
            this.dtpInicio.TabIndex = 1;
            // 
            // dtpFim
            // 
            this.dtpFim.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFim.Location = new System.Drawing.Point(285, 14);
            this.dtpFim.Name = "dtpFim";
            this.dtpFim.Size = new System.Drawing.Size(120, 23);
            this.dtpFim.TabIndex = 2;
            // 
            // dgvFluxo
            // 
            this.dgvFluxo.AllowUserToAddRows = false;
            this.dgvFluxo.AllowUserToDeleteRows = false;
            this.dgvFluxo.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFluxo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFluxo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFluxo.Location = new System.Drawing.Point(20, 340);
            this.dgvFluxo.Margin = new System.Windows.Forms.Padding(20, 10, 20, 20);
            this.dgvFluxo.Name = "dgvFluxo";
            this.dgvFluxo.ReadOnly = true;
            this.dgvFluxo.RowHeadersVisible = false;
            this.dgvFluxo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFluxo.Size = new System.Drawing.Size(860, 340);
            this.dgvFluxo.TabIndex = 3;
            // 
            // FrmFinanceiro
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 750);
            this.Controls.Add(this.tlpMain);
            this.Name = "FrmFinanceiro";
            this.Text = "Financeiro";
            this.tlpMain.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlChart.ResumeLayout(false);
            this.pnlChart.PerformLayout();
            this.tlpStats.ResumeLayout(false);
            this.pnlEntradas.ResumeLayout(false);
            this.pnlSaidas.ResumeLayout(false);
            this.pnlSaldo.ResumeLayout(false);
            this.pnlFilters.ResumeLayout(false);
            this.pnlFilters.PerformLayout();
            this.pnlAbertura.ResumeLayout(false);
            this.pnlPix.ResumeLayout(false);
            this.pnlDinheiro.ResumeLayout(false);
            this.pnlCredito.ResumeLayout(false);
            this.pnlDebito.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFluxo)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnAbrirCaixa;
        private System.Windows.Forms.Button btnFecharCaixa;
        private System.Windows.Forms.Button btnHistorico;
        private System.Windows.Forms.Panel pnlChart;
        private System.Windows.Forms.Label lblChartTitle;
        private System.Windows.Forms.TableLayoutPanel tlpStats;
        private System.Windows.Forms.Panel pnlEntradas;
        private System.Windows.Forms.Label lblEntradasValue;
        private System.Windows.Forms.Label lblEntradasTitle;
        private System.Windows.Forms.Panel pnlSaidas;
        private System.Windows.Forms.Label lblSaidasValue;
        private System.Windows.Forms.Label lblSaidasTitle;
        private System.Windows.Forms.Panel pnlSaldo;
        private System.Windows.Forms.Label lblSaldoValue;
        private System.Windows.Forms.Label lblSaldoTitle;
        private System.Windows.Forms.Panel pnlFilters;
        private System.Windows.Forms.Label lblFiltro;
        private System.Windows.Forms.DateTimePicker dtpInicio;
        private System.Windows.Forms.DateTimePicker dtpFim;
        private System.Windows.Forms.DataGridView dgvFluxo;
        private System.Windows.Forms.Panel pnlAbertura;
        private System.Windows.Forms.Label lblAberturaValue;
        private System.Windows.Forms.Label lblAberturaTitle;
        private System.Windows.Forms.Panel pnlPix;
        private System.Windows.Forms.Label lblPixValue;
        private System.Windows.Forms.Label lblPixTitle;
        private System.Windows.Forms.Panel pnlDinheiro;
        private System.Windows.Forms.Label lblDinheiroValue;
        private System.Windows.Forms.Label lblDinheiroTitle;
        private System.Windows.Forms.Panel pnlCredito;
        private System.Windows.Forms.Label lblCreditoValue;
        private System.Windows.Forms.Label lblCreditoTitle;
        private System.Windows.Forms.Panel pnlDebito;
        private System.Windows.Forms.Label lblDebitoValue;
        private System.Windows.Forms.Label lblDebitoTitle;
    }
}
