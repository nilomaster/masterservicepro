using System;
using System.Drawing;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public partial class FrmConfiguracoes : Form
    {
        public FrmConfiguracoes()
        {
            InitializeComponent();
            ApplyTheme();
            BindEvents();
        }

        private void ApplyTheme()
        {
            this.BackColor = UITheme.BgApp;
            lblTitle.ForeColor = UITheme.TextTitle;
            
            // Estilo do Card Backup
            pnlBackupCard.BackColor = UITheme.BgApp;
            lblIcon.ForeColor = UITheme.TextTitle;
            lblBackupTitle.ForeColor = UITheme.TextTitle;
            lblBackupDesc.ForeColor = UITheme.TextMuted;
            
            // Botão Backup
            UITheme.FormatSaaSButton(btnBackup, true);
            
            // Borda arredondada do card Backup
            pnlBackupCard.Paint += (s, e) => UITheme.DrawRoundedPanel(e.Graphics, pnlBackupCard.ClientRectangle, UITheme.BgCard, 12, UITheme.ElementBorder);

            // Estilo do Card Usuarios
            pnlUsuariosCard.BackColor = UITheme.BgApp;
            lblUsuariosIcon.ForeColor = UITheme.TextTitle;
            lblUsuariosTitle.ForeColor = UITheme.TextTitle;
            lblUsuariosDesc.ForeColor = UITheme.TextMuted;
            
            // Botão Gerenciar Usuários
            UITheme.FormatSaaSButton(btnGerenciarUsuarios, true);
            
            // Borda arredondada do card Usuarios
            pnlUsuariosCard.Paint += (s, e) => UITheme.DrawRoundedPanel(e.Graphics, pnlUsuariosCard.ClientRectangle, UITheme.BgCard, 12, UITheme.ElementBorder);

            // Estilo do Card Reset
            pnlResetCard.BackColor = UITheme.BgApp;
            lblResetIcon.ForeColor = UITheme.TextTitle;
            lblResetTitle.ForeColor = UITheme.TextTitle;
            lblResetDesc.ForeColor = UITheme.TextMuted;

            // Botões de Reset (Estilo Danger/Atenção)
            FormatDangerButton(btnResetProdutos);
            FormatDangerButton(btnResetClientes);
            FormatDangerButton(btnResetFinanceiro);
            FormatDangerButton(btnResetRelatorios);

            // Borda arredondada do card Reset
            pnlResetCard.Paint += (s, e) => UITheme.DrawRoundedPanel(e.Graphics, pnlResetCard.ClientRectangle, UITheme.BgCard, 12, UITheme.ElementBorder);

            // Estilo do Card WhatsApp
            pnlWhatsAppCard.BackColor = UITheme.BgApp;
            lblWhatsAppIcon.ForeColor = UITheme.TextTitle;
            lblWhatsAppTitle.ForeColor = UITheme.TextTitle;
            lblWhatsAppDesc.ForeColor = UITheme.TextMuted;
            
            // Botão Configurar WhatsApp
            UITheme.FormatSaaSButton(btnWhatsAppConfig, true);
            
            // Borda arredondada do card WhatsApp
            pnlWhatsAppCard.Paint += (s, e) => UITheme.DrawRoundedPanel(e.Graphics, pnlWhatsAppCard.ClientRectangle, UITheme.BgCard, 12, UITheme.ElementBorder);

            // Estilo do Card Termos
            pnlTermosCard.BackColor = UITheme.BgApp;
            lblTermosIcon.ForeColor = UITheme.TextTitle;
            lblTermosTitle.ForeColor = UITheme.TextTitle;
            lblTermosDesc.ForeColor = UITheme.TextMuted;
            
            // Botão Configurar Termos
            UITheme.FormatSaaSButton(btnTermosConfig, true);
            
            // Borda arredondada do card Termos
            pnlTermosCard.Paint += (s, e) => UITheme.DrawRoundedPanel(e.Graphics, pnlTermosCard.ClientRectangle, UITheme.BgCard, 12, UITheme.ElementBorder);
        }

        private void FormatDangerButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = UITheme.Danger;
            btn.ForeColor = Color.White;
            btn.Font = UITheme.FontBodyBold;
            btn.Cursor = Cursors.Hand;
        }

        private void BindEvents()
        {
            btnResetProdutos.Click += BtnResetProdutos_Click;
            btnResetClientes.Click += BtnResetClientes_Click;
            btnResetFinanceiro.Click += BtnResetFinanceiro_Click;
            btnResetRelatorios.Click += BtnResetRelatorios_Click;
            btnGerenciarUsuarios.Click += BtnGerenciarUsuarios_Click;
            btnWhatsAppConfig.Click += BtnWhatsAppConfig_Click;
            btnTermosConfig.Click += BtnTermosConfig_Click;
        }

        private void BtnWhatsAppConfig_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmConfiguracaoWhatsApp())
            {
                frm.ShowDialog();
            }
        }

        private void BtnTermosConfig_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmConfiguracaoTermos())
            {
                frm.ShowDialog();
            }
        }

        private void BtnGerenciarUsuarios_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmUsuarios())
            {
                frm.ShowDialog();
            }
        }

        private void BtnBackup_Click(object sender, EventArgs e)
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

        private void BtnResetProdutos_Click(object sender, EventArgs e)
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

        private void BtnResetClientes_Click(object sender, EventArgs e)
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

        private void BtnResetFinanceiro_Click(object sender, EventArgs e)
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

        private void BtnResetRelatorios_Click(object sender, EventArgs e)
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
