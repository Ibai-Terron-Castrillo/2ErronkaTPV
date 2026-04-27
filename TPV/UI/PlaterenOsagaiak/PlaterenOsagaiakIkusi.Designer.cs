using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace TPV.BISTAK
{
    partial class PlaterenOsagaiakIkusi
    {
        private IContainer components = null;
        private TableLayoutPanel layoutNagusia;
        private Panel headerPanel;
        private Label lblTitulo;
        private FlowLayoutPanel cardsPanel;

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

            SuspendLayout();

            layoutNagusia.ColumnCount = 1;
            layoutNagusia.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutNagusia.RowCount = 2;
            layoutNagusia.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
            layoutNagusia.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutNagusia.Dock = DockStyle.Fill;

            headerPanel.BackColor = Color.Black;
            headerPanel.Dock = DockStyle.Fill;
            headerPanel.Controls.Add(lblTitulo);

            lblTitulo.Dock = DockStyle.Fill;
            lblTitulo.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.Goldenrod;
            lblTitulo.Text = "Platerak eta Osagaiak";
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;

            cardsPanel.Dock = DockStyle.Fill;
            cardsPanel.AutoScroll = true;
            cardsPanel.Padding = new Padding(18);
            cardsPanel.WrapContents = true;
            cardsPanel.FlowDirection = FlowDirection.LeftToRight;

            layoutNagusia.Controls.Add(headerPanel, 0, 0);
            layoutNagusia.Controls.Add(cardsPanel, 0, 1);

            Controls.Add(layoutNagusia);

            BackColor = Color.White;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1000, 700);
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Text = "Plateren Osagaiak Ikusi";
            Name = "PlaterenOsagaiakIkusi";

            ResumeLayout(false);
        }
    }
}
