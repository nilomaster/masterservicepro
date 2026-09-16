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
    public partial class FrmUsuarioAddEdit : Form
    {
        private UsuarioRepository repository = new UsuarioRepository();
        private Usuario usuario;
        private bool isEdit = false;
        
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;

        public FrmUsuarioAddEdit(Usuario u = null)
        {
            if (u != null)
            {
                usuario = u;
                isEdit = true;
            }
            else
            {
                usuario = new Usuario();
            }

            InitializeComponent();
            this.Shown += FrmUsuarioAddEdit_Shown;
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
            this.webView.Size = new System.Drawing.Size(600, 600);
            this.webView.TabIndex = 0;
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.ClientSize = new System.Drawing.Size(600, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(1);
            try { this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath); } catch { }
            
            this.Controls.Add(this.webView);
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();
            this.ResumeLayout(false);
        }



        private void FrmUsuarioAddEdit_Shown(object sender, EventArgs e)
        {
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Web\UsuarioAddEdit\index.html");
                path = Path.GetFullPath(path);
                
                webView.CoreWebView2.Navigate(path);
                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            }
            catch (System.Runtime.InteropServices.COMException ex) when ((uint)ex.ErrorCode == 0x80004004) { }
            catch { }
        }

        private async void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            var data = new {
                isEdit = this.isEdit,
                usuario = this.usuario
            };

            string safeJsString = JsonConvert.SerializeObject(data);
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
                else if (actionName == "SAVE")
                {
                    dynamic u = message.usuario;
                    string password = u.Senha;
                    
                    usuario.Username = u.Username;
                    usuario.Nivel = u.Nivel;

                    if (isEdit)
                    {
                        bool updatePass = !string.IsNullOrEmpty(password);
                        if (updatePass) usuario.Senha = password;
                        
                        repository.Atualizar(usuario, updatePass);
                    }
                    else
                    {
                        usuario.Senha = password;
                        repository.Inserir(usuario);
                    }

                    this.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                this.BeginInvoke((Action)(() => {
                    FrmNotification.ShowError("Erro ao salvar usuário: " + ex.Message, "Erro");
                }));
            }
        }
    }
}

