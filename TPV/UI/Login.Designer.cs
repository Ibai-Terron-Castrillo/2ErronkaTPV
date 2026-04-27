namespace TPV
{
    partial class Login
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitulua;
        private Label lblErabiltzailea;
        private Label lblPasahitza;
        private TextBox txtErabiltzailea;
        private TextBox txtPasahitza;
        private Panel headerPanel;
        private TableLayoutPanel mainLayout;
        private TableLayoutPanel formLayout;
        private Button btnLogin;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            headerPanel = new Panel();
            lblTitulua = new Label();
            mainLayout = new TableLayoutPanel();
            formLayout = new TableLayoutPanel();
            lblErabiltzailea = new Label();
            txtErabiltzailea = new TextBox();
            lblPasahitza = new Label();
            txtPasahitza = new TextBox();
            btnLogin = new Button();
            headerPanel.SuspendLayout();
            mainLayout.SuspendLayout();
            formLayout.SuspendLayout();
            SuspendLayout();
            // 
            // headerPanel
            // 
            headerPanel.BackColor = Color.Black;
            headerPanel.Controls.Add(lblTitulua);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Location = new Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new Size(520, 90);
            headerPanel.TabIndex = 1;
            // 
            // lblTitulua
            // 
            lblTitulua.Dock = DockStyle.Fill;
            lblTitulua.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitulua.ForeColor = Color.Goldenrod;
            lblTitulua.Location = new Point(0, 0);
            lblTitulua.Name = "lblTitulua";
            lblTitulua.Size = new Size(520, 90);
            lblTitulua.TabIndex = 0;
            lblTitulua.Text = "ABEJ - Saioa Hasi";
            lblTitulua.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // mainLayout
            // 
            mainLayout.ColumnCount = 3;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            mainLayout.Controls.Add(formLayout, 1, 1);
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Location = new Point(0, 90);
            mainLayout.Name = "mainLayout";
            mainLayout.RowCount = 3;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            mainLayout.Size = new Size(520, 330);
            mainLayout.TabIndex = 0;
            // 
            // formLayout
            // 
            formLayout.ColumnCount = 2;
            formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45.4248352F));
            formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 54.5751648F));
            formLayout.Controls.Add(lblErabiltzailea, 0, 0);
            formLayout.Controls.Add(txtErabiltzailea, 1, 0);
            formLayout.Controls.Add(lblPasahitza, 0, 1);
            formLayout.Controls.Add(txtPasahitza, 1, 1);
            formLayout.Controls.Add(btnLogin, 0, 2);
            formLayout.Dock = DockStyle.Fill;
            formLayout.Location = new Point(107, 69);
            formLayout.Name = "formLayout";
            formLayout.RowCount = 3;
            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            formLayout.Size = new Size(306, 192);
            formLayout.TabIndex = 0;
            // 
            // lblErabiltzailea
            // 
            lblErabiltzailea.Anchor = AnchorStyles.Right;
            lblErabiltzailea.Font = new Font("Segoe UI", 12F);
            lblErabiltzailea.Location = new Point(8, 13);
            lblErabiltzailea.Name = "lblErabiltzailea";
            lblErabiltzailea.Size = new Size(128, 23);
            lblErabiltzailea.TabIndex = 0;
            lblErabiltzailea.Text = "Erabiltzailea:";
            lblErabiltzailea.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtErabiltzailea
            // 
            txtErabiltzailea.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtErabiltzailea.Font = new Font("Segoe UI", 12F);
            txtErabiltzailea.Location = new Point(142, 10);
            txtErabiltzailea.MaximumSize = new Size(250, 34);
            txtErabiltzailea.Name = "txtErabiltzailea";
            txtErabiltzailea.Size = new Size(161, 29);
            txtErabiltzailea.TabIndex = 1;
            // 
            // lblPasahitza
            // 
            lblPasahitza.Anchor = AnchorStyles.Right;
            lblPasahitza.Font = new Font("Segoe UI", 12F);
            lblPasahitza.Location = new Point(36, 63);
            lblPasahitza.Name = "lblPasahitza";
            lblPasahitza.Size = new Size(100, 23);
            lblPasahitza.TabIndex = 2;
            lblPasahitza.Text = "Pasahitza:";
            lblPasahitza.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtPasahitza
            // 
            txtPasahitza.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtPasahitza.Font = new Font("Segoe UI", 12F);
            txtPasahitza.Location = new Point(142, 60);
            txtPasahitza.MaximumSize = new Size(250, 34);
            txtPasahitza.Name = "txtPasahitza";
            txtPasahitza.PasswordChar = '•';
            txtPasahitza.Size = new Size(161, 29);
            txtPasahitza.TabIndex = 3;
            // 
            // btnLogin
            // 
            btnLogin.Anchor = AnchorStyles.Top;
            btnLogin.BackColor = Color.Goldenrod;
            formLayout.SetColumnSpan(btnLogin, 2);
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnLogin.Location = new Point(63, 103);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(180, 45);
            btnLogin.TabIndex = 4;
            btnLogin.Text = "SARTU";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            // 
            // Login
            // 
            BackColor = Color.White;
            ClientSize = new Size(520, 420);
            Controls.Add(mainLayout);
            Controls.Add(headerPanel);
            MinimumSize = new Size(500, 420);
            Name = "Login";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login";
            headerPanel.ResumeLayout(false);
            mainLayout.ResumeLayout(false);
            formLayout.ResumeLayout(false);
            formLayout.PerformLayout();
            ResumeLayout(false);
        }
    }
}
