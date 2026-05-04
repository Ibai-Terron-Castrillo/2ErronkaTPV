using System.Drawing;
using System.Windows.Forms;

namespace TPV.BISTAK
{
    partial class ZerbitzuaHasi
    {
        private System.ComponentModel.IContainer components = null;
        private TableLayoutPanel layoutNagusia;
        private Panel headerPanel;
        private Label lblTitulo;
        private TableLayoutPanel filtroLayout;
        private ComboBox hourFilter;
        private Button btnBilatu;
        private FlowLayoutPanel erreserbakPanel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            layoutNagusia = new TableLayoutPanel();
            headerPanel = new Panel();
            lblTitulo = new Label();
            filtroLayout = new TableLayoutPanel();
            hourFilter = new ComboBox();
            btnBilatu = new Button();
            erreserbakPanel = new FlowLayoutPanel();
            layoutNagusia.SuspendLayout();
            headerPanel.SuspendLayout();
            filtroLayout.SuspendLayout();
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
            lblTitulo.ForeColor = Color.Goldenrod;
            lblTitulo.Location = new Point(0, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(894, 84);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Gaurko Erreserbak";
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // filtroLayout
            // 
            filtroLayout.ColumnCount = 2;
            filtroLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            filtroLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            filtroLayout.Controls.Add(hourFilter, 0, 0);
            filtroLayout.Controls.Add(btnBilatu, 1, 0);
            filtroLayout.Dock = DockStyle.Fill;
            filtroLayout.Location = new Point(3, 93);
            filtroLayout.Name = "filtroLayout";
            filtroLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            filtroLayout.Size = new Size(894, 64);
            filtroLayout.TabIndex = 1;
            // 
            // hourFilter
            // 
            hourFilter.Dock = DockStyle.Fill;
            hourFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            hourFilter.Location = new Point(3, 3);
            hourFilter.Name = "hourFilter";
            hourFilter.Size = new Size(619, 28);
            hourFilter.TabIndex = 0;
            // 
            // btnBilatu
            // 
            btnBilatu.BackColor = Color.Goldenrod;
            btnBilatu.Dock = DockStyle.Fill;
            btnBilatu.FlatAppearance.BorderSize = 0;
            btnBilatu.FlatStyle = FlatStyle.Flat;
            btnBilatu.ForeColor = Color.Black;
            btnBilatu.Location = new Point(628, 3);
            btnBilatu.Name = "btnBilatu";
            btnBilatu.Size = new Size(263, 58);
            btnBilatu.TabIndex = 1;
            btnBilatu.Text = "Bilatu";
            btnBilatu.UseVisualStyleBackColor = false;
            btnBilatu.Click += btnBilatu_Click;
            // 
            // erreserbakPanel
            // 
            erreserbakPanel.AutoScroll = true;
            erreserbakPanel.Dock = DockStyle.Fill;
            erreserbakPanel.Location = new Point(3, 163);
            erreserbakPanel.Name = "erreserbakPanel";
            erreserbakPanel.Size = new Size(894, 434);
            erreserbakPanel.TabIndex = 2;
            // 
            // ZerbitzuaHasi
            // 
            ClientSize = new Size(900, 600);
            Controls.Add(layoutNagusia);
            Name = "ZerbitzuaHasi";
            Text = "Erreserbak";
            WindowState = FormWindowState.Maximized;
            layoutNagusia.ResumeLayout(false);
            headerPanel.ResumeLayout(false);
            filtroLayout.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
