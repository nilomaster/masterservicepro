namespace MasterServicePro.Forms
{
    partial class FrmLogin
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
            this.pnlLogin = new System.Windows.Forms.Panel();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.lblError = new System.Windows.Forms.Label();
            this.btnLogin = new System.Windows.Forms.Button();
            this.txtSenha = new System.Windows.Forms.TextBox();
            this.lblSenha = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.pnlLogin.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLogin
            // 
            this.pnlLogin.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pnlLogin.Controls.Add(this.picLogo);
            this.pnlLogin.Controls.Add(this.lblError);
            this.pnlLogin.Controls.Add(this.btnLogin);
            this.pnlLogin.Controls.Add(this.txtSenha);
            this.pnlLogin.Controls.Add(this.lblSenha);
            this.pnlLogin.Controls.Add(this.txtUsuario);
            this.pnlLogin.Controls.Add(this.lblUsuario);
            this.pnlLogin.Location = new System.Drawing.Point(50, 50);
            this.pnlLogin.Name = "pnlLogin";
            this.pnlLogin.Padding = new System.Windows.Forms.Padding(30);
            this.pnlLogin.Size = new System.Drawing.Size(350, 400);
            this.pnlLogin.TabIndex = 0;
            // 
            // picLogo
            // 
            this.picLogo.Location = new System.Drawing.Point(30, 10);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(290, 120);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 10;
            this.picLogo.TabStop = false;
            // 
            // lblError
            // 
            this.lblError.ForeColor = System.Drawing.Color.Tomato;
            this.lblError.Location = new System.Drawing.Point(30, 290);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(290, 20);
            this.lblError.TabIndex = 7;
            this.lblError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblError.Visible = false;
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(30, 320);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(290, 45);
            this.btnLogin.TabIndex = 6;
            this.btnLogin.Text = "ENTRAR";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.BtnLogin_Click);
            // 
            // txtSenha
            // 
            this.txtSenha.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSenha.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtSenha.Location = new System.Drawing.Point(30, 240);
            this.txtSenha.Name = "txtSenha";
            this.txtSenha.PasswordChar = '●';
            this.txtSenha.Size = new System.Drawing.Size(290, 29);
            this.txtSenha.TabIndex = 5;
            // 
            // lblSenha
            // 
            this.lblSenha.AutoSize = true;
            this.lblSenha.ForeColor = System.Drawing.Color.Gray;
            this.lblSenha.Location = new System.Drawing.Point(30, 220);
            this.lblSenha.Name = "lblSenha";
            this.lblSenha.Size = new System.Drawing.Size(39, 15);
            this.lblSenha.TabIndex = 4;
            this.lblSenha.Text = "Senha";
            // 
            // txtUsuario
            // 
            this.txtUsuario.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsuario.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtUsuario.Location = new System.Drawing.Point(30, 170);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(290, 29);
            this.txtUsuario.TabIndex = 3;
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.ForeColor = System.Drawing.Color.Gray;
            this.lblUsuario.Location = new System.Drawing.Point(30, 150);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(47, 15);
            this.lblUsuario.TabIndex = 2;
            this.lblUsuario.Text = "Usuário";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.Location = new System.Drawing.Point(410, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(40, 40);
            this.btnClose.TabIndex = 8;
            this.btnClose.Text = "X";
            this.btnClose.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.LightGray;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            
            // 
            // btnMinimize
            // 
            this.btnMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.Location = new System.Drawing.Point(370, 0);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(40, 40);
            this.btnMinimize.TabIndex = 9;
            this.btnMinimize.Text = "—";
            this.btnMinimize.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnMinimize.ForeColor = System.Drawing.Color.LightGray;
            this.btnMinimize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMinimize.Click += new System.EventHandler(this.BtnMinimize_Click);

            // 
            // FrmLogin
            // 
            this.AcceptButton = this.btnLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 500);
            this.Controls.Add(this.btnMinimize);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.pnlLogin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.pnlLogin.ResumeLayout(false);
            this.pnlLogin.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlLogin;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.TextBox txtSenha;
        private System.Windows.Forms.Label lblSenha;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.PictureBox picLogo;
    }
}
