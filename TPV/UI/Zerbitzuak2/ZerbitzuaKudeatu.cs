using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using TPV.DTOak;
using TPV.MODELOAK;

namespace TPV.BISTAK
{
    public partial class ZerbitzuaKudeatu : Form
    {
        private readonly HttpClient bezeroa;
        private readonly int erreserbaId;
        private readonly int langileId;
        private readonly int mahaiaId;
        //private const string ApiOinarria = "http://localhost:5093";
        private const string ApiOinarria = "http://192.168.10.5:5093";

        private List<Platerak> platerak;
        private List<MODELOAK.PlaterenOsagaiak> platerenosagaiak;
        private List<Inbentarioa> inbentarioa;
        private List<Kategoria> kategoriak;

        private readonly Dictionary<int, int> aukerak = new();
        private readonly Dictionary<int, int> minimoak = new();
        private readonly Dictionary<int, int> hasierakoak = new();
        private readonly Dictionary<int, Panel> panelak = new();

        public ZerbitzuaKudeatu(int eId, int lId, int mId)
        {
            InitializeComponent();
            bezeroa = new HttpClient();
            erreserbaId = eId;
            langileId = lId;
            mahaiaId = mId;

            MinimumSize = new Size(900, 600);

            _ = DenaKargatu();
            btnEskaeraAmaitu.Click += Amaitu;
        }

        private async Task DenaKargatu()
        {
            platerak = await bezeroa.GetFromJsonAsync<List<Platerak>>(ApiOinarria + "/api/Platerak");
            platerenosagaiak = await bezeroa.GetFromJsonAsync<List<MODELOAK.PlaterenOsagaiak>>(ApiOinarria + "/api/PlaterenOsagaiak");
            inbentarioa = await bezeroa.GetFromJsonAsync<List<Inbentarioa>>(ApiOinarria + "/api/Inbentarioa");
            kategoriak = await bezeroa.GetFromJsonAsync<List<Kategoria>>(ApiOinarria + "/api/Kategoria");

            try
            {
                var aurrekoak = await bezeroa.GetFromJsonAsync<List<AurrekoPlatera>>(
                    $"{ApiOinarria}/api/Zerbitzuak/erreserba/{erreserbaId}/platerak"
                );

                foreach (var p in aurrekoak)
                {
                    hasierakoak[p.PlateraId] = p.Kantitatea;
                    aukerak[p.PlateraId] = p.Kantitatea;
                    minimoak[p.PlateraId] = p.Zerbitzatuta ? p.Kantitatea : 0;
                }
            }
            catch { }

            KargatuPlaterak();
            await StockEgiaztatu();
        }

        private void KargatuPlaterak()
        {
            kategoriakPanel.Controls.Clear();
            panelak.Clear();

            foreach (var k in kategoriak.OrderBy(x => x.Id))
            {
                var lblKat = new Label
                {
                    Text = k.Izena,
                    AutoSize = true,
                    Font = new Font("Segoe UI", 16, FontStyle.Bold),
                    Margin = new Padding(10, 20, 10, 5)
                };

                kategoriakPanel.Controls.Add(lblKat);

                var pnlKat = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.LeftToRight,
                    AutoSize = true,
                    WrapContents = true,
                    Margin = new Padding(20, 5, 20, 25)
                };

                foreach (var p in platerak.Where(x => x.KategoriaId == k.Id))
                {
                    if (!aukerak.ContainsKey(p.Id)) aukerak[p.Id] = 0;
                    if (!minimoak.ContainsKey(p.Id)) minimoak[p.Id] = 0;
                    if (!hasierakoak.ContainsKey(p.Id)) hasierakoak[p.Id] = 0;

                    var pnl = new Panel
                    {
                        Width = 200,
                        Height = 300,
                        BorderStyle = BorderStyle.FixedSingle,
                        Margin = new Padding(10)
                    };

                    var pic = new PictureBox
                    {
                        Dock = DockStyle.Top,
                        Height = 120,
                        SizeMode = PictureBoxSizeMode.Zoom
                    };

                    if (!string.IsNullOrEmpty(p.Irudia))
                        pic.LoadAsync(p.Irudia);

                    var lblIzena = new Label
                    {
                        Text = p.Izena,
                        Dock = DockStyle.Top,
                        Height = 30,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Segoe UI", 10, FontStyle.Bold)
                    };

                    var lblPrezio = new Label
                    {
                        Text = p.Prezioa.ToString("0.00") + " €",
                        Dock = DockStyle.Top,
                        Height = 25,
                        TextAlign = ContentAlignment.MiddleCenter
                    };

                    var pnlKontrol = new FlowLayoutPanel
                    {
                        Dock = DockStyle.Bottom,
                        Height = 45
                    };

                    var btnMinus = new Button { Text = "-", Width = 50, Name = "btnMinus" };
                    var txtKop = new Label
                    {
                        Text = aukerak[p.Id].ToString(),
                        Width = 50,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Name = "txtKop"
                    };
                    var btnPlus = new Button { Text = "+", Width = 50, Name = "btnPlus" };

                    btnPlus.Click += async (_, __) =>
                    {
                        await StockEgiaztatu();
                        int stock = GehienezkoStocka(p);
                        int max = stock + hasierakoak[p.Id];
                        if (aukerak[p.Id] < max)
                            aukerak[p.Id]++;
                        await StockEgiaztatu();
                    };

                    btnMinus.Click += async (_, __) =>
                    {
                        await StockEgiaztatu();
                        if (aukerak[p.Id] > minimoak[p.Id])
                            aukerak[p.Id]--;
                        await StockEgiaztatu();
                    };

                    pnlKontrol.Controls.Add(btnMinus);
                    pnlKontrol.Controls.Add(txtKop);
                    pnlKontrol.Controls.Add(btnPlus);

                    pnl.Controls.Add(pnlKontrol);
                    pnl.Controls.Add(lblPrezio);
                    pnl.Controls.Add(lblIzena);
                    pnl.Controls.Add(pic);

                    panelak[p.Id] = pnl;
                    pnlKat.Controls.Add(pnl);
                }

                kategoriakPanel.Controls.Add(pnlKat);
            }
        }

