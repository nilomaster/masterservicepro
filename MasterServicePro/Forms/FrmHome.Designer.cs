namespace MasterServicePro.Forms
{
    partial class FrmHome
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
            this.pnlAlerts = new System.Windows.Forms.Panel();
            this.dgvAlertas = new System.Windows.Forms.DataGridView();
            this.lblAlertTitle = new System.Windows.Forms.Label();
            this.lblStatsTitle = new System.Windows.Forms.Label();
            this.lblVendasHoje = new System.Windows.Forms.Label();
            this.pnlAlerts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlertas)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlAlerts
            // 
            this.pnlAlerts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlAlerts.Controls.Add(this.dgvAlertas);
            this.pnlAlerts.Controls.Add(this.lblAlertTitle);
            this.pnlAlerts.Location = new System.Drawing.Point(30, 150);
            this.pnlAlerts.Name = "pnlAlerts";
            this.pnlAlerts.Padding = new System.Windows.Forms.Padding(15);
            this.pnlAlerts.Size = new System.Drawing.Size(500, 400);
            this.pnlAlerts.TabIndex = 0;
            // 
            // dgvAlertas
            // 
            this.dgvAlertas.AllowUserToAddRows = false;
            this.dgvAlertas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAlertas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAlertas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAlertas.Location = new System.Drawing.Point(15, 55);
            this.dgvAlertas.Name = "dgvAlertas";
            this.dgvAlertas.ReadOnly = true;
            this.dgvAlertas.RowHeadersVisible = false;
            this.dgvAlertas.Size = new System.Drawing.Size(470, 330);
            this.dgvAlertas.TabIndex = 1;
            // 
            // lblAlertTitle
            // 
            this.lblAlertTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAlertTitle.Location = new System.Drawing.Point(15, 15);
            this.lblAlertTitle.Name = "lblAlertTitle";
            this.lblAlertTitle.Size = new System.Drawing.Size(470, 40);
            this.lblAlertTitle.TabIndex = 0;
            this.lblAlertTitle.Text = "⚠️ ESTOQUE CRÍTICO (REPOR URGENTE)";
            this.lblAlertTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStatsTitle
            // 
            this.lblStatsTitle.AutoSize = true;
            this.lblStatsTitle.Location = new System.Drawing.Point(30, 30);
            this.lblStatsTitle.Name = "lblStatsTitle";
            this.lblStatsTitle.Size = new System.Drawing.Size(123, 15);
            this.lblStatsTitle.TabIndex = 1;
            this.lblStatsTitle.Text = "Vendas Realizadas Hoje";
            // 
            // lblVendasHoje
            // 
            this.lblVendasHoje.AutoSize = true;
            this.lblVendasHoje.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblVendasHoje.ForeColor = System.Drawing.Color.MediumSeaGreen;
            this.lblVendasHoje.Location = new System.Drawing.Point(30, 55);
            this.lblVendasHoje.Name = "lblVendasHoje";
            this.lblVendasHoje.Size = new System.Drawing.Size(130, 45);
            this.lblVendasHoje.TabIndex = 2;
            this.lblVendasHoje.Text = "R$ 0,00";
            // 
            // FrmHome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.lblVendasHoje);
            this.Controls.Add(this.lblStatsTitle);
            this.Controls.Add(this.pnlAlerts);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmHome";
            this.Text = "Resumo";
            this.pnlAlerts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlertas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Panel pnlAlerts;
        private System.Windows.Forms.DataGridView dgvAlertas;
        private System.Windows.Forms.Label lblAlertTitle;
        private System.Windows.Forms.Label lblStatsTitle;
        private System.Windows.Forms.Label lblVendasHoje;
    }
}
