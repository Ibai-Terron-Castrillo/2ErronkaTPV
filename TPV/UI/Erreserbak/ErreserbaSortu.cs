using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using TPV.MODELOAK;

namespace TPV
{
    public partial class ErreserbaSortu : Form
    {
        private List<Mahaiak> mahaiaLista;
        private List<Erreserbak> erreserbaLista;
        private readonly HttpClient client = new HttpClient();

        public ErreserbaSortu()
        {
            InitializeComponent();
            mahaiaLista = new List<Mahaiak>();
            erreserbaLista = new List<Erreserbak>();
            datePicker.MinDate = DateTime.Today;
            HasieratuHautaketak();
            datePicker.ValueChanged += (s, e) => HasieratuHautaketak();
        }

        private void HasieratuHautaketak()
        {
            List<string> orduak = new List<string> { "13:30", "15:00", "20:00", "21:30", "23:00" };
            DateTime orain = DateTime.Now;

            string lehenOrdua = orduak.FirstOrDefault(h =>
            {
                var t = TimeSpan.Parse(h);
                return datePicker.Value.Date > orain.Date || t > orain.TimeOfDay;
            }) ?? orduak.Last();

            hourPicker.Items.Clear();
            foreach (var h in orduak)
            {
                var t = TimeSpan.Parse(h);
                if (datePicker.Value.Date > orain.Date || t > orain.TimeOfDay)
                    hourPicker.Items.Add(h);
            }

            hourPicker.SelectedItem = lehenOrdua;
        }

        private async Task KargatuMahaiak()
        {
            if (hourPicker.SelectedItem == null || datePicker.Value == null)
            {
                lblMensaje.Text = "Aukeratu ezazu egun eta ordu bat.";
                mesasPanel.Controls.Clear();
                mesasPanel.Controls.Add(lblMensaje);
                return;
            }

            if (!mahaiaLista.Any()) await LortuMahaiaak();
            if (!erreserbaLista.Any()) await LortuErreserbak();

            string data = datePicker.Value.ToString("yyyy-MM-dd");
            string ordua = hourPicker.SelectedItem.ToString();
            mesasPanel.Controls.Clear();

            var erreserbakHautatutako = erreserbaLista
                .Where(r => r.ErreserbaData.HasValue &&
                            r.ErreserbaData.Value.ToString("yyyy-MM-dd") == data &&
                            r.ErreserbaData.Value.ToString("HH:mm") == ordua)
                .Select(r => r.MahaiaId)
                .ToList();

            var mahaiLibre = mahaiaLista.Where(m => !erreserbakHautatutako.Contains(m.Id)).ToList();

            if (!mahaiLibre.Any())
            {
                lblMensaje.Text = "Ez dago mahaiarik libre ordu horretan.";
                mesasPanel.Controls.Add(lblMensaje);
                return;
            }

            foreach (var m in mahaiLibre)
            {
                var btn = new Button
                {
                    Width = 150,
                    Height = 150,
                    Text = $"Mahaia: {m.Id}\nPertsonak: {m.Edukiera}",
                    Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold),
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = System.Drawing.Color.White,
                    BackColor = m.Edukiera switch
                    {
                        2 => System.Drawing.Color.FromArgb(52, 152, 219),
                        4 => System.Drawing.Color.FromArgb(46, 204, 113),
                        8 => System.Drawing.Color.FromArgb(231, 76, 60),
                        16 => System.Drawing.Color.Purple,
                        _ => System.Drawing.Color.Gray
                    },
                    Tag = m
                };
                btn.Click += (s, e) => IrekiErreserbaLeihoa(m);
                mesasPanel.Controls.Add(btn);
            }
        }

        private void IrekiErreserbaLeihoa(Mahaiak mahaia)
        {
            Panel fondoa = DiseinuErreserbaLeihoa(mahaia);
            Controls.Add(fondoa);
            fondoa.BringToFront();
        }

        private Panel DiseinuErreserbaLeihoa(Mahaiak mahaia)
        {
            Panel fondoa = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.FromArgb(150, 0, 0, 0)
            };

            Panel popup = new Panel
            {
                Width = 400,
                Height = 350,
                BackColor = System.Drawing.Color.Black
            };

            popup.Left = (Width - popup.Width) / 2;
            popup.Top = (Height - popup.Height) / 2;

            Label lblTit = new Label
            {
                Text = $"Erreserba — Mahaia {mahaia.Id}",
                ForeColor = System.Drawing.Color.Goldenrod,
                Dock = DockStyle.Top,
                Height = 50,
                Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };
            popup.Controls.Add(lblTit);

            TextBox txtIzena = new TextBox { PlaceholderText = "Izena", Width = 350, Top = 60, Left = 25 };
            TextBox txtTelefonoa = new TextBox { PlaceholderText = "Telefonoa", Width = 350, Top = 100, Left = 25 };
            ComboBox cmbPertsonaKop = new ComboBox { Width = 350, Top = 140, Left = 25, DropDownStyle = ComboBoxStyle.DropDownList };

