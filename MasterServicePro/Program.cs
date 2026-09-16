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

            // Verifica conexão com o banco de dados antes de iniciar o aplicativo
            try
            {
                using (var conn = new MasterServicePro.Utils.DbConnection().GetConnection())
                {
                    conn.Open();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Conexão com banco de dados falhou!!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Sai do aplicativo sem tentar abrir o login
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
