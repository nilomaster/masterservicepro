using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Utils;
using MasterServicePro.Models;

namespace MasterServicePro.Forms
{
    public class FrmTrocaVenda : Form
    {
        private int vendaId;
        private VendaRepository vendaRepo = new VendaRepository();
        private FinanceiroRepository finRepo = new FinanceiroRepository();
        private DataTable dtItens;
        private Produto produtoNovo;
        private bool hasCliente;

        private ComboBox cboItensDevolvidos;
        private TextBox txtProdutoNovo;
        private Button btnBuscarProduto;
        private Label lblDiferenca;
        private ComboBox cboFormaPagamento;
        private CheckBox chkAdicionarSaldo;
        private Button btnConfirmar;
        private Button btnCancelar;
        private Panel pnlDiferenca;

        public FrmTrocaVenda(int vendaId)
        {
            this.vendaId = vendaId;
            InitializeComponent();
            this.Load += FrmTrocaVenda_Load;
        }

        private void InitializeComponent()
        {
            this.Size = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Text = $"Troca de Produto - Venda #{vendaId}";
            this.BackColor = UITheme.BgApp;
            this.ForeColor = UITheme.TextTitle;

            Label lblItem = new Label { Text = "1. Selecione o item sendo devolvido:", AutoSize = true, Location = new Point(20, 20), ForeColor = UITheme.TextTitle };
            cboItensDevolvidos = new ComboBox { Location = new Point(20, 40), Width = 440, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = UITheme.BgInput, ForeColor = UITheme.TextTitle, FlatStyle = FlatStyle.Flat };
            cboItensDevolvidos.SelectedIndexChanged += CalcularDiferenca;

            Label lblNovo = new Label { Text = "2. Selecione o NOVO produto:", AutoSize = true, Location = new Point(20, 80), ForeColor = UITheme.TextTitle };
            txtProdutoNovo = new TextBox { Location = new Point(20, 100), Width = 340, ReadOnly = true, BackColor = UITheme.BgInput, ForeColor = UITheme.TextTitle, BorderStyle = BorderStyle.FixedSingle };
            btnBuscarProduto = new Button { Text = "Buscar", Location = new Point(370, 99), Width = 90, Height = 25, BackColor = UITheme.Primary, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnBuscarProduto.FlatAppearance.BorderSize = 0;
            btnBuscarProduto.Click += BtnBuscarProduto_Click;

            pnlDiferenca = new Panel { Location = new Point(20, 140), Width = 440, Height = 180, BorderStyle = BorderStyle.FixedSingle, BackColor = UITheme.BgCard };
            
            lblDiferenca = new Label { Text = "Diferença: R$ 0,00", AutoSize = false, Width = 400, Height = 30, Location = new Point(10, 10), Font = new Font("Segoe UI", 12F, FontStyle.Bold), ForeColor = UITheme.TextTitle };
            
            Label lblPagto = new Label { Text = "Forma de Pgto (Complemento):", AutoSize = true, Location = new Point(10, 50), ForeColor = UITheme.TextTitle };
            cboFormaPagamento = new ComboBox { Location = new Point(10, 70), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = UITheme.BgInput, ForeColor = UITheme.TextTitle, FlatStyle = FlatStyle.Flat };
            cboFormaPagamento.Items.AddRange(new object[] { "Dinheiro", "Pix", "Cartão de Crédito", "Cartão de Débito" });
            cboFormaPagamento.SelectedIndex = 0;

            chkAdicionarSaldo = new CheckBox { Text = "Adicionar diferença como Saldo do Cliente", AutoSize = true, Location = new Point(10, 110), ForeColor = UITheme.TextTitle, Visible = false };

            pnlDiferenca.Controls.Add(lblDiferenca);
            pnlDiferenca.Controls.Add(lblPagto);
            pnlDiferenca.Controls.Add(cboFormaPagamento);
            pnlDiferenca.Controls.Add(chkAdicionarSaldo);

            btnCancelar = new Button { Text = "Cancelar", Location = new Point(260, 360), Width = 100, Height = 35, BackColor = UITheme.ElementBorder, ForeColor = UITheme.TextTitle, FlatStyle = FlatStyle.Flat };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            btnConfirmar = new Button { Text = "Confirmar Troca", Location = new Point(370, 360), Width = 110, Height = 35, BackColor = UITheme.Success, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Enabled = false };
            btnConfirmar.FlatAppearance.BorderSize = 0;
            btnConfirmar.Click += BtnConfirmar_Click;

            this.Controls.Add(lblItem);
            this.Controls.Add(cboItensDevolvidos);
            this.Controls.Add(lblNovo);
            this.Controls.Add(txtProdutoNovo);
            this.Controls.Add(btnBuscarProduto);
            this.Controls.Add(pnlDiferenca);
            this.Controls.Add(btnCancelar);
            this.Controls.Add(btnConfirmar);
        }

        private void FrmTrocaVenda_Load(object sender, EventArgs e)
        {
            try
            {
                var dtVenda = vendaRepo.BuscarPorId(vendaId);
                if (dtVenda.Rows.Count > 0)
                {
                    object clienteId = dtVenda.Rows[0]["ClienteId"];
                    hasCliente = clienteId != DBNull.Value && Convert.ToInt32(clienteId) > 0;
                }

                dtItens = vendaRepo.BuscarItensVenda(vendaId);
                cboItensDevolvidos.DisplayMember = "Display";
                cboItensDevolvidos.ValueMember = "Id";

                var dtCombo = new DataTable();
                dtCombo.Columns.Add("Id", typeof(int));
                dtCombo.Columns.Add("Display", typeof(string));
                dtCombo.Columns.Add("PrecoUnitario", typeof(decimal));

                foreach (DataRow row in dtItens.Rows)
                {
                    decimal preco = Convert.ToDecimal(row["PrecoUnitario"]);
                    string display = $"{row["Quantidade"]}x {row["Produto"]} (R$ {preco:N2})";
                    dtCombo.Rows.Add(row["Id"], display, preco);
                }

                cboItensDevolvidos.DataSource = dtCombo;

                CalcularDiferenca(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar itens: " + ex.Message);
            }
        }

        private void BtnBuscarProduto_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmPDVBuscaProduto())
            {
                if (frm.ShowDialog() == DialogResult.OK && frm.ProdutoSelecionado != null)
                {
                    produtoNovo = frm.ProdutoSelecionado;
                    txtProdutoNovo.Text = $"{produtoNovo.Nome} - R$ {produtoNovo.PrecoVenda:N2}";
                    CalcularDiferenca(null, null);
                }
            }
        }

