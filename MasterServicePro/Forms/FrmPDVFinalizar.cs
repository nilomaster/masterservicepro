using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public partial class FrmPDVFinalizar : Form
    {
        public decimal SubtotalVenda { get; set; }
        public decimal DescontoAplicado { get; set; } = 0;
        public decimal TotalVenda { get; set; }
        public decimal Troco { get; set; }
        public BindingList<PagamentoItem> ListaPagamentos { get; set; } = new BindingList<PagamentoItem>();
        public bool VendaFinalizada { get; set; }
        public int SelectedClienteId { get; set; }
        public bool ConverterTrocoEmSaldo { get; set; }
        private ClienteRepository clienteRepo = new ClienteRepository();
        private List<Cliente> clientes;
        private CheckBox chkConverterSaldo;
        private decimal clienteSaldoAtual = 0;
        
        // Novos controles para Desconto
        private Label lblDescontoLabel;
        private TextBox txtDesconto;

        public FrmPDVFinalizar(decimal total)
        {
            InitializeComponent();
            SubtotalVenda = total;
            TotalVenda = total;
            VendaFinalizada = false;

            lblDescontoLabel = new Label { Text = "Desconto (R$):", AutoSize = true };
            txtDesconto = new TextBox { Text = "0,00" };

            ApplyTheme();
            
            this.Size = new Size(1200, 750);
            this.Load += (s, e) => {
                pnlContainer.Location = new Point((this.Width - pnlContainer.Width) / 2, (this.Height - pnlContainer.Height) / 2);
            };

            SetupModernUI();
            SetupGrid();
            
            lblTotalValue.Text = TotalVenda.ToString("C2");
            txtRecebido.Text = TotalVenda.ToString("N2");
            
            chkConverterSaldo = new CheckBox
            {
                Text = "Converter Troco em Saldo",
                Location = new Point(30, 680), // Movido mais para baixo
                AutoSize = true,
                Visible = false,
                ForeColor = UITheme.TextMuted,
                Font = UITheme.FontBodyBold,
                Cursor = Cursors.Hand
            };
            pnlContainer.Controls.Add(chkConverterSaldo);

            cboPagamento.SelectedIndex = 0; // Dinheiro
            
            cboCliente.SelectedIndexChanged += CboCliente_SelectedIndexChanged;
            
            txtDesconto.Leave += TxtDesconto_Leave;
            txtDesconto.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter) {
                    TxtDesconto_Leave(null, null);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };

            btnAddPagamento.Click += BtnAddPagamento_Click;
            btnConfirmar.Click += BtnConfirmar_Click;
            btnCancelar.Click += (s, e) => this.Close();
            
            txtRecebido.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter) {
                    BtnAddPagamento_Click(null, null);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };

            this.Shown += (s, e) => {
                txtRecebido.Focus();
                txtRecebido.SelectAll();
            };

            AtualizarSaldos();
            this.Load += async (s, e) => await CarregarClientesAsync();
        }

        private void CboCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCliente.SelectedValue != null && int.TryParse(cboCliente.SelectedValue.ToString(), out int cId))
            {
                SelectedClienteId = cId;
                var cliente = clientes?.FirstOrDefault(c => c.Id == cId);
                clienteSaldoAtual = cliente?.Saldo ?? 0;
                
                lblSaldoDisponivel.Text = $"Saldo Disponível: {clienteSaldoAtual:C2}";
                lblSaldoDisponivel.Visible = clienteSaldoAtual > 0;
                
                AtualizarMetodosPagamento();
                AtualizarSaldos();
            }
        }

        private void AtualizarMetodosPagamento()
        {
            string current = cboPagamento.Text;
            cboPagamento.Items.Clear();
            cboPagamento.Items.AddRange(new string[] { "Dinheiro", "Cartão de Crédito", "Cartão de Débito", "PIX", "A Prazo (Fiado)" });
            
            if (clienteSaldoAtual > 0)
            {
                cboPagamento.Items.Add($"Saldo do Cliente (R$ {clienteSaldoAtual:N2})");
            }
            
            if (cboPagamento.Items.Contains(current))
                cboPagamento.Text = current;
            else
                cboPagamento.SelectedIndex = 0;
        }

        private async System.Threading.Tasks.Task CarregarClientesAsync()
        {
            try
            {
                clientes = await clienteRepo.BuscarTodosAsync();
                
                // Adiciona Consumidor Final se não existir (ou como opção padrão)
                var lista = new List<object>();
                lista.Add(new { Id = 0, Nome = "CONSUMIDOR FINAL", Saldo = 0m });
                
                foreach (var c in clientes)
                {
                    lista.Add(new { Id = c.Id, Nome = c.Nome.ToUpper(), Saldo = c.Saldo });
                }

                cboCliente.DataSource = lista;
                cboCliente.DisplayMember = "Nome";
                cboCliente.ValueMember = "Id";
                cboCliente.SelectedIndex = 0;
            }
            catch { }
        }

        private void SetupGrid()
        {
            dgvPagamentos.DataSource = ListaPagamentos;
            dgvPagamentos.Columns["Metodo"].HeaderText = "Método";
            dgvPagamentos.Columns["Valor"].HeaderText = "Valor (R$)";
            dgvPagamentos.Columns["Valor"].DefaultCellStyle.Format = "C2";
            
            // Estilo do Grid
            dgvPagamentos.BackgroundColor = UITheme.BgCard;
            dgvPagamentos.ColumnHeadersDefaultCellStyle.BackColor = UITheme.BgSidebar;
            dgvPagamentos.ColumnHeadersDefaultCellStyle.ForeColor = UITheme.TextTitle;
            dgvPagamentos.EnableHeadersVisualStyles = false;
            dgvPagamentos.DefaultCellStyle.BackColor = UITheme.BgCard;
            dgvPagamentos.DefaultCellStyle.ForeColor = UITheme.TextMuted;
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModalWindowStyle(this);
            this.BackColor = UITheme.BgModal;
            pnlContainer.BackColor = UITheme.BgModal;
            
            lblTitle.Font = UITheme.FontTitle;
            lblTitle.ForeColor = UITheme.TextTitle;

            lblTotalLabel.ForeColor = UITheme.TextMuted;
            lblTotalValue.ForeColor = UITheme.Primary;
            lblTotalValue.Font = new Font("Segoe UI", 28, FontStyle.Bold);
            
            lblRestanteLabel.ForeColor = UITheme.TextMuted;
            lblRestanteValue.ForeColor = Color.LightSalmon;

            lblTrocoLabel.ForeColor = UITheme.TextMuted;
            lblTrocoValue.ForeColor = UITheme.Success;

            UITheme.FormatSaaSButton(btnConfirmar, true);
            UITheme.FormatSaaSButton(btnAddPagamento, true);
            UITheme.FormatSaaSButton(btnCancelar, false);
            
            txtRecebido.BackColor = UITheme.BgApp;
            txtRecebido.ForeColor = UITheme.TextTitle;
            txtRecebido.BorderStyle = BorderStyle.FixedSingle;

            cboPagamento.BackColor = UITheme.BgApp;
            cboPagamento.ForeColor = UITheme.TextTitle;
            cboPagamento.FlatStyle = FlatStyle.Flat;
            
            cboCliente.BackColor = UITheme.BgApp;
            cboCliente.ForeColor = UITheme.TextTitle;
            cboCliente.FlatStyle = FlatStyle.Flat;
            
            lblClienteLabel.ForeColor = UITheme.TextMuted;
            lblDescontoLabel.ForeColor = UITheme.TextMuted;
            txtDesconto.BackColor = UITheme.BgApp;
            txtDesconto.ForeColor = UITheme.TextTitle;
            txtDesconto.BorderStyle = BorderStyle.FixedSingle;
            lblPagamentoLabel.ForeColor = UITheme.TextMuted;
            lblRecebidoLabel.ForeColor = UITheme.TextMuted;
        }

        private void SetupModernUI()
        {
            // Separador abaixo do ttulo
            var separator = new Panel {
                Location = new Point(0, 50),
                Size = new Size(pnlContainer.Width, 1),
                BackColor = UITheme.ElementBorder
            };
            pnlContainer.Controls.Add(separator);

            // Pnl Total Card
            var pnlTotalCard = new Panel {
                Location = new Point(20, 60),
                Size = new Size(310, 100),
                BackColor = UITheme.BgCard,
                BorderStyle = BorderStyle.None
            };
            pnlContainer.Controls.Add(pnlTotalCard);
            
            lblTotalLabel.Location = new Point(15, 15);
            lblTotalValue.Location = new Point(15, 35);
            pnlTotalCard.Controls.Add(lblTotalLabel);
            pnlTotalCard.Controls.Add(lblTotalValue);

            // Pnl Input Card
            var pnlInputCard = new Panel {
                Location = new Point(20, 180),
                Size = new Size(310, 310),
                BackColor = UITheme.BgCard,
                BorderStyle = BorderStyle.None
            };
            pnlContainer.Controls.Add(pnlInputCard);
            
            lblClienteLabel.Location = new Point(15, 15);
            cboCliente.Location = new Point(15, 35);
            cboCliente.Size = new Size(280, 29);
            lblSaldoDisponivel.Location = new Point(15, 70);
            
            lblPagamentoLabel.Location = new Point(15, 100);
            cboPagamento.Location = new Point(15, 120);
            cboPagamento.Size = new Size(280, 33);
            
            lblDescontoLabel.Location = new Point(15, 165);
            txtDesconto.Location = new Point(15, 185);
            txtDesconto.Size = new Size(280, 33);
            
            lblRecebidoLabel.Location = new Point(15, 230);
            txtRecebido.Location = new Point(15, 250);
            txtRecebido.Size = new Size(185, 50);
            
            btnAddPagamento.Location = new Point(205, 250);
            btnAddPagamento.Size = new Size(90, 50);

            pnlInputCard.Controls.Add(lblClienteLabel);
            pnlInputCard.Controls.Add(cboCliente);
            pnlInputCard.Controls.Add(lblSaldoDisponivel);
            pnlInputCard.Controls.Add(lblPagamentoLabel);
            pnlInputCard.Controls.Add(cboPagamento);
            pnlInputCard.Controls.Add(lblDescontoLabel);
            pnlInputCard.Controls.Add(txtDesconto);
            pnlInputCard.Controls.Add(lblRecebidoLabel);
            pnlInputCard.Controls.Add(txtRecebido);
            pnlInputCard.Controls.Add(btnAddPagamento);

            // Pnl Resumo Card
            var pnlResumoCard = new Panel {
                Location = new Point(20, 510),
                Size = new Size(310, 160),
                BackColor = UITheme.BgCard,
                BorderStyle = BorderStyle.None
            };
            pnlContainer.Controls.Add(pnlResumoCard);
            
            lblRestanteLabel.Location = new Point(15, 15);
            lblRestanteValue.Location = new Point(15, 35);
            lblRestanteValue.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            
            lblTrocoLabel.Location = new Point(15, 85);
            lblTrocoValue.Location = new Point(15, 105);
            lblTrocoValue.Font = new Font("Segoe UI", 22, FontStyle.Bold);

            pnlResumoCard.Controls.Add(lblRestanteLabel);
            pnlResumoCard.Controls.Add(lblRestanteValue);
            pnlResumoCard.Controls.Add(lblTrocoLabel);
            pnlResumoCard.Controls.Add(lblTrocoValue);
            
            // Tabela da Direita
            UITheme.FormatGrid(dgvPagamentos);
            dgvPagamentos.BackgroundColor = UITheme.BgCard;
            dgvPagamentos.GridColor = UITheme.BgModal;
        }

        private void TxtDesconto_Leave(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtDesconto.Text, out decimal descontoVal))
            {
                if (descontoVal < 0) descontoVal = 0;
                if (descontoVal > SubtotalVenda) descontoVal = SubtotalVenda;
                
                DescontoAplicado = descontoVal;
                txtDesconto.Text = DescontoAplicado.ToString("N2");
            }
            else
            {
                DescontoAplicado = 0;
                txtDesconto.Text = "0,00";
            }
            
            TotalVenda = SubtotalVenda - DescontoAplicado;
            lblTotalValue.Text = TotalVenda.ToString("C2");
            AtualizarSaldos();
        }

        private void BtnAddPagamento_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtRecebido.Text, out decimal valor) && valor > 0)
            {
                string metodo = cboPagamento.Text;
                if (metodo.StartsWith("Saldo do Cliente"))
                {
                    decimal saldoUsado = ListaPagamentos.Where(p => p.Metodo.StartsWith("Saldo")).Sum(p => p.Valor);
                    if (valor + saldoUsado > clienteSaldoAtual)
                    {
                        MessageBox.Show($"O cliente só possui R$ {clienteSaldoAtual - saldoUsado:N2} de saldo disponível.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    metodo = "Saldo do Cliente";
                }

                ListaPagamentos.Add(new PagamentoItem { Metodo = metodo, Valor = valor });
                AtualizarSaldos();
                
                decimal restante = TotalVenda - ListaPagamentos.Sum(p => p.Valor);
                if (restante > 0)
                {
                    txtRecebido.Text = restante.ToString("N2");
                    txtRecebido.Focus();
                    txtRecebido.SelectAll();
                }
                else
                {
                    txtRecebido.Text = "0,00";
                    btnConfirmar.Focus();
                }
            }
        }

        private void AtualizarSaldos()
        {
            decimal totalPago = ListaPagamentos.Sum(p => p.Valor);
            decimal restante = TotalVenda - totalPago;
            
            if (restante > 0)
            {
                lblRestanteValue.Text = restante.ToString("C2");
                lblTrocoValue.Text = "R$ 0,00";
                Troco = 0;
                btnConfirmar.Enabled = false;
                if (chkConverterSaldo != null) chkConverterSaldo.Visible = false;
                
                // Se ainda não houve pagamento, atualiza o valor sugerido para o cliente
                if (totalPago == 0)
                {
                    txtRecebido.Text = restante.ToString("N2");
                }
            }
            else
            {
                lblRestanteValue.Text = "PAGO";
                Troco = Math.Abs(restante);
                lblTrocoValue.Text = Troco.ToString("C2");
                btnConfirmar.Enabled = true;

                if (Troco > 0 && SelectedClienteId > 0)
                {
                    chkConverterSaldo.Visible = true;
                    chkConverterSaldo.Text = $"Converter Troco em Saldo (R$ {Troco:N2})";
                }
                else
                {
                    chkConverterSaldo.Visible = false;
                    chkConverterSaldo.Checked = false;
                }
            }
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            if (cboCliente.SelectedValue == null)
            {
                MessageBox.Show("Selecione um cliente válido.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SelectedClienteId = (int)cboCliente.SelectedValue;

            bool temPrazo = ListaPagamentos.Any(p => p.Metodo.IndexOf("Prazo", StringComparison.OrdinalIgnoreCase) >= 0 || p.Metodo.IndexOf("Fiado", StringComparison.OrdinalIgnoreCase) >= 0);
            if (temPrazo && SelectedClienteId == 0)
            {
                MessageBox.Show("Vendas a prazo (fiado) exigem a seleção de um cliente cadastrado (não pode ser CONSUMIDOR FINAL).", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            VendaFinalizada = true;
            ConverterTrocoEmSaldo = chkConverterSaldo.Checked;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter && btnConfirmar.Enabled && !txtRecebido.Focused)
            {
                BtnConfirmar_Click(null, null);
                return true;
            }
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            if (keyData == Keys.Delete && dgvPagamentos.Focused && dgvPagamentos.SelectedRows.Count > 0)
            {
                var item = (PagamentoItem)dgvPagamentos.SelectedRows[0].DataBoundItem;
                ListaPagamentos.Remove(item);
                AtualizarSaldos();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }

    public class PagamentoItem
    {
        public string Metodo { get; set; }
        public decimal Valor { get; set; }
    }
}
