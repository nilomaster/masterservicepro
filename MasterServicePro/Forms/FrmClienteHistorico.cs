using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.IO;
using Newtonsoft.Json;
using MasterServicePro.Models;
using MasterServicePro.DAL;
using System.Data;
using System.Collections.Generic;

namespace MasterServicePro.Forms
{
    public partial class FrmClienteHistorico : Form
    {
        private Cliente cliente;
        private ClienteRepository repository = new ClienteRepository();
        
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;

        public FrmClienteHistorico(Cliente cliente)
        {
            this.cliente = cliente;
            InitializeComponent();
            this.Shown += FrmClienteHistorico_Shown;
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
            this.webView.Size = new System.Drawing.Size(900, 600);
            this.webView.TabIndex = 0;
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(1);
            try { this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath); } catch { }
            
            this.Controls.Add(this.webView);
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();
            this.ResumeLayout(false);
        }

        private void FrmClienteHistorico_Shown(object sender, EventArgs e)
        {
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"ClienteHistorico\index.html");
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
            DataTable dtOS = repository.GetHistoricoOS(cliente.Id);
            DataTable dtVendas = repository.GetHistoricoVendas(cliente.Id);

            var osList = new List<object>();
            foreach(DataRow row in dtOS.Rows)
            {
                osList.Add(new {
                    Id = row["Id"],
                    Marca = row["Marca"]?.ToString(),
                    Modelo = row["Modelo"]?.ToString(),
                    Defeito = row["Defeito"]?.ToString(),
                    Status = row["Status"]?.ToString(),
                    ValorTotal = row["ValorTotal"] != DBNull.Value ? Convert.ToDecimal(row["ValorTotal"]) : 0,
                    DataAbertura = row["DataAbertura"] != DBNull.Value ? Convert.ToDateTime(row["DataAbertura"]).ToString("yyyy-MM-ddTHH:mm:ss") : null
                });
            }

            var vendasList = new List<object>();
            foreach(DataRow row in dtVendas.Rows)
            {
                vendasList.Add(new {
                    Id = row["Id"],
                    TotalFinal = row["TotalFinal"] != DBNull.Value ? Convert.ToDecimal(row["TotalFinal"]) : 0,
                    FormaPagamento = row["FormaPagamento"]?.ToString(),
                    DataVenda = row["DataVenda"] != DBNull.Value ? Convert.ToDateTime(row["DataVenda"]).ToString("yyyy-MM-ddTHH:mm:ss") : null
                });
            }

            var data = new {
                cliente = cliente,
                historicoOS = osList,
                historicoVendas = vendasList
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

                if (actionName == "CLOSE")
                {
                    this.BeginInvoke((Action)(() => {
                        this.Close();
                    }));
                }
            }
            catch (Exception ex)
            {
                this.BeginInvoke((Action)(() => {
                    FrmNotification.ShowError("Erro: " + ex.Message, "Erro");
                }));
            }
        }
    }
}