        private void CalcularDiferenca(object sender, EventArgs e)
        {
            if (cboItensDevolvidos.SelectedValue == null || produtoNovo == null)
            {
                lblDiferenca.Text = "Selecione o item e o novo produto.";
                btnConfirmar.Enabled = false;
                return;
            }

            try
            {
                int idItemDb = Convert.ToInt32(cboItensDevolvidos.SelectedValue);
                DataRow[] rows = ((DataTable)cboItensDevolvidos.DataSource).Select($"Id = {idItemDb}");
                if (rows.Length == 0) return;

                decimal precoAntigo = Convert.ToDecimal(rows[0]["PrecoUnitario"]);
                decimal precoNovo = produtoNovo.PrecoVenda;
                decimal diferenca = precoNovo - precoAntigo;

                if (diferenca > 0)
                {
                    lblDiferenca.Text = $"Diferença: R$ {diferenca:N2} (Cliente PAGA)";
                    lblDiferenca.ForeColor = UITheme.Warning; // Laranja
                    cboFormaPagamento.Enabled = true;
                    chkAdicionarSaldo.Visible = false;
                }
                else if (diferenca < 0)
                {
                    lblDiferenca.Text = $"Diferença: R$ {Math.Abs(diferenca):N2} (Loja DEVOLVE)";
                    lblDiferenca.ForeColor = UITheme.Success; // Verde
                    cboFormaPagamento.Enabled = false;
                    chkAdicionarSaldo.Visible = hasCliente;
                    if (hasCliente) chkAdicionarSaldo.Checked = true;
                }
                else
                {
                    lblDiferenca.Text = $"Diferença: R$ 0,00 (Troca Elas por Elas)";
                    lblDiferenca.ForeColor = UITheme.TextTitle;
                    cboFormaPagamento.Enabled = false;
                    chkAdicionarSaldo.Visible = false;
                }

                btnConfirmar.Enabled = true;
            }
            catch { }
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            if (cboItensDevolvidos.SelectedValue == null || produtoNovo == null) return;

            var caixa = finRepo.ObterCaixaAberto();
            if (caixa == null)
            {
                MessageBox.Show("Você precisa ter um Caixa Aberto para realizar a troca (pode haver necessidade de movimentar dinheiro).", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idItemDb = Convert.ToInt32(cboItensDevolvidos.SelectedValue);
            DataRow[] rows = ((DataTable)cboItensDevolvidos.DataSource).Select($"Id = {idItemDb}");
            decimal precoAntigo = Convert.ToDecimal(rows[0]["PrecoUnitario"]);
            decimal precoNovo = produtoNovo.PrecoVenda;
            decimal diferenca = precoNovo - precoAntigo;

            string formaPagamento = cboFormaPagamento.Enabled ? cboFormaPagamento.Text : "Dinheiro";
            bool adicionarSaldo = chkAdicionarSaldo.Visible && chkAdicionarSaldo.Checked;

            try
            {
                vendaRepo.TrocarItemVenda(vendaId, idItemDb, produtoNovo.Id, precoNovo, formaPagamento, diferenca, adicionarSaldo, caixa.Id, AuthSession.Id);
                FrmNotification.ShowSuccess("Troca realizada com sucesso!");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao efetuar a troca: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
