namespace MasterServicePro.Forms
{
    partial class FrmPDVSucesso
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
            this.lblCheck = new System.Windows.Forms.Label();
            this.lblSucesso = new System.Windows.Forms.Label();
            this.lblTotalLabel = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblPagamentoLabel = new System.Windows.Forms.Label();
            this.lblPagamento = new System.Windows.Forms.Label();
            this.lblTrocoLabel = new System.Windows.Forms.Label();
            this.lblTroco = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.pnlCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCard
            // 
            this.pnlCard.Controls.Add(this.btnOk);
            this.pnlCard.Controls.Add(this.lblTroco);
            this.pnlCard.Controls.Add(this.lblTrocoLabel);
            this.pnlCard.Controls.Add(this.lblPagamento);
            this.pnlCard.Controls.Add(this.lblPagamentoLabel);
            this.pnlCard.Controls.Add(this.lblTotal);
            this.pnlCard.Controls.Add(this.lblTotalLabel);
            this.pnlCard.Controls.Add(this.lblSucesso);
            this.pnlCard.Controls.Add(this.lblCheck);
            this.pnlCard.Location = new System.Drawing.Point(30, 30);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(440, 440);
            this.pnlCard.TabIndex = 0;
            // 
            // lblCheck
            // 
            this.lblCheck.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCheck.Font = new System.Drawing.Font("Segoe UI", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCheck.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblCheck.Location = new System.Drawing.Point(0, 0);
            this.lblCheck.Name = "lblCheck";
            this.lblCheck.Size = new System.Drawing.Size(440, 100);
            this.lblCheck.TabIndex = 0;
            this.lblCheck.Text = "✓";
            this.lblCheck.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSucesso
            // 
            this.lblSucesso.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSucesso.Location = new System.Drawing.Point(0, 100);
            this.lblSucesso.Name = "lblSucesso";
            this.lblSucesso.Size = new System.Drawing.Size(440, 60);
            this.lblSucesso.TabIndex = 1;
            this.lblSucesso.Text = "VENDA FINALIZADA!";
            this.lblSucesso.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalLabel
            // 
            this.lblTotalLabel.AutoSize = true;
            this.lblTotalLabel.Location = new System.Drawing.Point(50, 180);
            this.lblTotalLabel.Name = "lblTotalLabel";
            this.lblTotalLabel.Size = new System.Drawing.Size(73, 15);
            this.lblTotalLabel.TabIndex = 2;
            this.lblTotalLabel.Text = "VALOR TOTAL";
            // 
            // lblTotal
            // 
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTotal.Location = new System.Drawing.Point(50, 200);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(340, 35);
            this.lblTotal.TabIndex = 3;
            this.lblTotal.Text = "R$ 0,00";
            // 
            // lblPagamentoLabel
            // 
            this.lblPagamentoLabel.AutoSize = true;
            this.lblPagamentoLabel.Location = new System.Drawing.Point(50, 250);
            this.lblPagamentoLabel.Name = "lblPagamentoLabel";
            this.lblPagamentoLabel.Size = new System.Drawing.Size(124, 15);
            this.lblPagamentoLabel.TabIndex = 4;
            this.lblPagamentoLabel.Text = "FORMA PAGAMENTO";
            // 
            // lblPagamento
            // 
            this.lblPagamento.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblPagamento.Location = new System.Drawing.Point(50, 270);
            this.lblPagamento.Name = "lblPagamento";
            this.lblPagamento.Size = new System.Drawing.Size(340, 30);
            this.lblPagamento.TabIndex = 5;
            this.lblPagamento.Text = "Dinheiro";
            // 
            // lblTrocoLabel
            // 
            this.lblTrocoLabel.AutoSize = true;
            this.lblTrocoLabel.Location = new System.Drawing.Point(50, 320);
            this.lblTrocoLabel.Name = "lblTrocoLabel";
            this.lblTrocoLabel.Size = new System.Drawing.Size(117, 15);
            this.lblTrocoLabel.TabIndex = 6;
            this.lblTrocoLabel.Text = "TROCO AO CLIENTE";
            // 
            // lblTroco
            // 
            this.lblTroco.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTroco.Location = new System.Drawing.Point(50, 340);
            this.lblTroco.Name = "lblTroco";
            this.lblTroco.Size = new System.Drawing.Size(340, 45);
            this.lblTroco.TabIndex = 7;
            this.lblTroco.Text = "R$ 0,00";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(50, 390);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(340, 45);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "NOVA VENDA (ENTER)";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // FrmPDVSucesso
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 500);
            this.Controls.Add(this.pnlCard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmPDVSucesso";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Venda Finalizada";
            this.pnlCard.ResumeLayout(false);
            this.pnlCard.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlCard;
        private System.Windows.Forms.Label lblCheck;
        private System.Windows.Forms.Label lblSucesso;
        private System.Windows.Forms.Label lblTotalLabel;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblPagamentoLabel;
        private System.Windows.Forms.Label lblPagamento;
        private System.Windows.Forms.Label lblTrocoLabel;
        private System.Windows.Forms.Label lblTroco;
        private System.Windows.Forms.Button btnOk;
    }
}
