using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.IO;
using Newtonsoft.Json;
using MasterServicePro.DAL;

namespace MasterServicePro.Forms
{
    public partial class FrmCaixaHistorico : Form
    {
        private FinanceiroRepository repository = new FinanceiroRepository();
        public DateTime DataInicioSelecionada { get; private set; }
        public DateTime DataFimSelecionada { get; private set; }
        public decimal ValorAberturaSelecionado { get; private set; }
        public bool Selecionou { get; private set; }
        
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;

        public FrmCaixaHistorico()
        {
            InitializeComponent();
            this.Shown += FrmCaixaHistorico_Shown;
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
            this.webView.Size = new System.Drawing.Size(1000, 700);
            this.webView.TabIndex = 0;
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(1);
            try { this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath); } catch { }
            
            this.Controls.Add(this.webView);
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();
            this.ResumeLayout(false);
        }



        private void FrmCaixaHistorico_Shown(object sender, EventArgs e)
        {
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"CaixaHistorico\index.html");
                path = Path.GetFullPath(path);
                
                string webFolder = Path.GetDirectoryName(Path.GetDirectoryName(path));
                webView.CoreWebView2.SetVirtualHostNameToFolderMapping("app.local", webFolder, CoreWebView2HostResourceAccessKind.Allow);
                webView.CoreWebView2.Navigate("http://app.local/CaixaHistorico/index.html");
                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            }
            catch (System.Runtime.InteropServices.COMException ex) when ((uint)ex.ErrorCode == 0x80004004) { }
            catch { }
        }

        private async void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            var dt = repository.ListarCaixasFechados();
            var list = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns) dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                list.Add(dict);
            }

            string safeJsString = JsonConvert.SerializeObject(list);
            await webView.CoreWebView2.ExecuteScriptAsync($"loadData({safeJsString})");
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
                    this.DialogResult = DialogResult.Cancel;
                }
                else if (actionName == "SELECT")
                {
                    int id = (int)message.id;
                    var dt = repository.ListarCaixasFechados();
                    DataRow[] rows = dt.Select("Id = " + id);
                    if (rows.Length > 0)
                    {
                        var row = rows[0];
                        DataInicioSelecionada = Convert.ToDateTime(row["DataAbertura"]);
                        DataFimSelecionada = Convert.ToDateTime(row["DataFechamento"]);
                        ValorAberturaSelecionado = Convert.ToDecimal(row["ValorAbertura"]);
                        Selecionou = true;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch { }
        }
    }
}

