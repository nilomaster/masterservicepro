using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public partial class FrmLogin : Form
    {
        private UsuarioRepository repository = new UsuarioRepository();

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        public FrmLogin()
        {
            InitializeComponent();
            try { this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
            ApplyTheme();
            this.MouseDown += FrmLogin_MouseDown;
        }

        private void FrmLogin_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        }

        private void ApplyTheme()
        {
            this.BackColor = UITheme.BgApp;
            pnlLogin.BackColor = UITheme.BgCard;

            txtUsuario.BackColor = UITheme.BgSidebar;
            txtUsuario.ForeColor = UITheme.TextTitle;
            txtSenha.BackColor = UITheme.BgSidebar;
            txtSenha.ForeColor = UITheme.TextTitle;

            try
            {
                string logoPath = System.IO.Path.Combine(Application.StartupPath, "LogoMasterServicePro.png");
                if (System.IO.File.Exists(logoPath))
                    picLogo.Image = Image.FromFile(logoPath);
            }
            catch { }

            UITheme.FormatSaaSButton(btnLogin, true);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (repository.Autenticar(txtUsuario.Text, txtSenha.Text))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                lblError.Text = "Usuário ou senha inválidos!";
                lblError.Visible = true;
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BtnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
