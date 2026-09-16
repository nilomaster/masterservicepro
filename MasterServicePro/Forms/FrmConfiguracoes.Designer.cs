namespace MasterServicePro.Forms
{
    partial class FrmConfiguracoes
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnBackup = new System.Windows.Forms.Button();
            this.pnlBackupCard = new System.Windows.Forms.Panel();
            this.lblBackupTitle = new System.Windows.Forms.Label();
            this.lblBackupDesc = new System.Windows.Forms.Label();
            this.lblIcon = new System.Windows.Forms.Label();
            this.pnlUsuariosCard = new System.Windows.Forms.Panel();
            this.lblUsuariosIcon = new System.Windows.Forms.Label();
            this.lblUsuariosTitle = new System.Windows.Forms.Label();
            this.lblUsuariosDesc = new System.Windows.Forms.Label();
            this.btnGerenciarUsuarios = new System.Windows.Forms.Button();
            this.pnlWhatsAppCard = new System.Windows.Forms.Panel();
            this.lblWhatsAppIcon = new System.Windows.Forms.Label();
            this.lblWhatsAppTitle = new System.Windows.Forms.Label();
            this.lblWhatsAppDesc = new System.Windows.Forms.Label();
            this.btnWhatsAppConfig = new System.Windows.Forms.Button();
            this.pnlTermosCard = new System.Windows.Forms.Panel();
            this.lblTermosIcon = new System.Windows.Forms.Label();
            this.lblTermosTitle = new System.Windows.Forms.Label();
            this.lblTermosDesc = new System.Windows.Forms.Label();
            this.btnTermosConfig = new System.Windows.Forms.Button();
            this.pnlBackupCard.SuspendLayout();
            this.pnlUsuariosCard.SuspendLayout();
            this.pnlWhatsAppCard.SuspendLayout();
            this.pnlTermosCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.Location = new System.Drawing.Point(40, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(350, 37);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Configurações do Sistema";
            // 
            // pnlBackupCard
            // 
            this.pnlBackupCard.Controls.Add(this.lblIcon);
            this.pnlBackupCard.Controls.Add(this.lblBackupTitle);
            this.pnlBackupCard.Controls.Add(this.lblBackupDesc);
            this.pnlBackupCard.Controls.Add(this.btnBackup);
            this.pnlBackupCard.Location = new System.Drawing.Point(40, 100);
            this.pnlBackupCard.Name = "pnlBackupCard";
            this.pnlBackupCard.Size = new System.Drawing.Size(400, 150);
            this.pnlBackupCard.TabIndex = 2;
            // 
            // lblIcon
            // 
            this.lblIcon.AutoSize = true;
            this.lblIcon.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblIcon.Location = new System.Drawing.Point(20, 20);
            this.lblIcon.Name = "lblIcon";
            this.lblIcon.Size = new System.Drawing.Size(60, 50);
            this.lblIcon.TabIndex = 3;
            this.lblIcon.Text = "💾";
            // 
            // lblBackupTitle
            // 
            this.lblBackupTitle.AutoSize = true;
            this.lblBackupTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblBackupTitle.Location = new System.Drawing.Point(100, 20);
            this.lblBackupTitle.Name = "lblBackupTitle";
            this.lblBackupTitle.Size = new System.Drawing.Size(220, 25);
            this.lblBackupTitle.TabIndex = 4;
            this.lblBackupTitle.Text = "Backup de Segurança";
            // 
            // lblBackupDesc
            // 
            this.lblBackupDesc.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblBackupDesc.Location = new System.Drawing.Point(100, 50);
            this.lblBackupDesc.Name = "lblBackupDesc";
            this.lblBackupDesc.Size = new System.Drawing.Size(280, 40);
            this.lblBackupDesc.TabIndex = 5;
            this.lblBackupDesc.Text = "Crie uma cópia de segurança de todos os dados.";
            // 
            // btnBackup
            // 
            this.btnBackup.Location = new System.Drawing.Point(100, 95);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(180, 40);
            this.btnBackup.TabIndex = 1;
            this.btnBackup.Text = "Gerar Backup";
            this.btnBackup.UseVisualStyleBackColor = true;
            this.btnBackup.Click += new System.EventHandler(this.BtnBackup_Click);
            // 
            // pnlResetCard
            // 
            this.pnlResetCard = new System.Windows.Forms.Panel();
            this.lblResetIcon = new System.Windows.Forms.Label();
            this.lblResetTitle = new System.Windows.Forms.Label();
            this.lblResetDesc = new System.Windows.Forms.Label();
            this.btnResetProdutos = new System.Windows.Forms.Button();
            this.btnResetClientes = new System.Windows.Forms.Button();
            this.btnResetFinanceiro = new System.Windows.Forms.Button();
            this.btnResetRelatorios = new System.Windows.Forms.Button();
            this.pnlUsuariosCard = new System.Windows.Forms.Panel();
            this.pnlResetCard.SuspendLayout();
            this.pnlResetCard.Controls.Add(this.lblResetIcon);
            this.pnlResetCard.Controls.Add(this.lblResetTitle);
            this.pnlResetCard.Controls.Add(this.lblResetDesc);
            this.pnlResetCard.Controls.Add(this.btnResetProdutos);
            this.pnlResetCard.Controls.Add(this.btnResetClientes);
            this.pnlResetCard.Controls.Add(this.btnResetFinanceiro);
            this.pnlResetCard.Controls.Add(this.btnResetRelatorios);
            this.pnlResetCard.Location = new System.Drawing.Point(460, 100);
            this.pnlResetCard.Name = "pnlResetCard";
            this.pnlResetCard.Size = new System.Drawing.Size(400, 350);
            this.pnlResetCard.TabIndex = 3;
            // 
            // lblResetIcon
            // 
            this.lblResetIcon.AutoSize = true;
            this.lblResetIcon.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblResetIcon.Location = new System.Drawing.Point(20, 20);
            this.lblResetIcon.Name = "lblResetIcon";
            this.lblResetIcon.Size = new System.Drawing.Size(60, 50);
            this.lblResetIcon.TabIndex = 0;
            this.lblResetIcon.Text = "🗑️";
            // 
            // lblResetTitle
            // 
            this.lblResetTitle.AutoSize = true;
            this.lblResetTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblResetTitle.Location = new System.Drawing.Point(100, 20);
            this.lblResetTitle.Name = "lblResetTitle";
            this.lblResetTitle.Size = new System.Drawing.Size(220, 25);
            this.lblResetTitle.TabIndex = 1;
            this.lblResetTitle.Text = "Zerar Banco de Dados";
            // 
            // lblResetDesc
            // 
            this.lblResetDesc.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblResetDesc.Location = new System.Drawing.Point(100, 50);
            this.lblResetDesc.Name = "lblResetDesc";
            this.lblResetDesc.Size = new System.Drawing.Size(280, 40);
            this.lblResetDesc.TabIndex = 2;
            this.lblResetDesc.Text = "Exclua permanentemente os registros selecionados.";
            // 
            // btnResetProdutos
            // 
            this.btnResetProdutos.Location = new System.Drawing.Point(100, 100);
            this.btnResetProdutos.Name = "btnResetProdutos";
            this.btnResetProdutos.Size = new System.Drawing.Size(250, 45);
            this.btnResetProdutos.TabIndex = 3;
            this.btnResetProdutos.Text = "Zerar Produtos";
            this.btnResetProdutos.UseVisualStyleBackColor = true;
            // 
            // btnResetClientes
            // 
            this.btnResetClientes.Location = new System.Drawing.Point(100, 160);
            this.btnResetClientes.Name = "btnResetClientes";
            this.btnResetClientes.Size = new System.Drawing.Size(250, 45);
            this.btnResetClientes.TabIndex = 4;
            this.btnResetClientes.Text = "Zerar Clientes";
            this.btnResetClientes.UseVisualStyleBackColor = true;
            // 
            // btnResetFinanceiro
            // 
            this.btnResetFinanceiro.Location = new System.Drawing.Point(100, 220);
            this.btnResetFinanceiro.Name = "btnResetFinanceiro";
            this.btnResetFinanceiro.Size = new System.Drawing.Size(250, 45);
            this.btnResetFinanceiro.TabIndex = 5;
            this.btnResetFinanceiro.Text = "Zerar Financeiro";
            this.btnResetFinanceiro.UseVisualStyleBackColor = true;
            // 
            // btnResetRelatorios
            // 
            this.btnResetRelatorios.Location = new System.Drawing.Point(100, 280);
            this.btnResetRelatorios.Name = "btnResetRelatorios";
            this.btnResetRelatorios.Size = new System.Drawing.Size(250, 45);
            this.btnResetRelatorios.TabIndex = 6;
            this.btnResetRelatorios.Text = "Zerar Relatórios / Logs";
            this.btnResetRelatorios.UseVisualStyleBackColor = true;
            // 
            // pnlUsuariosCard
            // 
            this.pnlUsuariosCard.Controls.Add(this.lblUsuariosIcon);
            this.pnlUsuariosCard.Controls.Add(this.lblUsuariosTitle);
            this.pnlUsuariosCard.Controls.Add(this.lblUsuariosDesc);
            this.pnlUsuariosCard.Controls.Add(this.btnGerenciarUsuarios);
            this.pnlUsuariosCard.Location = new System.Drawing.Point(40, 270);
            this.pnlUsuariosCard.Name = "pnlUsuariosCard";
            this.pnlUsuariosCard.Size = new System.Drawing.Size(400, 150);
            this.pnlUsuariosCard.TabIndex = 6;
            // 
            // lblUsuariosIcon
            // 
            this.lblUsuariosIcon.AutoSize = true;
            this.lblUsuariosIcon.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblUsuariosIcon.Location = new System.Drawing.Point(20, 20);
            this.lblUsuariosIcon.Name = "lblUsuariosIcon";
            this.lblUsuariosIcon.Size = new System.Drawing.Size(60, 50);
            this.lblUsuariosIcon.TabIndex = 0;
            this.lblUsuariosIcon.Text = "👥";
            // 
            // lblUsuariosTitle
            // 
            this.lblUsuariosTitle.AutoSize = true;
            this.lblUsuariosTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblUsuariosTitle.Location = new System.Drawing.Point(100, 20);
            this.lblUsuariosTitle.Name = "lblUsuariosTitle";
            this.lblUsuariosTitle.Size = new System.Drawing.Size(220, 25);
            this.lblUsuariosTitle.TabIndex = 1;
            this.lblUsuariosTitle.Text = "Controle de Usuários";
            // 
            // lblUsuariosDesc
            // 
            this.lblUsuariosDesc.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblUsuariosDesc.Location = new System.Drawing.Point(100, 50);
            this.lblUsuariosDesc.Name = "lblUsuariosDesc";
            this.lblUsuariosDesc.Size = new System.Drawing.Size(280, 40);
            this.lblUsuariosDesc.TabIndex = 2;
            this.lblUsuariosDesc.Text = "Gerencie os funcionários e níveis de acesso do sistema.";
            // 
            // btnGerenciarUsuarios
            // 
            this.btnGerenciarUsuarios.Location = new System.Drawing.Point(100, 95);
            this.btnGerenciarUsuarios.Name = "btnGerenciarUsuarios";
            this.btnGerenciarUsuarios.Size = new System.Drawing.Size(180, 40);
            this.btnGerenciarUsuarios.TabIndex = 3;
            this.btnGerenciarUsuarios.Text = "Gerenciar Usuários";
            this.btnGerenciarUsuarios.UseVisualStyleBackColor = true;
            // 
            // pnlWhatsAppCard
            // 
            this.pnlWhatsAppCard.Controls.Add(this.lblWhatsAppIcon);
            this.pnlWhatsAppCard.Controls.Add(this.lblWhatsAppTitle);
            this.pnlWhatsAppCard.Controls.Add(this.lblWhatsAppDesc);
            this.pnlWhatsAppCard.Controls.Add(this.btnWhatsAppConfig);
            this.pnlWhatsAppCard.Location = new System.Drawing.Point(40, 440);
            this.pnlWhatsAppCard.Name = "pnlWhatsAppCard";
            this.pnlWhatsAppCard.Size = new System.Drawing.Size(400, 150);
            this.pnlWhatsAppCard.TabIndex = 7;
            // 
            // lblWhatsAppIcon
            // 
            this.lblWhatsAppIcon.AutoSize = true;
            this.lblWhatsAppIcon.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblWhatsAppIcon.Location = new System.Drawing.Point(20, 20);
            this.lblWhatsAppIcon.Name = "lblWhatsAppIcon";
            this.lblWhatsAppIcon.Size = new System.Drawing.Size(60, 50);
            this.lblWhatsAppIcon.TabIndex = 0;
            this.lblWhatsAppIcon.Text = "💬";
            // 
            // lblWhatsAppTitle
            // 
            this.lblWhatsAppTitle.AutoSize = true;
            this.lblWhatsAppTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblWhatsAppTitle.Location = new System.Drawing.Point(100, 20);
            this.lblWhatsAppTitle.Name = "lblWhatsAppTitle";
            this.lblWhatsAppTitle.Size = new System.Drawing.Size(220, 25);
            this.lblWhatsAppTitle.TabIndex = 1;
            this.lblWhatsAppTitle.Text = "Integração WhatsApp";
            // 
            // lblWhatsAppDesc
            // 
            this.lblWhatsAppDesc.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblWhatsAppDesc.Location = new System.Drawing.Point(100, 50);
            this.lblWhatsAppDesc.Name = "lblWhatsAppDesc";
            this.lblWhatsAppDesc.Size = new System.Drawing.Size(280, 40);
            this.lblWhatsAppDesc.TabIndex = 2;
            this.lblWhatsAppDesc.Text = "Configure o envio manual ou automático de mensagens e notificações.";
            // 
            // btnWhatsAppConfig
            // 
            this.btnWhatsAppConfig.Location = new System.Drawing.Point(100, 95);
            this.btnWhatsAppConfig.Name = "btnWhatsAppConfig";
            this.btnWhatsAppConfig.Size = new System.Drawing.Size(180, 40);
            this.btnWhatsAppConfig.TabIndex = 3;
            this.btnWhatsAppConfig.Text = "Configurar WhatsApp";
            this.btnWhatsAppConfig.UseVisualStyleBackColor = true;
            // 
            // pnlTermosCard
            // 
            this.pnlTermosCard.Controls.Add(this.lblTermosIcon);
            this.pnlTermosCard.Controls.Add(this.lblTermosTitle);
            this.pnlTermosCard.Controls.Add(this.lblTermosDesc);
            this.pnlTermosCard.Controls.Add(this.btnTermosConfig);
            this.pnlTermosCard.Location = new System.Drawing.Point(460, 440);
            this.pnlTermosCard.Name = "pnlTermosCard";
            this.pnlTermosCard.Size = new System.Drawing.Size(400, 150);
            this.pnlTermosCard.TabIndex = 8;
            // 
            // lblTermosIcon
            // 
            this.lblTermosIcon.AutoSize = true;
            this.lblTermosIcon.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblTermosIcon.Location = new System.Drawing.Point(20, 20);
            this.lblTermosIcon.Name = "lblTermosIcon";
            this.lblTermosIcon.Size = new System.Drawing.Size(60, 50);
            this.lblTermosIcon.TabIndex = 0;
            this.lblTermosIcon.Text = "📄";
            // 
            // lblTermosTitle
            // 
            this.lblTermosTitle.AutoSize = true;
            this.lblTermosTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTermosTitle.Location = new System.Drawing.Point(100, 20);
            this.lblTermosTitle.Name = "lblTermosTitle";
            this.lblTermosTitle.Size = new System.Drawing.Size(220, 25);
            this.lblTermosTitle.TabIndex = 1;
            this.lblTermosTitle.Text = "Termos da Impressão";
            // 
            // lblTermosDesc
            // 
            this.lblTermosDesc.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblTermosDesc.Location = new System.Drawing.Point(100, 50);
            this.lblTermosDesc.Name = "lblTermosDesc";
            this.lblTermosDesc.Size = new System.Drawing.Size(280, 40);
            this.lblTermosDesc.TabIndex = 2;
            this.lblTermosDesc.Text = "Personalize o rodapé dos comprovantes de Ordem de Serviço e Orçamentos.";
            // 
            // btnTermosConfig
            // 
            this.btnTermosConfig.Location = new System.Drawing.Point(100, 95);
            this.btnTermosConfig.Name = "btnTermosConfig";
            this.btnTermosConfig.Size = new System.Drawing.Size(180, 40);
            this.btnTermosConfig.TabIndex = 3;
            this.btnTermosConfig.Text = "Configurar Termos";
            this.btnTermosConfig.UseVisualStyleBackColor = true;
            // 
            // FrmConfiguracoes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 620);
            this.Controls.Add(this.pnlBackupCard);
            this.Controls.Add(this.pnlResetCard);
            this.Controls.Add(this.pnlUsuariosCard);
            this.Controls.Add(this.pnlWhatsAppCard);
            this.Controls.Add(this.pnlTermosCard);
            this.Controls.Add(this.lblTitle);
            this.Name = "FrmConfiguracoes";
            this.Text = "Configurações";
            this.pnlBackupCard.ResumeLayout(false);
            this.pnlBackupCard.PerformLayout();
            this.pnlResetCard.ResumeLayout(false);
            this.pnlResetCard.PerformLayout();
            this.pnlUsuariosCard.ResumeLayout(false);
            this.pnlUsuariosCard.PerformLayout();
            this.pnlWhatsAppCard.ResumeLayout(false);
            this.pnlWhatsAppCard.PerformLayout();
            this.pnlTermosCard.ResumeLayout(false);
            this.pnlTermosCard.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.Panel pnlBackupCard;
        private System.Windows.Forms.Label lblBackupTitle;
        private System.Windows.Forms.Label lblBackupDesc;
        private System.Windows.Forms.Label lblIcon;
        private System.Windows.Forms.Panel pnlResetCard;
        private System.Windows.Forms.Label lblResetIcon;
        private System.Windows.Forms.Label lblResetTitle;
        private System.Windows.Forms.Label lblResetDesc;
        private System.Windows.Forms.Button btnResetProdutos;
        private System.Windows.Forms.Button btnResetClientes;
        private System.Windows.Forms.Button btnResetFinanceiro;
        private System.Windows.Forms.Button btnResetRelatorios;
        private System.Windows.Forms.Panel pnlUsuariosCard;
        private System.Windows.Forms.Label lblUsuariosIcon;
        private System.Windows.Forms.Label lblUsuariosTitle;
        private System.Windows.Forms.Label lblUsuariosDesc;
        public System.Windows.Forms.Button btnGerenciarUsuarios;
        private System.Windows.Forms.Panel pnlWhatsAppCard;
        private System.Windows.Forms.Label lblWhatsAppIcon;
        private System.Windows.Forms.Label lblWhatsAppTitle;
        private System.Windows.Forms.Label lblWhatsAppDesc;
        private System.Windows.Forms.Button btnWhatsAppConfig;
        private System.Windows.Forms.Panel pnlTermosCard;
        private System.Windows.Forms.Label lblTermosIcon;
        private System.Windows.Forms.Label lblTermosTitle;
        private System.Windows.Forms.Label lblTermosDesc;
        private System.Windows.Forms.Button btnTermosConfig;
    }
}
