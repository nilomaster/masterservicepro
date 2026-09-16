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

            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ConfigurationSection section = config.GetSection("connectionStrings");
                if (section != null && !section.SectionInformation.IsProtected)
                {
                    section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                    section.SectionInformation.ForceSave = true;
                    config.Save(ConfigurationSaveMode.Modified);
                }
            }
            catch { }

            // Verifica conexao com o banco de dados antes de iniciar o aplicativo
            try
            {
                using (var conn = new MasterServicePro.Utils.DbConnection().GetConnection())
                {
                    conn.Open();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Conexao com banco de dados falhou!!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Sai do aplicativo sem tentar abrir o login
            }

            // Validacao de Licenca e Bloqueio Automatico
            try
            {
                var licResult = MasterServicePro.Services.LicenseService.CheckLicenseAsync().GetAwaiter().GetResult();
                if (!licResult.Success || licResult.Status != "active")
                {
                    using (var frmLic = new Forms.FrmLicencaWeb(licResult.Chave, licResult.Status == "expired", licResult.VencimentoBr))
                    {
                        if (frmLic.ShowDialog() != DialogResult.OK)
                        {
                            return; // Encerra o aplicativo se a licenca nao for liberada
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Falha ao checar licenca do sistema: " + ex.Message, "Licenciamento", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            using (var login = new Forms.FrmLoginWeb())
            {
                if (login.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new Forms.FrmDashboard());
                }
            }
        }
    }
}
