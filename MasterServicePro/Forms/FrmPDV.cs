using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public partial class FrmPDV : Form
    {
        private ProdutoRepository produtoRepository = new ProdutoRepository();
        private FinanceiroRepository financeiroRepository = new FinanceiroRepository();
        private VendaRepository vendaRepository = new VendaRepository();
        private BindingList<VendaItem> carrinho = new BindingList<VendaItem>();
        private decimal totalVenda = 0;
        
        private Label lblQtdItens;
        private Label lblSubtotal;
        private Label lblDescontos;
        private Label lblAcrescimos;

        public FrmPDV()
        {
            InitializeComponent();
            ApplyTheme();
            SetupResumoFinanceiro();
            SetupAtalhosECliente();
            SetupLastItemCard();
            SetupCart();
            ResetPDV();
            
            // Events
            txtBusca.KeyDown += TxtBusca_KeyDown;
            btnRemover.Click += BtnRemover_Click;
            btnCancelar.Click += BtnCancelar_Click;
            btnFinalizar.Click += BtnFinalizar_Click;
            
            this.KeyPreview = true;
            this.KeyDown += FrmPDV_KeyDown;
        }

        private void SetupCart()
        {
            dgvCarrinho.DataSource = carrinho;
            dgvCarrinho.Columns["Id"].Visible = false;
            dgvCarrinho.Columns["IdProduto"].Visible = false;
            
            dgvCarrinho.Columns["Nome"].HeaderText = "Produto";
            dgvCarrinho.Columns["PrecoUnitario"].HeaderText = "Unitário (R$)";
            dgvCarrinho.Columns["PrecoUnitario"].DefaultCellStyle.Format = "C2";
            dgvCarrinho.Columns["Quantidade"].HeaderText = "Qtd";
            dgvCarrinho.Columns["Total"].HeaderText = "Subtotal (R$)";
            dgvCarrinho.Columns["Total"].DefaultCellStyle.Format = "C2";
            
            dgvCarrinho.Columns["Nome"].FillWeight = 200;

            dgvCarrinho.SelectionChanged += (s, e) => AtualizarEstadoBotoes();
        }

        private void SetupResumoFinanceiro()
        {
            Panel pnlDetails = new Panel
            {
                Dock = DockStyle.Top,
                Height = 175, // Increased height to accommodate padding and separators
                Padding = new Padding(10),
                BackColor = UITheme.BgApp // Darker background to make it look like an inset table/receipt
            };
            
            lblAcrescimos = AddLinhaResumo(pnlDetails, "Acréscimos:");
            lblDescontos = AddLinhaResumo(pnlDetails, "Descontos:");
            lblSubtotal = AddLinhaResumo(pnlDetails, "Subtotal:");
            lblQtdItens = AddLinhaResumo(pnlDetails, "Qtd. Itens:");

            pnlSummary.Controls.Add(pnlDetails);
            pnlDetails.BringToFront();
        }

        private Label AddLinhaResumo(Panel parent, string texto)
        {
            Panel row = new Panel { Dock = DockStyle.Top, Height = 35, Padding = new Padding(5, 0, 5, 0) };
            
            Label lblValue = new Label { Text = texto == "Qtd. Itens:" ? "0" : "R$ 0,00", Dock = DockStyle.Right, AutoSize = false, Width = 120, ForeColor = UITheme.TextTitle, Font = UITheme.FontBodyBold, TextAlign = ContentAlignment.MiddleRight };
            Label lblTitle = new Label { Text = texto, Dock = DockStyle.Left, AutoSize = true, ForeColor = UITheme.TextMuted, Font = UITheme.FontBody, TextAlign = ContentAlignment.MiddleLeft };
            
            row.Controls.Add(lblTitle);
            row.Controls.Add(lblValue);
            
            parent.Controls.Add(row);
            row.BringToFront();

            // Adiciona uma linha fina separadora abaixo de cada item
            Panel separator = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = UITheme.ElementBorder };
            parent.Controls.Add(separator);
            separator.BringToFront();

            return lblValue;
        }

        private void SetupAtalhosECliente()
        {
            Panel pnlVazio = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10, 20, 10, 20)
            };

            // Cliente
            Button btnCliente = new Button
            {
                Text = "VINCULAR CLIENTE (CPF/CNPJ)",
                Dock = DockStyle.Top,
                Height = 45,
                FlatStyle = FlatStyle.Flat,
                ForeColor = UITheme.TextTitle,
                BackColor = UITheme.BgSidebar,
                Font = UITheme.FontBodyBold,
                Cursor = Cursors.Hand
            };
            btnCliente.FlatAppearance.BorderSize = 1;
            btnCliente.FlatAppearance.BorderColor = UITheme.ElementBorder;
            pnlVazio.Controls.Add(btnCliente);

            // Atalhos (Label)
            Label lblAtalhos = new Label
            {
                Text = "[F2] Buscar   •   [F5] Finalizar\n[DEL] Remover   •   [ESC] Cancelar",
                Dock = DockStyle.Bottom,
                Height = 50,
                ForeColor = UITheme.TextMuted,
                Font = UITheme.FontSmall,
                TextAlign = ContentAlignment.MiddleCenter
            };
            
            Label lblAtalhosTitle = new Label
            {
                Text = "ATALHOS DO SISTEMA",
                Dock = DockStyle.Bottom,
                Height = 25,
                ForeColor = UITheme.TextMuted,
                Font = UITheme.FontSmallBold,
                TextAlign = ContentAlignment.BottomCenter
            };

            pnlVazio.Controls.Add(lblAtalhosTitle);
            pnlVazio.Controls.Add(lblAtalhos);
            lblAtalhos.BringToFront();
            lblAtalhosTitle.BringToFront();

            pnlSummary.Controls.Add(pnlVazio);
            pnlVazio.BringToFront();
        }

        private void SetupLastItemCard()
        {
            Panel card = new Panel
            {
                Location = new Point(0, 80),
                Size = new Size(pnlSearch.Width, 75),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = UITheme.BgApp, // Fundo escuro como a tabela
            };

            Label lblIcon = new Label
            {
                Text = "🛒",
                Font = new Font("Segoe UI Emoji", 26F),
                Dock = DockStyle.Left,
                Width = 70,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = UITheme.TextMuted
            };

            pnlSearch.Controls.Remove(lblUltimoNome);
            pnlSearch.Controls.Remove(lblUltimoPreco);

            lblUltimoNome.Dock = DockStyle.Fill;
            lblUltimoNome.Location = new Point(0, 0);
            lblUltimoNome.Padding = new Padding(10, 0, 0, 0);
            lblUltimoNome.TextAlign = ContentAlignment.MiddleLeft; // Garante centralização vertical
            
            lblUltimoPreco.Dock = DockStyle.Right;
            lblUltimoPreco.Width = 220;
            lblUltimoPreco.TextAlign = ContentAlignment.MiddleRight; // Garante centralização vertical

            card.Controls.Add(lblUltimoNome);
            card.Controls.Add(lblUltimoPreco);
            card.Controls.Add(lblIcon);
            
            lblUltimoNome.BringToFront();

            pnlSearch.Controls.Add(card);
            
            // Draw a subtle border around the card
            card.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle, UITheme.ElementBorder, ButtonBorderStyle.Solid);
            };
        }

        private void ApplyTheme()
        {
            this.BackColor = UITheme.BgApp;
            pnlHeader.BackColor = UITheme.BgCard;
            tlpMain.BackColor = UITheme.BgApp;
            tlpContent.BackColor = UITheme.BgApp;
            pnlCart.BackColor = UITheme.BgCard;
            pnlSummary.BackColor = UITheme.BgCard;
            pnlSearch.BackColor = UITheme.BgCard;

            lblTitle.Font = UITheme.FontTitle;
            lblTitle.ForeColor = UITheme.TextTitle;

            // Exibir funcionário logado no topo direito do PDV
            Label lblUserStatus = new Label
            {
                Text = $"LOGADO COMO: {AuthSession.Usuario.ToUpper()} ({AuthSession.Nivel})",
                Dock = DockStyle.Right,
                Width = 350,
                ForeColor = UITheme.TextMuted,
                Font = UITheme.FontBodyBold,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 20, 0)
            };

            Button btnOpcoesCaixa = new Button
            {
                Text = "💵 Opções do Caixa",
                Dock = DockStyle.Right,
                Width = 180,
                FlatStyle = FlatStyle.Flat,
                Font = UITheme.FontBodyBold,
                Cursor = Cursors.Hand,
                BackColor = UITheme.BgApp, 
                ForeColor = UITheme.TextTitle
            };
            btnOpcoesCaixa.FlatAppearance.BorderSize = 0;
            btnOpcoesCaixa.Click += (s, e) =>
            {
                using (var frm = new FrmCaixaOpcoes())
                {
                    frm.ShowDialog();
                }
            };

            Button btnOS = new Button
            {
                Text = "🛠️ Ordem de Serviço",
                Dock = DockStyle.Right,
                Width = 200,
                FlatStyle = FlatStyle.Flat,
                Font = UITheme.FontBodyBold,
                Cursor = Cursors.Hand,
                BackColor = UITheme.BgApp,
                ForeColor = UITheme.TextTitle
            };
            btnOS.FlatAppearance.BorderSize = 0;
            btnOS.Click += (s, e) =>
            {
                using (var frm = new FrmOSWeb())
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.Size = new Size(1200, 750);
                    frm.ShowDialog();
                }
            };

            // Para ficarem do lado direito, adicionamos na ordem inversa
            pnlHeader.Controls.Add(btnOS);
            pnlHeader.Controls.Add(btnOpcoesCaixa);
            pnlHeader.Controls.Add(lblUserStatus);

            lblBusca.Font = UITheme.FontBodyBold;
            lblBusca.ForeColor = Color.LightSkyBlue; // Clareado para melhorar a leitura no fundo escuro

            txtBusca.BackColor = UITheme.BgSidebar;
            txtBusca.ForeColor = UITheme.TextTitle;
            txtBusca.BorderStyle = BorderStyle.FixedSingle;
            txtBusca.Font = new Font("Segoe UI", 16F, FontStyle.Regular); // Input maior

            lblTotalLabel.Font = UITheme.FontSubtitle;
            lblTotalLabel.ForeColor = UITheme.TextMuted;
            
            lblTotalValue.Font = new Font("Segoe UI", 36, FontStyle.Bold);
            lblTotalValue.ForeColor = UITheme.Primary;

            UITheme.FormatSaaSButton(btnFinalizar, true);
            UITheme.FormatSaaSButton(btnCancelar, false);
            btnCancelar.BackColor = UITheme.Danger;
            UITheme.FormatSaaSButton(btnRemover, false);
            
            // Grid Style
            dgvCarrinho.BackgroundColor = UITheme.BgApp; // Fundo escuro para dar contraste de tabela
            dgvCarrinho.BorderStyle = BorderStyle.None;
            dgvCarrinho.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvCarrinho.GridColor = UITheme.ElementBorder;
            dgvCarrinho.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvCarrinho.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 55, 75); // Cor mais clara para destacar o cabeçalho
            dgvCarrinho.ColumnHeadersDefaultCellStyle.ForeColor = UITheme.TextTitle;
            dgvCarrinho.ColumnHeadersDefaultCellStyle.Font = UITheme.FontBodyBold;
            dgvCarrinho.EnableHeadersVisualStyles = false;
            dgvCarrinho.DefaultCellStyle.BackColor = UITheme.BgCard;
            dgvCarrinho.DefaultCellStyle.ForeColor = UITheme.TextMuted;
            dgvCarrinho.DefaultCellStyle.SelectionBackColor = UITheme.PrimaryHover;
            dgvCarrinho.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(35, 47, 65);
            dgvCarrinho.RowHeadersVisible = false;
        }

        private void TxtBusca_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrWhiteSpace(txtBusca.Text))
                {
                    AdicionarProduto(txtBusca.Text);
                    txtBusca.Clear();
                    txtBusca.Focus();
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void AdicionarProduto(string codigo)
        {
            var produto = produtoRepository.BuscarPorCodigo(codigo);
            if (produto != null)
            {
                AdicionarAoCarrinho(produto);
            }
            else
            {
                // Se não achar por código exato, abre a busca avançada automaticamente com o termo digitado
                AbrirBuscaProduto(codigo);
            }
        }

        private void AdicionarAoCarrinho(Produto produto)
        {
            // Atualiza painel do último item
            lblUltimoNome.Text = produto.Nome.ToUpper();
            lblUltimoPreco.Text = produto.PrecoVenda.ToString("C2");

            // Check if item already in cart to increment quantity
            bool exists = false;
            foreach (var item in carrinho)
            {
                if (item.IdProduto == produto.Id)
                {
                    item.Quantidade += 1;
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                carrinho.Add(new VendaItem(produto));
            }

            dgvCarrinho.Refresh();
            AtualizarTotal();
        }

        private void ResetPDV()
        {
            carrinho.Clear();
            AtualizarTotal();
            lblUltimoNome.Text = "AGUARDANDO ITEM...";
            lblUltimoPreco.Text = "R$ 0,00";
            txtBusca.Clear();
            txtBusca.Focus();
        }

        private void AbrirBuscaProduto(string filtroInicial = "")
        {
            using (var frm = new FrmPDVBuscaProduto(filtroInicial))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    if (frm.ProdutoSelecionado != null)
                    {
                        AdicionarAoCarrinho(frm.ProdutoSelecionado);
                        txtBusca.Focus();
                    }
                }
            }
        }

        private void AtualizarTotal()
        {
            totalVenda = 0;
            decimal qtd = 0;
            foreach (var item in carrinho)
            {
                totalVenda += item.Total;
                qtd += item.Quantidade;
            }
            lblTotalValue.Text = totalVenda.ToString("C2");

            if (lblQtdItens != null)
            {
                lblQtdItens.Text = qtd.ToString();
                lblSubtotal.Text = totalVenda.ToString("C2");
                lblDescontos.Text = "R$ 0,00";
                lblAcrescimos.Text = "R$ 0,00";
            }

            AtualizarEstadoBotoes();
        }

        private void AtualizarEstadoBotoes()
        {
            bool temItens = carrinho.Count > 0;
            bool temSelecao = dgvCarrinho.SelectedRows.Count > 0;

            btnCancelar.Enabled = temItens;
            btnFinalizar.Enabled = temItens;
            btnRemover.Enabled = temSelecao;

            btnFinalizar.BackColor = temItens ? UITheme.Primary : Color.FromArgb(40, 50, 70);
            btnCancelar.BackColor = temItens ? UITheme.Danger : Color.FromArgb(40, 50, 70);
            btnRemover.BackColor = temSelecao ? UITheme.ElementBorder : Color.FromArgb(40, 50, 70);
        }

        private void BtnRemover_Click(object sender, EventArgs e)
        {
            if (carrinho.Count == 0)
            {
                MessageBox.Show("Não há itens na venda para remover.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvCarrinho.SelectedRows.Count > 0)
            {
                var item = (VendaItem)dgvCarrinho.SelectedRows[0].DataBoundItem;
                carrinho.Remove(item);
                AtualizarTotal();
            }
            else
            {
                MessageBox.Show("Selecione um item na lista para remover.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            if (carrinho.Count == 0)
            {
                MessageBox.Show("Não há itens na venda para cancelar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Deseja cancelar esta venda?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ResetPDV();
            }
        }

        private void BtnFinalizar_Click(object sender, EventArgs e)
        {
            if (carrinho.Count == 0)
            {
                MessageBox.Show("Adicione pelo menos um item para finalizar a venda.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Verifica se o caixa está aberto
            var caixa = financeiroRepository.ObterCaixaAberto();
            if (caixa == null)
            {
                using (var frmAlerta = new FrmPDVAlerta("Caixa Fechado", "O caixa está FECHADO!\nPor favor, abra o caixa no módulo Financeiro antes de realizar vendas."))
                {
                    frmAlerta.ShowDialog();
                }
                return;
            }

            // Verifica estoque antes de prosseguir
            foreach (var item in carrinho)
            {
                var produto = produtoRepository.BuscarPorId(item.IdProduto);
                if (produto != null && produto.Estoque < item.Quantidade)
                {
                    MessageBox.Show($"Estoque insuficiente para o produto '{produto.Nome}'.\nEstoque atual: {produto.Estoque}\nQuantidade solicitada: {item.Quantidade}", "Estoque Insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            using (var frm = new FrmPDVFinalizar(totalVenda))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    // 1. Salva o histórico da venda no MasterPDV (Vendas e ItensVenda)
                    int vendaIdGerada = 0;
                    try
                    {
                        string formaResumo = "Misto";
                        if (frm.ListaPagamentos != null && frm.ListaPagamentos.Count > 0)
                        {
                            formaResumo = frm.ListaPagamentos.Count > 1 ? "Misto" : frm.ListaPagamentos[0].Metodo;
                        }

                        vendaIdGerada = vendaRepository.SalvarVenda(
                            frm.SelectedClienteId,
                            AuthSession.Id,
                            totalVenda,
                            frm.DescontoAplicado,
                            frm.TotalVenda,
                            formaResumo,
                            new System.Collections.Generic.List<VendaItem>(carrinho)
                        );
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Atenção: A venda foi processada financeiramente, mas houve um erro ao salvar o histórico detalhado: " + ex.Message);
                    }

                    // 1.5. Saldo do Cliente
                    if (frm.ConverterTrocoEmSaldo && frm.SelectedClienteId > 0 && frm.Troco > 0)
                    {
                        var clienteRepo = new ClienteRepository();
                        clienteRepo.AdicionarSaldo(frm.SelectedClienteId, frm.Troco);
                    }

                    // 2. Lança cada forma de pagamento no Financeiro (ou Contas a Receber se for a prazo)
                    // Abater o troco FÍSICO para não lançar a mais no fluxo de caixa
                    decimal trocoRestante = frm.Troco;
                    bool deduzirTroco = (trocoRestante > 0 && !frm.ConverterTrocoEmSaldo);

                    if (deduzirTroco)
                    {
                        // Prioriza deduzir do Dinheiro
                        var pagDinheiro = frm.ListaPagamentos.FirstOrDefault(p => p.Metodo.Equals("Dinheiro", StringComparison.OrdinalIgnoreCase));
                        if (pagDinheiro != null)
                        {
                            if (pagDinheiro.Valor >= trocoRestante)
                            {
                                pagDinheiro.Valor -= trocoRestante;
                                trocoRestante = 0;
                            }
                            else
                            {
                                trocoRestante -= pagDinheiro.Valor;
                                pagDinheiro.Valor = 0;
                            }
                        }

                        // Se sobrar troco físico, deduz do que restar
                        foreach (var pag in frm.ListaPagamentos)
                        {
                            if (trocoRestante <= 0) break;
                            if (pag.Metodo.StartsWith("Saldo")) continue; // não desconta do saldo
                            
                            if (pag.Valor >= trocoRestante)
                            {
                                pag.Valor -= trocoRestante;
                                trocoRestante = 0;
                            }
                            else
                            {
                                trocoRestante -= pag.Valor;
                                pag.Valor = 0;
                            }
                        }
                    }

                    foreach (var pag in frm.ListaPagamentos)
                    {
                        if (pag.Valor <= 0) continue; // Pula se foi zerado pelo troco

                        if (pag.Metodo.StartsWith("Saldo do Cliente"))
                        {
                            var clienteRepo = new ClienteRepository();
                            clienteRepo.DescontarSaldo(frm.SelectedClienteId, pag.Valor);
                            continue;
                        }

                        if (pag.Metodo.IndexOf("Prazo", StringComparison.OrdinalIgnoreCase) >= 0 || pag.Metodo.IndexOf("Fiado", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            var contasReceberRepo = new ContasReceberRepository();
                            contasReceberRepo.Inserir(new ContaReceber
                            {
                                IdCliente = frm.SelectedClienteId,
                                Descricao = $"Venda PDV (Total Venda: {frm.TotalVenda:C2})",
                                ValorTotal = pag.Valor,
                                ValorPago = 0,
                                DataLancamento = DateTime.Now,
                                DataVencimento = DateTime.Now.AddDays(30),
                                Status = "Pendente"
                            });
                        }
                        else
                        {
                            financeiroRepository.LançarMovimentacao(new Movimentacao
                            {
                                IdCaixa = caixa.Id,
                                Tipo = "Entrada",
                                Categoria = "Venda PDV",
                                Subcategoria = "Venda de Produtos",
                                Valor = pag.Valor,
                                FormaPagamento = pag.Metodo,
                                Descricao = "Venda realizada no PDV"
                            });
                        }
                    }

                    // 3. Baixa de Estoque Automática
                    foreach (var item in carrinho)
                    {
                        produtoRepository.DeduzirEstoque(item.IdProduto, item.Quantidade);
                    }

                    // 4. Mostra o modal de sucesso personalizado
                    string formaFinal = "Venda";
                    if (frm.ListaPagamentos != null && frm.ListaPagamentos.Count > 0)
                    {
                        formaFinal = frm.ListaPagamentos.Count > 1 ? "Misto" : frm.ListaPagamentos[0].Metodo;
                    }
                    
                    using (var frmSucesso = new FrmPDVSucesso(formaFinal, frm.TotalVenda, frm.Troco))
                    {
                        frmSucesso.ShowDialog();
                    }

                    if (MessageBox.Show("Deseja imprimir o comprovante de venda?", "Imprimir Comprovante", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (vendaIdGerada > 0)
                        {
                            // Calcula valor recebido
                            decimal valorRecebido = frm.TotalVenda + frm.Troco;
                            new MasterServicePro.Utils.PDVPrinter().ImprimirVenda(vendaIdGerada, valorRecebido, frm.Troco, formaFinal);
                        }
                    }

                    ResetPDV();
                }
            }
        }

        private void FrmPDV_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5 && btnFinalizar.Enabled) BtnFinalizar_Click(null, null);
            if (e.KeyCode == Keys.Delete && btnRemover.Enabled) BtnRemover_Click(null, null);
            if (e.KeyCode == Keys.Escape && btnCancelar.Enabled) BtnCancelar_Click(null, null);
            if (e.KeyCode == Keys.F2) AbrirBuscaProduto();
        }
    }
}
