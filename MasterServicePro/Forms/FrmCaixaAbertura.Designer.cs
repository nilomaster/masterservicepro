namespace MasterServicePro.Forms
{
    partial class FrmCaixaAbertura
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblValor = new System.Windows.Forms.Label();
            this.numValor = new System.Windows.Forms.NumericUpDown();
            this.btnAbrir = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.pnlCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numValor)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlCard
            // 
            this.pnlCard.Controls.Add(this.btnCancelar);
            this.pnlCard.Controls.Add(this.btnAbrir);
            this.pnlCard.Controls.Add(this.numValor);
            this.pnlCard.Controls.Add(this.lblValor);
            this.pnlCard.Controls.Add(this.lblTitle);
            this.pnlCard.Location = new System.Drawing.Point(20, 20);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(360, 260);
            this.pnlCard.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(360, 60);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "ABERTURA DE CAIXA";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblValor
            // 
            this.lblValor.AutoSize = true;
            this.lblValor.Location = new System.Drawing.Point(30, 80);
            this.lblValor.Name = "lblValor";
            this.lblValor.Size = new System.Drawing.Size(126, 15);
            this.lblValor.TabIndex = 1;
            this.lblValor.Text = "Valor Inicial em Caixa:";
            // 
            // numValor
            // 
            this.numValor.DecimalPlaces = 2;
            this.numValor.Location = new System.Drawing.Point(30, 100);
            this.numValor.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numValor.Name = "numValor";
            this.numValor.Size = new System.Drawing.Size(300, 23);
            this.numValor.TabIndex = 2;
            // 
            // btnAbrir
            // 
            this.btnAbrir.Location = new System.Drawing.Point(30, 150);
            this.btnAbrir.Name = "btnAbrir";
            this.btnAbrir.Size = new System.Drawing.Size(300, 45);
            this.btnAbrir.TabIndex = 3;
            this.btnAbrir.Text = "ABRIR CAIXA";
            this.btnAbrir.UseVisualStyleBackColor = true;
            // 
            // btnCancelar
            // 
            this.btnCancelar.Location = new System.Drawing.Point(30, 205);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(300, 35);
            this.btnCancelar.TabIndex = 4;
            this.btnCancelar.Text = "CANCELAR";
            this.btnCancelar.UseVisualStyleBackColor = true;
            // 
            // FrmCaixaAbertura
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.Controls.Add(this.pnlCard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmCaixaAbertura";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Abertura de Caixa";
            this.pnlCard.ResumeLayout(false);
            this.pnlCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numValor)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlCard;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblValor;
        private System.Windows.Forms.NumericUpDown numValor;
        private System.Windows.Forms.Button btnAbrir;
        private System.Windows.Forms.Button btnCancelar;
    }
}
