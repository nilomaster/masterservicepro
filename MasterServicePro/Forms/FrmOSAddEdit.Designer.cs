namespace MasterServicePro.Forms
{
    partial class FrmOSAddEdit
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
            this.lblCliente = new System.Windows.Forms.Label();
            this.cboCliente = new System.Windows.Forms.ComboBox();
            this.lblMarca = new System.Windows.Forms.Label();
            this.txtMarca = new System.Windows.Forms.TextBox();
            this.lblModelo = new System.Windows.Forms.Label();
            this.txtModelo = new System.Windows.Forms.TextBox();
            this.lblDefeito = new System.Windows.Forms.Label();
            this.txtDefeito = new System.Windows.Forms.TextBox();
            this.lblLaudo = new System.Windows.Forms.Label();
            this.txtLaudo = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cboStatus = new System.Windows.Forms.ComboBox();
            this.lblValorPecas = new System.Windows.Forms.Label();
            this.txtValorPecas = new System.Windows.Forms.TextBox();
            this.lblValorServico = new System.Windows.Forms.Label();
            this.txtValorServico = new System.Windows.Forms.TextBox();
            this.lblValor = new System.Windows.Forms.Label();
            this.txtValor = new System.Windows.Forms.TextBox();
            this.lblImei = new System.Windows.Forms.Label();
            this.txtImei = new System.Windows.Forms.TextBox();
            this.lblCor = new System.Windows.Forms.Label();
            this.txtCor = new System.Windows.Forms.TextBox();
            this.btnSalvar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.dgvItens = new System.Windows.Forms.DataGridView();
            this.lblItens = new System.Windows.Forms.Label();
            this.btnAddItem = new System.Windows.Forms.Button();
            this.btnRemoverItem = new System.Windows.Forms.Button();
            this.lblTecnico = new System.Windows.Forms.Label();
            this.cboTecnico = new System.Windows.Forms.ComboBox();
            
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItens)).BeginInit();
            this.SuspendLayout();
            // 
            this.pnlMain.Controls.Add(this.dgvItens);
            this.pnlMain.Controls.Add(this.lblItens);
            this.pnlMain.Controls.Add(this.btnAddItem);
            this.pnlMain.Controls.Add(this.btnRemoverItem);
            this.pnlMain.Controls.Add(this.lblTecnico);
            this.pnlMain.Controls.Add(this.cboTecnico);
            
            this.pnlMain.Controls.Add(this.btnCancelar);
            this.pnlMain.Controls.Add(this.btnSalvar);
            this.pnlMain.Controls.Add(this.txtValor);
            this.pnlMain.Controls.Add(this.lblValor);
            this.pnlMain.Controls.Add(this.txtValorServico);
            this.pnlMain.Controls.Add(this.lblValorServico);
            this.pnlMain.Controls.Add(this.txtValorPecas);
            this.pnlMain.Controls.Add(this.lblValorPecas);
            this.pnlMain.Controls.Add(this.cboStatus);
            this.pnlMain.Controls.Add(this.lblStatus);
            this.pnlMain.Controls.Add(this.txtLaudo);
            this.pnlMain.Controls.Add(this.lblLaudo);
            this.pnlMain.Controls.Add(this.txtDefeito);
            this.pnlMain.Controls.Add(this.lblDefeito);
            this.pnlMain.Controls.Add(this.txtCor);
            this.pnlMain.Controls.Add(this.lblCor);
            this.pnlMain.Controls.Add(this.txtImei);
            this.pnlMain.Controls.Add(this.lblImei);
            this.pnlMain.Controls.Add(this.txtModelo);
            this.pnlMain.Controls.Add(this.lblModelo);
            this.pnlMain.Controls.Add(this.txtMarca);
            this.pnlMain.Controls.Add(this.lblMarca);
            this.pnlMain.Controls.Add(this.cboCliente);
            this.pnlMain.Controls.Add(this.lblCliente);
            this.pnlMain.Controls.Add(this.lblTitle);
            this.pnlMain.Location = new System.Drawing.Point(20, 20);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(910, 660);
            this.pnlMain.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(126, 15);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "ORDEM DE SERVIÇO";
            // 
            // lblCliente
            // 
            this.lblCliente.AutoSize = true;
            this.lblCliente.Location = new System.Drawing.Point(20, 60);
            this.lblCliente.Name = "lblCliente";
            this.lblCliente.Size = new System.Drawing.Size(47, 15);
            this.lblCliente.TabIndex = 1;
            this.lblCliente.Text = "Cliente:";
            // 
            // cboCliente
            // 
            this.cboCliente.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboCliente.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboCliente.FormattingEnabled = true;
            this.cboCliente.Location = new System.Drawing.Point(20, 80);
            this.cboCliente.Name = "cboCliente";
            this.cboCliente.Size = new System.Drawing.Size(520, 23);
            this.cboCliente.TabIndex = 2;
            // 
            // lblMarca
            // 
            this.lblMarca.AutoSize = true;
            this.lblMarca.Location = new System.Drawing.Point(20, 120);
            this.lblMarca.Name = "lblMarca";
            this.lblMarca.Size = new System.Drawing.Size(91, 15);
            this.lblMarca.TabIndex = 3;
            this.lblMarca.Text = "Marca/Aparelho:";
            // 
            // txtMarca
            // 
            this.txtMarca.Location = new System.Drawing.Point(20, 140);
            this.txtMarca.Name = "txtMarca";
            this.txtMarca.Size = new System.Drawing.Size(250, 23);
            this.txtMarca.TabIndex = 4;
            // 
            // lblModelo
            // 
            this.lblModelo.AutoSize = true;
            this.lblModelo.Location = new System.Drawing.Point(290, 120);
            this.lblModelo.Name = "lblModelo";
            this.lblModelo.Size = new System.Drawing.Size(51, 15);
            this.lblModelo.TabIndex = 5;
            this.lblModelo.Text = "Modelo:";
            // 
            // txtModelo
            // 
            this.txtModelo.Location = new System.Drawing.Point(290, 140);
            this.txtModelo.Name = "txtModelo";
            this.txtModelo.Size = new System.Drawing.Size(250, 23);
            this.txtModelo.TabIndex = 6;
            // 
            // lblImei
            // 
            this.lblImei.AutoSize = true;
            this.lblImei.Location = new System.Drawing.Point(20, 180);
            this.lblImei.Name = "lblImei";
            this.lblImei.Size = new System.Drawing.Size(43, 15);
            this.lblImei.TabIndex = 21;
            this.lblImei.Text = "IMEI 1:";
            // 
            // txtImei
            // 
            this.txtImei.Location = new System.Drawing.Point(20, 200);
            this.txtImei.Name = "txtImei";
            this.txtImei.Size = new System.Drawing.Size(250, 23);
            this.txtImei.TabIndex = 22;
            // 
            // lblCor
            // 
            this.lblCor.AutoSize = true;
            this.lblCor.Location = new System.Drawing.Point(290, 180);
            this.lblCor.Name = "lblCor";
            this.lblCor.Size = new System.Drawing.Size(100, 15);
            this.lblCor.TabIndex = 23;
            this.lblCor.Text = "Cor do Aparelho:";
            // 
            // txtCor
            // 
            this.txtCor.Location = new System.Drawing.Point(290, 200);
            this.txtCor.Name = "txtCor";
            this.txtCor.Size = new System.Drawing.Size(250, 23);
            this.txtCor.TabIndex = 24;
            // 
            // lblDefeito
            // 
            this.lblDefeito.AutoSize = true;
            this.lblDefeito.Location = new System.Drawing.Point(20, 240);
            this.lblDefeito.Name = "lblDefeito";
            this.lblDefeito.Size = new System.Drawing.Size(107, 15);
            this.lblDefeito.TabIndex = 7;
            this.lblDefeito.Text = "Problema Relatado:";
            // 
            // txtDefeito
            // 
            this.txtDefeito.Location = new System.Drawing.Point(20, 260);
            this.txtDefeito.Multiline = true;
            this.txtDefeito.Name = "txtDefeito";
            this.txtDefeito.Size = new System.Drawing.Size(520, 80);
            this.txtDefeito.TabIndex = 8;
            // 
            // lblLaudo
            // 
            this.lblLaudo.AutoSize = true;
            this.lblLaudo.Location = new System.Drawing.Point(20, 360);
            this.lblLaudo.Name = "lblLaudo";
            this.lblLaudo.Size = new System.Drawing.Size(91, 15);
            this.lblLaudo.TabIndex = 9;
            this.lblLaudo.Text = "Laudo Técnico:";
            // 
            // txtLaudo
            // 
            this.txtLaudo.Location = new System.Drawing.Point(20, 380);
            this.txtLaudo.Multiline = true;
            this.txtLaudo.Name = "txtLaudo";
            this.txtLaudo.Size = new System.Drawing.Size(520, 80);
            this.txtLaudo.TabIndex = 10;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(20, 480);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(42, 15);
            this.lblStatus.TabIndex = 11;
            this.lblStatus.Text = "Status:";
            // 
            // cboStatus
            // 
            this.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatus.FormattingEnabled = true;
            this.cboStatus.Items.AddRange(new object[] {
            "Pendente",
            "Em Manutenção",
            "Aguardando Peça",
            "Finalizado",
            "Entregue",
            "Cancelado"});
            this.cboStatus.Location = new System.Drawing.Point(20, 500);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(120, 23);
            this.cboStatus.TabIndex = 12;
            // 
            // lblValorPecas
            // 
            this.lblValorPecas.AutoSize = true;
            this.lblValorPecas.Location = new System.Drawing.Point(155, 480);
            this.lblValorPecas.Name = "lblValorPecas";
            this.lblValorPecas.Size = new System.Drawing.Size(83, 15);
            this.lblValorPecas.TabIndex = 35;
            this.lblValorPecas.Text = "Val. Custo (R$):";
            // 
            // txtValorPecas
            // 
            this.txtValorPecas.Location = new System.Drawing.Point(155, 500);
            this.txtValorPecas.Name = "txtValorPecas";
            this.txtValorPecas.Size = new System.Drawing.Size(120, 23);
            this.txtValorPecas.TabIndex = 36;
            this.txtValorPecas.Text = "0,00";
            // 
            // lblValorServico
            // 
            this.lblValorServico.AutoSize = true;
            this.lblValorServico.Location = new System.Drawing.Point(290, 480);
            this.lblValorServico.Name = "lblValorServico";
            this.lblValorServico.Size = new System.Drawing.Size(95, 15);
            this.lblValorServico.TabIndex = 37;
            this.lblValorServico.Text = "Val. Cobrado (R$):";
            // 
            // txtValorServico
            // 
            this.txtValorServico.Location = new System.Drawing.Point(290, 500);
            this.txtValorServico.Name = "txtValorServico";
            this.txtValorServico.Size = new System.Drawing.Size(120, 23);
            this.txtValorServico.TabIndex = 38;
            this.txtValorServico.Text = "0,00";
            // 
            // lblValor
            // 
            this.lblValor.AutoSize = true;
            this.lblValor.Location = new System.Drawing.Point(425, 480);
            this.lblValor.Name = "lblValor";
            this.lblValor.Size = new System.Drawing.Size(84, 15);
            this.lblValor.TabIndex = 13;
            this.lblValor.Text = "Val. Lucro (R$):";
            // 
            // txtValor
            // 
            this.txtValor.Location = new System.Drawing.Point(425, 500);
            this.txtValor.Name = "txtValor";
            this.txtValor.Size = new System.Drawing.Size(115, 30);
            this.txtValor.TabIndex = 14;
            this.txtValor.Text = "0,00";
            // 
            // btnSalvar
            // 
            this.btnSalvar.Location = new System.Drawing.Point(340, 580);
            this.btnSalvar.Name = "btnSalvar";
            this.btnSalvar.Size = new System.Drawing.Size(200, 45);
            this.btnSalvar.TabIndex = 15;
            this.btnSalvar.Text = "SALVAR";
            this.btnSalvar.UseVisualStyleBackColor = true;
            // 
            // btnCancelar
            // 
            this.btnCancelar.Location = new System.Drawing.Point(20, 580);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(200, 45);
            this.btnCancelar.TabIndex = 16;
            this.btnCancelar.Text = "CANCELAR";
            this.btnCancelar.UseVisualStyleBackColor = true;
            // 
            // lblItens
            // 
            this.lblItens.AutoSize = true;
            this.lblItens.Location = new System.Drawing.Point(580, 20);
            this.lblItens.Name = "lblItens";
            this.lblItens.Size = new System.Drawing.Size(150, 15);
            this.lblItens.TabIndex = 17;
            this.lblItens.Text = "Peças / Serviços Adicionais";
            // 
            // dgvItens
            // 
            this.dgvItens.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItens.Location = new System.Drawing.Point(580, 50);
            this.dgvItens.Name = "dgvItens";
            this.dgvItens.RowTemplate.Height = 25;
            this.dgvItens.Size = new System.Drawing.Size(310, 310);
            this.dgvItens.TabIndex = 18;
            // 
            // btnAddItem
            // 
            this.btnAddItem.Location = new System.Drawing.Point(580, 380);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(150, 45);
            this.btnAddItem.TabIndex = 19;
            this.btnAddItem.Text = "+ Adicionar Peça";
            this.btnAddItem.UseVisualStyleBackColor = true;
            // 
            // btnRemoverItem
            // 
            this.btnRemoverItem.Location = new System.Drawing.Point(740, 380);
            this.btnRemoverItem.Name = "btnRemoverItem";
            this.btnRemoverItem.Size = new System.Drawing.Size(150, 45);
            this.btnRemoverItem.TabIndex = 20;
            this.btnRemoverItem.Text = "Remover Peça";
            this.btnRemoverItem.UseVisualStyleBackColor = true;
            // 
            // lblTecnico
            // 
            this.lblTecnico.AutoSize = true;
            this.lblTecnico.Location = new System.Drawing.Point(580, 445);
            this.lblTecnico.Name = "lblTecnico";
            this.lblTecnico.Size = new System.Drawing.Size(125, 15);
            this.lblTecnico.TabIndex = 39;
            this.lblTecnico.Text = "Técnico Responsável:";
            // 
            // cboTecnico
            // 
            this.cboTecnico.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTecnico.FormattingEnabled = true;
            this.cboTecnico.Location = new System.Drawing.Point(580, 465);
            this.cboTecnico.Name = "cboTecnico";
            this.cboTecnico.Size = new System.Drawing.Size(310, 23);
            this.cboTecnico.TabIndex = 40;
            // 
            // FrmOSAddEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 710);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmOSAddEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manutenção de Ordem de Serviço";
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItens)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblCliente;
        private System.Windows.Forms.ComboBox cboCliente;
        private System.Windows.Forms.Label lblMarca;
        private System.Windows.Forms.TextBox txtMarca;
        private System.Windows.Forms.Label lblModelo;
        private System.Windows.Forms.TextBox txtModelo;
        private System.Windows.Forms.Label lblDefeito;
        private System.Windows.Forms.TextBox txtDefeito;
        private System.Windows.Forms.Label lblLaudo;
        private System.Windows.Forms.TextBox txtLaudo;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cboStatus;
        private System.Windows.Forms.Label lblValorPecas;
        private System.Windows.Forms.TextBox txtValorPecas;
        private System.Windows.Forms.Label lblValorServico;
        private System.Windows.Forms.TextBox txtValorServico;
        private System.Windows.Forms.Label lblValor;
        private System.Windows.Forms.TextBox txtValor;
        private System.Windows.Forms.Label lblImei;
        private System.Windows.Forms.TextBox txtImei;
        private System.Windows.Forms.Label lblCor;
        private System.Windows.Forms.TextBox txtCor;
        private System.Windows.Forms.Button btnSalvar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Label lblItens;
        private System.Windows.Forms.DataGridView dgvItens;
        private System.Windows.Forms.Button btnAddItem;
        private System.Windows.Forms.Button btnRemoverItem;
        private System.Windows.Forms.Label lblTecnico;
        private System.Windows.Forms.ComboBox cboTecnico;
    }
}
