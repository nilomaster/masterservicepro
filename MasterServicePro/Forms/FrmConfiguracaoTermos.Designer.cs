namespace MasterServicePro.Forms
{
    partial class FrmConfiguracaoTermos
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
            this.pnlMain = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblDesc = new System.Windows.Forms.Label();
            this.txtTermos = new System.Windows.Forms.TextBox();
            this.btnSalvar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.btnCancelar);
            this.pnlMain.Controls.Add(this.btnSalvar);
            this.pnlMain.Controls.Add(this.txtTermos);
            this.pnlMain.Controls.Add(this.lblDesc);
            this.pnlMain.Controls.Add(this.lblTitle);
            this.pnlMain.Location = new System.Drawing.Point(20, 20);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(600, 440);
            this.pnlMain.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(225, 15);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "TERMOS E CONDIÇÕES DE IMPRESSÃO";
            // 
            // lblDesc
            // 
            this.lblDesc.Location = new System.Drawing.Point(20, 50);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(560, 40);
            this.lblDesc.TabIndex = 1;
            this.lblDesc.Text = "Estes termos serão impressos no rodapé dos comprovantes de Ordem de Serviço e Orçamentos Rápidos entregues aos clientes.";
            // 
            // txtTermos
            // 
            this.txtTermos.Location = new System.Drawing.Point(20, 100);
            this.txtTermos.Multiline = true;
            this.txtTermos.Name = "txtTermos";
            this.txtTermos.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTermos.Size = new System.Drawing.Size(560, 240);
            this.txtTermos.TabIndex = 2;
            // 
            // btnSalvar
            // 
            this.btnSalvar.Location = new System.Drawing.Point(380, 370);
            this.btnSalvar.Name = "btnSalvar";
            this.btnSalvar.Size = new System.Drawing.Size(200, 45);
            this.btnSalvar.TabIndex = 3;
            this.btnSalvar.Text = "SALVAR TERMOS";
            this.btnSalvar.UseVisualStyleBackColor = true;
            this.btnSalvar.Click += new System.EventHandler(this.BtnSalvar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Location = new System.Drawing.Point(20, 370);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(200, 45);
            this.btnCancelar.TabIndex = 4;
            this.btnCancelar.Text = "CANCELAR";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += (s, e) => this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            // 
            // FrmConfiguracaoTermos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 480);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmConfiguracaoTermos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configurar Termos de Impressão";
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDesc;
        private System.Windows.Forms.TextBox txtTermos;
        private System.Windows.Forms.Button btnSalvar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
