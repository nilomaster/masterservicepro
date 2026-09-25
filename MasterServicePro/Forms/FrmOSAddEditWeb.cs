using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.IO;
using Newtonsoft.Json;
using MasterServicePro.DAL;
using MasterServicePro.Models;
using System.Collections.Generic;
using System.Linq;

namespace MasterServicePro.Forms
{
    public class FrmOSAddEditWeb : Form
    {
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        
        private int osId = 0;
        private OrdemServicoRepository repository = new OrdemServicoRepository();
        private ClienteRepository clienteRepository = new ClienteRepository();
        private TecnicoRepository tecnicoRepository = new TecnicoRepository();

        public FrmOSAddEditWeb(int id = 0)
        {
            osId = id;
            InitializeComponent();
            this.Load += FrmOSAddEditWeb_Load;
        }

        private void InitializeComponent()
        {
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            // WebView
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.FromArgb(26, 29, 36);
            this.webView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView.Location = new System.Drawing.Point(0, 0);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(1200, 750);
            this.webView.TabIndex = 0;
            this.webView.ZoomFactor = 1D;
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.ClientSize = new System.Drawing.Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(1);
            try { this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath); } catch { }
            
            this.Controls.Add(this.webView);
            this.Name = "FrmOSAddEditWeb";
            this.Text = "Manutenção de O.S.";
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();
            this.ResumeLayout(false);
        }



        private void FrmOSAddEditWeb_Load(object sender, EventArgs e)
        {
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"OSAddEdit\index.html");
                path = Path.GetFullPath(path);
                
                if (!File.Exists(path))
                {
                    MessageBox.Show("Erro: Arquivo HTML não encontrado em " + path, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                webView.CoreWebView2.Navigate(path);
                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            }
            catch (System.Runtime.InteropServices.COMException ex) when ((uint)ex.ErrorCode == 0x80004004 || (uint)ex.ErrorCode == 0x80040154)
            {
                // Swallowed E_ABORT
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inicializar o WebView2: " + ex.Message);
            }
        }

        private async void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            // When HTML is fully loaded, send data from DB
            var clientes = await clienteRepository.BuscarTodosAsync();
            var tecnicos = await tecnicoRepository.BuscarTodosAsync();
            var marcasDb = await repository.ObterMarcasDistintasAsync();
            var modelosDb = await repository.ObterModelosDistintosAsync();
            
            OrdemServico os = null;
            if (osId > 0)
            {
                os = repository.BuscarPorId(osId);
            }

            var payload = new
            {
                Clientes = clientes,
                Tecnicos = tecnicos,
                MarcasDb = marcasDb,
                ModelosDb = modelosDb,
                OS = os
            };

            string json = JsonConvert.SerializeObject(payload);
            string safeJsString = JsonConvert.SerializeObject(json); // Escape for string literal
            
