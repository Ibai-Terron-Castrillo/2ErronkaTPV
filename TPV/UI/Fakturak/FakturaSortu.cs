using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using TPV.MODELOAK;

namespace TPV.BISTAK
{
    public partial class FakturaSortu : Form
    {
        private readonly HttpClient bezeroa;
        private readonly int langileId;
        private const string ApiOinarria = "http://192.168.10.5:5093";
        // private const string ApiOinarria = "http://localhost:5093";

        private List<Zerbitzuak> zerbitzuak = new();

        public FakturaSortu(int langileIdPasatua)
        {
            InitializeComponent();
            bezeroa = new HttpClient();
            langileId = langileIdPasatua;
            _ = KargatuZerbitzatuak();
        }

        private async Task KargatuZerbitzatuak()
        {
            try
            {
                var t1 = bezeroa.GetFromJsonAsync<List<Zerbitzuak>>($"{ApiOinarria}/api/Zerbitzuak");
                var t2 = bezeroa.GetFromJsonAsync<List<ZerbitzuXehetasunak>>($"{ApiOinarria}/api/ZerbitzuXehetasunak");

                await Task.WhenAll(t1, t2);

                var denak = t1.Result ?? new List<Zerbitzuak>();
                var xehetasunak = t2.Result ?? new List<ZerbitzuXehetasunak>();

                var fakturaGaiak = xehetasunak
                    .GroupBy(x => x.ZerbitzuaId)
                    .Where(g => g.Any() && g.All(x => x.Zerbitzatuta))
                    .Select(g => g.Key)
                    .ToHashSet();

                zerbitzuak = denak
                    .Where(z =>
                        (string.Equals(z.Egoera, "Zerbitzatuta", StringComparison.OrdinalIgnoreCase) || fakturaGaiak.Contains(z.Id)) &&
                        !string.Equals(z.Egoera, "Ordainduta", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(z => z.Id)
                    .ToList();

                AzalduZerbitzuak(zerbitzuak);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea: " + ex.Message);
            }
        }

        private void AzalduZerbitzuak(List<Zerbitzuak> lista)
        {
            zerbitzuakPanel.Controls.Clear();

            if (lista == null || !lista.Any())
            {
                lblMezua.Text = "Ez dago fakturarako zerbitzurik.";
                zerbitzuakPanel.Controls.Add(lblMezua);
                return;
            }

            foreach (var z in lista)
            {
                zerbitzuakPanel.Controls.Add(SortuZerbitzuTxartela(z));
            }
        }

        private Control SortuZerbitzuTxartela(Zerbitzuak z)
        {
            var txartela = new Panel
            {
                Width = 360,
                Height = 180,
                BackColor = Color.White,
                Margin = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle
            };

            var etiketa = new Label
            {
                Dock = DockStyle.Top,
                Height = 75,
                Font = new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10),
                Text = $"Zerbitzua ID: {z.Id}\nEgoera: {z.Egoera}"
            };

            var btnTicketDeskargatu = new Button
            {
                Dock = DockStyle.Top,
                Height = 45,
                Text = "Ticket-a deskargatu",
                BackColor = Color.Goldenrod,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            btnTicketDeskargatu.FlatAppearance.BorderSize = 0;

            var btnOrdaindu = new Button
            {
                Dock = DockStyle.Top,
                Height = 45,
                Text = "Ordaindu",
                BackColor = Color.Black,
                ForeColor = Color.Goldenrod,
                FlatStyle = FlatStyle.Flat
            };
            btnOrdaindu.FlatAppearance.BorderSize = 0;

            btnTicketDeskargatu.Click += async (_, __) => await TicketDeskargatu(z.Id);
            btnOrdaindu.Click += async (_, __) => await OrdainduMarkatu(z.Id);

            txartela.Controls.Add(btnOrdaindu);
            txartela.Controls.Add(btnTicketDeskargatu);
            txartela.Controls.Add(etiketa);

            return txartela;
        }

        private async Task TicketDeskargatu(int zerbitzuId)
        {
            var erantzuna = MessageBox.Show(
                "Zerbitzuaren faktura deskargatuko da, zihur zaude hau egin nahi duzula?",
                "Baieztatu",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (erantzuna != DialogResult.Yes) return;

            try
            {
                var xehetasunak = await bezeroa.GetFromJsonAsync<List<ZerbitzuXehetasunak>>($"{ApiOinarria}/api/ZerbitzuXehetasunak");

                var lerroak = (xehetasunak ?? new List<ZerbitzuXehetasunak>())
                    .Where(x => x.ZerbitzuaId == zerbitzuId)
                    .ToList();

                if (!lerroak.Any())
                {
                    MessageBox.Show("Ez dago zerbitzu honetako daturik.");
                    return;
                }

                var platerak = await bezeroa.GetFromJsonAsync<List<Platerak>>($"{ApiOinarria}/api/Platerak");
                var platerMapa = (platerak ?? new List<Platerak>())
                    .GroupBy(p => p.Id)
                    .ToDictionary(g => g.Key, g => g.First());

                var itemak = lerroak
                    .GroupBy(l => l.PlateraId)
                    .Select(g =>
                    {
                        var plateraId = g.Key;
                        var kopurua = g.Sum(x => x.Kantitatea);

                        platerMapa.TryGetValue(plateraId, out var p);

                        return new TicketItem
                        {
                            PlateraId = plateraId,
                            Izena = p?.Izena ?? $"Platera {plateraId}",
                            Kantitatea = kopurua,
                            PrezioaUnitatea = p?.Prezioa ?? 0m
                        };
                    })
                    .OrderBy(i => i.Izena)
                    .ToList();

                var mahaigaina = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                var fitxIzena = $"{DateTime.Now:HH.mm.ss-dd-MM-yyyy}-ID{zerbitzuId}.pdf";
                var bidea = System.IO.Path.Combine(mahaigaina, fitxIzena);

                SortuPdfTicket(bidea, zerbitzuId, itemak);

                MessageBox.Show($"PDF-a sortuta:\n{bidea}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea: " + ex.Message);
            }
        }

        private async Task OrdainduMarkatu(int zerbitzuId)
        {
            var erantzuna = MessageBox.Show(
                "Zerbitzua ordainduta bezala markatuko da, seguru zaude zerbitzua ordaindu egin dela?",
                "Baieztatu",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (erantzuna != DialogResult.Yes) return;

            try
            {
                var patchEginDa = await EgoeraPatch(zerbitzuId, "Ordainduta");

                if (!patchEginDa)
                {
                    var zerbitzua = await bezeroa.GetFromJsonAsync<Zerbitzuak>($"{ApiOinarria}/api/Zerbitzuak/{zerbitzuId}");
                    if (zerbitzua == null)
                    {
                        MessageBox.Show("Ezin izan da zerbitzua aurkitu.");
                        return;
                    }

                    zerbitzua.Egoera = "Ordainduta";
                    var put = await bezeroa.PutAsJsonAsync($"{ApiOinarria}/api/Zerbitzuak/{zerbitzuId}", zerbitzua);
                    if (!put.IsSuccessStatusCode)
                    {
                        var edukia = await put.Content.ReadAsStringAsync();
                        MessageBox.Show("Ezin izan da egoera eguneratu.\n" + edukia);
                        return;
                    }
                }

                await KargatuZerbitzatuak();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea: " + ex.Message);
            }
        }

        private async Task<bool> EgoeraPatch(int zerbitzuId, string egoeraBerria)
        {
            try
            {
                var payload = JsonSerializer.Serialize(new { egoera = egoeraBerria });
                var edukia = new StringContent(payload, Encoding.UTF8, "application/json");

                var eskaera = new HttpRequestMessage(new HttpMethod("PATCH"), $"{ApiOinarria}/api/Zerbitzuak/{zerbitzuId}")
                {
                    Content = edukia
                };

                var erantzuna = await bezeroa.SendAsync(eskaera);
                return erantzuna.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private void SortuPdfTicket(string bidea, int zerbitzuId, List<TicketItem> itemak)
        {
            var dokumentua = new PdfSharpCore.Pdf.PdfDocument();
            dokumentua.Info.Title = $"Ticket ID{zerbitzuId}";

            var orria = dokumentua.AddPage();
            orria.Width = 240;  
            orria.Height = 700;

            var marrazkia = PdfSharpCore.Drawing.XGraphics.FromPdfPage(orria);

            var letraTitulua = new PdfSharpCore.Drawing.XFont("Segoe UI", 16, PdfSharpCore.Drawing.XFontStyle.Bold);
            var letraSubTitulo = new PdfSharpCore.Drawing.XFont("Segoe UI", 11, PdfSharpCore.Drawing.XFontStyle.Regular);
            var letraTestua = new PdfSharpCore.Drawing.XFont("Segoe UI", 10, PdfSharpCore.Drawing.XFontStyle.Regular);
            var letraLodia = new PdfSharpCore.Drawing.XFont("Segoe UI", 10, PdfSharpCore.Drawing.XFontStyle.Bold);

            double y = 20;
            double lineHeight = 14;

            void Lerroa(string t, PdfSharpCore.Drawing.XFont f, bool center = false)
            {
                var formatua = center ? PdfSharpCore.Drawing.XStringFormats.TopCenter : PdfSharpCore.Drawing.XStringFormats.TopLeft;

                marrazkia.DrawString(
                    t,
                    f,
                    PdfSharpCore.Drawing.XBrushes.Black,
                    new PdfSharpCore.Drawing.XRect(10, y, orria.Width - 20, 20),
                    formatua
                );

                y += lineHeight;
            }

            Lerroa("ABEJ JATETXEA", letraTitulua, true);
            Lerroa("__________________________________", letraTestua);
            Lerroa($"Ticket ID: {zerbitzuId}", letraSubTitulo);
            Lerroa($"Data: {DateTime.Now:dd/MM/yyyy HH:mm}", letraSubTitulo);
            y += 5;
            Lerroa("------------------------------------------", letraTestua);

            decimal guztira = 0m;

            foreach (var it in itemak)
            {
                var lerroGuztira = it.Kantitatea * it.PrezioaUnitatea;
                guztira += lerroGuztira;

                var izena = it.Izena.Length > 22 ? it.Izena.Substring(0, 22) : it.Izena;
                Lerroa(izena, letraLodia);

                Lerroa($"{it.Kantitatea} x {it.PrezioaUnitatea:0.00}€".PadRight(18) + $"{lerroGuztira:0.00}€", letraTestua);
                y += 3;
            }

            Lerroa("------------------------------------------", letraTestua);
            y += 3;
            Lerroa($"GUZTIRA: {guztira:0.00} €", letraTitulua);
            Lerroa("------------------------------------------", letraTestua);
            y += 10;

            Lerroa("Eskerrik asko!", letraLodia, true);
            Lerroa("Bueltatu nahi duzunean", letraTestua, true);

            dokumentua.Save(bidea);
        }

        private class TicketItem
        {
            public int PlateraId { get; set; }
            public string Izena { get; set; } = "";
            public int Kantitatea { get; set; }
            public decimal PrezioaUnitatea { get; set; }
        }
    }
}
