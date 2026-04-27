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
    public partial class ErreserbaGestionatu : Form
    {
        private readonly HttpClient bezeroa;
        private List<Erreserbak> erreserbak;
        private readonly int langileId;
        private const string ApiOinarria = "http://192.168.10.5:5093";
        // private const string ApiOinarria = "http://localhost:5093";

        public ErreserbaGestionatu(int langileIdPasatua)
        {
            InitializeComponent();
            bezeroa = new HttpClient();
            langileId = langileIdPasatua;
            KargatuOrduFiltroa();
            _ = KargatuEgunekoErreserbak();
        }

        private async Task KargatuEgunekoErreserbak()
        {
            try
            {
                var denak = await bezeroa.GetFromJsonAsync<List<Erreserbak>>($"{ApiOinarria}/api/Erreserbak");

                erreserbak = (denak ?? new List<Erreserbak>())
                    .Where(e => e.ErreserbaData.HasValue && e.ErreserbaData.Value.Date >= DateTime.Today)
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

            if (lista == null || !lista.Any())
            {
                lblMensaje.Text = "Ez da erreserbarik aurkitu.";
                erreserbakPanel.Controls.Add(lblMensaje);
                return;
            }

            foreach (var e in lista)
            {
                var ordua = e.ErreserbaData.HasValue ? e.ErreserbaData.Value.ToString("HH:mm") : "--:--";
                var pertsonak = e.PertsonaKop.HasValue ? e.PertsonaKop.Value.ToString() : "-";

                Button btn = new Button
                {
                    Width = 300,
                    Height = 90,
                    Text = $"{e.Izena}\n{ordua} - {pertsonak} pertsona\n{e.Egoera}",
                    Tag = e
                };

                btn.Click += async (s, a) =>
                {
                    var erreserbaSel = (Erreserbak)btn.Tag;
                    var editatu = new ErreserbaEditatu(erreserbaSel.Id);
                    editatu.FormClosed += async (_, __) => await KargatuEgunekoErreserbak();
                    editatu.Show();
                    await Task.CompletedTask;
                };

                erreserbakPanel.Controls.Add(btn);
            }
        }

        private void KargatuOrduFiltroa()
        {
            hourPicker.Items.Clear();
            string[] orduak = { "13:30", "15:00", "20:00", "21:30", "23:00" };
            foreach (var o in orduak) hourPicker.Items.Add(o);
            hourPicker.SelectedIndex = 0;
        }

        private void btnBilatu_Click(object sender, EventArgs e)
        {
            if (erreserbak == null) return;

            DateTime aukData = datePicker.Value.Date;
            string aukOrdua = hourPicker.SelectedItem?.ToString() ?? "";

            var filtratuta = erreserbak
                .Where(x => x.ErreserbaData.HasValue && x.ErreserbaData.Value.Date == aukData)
                .ToList();

            if (!string.IsNullOrWhiteSpace(aukOrdua))
            {
                filtratuta = filtratuta
                    .Where(x => x.ErreserbaData.HasValue && x.ErreserbaData.Value.ToString("HH:mm") == aukOrdua)
                    .ToList();
            }

            AzalduErreserbak(filtratuta);
        }
    }
}
