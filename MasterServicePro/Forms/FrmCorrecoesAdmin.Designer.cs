namespace MasterServicePro.Forms
{
    partial class FrmCorrecoesAdmin
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
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tpVendas = new System.Windows.Forms.TabPage();
            this.dgvVendas = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCorrigirVenda = new System.Windows.Forms.Button();
            this.tpOS = new System.Windows.Forms.TabPage();
            this.dgvOS = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnCorrigirOS = new System.Windows.Forms.Button();
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.cbNovaForma = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tcMain.SuspendLayout();
            this.tpVendas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVendas)).BeginInit();
            this.panel1.SuspendLayout();
            this.tpOS.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOS)).BeginInit();
            this.panel2.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tpVendas);
            this.tcMain.Controls.Add(this.tpOS);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Location = new System.Drawing.Point(0, 60);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(800, 390);
            this.tcMain.TabIndex = 0;
            // 
            // tpVendas
            // 
            this.tpVendas.Controls.Add(this.dgvVendas);
            this.tpVendas.Controls.Add(this.panel1);
            this.tpVendas.Location = new System.Drawing.Point(4, 24);
            this.tpVendas.Name = "tpVendas";
            this.tpVendas.Padding = new System.Windows.Forms.Padding(3);
            this.tpVendas.Size = new System.Drawing.Size(792, 362);
            this.tpVendas.TabIndex = 0;
            this.tpVendas.Text = "Vendas (PDV)";
            this.tpVendas.UseVisualStyleBackColor = true;
            // 
            // dgvVendas
            // 
            this.dgvVendas.AllowUserToAddRows = false;
            this.dgvVendas.AllowUserToDeleteRows = false;
            this.dgvVendas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvVendas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVendas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvVendas.Location = new System.Drawing.Point(3, 3);
            this.dgvVendas.Name = "dgvVendas";
            this.dgvVendas.ReadOnly = true;
            this.dgvVendas.RowHeadersVisible = false;
            this.dgvVendas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvVendas.Size = new System.Drawing.Size(786, 306);
            this.dgvVendas.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCorrigirVenda);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 309);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(786, 50);
            this.panel1.TabIndex = 1;
            // 
            // btnCorrigirVenda
            // 
            this.btnCorrigirVenda.Location = new System.Drawing.Point(5, 10);
            this.btnCorrigirVenda.Name = "btnCorrigirVenda";
            this.btnCorrigirVenda.Size = new System.Drawing.Size(200, 30);
            this.btnCorrigirVenda.TabIndex = 0;
            this.btnCorrigirVenda.Text = "Aplicar Nova Forma de Pag.";
            this.btnCorrigirVenda.UseVisualStyleBackColor = true;
            this.btnCorrigirVenda.Click += new System.EventHandler(this.btnCorrigirVenda_Click);
            // 
            // tpOS
            // 
            this.tpOS.Controls.Add(this.dgvOS);
            this.tpOS.Controls.Add(this.panel2);
            this.tpOS.Location = new System.Drawing.Point(4, 24);
            this.tpOS.Name = "tpOS";
            this.tpOS.Padding = new System.Windows.Forms.Padding(3);
            this.tpOS.Size = new System.Drawing.Size(792, 362);
            this.tpOS.TabIndex = 1;
            this.tpOS.Text = "Ordens de Serviço";
            this.tpOS.UseVisualStyleBackColor = true;
            // 
            // dgvOS
            // 
            this.dgvOS.AllowUserToAddRows = false;
            this.dgvOS.AllowUserToDeleteRows = false;
            this.dgvOS.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvOS.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvOS.Location = new System.Drawing.Point(3, 3);
            this.dgvOS.Name = "dgvOS";
            this.dgvOS.ReadOnly = true;
            this.dgvOS.RowHeadersVisible = false;
            this.dgvOS.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvOS.Size = new System.Drawing.Size(786, 306);
            this.dgvOS.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnCorrigirOS);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(3, 309);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(786, 50);
            this.panel2.TabIndex = 2;
            // 
            // btnCorrigirOS
            // 
            this.btnCorrigirOS.Location = new System.Drawing.Point(5, 10);
            this.btnCorrigirOS.Name = "btnCorrigirOS";
            this.btnCorrigirOS.Size = new System.Drawing.Size(200, 30);
            this.btnCorrigirOS.TabIndex = 1;
            this.btnCorrigirOS.Text = "Aplicar Nova Forma de Pag.";
            this.btnCorrigirOS.UseVisualStyleBackColor = true;
            this.btnCorrigirOS.Click += new System.EventHandler(this.btnCorrigirOS_Click);
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Controls.Add(this.cbNovaForma);
            this.panelTop.Controls.Add(this.lblTitle);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(800, 60);
            this.panelTop.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.Location = new System.Drawing.Point(12, 18);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(232, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Correções (Admin/Caixa)";
            // 
            // cbNovaForma
            // 
            this.cbNovaForma.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbNovaForma.FormattingEnabled = true;
            this.cbNovaForma.Location = new System.Drawing.Point(565, 20);
            this.cbNovaForma.Name = "cbNovaForma";
            this.cbNovaForma.Size = new System.Drawing.Size(200, 23);
            this.cbNovaForma.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(400, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(159, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Nova Forma de Pagamento:";
            // 
            // FrmCorrecoesAdmin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tcMain);
            this.Controls.Add(this.panelTop);
            this.Name = "FrmCorrecoesAdmin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Correções";
            this.tcMain.ResumeLayout(false);
            this.tpVendas.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVendas)).EndInit();
            this.panel1.ResumeLayout(false);
            this.tpOS.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOS)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tpVendas;
        private System.Windows.Forms.DataGridView dgvVendas;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCorrigirVenda;
        private System.Windows.Forms.TabPage tpOS;
        private System.Windows.Forms.DataGridView dgvOS;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnCorrigirOS;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbNovaForma;
    }
}
