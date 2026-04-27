using System.Drawing;
using System.Windows.Forms;

namespace TPV.BISTAK
{
    partial class ErreserbaGestionatu
    {
        private System.ComponentModel.IContainer components = null;
        private TableLayoutPanel layoutNagusia;
        private Panel headerPanel;
        private Label lblTitulo;
        private TableLayoutPanel filtroLayout;
        private DateTimePicker datePicker;
        private ComboBox hourPicker;
        private Button btnBilatu;
        private FlowLayoutPanel erreserbakPanel;
        private Label lblMensaje;

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
            filtroLayout = new TableLayoutPanel();
            datePicker = new DateTimePicker();
            hourPicker = new ComboBox();
            btnBilatu = new Button();
            erreserbakPanel = new FlowLayoutPanel();
            lblMensaje = new Label();
            layoutNagusia.SuspendLayout();
            headerPanel.SuspendLayout();
            filtroLayout.SuspendLayout();
            erreserbakPanel.SuspendLayout();
            SuspendLayout();
            // 
            // layoutNagusia
            // 
            layoutNagusia.ColumnCount = 1;
            layoutNagusia.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutNagusia.Controls.Add(headerPanel, 0, 0);
            layoutNagusia.Controls.Add(filtroLayout, 0, 1);
            layoutNagusia.Controls.Add(erreserbakPanel, 0, 2);
            layoutNagusia.Dock = DockStyle.Fill;
            layoutNagusia.Location = new Point(0, 0);
            layoutNagusia.Name = "layoutNagusia";
            layoutNagusia.RowCount = 3;
            layoutNagusia.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
            layoutNagusia.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            layoutNagusia.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutNagusia.Size = new Size(900, 600);
            layoutNagusia.TabIndex = 0;
            // 
            // headerPanel
            // 
            headerPanel.BackColor = Color.Black;
            headerPanel.Controls.Add(lblTitulo);
            headerPanel.Dock = DockStyle.Fill;
            headerPanel.Location = new Point(3, 3);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new Size(894, 84);
            headerPanel.TabIndex = 0;
            // 
            // lblTitulo
            // 
            lblTitulo.Dock = DockStyle.Fill;
            lblTitulo.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.Goldenrod;
            lblTitulo.Location = new Point(0, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(894, 84);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Erreserbak Kudeatu";
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // filtroLayout
            // 
            filtroLayout.ColumnCount = 3;
            filtroLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            filtroLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            filtroLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            filtroLayout.Controls.Add(datePicker, 0, 0);
            filtroLayout.Controls.Add(hourPicker, 1, 0);
            filtroLayout.Controls.Add(btnBilatu, 2, 0);
            filtroLayout.Dock = DockStyle.Fill;
            filtroLayout.Location = new Point(3, 93);
            filtroLayout.Name = "filtroLayout";
            filtroLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            filtroLayout.Size = new Size(894, 64);
            filtroLayout.TabIndex = 1;
            // 
            // datePicker
            // 
            datePicker.CustomFormat = "yyyy-MM-dd";
            datePicker.Dock = DockStyle.Fill;
            datePicker.Format = DateTimePickerFormat.Custom;
            datePicker.Location = new Point(3, 3);
            datePicker.MinDate = new DateTime(2026, 1, 22, 0, 0, 0, 0);
            datePicker.Name = "datePicker";
            datePicker.Size = new Size(351, 23);
            datePicker.TabIndex = 0;
            // 
            // hourPicker
            // 
            hourPicker.Dock = DockStyle.Fill;
            hourPicker.DropDownStyle = ComboBoxStyle.DropDownList;
            hourPicker.Location = new Point(360, 3);
            hourPicker.Name = "hourPicker";
            hourPicker.Size = new Size(351, 23);
            hourPicker.TabIndex = 1;
            // 
            // btnBilatu
            // 
            btnBilatu.BackColor = Color.Goldenrod;
            btnBilatu.Dock = DockStyle.Fill;
            btnBilatu.FlatAppearance.BorderSize = 0;
            btnBilatu.FlatStyle = FlatStyle.Flat;
            btnBilatu.ForeColor = Color.Black;
            btnBilatu.Location = new Point(717, 3);
            btnBilatu.Name = "btnBilatu";
            btnBilatu.Size = new Size(174, 58);
            btnBilatu.TabIndex = 2;
            btnBilatu.Text = "Bilatu";
            btnBilatu.UseVisualStyleBackColor = false;
            btnBilatu.Click += btnBilatu_Click;
            // 
            // erreserbakPanel
            // 
            erreserbakPanel.AutoScroll = true;
            erreserbakPanel.Controls.Add(lblMensaje);
            erreserbakPanel.Dock = DockStyle.Fill;
            erreserbakPanel.Location = new Point(3, 163);
            erreserbakPanel.Name = "erreserbakPanel";
            erreserbakPanel.Size = new Size(894, 434);
            erreserbakPanel.TabIndex = 2;
            // 
            // lblMensaje
            // 
            lblMensaje.Dock = DockStyle.Fill;
            lblMensaje.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblMensaje.Location = new Point(3, 0);
            lblMensaje.Name = "lblMensaje";
            lblMensaje.Size = new Size(100, 0);
            lblMensaje.TabIndex = 0;
            lblMensaje.Text = "Aukeratu ezazu egun eta ordu bat.";
            lblMensaje.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ErreserbaGestionatu
            // 
            BackColor = Color.White;
            ClientSize = new Size(900, 600);
            Controls.Add(layoutNagusia);
            MinimumSize = new Size(900, 600);
            Name = "ErreserbaGestionatu";
            Text = "Erreserbak";
            WindowState = FormWindowState.Maximized;
            layoutNagusia.ResumeLayout(false);
            headerPanel.ResumeLayout(false);
            filtroLayout.ResumeLayout(false);
            erreserbakPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
