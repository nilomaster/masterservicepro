using System;
using System.Drawing;
using System.Windows.Forms;
using MasterServicePro.DAL;
using MasterServicePro.Models;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public partial class FrmUsuarios : Form
    {
        private UsuarioRepository repository = new UsuarioRepository();

        public FrmUsuarios()
        {
            InitializeComponent();
            ApplyTheme();
            
            this.Load += (s, e) => LoadData();

            dgvUsuarios.CellDoubleClick += (s, e) => EditarUsuario();
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModalWindowStyle(this);
            this.BackColor = UITheme.BgModal;
            pnlHeader.BackColor = UITheme.BgModalHeader;

            lblTitle.Font = UITheme.FontTitle;
            lblTitle.ForeColor = UITheme.TextTitle;

            UITheme.FormatSaaSButton(btnNovo, true);
            UITheme.FormatSaaSButton(btnEditar, false);
            UITheme.FormatSaaSButton(btnExcluir, false);
            btnExcluir.BackColor = UITheme.Danger;
            btnExcluir.ForeColor = Color.White;

            UITheme.FormatGrid(dgvUsuarios);
            dgvUsuarios.BackgroundColor = UITheme.BgModal;
        }

        private void LoadData()
        {
            try
            {
                var lista = repository.BuscarTodos();
                dgvUsuarios.DataSource = null;
                dgvUsuarios.DataSource = lista;

                if (dgvUsuarios.Columns.Count > 0)
                {
                    foreach (DataGridViewColumn col in dgvUsuarios.Columns) col.Visible = false;

                    if (dgvUsuarios.Columns["Username"] != null)
                    {
                        dgvUsuarios.Columns["Username"].Visible = true;
                        dgvUsuarios.Columns["Username"].HeaderText = "Nome de Usuário";
                        dgvUsuarios.Columns["Username"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }

                    if (dgvUsuarios.Columns["Nivel"] != null)
                    {
                        dgvUsuarios.Columns["Nivel"].Visible = true;
                        dgvUsuarios.Columns["Nivel"].HeaderText = "Nível de Acesso";
                        dgvUsuarios.Columns["Nivel"].Width = 150;
                    }
                }
            }
            catch (Exception ex)
            {
                FrmNotification.ShowError("Erro ao carregar usuários: " + ex.Message, "Erro");
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnNovo_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmUsuarioAddEdit())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void EditarUsuario()
        {
            if (dgvUsuarios.SelectedRows.Count > 0)
            {
                var usuario = (Usuario)dgvUsuarios.SelectedRows[0].DataBoundItem;
                using (var frm = new FrmUsuarioAddEdit(usuario))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            EditarUsuario();
        }

        private void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count > 0)
            {
                var usuario = (Usuario)dgvUsuarios.SelectedRows[0].DataBoundItem;

                if (usuario.Username.ToLower() == "admin")
                {
                    FrmNotification.ShowError("O usuário 'admin' padrão não pode ser desativado!", "Operação Negada");
                    return;
                }

                if (usuario.Id == AuthSession.Id)
                {
                    FrmNotification.ShowError("Você não pode desativar o seu próprio usuário logado!", "Operação Negada");
                    return;
                }

                bool confirm = FrmNotification.ShowConfirm($"Deseja realmente desativar o usuário '{usuario.Username}'?", "Desativar Usuário");
                if (confirm)
                {
                    try
                    {
                        repository.Excluir(usuario.Id);
                        FrmNotification.ShowSuccess("Usuário desativado com sucesso!", "Sucesso");
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        FrmNotification.ShowError("Erro ao desativar usuário: " + ex.Message, "Erro");
                    }
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
