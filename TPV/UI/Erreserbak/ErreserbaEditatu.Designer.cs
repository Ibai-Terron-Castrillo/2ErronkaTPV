using System.Drawing;
using System.Windows.Forms;

namespace TPV.BISTAK
{
    partial class ErreserbaEditatu
    {
        private System.ComponentModel.IContainer components = null;
        private TableLayoutPanel layoutNagusia;
        private Panel headerPanel;
        private Label lblTitulo;
        private TableLayoutPanel edukiaLayout;
        private TableLayoutPanel formularioLayout;
        private Panel footerPanel;
        private Button btnGorde;
        private Button btnEzabatu;
        private Label lblAzkenEgoera;

        private TextBox txtId;
        private NumericUpDown nudMahaiaId;
        private TextBox txtIzena;
        private TextBox txtTelefonoa;
        private DateTimePicker dtpErreserbaData;
        private ComboBox cmbPertsonaKop;
        private ComboBox cmbEgoera;
        private TextBox txtOharrak;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            layoutNagusia = new TableLayoutPanel();
            headerPanel = new Panel();
            lblTitulo = new Label();
            edukiaLayout = new TableLayoutPanel();
            formularioLayout = new TableLayoutPanel();
            footerPanel = new Panel();
            btnGorde = new Button();
            btnEzabatu = new Button();
            lblAzkenEgoera = new Label();

            txtId = new TextBox();
            nudMahaiaId = new NumericUpDown();
            txtIzena = new TextBox();
            txtTelefonoa = new TextBox();
            dtpErreserbaData = new DateTimePicker();
            cmbPertsonaKop = new ComboBox();
            cmbEgoera = new ComboBox();
            txtOharrak = new TextBox();

            layoutNagusia.SuspendLayout();
            headerPanel.SuspendLayout();
            edukiaLayout.SuspendLayout();
            formularioLayout.SuspendLayout();
            footerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudMahaiaId).BeginInit();
            SuspendLayout();

            layoutNagusia.ColumnCount = 1;
            layoutNagusia.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutNagusia.RowCount = 3;
            layoutNagusia.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
            layoutNagusia.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutNagusia.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
            layoutNagusia.Dock = DockStyle.Fill;

            layoutNagusia.Controls.Add(headerPanel, 0, 0);
            layoutNagusia.Controls.Add(edukiaLayout, 0, 1);
            layoutNagusia.Controls.Add(footerPanel, 0, 2);

            headerPanel.BackColor = Color.Black;
            headerPanel.Dock = DockStyle.Fill;
            headerPanel.Controls.Add(lblTitulo);

            lblTitulo.Dock = DockStyle.Fill;
            lblTitulo.Font = new System.Drawing.Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.Goldenrod;
            lblTitulo.Text = "Erreserba Editatu";
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;

            edukiaLayout.ColumnCount = 1;
            edukiaLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            edukiaLayout.RowCount = 1;
            edukiaLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            edukiaLayout.Dock = DockStyle.Fill;
            edukiaLayout.Padding = new Padding(20);
            edukiaLayout.Controls.Add(formularioLayout, 0, 0);

