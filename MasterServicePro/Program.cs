using System;
using System.Configuration;
using System.Windows.Forms;

namespace MasterServicePro
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Verifica conexao com o banco de dados antes de iniciar o aplicativo
            try
            {
                using (var conn = new MasterServicePro.Utils.DbConnection().GetConnection())
                {
                    conn.Open();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Conexao com banco de dados falhou!!\n\nDetalhes do erro: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Sai do aplicativo sem tentar abrir o login
            }

            // Validacao de Licenca e Bloqueio Automatico
            MasterServicePro.Services.LicenseCheckResult licResult = null;
            try
            {
                licResult = MasterServicePro.Services.LicenseService.CheckLicenseAsync().GetAwaiter().GetResult();
                if (!licResult.Success || licResult.Status != "active")
                {
                    using (var frmLic = new Forms.FrmLicencaWeb(licResult.Chave, licResult.Status == "expired", licResult.VencimentoBr))
                    {
                        if (frmLic.ShowDialog() != DialogResult.OK)
                        {
                            return; // Encerra o aplicativo se a licenca nao for liberada
                        }
                    }
                    // Re-check license after activation
                    licResult = MasterServicePro.Services.LicenseService.CheckLicenseAsync().GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Falha ao checar licenca do sistema: " + ex.Message, "Licenciamento", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            using (var login = new Forms.FrmLoginWeb(licResult))
            {
                if (login.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new Forms.FrmDashboard());
                }
            }
        }
    }
}
