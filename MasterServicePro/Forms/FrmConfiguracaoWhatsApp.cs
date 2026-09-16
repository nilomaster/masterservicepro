using System;
using System.Drawing;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Models;
using MasterServicePro.Services;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public class FrmConfiguracaoWhatsApp : Form
    {
        private readonly ConfiguracaoWhatsAppRepository _repository = new ConfiguracaoWhatsAppRepository();
        private readonly WhatsAppService _whatsAppService = new WhatsAppService();

        // Controls
        private Label lblTitle;
        private Label lblSubtitle;
        private CheckBox chkUsarAPI;
        private Panel pnlApiSettings;
        private Label lblApiUrl;
        private TextBox txtApiUrl;
        private Label lblApiToken;
        private TextBox txtApiToken;
        private Label lblInstancia;
        private TextBox txtInstancia;

        private Label lblAdminName;
        private TextBox txtAdminName;
        private Label lblAdminPhone;
        private TextBox txtAdminPhone;

        private Label lblTemplatesTitle;
        private Label lblAbertura;
        private TextBox txtTemplateAbertura;
        private Label lblFinalizado;
        private TextBox txtTemplateFinalizado;
        private Label lblAtualizacao;
        private TextBox txtTemplateAtualizacao;
        private Label lblVariaveisHelp;

        private Button btnSalvar;
        private Button btnTestar;
        private Button btnCancelar;

        public FrmConfiguracaoWhatsApp()
        {
            InitializeComponent();
            ApplyTheme();
            LoadConfig();
            BindEvents();
        }

        private void InitializeComponent()
        {
            this.Text = "Configurações de Integração do WhatsApp";
            this.Size = new Size(880, 660);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title
            lblTitle = new Label
            {
                Text = "Integração WhatsApp",
                Location = new Point(30, 20),
                Size = new Size(400, 32),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold)
            };

            lblSubtitle = new Label
            {
                Text = "Configure o envio manual via WhatsApp Web ou automático via API de mensageria.",
                Location = new Point(30, 55),
                Size = new Size(800, 20),
                Font = new Font("Segoe UI", 9.5F)
            };

            // Left Side: Connection settings
            chkUsarAPI = new CheckBox
            {
                Text = "Habilitar Envio Automático (via Gateway API)",
                Location = new Point(30, 100),
                Size = new Size(380, 24),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            pnlApiSettings = new Panel
            {
                Location = new Point(30, 135),
                Size = new Size(390, 270),
                BorderStyle = BorderStyle.None
            };

            lblApiUrl = new Label
            {
                Text = "URL do Gateway API:",
                Location = new Point(10, 10),
                Size = new Size(370, 20),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            txtApiUrl = new TextBox
            {
                Location = new Point(10, 32),
                Size = new Size(370, 27),
                Font = new Font("Segoe UI", 10.5F),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblApiToken = new Label
            {
                Text = "API Token (Token de Acesso / ApiKey):",
                Location = new Point(10, 80),
                Size = new Size(370, 20),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            txtApiToken = new TextBox
            {
                Location = new Point(10, 102),
                Size = new Size(370, 27),
                Font = new Font("Segoe UI", 10.5F),
                PasswordChar = '•',
                BorderStyle = BorderStyle.FixedSingle
            };

            lblInstancia = new Label
            {
                Text = "Nome da Instância (Opcional):",
                Location = new Point(10, 150),
                Size = new Size(370, 20),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            txtInstancia = new TextBox
            {
                Location = new Point(10, 172),
                Size = new Size(370, 27),
                Font = new Font("Segoe UI", 10.5F),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblConnectionHelp = new Label
            {
                Text = "Nota: Caso utilize Evolution API, use a URL completa da mensagem (/message/sendText) ou insira a tag {instance} na URL.",
                Location = new Point(10, 210),
                Size = new Size(370, 50),
                Font = new Font("Segoe UI", 9F, FontStyle.Italic)
            };

            lblAdminName = new Label
            {
                Text = "Nome do Administrador (Relatórios):",
                Location = new Point(30, 420),
                Size = new Size(370, 20),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            txtAdminName = new TextBox
            {
                Location = new Point(30, 442),
                Size = new Size(370, 27),
                Font = new Font("Segoe UI", 10.5F),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblAdminPhone = new Label
            {
                Text = "WhatsApp do Administrador:",
                Location = new Point(30, 480),
                Size = new Size(370, 20),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            txtAdminPhone = new TextBox
            {
                Location = new Point(30, 502),
                Size = new Size(370, 27),
                Font = new Font("Segoe UI", 10.5F),
                BorderStyle = BorderStyle.FixedSingle
            };

            pnlApiSettings.Controls.Add(lblApiUrl);
            pnlApiSettings.Controls.Add(txtApiUrl);
            pnlApiSettings.Controls.Add(lblApiToken);
            pnlApiSettings.Controls.Add(txtApiToken);
            pnlApiSettings.Controls.Add(lblInstancia);
            pnlApiSettings.Controls.Add(txtInstancia);
            pnlApiSettings.Controls.Add(lblConnectionHelp);

            // Right Side: Message Templates
            lblTemplatesTitle = new Label
            {
                Text = "Templates de Mensagens",
                Location = new Point(460, 100),
                Size = new Size(380, 22),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold)
            };

            lblAbertura = new Label
            {
                Text = "Quando O.S. for Aberta:",
                Location = new Point(460, 130),
                Size = new Size(380, 20),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            txtTemplateAbertura = new TextBox
            {
                Location = new Point(460, 152),
                Size = new Size(380, 75),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Segoe UI", 9.5F),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblFinalizado = new Label
            {
                Text = "Quando O.S. for Finalizada (Pronta):",
                Location = new Point(460, 240),
                Size = new Size(380, 20),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            txtTemplateFinalizado = new TextBox
            {
                Location = new Point(460, 262),
                Size = new Size(380, 75),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Segoe UI", 9.5F),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblAtualizacao = new Label
            {
                Text = "Outras Atualizações de Status:",
                Location = new Point(460, 350),
                Size = new Size(380, 20),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            txtTemplateAtualizacao = new TextBox
            {
                Location = new Point(460, 372),
                Size = new Size(380, 75),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Segoe UI", 9.5F),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblVariaveisHelp = new Label
            {
                Text = "Variáveis válidas:\n{Cliente} - Nome do Cliente | {OS} - Número da O.S.\n{Aparelho} - Marca/Modelo | {Valor} - R$ Total\n{Status} - Status da O.S. | {Defeito} - Defeito relatado | {Laudo} - Laudo Técnico",
                Location = new Point(460, 455),
                Size = new Size(380, 60),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic)
            };

            // Bottom buttons
            btnSalvar = new Button
            {
                Text = "💾 Salvar Configurações",
                Location = new Point(30, 560),
                Size = new Size(220, 40)
            };

            btnTestar = new Button
            {
                Text = "💬 Enviar Mensagem Teste",
                Location = new Point(265, 560),
                Size = new Size(220, 40)
            };

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(620, 560),
                Size = new Size(220, 40)
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblSubtitle);
            this.Controls.Add(chkUsarAPI);
            this.Controls.Add(pnlApiSettings);
            this.Controls.Add(lblAdminName);
            this.Controls.Add(txtAdminName);
            this.Controls.Add(lblAdminPhone);
            this.Controls.Add(txtAdminPhone);
            this.Controls.Add(lblTemplatesTitle);
            this.Controls.Add(lblAbertura);
            this.Controls.Add(txtTemplateAbertura);
            this.Controls.Add(lblFinalizado);
            this.Controls.Add(txtTemplateFinalizado);
            this.Controls.Add(lblAtualizacao);
            this.Controls.Add(txtTemplateAtualizacao);
            this.Controls.Add(lblVariaveisHelp);
            this.Controls.Add(btnSalvar);
            this.Controls.Add(btnTestar);
            this.Controls.Add(btnCancelar);
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModalWindowStyle(this);
            this.BackColor = UITheme.BgModal;

            lblTitle.ForeColor = UITheme.TextTitle;
            lblSubtitle.ForeColor = UITheme.TextMuted;
            chkUsarAPI.ForeColor = UITheme.TextTitle;

            pnlApiSettings.BackColor = UITheme.BgCard;
            pnlApiSettings.Paint += (s, e) => UITheme.DrawRoundedPanel(e.Graphics, pnlApiSettings.ClientRectangle, UITheme.BgCard, 10, UITheme.ElementBorder);

            foreach (Control c in pnlApiSettings.Controls)
            {
                if (c is Label lbl)
                {
                    lbl.ForeColor = lbl.Font.Italic ? UITheme.TextMuted : UITheme.TextTitle;
                }
                else if (c is TextBox txt)
                {
                    txt.BackColor = UITheme.BgSidebar;
                    txt.ForeColor = UITheme.TextTitle;
                }
            }

            lblTemplatesTitle.ForeColor = UITheme.TextTitle;
            lblAbertura.ForeColor = UITheme.TextTitle;
            lblFinalizado.ForeColor = UITheme.TextTitle;
            lblAtualizacao.ForeColor = UITheme.TextTitle;
            lblVariaveisHelp.ForeColor = UITheme.TextMuted;

            lblAdminName.ForeColor = UITheme.TextTitle;
            lblAdminPhone.ForeColor = UITheme.TextTitle;
            txtAdminName.BackColor = UITheme.BgSidebar;
            txtAdminName.ForeColor = UITheme.TextTitle;
            txtAdminPhone.BackColor = UITheme.BgSidebar;
            txtAdminPhone.ForeColor = UITheme.TextTitle;

            foreach (var txt in new[] { txtTemplateAbertura, txtTemplateFinalizado, txtTemplateAtualizacao })
            {
                txt.BackColor = UITheme.BgSidebar;
                txt.ForeColor = UITheme.TextTitle;
            }

            UITheme.FormatSaaSButton(btnSalvar, true);
            UITheme.FormatSaaSButton(btnTestar, false);
            btnTestar.BackColor = UITheme.Success;
            btnTestar.ForeColor = Color.White;
            UITheme.FormatSaaSButton(btnCancelar, false);
        }

        private void LoadConfig()
        {
            var config = _repository.Buscar();
            chkUsarAPI.Checked = config.UsarAPI;
            txtApiUrl.Text = config.ApiUrl;
            txtApiToken.Text = config.ApiToken;
            txtInstancia.Text = config.Instancia;
            txtTemplateAbertura.Text = config.TemplateAbertura;
            txtTemplateFinalizado.Text = config.TemplateFinalizado;
            txtTemplateAtualizacao.Text = config.TemplateAtualizacao;
            txtAdminName.Text = config.NomeAdministrador;
            txtAdminPhone.Text = config.TelefoneAdministrador;

            ToggleApiFields(config.UsarAPI);
        }

        private void BindEvents()
        {
            chkUsarAPI.CheckedChanged += (s, e) => ToggleApiFields(chkUsarAPI.Checked);
            btnSalvar.Click += BtnSalvar_Click;
            btnCancelar.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            btnTestar.Click += BtnTestar_Click;
        }

        private void ToggleApiFields(bool enabled)
        {
            txtApiUrl.Enabled = enabled;
            txtApiToken.Enabled = enabled;
            txtInstancia.Enabled = enabled;
            
            Color bg = enabled ? UITheme.BgSidebar : Color.FromArgb(50, 50, 50);
            txtApiUrl.BackColor = bg;
            txtApiToken.BackColor = bg;
            txtInstancia.BackColor = bg;
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            var config = new ConfiguracaoWhatsApp
            {
                UsarAPI = chkUsarAPI.Checked,
                ApiUrl = txtApiUrl.Text.Trim(),
                ApiToken = txtApiToken.Text.Trim(),
                Instancia = txtInstancia.Text.Trim(),
                TemplateAbertura = txtTemplateAbertura.Text,
                TemplateFinalizado = txtTemplateFinalizado.Text,
                TemplateAtualizacao = txtTemplateAtualizacao.Text,
                NomeAdministrador = txtAdminName.Text.Trim(),
                TelefoneAdministrador = txtAdminPhone.Text.Trim()
            };

            try
            {
                _repository.Salvar(config);
                FrmNotification.ShowSuccess("Configurações do WhatsApp salvas com sucesso!", "Salvo");
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                FrmNotification.ShowError("Erro ao salvar configurações: " + ex.Message, "Erro");
            }
        }

        private async void BtnTestar_Click(object sender, EventArgs e)
        {
            string num = PromptDialog("Digite o WhatsApp de destino (ex: 11999999999):", "Teste de Envio");
            if (string.IsNullOrWhiteSpace(num)) return;

            var configTemp = new ConfiguracaoWhatsApp
            {
                UsarAPI = chkUsarAPI.Checked,
                ApiUrl = txtApiUrl.Text.Trim(),
                ApiToken = txtApiToken.Text.Trim(),
                Instancia = txtInstancia.Text.Trim(),
                TemplateAbertura = txtTemplateAbertura.Text,
                TemplateFinalizado = txtTemplateFinalizado.Text,
                TemplateAtualizacao = txtTemplateAtualizacao.Text,
                NomeAdministrador = txtAdminName.Text.Trim(),
                TelefoneAdministrador = txtAdminPhone.Text.Trim()
            };

            string msg = "Esta é uma mensagem de teste enviada pelo sistema MasterServicePro.";
            
            btnTestar.Enabled = false;
            btnTestar.Text = "Enviando...";

            try
            {
                await _whatsAppService.EnviarMensagemAsync(num, msg, configTemp);
                FrmNotification.ShowSuccess("Mensagem enviada! Verifique seu dispositivo.", "Sucesso");
            }
            catch (Exception ex)
            {
                FrmNotification.ShowError("Falha ao enviar teste: " + ex.Message, "Erro no Teste");
            }
            finally
            {
                btnTestar.Enabled = true;
                btnTestar.Text = "💬 Enviar Mensagem Teste";
            }
        }

        private string PromptDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 350,
                Height = 180,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = UITheme.BgCard
            };
            Label textLabel = new Label() { Left = 20, Top = 20, Width = 300, Text = text, ForeColor = UITheme.TextTitle, Font = UITheme.FontBody };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 300, Font = UITheme.FontBody, BackColor = UITheme.BgSidebar, ForeColor = UITheme.TextTitle };
            Button confirmation = new Button() { Text = "Enviar", Left = 220, Width = 100, Top = 90, DialogResult = DialogResult.OK };
            UITheme.FormatSaaSButton(confirmation, true);
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