            formularioLayout.ColumnCount = 2;
            formularioLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28F));
            formularioLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 72F));
            formularioLayout.Dock = DockStyle.Top;
            formularioLayout.AutoSize = true;
            formularioLayout.GrowStyle = TableLayoutPanelGrowStyle.AddRows;

            GehituEremua("Id", txtId, false);
            GehituEremua("Mahaia Id", nudMahaiaId, false);
            GehituEremua("Izena", txtIzena, true);
            GehituEremua("Telefonoa", txtTelefonoa, true);
            GehituEremua("Data eta ordua", dtpErreserbaData, true);
            GehituEremua("Pertsona kop", cmbPertsonaKop, true);
            GehituEremua("Egoera", cmbEgoera, true);
            GehituEremua("Oharrak", txtOharrak, true, true);

            txtId.Dock = DockStyle.Fill;
            txtId.ReadOnly = true;

            nudMahaiaId.Dock = DockStyle.Fill;
            nudMahaiaId.Minimum = 1;
            nudMahaiaId.Maximum = 9999;
            nudMahaiaId.Enabled = false;

            txtIzena.Dock = DockStyle.Fill;

            txtTelefonoa.Dock = DockStyle.Fill;

            dtpErreserbaData.Dock = DockStyle.Fill;
            dtpErreserbaData.CustomFormat = "yyyy-MM-dd HH:mm";
            dtpErreserbaData.Format = DateTimePickerFormat.Custom;
            dtpErreserbaData.ShowUpDown = true;

            cmbPertsonaKop.Dock = DockStyle.Fill;
            cmbPertsonaKop.DropDownStyle = ComboBoxStyle.DropDownList;

            cmbEgoera.Dock = DockStyle.Fill;
            cmbEgoera.DropDownStyle = ComboBoxStyle.DropDownList;

            txtOharrak.Dock = DockStyle.Fill;
            txtOharrak.Multiline = true;
            txtOharrak.ScrollBars = ScrollBars.Vertical;
            txtOharrak.Height = 120;

            footerPanel.BackColor = Color.Black;
            footerPanel.Dock = DockStyle.Fill;

            btnGorde.BackColor = Color.Goldenrod;
            btnGorde.FlatStyle = FlatStyle.Flat;
            btnGorde.FlatAppearance.BorderSize = 0;
            btnGorde.ForeColor = Color.Black;
            btnGorde.Text = "Gorde";
            btnGorde.Width = 180;
            btnGorde.Height = 45;
            btnGorde.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            btnGorde.Click += btnGorde_Click;

            btnEzabatu.BackColor = Color.Goldenrod;
            btnEzabatu.FlatStyle = FlatStyle.Flat;
            btnEzabatu.FlatAppearance.BorderSize = 0;
            btnEzabatu.ForeColor = Color.Black;
            btnEzabatu.Text = "Ezabatu";
            btnEzabatu.Width = 180;
            btnEzabatu.Height = 45;
            btnEzabatu.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            btnEzabatu.Click += btnEzabatu_Click;

            lblAzkenEgoera.Dock = DockStyle.Fill;
            lblAzkenEgoera.Font = new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold);
            lblAzkenEgoera.ForeColor = Color.Goldenrod;
            lblAzkenEgoera.TextAlign = ContentAlignment.MiddleCenter;
            lblAzkenEgoera.Text = "Prest";

            var footerLayout = new TableLayoutPanel();
            footerLayout.ColumnCount = 3;
            footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            footerLayout.Dock = DockStyle.Fill;
            footerLayout.Padding = new Padding(20);

            footerLayout.Controls.Add(btnEzabatu, 0, 0);
            footerLayout.Controls.Add(lblAzkenEgoera, 1, 0);
            footerLayout.Controls.Add(btnGorde, 2, 0);

            footerPanel.Controls.Add(footerLayout);

            BackColor = Color.White;
            ClientSize = new Size(1100, 700);
            Controls.Add(layoutNagusia);
            MinimumSize = new Size(900, 600);
            Name = "ErreserbaEditatu";
            Text = "Erreserba Editatu";
            WindowState = FormWindowState.Maximized;

            layoutNagusia.ResumeLayout(false);
            headerPanel.ResumeLayout(false);
            edukiaLayout.ResumeLayout(false);
            edukiaLayout.PerformLayout();
            formularioLayout.ResumeLayout(false);
            footerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)nudMahaiaId).EndInit();
            ResumeLayout(false);
        }

        private void GehituEremua(string etiketa, Control kontrola, bool enabled, bool multiline = false)
        {
            int row = formularioLayout.RowCount;
            formularioLayout.RowCount = row + 1;
            formularioLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var lbl = new Label();
            lbl.Text = etiketa;
            lbl.Dock = DockStyle.Fill;
            lbl.Font = new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold);
            lbl.TextAlign = ContentAlignment.MiddleLeft;

            kontrola.Enabled = enabled;
            kontrola.Margin = new Padding(3, 8, 3, 8);

            if (multiline && kontrola is TextBox tb)
            {
                tb.Multiline = true;
            }

            formularioLayout.Controls.Add(lbl, 0, row);
            formularioLayout.Controls.Add(kontrola, 1, row);
        }
    }
}
