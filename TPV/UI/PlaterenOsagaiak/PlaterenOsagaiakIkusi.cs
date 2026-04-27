using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using TPV.MODELOAK;

namespace TPV.BISTAK
{
    public partial class PlaterenOsagaiakIkusi : Form
    {
        private readonly HttpClient bezeroa;
        private readonly int zerbitzuaId;

        private const string ApiOinarria = "http://192.168.10.5:5093";
        // private const string ApiOinarria = "http://localhost:5093";
        private const string ApiZerbitzuXehetasunak = ApiOinarria + "/api/ZerbitzuXehetasunak";
        private const string ApiPlaterak = ApiOinarria + "/api/Platerak";
        private const string ApiPlaterenOsagaiak = ApiOinarria + "/api/PlaterenOsagaiak";
        private const string ApiInbentarioa = ApiOinarria + "/api/Inbentarioa";

        private List<ZerbitzuXehetasunak> xehetasunak = new();
        private List<Platerak> platerak = new();
        private List<PlaterenOsagaiak> platerOsagaiak = new();
        private List<Inbentarioa> inbentarioa = new();

        private bool kargatzen;

        public PlaterenOsagaiakIkusi(int zerbitzuaIdPasatua)
        {
            InitializeComponent();
            bezeroa = new HttpClient();
            zerbitzuaId = zerbitzuaIdPasatua;

            Shown += async (_, __) => await KargatuDatuak();
        }

        private async Task KargatuDatuak()
        {
            if (kargatzen) return;

            try
            {
                kargatzen = true;
                cardsPanel.Enabled = false;
                cardsPanel.Controls.Clear();

                var t1 = LortuXehetasunak();
                var t2 = bezeroa.GetFromJsonAsync<List<Platerak>>(ApiPlaterak);
                var t3 = bezeroa.GetFromJsonAsync<List<PlaterenOsagaiak>>(ApiPlaterenOsagaiak);
                var t4 = bezeroa.GetFromJsonAsync<List<Inbentarioa>>(ApiInbentarioa);

                await Task.WhenAll(t1, t2, t3, t4);

                platerak = t2.Result ?? new();
                platerOsagaiak = t3.Result ?? new();
                inbentarioa = t4.Result ?? new();

                var lista = xehetasunak
                    .Where(x => x.ZerbitzuaId == zerbitzuaId)
                    .OrderBy(x => x.PlateraId)
                    .ToList();

                if (!lista.Any())
                {
                    cardsPanel.Controls.Add(new Label
                    {
                        Text = "Ez dago platerik zerbitzu honetan.",
                        Width = 700,
                        Height = 80,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Segoe UI", 14F, FontStyle.Bold)
                    });
                    return;
                }

                foreach (var x in lista)
                {
                    var p = platerak.FirstOrDefault(pp => pp.Id == x.PlateraId);
                    cardsPanel.Controls.Add(SortuTxartela(p, x));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea: " + ex.Message);
            }
            finally
            {
                cardsPanel.Enabled = true;
                kargatzen = false;
            }
        }

        private async Task LortuXehetasunak()
        {
            xehetasunak =
                await bezeroa.GetFromJsonAsync<List<ZerbitzuXehetasunak>>(ApiZerbitzuXehetasunak)
                ?? new();
        }

        private Control SortuTxartela(Platerak p, ZerbitzuXehetasunak x)
        {
            var card = new Panel
            {
                Width = 420,
                Height = 460,
                BackColor = Color.White,
                Margin = new Padding(12),
                Padding = new Padding(12),
                BorderStyle = BorderStyle.FixedSingle
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 170F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 65F));

            var irudia = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Gainsboro
            };
            KargatuIrudia(irudia, p);

            var lblIzena = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Text = "Izena: " + (p?.Izena ?? $"Platera {x.PlateraId}")
            };

            var lblKant = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Text = "Kantitatea: " + x.Kantitatea
            };

            var lblOsagai = new TextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Text = "Osagaiak:\r\n" + OsagaiTestua(x.PlateraId),
                MinimumSize = new Size(0, 90)
            };

            var btn = new Button
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btn.FlatAppearance.BorderSize = 0;

            EzarriBotoiEgoera(btn, x.Zerbitzatuta);

            btn.Click += async (_, __) =>
            {
                if (kargatzen) return;

                btn.Enabled = false;
                var aurrekoTestua = btn.Text;
                btn.Text = "Eguneratzen...";

                var berria = !x.Zerbitzatuta;

                var ondo = await ZerbitzuXehetasunaEguneratuSoilik(x.Id, x.ZerbitzuaId, x.PlateraId, x.Kantitatea, x.PrezioUnitarioa, berria);

                if (!ondo)
                {
                    btn.Text = aurrekoTestua;
                    btn.Enabled = true;
                    return;
                }

                await KargatuDatuak();
            };

            layout.Controls.Add(irudia, 0, 0);
            layout.Controls.Add(lblIzena, 0, 1);
            layout.Controls.Add(lblKant, 0, 2);
            layout.Controls.Add(lblOsagai, 0, 3);
            layout.Controls.Add(btn, 0, 4);

            card.Controls.Add(layout);
            return card;
        }

        private void EzarriBotoiEgoera(Button btn, bool eginda)
        {
            btn.Text = eginda ? "EGINDA" : "EGIN GABE";
            btn.BackColor = eginda ? Color.FromArgb(46, 204, 113) : Color.FromArgb(231, 76, 60);
            btn.ForeColor = Color.White;
        }

        private async Task<bool> ZerbitzuXehetasunaEguneratuSoilik(int id, int zerbitzuaId, int plateraId, int kantitatea, decimal prezioUnitarioa, bool zerbitzatuta)
        {
            try
            {
                var body = new
                {
                    zerbitzuaId = zerbitzuaId,
                    plateraId = plateraId,
                    kantitatea = kantitatea,
                    prezioUnitarioa = prezioUnitarioa,
                    zerbitzatuta = zerbitzatuta
                };

                var res = await bezeroa.PutAsJsonAsync($"{ApiZerbitzuXehetasunak}/{id}", body);

                if (!res.IsSuccessStatusCode)
                {
                    var edukia = await res.Content.ReadAsStringAsync();
                    MessageBox.Show("Ezin izan da eguneratu.\n" + edukia);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea: " + ex.Message);
                return false;
            }
        }

        private string OsagaiTestua(int plateraId)
        {
            var ids = platerOsagaiak
                .Where(o => o.PlateraId == plateraId)
                .Select(o => o.InbentarioaId)
                .Distinct();

            var izenak = ids
                .Select(id => inbentarioa.FirstOrDefault(i => i.Id == id)?.Izena)
                .Where(x => !string.IsNullOrWhiteSpace(x));

            return izenak.Any() ? string.Join(", ", izenak) : "Ez dago osagairik.";
        }

        private void KargatuIrudia(PictureBox pb, Platerak p)
        {
            if (p == null) return;

            var prop = p.GetType().GetProperty("Irudia");
            var bal = prop?.GetValue(p)?.ToString();

            if (string.IsNullOrWhiteSpace(bal)) return;

            try
            {
                if (System.IO.File.Exists(bal))
                    pb.Image = Image.FromFile(bal);
                else if (Uri.IsWellFormedUriString(bal, UriKind.Absolute))
                    pb.LoadAsync(bal);
            }
            catch { }
        }
    }
}
