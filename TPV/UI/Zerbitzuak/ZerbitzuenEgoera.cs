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
    public partial class ZerbitzuenEgoera : Form
    {
        private readonly HttpClient bezeroa;

        private const string ApiOinarria = "http://192.168.10.5:5093";
        // private const string ApiOinarria = "http://localhost:5093";
        private const string ApiZerbitzuak = ApiOinarria + "/api/Zerbitzuak";
        private const string ApiZerbitzuXehetasunak = ApiOinarria + "/api/ZerbitzuXehetasunak";

        private List<Zerbitzuak> zerbitzuak = new();
        private List<ZerbitzuXehetasunak> xehetasunak = new();

        private bool kargatzen;

        public ZerbitzuenEgoera()
        {
            InitializeComponent();
            bezeroa = new HttpClient();
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

                var t1 = bezeroa.GetFromJsonAsync<List<Zerbitzuak>>(ApiZerbitzuak);
                var t2 = bezeroa.GetFromJsonAsync<List<ZerbitzuXehetasunak>>(ApiZerbitzuXehetasunak);

                await Task.WhenAll(t1, t2);

                zerbitzuak = t1.Result ?? new();
                xehetasunak = t2.Result ?? new();

                var filtratuta = zerbitzuak
                    .Where(z =>
                    {
                        var baditu = xehetasunak.Any(x => x.ZerbitzuaId == z.Id);
                        if (!baditu) return true;
                        return xehetasunak.Any(x => x.ZerbitzuaId == z.Id && x.Zerbitzatuta == false);
                    })
                    .OrderBy(z => z.EskaeraData)
                    .ToList();

                if (!filtratuta.Any())
                {
                    cardsPanel.Controls.Add(new Label
                    {
                        Text = "Ez dago egiteko zerbitzurik.",
                        Width = 700,
                        Height = 80,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Segoe UI", 14F, FontStyle.Bold)
                    });
                    return;
                }

                foreach (var z in filtratuta)
                {
                    cardsPanel.Controls.Add(SortuTxartela(z));
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

        private Control SortuTxartela(Zerbitzuak z)
        {
            var card = new Panel
            {
                Width = 360,
                Height = 230,
                BackColor = Color.White,
                Margin = new Padding(12),
                Padding = new Padding(12),
                BorderStyle = BorderStyle.FixedSingle
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var lblId = SortuLbl("Id");
            var txtId = SortuTxt(z.Id.ToString());

            var lblData = SortuLbl("Eskaera data");
            var txtData = SortuTxt(z.EskaeraData?.ToString("yyyy-MM-dd HH:mm:ss") ?? "");

            var lblEgo = SortuLbl("Egoera");
            var txtEgo = SortuTxt(z.Egoera ?? "");

            var btn = new Button
            {
                Text = "Sukaldatzen hasi",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.Goldenrod,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            btn.FlatAppearance.BorderSize = 0;

            btn.Click += async (_, __) =>
            {
                if (kargatzen) return;

                btn.Enabled = false;
                var aurrekoa = btn.Text;
                btn.Text = "Eguneratzen...";

                using (var leihoa = new PlaterenOsagaiakIkusi(z.Id))
                {
                    leihoa.ShowDialog();
                }

                await KargatuDatuak();
                await KargatuDatuak();
            };

            layout.Controls.Add(lblId, 0, 0);
            layout.Controls.Add(txtId, 1, 0);

            layout.Controls.Add(lblData, 0, 1);
            layout.Controls.Add(txtData, 1, 1);

            layout.Controls.Add(lblEgo, 0, 2);
            layout.Controls.Add(txtEgo, 1, 2);

            layout.Controls.Add(btn, 0, 3);
            layout.SetColumnSpan(btn, 2);

            card.Controls.Add(layout);
            return card;
        }

        private Label SortuLbl(string testua)
        {
            return new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = testua
            };
        }

        private TextBox SortuTxt(string balioa)
        {
            return new TextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Text = balioa ?? ""
            };
        }

        private async Task<bool> EgoeraEgitenEzarri(Zerbitzuak z)
        {
            try
            {
                var body = new
                {
                    langileId = z.LangileId,
                    mahaiaId = z.MahaiaId,
                    erreserbaId = z.ErreserbaId,
                    eskaeraData = z.EskaeraData,
                    egoera = "Egiten",
                    guztira = z.Guztira
                };

                var res = await bezeroa.PutAsJsonAsync($"{ApiZerbitzuak}/{z.Id}", body);

                if (!res.IsSuccessStatusCode)
                {
                    var edukia = await res.Content.ReadAsStringAsync();
                    MessageBox.Show("Ezin izan da zerbitzua eguneratu.\n" + edukia);
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

        private async Task EguneratuZerbitzuarenEgoeraAmaituta(int zerbitzuaId)
        {
            var xehe = xehetasunak
                .Where(x => x.ZerbitzuaId == zerbitzuaId)
                .ToList();

            if (!xehe.Any()) return;

            if (!xehe.All(x => x.Zerbitzatuta)) return;

            var z = zerbitzuak.FirstOrDefault(x => x.Id == zerbitzuaId);
            if (z == null) return;

            try
            {
                var body = new
                {
                    langileId = z.LangileId,
                    mahaiaId = z.MahaiaId,
                    erreserbaId = z.ErreserbaId,
                    eskaeraData = z.EskaeraData,
                    egoera = "Zerbitzatuta",
                    guztira = z.Guztira
                };

                await bezeroa.PutAsJsonAsync($"{ApiZerbitzuak}/{z.Id}", body);
            }
            catch
            {
            }
        }
    }
}
