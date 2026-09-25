using System;
using System.Drawing;
using System.Windows.Forms;
using MasterServicePro.Models;
using MasterServicePro.DAL;
using MasterServicePro.Utils;
using System.Linq;
using System.ComponentModel;

namespace MasterServicePro.Forms
{
    public partial class FrmOSAddEdit : Form
    {
        private int osId = 0;
        private OrdemServicoRepository repository = new OrdemServicoRepository();
        private ClienteRepository clienteRepository = new ClienteRepository();
        private BindingList<OrdemServicoItem> itensOs = new BindingList<OrdemServicoItem>();
        private string osChecklist = "";
        private Button btnChecklist;
        private Button btnImprimirOrcamento;

        public FrmOSAddEdit(int id = 0)
        {
            InitializeComponent();
            osId = id;
            
            // Inicializar botões do checklist programaticamente
            btnChecklist = new Button
            {
                Text = "📋 Checklist de Entrada",
                Location = new Point(580, 505),
                Size = new Size(310, 35),
                Cursor = Cursors.Hand
            };
            btnImprimirOrcamento = new Button
            {
                Text = "🖨️ Orçamento Rápido",
                Location = new Point(580, 545),
                Size = new Size(310, 35),
                Cursor = Cursors.Hand
            };

            pnlMain.Controls.Add(btnChecklist);
            pnlMain.Controls.Add(btnImprimirOrcamento);

            btnChecklist.Click += BtnChecklist_Click;
            btnImprimirOrcamento.Click += BtnImprimirOrcamento_Click;

            ApplyTheme();
            this.Load += FrmOSAddEdit_Load;

            btnSalvar.Click += BtnSalvar_Click;
            btnCancelar.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            btnAddItem.Click += BtnAddItem_Click;
            btnRemoverItem.Click += BtnRemoverItem_Click;

            txtValorPecas.Leave += TxtValorPecas_Leave;
            txtValorServico.Leave += TxtValorServico_Leave;
            txtValor.ReadOnly = true;
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModalWindowStyle(this);
            this.BackColor = UITheme.BgModal;
            pnlMain.BackColor = UITheme.BgCard;

            lblTitle.Font = UITheme.FontTitle;
            lblTitle.ForeColor = UITheme.TextTitle;

            foreach (Control c in pnlMain.Controls)
            {
                if (c is Label && c != lblTitle)
                {
                    c.ForeColor = UITheme.TextMuted;
                    c.Font = UITheme.FontBody;
                }
                else if (c is TextBox || c is ComboBox)
                {
                    c.BackColor = UITheme.BgSidebar;
                    c.ForeColor = UITheme.TextTitle;
                    c.Font = new Font("Segoe UI", 12F); // Fonte um pouco maior para premium feel
                    
                    if (c is TextBox txt) 
                    {
                        txt.BorderStyle = BorderStyle.FixedSingle;
                    }
                    if (c is ComboBox cbo)
                    {
                        cbo.FlatStyle = FlatStyle.Flat;
                    }
                }
            }

            txtValor.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            txtValor.ForeColor = UITheme.Primary;
            txtValor.TextAlign = HorizontalAlignment.Center;

            UITheme.FormatSaaSButton(btnSalvar, true);
            UITheme.FormatSaaSButton(btnCancelar, false);
            
            lblItens.Font = UITheme.FontTitle;
            lblItens.ForeColor = UITheme.TextTitle;

            UITheme.FormatSaaSButton(btnAddItem, true);
            UITheme.FormatSaaSButton(btnRemoverItem, false);
            btnRemoverItem.BackColor = UITheme.Danger;
            btnRemoverItem.ForeColor = Color.White;

            UITheme.FormatSaaSButton(btnChecklist, true);
            btnChecklist.BackColor = UITheme.Warning;
            btnChecklist.ForeColor = Color.White;

            UITheme.FormatSaaSButton(btnImprimirOrcamento, true);
            btnImprimirOrcamento.BackColor = UITheme.Primary;
            btnImprimirOrcamento.ForeColor = Color.White;

            UITheme.FormatGrid(dgvItens);
        }

