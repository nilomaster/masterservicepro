namespace MasterServicePro.Forms
{
    partial class FrmOS
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
            this.btnImprimir = new System.Windows.Forms.Button();
            this.btnExcluir = new System.Windows.Forms.Button();
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.txtBusca = new System.Windows.Forms.TextBox();
            this.lblBusca = new System.Windows.Forms.Label();
            this.dgvOS = new System.Windows.Forms.DataGridView();
            this.flpFiltros = new System.Windows.Forms.FlowLayoutPanel();
            this.rdoTodas = new System.Windows.Forms.RadioButton();
            this.rdoPendentes = new System.Windows.Forms.RadioButton();
            this.rdoProntas = new System.Windows.Forms.RadioButton();
            this.rdoEntregues = new System.Windows.Forms.RadioButton();
            this.tlpMain.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            this.flpFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOS)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.pnlHeader, 0, 0);
            this.tlpMain.Controls.Add(this.pnlSearch, 0, 1);
            this.tlpMain.Controls.Add(this.dgvOS, 0, 2);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 3;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Size = new System.Drawing.Size(900, 600);
            this.tlpMain.TabIndex = 0;
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.btnExcluir);
            this.pnlHeader.Controls.Add(this.btnImprimir);
            this.pnlHeader.Controls.Add(this.btnEditar);
            this.pnlHeader.Controls.Add(this.btnNovo);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(900, 60);
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
            this.lblTitle.Text = "Ordens de Serviço (O.S.)";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnNovo
            // 
            this.btnNovo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNovo.Location = new System.Drawing.Point(720, 8);
            this.btnNovo.Name = "btnNovo";
            this.btnNovo.Size = new System.Drawing.Size(160, 44);
            this.btnNovo.TabIndex = 1;
            this.btnNovo.Text = "➕ Nova O.S.";
            this.btnNovo.UseVisualStyleBackColor = true;
            // 
            // btnEditar
            // 
            this.btnEditar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditar.Location = new System.Drawing.Point(540, 15);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(100, 35);
            this.btnEditar.TabIndex = 2;
            this.btnEditar.Text = "Editar / Ver";
            this.btnEditar.UseVisualStyleBackColor = true;
            // 
            // btnImprimir
            // 
            this.btnImprimir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImprimir.Location = new System.Drawing.Point(650, 15);
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.Size = new System.Drawing.Size(100, 35);
            this.btnImprimir.TabIndex = 4;
            this.btnImprimir.Text = "Imprimir";
            this.btnImprimir.UseVisualStyleBackColor = true;
            // 
            // btnExcluir
            // 
            this.btnExcluir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExcluir.Location = new System.Drawing.Point(760, 15);
            this.btnExcluir.Name = "btnExcluir";
            this.btnExcluir.Size = new System.Drawing.Size(100, 35);
            this.btnExcluir.TabIndex = 3;
            this.btnExcluir.Text = "Cancelar";
            this.btnExcluir.UseVisualStyleBackColor = true;
            // 
            // pnlSearch
            // 
            this.pnlSearch.Controls.Add(this.flpFiltros);
            this.pnlSearch.Controls.Add(this.txtBusca);
            this.pnlSearch.Controls.Add(this.lblBusca);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSearch.Location = new System.Drawing.Point(0, 60);
            this.pnlSearch.Margin = new System.Windows.Forms.Padding(0);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Size = new System.Drawing.Size(900, 100);
            this.pnlSearch.TabIndex = 1;
            // 
            // flpFiltros
            // 
            this.flpFiltros.Controls.Add(this.rdoTodas);
            this.flpFiltros.Controls.Add(this.rdoPendentes);
            this.flpFiltros.Controls.Add(this.rdoProntas);
            this.flpFiltros.Controls.Add(this.rdoEntregues);
            this.flpFiltros.Location = new System.Drawing.Point(20, 55);
            this.flpFiltros.Name = "flpFiltros";
            this.flpFiltros.Size = new System.Drawing.Size(840, 40);
            this.flpFiltros.TabIndex = 2;
            // 
            // rdoTodas
            // 
            this.rdoTodas.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdoTodas.AutoSize = true;
            this.rdoTodas.Checked = true;
            this.rdoTodas.Name = "rdoTodas";
            this.rdoTodas.Size = new System.Drawing.Size(60, 30);
            this.rdoTodas.TabIndex = 0;
            this.rdoTodas.TabStop = true;
            this.rdoTodas.Text = "Todas";
            this.rdoTodas.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rdoPendentes
            // 
            this.rdoPendentes.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdoPendentes.AutoSize = true;
            this.rdoPendentes.Name = "rdoPendentes";
            this.rdoPendentes.Size = new System.Drawing.Size(80, 30);
            this.rdoPendentes.TabIndex = 1;
            this.rdoPendentes.Text = "Pendentes";
            this.rdoPendentes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rdoProntas
            // 
            this.rdoProntas.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdoProntas.AutoSize = true;
            this.rdoProntas.Name = "rdoProntas";
            this.rdoProntas.Size = new System.Drawing.Size(70, 30);
            this.rdoProntas.TabIndex = 2;
            this.rdoProntas.Text = "Prontas";
            this.rdoProntas.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rdoEntregues
            // 
            this.rdoEntregues.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdoEntregues.AutoSize = true;
            this.rdoEntregues.Name = "rdoEntregues";
            this.rdoEntregues.Size = new System.Drawing.Size(80, 30);
            this.rdoEntregues.TabIndex = 3;
            this.rdoEntregues.Text = "Entregues";
            this.rdoEntregues.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.txtBusca.Size = new System.Drawing.Size(790, 23);
            this.txtBusca.TabIndex = 1;
            // 
            // dgvOS
            // 
            this.dgvOS.AllowUserToAddRows = false;
            this.dgvOS.AllowUserToDeleteRows = false;
            this.dgvOS.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvOS.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvOS.Location = new System.Drawing.Point(20, 160);
            this.dgvOS.Margin = new System.Windows.Forms.Padding(20, 0, 20, 20);
            this.dgvOS.MultiSelect = false;
            this.dgvOS.Name = "dgvOS";
            this.dgvOS.ReadOnly = true;
            this.dgvOS.RowTemplate.Height = 45;
            this.dgvOS.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvOS.Size = new System.Drawing.Size(860, 420);
            this.dgvOS.TabIndex = 2;
            // 
            // FrmOS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.tlpMain);
            this.Name = "FrmOS";
            this.Text = "Ordens de Serviço";
            this.tlpMain.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.flpFiltros.ResumeLayout(false);
            this.flpFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOS)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnNovo;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnImprimir;
        private System.Windows.Forms.Button btnExcluir;
        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.Label lblBusca;
        private System.Windows.Forms.TextBox txtBusca;
        private System.Windows.Forms.DataGridView dgvOS;
        private System.Windows.Forms.FlowLayoutPanel flpFiltros;
        private System.Windows.Forms.RadioButton rdoTodas;
        private System.Windows.Forms.RadioButton rdoPendentes;
        private System.Windows.Forms.RadioButton rdoProntas;
        private System.Windows.Forms.RadioButton rdoEntregues;
    }
}
