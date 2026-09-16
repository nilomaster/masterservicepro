namespace MasterServicePro.Forms
{
    partial class FrmPDVAlerta
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
            this.btnOk = new System.Windows.Forms.Button();
            this.lblMensagem = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblIcone = new System.Windows.Forms.Label();
            this.pnlCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCard
            // 
            this.pnlCard.Controls.Add(this.btnOk);
            this.pnlCard.Controls.Add(this.lblMensagem);
            this.pnlCard.Controls.Add(this.lblTitulo);
            this.pnlCard.Controls.Add(this.lblIcone);
            this.pnlCard.Location = new System.Drawing.Point(30, 30);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(440, 240);
            this.pnlCard.TabIndex = 0;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(120, 175);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(200, 45);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "ENTENDI (ENTER)";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // lblMensagem
            // 
            this.lblMensagem.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblMensagem.Location = new System.Drawing.Point(20, 105);
            this.lblMensagem.Name = "lblMensagem";
            this.lblMensagem.Size = new System.Drawing.Size(400, 60);
            this.lblMensagem.TabIndex = 2;
            this.lblMensagem.Text = "O caixa está fechado no momento. Abra o caixa no módulo financeiro para continuar" +
    ".";
            this.lblMensagem.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitulo
            // 
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitulo.Location = new System.Drawing.Point(20, 65);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(400, 35);
            this.lblTitulo.TabIndex = 1;
            this.lblTitulo.Text = "CAIXA FECHADO";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblIcone
            // 
            this.lblIcone.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblIcone.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.lblIcone.Location = new System.Drawing.Point(20, 5);
            this.lblIcone.Name = "lblIcone";
            this.lblIcone.Size = new System.Drawing.Size(400, 60);
            this.lblIcone.TabIndex = 0;
            this.lblIcone.Text = "⚠️";
            this.lblIcone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FrmPDVAlerta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 300);
            this.Controls.Add(this.pnlCard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmPDVAlerta";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Alerta";
            this.pnlCard.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlCard;
        private System.Windows.Forms.Label lblIcone;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblMensagem;
        private System.Windows.Forms.Button btnOk;
    }
}
