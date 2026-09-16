using System;
using System.Drawing;
using System.Windows.Forms;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public class FrmPromptPagamentoOS : Form
    {
        public string FormaPagamento { get; private set; }
        public bool JaRecebido { get; private set; }

        private ComboBox cboFormaPagamento;
        private CheckBox chkRecebido;
        private Button btnConfirmar;
        private Button btnCancelar;

        public FrmPromptPagamentoOS(decimal valorCobrado)
        {
            this.Size = new Size(350, 250);
            this.Text = "Lançamento Financeiro";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            UITheme.ApplyModalWindowStyle(this);
            this.BackColor = UITheme.BgModal;

            Panel pnlMain = new Panel { Location = new Point(20, 20), Size = new Size(310, 210), BackColor = UITheme.BgCard };
            this.Controls.Add(pnlMain);
            pnlMain.BringToFront();

            Label lblTitle = new Label { Text = $"Valor Cobrado: {valorCobrado:C2}\n\nSelecione a forma de pagamento:", Location = new Point(15, 15), Size = new Size(280, 40), Font = UITheme.FontBodyBold, ForeColor = UITheme.TextTitle };
            pnlMain.Controls.Add(lblTitle);

            cboFormaPagamento = new ComboBox { Location = new Point(15, 65), Size = new Size(280, 30), Font = new Font("Segoe UI", 12F), BackColor = UITheme.BgSidebar, ForeColor = UITheme.TextTitle, FlatStyle = FlatStyle.Flat, DropDownStyle = ComboBoxStyle.DropDownList };
            cboFormaPagamento.Items.AddRange(new object[] { "Dinheiro", "Cartão de Crédito", "Cartão de Débito", "Pix", "Transferência Bancária" });
            cboFormaPagamento.SelectedIndex = 0;
            pnlMain.Controls.Add(cboFormaPagamento);

            chkRecebido = new CheckBox { Text = "Dinheiro já recebido? (Lançar no Caixa)", Location = new Point(15, 105), Size = new Size(280, 30), Font = UITheme.FontBody, ForeColor = UITheme.TextTitle, Checked = true };
            pnlMain.Controls.Add(chkRecebido);

            btnConfirmar = new Button { Text = "CONFIRMAR", Location = new Point(165, 155), Size = new Size(130, 32) };
            UITheme.FormatSaaSButton(btnConfirmar, true);
            btnConfirmar.Click += (s, e) =>
            {
                FormaPagamento = cboFormaPagamento.Text;
                JaRecebido = chkRecebido.Checked;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            pnlMain.Controls.Add(btnConfirmar);

            btnCancelar = new Button { Text = "VOLTAR", Location = new Point(15, 155), Size = new Size(130, 32) };
            UITheme.FormatSaaSButton(btnCancelar, false);
            btnCancelar.BackColor = UITheme.ElementBorder;
            btnCancelar.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
            pnlMain.Controls.Add(btnCancelar);
        }
    }
}
