using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using TPV.MODELOAK;

namespace TPV.BISTAK
{
    public partial class FakturaSortu : Form
    {
        private readonly HttpClient bezeroa;
        private readonly int langileId;
        private readonly OdooZerbitzua odooZerbitzua;

        //private const string ApiOinarria = "http://localhost:5093";
        private const string ApiOinarria = "http://192.168.10.5:5093";

        private List<Zerbitzuak> zerbitzuak = new();
        private List<Platerak> platerak = new();

        public FakturaSortu(int langileIdPasatua)
        {
            InitializeComponent();

            bezeroa = new HttpClient();
            langileId = langileIdPasatua;
            odooZerbitzua = new OdooZerbitzua();

            _ = KargatuZerbitzatuak();
        }

        private async Task KargatuZerbitzatuak()
        {
            try
            {
                lblMezua.Text = "Zerbitzuak kargatzen...";
                lblMezua.Visible = true;

                var denak = await bezeroa.GetFromJsonAsync<List<Zerbitzuak>>(
                    ApiOinarria + "/api/Zerbitzuak"
                );

                if (denak != null)
                {
                    zerbitzuak = denak.FindAll(z => z.Egoera != "Ordainduta");
                }
                else
                {
                    zerbitzuak = new List<Zerbitzuak>();
                }

                AzalduZerbitzuak();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Errorea zerbitzuak kargatzean: " + ex.Message,
                    "Errorea",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void AzalduZerbitzuak()
        {
            zerbitzuakPanel.Controls.Clear();

            if (zerbitzuak.Count == 0)
            {
                lblMezua.Text = "Ez dago ordaindu gabeko zerbitzurik.";
                lblMezua.Visible = true;
                zerbitzuakPanel.Controls.Add(lblMezua);
                return;
            }

            lblMezua.Visible = false;

            foreach (var z in zerbitzuak)
            {
                zerbitzuakPanel.Controls.Add(SortuZerbitzuTxartela(z));
            }
        }

        private Control SortuZerbitzuTxartela(Zerbitzuak z)
        {
            var card = new Panel
            {
                Width = 460,
                Height = 150,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(10)
            };

            var lblMahaia = new Label
            {
                Text = $"Mahaia: {z.MahaiaId}",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            var lblPrezioa = new Label
            {
                Text = $"Guztira: {z.Guztira:C2}",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(10, 40),
                AutoSize = true
            };

            var btnOrdaindu = new Button
            {
                Text = "Ordaindu",
                Width = 130,
                Height = 40,
                Location = new Point(10, 80),
                BackColor = Color.Green,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            var btnTicket = new Button
            {
                Text = "Ticket-a Sortu",
                Width = 150,
                Height = 40,
                Location = new Point(150, 80),
                BackColor = Color.FromArgb(45, 85, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnOrdaindu.Click += async (_, __) => await OrdainduKlik(z);
            btnTicket.Click += async (_, __) => await TicketaSortuKlik(z);

            card.Controls.Add(lblMahaia);
            card.Controls.Add(lblPrezioa);
            card.Controls.Add(btnOrdaindu);
            card.Controls.Add(btnTicket);

            return card;
        }

        private async Task OrdainduKlik(Zerbitzuak z)
        {
            if (!OrdainketaBerretsi())
            {
                return;
            }

            try
            {
                var ondo = await OrdainduMarkatu(z);

                if (ondo)
                {
                    await KargatuZerbitzatuak();
                }
                else
                {
                    MessageBox.Show(
                        "Ezin izan da eskaera ordainduta bezala gorde.",
                        "Errorea",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Errorea ordainketa gordetzean: " + ex.Message,
                    "Errorea",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private async Task TicketaSortuKlik(Zerbitzuak z)
        {
            string? deskuntuKodea = null;
            double deskuntuEhunekoa = 0;
            string erroreMezua = string.Empty;
            string aurrekoKodea = string.Empty;

            while (true)
            {
                using var deskuntuDialog = new DeskuntuKodeaDialog(aurrekoKodea, erroreMezua);
                var dialogEmaitza = deskuntuDialog.ShowDialog(this);

                if (dialogEmaitza == DialogResult.Cancel)
                {
                    break;
                }

                aurrekoKodea = deskuntuDialog.Kodea;

                if (string.IsNullOrWhiteSpace(aurrekoKodea))
                {
                    erroreMezua = "Sartu ezazu kode bat!";
                    continue;
                }

                try
                {
                    var balidazioa = await odooZerbitzua.DeskuntuBalidatuAsync(aurrekoKodea);

                    if (!balidazioa.Valid)
                    {
                        erroreMezua = DeskuntuErroreMezua(balidazioa);
                        continue;
                    }

                    deskuntuKodea = aurrekoKodea;
                    deskuntuEhunekoa = balidazioa.Percentage;

                    MessageBox.Show(
                        $"Kodea zuzena da, {deskuntuEhunekoa:0.##}%-ko deskontua duzu!",
                        "Kode zuzena",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    break;
                }
                catch (Exception ex)
                {
                    erroreMezua = $"Errorea deskuntu kodea balidatzean: {ex.Message}";
                }
            }

            try
            {
                var lerroak = await KargatuTicketLerroak(z.Id);
                var azpitotala = KalkulatuAzpitotala(z, lerroak);
                var deskuntua = Math.Round(azpitotala * (decimal)(deskuntuEhunekoa / 100.0), 2);
                var guztiraDeskuntuarekin = azpitotala - deskuntua;

                if (!string.IsNullOrWhiteSpace(deskuntuKodea))
                {
                    var aplikatuRes = await odooZerbitzua.DeskuntuAplikatuAsync(deskuntuKodea, z.Id);

                    if (!aplikatuRes.Valid)
                    {
                        MessageBox.Show(
                            DeskuntuErroreMezua(aplikatuRes),
                            "Abisua",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return;
                    }

                    z.Guztira = guztiraDeskuntuarekin;
                    var eguneratuta = await ZerbitzuaEguneratu(z);

                    if (!eguneratuta)
                    {
                        MessageBox.Show(
                            "Deskuntua balidatu da, baina guztira ezin izan da datu basean eguneratu.",
                            "Abisua",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                }

                SortuEtaIrekiPdfTicket(z, lerroak, azpitotala, deskuntuEhunekoa, deskuntuKodea, deskuntua, guztiraDeskuntuarekin);
                await KargatuZerbitzatuak();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Errorea ticket-a sortzean: " + ex.Message,
                    "Errorea",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private async Task<bool> OrdainduMarkatu(Zerbitzuak z)
        {
            z.Egoera = "Ordainduta";

            var res = await bezeroa.PutAsJsonAsync(
                $"{ApiOinarria}/api/Zerbitzuak/{z.Id}",
                z
            );

            return res.IsSuccessStatusCode;
        }

        private async Task<bool> ZerbitzuaEguneratu(Zerbitzuak z)
        {
            var res = await bezeroa.PutAsJsonAsync(
                $"{ApiOinarria}/api/Zerbitzuak/{z.Id}",
                z
            );

            return res.IsSuccessStatusCode;
        }

        private static string DeskuntuErroreMezua(DeskuntuEmaitza emaitza)
        {
            return string.IsNullOrWhiteSpace(emaitza.Message)
                ? "Kodea ez dago erabilgarri edo ez da existitzen"
                : emaitza.Message;
        }

        private async Task<List<TicketLerroa>> KargatuTicketLerroak(int zerbitzuaId)
        {
            var xehetasunak = await KargatuZerbitzuXehetasunak(zerbitzuaId);

            if (platerak.Count == 0)
            {
                platerak = await bezeroa.GetFromJsonAsync<List<Platerak>>(
                    $"{ApiOinarria}/api/Platerak"
                ) ?? new List<Platerak>();
            }

            var platerMap = platerak.ToDictionary(p => p.Id, p => p.Izena);

            return xehetasunak
                .Where(x => x.Kantitatea > 0)
                .Select(x => new TicketLerroa
                {
                    Kantitatea = x.Kantitatea,
                    Izena = platerMap.TryGetValue(x.PlateraId, out var izena)
                        ? izena
                        : $"Platera {x.PlateraId}",
                    PrezioUnitarioa = x.PrezioUnitarioa
                })
                .ToList();
        }

        private async Task<List<ZerbitzuXehetasunak>> KargatuZerbitzuXehetasunak(int zerbitzuaId)
        {
            var endpointZehatza = $"{ApiOinarria}/api/ZerbitzuXehetasunak/zerbitzua/{zerbitzuaId}";
            using var erantzuna = await bezeroa.GetAsync(endpointZehatza);

            if (erantzuna.IsSuccessStatusCode)
            {
                return await erantzuna.Content.ReadFromJsonAsync<List<ZerbitzuXehetasunak>>()
                    ?? new List<ZerbitzuXehetasunak>();
            }

            var guztiak = await bezeroa.GetFromJsonAsync<List<ZerbitzuXehetasunak>>(
                $"{ApiOinarria}/api/ZerbitzuXehetasunak"
            ) ?? new List<ZerbitzuXehetasunak>();

            return guztiak
                .Where(x => x.ZerbitzuaId == zerbitzuaId)
                .ToList();
        }

        private static decimal KalkulatuAzpitotala(Zerbitzuak z, List<TicketLerroa> lerroak)
        {
            var lerroenGuztira = lerroak.Sum(l => l.Guztira);
            return lerroenGuztira > 0 ? lerroenGuztira : z.Guztira ?? 0m;
        }

        private void SortuEtaIrekiPdfTicket(
            Zerbitzuak z,
            List<TicketLerroa> lerroak,
            decimal azpitotala,
            double deskuntuEhunekoa,
            string? deskuntuKodea,
            decimal deskuntua,
            decimal azkenGuztira)
        {
            try
            {
                using var document = new PdfSharpCore.Pdf.PdfDocument();

                var page = document.AddPage();
                var gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(page);

                var fontBold = new PdfSharpCore.Drawing.XFont(
                    "Verdana",
                    20,
                    PdfSharpCore.Drawing.XFontStyle.Bold
                );

                var fontNormal = new PdfSharpCore.Drawing.XFont(
                    "Verdana",
                    12,
                    PdfSharpCore.Drawing.XFontStyle.Regular
                );

                var fontSmall = new PdfSharpCore.Drawing.XFont(
                    "Verdana",
                    10,
                    PdfSharpCore.Drawing.XFontStyle.Regular
                );

                gfx.DrawString(
                    "ABEJ JATETXEA",
                    fontBold,
                    PdfSharpCore.Drawing.XBrushes.Black,
                    new PdfSharpCore.Drawing.XRect(0, 0, page.Width, 50),
                    PdfSharpCore.Drawing.XStringFormats.Center
                );

                gfx.DrawLine(
                    PdfSharpCore.Drawing.XPens.Black,
                    50,
                    60,
                    page.Width - 50,
                    60
                );

                gfx.DrawString(
                    $"Mahaia: {z.MahaiaId}",
                    fontNormal,
                    PdfSharpCore.Drawing.XBrushes.Black,
                    50,
                    90
                );

                gfx.DrawString(
                    $"Data: {DateTime.Now:yyyy-MM-dd HH:mm}",
                    fontNormal,
                    PdfSharpCore.Drawing.XBrushes.Black,
                    50,
                    112
                );

                int yPos = 160;

                foreach (var lerroa in lerroak)
                {
                    gfx.DrawString(
                        lerroa.Kantitatea.ToString(),
                        fontNormal,
                        PdfSharpCore.Drawing.XBrushes.Black,
                        new PdfSharpCore.Drawing.XRect(50, yPos, 35, 20),
                        PdfSharpCore.Drawing.XStringFormats.TopLeft
                    );

                    gfx.DrawString(
                        lerroa.Izena,
                        fontNormal,
                        PdfSharpCore.Drawing.XBrushes.Black,
                        new PdfSharpCore.Drawing.XRect(85, yPos, 230, 20),
                        PdfSharpCore.Drawing.XStringFormats.TopLeft
                    );

                    gfx.DrawString(
                        FormateatuDirua(lerroa.PrezioUnitarioa),
                        fontNormal,
                        PdfSharpCore.Drawing.XBrushes.Black,
                        new PdfSharpCore.Drawing.XRect(315, yPos, 80, 20),
                        PdfSharpCore.Drawing.XStringFormats.TopRight
                    );

                    gfx.DrawString(
                        FormateatuDirua(lerroa.Guztira),
                        fontNormal,
                        PdfSharpCore.Drawing.XBrushes.Black,
                        new PdfSharpCore.Drawing.XRect(410, yPos, 100, 20),
                        PdfSharpCore.Drawing.XStringFormats.TopRight
                    );

                    yPos += 22;
                }

                yPos += 20;

                gfx.DrawString(
                    "TOTAL",
                    fontNormal,
                    PdfSharpCore.Drawing.XBrushes.Black,
                    new PdfSharpCore.Drawing.XRect(50, yPos, 180, 20),
                    PdfSharpCore.Drawing.XStringFormats.TopLeft
                );

                gfx.DrawString(
                    FormateatuDirua(azpitotala),
                    fontNormal,
                    PdfSharpCore.Drawing.XBrushes.Black,
                    new PdfSharpCore.Drawing.XRect(315, yPos, 195, 20),
                    PdfSharpCore.Drawing.XStringFormats.TopRight
                );

                yPos += 32;

                gfx.DrawString(
                    $"Deskuntu kodea: {deskuntuKodea ?? string.Empty}",
                    fontNormal,
                    PdfSharpCore.Drawing.XBrushes.Black,
                    50,
                    yPos
                );

                yPos += 22;

                gfx.DrawString(
                    $"Deskuntua (%{deskuntuEhunekoa:0.##}): -{FormateatuDirua(deskuntua)}",
                    fontNormal,
                    PdfSharpCore.Drawing.XBrushes.Black,
                    50,
                    yPos
                );

                yPos += 35;

                gfx.DrawString(
                    "TOTAL:",
                    fontBold,
                    PdfSharpCore.Drawing.XBrushes.Black,
                    50,
                    yPos
                );

                gfx.DrawString(
                    FormateatuDirua(azkenGuztira),
                    fontBold,
                    PdfSharpCore.Drawing.XBrushes.Black,
                    new PdfSharpCore.Drawing.XRect(250, yPos, 260, 30),
                    PdfSharpCore.Drawing.XStringFormats.TopRight
                );

                yPos += 55;

                gfx.DrawString(
                    "Mila esker zure bisitagatik!",
                    fontSmall,
                    PdfSharpCore.Drawing.XBrushes.Black,
                    new PdfSharpCore.Drawing.XRect(0, yPos, page.Width, 20),
                    PdfSharpCore.Drawing.XStringFormats.Center
                );

                string fitxategia = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    $"Ticket_{z.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                );

                document.Save(fitxategia);

                Process.Start(new ProcessStartInfo(fitxategia)
                {
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Errorea PDFa sortzean: " + ex.Message,
                    "Errorea",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private static string FormateatuDirua(decimal zenbatekoa)
        {
            return zenbatekoa.ToString("0.##", CultureInfo.CurrentCulture) + " €";
        }

        private bool OrdainketaBerretsi()
        {
            using var dialog = new Form
            {
                Text = "Ordainketa baieztatu",
                Width = 520,
                Height = 170,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lbl = new Label
            {
                Text = "Eskaera ordainduta bezala gelditu behar da, zihur zaude ordainketa eginda dagoela?",
                Location = new Point(20, 20),
                Width = 460,
                Height = 45,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            var btnBai = new Button
            {
                Text = "Bai",
                Width = 90,
                Height = 35,
                Location = new Point(295, 85),
                DialogResult = DialogResult.OK
            };

            var btnEz = new Button
            {
                Text = "Ez",
                Width = 90,
                Height = 35,
                Location = new Point(390, 85),
                DialogResult = DialogResult.Cancel
            };

            dialog.Controls.Add(lbl);
            dialog.Controls.Add(btnBai);
            dialog.Controls.Add(btnEz);
            dialog.AcceptButton = btnBai;
            dialog.CancelButton = btnEz;

            return dialog.ShowDialog(this) == DialogResult.OK;
        }

        private class TicketLerroa
        {
            public int Kantitatea { get; set; }
            public string Izena { get; set; } = string.Empty;
            public decimal PrezioUnitarioa { get; set; }
            public decimal Guztira => PrezioUnitarioa * Kantitatea;
        }
    }
}
