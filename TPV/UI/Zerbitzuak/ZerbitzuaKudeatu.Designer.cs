using System.Drawing;
using System.Windows.Forms;

namespace TPV.BISTAK
{
    partial class ZerbitzuaKudeatu
    {
        private System.ComponentModel.IContainer osagaiak = null;
        private TableLayoutPanel nagusiaLayout;
        private Panel buruaPanel;
        private Label lblTitulua;
        private FlowLayoutPanel kategoriakPanel;
        private Button btnEskaeraAmaitu;

        protected override void Dispose(bool disposing)
        {
            if (disposing && osagaiak != null) osagaiak.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            nagusiaLayout = new TableLayoutPanel();
            buruaPanel = new Panel();
            lblTitulua = new Label();
            kategoriakPanel = new FlowLayoutPanel();
            btnEskaeraAmaitu = new Button();

            nagusiaLayout.SuspendLayout();
            buruaPanel.SuspendLayout();
            SuspendLayout();

            nagusiaLayout.ColumnCount = 1;
            nagusiaLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            nagusiaLayout.Controls.Add(buruaPanel, 0, 0);
            nagusiaLayout.Controls.Add(kategoriakPanel, 0, 1);
            nagusiaLayout.Controls.Add(btnEskaeraAmaitu, 0, 2);
            nagusiaLayout.Dock = DockStyle.Fill;
            nagusiaLayout.RowCount = 3;
            nagusiaLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
            nagusiaLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            nagusiaLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));

            buruaPanel.BackColor = Color.Black;
            buruaPanel.Controls.Add(lblTitulua);
            buruaPanel.Dock = DockStyle.Fill;

            lblTitulua.Dock = DockStyle.Fill;
            lblTitulua.ForeColor = Color.Goldenrod;
            lblTitulua.Text = "Zerbitzua Kudeatu";
            lblTitulua.TextAlign = ContentAlignment.MiddleCenter;

            kategoriakPanel.Dock = DockStyle.Fill;
            kategoriakPanel.AutoScroll = true;
            kategoriakPanel.FlowDirection = FlowDirection.TopDown;
            kategoriakPanel.WrapContents = false;

            btnEskaeraAmaitu.Text = "Eskaera amaitu";
            btnEskaeraAmaitu.BackColor = Color.Goldenrod;
            btnEskaeraAmaitu.ForeColor = Color.Black;
            btnEskaeraAmaitu.FlatStyle = FlatStyle.Flat;
            btnEskaeraAmaitu.Dock = DockStyle.Fill;

            ClientSize = new Size(1000, 700);
            MinimumSize = new Size(900, 600);
            Controls.Add(nagusiaLayout);
            Text = "Zerbitzua Kudeatu";
            WindowState = FormWindowState.Maximized;

            nagusiaLayout.ResumeLayout(false);
            buruaPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
