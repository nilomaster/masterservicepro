using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.IO;
using Newtonsoft.Json;
using MasterServicePro.DAL;
using System.Collections.Generic;
using MasterServicePro.Models;
using System.Linq;

namespace MasterServicePro.Forms
{
    public class FrmContasReceberWeb : Form
    {
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private ContasReceberRepository _repository = new ContasReceberRepository();
        private ClienteRepository _clienteRepo = new ClienteRepository();
        private FinanceiroRepository _financeiroRepo = new FinanceiroRepository();

        public FrmContasReceberWeb()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitializeAsync();
        }

        private void InitializeComponent()
        {
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();
            
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.FromArgb(26, 29, 36);
            this.webView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView.Location = new System.Drawing.Point(0, 0);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(1120, 680);
            this.webView.TabIndex = 0;
            this.webView.ZoomFactor = 1D;
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(26, 29, 36);
            this.ClientSize = new System.Drawing.Size(1200, 750);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Controls.Add(this.webView);
            this.Name = "FrmContasReceberWeb";
            this.Text = "Contas a Receber";
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"ContasReceber\index.html");
                path = Path.GetFullPath(path);
                
                if (!File.Exists(path))
                {
                    MessageBox.Show("Erro: Arquivo HTML não encontrado em " + path, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                webView.CoreWebView2.Navigate(path);
                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                webView.NavigationCompleted += (s, e) => { LoadData(true); };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inicializar o navegador: " + ex.Message, "Erro WebView", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var json = e.WebMessageAsJson;
                dynamic msg = JsonConvert.DeserializeObject(json);
                string action = msg.action;

                if (action == "fechar")
                {
                    this.Close();
                }
                else if (action == "novo_lancamento")
                {
                    BtnNovoLancamento_Click();
                }
                else if (action == "registrar_recebimento")
                {
                    int id = (int)msg.id;
                    BtnReceber_Click(id);
                }
            }
            catch { }
        }

        private async void LoadData(bool loadClientes = false)
        {
            try
            {
                var contas = _repository.Listar();
                var clientes = await _clienteRepo.BuscarTodosAsync();
                
                var data = contas.Select(c => {
                    var cli = clientes.FirstOrDefault(x => x.Id == c.IdCliente);
                    return new {
                        Id = c.Id,
                        IdCliente = c.IdCliente,
                        ClienteNome = cli != null ? cli.Nome : "Cliente Desconhecido",
                        Descricao = c.Descricao,
                        ValorTotal = c.ValorTotal,
                        ValorPago = c.ValorPago,
                        ValorRestante = c.ValorRestante,
                        DataVencimento = c.DataVencimento,
                        Status = c.Status
                    };
                }).ToList();

                decimal totalPendente = 0;
                decimal totalAtrasado = 0;
                decimal totalRecebido = 0;

                foreach (var c in contas)
                {
                    if (c.Status == "Pendente") totalPendente += c.ValorRestante;
                    else if (c.Status == "Atrasado") totalAtrasado += c.ValorRestante;
                    totalRecebido += c.ValorPago;
                }

                dynamic payload;
                
                if (loadClientes)
                {
                    payload = new {
                        action = "load_data",
                        data = data,
                        valPendente = totalPendente,
                        valAtrasado = totalAtrasado,
                        valRecebido = totalRecebido,
                        clientes = clientes.Select(c => new { Id = c.Id, Nome = c.Nome }).ToList()
                    };
                }
                else
                {
                    payload = new {
                        action = "load_data",
                        data = data,
                        valPendente = totalPendente,
                        valAtrasado = totalAtrasado,
                        valRecebido = totalRecebido
                    };
                }
                
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(payload));
            }
            catch { }
        }

        private void BtnReceber_Click(int id)
        {
            var conta = _repository.BuscarPorId(id);
            if (conta == null) return;
            
            if (conta.Status == "Pago")
            {
                MessageBox.Show("Esta conta já foi paga.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var caixa = _financeiroRepo.ObterCaixaAberto();
            if (caixa == null)
            {
                MessageBox.Show("Você precisa abrir o caixa do dia antes de registrar um recebimento.", "Caixa Fechado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var prompt = new FrmPromptRecebimento(conta.ValorRestante))
            {
                if (prompt.ShowDialog() == DialogResult.OK)
                {
                    decimal valor = prompt.ValorInformado;
                    string metodo = prompt.MetodoPagamento;

                    try
                    {
                        _repository.DarBaixa(conta.Id, valor, metodo, caixa.Id);
                        MessageBox.Show("Recebimento lançado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData(false);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao registrar baixa: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnNovoLancamento_Click()
        {
            using (var frm = new FrmNovoLancamentoReceber())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _repository.Inserir(frm.NovoLancamento);
                        MessageBox.Show("Lançamento de débito efetuado com sucesso!", "Lançado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData(false);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao lançar conta: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
