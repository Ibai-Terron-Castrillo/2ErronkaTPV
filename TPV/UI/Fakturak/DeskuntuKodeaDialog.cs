using System;
using System.Drawing;
using System.Windows.Forms;

namespace TPV.BISTAK
{
    public class DeskuntuKodeaDialog : Form
    {
        private readonly TextBox txtKodea;
        private readonly Label lblErrorea;
        private readonly Button btnBalidatu;
        private readonly Button btnKodeGabe;

        public string Kodea => txtKodea.Text.Trim();

        public DeskuntuKodeaDialog(string aurrekoKodea = "", string erroreMezua = "")
        {
            Text = "Deskuntu kodea";
            Width = 420;
            Height = 240;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var lblGaldera = new Label
            {
                Text = "Deskontu kodea sartu ezazu:",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold)
            };

            txtKodea = new TextBox
            {
                Location = new Point(20, 60),
                Width = 360,
                Text = aurrekoKodea
            };

            lblErrorea = new Label
            {
                Location = new Point(20, 95),
                Width = 360,
                Height = 45,
                ForeColor = Color.Red,
                Text = erroreMezua,
                Visible = !string.IsNullOrWhiteSpace(erroreMezua)
            };

            btnKodeGabe = new Button
            {
                Text = "Ez dut koderik",
                Location = new Point(145, 150),
                Width = 115,
                Height = 35,
                DialogResult = DialogResult.Cancel
            };

            btnBalidatu = new Button
            {
                Text = "Egiaztatu",
                Location = new Point(265, 150),
                Width = 115,
                Height = 35,
                DialogResult = DialogResult.OK
            };

            Controls.Add(lblGaldera);
            Controls.Add(txtKodea);
            Controls.Add(lblErrorea);
            Controls.Add(btnKodeGabe);
            Controls.Add(btnBalidatu);

            AcceptButton = btnBalidatu;
            CancelButton = btnKodeGabe;
        }
    }
}
