using System;
using System.Drawing;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public partial class FrmCaixaAbertura : Form
    {
        private FinanceiroRepository repository = new FinanceiroRepository();

        public FrmCaixaAbertura()
        {
            InitializeComponent();
            ApplyTheme();
            
            btnAbrir.Click += BtnAbrir_Click;
            btnCancelar.Click += (s, e) => this.Close();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModalWindowStyle(this);
            this.BackColor = UITheme.BgModal;
            pnlCard.BackColor = UITheme.BgCard;

            lblTitle.Font = UITheme.FontTitle;
            lblTitle.ForeColor = UITheme.TextTitle;

            lblValor.Font = UITheme.FontBody;
            lblValor.ForeColor = UITheme.TextMuted;

            numValor.BackColor = UITheme.BgSidebar;
            numValor.ForeColor = UITheme.TextTitle;
            numValor.Font = new Font("Segoe UI", 18, FontStyle.Bold);

            UITheme.FormatSaaSButton(btnAbrir, true);
            UITheme.FormatSaaSButton(btnCancelar, false);
            btnCancelar.BackColor = UITheme.Danger;
        }

        private void BtnAbrir_Click(object sender, EventArgs e)
        {
            if (numValor.Value < 10m)
            {
                MessageBox.Show("O valor de fundo de troco para abertura do caixa deve ser de no mínimo R$ 10,00.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                repository.AbrirCaixa(numValor.Value);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao abrir caixa: " + ex.Message);
            }
        }
    }
}
