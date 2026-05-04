using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using TPV.MODELOAK;

namespace TPV.BISTAK
{
    public partial class ZerbitzuaHasi : Form
    {
        private readonly HttpClient http;
        private List<Erreserbak> erreserbak;
        private readonly int langileId;

        public ZerbitzuaHasi(int langileIdPasatua)
        {
            InitializeComponent();
            http = new HttpClient();
            langileId = langileIdPasatua;
            KargatuOrduFiltroa();
            _ = KargatuGaurkoErreserbak();
        }

        private async Task KargatuGaurkoErreserbak()
        {
            try
            {
                string gaur = DateTime.Now.ToString("yyyy-MM-dd");

                //var denak = await http.GetFromJsonAsync<List<Erreserbak>>("http://localhost:5093/api/Erreserbak");
                var denak = await http.GetFromJsonAsync<List<Erreserbak>>("http://192.168.10.5:5093/api/Erreserbak");

                erreserbak = denak
                    .Where(e => e.ErreserbaData.HasValue && e.ErreserbaData.Value.ToString("yyyy-MM-dd") == gaur)
                    .OrderBy(e => e.ErreserbaData)
                    .ToList();

                AzalduErreserbak(erreserbak);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea: " + ex.Message);
            }
        }

        private void AzalduErreserbak(List<Erreserbak> lista)
        {
            erreserbakPanel.Controls.Clear();

            if (!lista.Any())
            {
                var lbl = new Label
                {
                    Text = "Ez dago erreserbarik.",
                    AutoSize = true,
                    Dock = DockStyle.Top
                };
                erreserbakPanel.Controls.Add(lbl);
                return;
            }

            foreach (var e in lista)
            {
                Button btn = new Button
                {
                    Width = 300,
                    Height = 80,
                    Text = $"{e.Izena}\n{e.ErreserbaData:HH:mm} - {e.PertsonaKop} pertsona",
                    Tag = e
                };
                btn.Click += (s, a) =>
                {
                    var erreserbaSel = (Erreserbak)btn.Tag;
                    var kudeatu = new ZerbitzuaKudeatu(erreserbaSel.Id, langileId, erreserbaSel.MahaiaId);
                    kudeatu.Show();
                };
                erreserbakPanel.Controls.Add(btn);
            }
        }

        private void KargatuOrduFiltroa()
        {
            hourFilter.Items.Clear();
            hourFilter.Items.Add("Guztiak");

            string[] orduak = { "13:30", "15:00", "20:00", "21:30", "23:00" };

            foreach (var o in orduak)
                hourFilter.Items.Add(o);

            hourFilter.SelectedIndex = 0;
        }

        private void btnBilatu_Click(object sender, EventArgs e)
        {
            if (erreserbak == null) return;

            string auk = hourFilter.SelectedItem.ToString();

            var filtratuta = auk == "Guztiak"
                ? erreserbak
                : erreserbak.Where(x => x.ErreserbaData.HasValue && x.ErreserbaData.Value.ToString("HH:mm") == auk).ToList();

            AzalduErreserbak(filtratuta);
        }
    }
}
