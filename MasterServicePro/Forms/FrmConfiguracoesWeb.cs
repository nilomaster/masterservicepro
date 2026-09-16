using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.IO;
using Newtonsoft.Json;
using MasterServicePro.DAL;

namespace MasterServicePro.Forms
{
    public class FrmConfiguracoesWeb : Form
    {
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;

        public FrmConfiguracoesWeb()
        {
            InitializeComponent();
            this.Load += FrmConfiguracoesWeb_Load;
        }

        private void FrmConfiguracoesWeb_Load(object sender, EventArgs e)
        {
            InitializeAsync();
        }

        private void InitializeComponent()
        {
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();
            
            // WebView
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.FromArgb(26, 29, 36);
            this.webView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView.Location = new System.Drawing.Point(0, 0);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(1200, 750);
            this.webView.TabIndex = 0;
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(26, 29, 36);
            this.ClientSize = new System.Drawing.Size(1200, 750);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Controls.Add(this.webView);
            this.Name = "FrmConfiguracoesWeb";
            this.Text = "Configurações (Web)";
            
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);
        }

        async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
                
                string path = MasterServicePro.Utils.WebPathResolver.GetHtmlPath(@"Configuracoes\index.html");
                path = Path.GetFullPath(path);
                
                if (!File.Exists(path))
                {
                    MessageBox.Show("Erro: Arquivo HTML não encontrado em " + path, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                webView.CoreWebView2.Navigate(path);
                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            }
            catch (System.Runtime.InteropServices.COMException ex) when ((uint)ex.ErrorCode == 0x80004004 || (uint)ex.ErrorCode == 0x80040154)
            {
                // Ignorar erros de form fechado ou WebView2 destruído prematuramente
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inicializar WebView2: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var json = e.WebMessageAsJson;
                dynamic msg = JsonConvert.DeserializeObject(json);
                string action = msg.action;

                this.BeginInvoke((Action)(() => {
                    switch (action)
                    {
                        case "backup": BtnBackup_Click(); break;
                        case "reset_produtos": BtnResetProdutos_Click(); break;
                        case "reset_clientes": BtnResetClientes_Click(); break;
                        case "reset_financeiro": BtnResetFinanceiro_Click(); break;
                        case "reset_relatorios": BtnResetRelatorios_Click(); break;

                        case "request_termos": LoadTermos(); break;
                        case "save_termos": SaveTermos(msg.data); break;
                        
                        case "request_whatsapp": LoadWhatsApp(); break;
                        case "save_whatsapp": SaveWhatsApp(msg.data); break;
                        
                        case "request_usuarios": LoadUsuarios(); break;
                        case "save_usuario": SaveUsuario(msg.data); break;
                        case "delete_usuario": DeleteUsuario((int)msg.id); break;

                        case "request_tecnicos": LoadTecnicos(); break;
                        case "save_tecnico": SaveTecnico(msg.data); break;
                        case "delete_tecnico": DeleteTecnico((int)msg.id); break;

                        case "request_fornecedores": LoadFornecedores(); break;
                        case "save_fornecedor": SaveFornecedor(msg.data); break;
                        case "delete_fornecedor": DeleteFornecedor((int)msg.id); break;
                    }
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao processar mensagem web: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTermos()
        {
            try
            {
                var conf = new ConfiguracaoTermosRepository().ObterConfiguracao();
                var response = new { action = "load_termos", data = conf };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                FrmNotification.ShowError("Erro ao carregar termos: " + ex.Message, "Erro");
            }
        }

        private void SaveTermos(dynamic data)
        {
            try
            {
                var conf = new MasterServicePro.Models.ConfiguracaoImpressao
                {
                    NomeLoja = data.NomeLoja,
                    Cnpj = data.Cnpj,
                    Contato = data.Contato,
                    Endereco = data.Endereco,
                    TermosOS = data.TermosOS
                };
                new ConfiguracaoTermosRepository().Salvar(conf);
                FrmNotification.ShowSuccess("Termos e condições salvos com sucesso!", "Salvo");
                var response = new { action = "close_termos" };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                FrmNotification.ShowError("Erro ao salvar termos: " + ex.Message, "Erro");
            }
        }

        private void LoadWhatsApp()
        {
            try {
                var config = new ConfiguracaoWhatsAppRepository().Buscar();
                var response = new { action = "load_whatsapp", data = config };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(response));
            } catch { }
        }

        private void SaveWhatsApp(dynamic data)
        {
            try {
                var config = new MasterServicePro.Models.ConfiguracaoWhatsApp {
                    Id = 1,
                    UsarAPI = (bool)data.UsarAPI,
                    ApiUrl = (string)data.ApiUrl,
                    ApiToken = (string)data.ApiToken,
                    Instancia = (string)data.Instancia,
                    NomeAdministrador = (string)data.NomeAdministrador,
                    TelefoneAdministrador = (string)data.TelefoneAdministrador,
                    TemplateAbertura = (string)data.TemplateAbertura,
                    TemplateFinalizado = (string)data.TemplateFinalizado,
                    TemplateAtualizacao = (string)data.TemplateAtualizacao
                };
                new ConfiguracaoWhatsAppRepository().Salvar(config);
                FrmNotification.ShowSuccess("Configurações do WhatsApp salvas com sucesso!", "Salvo");
                var response = new { action = "close_whatsapp" };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(response));
            } catch (Exception ex) {
                FrmNotification.ShowError("Erro: " + ex.Message, "Erro");
            }
        }

        private void LoadUsuarios()
        {
            try {
                var usuarios = new UsuarioRepository().BuscarTodos();
                var response = new { action = "load_usuarios", data = usuarios };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(response));
            } catch { }
        }

        private void SaveUsuario(dynamic data)
        {
            try {
                var repo = new UsuarioRepository();
                var u = new MasterServicePro.Models.Usuario {
                    Id = (int)data.Id,
                    Username = (string)data.Username,
                    Senha = (string)data.Senha,
                    Nivel = (string)data.Nivel
                };

                if (string.IsNullOrEmpty(u.Username)) {
                    FrmNotification.ShowError("O login é obrigatório.", "Atenção");
                    return;
                }

                if (u.Id > 0)
                {
                    var existente = System.Linq.Enumerable.FirstOrDefault(repo.BuscarTodos(), x => x.Id == u.Id);
                    if (existente != null)
                    {
                        existente.Username = u.Username;
                        bool updateSenha = !string.IsNullOrEmpty(u.Senha);
                        if (updateSenha) existente.Senha = u.Senha;
                        existente.Nivel = u.Nivel;
                        repo.Atualizar(existente, updateSenha);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(u.Senha)) {
                        FrmNotification.ShowError("A senha é obrigatória para novos usuários.", "Atenção");
                        return;
                    }
                    repo.Inserir(u);
                }

                FrmNotification.ShowSuccess("Usuário salvo com sucesso!", "Salvo");
                var response = new { action = "refresh_usuarios" };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(response));
            } catch (Exception ex) {
                FrmNotification.ShowError("Erro: " + ex.Message, "Erro");
            }
        }

        private void DeleteUsuario(int id)
        {
            try {
                if (id == MasterServicePro.DAL.AuthSession.Id) {
                    FrmNotification.ShowError("Você não pode excluir a si mesmo!", "Erro");
                    return;
                }
                new UsuarioRepository().Excluir(id);
                FrmNotification.ShowSuccess("Usuário removido!", "Excluído");
                var response = new { action = "refresh_usuarios" };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(response));
            } catch (Exception ex) {
                FrmNotification.ShowError("Erro: " + ex.Message, "Erro");
            }
        }

