namespace MasterServicePro.Forms
{
    partial class FrmProdutos
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
            this.btnNovo = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnExcluir = new System.Windows.Forms.Button();
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.txtBusca = new System.Windows.Forms.TextBox();
            this.lblBusca = new System.Windows.Forms.Label();
            this.dgvProdutos = new System.Windows.Forms.DataGridView();
            this.pnlCategorias = new System.Windows.Forms.Panel();
            this.lblCategoriasTitle = new System.Windows.Forms.Label();
            this.lstCategorias = new System.Windows.Forms.ListBox();
            this.tlpMain.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdutos)).BeginInit();
            this.pnlCategorias.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.pnlHeader, 0, 0);
            this.tlpMain.Controls.Add(this.pnlSearch, 0, 1);
            this.tlpMain.Controls.Add(this.dgvProdutos, 0, 2);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(220, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 3;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Size = new System.Drawing.Size(680, 600);
            this.tlpMain.TabIndex = 0;
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.btnExcluir);
            this.pnlHeader.Controls.Add(this.btnEditar);
            this.pnlHeader.Controls.Add(this.btnNovo);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(680, 60);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.lblTitle.Size = new System.Drawing.Size(300, 60);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Estoque: Produtos e Peças";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnNovo
            // 
            this.btnNovo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNovo.Location = new System.Drawing.Point(340, 15);
            this.btnNovo.Name = "btnNovo";
            this.btnNovo.Size = new System.Drawing.Size(100, 35);
            this.btnNovo.TabIndex = 1;
            this.btnNovo.Text = "Novo";
            this.btnNovo.UseVisualStyleBackColor = true;
            // 
            // btnEditar
            // 
            this.btnEditar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditar.Location = new System.Drawing.Point(450, 15);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(100, 35);
            this.btnEditar.TabIndex = 2;
            this.btnEditar.Text = "Editar";
            this.btnEditar.UseVisualStyleBackColor = true;
            // 
            // btnExcluir
            // 
            this.btnExcluir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExcluir.Location = new System.Drawing.Point(560, 15);
            this.btnExcluir.Name = "btnExcluir";
            this.btnExcluir.Size = new System.Drawing.Size(100, 35);
            this.btnExcluir.TabIndex = 3;
            this.btnExcluir.Text = "Excluir";
            this.btnExcluir.UseVisualStyleBackColor = true;
            // 
            // pnlSearch
            // 
            this.pnlSearch.Controls.Add(this.txtBusca);
            this.pnlSearch.Controls.Add(this.lblBusca);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSearch.Location = new System.Drawing.Point(0, 60);
            this.pnlSearch.Margin = new System.Windows.Forms.Padding(0);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Size = new System.Drawing.Size(680, 60);
            this.pnlSearch.TabIndex = 1;
            // 
            // lblBusca
            // 
            this.lblBusca.AutoSize = true;
            this.lblBusca.Location = new System.Drawing.Point(20, 22);
            this.lblBusca.Name = "lblBusca";
            this.lblBusca.Size = new System.Drawing.Size(43, 15);
            this.lblBusca.TabIndex = 0;
            this.lblBusca.Text = "Buscar:";
            // 
            // txtBusca
            // 
            this.txtBusca.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBusca.Location = new System.Drawing.Point(70, 19);
            this.txtBusca.Name = "txtBusca";
            this.txtBusca.Size = new System.Drawing.Size(590, 23);
            this.txtBusca.TabIndex = 1;
            // 
            // dgvProdutos
            // 
            this.dgvProdutos.AllowUserToAddRows = false;
            this.dgvProdutos.AllowUserToDeleteRows = false;
            this.dgvProdutos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProdutos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProdutos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProdutos.Location = new System.Drawing.Point(20, 120);
            this.dgvProdutos.Margin = new System.Windows.Forms.Padding(20, 0, 20, 20);
            this.dgvProdutos.MultiSelect = false;
            this.dgvProdutos.Name = "dgvProdutos";
            this.dgvProdutos.ReadOnly = true;
            this.dgvProdutos.RowTemplate.Height = 35;
            this.dgvProdutos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProdutos.Size = new System.Drawing.Size(640, 460);
            this.dgvProdutos.TabIndex = 2;
            // 
            // pnlCategorias
            // 
            this.pnlCategorias.Controls.Add(this.lstCategorias);
            this.pnlCategorias.Controls.Add(this.lblCategoriasTitle);
            this.pnlCategorias.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlCategorias.Location = new System.Drawing.Point(0, 0);
            this.pnlCategorias.Name = "pnlCategorias";
            this.pnlCategorias.Size = new System.Drawing.Size(220, 600);
            this.pnlCategorias.TabIndex = 3;
            // 
            // lblCategoriasTitle
            // 
            this.lblCategoriasTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCategoriasTitle.Location = new System.Drawing.Point(0, 0);
            this.lblCategoriasTitle.Name = "lblCategoriasTitle";
            this.lblCategoriasTitle.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.lblCategoriasTitle.Size = new System.Drawing.Size(220, 60);
            this.lblCategoriasTitle.TabIndex = 0;
            this.lblCategoriasTitle.Text = "CATEGORIAS";
            this.lblCategoriasTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lstCategorias
            // 
            this.lstCategorias.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstCategorias.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstCategorias.ItemHeight = 40;
            this.lstCategorias.Location = new System.Drawing.Point(0, 60);
            this.lstCategorias.Name = "lstCategorias";
            this.lstCategorias.Size = new System.Drawing.Size(220, 540);
            this.lstCategorias.TabIndex = 1;
            // 
            // FrmProdutos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.tlpMain);
            this.Controls.Add(this.pnlCategorias);
            this.Name = "FrmProdutos";
            this.Text = "Produtos";
            this.tlpMain.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdutos)).EndInit();
            this.pnlCategorias.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnNovo;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnExcluir;
        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.Label lblBusca;
        private System.Windows.Forms.TextBox txtBusca;
        private System.Windows.Forms.DataGridView dgvProdutos;
        private System.Windows.Forms.Panel pnlCategorias;
        private System.Windows.Forms.Label lblCategoriasTitle;
        private System.Windows.Forms.ListBox lstCategorias;
    }
}
