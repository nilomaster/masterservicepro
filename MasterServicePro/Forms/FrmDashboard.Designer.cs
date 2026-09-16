namespace MasterServicePro.Forms
{
    partial class FrmDashboard
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
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.pnlSidebar = new System.Windows.Forms.Panel();

            this.tableLayoutPanelRight = new System.Windows.Forms.TableLayoutPanel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.pnlMain = new System.Windows.Forms.Panel();

            this.tableLayoutPanelMain.SuspendLayout();
            this.pnlSidebar.SuspendLayout();

            this.tableLayoutPanelRight.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();

            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 2;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Controls.Add(this.pnlSidebar, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelRight, 1, 0);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 1;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(1200, 700);
            this.tableLayoutPanelMain.TabIndex = 0;

            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSidebar.Location = new System.Drawing.Point(0, 0);
            this.pnlSidebar.Margin = new System.Windows.Forms.Padding(0);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Size = new System.Drawing.Size(250, 700);
            this.pnlSidebar.TabIndex = 0;


            // 
            // tableLayoutPanelRight
            // 
            this.tableLayoutPanelRight.ColumnCount = 1;
            this.tableLayoutPanelRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelRight.Controls.Add(this.pnlHeader, 0, 0);
            this.tableLayoutPanelRight.Controls.Add(this.pnlMain, 0, 1);
            this.tableLayoutPanelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelRight.Location = new System.Drawing.Point(250, 0);
            this.tableLayoutPanelRight.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelRight.Name = "tableLayoutPanelRight";
            this.tableLayoutPanelRight.RowCount = 2;
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelRight.Size = new System.Drawing.Size(950, 700);
            this.tableLayoutPanelRight.TabIndex = 1;

            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.btnMinimize);
            this.pnlHeader.Controls.Add(this.btnClose);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(950, 60);
            this.pnlHeader.TabIndex = 0;
            
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.Location = new System.Drawing.Point(910, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(40, 60);
            this.btnClose.TabIndex = 1;
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
            this.btnMinimize.Location = new System.Drawing.Point(870, 0);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(40, 60);
            this.btnMinimize.TabIndex = 2;
            this.btnMinimize.Text = "—";
            this.btnMinimize.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnMinimize.ForeColor = System.Drawing.Color.LightGray;
            this.btnMinimize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMinimize.Click += new System.EventHandler(this.BtnMinimize_Click);

            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.lblTitle.Size = new System.Drawing.Size(950, 60);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Dashboard";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // pnlMain
            // 
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(10, 70);
            this.pnlMain.Margin = new System.Windows.Forms.Padding(10);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(930, 620);
            this.pnlMain.TabIndex = 1;

            // 
            // FrmDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Name = "FrmDashboard";
            this.Text = "Master Unlocker - PDV / ERP";
            
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.pnlSidebar.ResumeLayout(false);
            this.tableLayoutPanelRight.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.Panel pnlSidebar;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelRight;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnMinimize;
        public System.Windows.Forms.Panel pnlMain; // Public to allow injecting forms
    }
}
