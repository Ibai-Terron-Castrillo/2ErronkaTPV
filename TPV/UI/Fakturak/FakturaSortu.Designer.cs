using System.Drawing;
using System.Windows.Forms;

namespace TPV.BISTAK
{
    partial class FakturaSortu
    {
        private System.ComponentModel.IContainer components = null;
        private TableLayoutPanel layoutNagusia;
        private Panel headerPanel;
        private Label lblIzenburua;
        private FlowLayoutPanel zerbitzuakPanel;
        private Label lblMezua;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            layoutNagusia = new TableLayoutPanel();
            headerPanel = new Panel();
            lblIzenburua = new Label();
            zerbitzuakPanel = new FlowLayoutPanel();
            lblMezua = new Label();

            layoutNagusia.SuspendLayout();
            headerPanel.SuspendLayout();
            zerbitzuakPanel.SuspendLayout();
            SuspendLayout();

            layoutNagusia.ColumnCount = 1;
            layoutNagusia.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutNagusia.Controls.Add(headerPanel, 0, 0);
            layoutNagusia.Controls.Add(zerbitzuakPanel, 0, 1);
            layoutNagusia.Dock = DockStyle.Fill;
            layoutNagusia.Location = new Point(0, 0);
            layoutNagusia.Name = "layoutNagusia";
            layoutNagusia.RowCount = 2;
            layoutNagusia.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
            layoutNagusia.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutNagusia.Size = new Size(900, 600);
            layoutNagusia.TabIndex = 0;

            headerPanel.BackColor = Color.Black;
            headerPanel.Controls.Add(lblIzenburua);
            headerPanel.Dock = DockStyle.Fill;
            headerPanel.Location = new Point(3, 3);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new Size(894, 84);
            headerPanel.TabIndex = 0;

            lblIzenburua.Dock = DockStyle.Fill;
            lblIzenburua.Font = new System.Drawing.Font("Segoe UI", 20F, FontStyle.Bold);
            lblIzenburua.ForeColor = Color.Goldenrod;
            lblIzenburua.Location = new Point(0, 0);
            lblIzenburua.Name = "lblIzenburua";
            lblIzenburua.Size = new Size(894, 84);
            lblIzenburua.TabIndex = 0;
            lblIzenburua.Text = "Fakturak";
            lblIzenburua.TextAlign = ContentAlignment.MiddleCenter;

            zerbitzuakPanel.AutoScroll = true;
            zerbitzuakPanel.Controls.Add(lblMezua);
            zerbitzuakPanel.Dock = DockStyle.Fill;
            zerbitzuakPanel.Location = new Point(3, 93);
            zerbitzuakPanel.Name = "zerbitzuakPanel";
            zerbitzuakPanel.Size = new Size(894, 504);
            zerbitzuakPanel.TabIndex = 1;

            lblMezua.Dock = DockStyle.Fill;
            lblMezua.Font = new System.Drawing.Font("Segoe UI", 14F, FontStyle.Bold);
            lblMezua.Location = new Point(3, 0);
            lblMezua.Name = "lblMezua";
            lblMezua.Size = new Size(100, 0);
            lblMezua.TabIndex = 0;
            lblMezua.Text = "Zerbitzuak kargatzen...";
            lblMezua.TextAlign = ContentAlignment.MiddleCenter;

            BackColor = Color.White;
            ClientSize = new Size(900, 600);
            Controls.Add(layoutNagusia);
            MinimumSize = new Size(900, 600);
            Name = "FakturaSortu";
            Text = "Fakturak";
            WindowState = FormWindowState.Maximized;

            layoutNagusia.ResumeLayout(false);
            headerPanel.ResumeLayout(false);
            zerbitzuakPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
