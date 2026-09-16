namespace MasterServicePro.Forms
{
    partial class FrmPDVFinalizar
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
            this.pnlContainer = new System.Windows.Forms.Panel();
            this.lblRestanteLabel = new System.Windows.Forms.Label();
            this.lblRestanteValue = new System.Windows.Forms.Label();
            this.btnAddPagamento = new System.Windows.Forms.Button();
            this.dgvPagamentos = new System.Windows.Forms.DataGridView();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnConfirmar = new System.Windows.Forms.Button();
            this.lblTrocoValue = new System.Windows.Forms.Label();
            this.lblTrocoLabel = new System.Windows.Forms.Label();
            this.txtRecebido = new System.Windows.Forms.TextBox();
            this.lblRecebidoLabel = new System.Windows.Forms.Label();
            this.cboPagamento = new System.Windows.Forms.ComboBox();
            this.lblPagamentoLabel = new System.Windows.Forms.Label();
            this.lblTotalValue = new System.Windows.Forms.Label();
            this.lblTotalLabel = new System.Windows.Forms.Label();
            this.lblClienteLabel = new System.Windows.Forms.Label();
            this.cboCliente = new System.Windows.Forms.ComboBox();
            this.lblSaldoDisponivel = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPagamentos)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlContainer
            // 
            this.pnlContainer.Controls.Add(this.lblRestanteLabel);
            this.pnlContainer.Controls.Add(this.lblRestanteValue);
            this.pnlContainer.Controls.Add(this.btnAddPagamento);
            this.pnlContainer.Controls.Add(this.dgvPagamentos);
            this.pnlContainer.Controls.Add(this.btnCancelar);
            this.pnlContainer.Controls.Add(this.btnConfirmar);
            this.pnlContainer.Controls.Add(this.lblTrocoValue);
            this.pnlContainer.Controls.Add(this.lblTrocoLabel);
            this.pnlContainer.Controls.Add(this.txtRecebido);
            this.pnlContainer.Controls.Add(this.lblRecebidoLabel);
            this.pnlContainer.Controls.Add(this.cboPagamento);
            this.pnlContainer.Controls.Add(this.lblPagamentoLabel);
            this.pnlContainer.Controls.Add(this.lblTotalValue);
            this.pnlContainer.Controls.Add(this.lblTotalLabel);
            this.pnlContainer.Controls.Add(this.lblClienteLabel);
            this.pnlContainer.Controls.Add(this.cboCliente);
            this.pnlContainer.Controls.Add(this.lblSaldoDisponivel);
            this.pnlContainer.Controls.Add(this.lblTitle);
            this.pnlContainer.Location = new System.Drawing.Point(20, 20);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Size = new System.Drawing.Size(760, 660);
            this.pnlContainer.TabIndex = 0;
            // 
            this.lblRestanteValue.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblRestanteValue.ForeColor = System.Drawing.Color.LightSalmon;
            this.lblRestanteValue.Location = new System.Drawing.Point(30, 470);
            this.lblRestanteValue.Name = "lblRestanteValue";
            this.lblRestanteValue.Size = new System.Drawing.Size(300, 50);
            this.lblRestanteValue.TabIndex = 14;
            this.lblRestanteValue.Text = "R$ 0,00";
            this.lblRestanteValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblRestanteLabel
            // 
            this.lblRestanteLabel.AutoSize = true;
            this.lblRestanteLabel.Location = new System.Drawing.Point(30, 450);
            this.lblRestanteLabel.Name = "lblRestanteLabel";
            this.lblRestanteLabel.Size = new System.Drawing.Size(63, 15);
            this.lblRestanteLabel.TabIndex = 13;
            this.lblRestanteLabel.Text = "RESTANTE:";
            this.lblRestanteValue.Name = "lblRestanteValue";
            this.lblRestanteValue.Size = new System.Drawing.Size(300, 50);
            this.lblRestanteValue.TabIndex = 14;
            this.lblRestanteValue.Text = "R$ 0,00";
            this.lblRestanteValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            this.btnAddPagamento.Location = new System.Drawing.Point(240, 370);
            this.btnAddPagamento.Name = "btnAddPagamento";
            this.btnAddPagamento.Size = new System.Drawing.Size(90, 50);
            this.btnAddPagamento.TabIndex = 12;
            this.btnAddPagamento.Text = "ADD (+)";
            this.btnAddPagamento.UseVisualStyleBackColor = true;
            // 
            // dgvPagamentos
            // 
            this.dgvPagamentos.AllowUserToAddRows = false;
            this.dgvPagamentos.AllowUserToResizeRows = false;
            this.dgvPagamentos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPagamentos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPagamentos.Location = new System.Drawing.Point(350, 70);
            this.dgvPagamentos.Name = "dgvPagamentos";
            this.dgvPagamentos.ReadOnly = true;
            this.dgvPagamentos.RowHeadersVisible = false;
            this.dgvPagamentos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPagamentos.Size = new System.Drawing.Size(380, 380);
            this.dgvPagamentos.TabIndex = 11;
            // 
            this.btnCancelar.Location = new System.Drawing.Point(350, 580);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(180, 50);
            this.btnCancelar.TabIndex = 10;
            this.btnCancelar.Text = "CANCELAR (ESC)";
            this.btnCancelar.UseVisualStyleBackColor = true;
            // 
            // btnConfirmar
            // 
            this.btnConfirmar.Enabled = false;
            this.btnConfirmar.Location = new System.Drawing.Point(540, 580);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.Size = new System.Drawing.Size(190, 50);
            this.btnConfirmar.TabIndex = 9;
            this.btnConfirmar.Text = "FINALIZAR (ENTER)";
            this.btnConfirmar.UseVisualStyleBackColor = true;
            // 
            this.lblTrocoValue.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTrocoValue.Location = new System.Drawing.Point(30, 550);
            this.lblTrocoValue.Name = "lblTrocoValue";
            this.lblTrocoValue.Size = new System.Drawing.Size(300, 50);
            this.lblTrocoValue.TabIndex = 8;
            this.lblTrocoValue.Text = "R$ 0,00";
            this.lblTrocoValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTrocoLabel
            // 
            this.lblTrocoLabel.AutoSize = true;
            this.lblTrocoLabel.Location = new System.Drawing.Point(30, 530);
            this.lblTrocoLabel.Name = "lblTrocoLabel";
            this.lblTrocoLabel.Size = new System.Drawing.Size(48, 15);
            this.lblTrocoLabel.TabIndex = 7;
            this.lblTrocoLabel.Text = "TROCO:";
            // 
            this.lblRecebidoLabel.Location = new System.Drawing.Point(30, 350);
            this.lblRecebidoLabel.Name = "lblRecebidoLabel";
            this.lblRecebidoLabel.Size = new System.Drawing.Size(124, 15);
            this.lblRecebidoLabel.TabIndex = 5;
            this.lblRecebidoLabel.Text = "VALOR DO MÉTODO:";
            // 
            // txtRecebido
            // 
            this.txtRecebido.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtRecebido.Location = new System.Drawing.Point(30, 370);
            this.txtRecebido.Name = "txtRecebido";
            this.txtRecebido.Size = new System.Drawing.Size(200, 50);
            this.txtRecebido.TabIndex = 6;
            this.txtRecebido.Text = "0,00";
            // 
            // cboPagamento
            // 
            this.cboPagamento.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPagamento.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cboPagamento.FormattingEnabled = true;
            this.cboPagamento.Items.AddRange(new object[] {
            "Dinheiro",
            "Cartão de Crédito",
            "Cartão de Débito",
            "Pix",
            "A Prazo (Fiado)",
            "Saldo do Cliente"});
            this.lblPagamentoLabel.Location = new System.Drawing.Point(30, 270);
            this.lblPagamentoLabel.Name = "lblPagamentoLabel";
            this.lblPagamentoLabel.Size = new System.Drawing.Size(124, 15);
            this.lblPagamentoLabel.TabIndex = 3;
            this.lblPagamentoLabel.Text = "FORMA PAGAMENTO:";
            // 
            // cboPagamento
            // 
            this.cboPagamento.Location = new System.Drawing.Point(30, 290);
            this.cboPagamento.Name = "cboPagamento";
            this.cboPagamento.Size = new System.Drawing.Size(300, 33);
            // 
            this.lblClienteLabel.Location = new System.Drawing.Point(30, 190);
            this.lblClienteLabel.Name = "lblClienteLabel";
            this.lblClienteLabel.Size = new System.Drawing.Size(56, 15);
            this.lblClienteLabel.TabIndex = 15;
            this.lblClienteLabel.Text = "CLIENTE:";
            // 
            // cboCliente
            // 
            this.cboCliente.Location = new System.Drawing.Point(30, 210);
            this.cboCliente.Name = "cboCliente";
            this.cboCliente.Size = new System.Drawing.Size(300, 29);
            // 
            // lblSaldoDisponivel
            // 
            this.lblSaldoDisponivel.AutoSize = true;
            this.lblSaldoDisponivel.ForeColor = System.Drawing.Color.LightGreen;
            this.lblSaldoDisponivel.Location = new System.Drawing.Point(30, 245);
            this.lblSaldoDisponivel.Name = "lblSaldoDisponivel";
            this.lblSaldoDisponivel.Size = new System.Drawing.Size(150, 15);
            this.lblSaldoDisponivel.TabIndex = 16;
            this.lblSaldoDisponivel.Text = "Saldo Disponível: R$ 0,00";
            // 
            // lblTotalValue
            // 
            this.lblTotalValue.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTotalValue.Location = new System.Drawing.Point(30, 90);
            this.lblTotalValue.Name = "lblTotalValue";
            this.lblTotalValue.Size = new System.Drawing.Size(300, 60);
            this.lblTotalValue.TabIndex = 2;
            this.lblTotalValue.Text = "R$ 0,00";
            this.lblTotalValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotalLabel
            // 
            this.lblTotalLabel.AutoSize = true;
            this.lblTotalLabel.Location = new System.Drawing.Point(30, 70);
            this.lblTotalLabel.Name = "lblTotalLabel";
            this.lblTotalLabel.Size = new System.Drawing.Size(95, 15);
            this.lblTotalLabel.TabIndex = 1;
            this.lblTotalLabel.Text = "TOTAL A PAGAR:";
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(760, 50);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "FINALIZAR VENDA (PAGAMENTO MISTO)";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            this.ClientSize = new System.Drawing.Size(800, 700);
            this.Controls.Add(this.pnlContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmPDVFinalizar";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Checkout";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.pnlContainer.ResumeLayout(false);
            this.pnlContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPagamentos)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlContainer;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblTotalValue;
        private System.Windows.Forms.Label lblTotalLabel;
        private System.Windows.Forms.ComboBox cboPagamento;
        private System.Windows.Forms.Label lblPagamentoLabel;
        private System.Windows.Forms.TextBox txtRecebido;
        private System.Windows.Forms.Label lblRecebidoLabel;
        private System.Windows.Forms.Label lblTrocoValue;
        private System.Windows.Forms.Label lblTrocoLabel;
        private System.Windows.Forms.Button btnConfirmar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.DataGridView dgvPagamentos;
        private System.Windows.Forms.Button btnAddPagamento;
        private System.Windows.Forms.Label lblRestanteLabel;
        private System.Windows.Forms.Label lblRestanteValue;
        private System.Windows.Forms.Label lblClienteLabel;
        private System.Windows.Forms.ComboBox cboCliente;
        private System.Windows.Forms.Label lblSaldoDisponivel;
    }
}