        private async Task StockEgiaztatu()
        {
            inbentarioa = await bezeroa.GetFromJsonAsync<List<Inbentarioa>>(ApiOinarria + "/api/Inbentarioa");

            foreach (var p in platerak)
            {
                if (!panelak.ContainsKey(p.Id)) continue;

                int stock = GehienezkoStocka(p);
                int max = stock + hasierakoak[p.Id];

                var pnl = panelak[p.Id];
                var txt = pnl.Controls.Find("txtKop", true).First() as Label;
                var btnPlus = pnl.Controls.Find("btnPlus", true).First() as Button;
                var btnMinus = pnl.Controls.Find("btnMinus", true).First() as Button;

                if (aukerak[p.Id] > max) aukerak[p.Id] = max;

                txt.Text = aukerak[p.Id].ToString();
                btnMinus.Enabled = aukerak[p.Id] > minimoak[p.Id];
                btnPlus.Enabled = aukerak[p.Id] < max;

                if (stock == 0) pnl.BackColor = Color.Red;
                else if (stock <= 5) pnl.BackColor = Color.Yellow;
                else pnl.BackColor = Color.White;
            }
        }

        private int GehienezkoStocka(Platerak p)
        {
            var osa = platerenosagaiak.Where(o => o.PlateraId == p.Id);
            int max = int.MaxValue;

            foreach (var o in osa)
            {
                var s = inbentarioa.FirstOrDefault(i => i.Id == o.InbentarioaId);
                if (s == null || s.Kantitatea <= 0) return 0;

                int balioa = (int)Math.Floor(s.Kantitatea / o.Kantitatea);
                if (balioa < max) max = balioa;
            }

            return max;
        }

        private async Task EzarriEgoeraEskatutaErreserbarentzat()
        {
            try
            {
                string api = ApiOinarria + "/api/Zerbitzuak";
                var denak = await bezeroa.GetFromJsonAsync<List<Zerbitzuak>>(api) ?? new List<Zerbitzuak>();

                var z = denak
                    .Where(x => x.ErreserbaId == erreserbaId)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefault();

                if (z == null) return;

                if (string.Equals(z.Egoera, "Eskatuta", StringComparison.OrdinalIgnoreCase)) return;

                var body = new
                {
                    langileId = z.LangileId,
                    mahaiaId = z.MahaiaId,
                    erreserbaId = z.ErreserbaId,
                    eskaeraData = z.EskaeraData,
                    egoera = "Eskatuta",
                    guztira = z.Guztira
                };

                await bezeroa.PutAsJsonAsync($"{api}/{z.Id}", body);
            }
            catch
            {
            }
        }

