using System;
using System.Drawing;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public partial class FrmConfiguracaoTermos : Form
    {
        private readonly ConfiguracaoTermosRepository _repository = new ConfiguracaoTermosRepository();

        public FrmConfiguracaoTermos()
        {
            InitializeComponent();
            ApplyTheme();
            LoadTermos();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModalWindowStyle(this);
            this.BackColor = UITheme.BgModal;
            pnlMain.BackColor = UITheme.BgCard;

            lblTitle.Font = UITheme.FontTitle;
            lblTitle.ForeColor = UITheme.TextTitle;

            lblDesc.Font = UITheme.FontBody;
            lblDesc.ForeColor = UITheme.TextMuted;

            UITheme.FormatModernTextBox(txtTermos);
            txtTermos.Font = new Font("Consolas", 10F); // Keep monospaced font for code/terms

            // Adding a subtle border panel wrapping txtTermos is ideal, but for now we remove the ugly 3D border
            // and rely on the BgInput color difference.

            UITheme.FormatSaaSButton(btnSalvar, true);
            UITheme.FormatSaaSButton(btnCancelar, false);
        }

        private void LoadTermos()
        {
            txtTermos.Text = _repository.ObterTermos();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            string novosTermos = txtTermos.Text.Trim();
            if (string.IsNullOrEmpty(novosTermos))
            {
                MessageBox.Show("Os termos não podem ficar totalmente vazios.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var conf = _repository.ObterConfiguracao();
                conf.TermosOS = novosTermos;
                _repository.Salvar(conf);
                FrmNotification.ShowSuccess("Termos e condies salvos com sucesso!", "Configuraǜo Salva");
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                FrmNotification.ShowError("Erro ao salvar termos: " + ex.Message, "Erro");
            }
        }
    }
}
