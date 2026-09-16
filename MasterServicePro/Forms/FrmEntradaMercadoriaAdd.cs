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
    public class FrmEntradaMercadoriaAdd : Form
    {
        private readonly FornecedorRepository _fornecedorRepository = new FornecedorRepository();
        private readonly EntradaMercadoriaRepository _entradaRepository = new EntradaMercadoriaRepository();
        private readonly BindingList<EntradaMercadoriaItem> _itens = new BindingList<EntradaMercadoriaItem>();

        // Controls
        private Label lblFornecedor;
        private ComboBox cboFornecedores;
        private Label lblNota;
        private TextBox txtNota;
        private Label lblObs;
        private TextBox txtObs;
        private DataGridView dgvItens;
        private Button btnAdd;
        private Button btnRemove;
        private Label lblTotalLabel;
        private Label lblTotal;
        private Button btnGravar;
        private Button btnCancelar;
        private Panel pnlMain;

        public FrmEntradaMercadoriaAdd()
        {
            InitializeComponent();
            ApplyTheme();
            LoadFornecedores();
            BindEvents();
        }

        private void InitializeComponent()
        {
            this.Text = "Registrar Entrada de Mercadoria";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            pnlMain = new Panel { Size = new Size(760, 520) };
            this.ClientSize = new Size(1200, 750);
            this.Load += (s, e) => {
                pnlMain.Location = new Point((this.Width - pnlMain.Width) / 2, (this.Height - pnlMain.Height) / 2);
            };
            this.Controls.Add(pnlMain);

            // Row 1: Fornecedor
            lblFornecedor = new Label { Text = "Fornecedor:", Location = new Point(25, 20), Size = new Size(100, 20) };
            cboFornecedores = new ComboBox 
            { 
                Location = new Point(25, 40), 
                Size = new Size(340, 25), 
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = UITheme.BgSidebar,
                ForeColor = UITheme.TextTitle
            };

            // Row 1: Nota Fiscal
            lblNota = new Label { Text = "Nota Fiscal:", Location = new Point(390, 20), Size = new Size(100, 20) };
            txtNota = new TextBox 
            { 
                Location = new Point(390, 40), 
                Size = new Size(340, 25),
                BackColor = UITheme.BgSidebar,
                ForeColor = UITheme.TextTitle,
                BorderStyle = BorderStyle.FixedSingle,
                MaxLength = 50
            };

            // Row 2: Observação
            lblObs = new Label { Text = "Observação:", Location = new Point(25, 75), Size = new Size(100, 20) };
            txtObs = new TextBox 
            { 
                Location = new Point(25, 95), 
                Size = new Size(705, 25),
                BackColor = UITheme.BgSidebar,
                ForeColor = UITheme.TextTitle,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Row 3: Grid
            dgvItens = new DataGridView
            {
                Location = new Point(25, 135),
                Size = new Size(705, 240),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            // Row 4: Buttons / Total
            btnAdd = new Button { Text = "➕ Adicionar Item", Location = new Point(25, 385), Size = new Size(160, 35) };
            btnRemove = new Button { Text = "❌ Remover Item", Location = new Point(195, 385), Size = new Size(160, 35) };

            lblTotalLabel = new Label 
            { 
                Text = "VALOR TOTAL DA ENTRADA:", 
                Location = new Point(390, 392), 
                Size = new Size(190, 20),
                Font = UITheme.FontBodyBold,
                ForeColor = UITheme.TextMuted,
                TextAlign = ContentAlignment.MiddleRight
            };

            lblTotal = new Label 
            { 
                Text = "R$ 0,00", 
                Location = new Point(585, 390), 
                Size = new Size(145, 24),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = UITheme.Success,
                TextAlign = ContentAlignment.MiddleRight
            };

            // Footer buttons
            btnGravar = new Button { Text = "Gravar Entrada", Location = new Point(390, 440), Size = new Size(160, 40) };
            btnCancelar = new Button { Text = "Cancelar", Location = new Point(570, 440), Size = new Size(160, 40) };

            pnlMain.Controls.Add(lblFornecedor);
            pnlMain.Controls.Add(cboFornecedores);
            pnlMain.Controls.Add(lblNota);
            pnlMain.Controls.Add(txtNota);
            pnlMain.Controls.Add(lblObs);
            pnlMain.Controls.Add(txtObs);
            pnlMain.Controls.Add(dgvItens);
            pnlMain.Controls.Add(btnAdd);
            pnlMain.Controls.Add(btnRemove);
            pnlMain.Controls.Add(lblTotalLabel);
            pnlMain.Controls.Add(lblTotal);
            pnlMain.Controls.Add(btnGravar);
            pnlMain.Controls.Add(btnCancelar);
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModalWindowStyle(this);
            this.BackColor = UITheme.BgApp; // Fundo mais escuro no fundo geral

            pnlMain.BackColor = UITheme.BgCard;
            pnlMain.Paint += (s, e) => UITheme.DrawRoundedPanel(e.Graphics, pnlMain.ClientRectangle, UITheme.BgCard, 10, UITheme.ElementBorder);

            lblFornecedor.ForeColor = UITheme.TextTitle;
            lblFornecedor.Font = UITheme.FontBodyBold;
            lblNota.ForeColor = UITheme.TextTitle;
            lblNota.Font = UITheme.FontBodyBold;
            lblObs.ForeColor = UITheme.TextTitle;
            lblObs.Font = UITheme.FontBodyBold;

            cboFornecedores.FlatStyle = FlatStyle.Flat;
            txtNota.BorderStyle = BorderStyle.None;
            txtObs.BorderStyle = BorderStyle.None;
            
            // Simular borda nos inputs com pad
            txtNota.AutoSize = false;
            txtNota.Height = 30;
            txtObs.AutoSize = false;
            txtObs.Height = 30;

            UITheme.FormatGrid(dgvItens);
            UITheme.FormatSaaSButton(btnAdd, true);
            UITheme.FormatSaaSButton(btnRemove, false);
            btnRemove.BackColor = UITheme.Danger;

            UITheme.FormatSaaSButton(btnGravar, true);
            UITheme.FormatSaaSButton(btnCancelar, false);
        }

        private async void LoadFornecedores()
        {
            try
            {
                var list = await _fornecedorRepository.BuscarTodosAsync();
                cboFornecedores.DataSource = list;
                cboFornecedores.DisplayMember = "Nome";
                cboFornecedores.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar fornecedores: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindEvents()
        {
            dgvItens.DataSource = _itens;

            // Configure Columns
            if (dgvItens.Columns.Count > 0)
            {
                foreach (DataGridViewColumn col in dgvItens.Columns) col.Visible = false;

                if (dgvItens.Columns["NomeProduto"] != null)
                {
                    dgvItens.Columns["NomeProduto"].Visible = true;
                    dgvItens.Columns["NomeProduto"].HeaderText = "Descrição do Produto";
                    dgvItens.Columns["NomeProduto"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                if (dgvItens.Columns["Quantidade"] != null)
                {
                    dgvItens.Columns["Quantidade"].Visible = true;
                    dgvItens.Columns["Quantidade"].HeaderText = "Qtd";
                    dgvItens.Columns["Quantidade"].Width = 80;
                }

                if (dgvItens.Columns["PrecoCusto"] != null)
                {
                    dgvItens.Columns["PrecoCusto"].Visible = true;
                    dgvItens.Columns["PrecoCusto"].HeaderText = "Custo Unit.";
                    dgvItens.Columns["PrecoCusto"].DefaultCellStyle.Format = "C2";
                    dgvItens.Columns["PrecoCusto"].Width = 120;
                }

                if (dgvItens.Columns["SubTotal"] != null)
                {
                    dgvItens.Columns["SubTotal"].Visible = true;
                    dgvItens.Columns["SubTotal"].HeaderText = "SubTotal";
                    dgvItens.Columns["SubTotal"].DefaultCellStyle.Format = "C2";
                    dgvItens.Columns["SubTotal"].Width = 130;
                }
            }

            btnAdd.Click += BtnAdd_Click;
            btnRemove.Click += BtnRemove_Click;
            btnGravar.Click += BtnGravar_Click;
            btnCancelar.Click += (s, e) => this.Close();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var frmBusca = new FrmPDVBuscaProduto())
            {
                if (frmBusca.ShowDialog() == DialogResult.OK && frmBusca.ProdutoSelecionado != null)
                {
                    var prod = frmBusca.ProdutoSelecionado;

                    // Abrir helper input
                    using (var frmInput = new FrmInputQuantidadePreco(prod.Nome, prod.PrecoCusto))
                    {
                        if (frmInput.ShowDialog() == DialogResult.OK)
                        {
                            var existing = _itens.FirstOrDefault(i => i.IdProduto == prod.Id);
                            if (existing != null)
                            {
                                existing.Quantidade += frmInput.Quantidade;
                                existing.SubTotal = existing.Quantidade * existing.PrecoCusto;
                                _itens.ResetBindings();
                            }
                            else
                            {
                                _itens.Add(new EntradaMercadoriaItem
                                {
                                    IdProduto = prod.Id,
                                    NomeProduto = prod.Nome,
                                    Quantidade = frmInput.Quantidade,
                                    PrecoCusto = frmInput.PrecoCusto,
                                    SubTotal = frmInput.Quantidade * frmInput.PrecoCusto
                                });
                            }

                            RecalcularTotal();
                        }
                    }
                }
            }
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (dgvItens.SelectedRows.Count > 0)
            {
                var item = (EntradaMercadoriaItem)dgvItens.SelectedRows[0].DataBoundItem;
                _itens.Remove(item);
                RecalcularTotal();
            }
        }

        private void RecalcularTotal()
        {
            decimal total = _itens.Sum(i => i.SubTotal);
            lblTotal.Text = total.ToString("C2");
        }

        private void BtnGravar_Click(object sender, EventArgs e)
        {
            if (cboFornecedores.SelectedValue == null)
            {
                MessageBox.Show("Por favor, selecione um fornecedor válido.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_itens.Count == 0)
            {
                MessageBox.Show("Adicione pelo menos um item para registrar a entrada.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var entrada = new EntradaMercadoria
                {
                    IdFornecedor = (int)cboFornecedores.SelectedValue,
                    NumeroNota = txtNota.Text.Trim(),
                    Observacao = txtObs.Text.Trim(),
                    ValorTotal = _itens.Sum(i => i.SubTotal)
                };

                foreach (var item in _itens)
                {
                    entrada.Itens.Add(item);
                }

                _entradaRepository.RegistrarEntrada(entrada);

                MessageBox.Show("Entrada registrada com sucesso e custo médio recalculado!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao gravar entrada: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // Mini modal de input auxiliar
    public class FrmInputQuantidadePreco : Form
    {
        public int Quantidade { get; private set; }
        public decimal PrecoCusto { get; private set; }

        private Label lblProd;
        private Label lblQtd;
        private TextBox txtQtd;
        private Label lblPreco;
        private TextBox txtPreco;
        private Button btnOk;
        private Button btnCancelar;

        public FrmInputQuantidadePreco(string nomeProduto, decimal custoAtual)
        {
            InitializeComponent(nomeProduto, custoAtual);
            ApplyTheme();
        }

        private void InitializeComponent(string nomeProduto, decimal custoAtual)
        {
            this.Text = "Informações de Entrada do Item";
            this.Size = new Size(360, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            lblProd = new Label
            {
                Text = nomeProduto,
                Location = new Point(20, 15),
                Size = new Size(320, 30),
                Font = UITheme.FontBodyBold
            };

            lblQtd = new Label { Text = "Quantidade:", Location = new Point(20, 50), Size = new Size(130, 20) };
            txtQtd = new TextBox { Location = new Point(20, 70), Size = new Size(130, 25), Text = "1" };

            lblPreco = new Label { Text = "Preço Custo Unit.:", Location = new Point(170, 50), Size = new Size(130, 20) };
            txtPreco = new TextBox { Location = new Point(170, 70), Size = new Size(150, 25), Text = custoAtual.ToString("F2") };

            btnOk = new Button { Text = "Adicionar", Location = new Point(20, 120), Size = new Size(130, 35) };
            btnCancelar = new Button { Text = "Cancelar", Location = new Point(170, 120), Size = new Size(150, 35) };

            this.Controls.Add(lblProd);
            this.Controls.Add(lblQtd);
            this.Controls.Add(txtQtd);
            this.Controls.Add(lblPreco);
            this.Controls.Add(txtPreco);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancelar);

            btnOk.Click += (s, e) => {
                if (int.TryParse(txtQtd.Text, out int q) && q > 0 && decimal.TryParse(txtPreco.Text, out decimal p) && p >= 0)
                {
                    Quantidade = q;
                    PrecoCusto = p;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Por favor, insira valores válidos para quantidade e preço de custo.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            btnCancelar.Click += (s, e) => this.Close();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModalWindowStyle(this);
            this.BackColor = UITheme.BgModal;

            lblProd.ForeColor = UITheme.TextTitle;
            lblQtd.ForeColor = UITheme.TextMuted;
            lblQtd.Font = UITheme.FontBody;
            lblPreco.ForeColor = UITheme.TextMuted;
            lblPreco.Font = UITheme.FontBody;

            txtQtd.BackColor = UITheme.BgSidebar;
            txtQtd.ForeColor = UITheme.TextTitle;
            txtQtd.BorderStyle = BorderStyle.FixedSingle;

            txtPreco.BackColor = UITheme.BgSidebar;
            txtPreco.ForeColor = UITheme.TextTitle;
            txtPreco.BorderStyle = BorderStyle.FixedSingle;

            UITheme.FormatSaaSButton(btnOk, true);
            UITheme.FormatSaaSButton(btnCancelar, false);
        }
    }
}