            for (int i = 1; i <= mahaia.Edukiera; i++)
                cmbPertsonaKop.Items.Add(i);
            cmbPertsonaKop.SelectedIndex = 0;

            TextBox txtOharrak = new TextBox
            {
                PlaceholderText = "Oharrak (aukerakoa)",
                Width = 350,
                Top = 180,
                Left = 25,
                Height = 60,
                Multiline = true
            };

            Button btnGorde = new Button
            {
                Text = "Gorde",
                Width = 150,
                Height = 40,
                Top = 260,
                Left = 50,
                BackColor = System.Drawing.Color.Goldenrod,
                FlatStyle = FlatStyle.Flat,
                ForeColor = System.Drawing.Color.Black
            };

            Button btnItxi = new Button
            {
                Text = "Itxi",
                Width = 150,
                Height = 40,
                Top = 260,
                Left = 210,
                BackColor = System.Drawing.Color.Gray,
                FlatStyle = FlatStyle.Flat,
                ForeColor = System.Drawing.Color.White
            };

            popup.Controls.Add(txtIzena);
            popup.Controls.Add(txtTelefonoa);
            popup.Controls.Add(cmbPertsonaKop);
            popup.Controls.Add(txtOharrak);
            popup.Controls.Add(btnGorde);
            popup.Controls.Add(btnItxi);
            fondoa.Controls.Add(popup);

            btnItxi.Click += (s, e) => Controls.Remove(fondoa);

            btnGorde.Click += async (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtIzena.Text))
                {
                    MessageBox.Show("Izena bete behar da.");
                    return;
                }
                if (!long.TryParse(txtTelefonoa.Text, out long tel) || txtTelefonoa.Text.Length < 6)
                {
                    MessageBox.Show("Telefonoa ez da baliozkoa.");
                    return;
                }

                DateTime erreserbaData = datePicker.Value.Date + TimeSpan.Parse(hourPicker.SelectedItem.ToString());

                var payload1 = new
                {
                    mahaiaId = mahaia.Id,
                    izena = txtIzena.Text,
                    telefonoa = tel,
                    erreserbaData = erreserbaData,
                    pertsonaKop = (int)cmbPertsonaKop.SelectedItem,
                    egoera = "Konfirmatuta",
                    oharrak = txtOharrak.Text
                };

                try
                {
                    var helbidea = "http://192.168.10.5:5093/api/Erreserbak";

                    var payload2 = new
                    {
                        mahaiaId = mahaia.Id,
                        izena = txtIzena.Text,
                        telefonoa = tel,
                        erreserbaData = erreserbaData,
                        pertsonaKop = (int)cmbPertsonaKop.SelectedItem,
                        oharrak = txtOharrak.Text
                    };

                    var payload3 = new
                    {
                        mahaiaId = mahaia.Id,
                        izena = txtIzena.Text,
                        telefonoa = tel,
                        erreserbaData = erreserbaData,
                        pertsonaKop = (int)cmbPertsonaKop.SelectedItem,
                        egoera = "Itxaropean",
                        oharrak = txtOharrak.Text
                    };

                    var saiakerak = new List<object> { payload1, payload2, payload3 };

                    HttpResponseMessage? res = null;
                    string? azkenEdukia = null;

                    foreach (var p in saiakerak)
                    {
                        res = await client.PostAsJsonAsync(helbidea, p);
                        if (res.IsSuccessStatusCode) break;

                        azkenEdukia = await res.Content.ReadAsStringAsync();
                        if (azkenEdukia.IndexOf("egoera", StringComparison.OrdinalIgnoreCase) < 0 &&
                            azkenEdukia.IndexOf("Data truncated", StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            break;
                        }
                    }

                    if (res == null || !res.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Errorea erreserban.\n" + (azkenEdukia ?? "Ezin izan da erantzuna irakurri."));
                        return;
                    }

                    MessageBox.Show("Erreserba ondo gorde da!");
                    await LortuErreserbak();
                    await KargatuMahaiak();
                    Controls.Remove(fondoa);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Errorea API-ra konektatzean: " + ex.Message);
                }
            };

            return fondoa;
        }

        private async Task LortuMahaiaak()
        {
            try
            {
                //mahaiaLista = await client.GetFromJsonAsync<List<Mahaiak>>("http://localhost:5093/api/Mahaiak");
                mahaiaLista = await client.GetFromJsonAsync<List<Mahaiak>>("http://192.168.10.5:5093/api/Mahaiak");
            }
            catch
            {
                MessageBox.Show("Errorea mahaien API-an.");
            }
        }

        private async Task LortuErreserbak()
        {
            try
            {
                //erreserbaLista = await client.GetFromJsonAsync<List<Erreserbak>>("http://localhost:5093/api/Erreserbak");
                erreserbaLista = await client.GetFromJsonAsync<List<Erreserbak>>("http://192.168.10.5:5093/api/Erreserbak");
            }
            catch
            {
                MessageBox.Show("Errorea erreserben API-an.");
            }
        }

        private async void btnBilatu_Click(object sender, EventArgs e)
        {
            await KargatuMahaiak();
        }
    }
}