            await webView.CoreWebView2.ExecuteScriptAsync($"loadFormData({safeJsString})");
        }

        private async void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string jsonString = e.WebMessageAsJson;
                dynamic message = JsonConvert.DeserializeObject(jsonString);
                string actionName = message.action.ToString();

                switch (actionName)
                {
                    case "ADD_CLIENT":
                        this.BeginInvoke(new Action(() => 
                        {
                            using (var frm = new FrmClienteAddEdit())
                            {
                                if (frm.ShowDialog() == DialogResult.OK)
                                {
                                    // Refresh clients list in web view
                                    CoreWebView2_NavigationCompleted(null, null);
                                }
                            }
                        }));
                        break;
                        
                    case "ADD_PART":
                        this.BeginInvoke(new Action(() => 
                        {
                            using (var frmBusca = new FrmPDVBuscaProduto())
                            {
                                if (frmBusca.ShowDialog() == DialogResult.OK && frmBusca.ProdutoSelecionado != null)
                                {
                                    var part = new OrdemServicoItem
                                    {
                                        ProdutoId = frmBusca.ProdutoSelecionado.Id,
                                        NomeProduto = frmBusca.ProdutoSelecionado.Nome,
                                        Quantidade = 1,
                                        ValorUnitario = frmBusca.ProdutoSelecionado.PrecoVenda,
                                        SubTotal = frmBusca.ProdutoSelecionado.PrecoVenda
                                    };
                                    
                                    string partJson = JsonConvert.SerializeObject(part);
                                    string safePartJsString = JsonConvert.SerializeObject(partJson);
                                    _ = webView.CoreWebView2.ExecuteScriptAsync($"addPart({safePartJsString})");
                                }
                            }
                        }));
                        break;

                    case "OPEN_CHECKLIST":
                        this.BeginInvoke(new Action(() => 
                        {
                            var currentOS = repository.BuscarPorId(osId);
                            string cl = currentOS != null ? currentOS.Checklist : "";
                            
                            using (var frm = new FrmOSChecklistWeb(cl))
                            {
                                if (frm.ShowDialog() == DialogResult.OK)
                                {
                                    if (currentOS != null)
                                    {
                                        currentOS.Checklist = frm.ChecklistResult;
                                        repository.Atualizar(currentOS);
                                    }
                                }
                            }
                        }));
                        break;
                        
                    case "PRINT_BUDGET":
                        if (osId > 0)
                        {
                            new MasterServicePro.Utils.OSPrinter().Imprimir(osId);
                        }
                        break;

                    case "CANCEL":
                        this.DialogResult = DialogResult.Cancel;
                        break;

                    case "SAVE_OS":
                        dynamic data = message.data;
                        this.BeginInvoke((Action)(() => {
                            SaveData(data);
                        }));
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao processar mensagem do JS: " + ex.Message);
            }
        }

        private void SaveData(dynamic data)
        {
            try
            {
                var os = new OrdemServico();
                if (osId > 0)
                {
                    os = repository.BuscarPorId(osId);
                }
                else
                {
                    os.DataAbertura = DateTime.Now;
                }

                int idCliente = (int)data.IdCliente;
                os.IdCliente = idCliente;
                
                if (data.ClienteFinal != null)
                {
                    os.ClienteFinal = (string)data.ClienteFinal;
                }
                else
                {
                    os.ClienteFinal = null;
                }
                
                int? idTecnico = data.IdTecnico != null && (int)data.IdTecnico > 0 ? (int?)data.IdTecnico : null;
                os.IdTecnico = idTecnico;

                os.Marca = data.Marca;
                os.Modelo = data.Modelo;
                os.IMEI = data.IMEI;
                os.Cor = data.Cor;
                os.Defeito = data.Defeito;
                os.LaudoTecnico = data.LaudoTecnico;
                os.Status = data.Status;
                
                os.ValorPecas = (decimal)data.ValorPecas;
                os.ValorTotal = (decimal)data.ValorServico; // O total cobrado do cliente vem nesse campo

                decimal custoAdicionalTotal = 0;

                os.Itens.Clear();
                if (data.Itens != null)
                {
                    foreach (var item in data.Itens)
                    {
                        decimal custoAdicional = item.CustoAdicional != null ? (decimal)item.CustoAdicional : 0m;
                        custoAdicionalTotal += custoAdicional;

                        os.Itens.Add(new OrdemServicoItem
                        {
                            OrdemServicoId = os.Id,
                            ProdutoId = (int?)item.ProdutoId ?? ((int?)item.IdProduto ?? 0),
                            NomeProduto = item.NomeProduto,
                            Quantidade = (decimal)item.Quantidade,
                            ValorUnitario = (decimal)item.ValorUnitario,
                            SubTotal = (decimal)item.SubTotal,
                            CustoAdicional = custoAdicional
                        });
                    }
                }

                os.ValorServico = os.ValorTotal - os.ValorPecas - custoAdicionalTotal; // O valor do serviço real é o lucro

                if (osId > 0)
                {
                    repository.Atualizar(os);
                }
                else
                {
                    repository.Inserir(os);
                }

                if (os.Status.Equals("Finalizado", StringComparison.OrdinalIgnoreCase) && !os.Faturado)
                {
                    using (var frm = new FrmPromptPagamentoOSWeb(os.ValorTotal))
                    {
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            os.Faturado = true;
                            
                            if (frm.JaRecebido)
                            {
                                var finRepo = new MasterServicePro.DAL.FinanceiroRepository();
                                var caixa = finRepo.ObterCaixaAberto();
                                int idCaixa = caixa != null ? caixa.Id : 0;
                                
                                if (frm.FormaPagamento.ToLower() == "misto")
                                {
                                    if (frm.MistoDinheiro > 0)
                                        finRepo.LançarMovimentacao(new MasterServicePro.Models.Movimentacao { Tipo = "Entrada", Categoria = "Serviços", Subcategoria = "Ordem de Serviço", Valor = frm.MistoDinheiro, FormaPagamento = "Dinheiro", Descricao = $"Recebimento OS #{os.NumeroOS} (Misto)", IdCaixa = idCaixa });
                                    if (frm.MistoPix > 0)
                                        finRepo.LançarMovimentacao(new MasterServicePro.Models.Movimentacao { Tipo = "Entrada", Categoria = "Serviços", Subcategoria = "Ordem de Serviço", Valor = frm.MistoPix, FormaPagamento = "Pix", Descricao = $"Recebimento OS #{os.NumeroOS} (Misto)", IdCaixa = idCaixa });
                                    if (frm.MistoCartao > 0)
                                        finRepo.LançarMovimentacao(new MasterServicePro.Models.Movimentacao { Tipo = "Entrada", Categoria = "Serviços", Subcategoria = "Ordem de Serviço", Valor = frm.MistoCartao, FormaPagamento = "Cartão", Descricao = $"Recebimento OS #{os.NumeroOS} (Misto)", IdCaixa = idCaixa });
                                    if (frm.MistoSaldo > 0)
                                    {
                                        var repoCli = new MasterServicePro.DAL.ClienteRepository();
                                        repoCli.DescontarSaldo(os.IdCliente, frm.MistoSaldo);
                                    }
                                    if (frm.Troco > 0)
                                        finRepo.LançarMovimentacao(new MasterServicePro.Models.Movimentacao { Tipo = "Saída", Categoria = "Serviços", Subcategoria = "Troco", Valor = frm.Troco, FormaPagamento = "Dinheiro", Descricao = $"Troco OS #{os.NumeroOS}", IdCaixa = idCaixa });
                                }
                                else
                                {
                                    finRepo.LançarMovimentacao(new MasterServicePro.Models.Movimentacao
                                    {
                                        Tipo = "Entrada",
                                        Categoria = "Serviços",
                                        Subcategoria = "Ordem de Serviço",
                                        Valor = os.ValorTotal,
                                        FormaPagamento = frm.FormaPagamento,
                                        Descricao = $"Recebimento OS #{os.NumeroOS}",
                                        IdCaixa = idCaixa
                                    });
                                    if (frm.Troco > 0)
                                        finRepo.LançarMovimentacao(new MasterServicePro.Models.Movimentacao { Tipo = "Saída", Categoria = "Serviços", Subcategoria = "Troco", Valor = frm.Troco, FormaPagamento = "Dinheiro", Descricao = $"Troco OS #{os.NumeroOS}", IdCaixa = idCaixa });
                                }
                            }
                            else
                            {
                                var crRepo = new MasterServicePro.DAL.ContasReceberRepository();
                                crRepo.Inserir(new MasterServicePro.Models.ContaReceber
                                {
                                    IdCliente = os.IdCliente,
                                    Descricao = $"Serviço OS #{os.NumeroOS}",
                                    ValorTotal = os.ValorTotal,
                                    ValorPago = 0,
                                    DataLancamento = DateTime.Now,
                                    DataVencimento = DateTime.Now.AddDays(30),
                                    Status = "Pendente"
                                });
                            }
                            repository.Atualizar(os);
                        }
                    }
                }

                FrmNotification.ShowSuccess("Ordem de Serviço salva com sucesso!", "Sucesso");
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                FrmNotification.ShowError("Erro ao salvar: " + ex.Message, "Erro");
            }
        }
    }
}
