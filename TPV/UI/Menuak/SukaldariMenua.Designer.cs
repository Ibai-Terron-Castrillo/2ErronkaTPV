using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace TPV
{
    partial class SukaldariMenua
    {
        private IContainer components = null;
        private TableLayoutPanel layoutNagusia;
        private TableLayoutPanel botoiLayout;
        private Panel headerPanel;
        private Button btnInbentarioaKudeatu;
        private Button btnPlaterenEgoera;
        private Button btnItzuli;

        private Panel txatPanela;
        private TxatKontrola txatKontrola;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            layoutNagusia = new TableLayoutPanel();
            botoiLayout = new TableLayoutPanel();
            headerPanel = new Panel();
            btnInbentarioaKudeatu = new Button();
            btnPlaterenEgoera = new Button();
            btnItzuli = new Button();

            txatPanela = new Panel();
            txatKontrola = new TxatKontrola();

            SuspendLayout();

            headerPanel.BackColor = Color.Black;
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 90;
            headerPanel.Controls.Add(btnItzuli);

            btnItzuli.Text = "Zerbitzari Menua";
            btnItzuli.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnItzuli.Size = new Size(220, 45);
            btnItzuli.Location = new Point(15, 22);
            btnItzuli.BackColor = Color.Goldenrod;
            btnItzuli.ForeColor = Color.Black;
            btnItzuli.FlatStyle = FlatStyle.Flat;
            btnItzuli.FlatAppearance.BorderSize = 0;
            btnItzuli.Visible = false;

            layoutNagusia.Dock = DockStyle.Fill;
            layoutNagusia.RowCount = 2;
            layoutNagusia.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
            layoutNagusia.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            botoiLayout.Dock = DockStyle.Fill;
            botoiLayout.ColumnCount = 3;
            botoiLayout.RowCount = 5;
            botoiLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            botoiLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            botoiLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            for (int i = 0; i < 5; i++)
                botoiLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));

            EstiloaEzarri(btnInbentarioaKudeatu, "Inbentarioa kudeatu");
            EstiloaEzarri(btnPlaterenEgoera, "Zerbitzuaren egoera");

            botoiLayout.Controls.Add(btnInbentarioaKudeatu, 1, 1);
            botoiLayout.Controls.Add(btnPlaterenEgoera, 1, 2);

            layoutNagusia.Controls.Add(headerPanel, 0, 0);
            layoutNagusia.Controls.Add(botoiLayout, 0, 1);

            txatPanela.Dock = DockStyle.Right;
            txatPanela.Width = 420;
            txatPanela.BackColor = Color.White;
            txatPanela.Padding = new Padding(12);

            txatKontrola.Dock = DockStyle.Fill;

            txatPanela.Controls.Add(txatKontrola);

            Controls.Add(layoutNagusia);
            Controls.Add(txatPanela);

            BackColor = Color.White;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(800, 600);
            Text = "Sukaldari Menua";

            ResumeLayout(false);
        }

        private void EstiloaEzarri(Button btn, string testua)
        {
            btn.Text = testua;
            btn.Dock = DockStyle.Fill;
            btn.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            btn.BackColor = Color.Goldenrod;
            btn.ForeColor = Color.Black;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
        }
    }
}