        private void TxtValorPecas_Leave(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtValorPecas.Text.Replace("R$", "").Trim(), out decimal val))
            {
                txtValorPecas.Text = val.ToString("N2");
            }
            CalcularTotal();
        }

        private void TxtValorServico_Leave(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtValorServico.Text.Replace("R$", "").Trim(), out decimal val))
            {
                txtValorServico.Text = val.ToString("N2");
            }
            CalcularTotal();
        }

        private void CalcularTotal()
        {
            decimal.TryParse(txtValorPecas.Text.Replace("R$", "").Trim(), out decimal custo);
            decimal.TryParse(txtValorServico.Text.Replace("R$", "").Trim(), out decimal cobrado);
            txtValor.Text = (cobrado - custo).ToString("N2");
        }

        private async void FrmOSAddEdit_Load(object sender, EventArgs e)
        {
            dgvItens.DataSource = itensOs;
            FormatGridItens();

            // Configure IMEI max length to 17 digits and numbers only
            txtImei.MaxLength = 17;
            txtImei.KeyPress += (s, ev) =>
            {
                if (!char.IsControl(ev.KeyChar) && !char.IsDigit(ev.KeyChar))
                    ev.Handled = true;
            };

            await LoadClientesAsync();
            await LoadTecnicosAsync();
            await LoadMarcasEModelosAsync();
            
            if (osId > 0)
            {
                lblTitle.Text = "Editar Ordem de Serviço";
                LoadOS();
            }
        }

        private void FormatGridItens()
        {
            if (dgvItens.Columns.Count > 0)
            {
                foreach(DataGridViewColumn c in dgvItens.Columns) c.Visible = false;
                
                dgvItens.Columns["NomeProduto"].Visible = true;
                dgvItens.Columns["NomeProduto"].HeaderText = "Descrição";
                dgvItens.Columns["NomeProduto"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                dgvItens.Columns["Quantidade"].Visible = true;
                dgvItens.Columns["Quantidade"].HeaderText = "Qtd";
                dgvItens.Columns["Quantidade"].Width = 50;

                dgvItens.Columns["SubTotal"].Visible = true;
                dgvItens.Columns["SubTotal"].HeaderText = "SubTotal";
                dgvItens.Columns["SubTotal"].DefaultCellStyle.Format = "C2";
                dgvItens.Columns["SubTotal"].Width = 80;
            }
        }

        private async System.Threading.Tasks.Task LoadClientesAsync()
        {
            var repo = new ClienteRepository();
            var clientes = await repo.BuscarTodosAsync();
            cboCliente.DataSource = clientes;
            cboCliente.DisplayMember = "Nome";
            cboCliente.ValueMember = "Id";
            cboCliente.SelectedIndex = -1;
        }

        private async System.Threading.Tasks.Task LoadTecnicosAsync()
        {
            var repo = new TecnicoRepository();
            var lista = await repo.BuscarTodosAsync();
            
            // Adiciona opção vazia
            lista.Insert(0, new Tecnico { Id = 0, Nome = "-- Selecione um Técnico --" });
            
            cboTecnico.DataSource = lista;
            cboTecnico.DisplayMember = "Nome";
            cboTecnico.ValueMember = "Id";
            cboTecnico.SelectedIndex = 0;
        }

        private async System.Threading.Tasks.Task LoadMarcasEModelosAsync()
        {
            try
            {
                var marcasDb = await repository.ObterMarcasDistintasAsync();
                var autoMarcas = new AutoCompleteStringCollection();
                string[] marcasPadrao = { "Apple", "Samsung", "Motorola", "Xiaomi", "Realme", "LG", "Asus", "Infinix", "Huawei", "Positivo", "Multilaser", "Philco", "TCL", "Nokia", "Sony", "Google" };
                autoMarcas.AddRange(marcasPadrao);
                if (marcasDb != null)
                {
                    foreach (var m in marcasDb)
                    {
                        if (!string.IsNullOrEmpty(m) && !autoMarcas.Contains(m))
                            autoMarcas.Add(m);
                    }
                }
                txtMarca.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                txtMarca.AutoCompleteSource = AutoCompleteSource.CustomSource;
                txtMarca.AutoCompleteCustomSource = autoMarcas;

                var modelosDb = await repository.ObterModelosDistintosAsync();
                var autoModelos = new AutoCompleteStringCollection();
                if (modelosDb != null)
                {
                    foreach (var mod in modelosDb)
                    {
                        if (!string.IsNullOrEmpty(mod) && !autoModelos.Contains(mod))
                            autoModelos.Add(mod);
                    }
                }
                txtModelo.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                txtModelo.AutoCompleteSource = AutoCompleteSource.CustomSource;
                txtModelo.AutoCompleteCustomSource = autoModelos;
            }
            catch { }
        }

        private void LoadOS()
        {
            var os = repository.BuscarPorId(osId);
            if (os != null)
            {
                if (os.IdCliente > 0)
                    cboCliente.SelectedValue = os.IdCliente;
                else
                    cboCliente.Text = os.ClienteFinal;

                if (os.IdTecnico.HasValue && os.IdTecnico.Value > 0)
                    cboTecnico.SelectedValue = os.IdTecnico.Value;
                else
                    cboTecnico.SelectedIndex = 0;

                txtMarca.Text = os.Marca;
                txtModelo.Text = os.Modelo;
                txtCor.Text = os.Cor;
                txtImei.Text = os.IMEI;
                txtDefeito.Text = os.Defeito;
                txtLaudo.Text = os.LaudoTecnico;
                txtValorPecas.Text = os.ValorPecas.ToString("N2");
                txtValorServico.Text = os.ValorServico.ToString("N2");
                txtValor.Text = os.ValorTotal.ToString("N2");
                cboStatus.Text = os.Status;
                osChecklist = os.Checklist;
                
                foreach (var item in os.Itens)
                {
                    itensOs.Add(item);
                }
                
                osFaturado = os.Faturado;
            }
        }
        
        private bool osFaturado = false;

        private string PromptDialog(string text, string caption, string defaultValue = "")
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = UITheme.BgCard
            };
            Label textLabel = new Label() { Left = 20, Top = 20, Width = 340, Text = text, ForeColor = UITheme.TextTitle, Font = UITheme.FontBody };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 340, Text = defaultValue, Font = UITheme.FontBody };
            Button confirmation = new Button() { Text = "OK", Left = 260, Width = 100, Top = 100, DialogResult = DialogResult.OK };
            UITheme.FormatSaaSButton(confirmation, true);
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        private void BtnAddItem_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmPDVBuscaProduto())
            {
                if (frm.ShowDialog() == DialogResult.OK && frm.ProdutoSelecionado != null)
                {
                    var prod = frm.ProdutoSelecionado;
                    string input = PromptDialog("Digite a quantidade:", "Quantidade", "1");
                    
                    if (decimal.TryParse(input, out decimal qtd) && qtd > 0)
                    {
                        var item = new OrdemServicoItem
                        {
                            ProdutoId = prod.Id,
                            NomeProduto = prod.Nome,
                            Quantidade = qtd,
                            ValorUnitario = prod.PrecoVenda,
                            SubTotal = prod.PrecoVenda * qtd
                        };
                        itensOs.Add(item);
                        
                        decimal.TryParse(txtValorPecas.Text.Replace("R$", "").Trim(), out decimal valorAtual);
                        txtValorPecas.Text = (valorAtual + item.SubTotal).ToString("N2");
                        CalcularTotal();
                    }
                }
            }
        }

        private void BtnRemoverItem_Click(object sender, EventArgs e)
        {
            if (dgvItens.SelectedRows.Count > 0)
            {
                var item = (OrdemServicoItem)dgvItens.SelectedRows[0].DataBoundItem;
                itensOs.Remove(item);
                
                decimal.TryParse(txtValorPecas.Text.Replace("R$", "").Trim(), out decimal valorAtual);
                decimal novoValor = valorAtual - item.SubTotal;
                if (novoValor < 0) novoValor = 0;
                txtValorPecas.Text = novoValor.ToString("N2");
                CalcularTotal();
            }
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            int idCliente = 0;
            string clienteFinal = "";

            if (cboCliente.SelectedValue != null)
            {
                idCliente = (int)cboCliente.SelectedValue;
            }
            else if (!string.IsNullOrWhiteSpace(cboCliente.Text))
            {
                idCliente = 0;
                clienteFinal = cboCliente.Text.Trim();
            }
            else
            {
                MessageBox.Show("Selecione um cliente ou digite o nome do cliente final.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal valorPecas = 0;
            decimal.TryParse(txtValorPecas.Text.Replace("R$", "").Trim(), out valorPecas);

            decimal valorServico = 0;
            decimal.TryParse(txtValorServico.Text.Replace("R$", "").Trim(), out valorServico);

            decimal valorTotal = 0;
            decimal.TryParse(txtValor.Text.Replace("R$", "").Trim(), out valorTotal);

            int? idTecnico = null;
            if (cboTecnico.SelectedValue != null && (int)cboTecnico.SelectedValue > 0)
            {
                idTecnico = (int)cboTecnico.SelectedValue;
            }

            var os = new OrdemServico
            {
                Id = osId,
                IdCliente = idCliente,
                IdTecnico = idTecnico,
                ClienteFinal = clienteFinal,
                Marca = txtMarca.Text.Trim(),
                Modelo = txtModelo.Text.Trim(),
                Cor = txtCor.Text.Trim(),
                IMEI = txtImei.Text.Trim(),
                Defeito = txtDefeito.Text.Trim(),
                LaudoTecnico = txtLaudo.Text.Trim(),
                ValorPecas = valorPecas,
                ValorServico = valorServico,
                ValorTotal = valorTotal,
                Status = cboStatus.Text,
                Checklist = osChecklist,
                Faturado = osFaturado,
                DataAtualizacao = DateTime.Now,
                Itens = itensOs.ToList()
            };

            // Processar Faturamento se foi Finalizado e não estava faturado
            if (os.Status.Equals("Finalizado", StringComparison.OrdinalIgnoreCase) && !os.Faturado)
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
                                Descricao = $"Recebimento OS #{(osId > 0 ? os.NumeroOS : "NOVA")}",
                                IdCaixa = caixa != null ? caixa.Id : 0
                            });
                        }
                        else
                        {
                            var crRepo = new MasterServicePro.DAL.ContasReceberRepository();
                            crRepo.Inserir(new MasterServicePro.Models.ContaReceber
                            {
                                IdCliente = os.IdCliente,
                                Descricao = $"Serviço OS #{(osId > 0 ? os.NumeroOS : "NOVA")}",
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

            if (osId == 0)
            {
                os.DataAbertura = DateTime.Now;
                repository.Inserir(os);
                
                // Enviar notificação de Abertura
                System.Threading.Tasks.Task.Run(async () => {
                    var ws = new MasterServicePro.Services.WhatsAppService();
                    await ws.EnviarNotificacaoOSAsync(os, "Abertura");
                });
            }
            else
            {
                repository.Atualizar(os);
                
                // Enviar notificação de alteração / conclusão
                System.Threading.Tasks.Task.Run(async () => {
                    var ws = new MasterServicePro.Services.WhatsAppService();
                    string tipo = os.Status.Equals("Finalizado", StringComparison.OrdinalIgnoreCase) ? "Finalizado" : "Atualizacao";
                    await ws.EnviarNotificacaoOSAsync(os, tipo);
                });
            }

            this.DialogResult = DialogResult.OK;
        }

        private void BtnChecklist_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmOSChecklist(osChecklist))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    osChecklist = frm.ChecklistResult;
                }
            }
        }

        private void BtnImprimirOrcamento_Click(object sender, EventArgs e)
        {
            int idCliente = 0;
            string clienteFinal = "";

            if (cboCliente.SelectedValue != null)
            {
                idCliente = (int)cboCliente.SelectedValue;
            }
            else if (!string.IsNullOrWhiteSpace(cboCliente.Text))
            {
                idCliente = 0;
                clienteFinal = cboCliente.Text.Trim();
            }
            else
            {
                MessageBox.Show("Selecione um cliente ou digite o nome do cliente final para gerar o orçamento.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal valorPecas = 0;
            decimal.TryParse(txtValorPecas.Text.Replace("R$", "").Trim(), out valorPecas);

            decimal valorServico = 0;
            decimal.TryParse(txtValorServico.Text.Replace("R$", "").Trim(), out valorServico);

            decimal valorTotal = 0;
            decimal.TryParse(txtValor.Text.Replace("R$", "").Trim(), out valorTotal);

            int? idTecnico = null;
            if (cboTecnico.SelectedValue != null && (int)cboTecnico.SelectedValue > 0)
            {
                idTecnico = (int)cboTecnico.SelectedValue;
            }

            var os = new OrdemServico
            {
                Id = osId,
                IdCliente = idCliente,
                IdTecnico = idTecnico,
                ClienteFinal = clienteFinal,
                Marca = txtMarca.Text.Trim(),
                Modelo = txtModelo.Text.Trim(),
                Cor = txtCor.Text.Trim(),
                IMEI = txtImei.Text.Trim(),
                Defeito = txtDefeito.Text.Trim(),
                LaudoTecnico = txtLaudo.Text.Trim(),
                ValorPecas = valorPecas,
                ValorServico = valorServico,
                ValorTotal = valorTotal,
                Status = cboStatus.Text,
                Checklist = osChecklist,
                DataAtualizacao = DateTime.Now,
                Itens = itensOs.ToList()
            };

            if (osId == 0)
            {
                os.DataAbertura = DateTime.Now;
                repository.Inserir(os);
                osId = os.Id;
                lblTitle.Text = "Editar Ordem de Serviço";
            }
            else
            {
                repository.Atualizar(os);
            }

            var printer = new OSPrinter();
            printer.ImprimirOrcamento(osId);
        }
    }
}
