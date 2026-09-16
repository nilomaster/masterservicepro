namespace MasterServicePro.Forms
{
    partial class FrmPDV
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
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tlpContent = new System.Windows.Forms.TableLayoutPanel();
            this.pnlCart = new System.Windows.Forms.Panel();
            this.dgvCarrinho = new System.Windows.Forms.DataGridView();
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.lblUltimoPreco = new System.Windows.Forms.Label();
            this.lblUltimoNome = new System.Windows.Forms.Label();
            this.txtBusca = new System.Windows.Forms.TextBox();
            this.lblBusca = new System.Windows.Forms.Label();
            this.pnlSummary = new System.Windows.Forms.Panel();
            this.lblTotalLabel = new System.Windows.Forms.Label();
            this.lblTotalValue = new System.Windows.Forms.Label();
            this.btnFinalizar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnRemover = new System.Windows.Forms.Button();
            this.tlpMain.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.tlpContent.SuspendLayout();
            this.pnlCart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCarrinho)).BeginInit();
            this.pnlSearch.SuspendLayout();
            this.pnlSummary.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.pnlHeader, 0, 0);
            this.tlpMain.Controls.Add(this.tlpContent, 0, 1);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 2;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Size = new System.Drawing.Size(1000, 650);
            this.tlpMain.TabIndex = 0;
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1000, 60);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.lblTitle.Size = new System.Drawing.Size(400, 60);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Ponto de Venda (PDV)";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tlpContent
            // 
            this.tlpContent.ColumnCount = 2;
            this.tlpContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tlpContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tlpContent.Controls.Add(this.pnlCart, 0, 0);
            this.tlpContent.Controls.Add(this.pnlSummary, 1, 0);
            this.tlpContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpContent.Location = new System.Drawing.Point(3, 63);
            this.tlpContent.Name = "tlpContent";
            this.tlpContent.RowCount = 1;
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpContent.Size = new System.Drawing.Size(994, 584);
            this.tlpContent.TabIndex = 1;
            // 
            // pnlCart
            // 
            this.pnlCart.Controls.Add(this.dgvCarrinho);
            this.pnlCart.Controls.Add(this.pnlSearch);
            this.pnlCart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCart.Location = new System.Drawing.Point(0, 0);
            this.pnlCart.Margin = new System.Windows.Forms.Padding(0);
            this.pnlCart.Name = "pnlCart";
            this.pnlCart.Padding = new System.Windows.Forms.Padding(10);
            this.pnlCart.Size = new System.Drawing.Size(695, 584);
            this.pnlCart.TabIndex = 0;
            // 
            // dgvCarrinho
            // 
            this.dgvCarrinho.AllowUserToAddRows = false;
            this.dgvCarrinho.AllowUserToDeleteRows = false;
            this.dgvCarrinho.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCarrinho.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCarrinho.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCarrinho.Location = new System.Drawing.Point(10, 170);
            this.dgvCarrinho.Name = "dgvCarrinho";
            this.dgvCarrinho.ReadOnly = true;
            this.dgvCarrinho.RowHeadersVisible = false;
            this.dgvCarrinho.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCarrinho.Size = new System.Drawing.Size(675, 404);
            this.dgvCarrinho.TabIndex = 1;
            // 
            // pnlSearch
            // 
            this.pnlSearch.Controls.Add(this.lblUltimoPreco);
            this.pnlSearch.Controls.Add(this.lblUltimoNome);
            this.pnlSearch.Controls.Add(this.txtBusca);
            this.pnlSearch.Controls.Add(this.lblBusca);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSearch.Location = new System.Drawing.Point(10, 10);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Size = new System.Drawing.Size(675, 160);
            this.pnlSearch.TabIndex = 0;
            // 
            // lblUltimoNome
            // 
            this.lblUltimoNome.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUltimoNome.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblUltimoNome.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.lblUltimoNome.Location = new System.Drawing.Point(4, 80);
            this.lblUltimoNome.Name = "lblUltimoNome";
            this.lblUltimoNome.Size = new System.Drawing.Size(460, 70);
            this.lblUltimoNome.TabIndex = 2;
            this.lblUltimoNome.Text = "AGUARDANDO ITEM...";
            this.lblUltimoNome.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblUltimoPreco
            // 
            this.lblUltimoPreco.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUltimoPreco.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblUltimoPreco.ForeColor = System.Drawing.Color.White;
            this.lblUltimoPreco.Location = new System.Drawing.Point(470, 80);
            this.lblUltimoPreco.Name = "lblUltimoPreco";
            this.lblUltimoPreco.Size = new System.Drawing.Size(200, 70);
            this.lblUltimoPreco.TabIndex = 3;
            this.lblUltimoPreco.Text = "R$ 0,00";
            this.lblUltimoPreco.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtBusca
            // 
            this.txtBusca.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBusca.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtBusca.Location = new System.Drawing.Point(4, 28);
            this.txtBusca.Name = "txtBusca";
            this.txtBusca.Size = new System.Drawing.Size(668, 39);
            this.txtBusca.TabIndex = 1;
            // 
            // lblBusca
            // 
            this.lblBusca.AutoSize = true;
            this.lblBusca.Location = new System.Drawing.Point(4, 8);
            this.lblBusca.Name = "lblBusca";
            this.lblBusca.Size = new System.Drawing.Size(188, 15);
            this.lblBusca.TabIndex = 0;
            this.lblBusca.Text = "Código de Barras ou F2 para busca";
            // 
            // pnlSummary
            // 
            this.pnlSummary.Controls.Add(this.btnRemover);
            this.pnlSummary.Controls.Add(this.btnCancelar);
            this.pnlSummary.Controls.Add(this.btnFinalizar);
            this.pnlSummary.Controls.Add(this.lblTotalValue);
            this.pnlSummary.Controls.Add(this.lblTotalLabel);
            this.pnlSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSummary.Location = new System.Drawing.Point(695, 0);
            this.pnlSummary.Margin = new System.Windows.Forms.Padding(0);
            this.pnlSummary.Name = "pnlSummary";
            this.pnlSummary.Padding = new System.Windows.Forms.Padding(20);
            this.pnlSummary.Size = new System.Drawing.Size(299, 584);
            this.pnlSummary.TabIndex = 1;
            // 
            // lblTotalLabel
            // 
            this.lblTotalLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTotalLabel.Location = new System.Drawing.Point(20, 20);
            this.lblTotalLabel.Name = "lblTotalLabel";
            this.lblTotalLabel.Size = new System.Drawing.Size(259, 30);
            this.lblTotalLabel.TabIndex = 0;
            this.lblTotalLabel.Text = "TOTAL DA VENDA";
            this.lblTotalLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalValue
            // 
            this.lblTotalValue.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTotalValue.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTotalValue.Location = new System.Drawing.Point(20, 50);
            this.lblTotalValue.Name = "lblTotalValue";
            this.lblTotalValue.Size = new System.Drawing.Size(259, 80);
            this.lblTotalValue.TabIndex = 1;
            this.lblTotalValue.Text = "R$ 0,00";
            this.lblTotalValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnFinalizar
            // 
            this.btnFinalizar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnFinalizar.Location = new System.Drawing.Point(20, 514);
            this.btnFinalizar.Name = "btnFinalizar";
            this.btnFinalizar.Size = new System.Drawing.Size(259, 50);
            this.btnFinalizar.TabIndex = 2;
            this.btnFinalizar.Text = "FINALIZAR VENDA (F5)";
            this.btnFinalizar.UseVisualStyleBackColor = true;
            // 
            // btnCancelar
            // 
            this.btnCancelar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnCancelar.Location = new System.Drawing.Point(20, 464);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(259, 50);
            this.btnCancelar.TabIndex = 3;
            this.btnCancelar.Text = "CANCELAR VENDA";
            this.btnCancelar.UseVisualStyleBackColor = true;
            // 
            // btnRemover
            // 
            this.btnRemover.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnRemover.Location = new System.Drawing.Point(20, 414);
            this.btnRemover.Name = "btnRemover";
            this.btnRemover.Size = new System.Drawing.Size(259, 50);
            this.btnRemover.TabIndex = 4;
            this.btnRemover.Text = "REMOVER ITEM (DEL)";
            this.btnRemover.UseVisualStyleBackColor = true;
            // 
            // FrmPDV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 650);
            this.Controls.Add(this.tlpMain);
            this.Name = "FrmPDV";
            this.Text = "PDV";
            this.tlpMain.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.tlpContent.ResumeLayout(false);
            this.pnlCart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCarrinho)).EndInit();
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.pnlSummary.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TableLayoutPanel tlpContent;
        private System.Windows.Forms.Panel pnlCart;
        private System.Windows.Forms.DataGridView dgvCarrinho;
        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.TextBox txtBusca;
        private System.Windows.Forms.Label lblBusca;
        private System.Windows.Forms.Panel pnlSummary;
        private System.Windows.Forms.Label lblTotalLabel;
        private System.Windows.Forms.Label lblTotalValue;
        private System.Windows.Forms.Button btnFinalizar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnRemover;
        private System.Windows.Forms.Label lblUltimoNome;
        private System.Windows.Forms.Label lblUltimoPreco;
    }
}
