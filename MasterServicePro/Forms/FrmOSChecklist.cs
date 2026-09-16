using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MasterServicePro.Utils;

namespace MasterServicePro.Forms
{
    public partial class FrmOSChecklist : Form
    {
        public string ChecklistResult { get; private set; }

        private static readonly string[] ItemsToTest = new[]
        {
            "Touchscreen / Display",
            "Vidro / Carcaça",
            "Câmera Frontal",
            "Câmera Traseira",
            "Conector de Carga",
            "Microfone",
            "Alto-falante",
            "Botões Físicos",
            "Wi-Fi / Bluetooth",
            "Sinal de Chip",
            "Biometria / FaceID",
            "Bateria / Saúde"
        };

        private Dictionary<string, string> _states = new Dictionary<string, string>();
        private Dictionary<string, Button> _buttons = new Dictionary<string, Button>();
        private TextBox txtObs;

        public FrmOSChecklist(string initialData = "")
        {
            InitializeComponent();
            LoadStates(initialData);
            ApplyTheme();
            CreateChecklistUI();
            CreateCustomTitleBar();
        }

        // Win32 API para arrastar a janela
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void CreateCustomTitleBar()
        {
            Panel pnlTitleBar = new Panel();
            Label lblTitleBar = new Label();
            Button btnClose = new Button();
            PictureBox picLogo = new PictureBox();

            pnlTitleBar.BackColor = System.Drawing.Color.FromArgb(26, 29, 36);
            pnlTitleBar.Dock = DockStyle.Top;
            pnlTitleBar.Height = 40;
            pnlTitleBar.Controls.Add(lblTitleBar);
            pnlTitleBar.Controls.Add(btnClose);
            pnlTitleBar.Controls.Add(picLogo);
            pnlTitleBar.MouseDown += TitleBar_MouseDown;

            btnClose.Dock = DockStyle.Right;
            btnClose.Width = 45;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.ForeColor = System.Drawing.Color.White;
            btnClose.Text = "X";
            btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => this.Close();
            btnClose.MouseEnter += (s, e) => btnClose.BackColor = System.Drawing.Color.FromArgb(232, 17, 35);
            btnClose.MouseLeave += (s, e) => btnClose.BackColor = System.Drawing.Color.Transparent;

            picLogo.Dock = DockStyle.Left;
            picLogo.Width = 40;
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.Padding = new Padding(8);
            try
            {
                var appIcon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                this.Icon = appIcon;
                picLogo.Image = appIcon.ToBitmap();
            }
            catch { }
            picLogo.MouseDown += TitleBar_MouseDown;

            lblTitleBar.Dock = DockStyle.Fill;
            lblTitleBar.ForeColor = System.Drawing.Color.White;
            lblTitleBar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            lblTitleBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lblTitleBar.Text = "Checklist Técnico";
            lblTitleBar.MouseDown += TitleBar_MouseDown;

            this.Controls.Add(pnlTitleBar);
            pnlMain.Location = new Point(20, 50);
        }

        private void LoadStates(string data)
        {
            // Carrega estados iniciais ou define padrão
            foreach (var item in ItemsToTest)
            {
                _states[item] = "Não Testado";
            }

            string obs = "";
            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    string mainPart = data;
                    int obsIdx = data.IndexOf("[OBS]");
                    if (obsIdx >= 0)
                    {
                        mainPart = data.Substring(0, obsIdx);
                        obs = data.Substring(obsIdx + 5);
                    }

                    var pairs = mainPart.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var p in pairs)
                    {
                        var parts = p.Split(new[] { ':' }, 2);
                        if (parts.Length == 2 && _states.ContainsKey(parts[0]))
                        {
                            _states[parts[0]] = parts[1];
                        }
                    }
                }
                catch { }
            }

            _states["[OBS]"] = obs;
        }

        private void ApplyTheme()
        {
            UITheme.ApplyModalWindowStyle(this);
            this.BackColor = UITheme.BgModal;
            pnlMain.BackColor = UITheme.BgCard;

            lblTitle.Font = UITheme.FontTitle;
            lblTitle.ForeColor = UITheme.TextTitle;

            UITheme.FormatSaaSButton(btnSalvar, true);
            UITheme.FormatSaaSButton(btnCancelar, false);
        }

        private void CreateChecklistUI()
        {
            int startY = 70;
            int rowHeight = 45;
            int colWidth = 340;

            for (int i = 0; i < ItemsToTest.Length; i++)
            {
                string itemName = ItemsToTest[i];
                int col = i / 6; // 0 ou 1
                int row = i % 6; // 0 a 5

                int xLabel = 20 + col * colWidth;
                int xButton = 200 + col * colWidth;
                int y = startY + row * rowHeight;

                Label lbl = new Label
                {
                    Text = itemName,
                    ForeColor = UITheme.TextTitle,
                    Font = UITheme.FontBodyBold,
                    Location = new Point(xLabel, y + 8),
                    Width = 170,
                    Height = 25,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                pnlMain.Controls.Add(lbl);

                Button btn = new Button
                {
                    Location = new Point(xButton, y),
                    Width = 130,
                    Height = 32,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Font = UITheme.FontSmallBold,
                    Tag = itemName
                };
                btn.FlatAppearance.BorderSize = 0;

                UpdateButtonState(btn, _states[itemName]);

                btn.Click += BtnItem_Click;
                pnlMain.Controls.Add(btn);
                _buttons[itemName] = btn;
            }

            // Observações
            Label lblObs = new Label
            {
                Text = "Observações Adicionais / Detalhes Visuais:",
                ForeColor = UITheme.TextMuted,
                Font = UITheme.FontBody,
                Location = new Point(20, startY + 6 * rowHeight + 10),
                Width = 400,
                Height = 20
            };
            pnlMain.Controls.Add(lblObs);

            txtObs = new TextBox
            {
                Multiline = true,
                Location = new Point(20, startY + 6 * rowHeight + 35),
                Width = 660,
                Height = 80,
                BackColor = UITheme.BgSidebar,
                ForeColor = UITheme.TextTitle,
                Font = UITheme.FontBody,
                BorderStyle = BorderStyle.FixedSingle,
                Text = _states["[OBS]"]
            };
            pnlMain.Controls.Add(txtObs);

            // Ajustando a posição dos botões de controle de acordo com o novo conteúdo
            btnSalvar.Location = new Point(480, startY + 6 * rowHeight + 130);
            btnCancelar.Location = new Point(20, startY + 6 * rowHeight + 130);
        }

        private void BtnItem_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string itemName = (string)btn.Tag;
            string currentState = _states[itemName];

            string newState;
            if (currentState == "Não Testado") newState = "OK";
            else if (currentState == "OK") newState = "Defeito";
            else newState = "Não Testado";

            _states[itemName] = newState;
            UpdateButtonState(btn, newState);
        }

        private void UpdateButtonState(Button btn, string state)
        {
            btn.Text = state;
            if (state == "OK")
            {
                btn.BackColor = UITheme.Success;
                btn.ForeColor = Color.White;
            }
            else if (state == "Defeito")
            {
                btn.BackColor = UITheme.Danger;
                btn.ForeColor = Color.White;
            }
            else
            {
                btn.BackColor = UITheme.ElementBorder;
                btn.ForeColor = UITheme.TextMuted;
            }
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            var list = new List<string>();
            foreach (var item in ItemsToTest)
            {
                list.Add($"{item}:{_states[item]}");
            }
            ChecklistResult = string.Join("|", list) + "[OBS]" + txtObs.Text.Trim();
            this.DialogResult = DialogResult.OK;
        }
    }
}
