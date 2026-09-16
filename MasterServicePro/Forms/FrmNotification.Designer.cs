namespace MasterServicePro.Forms
{
    partial class FrmNotification
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
            this.pnlCard = new System.Windows.Forms.Panel();
            this.btnNo = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.lblMensagem = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblIcone = new System.Windows.Forms.Label();
            this.pnlCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCard
            // 
            this.pnlCard.Controls.Add(this.btnNo);
            this.pnlCard.Controls.Add(this.btnOk);
            this.pnlCard.Controls.Add(this.lblMensagem);
            this.pnlCard.Controls.Add(this.lblTitulo);
            this.pnlCard.Controls.Add(this.lblIcone);
            this.pnlCard.Location = new System.Drawing.Point(20, 20);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(460, 220);
            this.pnlCard.TabIndex = 0;
            // 
            // btnNo
            // 
            this.btnNo.Location = new System.Drawing.Point(40, 160);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(180, 42);
            this.btnNo.TabIndex = 4;
            this.btnNo.Text = "NÃO (ESC)";
            this.btnNo.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(240, 160);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(180, 42);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "SIM (ENTER)";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // lblMensagem
            // 
            this.lblMensagem.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblMensagem.Location = new System.Drawing.Point(15, 85);
            this.lblMensagem.Name = "lblMensagem";
            this.lblMensagem.Size = new System.Drawing.Size(430, 65);
            this.lblMensagem.TabIndex = 2;
            this.lblMensagem.Text = "Mensagem do sistema";
            this.lblMensagem.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitulo
            // 
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitulo.Location = new System.Drawing.Point(15, 52);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(430, 30);
            this.lblTitulo.TabIndex = 1;
            this.lblTitulo.Text = "TÍTULO DA NOTIFICAÇÃO";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblIcone
            // 
            this.lblIcone.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblIcone.Location = new System.Drawing.Point(15, 3);
            this.lblIcone.Name = "lblIcone";
            this.lblIcone.Size = new System.Drawing.Size(430, 48);
            this.lblIcone.TabIndex = 0;
            this.lblIcone.Text = "✔";
            this.lblIcone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FrmNotification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 260);
            this.Controls.Add(this.pnlCard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmNotification";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Notificação";
            this.pnlCard.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlCard;
        private System.Windows.Forms.Label lblIcone;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblMensagem;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnNo;
    }
}