        private async Task<bool> ZiurtatuZerbitzuaBadago()
        {
            try
            {
                string api = ApiOinarria + "/api/Zerbitzuak";

                var denak = await bezeroa.GetFromJsonAsync<List<Zerbitzuak>>(api);
                denak ??= new List<Zerbitzuak>();

                var badago = denak.Any(z => z.ErreserbaId == erreserbaId);
                if (badago) return true;

                var egoeraBalioBat = denak
                    .Select(z => z.Egoera)
                    .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

                var egoeraSaiakerak = new List<string>();
                if (!string.IsNullOrWhiteSpace(egoeraBalioBat)) egoeraSaiakerak.Add(egoeraBalioBat);
                egoeraSaiakerak.Add("Eskatuta");
                egoeraSaiakerak.Add("Egiten");
                egoeraSaiakerak.Add("Zerbitzatuta");
                egoeraSaiakerak.Add("Ordainduta");
                egoeraSaiakerak.Add("Itxaropean");
                egoeraSaiakerak.Add("Eskatutta");

                var saiakerak = new List<object>();

                saiakerak.Add(new
                {
                    langileId = langileId,
                    mahaiaId = mahaiaId,
                    erreserbaId = erreserbaId,
                    eskaeraData = DateTime.Now,
                    guztira = 0m
                });

                foreach (var egoera in egoeraSaiakerak.Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    saiakerak.Add(new
                    {
                        langileId = langileId,
                        mahaiaId = mahaiaId,
                        erreserbaId = erreserbaId,
                        eskaeraData = DateTime.Now,
                        egoera = egoera,
                        guztira = 0m
                    });
                }

                HttpResponseMessage? azkenErantzuna = null;
                string? azkenEdukia = null;

                foreach (var p in saiakerak)
                {
                    azkenErantzuna = await bezeroa.PostAsJsonAsync(api, p);
                    if (azkenErantzuna.IsSuccessStatusCode)
                    {
                        var berrizDenak = await bezeroa.GetFromJsonAsync<List<Zerbitzuak>>(api) ?? new List<Zerbitzuak>();
                        if (berrizDenak.Any(z => z.ErreserbaId == erreserbaId)) return true;
                        continue;
                    }

                    azkenEdukia = await azkenErantzuna.Content.ReadAsStringAsync();
                    if (azkenEdukia.IndexOf("egoera", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        azkenEdukia.IndexOf("Data truncated", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        continue;
                    }

                    break;
                }

                if (azkenErantzuna != null && azkenErantzuna.IsSuccessStatusCode) return true;

                MessageBox.Show("Ezin izan da zerbitzua sortu.\n" + (azkenEdukia ?? "Errore ezezaguna."));
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea: " + ex.Message);
                return false;
            }
        }

        private async void Amaitu(object sender, EventArgs e)
        {
            btnEskaeraAmaitu.Enabled = false;

            try
            {
                var sortuta = await ZiurtatuZerbitzuaBadago();
                if (!sortuta) return;

                var platerak = aukerak.Select(x => new PlateraEskariaDto
                {
                    PlateraId = x.Key,
                    Kantitatea = x.Value
                }).ToList();

                var egoeraSaiakerak = new List<string?> { null, "Eskatuta", "Egiten", "Itxaropean", "Eskatutta" };

                HttpResponseMessage? res = null;
                string? azkenEdukia = null;

                foreach (var egoera in egoeraSaiakerak)
                {
                    object payload = egoera == null
                        ? new
                        {
                            LangileId = langileId,
                            MahaiaId = mahaiaId,
                            ErreserbaId = erreserbaId,
                            Platerak = platerak
                        }
                        : new
                        {
                            LangileId = langileId,
                            MahaiaId = mahaiaId,
                            ErreserbaId = erreserbaId,
                            Egoera = egoera,
                            Platerak = platerak
                        };

                    res = await bezeroa.PostAsJsonAsync(ApiOinarria + "/api/Zerbitzuak/egin", payload);
                    if (res.IsSuccessStatusCode) break;

                    azkenEdukia = await res.Content.ReadAsStringAsync();
                    if (azkenEdukia.IndexOf("egoera", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        azkenEdukia.IndexOf("Data truncated", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        continue;
                    }

                    break;
                }

                if (res == null || !res.IsSuccessStatusCode)
                {
                    MessageBox.Show("Errorea eskaera amaitzean.\n" + (azkenEdukia ?? "Errore ezezaguna."));
                    return;
                }

                MessageBox.Show("Eskaera ondo egin da!");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea: " + ex.Message);
            }
            finally
            {
                if (!IsDisposed) btnEskaeraAmaitu.Enabled = true;
            }
        }
    }
}
