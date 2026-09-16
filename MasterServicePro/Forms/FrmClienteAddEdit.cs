using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.IO;
using Newtonsoft.Json;
using MasterServicePro.Models;
using MasterServicePro.DAL;

namespace MasterServicePro.Forms
{
    public partial class FrmClienteAddEdit : Form
    {
        private int clienteId = 0;
        private ClienteRepository repository = new ClienteRepository();
        
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;

        public FrmClienteAddEdit(int id = 0)
        {
            this.clienteId = id;
            InitializeComponent();
            this.Shown += FrmClienteAddEdit_Shown;
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
            this.webView.Size = new System.Drawing.Size(800, 700);
            this.webView.TabIndex = 0;
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(51, 65, 85); // Border color (#334155)
            this.ClientSize = new System.Drawing.Size(800, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(1);
            try { this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath); } catch { }
            
            this.Controls.Add(this.webView);
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();
            this.ResumeLayout(false);
        }



        private void FrmClienteAddEdit_Shown(object sender, EventArgs e)
        {
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"ClienteAddEdit\index.html");
                path = Path.GetFullPath(path);
                
                webView.CoreWebView2.Navigate(path);
                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            }
            catch (Exception ex)
            {
                MessageBox.Show("InitializeAsync Error: " + ex.ToString());
            }
        }

        private async void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            Cliente c = null;
            if (clienteId > 0)
            {
                c = repository.BuscarPorId(clienteId);
            }

            var data = new {
                id = clienteId,
                cliente = c
            };

            string safeJsString = JsonConvert.SerializeObject(data);
            try {
                await webView.CoreWebView2.ExecuteScriptAsync($"loadData({safeJsString})");
            } catch (Exception ex) {
                MessageBox.Show("ExecuteScriptAsync Error: " + ex.Message);
            }
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string jsonString = e.WebMessageAsJson;
                dynamic message = JsonConvert.DeserializeObject(jsonString);
                string actionName = message.action.ToString();

                if (actionName == "CANCEL")
                {
                    this.BeginInvoke((Action)(() => {
                        FrmNotification.ShowError("Edição/Cadastro do cliente cancelado.", "Cancelado");
                        this.DialogResult = DialogResult.Cancel;
                    }));
                }
                else if (actionName == "SAVE")
                {
                    dynamic cl = message.cliente;

                    var cliente = new Cliente
                    {
                        Id = clienteId,
                        Nome = cl.Nome,
                        CpfCnpj = cl.CpfCnpj,
                        Telefone = cl.Telefone,
                        WhatsApp = cl.WhatsApp,
                        Email = cl.Email,
                        Endereco = cl.Endereco,
                        Historico = cl.Historico,
                        Saldo = (decimal)cl.Saldo,
                        DataAtualizacao = DateTime.Now
                    };

                    if (clienteId == 0)
                    {
                        cliente.Ativo = true;
                        repository.Inserir(cliente);
                    }
                    else
                    {
                        repository.Atualizar(cliente);
                    }
                    this.BeginInvoke((Action)(() => {
                        FrmNotification.ShowSuccess("Cliente salvo com sucesso!", "Sucesso");
                        this.DialogResult = DialogResult.OK;
                    }));
                }
            }
            catch (Exception ex)
            {
                this.BeginInvoke((Action)(() => {
                    FrmNotification.ShowError("Erro ao salvar cliente: " + ex.Message, "Erro");
                }));
            }
        }
    }
}