        private void LoadTecnicos()
        {
            try {
                var tecnicos = new TecnicoRepository().BuscarTodos();
                var response = new { action = "load_tecnicos", data = tecnicos };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(response));
            } catch { }
        }

        private void SaveTecnico(dynamic data)
        {
            try {
                var repo = new TecnicoRepository();
                var t = new MasterServicePro.Models.Tecnico {
                    Id = (int)data.Id,
                    Nome = (string)data.Nome,
                    Telefone = (string)data.Telefone,
                    Especialidade = (string)data.Especialidade,
                    Ativo = true,
                    Status = "Disponível",
                    DataAtualizacao = DateTime.Now
                };

                if (string.IsNullOrEmpty(t.Nome)) {
                    FrmNotification.ShowError("O nome do técnico é obrigatório.", "Atenção");
                    return;
                }

                if (t.Id > 0)
                {
                    var existente = repo.BuscarPorId(t.Id);
                    if (existente != null)
                    {
                        existente.Nome = t.Nome;
                        existente.Telefone = t.Telefone;
                        existente.Especialidade = t.Especialidade;
                        existente.DataAtualizacao = DateTime.Now;
                        repo.Atualizar(existente);
                    }
                }
                else
                {
                    t.DataCadastro = DateTime.Now;
                    repo.Inserir(t);
                }

                FrmNotification.ShowSuccess("Técnico salvo com sucesso!", "Salvo");
                var response = new { action = "refresh_tecnicos" };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(response));
            } catch (Exception ex) {
                FrmNotification.ShowError("Erro: " + ex.Message, "Erro");
            }
        }

        private void DeleteTecnico(int id)
        {
            try {
                new TecnicoRepository().Excluir(id);
                FrmNotification.ShowSuccess("Técnico removido com sucesso!", "Excluído");
                var response = new { action = "refresh_tecnicos" };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(response));
            } catch (Exception ex) {
                FrmNotification.ShowError("Erro: " + ex.Message, "Erro");
            }
        }

        private async void LoadFornecedores()
        {
            try {
                var fornecedores = await new FornecedorRepository().BuscarTodosAsync();
                var response = new { action = "load_fornecedores", data = fornecedores };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(response));
            } catch { }
        }

        private void SaveFornecedor(dynamic data)
        {
            try {
                var repo = new FornecedorRepository();
                var f = new MasterServicePro.Models.Fornecedor {
                    Id = (int)data.Id,
                    Nome = (string)data.Nome,
                    Telefone = (string)data.Telefone,
                    Cnpj = (string)data.Cnpj,
                    NomeLoja = (string)data.NomeLoja,
                    Ativo = true
                };

                if (string.IsNullOrEmpty(f.Nome)) {
                    FrmNotification.ShowError("O nome do fornecedor é obrigatório.", "Atenção");
                    return;
                }

                if (f.Id > 0) {
                    var existente = repo.BuscarPorId(f.Id);
                    if (existente != null) {
                        existente.Nome = f.Nome;
                        existente.Telefone = f.Telefone;
                        existente.Cnpj = f.Cnpj;
                        existente.NomeLoja = f.NomeLoja;
                        repo.Atualizar(existente);
                    }
                } else {
                    repo.Inserir(f);
                }

                FrmNotification.ShowSuccess("Fornecedor salvo com sucesso!", "Salvo");
                var response = new { action = "refresh_fornecedores" };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(response));
            } catch (Exception ex) {
                FrmNotification.ShowError("Erro ao salvar fornecedor: " + ex.Message, "Erro");
            }
        }

        private void DeleteFornecedor(int id)
        {
            try {
                new FornecedorRepository().Excluir(id);
                FrmNotification.ShowSuccess("Fornecedor removido com sucesso!", "Excluído");
                var response = new { action = "refresh_fornecedores" };
                webView.CoreWebView2.PostWebMessageAsString(JsonConvert.SerializeObject(response));
            } catch (Exception ex) {
                FrmNotification.ShowError("Erro: " + ex.Message, "Erro");
            }
        }

        private void BtnBackup_Click()
        {
            try
            {
                var repo = new BackupRepository();
                string path = repo.RealizarBackup();
                FrmNotification.ShowSuccess($"Backup realizado com sucesso!\nSalvo em: {path}", "Backup Concluído");
            }
            catch (Exception ex)
            {
                FrmNotification.ShowError("Erro ao realizar backup: " + ex.Message, "Erro no Backup");
            }
        }

        private void BtnResetProdutos_Click()
        {
            bool confirm = FrmNotification.ShowConfirm("Deseja realmente ZERAR todos os produtos?\nEsta ação é irreversível e apagará o estoque e vínculos de itens!", "Atenção - Zerar Produtos");
            if (confirm)
            {
                try
                {
                    var repo = new BackupRepository();
                    repo.ZerarProdutos();
                    FrmNotification.ShowSuccess("Todos os produtos foram removidos com sucesso!", "Banco Resetado");
                }
                catch (Exception ex)
                {
                    FrmNotification.ShowError("Erro ao zerar produtos: " + ex.Message, "Erro");
                }
            }
        }

        private void BtnResetClientes_Click()
        {
            bool confirm = FrmNotification.ShowConfirm("Deseja realmente ZERAR todos os clientes?\nEsta ação é irreversível e também apagará todas as ordens de serviço vinculadas!", "Atenção - Zerar Clientes");
            if (confirm)
            {
                try
                {
                    var repo = new BackupRepository();
                    repo.ZerarClientes();
                    FrmNotification.ShowSuccess("Todos os clientes e ordens de serviço foram removidos com sucesso!", "Banco Resetado");
                }
                catch (Exception ex)
                {
                    FrmNotification.ShowError("Erro ao zerar clientes: " + ex.Message, "Erro");
                }
            }
        }

        private void BtnResetFinanceiro_Click()
        {
            bool confirm = FrmNotification.ShowConfirm("Deseja realmente ZERAR todo o financeiro?\nEsta ação é irreversível e apagará todas as vendas, pagamentos e movimentações de caixa!", "Atenção - Zerar Financeiro");
            if (confirm)
            {
                try
                {
                    var repo = new BackupRepository();
                    repo.ZerarFinanceiro();
                    FrmNotification.ShowSuccess("Todos os registros financeiros, vendas e fluxos de caixa foram limpos!", "Banco Resetado");
                }
                catch (Exception ex)
                {
                    FrmNotification.ShowError("Erro ao zerar financeiro: " + ex.Message, "Erro");
                }
            }
        }

        private void BtnResetRelatorios_Click()
        {
            bool confirm = FrmNotification.ShowConfirm("Deseja realmente ZERAR todos os relatórios e auditorias de sistema?\nEssa ação limpará os registros das movimentações passadas.", "Atenção - Zerar Relatórios");
            if (confirm)
            {
                try
                {
                    var repo = new BackupRepository();
                    repo.ZerarAuditoria();
                    FrmNotification.ShowSuccess("Todos os relatórios e logs de sistema foram limpos!", "Auditoria Resetada");
                }
                catch (Exception ex)
                {
                    FrmNotification.ShowError("Erro ao zerar relatórios: " + ex.Message, "Erro");
                }
            }
        }
    }
}
