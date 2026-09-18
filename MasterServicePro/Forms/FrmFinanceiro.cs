using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public partial class FrmFinanceiro : Form
    {
        private FinanceiroRepository repository = new FinanceiroRepository();
        private Dictionary<DateTime, decimal> dadosGrafico = new Dictionary<DateTime, decimal>();
        
        // Dados para a distribuição de formas de pagamento
        private decimal pmDinheiro = 0;
        private decimal pmPix = 0;
        private decimal pmCredito = 0;
        private decimal pmDebito = 0;

        private Button btnSuprimento;
        private Button btnSangria;

        private Button btnContasReceber;
        private ComboBox cmbChartToggle;

        public FrmFinanceiro()
        {
            InitializeComponent();

            btnSuprimento = new Button { Text = "ADD DINHEIRO", Size = new Size(140, 35), Anchor = AnchorStyles.Top | AnchorStyles.Right, Location = new Point(btnHistorico.Left - 300, 12) };
            btnSangria = new Button { Text = "RETIRAR DINHEIRO", Size = new Size(150, 35), Anchor = AnchorStyles.Top | AnchorStyles.Right, Location = new Point(btnHistorico.Left - 155, 12) };

            btnContasReceber = new Button { Text = "CONTAS A RECEBER", Size = new Size(150, 35), Anchor = AnchorStyles.Top | AnchorStyles.Right, Location = new Point(btnHistorico.Left - 600, 12) };
            
            pnlHeader.Controls.Add(btnSuprimento);
            pnlHeader.Controls.Add(btnSangria);

            pnlHeader.Controls.Add(btnContasReceber);
            lblTitle.SendToBack();

            // Chart display selector (Top-Right on Chart Panel)
            cmbChartToggle = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Size = new Size(200, 25),
                Location = new Point(460, 5),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            cmbChartToggle.Items.AddRange(new string[] { "Meios de Pagamento", "Subcategorias (Entradas)", "Subcategorias (Saídas)" });
            cmbChartToggle.SelectedIndex = 0;
            cmbChartToggle.SelectedIndexChanged += (s, e) => pnlChart.Invalidate();
            pnlChart.Controls.Add(cmbChartToggle);

            ApplyTheme();
            LoadData();

            btnAbrirCaixa.Click += BtnAbrirCaixa_Click;
            btnFecharCaixa.Click += BtnFecharCaixa_Click;
            btnHistorico.Click += BtnHistorico_Click;
            btnSuprimento.Click += (s, e) => LançarMovimentacao("Suprimento");
            btnSangria.Click += (s, e) => LançarMovimentacao("Sangria");

            btnContasReceber.Click += BtnContasReceber_Click;
            pnlChart.Paint += PnlChart_Paint;
            dtpInicio.ValueChanged += (s, e) => LoadData();
            dtpFim.ValueChanged += (s, e) => LoadData();
        }

        private void BtnContasReceber_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmContasReceberWeb())
            {
                frm.ShowDialog();
            }
        }

        private void ConfigurePremiumCard(Panel pnl, Color accentColor)
        {
            pnl.BackColor = UITheme.BgApp;
            pnl.BorderStyle = BorderStyle.None;
            pnl.Padding = new Padding(18, 10, 10, 10);
            
            pnl.Paint += (s, e) => {
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                
                // Draw rounded panel background
                UITheme.DrawRoundedPanel(g, pnl.ClientRectangle, UITheme.BgCard, 10, UITheme.ElementBorder);
                
                // Draw vertical indicator on the left
                using (var brush = new SolidBrush(accentColor))
                {
                    int radius = 4;
                    Rectangle rectAccent = new Rectangle(6, 12, 5, pnl.Height - 24);
                    using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
                    {
                        path.AddArc(rectAccent.X, rectAccent.Y, radius, radius, 180, 90);
                        path.AddArc(rectAccent.Right - radius, rectAccent.Y, radius, radius, 270, 90);
                        path.AddArc(rectAccent.Right - radius, rectAccent.Bottom - radius, radius, radius, 0, 90);
                        path.AddArc(rectAccent.X, rectAccent.Bottom - radius, radius, radius, 90, 90);
                        path.CloseFigure();
                        g.FillPath(brush, path);
                    }
                }
            };
        }

        private void ApplyTheme()
        {
            this.BackColor = UITheme.BgApp;
            pnlHeader.BackColor = UITheme.BgApp;
            
            pnlChart.BackColor = UITheme.BgApp; // Background da form
            pnlChart.BorderStyle = BorderStyle.None;
            pnlChart.Paint += (s, e) => {
                // Desenha o fundo arredondado antes do gráfico
                UITheme.DrawRoundedPanel(e.Graphics, pnlChart.ClientRectangle, UITheme.BgCard, 10, UITheme.ElementBorder);
            };

            lblChartTitle.ForeColor = UITheme.TextTitle;
            lblChartTitle.Font = UITheme.FontSubtitle;

            pnlHeader.BackColor = UITheme.BgCard;

            lblTitle.Font = UITheme.FontTitle;
            lblTitle.ForeColor = UITheme.TextTitle;

            // Stats Cards Premium Configuration
            ConfigurePremiumCard(pnlAbertura, UITheme.Warning);
            ConfigurePremiumCard(pnlEntradas, UITheme.Success);
            ConfigurePremiumCard(pnlSaidas, UITheme.Danger);
            ConfigurePremiumCard(pnlSaldo, UITheme.Primary);
            ConfigurePremiumCard(pnlPix, UITheme.Primary);
            ConfigurePremiumCard(pnlDinheiro, UITheme.Success);
            ConfigurePremiumCard(pnlCredito, UITheme.Warning);
            ConfigurePremiumCard(pnlDebito, UITheme.Primary);

            // Fonts and Colors
            Font valueFont = new Font("Segoe UI", 16F, FontStyle.Bold);

            lblEntradasTitle.Font = UITheme.FontSmallBold;
            lblEntradasTitle.ForeColor = UITheme.TextMuted;
            lblEntradasValue.Font = valueFont;
            lblEntradasValue.ForeColor = UITheme.Success;

            lblSaidasTitle.Font = UITheme.FontSmallBold;
            lblSaidasTitle.ForeColor = UITheme.TextMuted;
            lblSaidasValue.Font = valueFont;
            lblSaidasValue.ForeColor = UITheme.Danger;

            lblAberturaTitle.Font = UITheme.FontSmallBold;
            lblAberturaTitle.ForeColor = UITheme.TextMuted;
            lblAberturaValue.Font = valueFont;
            lblAberturaValue.ForeColor = UITheme.Warning;

            lblPixTitle.Font = UITheme.FontSmallBold;
            lblPixTitle.ForeColor = UITheme.TextMuted;
            lblPixValue.Font = valueFont;
            lblPixValue.ForeColor = UITheme.Primary;

            lblDinheiroTitle.Font = UITheme.FontSmallBold;
            lblDinheiroTitle.ForeColor = UITheme.TextMuted;
            lblDinheiroValue.Font = valueFont;
            lblDinheiroValue.ForeColor = UITheme.Success;

            lblCreditoTitle.Font = UITheme.FontSmallBold;
            lblCreditoTitle.ForeColor = UITheme.TextMuted;
            lblCreditoValue.Font = valueFont;
            lblCreditoValue.ForeColor = UITheme.Warning;

            lblDebitoTitle.Font = UITheme.FontSmallBold;
            lblDebitoTitle.ForeColor = UITheme.TextMuted;
            lblDebitoValue.Font = valueFont;
            lblDebitoValue.ForeColor = UITheme.Primary;

            lblSaldoTitle.Font = UITheme.FontSmallBold;
            lblSaldoTitle.ForeColor = UITheme.TextMuted;
            lblSaldoValue.Font = valueFont;
            lblSaldoValue.ForeColor = UITheme.Primary;

            UITheme.FormatSaaSButton(btnAbrirCaixa, true);
            UITheme.FormatSaaSButton(btnFecharCaixa, false);
            UITheme.FormatSaaSButton(btnHistorico, false);
            UITheme.FormatSaaSButton(btnSuprimento, false);
            UITheme.FormatSaaSButton(btnSangria, false);

            
            btnAbrirCaixa.BackColor = UITheme.Success;
            btnFecharCaixa.BackColor = UITheme.Danger;
            btnSuprimento.BackColor = UITheme.Success;
            btnSangria.BackColor = UITheme.Warning;


            UITheme.FormatSaaSButton(btnContasReceber, false);
            btnContasReceber.BackColor = UITheme.Warning;
            btnContasReceber.ForeColor = Color.White;

            cmbChartToggle.BackColor = UITheme.BgSidebar;
            cmbChartToggle.ForeColor = UITheme.TextTitle;
            cmbChartToggle.FlatStyle = FlatStyle.Flat;
            cmbChartToggle.Font = UITheme.FontSmallBold;

            // Grid
            UITheme.FormatGrid(dgvFluxo);
        }

        private void LoadData()
        {
            DateTime inicio = dtpInicio.Value;
            DateTime fim = dtpFim.Value;

            // O Grid (Lista) continua respeitando o filtro de data para consultas
            DataTable dt = repository.GetFluxoCaixa(inicio, fim);
            dgvFluxo.DataSource = dt;

            if (dgvFluxo.Columns.Count > 0)
            {
                try
                {
                    if (dgvFluxo.Columns["Id"] != null) dgvFluxo.Columns["Id"].Visible = false;
                    if (dgvFluxo.Columns["IdCaixa"] != null) dgvFluxo.Columns["IdCaixa"].Visible = false;
                    if (dgvFluxo.Columns["Valor"] != null) dgvFluxo.Columns["Valor"].DefaultCellStyle.Format = "C2";
                    if (dgvFluxo.Columns["Data"] != null) dgvFluxo.Columns["Data"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
                catch { }
            }

            // --- Lógica dos Cards (Focada na Sessão Atual) ---
            var caixa = repository.ObterCaixaAberto();

            try
            {
                if (caixa != null)
                {
                    // Calcula totais APENAS desta sessão (desde a abertura até agora)
                    DateTime dataAbertura = caixa.DataAbertura;
                    DateTime agora = DateTime.Now;

                    decimal entradas = repository.GetTotalPorPeriodo(dataAbertura, agora, "Entrada");
                    decimal saidas = repository.GetTotalPorPeriodo(dataAbertura, agora, "Saída");
                    
                    lblAberturaValue.Text = caixa.ValorAbertura.ToString("C2");
                    lblAberturaValue.ForeColor = UITheme.Warning;
                    lblAberturaTitle.Text = "SALDO INICIAL (ABERTO)";

                    decimal dinEntradas = repository.GetEntradasPorFormaPagamento(dataAbertura, agora, "Dinheiro");
                    decimal dinSaidas = repository.GetSaidasPorFormaPagamento(dataAbertura, agora, "Dinheiro");

                    lblEntradasValue.Text = entradas.ToString("C2");
                    lblSaidasValue.Text = saidas.ToString("C2");
                    // Physical cash in drawer = Opening balance + Cash in - Cash out
                    lblSaldoValue.Text = (caixa.ValorAbertura + dinEntradas - dinSaidas).ToString("C2");
                    lblSaldoTitle.Text = "DINHEIRO NO CAIXA";

                    lblPixValue.Text = repository.GetEntradasPorFormaPagamento(dataAbertura, agora, "Pix").ToString("C2");
                    lblDinheiroValue.Text = dinEntradas.ToString("C2");
                    lblCreditoValue.Text = repository.GetEntradasPorFormaPagamento(dataAbertura, agora, "Cartão de Crédito").ToString("C2");
                    lblDebitoValue.Text = repository.GetEntradasPorFormaPagamento(dataAbertura, agora, "Cartão de Débito").ToString("C2");
                }
                else
                {
                    // Caixa Fechado: Zera todos os painéis
                    lblAberturaValue.Text = "R$ 0,00";
                    lblAberturaValue.ForeColor = UITheme.TextMuted;
                    lblAberturaTitle.Text = "CAIXA FECHADO";

                    lblEntradasValue.Text = "R$ 0,00";
                    lblSaidasValue.Text = "R$ 0,00";
                    lblSaldoValue.Text = "R$ 0,00";
                    lblPixValue.Text = "R$ 0,00";
                    lblDinheiroValue.Text = "R$ 0,00";
                    lblCreditoValue.Text = "R$ 0,00";
                    lblDebitoValue.Text = "R$ 0,00";
                }
            }
            catch { }

            btnAbrirCaixa.Enabled = (caixa == null);
            btnFecharCaixa.Enabled = (caixa != null);
            btnSuprimento.Enabled = (caixa != null);
            btnSangria.Enabled = (caixa != null);

            // Atualiza Gráfico de Vendas
            dadosGrafico = repository.GetVendasUltimos7Dias();

            // Atualiza distribuição de meios de pagamento no período selecionado (inicio e fim)
            try
            {
                pmDinheiro = repository.GetTotalPorFormaPagamento(inicio, fim, "Dinheiro");
                pmPix = repository.GetTotalPorFormaPagamento(inicio, fim, "Pix");
                pmCredito = repository.GetTotalPorFormaPagamento(inicio, fim, "Cartão de Crédito");
                pmDebito = repository.GetTotalPorFormaPagamento(inicio, fim, "Cartão de Débito");
            }
            catch
            {
                pmDinheiro = pmPix = pmCredito = pmDebito = 0;
            }

            pnlChart.Invalidate();
        }

        private void PnlChart_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Dividir área total: Esquerda (Tendência 7 dias), Direita (Breakdown Meios de Pagamento)
            int dividerX = pnlChart.Width / 2;

            // Desenhar Linha Divisória Vertical sutil
            using (var pen = new Pen(UITheme.ElementBorder, 1f))
            {
                g.DrawLine(pen, dividerX, 15, dividerX, pnlChart.Height - 15);
            }

            // ==========================================
            // LADO ESQUERDO: GRÁFICO DE ÁREA SUAVIZADA (TENDÊNCIA 7 DIAS)
            // ==========================================
            if (dadosGrafico != null && dadosGrafico.Count > 0)
            {
                int leftPadding = 35;
                int leftWidth = dividerX - (leftPadding * 2);
                int leftHeight = pnlChart.Height - 65;

                decimal maxValor = 0;
                foreach (var v in dadosGrafico.Values) if (v > maxValor) maxValor = v;
                if (maxValor == 0) maxValor = 100;

                int stepX = leftWidth / 6;
                int x = leftPadding;

                // Construir pontos para a curva de área
                PointF[] points = new PointF[dadosGrafico.Count];
                int idx = 0;
                foreach (var item in dadosGrafico)
                {
                    float yVal = (float)(leftHeight * (item.Value / maxValor));
                    float y = (pnlChart.Height - 35) - yVal;
                    points[idx] = new PointF(x, y);
                    x += stepX;
                    idx++;
                }

                // Desenhar área preenchida com gradiente semitransparente
                if (points.Length > 1)
                {
                    using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
                    {
                        path.AddLine(points[0].X, pnlChart.Height - 30, points[0].X, points[0].Y);
                        for (int i = 0; i < points.Length - 1; i++)
                        {
                            // Desenha curva suave Bézier
                            float cX1 = points[i].X + (stepX / 2f);
                            float cY1 = points[i].Y;
                            float cX2 = points[i+1].X - (stepX / 2f);
                            float cY2 = points[i+1].Y;
                            path.AddBezier(points[i], new PointF(cX1, cY1), new PointF(cX2, cY2), points[i+1]);
                        }
                        path.AddLine(points[points.Length - 1].X, points[points.Length - 1].Y, points[points.Length - 1].X, pnlChart.Height - 30);
                        path.CloseFigure();

                        using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                            new Rectangle(leftPadding, 10, leftWidth, pnlChart.Height - 40),
                            Color.FromArgb(80, UITheme.Primary),
                            Color.FromArgb(0, UITheme.Primary),
                            90f))
                        {
                            g.FillPath(brush, path);
                        }
                    }

                    // Desenhar a linha contínua suavizada
                    using (var pen = new Pen(UITheme.Primary, 2.5f))
                    {
                        System.Drawing.Drawing2D.GraphicsPath linePath = new System.Drawing.Drawing2D.GraphicsPath();
                        for (int i = 0; i < points.Length - 1; i++)
                        {
                            float cX1 = points[i].X + (stepX / 2f);
                            float cY1 = points[i].Y;
                            float cX2 = points[i+1].X - (stepX / 2f);
                            float cY2 = points[i+1].Y;
                            linePath.AddBezier(points[i], new PointF(cX1, cY1), new PointF(cX2, cY2), points[i+1]);
                        }
                        g.DrawPath(pen, linePath);
                    }
                }

                // Desenhar rótulos de dados e marcadores redondos
                idx = 0;
                foreach (var item in dadosGrafico)
                {
                    PointF pt = points[idx];
                    
                    // Círculo no vértice
                    using (var brush = new SolidBrush(UITheme.BgCard))
                    using (var borderPen = new Pen(UITheme.Primary, 2.5f))
                    {
                        g.FillEllipse(brush, pt.X - 5, pt.Y - 5, 10, 10);
                        g.DrawEllipse(borderPen, pt.X - 5, pt.Y - 5, 10, 10);
                    }

                    // Rótulo da data
                    string dateLabel = item.Key.ToString("dd/MM");
                    SizeF size = g.MeasureString(dateLabel, UITheme.FontSmallBold);
                    g.DrawString(dateLabel, UITheme.FontSmallBold, new SolidBrush(UITheme.TextMuted), pt.X - (size.Width / 2f), pnlChart.Height - 25);

                    // Rótulo do valor acima do ponto
                    if (item.Value > 0)
                    {
                        string valLabel = item.Value.ToString("N0");
                        SizeF valSize = g.MeasureString(valLabel, UITheme.FontSmallBold);
                        g.DrawString(valLabel, UITheme.FontSmallBold, new SolidBrush(UITheme.TextTitle), pt.X - (valSize.Width / 2f), pt.Y - 20);
                    }

                    idx++;
                }

                // Título esquerdo
                g.DrawString("Faturamento Diário (Últimos 7 Dias)", UITheme.FontSmallBold, new SolidBrush(UITheme.TextTitle), leftPadding, 10);
            }

            // ==========================================
            // LADO DIREITO: DISTRIBUIÇÃO MÉTODOS OU SUBCATEGORIAS
            // ==========================================
            int rightPadding = 20;
            int rightX = dividerX + rightPadding;
            int rightWidth = pnlChart.Width - rightX - rightPadding;

            int chartType = cmbChartToggle.SelectedIndex; // 0: Meios, 1: Subcat Entrada, 2: Subcat Saida

            if (chartType == 0)
            {
                g.DrawString("Faturamento por Meio de Pagamento", UITheme.FontSmallBold, new SolidBrush(UITheme.TextTitle), rightX, 10);

                decimal totalMeios = pmDinheiro + pmPix + pmCredito + pmDebito;
                string[] nomes = { "Pix", "Dinheiro", "Cartão de Crédito", "Cartão de Débito" };
                decimal[] valores = { pmPix, pmDinheiro, pmCredito, pmDebito };
                Color[] cores = { UITheme.Primary, UITheme.Success, UITheme.Warning, Color.DeepSkyBlue };

                int yOffset = 38;
                for (int i = 0; i < 4; i++)
                {
                    decimal valor = valores[i];
                    double percent = totalMeios > 0 ? (double)(valor / totalMeios) : 0;
                    string label = $"{nomes[i]} ({valor:C2})";
                    
                    // Texto do rótulo
                    g.DrawString(label, UITheme.FontSmall, new SolidBrush(UITheme.TextMuted), rightX, yOffset);

                    // Texto da porcentagem à direita
                    string pctText = $"{percent * 100:F1}%";
                    SizeF pctSize = g.MeasureString(pctText, UITheme.FontSmallBold);
                    g.DrawString(pctText, UITheme.FontSmallBold, new SolidBrush(UITheme.TextTitle), rightX + rightWidth - pctSize.Width, yOffset);

                    // Desenhar barra de fundo cinza
                    int barY = yOffset + 15;
                    int barHeight = 8;
                    using (var bgBrush = new SolidBrush(Color.FromArgb(35, 35, 35)))
                    {
                        System.Drawing.Drawing2D.GraphicsPath barPath = new System.Drawing.Drawing2D.GraphicsPath();
                        Rectangle rectBg = new Rectangle(rightX, barY, rightWidth, barHeight);
                        barPath.AddArc(rectBg.X, rectBg.Y, barHeight, barHeight, 180, 90);
                        barPath.AddArc(rectBg.Right - barHeight, rectBg.Y, barHeight, barHeight, 270, 90);
                        barPath.AddArc(rectBg.Right - barHeight, rectBg.Bottom - barHeight, barHeight, barHeight, 0, 90);
                        barPath.AddArc(rectBg.X, rectBg.Bottom - barHeight, barHeight, barHeight, 90, 90);
                        barPath.CloseFigure();
                        g.FillPath(bgBrush, barPath);
                    }

                    // Desenhar barra preenchida se percent > 0
                    if (percent > 0)
                    {
                        int fillWidth = (int)(rightWidth * percent);
                        if (fillWidth < barHeight) fillWidth = barHeight;
                        using (var fillBrush = new SolidBrush(cores[i]))
                        {
                            System.Drawing.Drawing2D.GraphicsPath fillPath = new System.Drawing.Drawing2D.GraphicsPath();
                            Rectangle rectFill = new Rectangle(rightX, barY, fillWidth, barHeight);
                            fillPath.AddArc(rectFill.X, rectFill.Y, barHeight, barHeight, 180, 90);
                            fillPath.AddArc(rectFill.Right - barHeight, rectFill.Y, barHeight, barHeight, 270, 90);
                            fillPath.AddArc(rectFill.Right - barHeight, rectFill.Bottom - barHeight, barHeight, barHeight, 0, 90);
                            fillPath.AddArc(rectFill.X, rectFill.Bottom - barHeight, barHeight, barHeight, 90, 90);
                            fillPath.CloseFigure();
                            g.FillPath(fillBrush, fillPath);
                        }
                    }

                    yOffset += 32;
                }
            }
            else
            {
                string tipo = chartType == 1 ? "Entrada" : "Saída";
                g.DrawString($"Distribuição de {tipo}s por Subcategoria", UITheme.FontSmallBold, new SolidBrush(UITheme.TextTitle), rightX, 10);

                var subtotais = repository.GetTotaisPorSubcategoria(dtpInicio.Value, dtpFim.Value, tipo == "Entrada" ? "Entrada" : "Saída");
                decimal totalSub = 0;
                foreach (var v in subtotais.Values) totalSub += v;

                // Sort by value descending and take top 4
                var topSubcategories = System.Linq.Enumerable.ToList(System.Linq.Enumerable.Take(System.Linq.Enumerable.OrderByDescending(subtotais, kv => kv.Value), 4));

                Color[] cores = { UITheme.Primary, UITheme.Warning, UITheme.Danger, UITheme.Success };

                int yOffset = 38;
                if (topSubcategories.Count == 0)
                {
                    g.DrawString("Nenhum lançamento no período.", UITheme.FontSmall, new SolidBrush(UITheme.TextMuted), rightX, yOffset + 20);
                }
                else
                {
                    for (int i = 0; i < topSubcategories.Count; i++)
                    {
                        var item = topSubcategories[i];
                        string label = $"{item.Key} ({item.Value:C2})";
                        double percent = totalSub > 0 ? (double)(item.Value / totalSub) : 0;

                        // Texto do rótulo
                        g.DrawString(label, UITheme.FontSmall, new SolidBrush(UITheme.TextMuted), rightX, yOffset);

                        // Texto da porcentagem à direita
                        string pctText = $"{percent * 100:F1}%";
                        SizeF pctSize = g.MeasureString(pctText, UITheme.FontSmallBold);
                        g.DrawString(pctText, UITheme.FontSmallBold, new SolidBrush(UITheme.TextTitle), rightX + rightWidth - pctSize.Width, yOffset);

                        // Desenhar barra de fundo cinza
                        int barY = yOffset + 15;
                        int barHeight = 8;
                        using (var bgBrush = new SolidBrush(Color.FromArgb(35, 35, 35)))
                        {
                            System.Drawing.Drawing2D.GraphicsPath barPath = new System.Drawing.Drawing2D.GraphicsPath();
                            Rectangle rectBg = new Rectangle(rightX, barY, rightWidth, barHeight);
                            barPath.AddArc(rectBg.X, rectBg.Y, barHeight, barHeight, 180, 90);
                            barPath.AddArc(rectBg.Right - barHeight, rectBg.Y, barHeight, barHeight, 270, 90);
                            barPath.AddArc(rectBg.Right - barHeight, rectBg.Bottom - barHeight, barHeight, barHeight, 0, 90);
                            barPath.AddArc(rectBg.X, rectBg.Bottom - barHeight, barHeight, barHeight, 90, 90);
                            barPath.CloseFigure();
                            g.FillPath(bgBrush, barPath);
                        }

                        // Desenhar barra preenchida
                        if (percent > 0)
                        {
                            int fillWidth = (int)(rightWidth * percent);
                            if (fillWidth < barHeight) fillWidth = barHeight;
                            using (var fillBrush = new SolidBrush(cores[i % cores.Length]))
                            {
                                System.Drawing.Drawing2D.GraphicsPath fillPath = new System.Drawing.Drawing2D.GraphicsPath();
                                Rectangle rectFill = new Rectangle(rightX, barY, fillWidth, barHeight);
                                fillPath.AddArc(rectFill.X, rectFill.Y, barHeight, barHeight, 180, 90);
                                fillPath.AddArc(rectFill.Right - barHeight, rectFill.Y, barHeight, barHeight, 270, 90);
                                fillPath.AddArc(rectFill.Right - barHeight, rectFill.Bottom - barHeight, barHeight, barHeight, 0, 90);
                                fillPath.AddArc(rectFill.X, rectFill.Bottom - barHeight, barHeight, barHeight, 90, 90);
                                fillPath.CloseFigure();
                                g.FillPath(fillBrush, fillPath);
                            }
                        }

                        yOffset += 32;
                    }
                }
            }
        }

        private void LoadDataHistorico(DateTime inicio, DateTime fim, decimal vlrAbertura)
        {
            // Filtra o Grid
            dgvFluxo.DataSource = repository.GetFluxoCaixa(inicio, fim);
            
            // Atualiza os Cards com os dados daquela sessão específica
            lblAberturaValue.Text = vlrAbertura.ToString("C2");
            lblAberturaValue.ForeColor = UITheme.Primary;
            lblAberturaTitle.Text = "VISUALIZANDO CAIXA ANTIGO";

            decimal entradas = repository.GetTotalPorPeriodo(inicio, fim, "Entrada");
            decimal saidas = repository.GetTotalPorPeriodo(inicio, fim, "Saída");

            lblEntradasValue.Text = entradas.ToString("C2");
            lblSaidasValue.Text = saidas.ToString("C2");
            decimal histDinEntradas = repository.GetEntradasPorFormaPagamento(inicio, fim, "Dinheiro");
            decimal histDinSaidas = repository.GetSaidasPorFormaPagamento(inicio, fim, "Dinheiro");
            lblSaldoValue.Text = (vlrAbertura + histDinEntradas - histDinSaidas).ToString("C2");
            lblSaldoTitle.Text = "DINHEIRO NO CAIXA";

            lblPixValue.Text = repository.GetEntradasPorFormaPagamento(inicio, fim, "Pix").ToString("C2");
            lblDinheiroValue.Text = histDinEntradas.ToString("C2");
            lblCreditoValue.Text = repository.GetEntradasPorFormaPagamento(inicio, fim, "Cartão de Crédito").ToString("C2");
            lblDebitoValue.Text = repository.GetEntradasPorFormaPagamento(inicio, fim, "Cartão de Débito").ToString("C2");

            // Block actions while viewing history
            btnAbrirCaixa.Enabled = false;
            btnFecharCaixa.Enabled = false;

            try
            {
                pmDinheiro = repository.GetTotalPorFormaPagamento(inicio, fim, "Dinheiro");
                pmPix = repository.GetTotalPorFormaPagamento(inicio, fim, "Pix");
                pmCredito = repository.GetTotalPorFormaPagamento(inicio, fim, "Cartão de Crédito");
                pmDebito = repository.GetTotalPorFormaPagamento(inicio, fim, "Cartão de Débito");
            }
            catch
            {
                pmDinheiro = pmPix = pmCredito = pmDebito = 0;
            }

            pnlChart.Invalidate();
            
            MessageBox.Show("Você está visualizando um caixa antigo. Clique em qualquer data no calendário para voltar ao painel atual.", "Modo Histórico", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnHistorico_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmCaixaHistorico())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    LoadDataHistorico(frm.DataInicioSelecionada, frm.DataFimSelecionada, frm.ValorAberturaSelecionado);
                }
            }
        }

        private void BtnAbrirCaixa_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmCaixaAberturaFechamento(true))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    repository.AbrirCaixa(frm.Valor);
                    LoadData();
                }
            }
        }

        private async void BtnFecharCaixa_Click(object sender, EventArgs e)
        {
            var caixa = repository.ObterCaixaAberto();
            if (caixa == null) return;

            decimal totalVendas = repository.GetTotalPorPeriodo(caixa.DataAbertura, DateTime.Now, "Entrada");
            decimal totalSaidas = repository.GetTotalPorPeriodo(caixa.DataAbertura, DateTime.Now, "Saída");
            decimal saldoFinal = caixa.ValorAbertura + totalVendas - totalSaidas;

            using (var frm = new FrmCaixaAberturaFechamento(false))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    repository.FecharCaixa(caixa.Id, frm.Valor);
                    LoadData();
                    
                    // Envia relatório WhatsApp de forma assíncrona
                    await new Services.WhatsAppService().EnviarRelatorioFechamentoCaixaAsync(caixa, repository);
                }
            }
        }

        private void LançarMovimentacao(string tipo)
        {
            var caixa = repository.ObterCaixaAberto();
            if (caixa == null)
            {
                MessageBox.Show("O caixa precisa estar aberto para registrar movimentações.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var frm = new FrmCaixaMovimentacaoWeb(tipo))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    var mov = frm.Movimentacao;
                    mov.IdCaixa = caixa.Id;
                    repository.LançarMovimentacao(mov);
                    LoadData();
                }
            }
        }
    }
}
