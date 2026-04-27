﻿﻿﻿﻿﻿using System.Drawing;
using System.Windows.Forms;

namespace TPV
{
    partial class TxatKontrola
    {
        private System.ComponentModel.IContainer components = null;
        private TableLayoutPanel layoutNagusia;
        private Panel headerPanel;
        private Label lblIzenburua;
        private Label lblEgoera;
        private Panel edukiaPanel;
        private FlowLayoutPanel mezuakPanel;
        private TableLayoutPanel layoutBehea;
        private TextBox txtMezua;
        private Button btnEmoji;
        private Button btnFitxategia;
        private Button btnBidali;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        void InitializeComponent()
        {
            layoutNagusia = new TableLayoutPanel();
            headerPanel = new Panel();
            lblEgoera = new Label();
            lblIzenburua = new Label();
            edukiaPanel = new Panel();
            layoutBehea = new TableLayoutPanel();
            btnEmoji = new Button();
            btnFitxategia = new Button();
            txtMezua = new TextBox();
            btnBidali = new Button();
            mezuakPanel = new FlowLayoutPanel();
            layoutNagusia.SuspendLayout();
            headerPanel.SuspendLayout();
            edukiaPanel.SuspendLayout();
            layoutBehea.SuspendLayout();
            SuspendLayout();
            // 
            // layoutNagusia
            // 
            layoutNagusia.ColumnCount = 1;
            layoutNagusia.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutNagusia.Controls.Add(headerPanel, 0, 0);
            layoutNagusia.Controls.Add(edukiaPanel, 0, 1);
            layoutNagusia.Dock = DockStyle.Fill;
            layoutNagusia.Location = new Point(0, 0);
            layoutNagusia.Name = "layoutNagusia";
            layoutNagusia.RowCount = 2;
            layoutNagusia.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            layoutNagusia.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutNagusia.Size = new Size(380, 650);
            layoutNagusia.TabIndex = 0;
            // 
            // headerPanel
            // 
            headerPanel.BackColor = Color.Black;
            headerPanel.Controls.Add(lblEgoera);
            headerPanel.Controls.Add(lblIzenburua);
            headerPanel.Dock = DockStyle.Fill;
            headerPanel.Location = new Point(3, 3);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new Size(374, 64);
            headerPanel.TabIndex = 0;
            // 
            // lblEgoera
            // 
            lblEgoera.Dock = DockStyle.Right;
            lblEgoera.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblEgoera.ForeColor = Color.Goldenrod;
            lblEgoera.Location = new Point(214, 0);
            lblEgoera.Name = "lblEgoera";
            lblEgoera.Padding = new Padding(0, 0, 16, 0);
            lblEgoera.Size = new Size(160, 64);
            lblEgoera.TabIndex = 0;
            lblEgoera.Text = "Deskonektatuta";
            lblEgoera.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblIzenburua
            // 
            lblIzenburua.Dock = DockStyle.Fill;
            lblIzenburua.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblIzenburua.ForeColor = Color.Goldenrod;
            lblIzenburua.Location = new Point(0, 0);
            lblIzenburua.Name = "lblIzenburua";
            lblIzenburua.Padding = new Padding(16, 0, 16, 0);
            lblIzenburua.Size = new Size(374, 64);
            lblIzenburua.TabIndex = 1;
            lblIzenburua.Text = "Txata";
            lblIzenburua.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // edukiaPanel
            // 
            edukiaPanel.BackColor = Color.White;
            edukiaPanel.Controls.Add(layoutBehea);
            edukiaPanel.Controls.Add(mezuakPanel);
            edukiaPanel.Dock = DockStyle.Fill;
            edukiaPanel.Location = new Point(3, 73);
            edukiaPanel.Name = "edukiaPanel";
            edukiaPanel.Padding = new Padding(12);
            edukiaPanel.Size = new Size(374, 574);
            edukiaPanel.TabIndex = 1;
            // 
            // layoutBehea
            // 
            layoutBehea.ColumnCount = 4;
            layoutBehea.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 44F));
            layoutBehea.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 44F));
            layoutBehea.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutBehea.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            layoutBehea.Controls.Add(btnEmoji, 0, 0);
            layoutBehea.Controls.Add(btnFitxategia, 1, 0);
            layoutBehea.Controls.Add(txtMezua, 2, 0);
            layoutBehea.Controls.Add(btnBidali, 3, 0);
            layoutBehea.Dock = DockStyle.Bottom;
            layoutBehea.Location = new Point(12, 506);
            layoutBehea.Name = "layoutBehea";
            layoutBehea.Padding = new Padding(0, 12, 0, 0);
            layoutBehea.RowCount = 1;
            layoutBehea.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutBehea.Size = new Size(350, 56);
            layoutBehea.TabIndex = 0;
            // 
            // btnEmoji
            // 
            btnEmoji.BackColor = Color.Goldenrod;
            btnEmoji.Dock = DockStyle.Fill;
            btnEmoji.FlatAppearance.BorderSize = 0;
            btnEmoji.FlatStyle = FlatStyle.Flat;
            btnEmoji.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnEmoji.ForeColor = Color.Black;
            btnEmoji.Location = new Point(3, 15);
            btnEmoji.Name = "btnEmoji";
            btnEmoji.Size = new Size(38, 38);
            btnEmoji.TabIndex = 2;
            btnEmoji.Text = "🙂";
            btnEmoji.UseVisualStyleBackColor = false;
            btnEmoji.Click += btnEmoji_Click;
            // 
            // btnFitxategia
            // 
            btnFitxategia.BackColor = Color.Goldenrod;
            btnFitxategia.Dock = DockStyle.Fill;
            btnFitxategia.FlatAppearance.BorderSize = 0;
            btnFitxategia.FlatStyle = FlatStyle.Flat;
            btnFitxategia.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnFitxategia.ForeColor = Color.Black;
            btnFitxategia.Location = new Point(47, 15);
            btnFitxategia.Name = "btnFitxategia";
            btnFitxategia.Size = new Size(38, 38);
            btnFitxategia.TabIndex = 3;
            btnFitxategia.Text = "📎";
            btnFitxategia.UseVisualStyleBackColor = false;
            btnFitxategia.Click += btnFitxategia_Click;
            // 
            // txtMezua
            // 
            txtMezua.BorderStyle = BorderStyle.FixedSingle;
            txtMezua.Dock = DockStyle.Fill;
            txtMezua.Font = new Font("Segoe UI", 11F);
            txtMezua.Location = new Point(91, 15);
            txtMezua.Name = "txtMezua";
            txtMezua.Size = new Size(136, 27);
            txtMezua.TabIndex = 0;
            txtMezua.KeyDown += txtMezua_KeyDown;
            // 
            // btnBidali
            // 
            btnBidali.BackColor = Color.Goldenrod;
            btnBidali.Dock = DockStyle.Fill;
            btnBidali.FlatAppearance.BorderSize = 0;
            btnBidali.FlatStyle = FlatStyle.Flat;
            btnBidali.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnBidali.ForeColor = Color.Black;
            btnBidali.Location = new Point(233, 15);
            btnBidali.Name = "btnBidali";
            btnBidali.Size = new Size(114, 38);
            btnBidali.TabIndex = 1;
            btnBidali.Text = "Bidali";
            btnBidali.UseVisualStyleBackColor = false;
            btnBidali.Click += btnBidali_Click;
            // 
            // mezuakPanel
            // 
            mezuakPanel.AutoScroll = true;
            mezuakPanel.BackColor = Color.White;
            mezuakPanel.Dock = DockStyle.Fill;
            mezuakPanel.FlowDirection = FlowDirection.TopDown;
            mezuakPanel.Location = new Point(12, 12);
            mezuakPanel.Name = "mezuakPanel";
            mezuakPanel.Size = new Size(350, 550);
            mezuakPanel.TabIndex = 1;
            mezuakPanel.WrapContents = false;
            // 
            // TxatKontrola
            // 
            Controls.Add(layoutNagusia);
            Name = "TxatKontrola";
            Size = new Size(380, 650);
            layoutNagusia.ResumeLayout(false);
            headerPanel.ResumeLayout(false);
            edukiaPanel.ResumeLayout(false);
            layoutBehea.ResumeLayout(false);
            layoutBehea.PerformLayout();
            ResumeLayout(false);
        }
    }
}
