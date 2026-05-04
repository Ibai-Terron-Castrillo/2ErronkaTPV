using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace TPV.BISTAK
{
    partial class ZerbitzuenEgoera
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
            layoutNagusia.SuspendLayout();
            headerPanel.SuspendLayout();
            SuspendLayout();
            // 
            // layoutNagusia
            // 
            layoutNagusia.ColumnCount = 1;
            layoutNagusia.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutNagusia.Controls.Add(headerPanel, 0, 0);
            layoutNagusia.Controls.Add(cardsPanel, 0, 1);
            layoutNagusia.Dock = DockStyle.Fill;
            layoutNagusia.Location = new Point(0, 0);
            layoutNagusia.Name = "layoutNagusia";
            layoutNagusia.RowCount = 2;
            layoutNagusia.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
            layoutNagusia.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutNagusia.Size = new Size(984, 661);
            layoutNagusia.TabIndex = 0;
            // 
            // headerPanel
            // 
            headerPanel.BackColor = Color.Black;
            headerPanel.Controls.Add(lblTitulo);
            headerPanel.Dock = DockStyle.Fill;
            headerPanel.Location = new Point(3, 3);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new Size(978, 84);
            headerPanel.TabIndex = 0;
            // 
            // lblTitulo
            // 
            lblTitulo.Dock = DockStyle.Fill;
            lblTitulo.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.Goldenrod;
            lblTitulo.Location = new Point(0, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(978, 84);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Zerbitzuen Egoera";
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cardsPanel
            // 
            cardsPanel.AutoScroll = true;
            cardsPanel.Dock = DockStyle.Fill;
            cardsPanel.Location = new Point(3, 93);
            cardsPanel.Name = "cardsPanel";
            cardsPanel.Padding = new Padding(18);
            cardsPanel.Size = new Size(978, 565);
            cardsPanel.TabIndex = 1;
            // 
            // ZerbitzuenEgoera
            // 
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = Color.White;
            ClientSize = new Size(984, 661);
            Controls.Add(layoutNagusia);
            MinimumSize = new Size(1000, 700);
            Name = "ZerbitzuenEgoera";
            Text = "Zerbitzuen Egoera";
            WindowState = FormWindowState.Maximized;
            layoutNagusia.ResumeLayout(false);
            headerPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
