using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace TPV.BISTAK
{
    partial class InbentarioaKudeatu
    {
        private IContainer components = null;
        private TableLayoutPanel layoutNagusia;
        private Panel headerPanel;
        private Label lblTitulo;
        private FlowLayoutPanel cardsPanel;
        private Panel footerPanel;
        private Button btnGorde;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            layoutNagusia = new TableLayoutPanel();
            headerPanel = new Panel();
            lblTitulo = new Label();
            cardsPanel = new FlowLayoutPanel();
            footerPanel = new Panel();
            btnGorde = new Button();

            SuspendLayout();

            layoutNagusia.ColumnCount = 1;
            layoutNagusia.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutNagusia.RowCount = 3;
            layoutNagusia.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
            layoutNagusia.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutNagusia.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
            layoutNagusia.Dock = DockStyle.Fill;

            headerPanel.BackColor = Color.Black;
            headerPanel.Dock = DockStyle.Fill;
            headerPanel.Controls.Add(lblTitulo);

            lblTitulo.Dock = DockStyle.Fill;
            lblTitulo.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.Goldenrod;
            lblTitulo.Text = "Inbentarioa";
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;

            cardsPanel.Dock = DockStyle.Fill;
            cardsPanel.AutoScroll = true;
            cardsPanel.Padding = new Padding(18);
            cardsPanel.WrapContents = true;
            cardsPanel.FlowDirection = FlowDirection.LeftToRight;

            footerPanel.BackColor = Color.Black;
            footerPanel.Dock = DockStyle.Fill;
            footerPanel.Padding = new Padding(20);

            btnGorde.BackColor = Color.Goldenrod;
            btnGorde.FlatStyle = FlatStyle.Flat;
            btnGorde.FlatAppearance.BorderSize = 0;
            btnGorde.ForeColor = Color.Black;
            btnGorde.Text = "Gorde";
            btnGorde.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            btnGorde.Size = new Size(200, 48);
            btnGorde.Dock = DockStyle.Right;

            footerPanel.Controls.Add(btnGorde);

            layoutNagusia.Controls.Add(headerPanel, 0, 0);
            layoutNagusia.Controls.Add(cardsPanel, 0, 1);
            layoutNagusia.Controls.Add(footerPanel, 0, 2);

            Controls.Add(layoutNagusia);

            BackColor = Color.White;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1000, 700);
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Text = "Inbentarioa Kudeatu";
            Name = "InbentarioaKudeatu";

            ResumeLayout(false);
        }
    }
}
