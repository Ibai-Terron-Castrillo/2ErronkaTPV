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
    public partial class ErreserbaEditatu : Form
    {
        private readonly HttpClient bezeroa;
        private readonly int erreserbaId;
        private Erreserbak erreserba;
        private List<Mahaiak> mahaiaLista;
        private const string ApiOinarria = "http://192.168.10.5:5093";
        // private const string ApiOinarria = "http://localhost:5093";

        public ErreserbaEditatu(int erreserbaIdPasatua)
        {
            InitializeComponent();
            bezeroa = new HttpClient();
            erreserbaId = erreserbaIdPasatua;
            mahaiaLista = new List<Mahaiak>();
            KargatuEgoerak();
            Shown += async (_, __) => await HasieratuDatuak();
        }

        private async Task HasieratuDatuak()
        {
            await LortuMahaiak();
            await KargatuErreserba();
        }

        private void KargatuEgoerak()
        {
            cmbEgoera.Items.Clear();
            cmbEgoera.Items.Add("Itxaropean");
            cmbEgoera.Items.Add("Konfirmatuta");
            cmbEgoera.Items.Add("Kantzelatuta");
            cmbEgoera.SelectedIndex = 0;
        }

        private async Task LortuMahaiak()
        {
            try
            {
                var datuak = await bezeroa.GetFromJsonAsync<List<Mahaiak>>($"{ApiOinarria}/api/Mahaiak");
                mahaiaLista = datuak ?? new List<Mahaiak>();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea mahaien API-an: " + ex.Message);
                mahaiaLista = new List<Mahaiak>();
            }
        }

        private int LortuEdukiera(int mahaiaId)
        {
            var m = mahaiaLista.FirstOrDefault(x => x.Id == mahaiaId);
            if (m == null) return 50;
            if (m.Edukiera <= 0) return 50;
            return m.Edukiera;
        }

        private void KargatuPertsonaCombo(int edukiera, int? aukeratua)
        {
            cmbPertsonaKop.Items.Clear();

            for (int i = 1; i <= edukiera; i++)
                cmbPertsonaKop.Items.Add(i);

            int auk = aukeratua.HasValue ? aukeratua.Value : 1;
            if (auk < 1) auk = 1;
            if (auk > edukiera) auk = edukiera;

            cmbPertsonaKop.SelectedItem = auk;
            if (cmbPertsonaKop.SelectedIndex < 0) cmbPertsonaKop.SelectedIndex = 0;
        }

        private async Task KargatuErreserba()
        {
            try
            {
                var datua = await bezeroa.GetFromJsonAsync<Erreserbak>($"{ApiOinarria}/api/Erreserbak/{erreserbaId}");

                if (datua == null)
                {
                    MessageBox.Show("Erreserba ez da aurkitu.");
                    Close();
                    return;
                }

                erreserba = datua;
                InprimakianJarri(erreserba);
                lblAzkenEgoera.Text = "Kargatuta";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea: " + ex.Message);
            }
        }

        private void InprimakianJarri(Erreserbak e)
        {
            txtId.Text = e.Id.ToString();

            nudMahaiaId.Value = e.MahaiaId < (int)nudMahaiaId.Minimum ? nudMahaiaId.Minimum : e.MahaiaId;

            var edukiera = LortuEdukiera(e.MahaiaId);
            KargatuPertsonaCombo(edukiera, e.PertsonaKop);

            txtIzena.Text = e.Izena ?? "";
            txtTelefonoa.Text = e.Telefonoa.ToString();

            dtpErreserbaData.Value = e.ErreserbaData ?? DateTime.Now;

            var egoera = string.IsNullOrWhiteSpace(e.Egoera) ? "Itxaropean" : e.Egoera;
            var idx = cmbEgoera.Items.IndexOf(egoera);
            cmbEgoera.SelectedIndex = idx >= 0 ? idx : 0;

            txtOharrak.Text = e.Oharrak ?? "";
        }

        private Erreserbak InprimakitikHartu()
        {
            int telefonoa = 0;
            int.TryParse(txtTelefonoa.Text.Trim(), out telefonoa);

            int kop = 1;
            if (cmbPertsonaKop.SelectedItem != null)
                kop = (int)cmbPertsonaKop.SelectedItem;

            return new Erreserbak
            {
                Id = erreserbaId,
                MahaiaId = erreserba?.MahaiaId ?? (int)nudMahaiaId.Value,
                Izena = txtIzena.Text.Trim(),
                Telefonoa = telefonoa,
                ErreserbaData = dtpErreserbaData.Value,
                PertsonaKop = kop,
                Egoera = cmbEgoera.SelectedItem?.ToString() ?? "Itxaropean",
                Oharrak = string.IsNullOrWhiteSpace(txtOharrak.Text) ? null : txtOharrak.Text
            };
        }

        private bool Balioztatu()
        {
            if (string.IsNullOrWhiteSpace(txtIzena.Text))
            {
                MessageBox.Show("Izena derrigorrezkoa da.");
                return false;
            }

            if (!int.TryParse(txtTelefonoa.Text.Trim(), out var tel) || tel <= 0)
            {
                MessageBox.Show("Telefonoa ez da zuzena.");
                return false;
            }

            var mahaiaId = erreserba?.MahaiaId ?? (int)nudMahaiaId.Value;
            var edukiera = LortuEdukiera(mahaiaId);

            if (cmbPertsonaKop.SelectedItem == null)
            {
                MessageBox.Show("Aukeratu pertsona kopurua.");
                return false;
            }

            var kop = (int)cmbPertsonaKop.SelectedItem;
            if (kop < 1 || kop > edukiera)
            {
                MessageBox.Show($"Mahai honetan gehienez {edukiera} pertsona egon daitezke.");
                KargatuPertsonaCombo(edukiera, edukiera);
                return false;
            }

            return true;
        }

        private async void btnGorde_Click(object sender, EventArgs e)
        {
            if (!Balioztatu()) return;

            try
            {
                btnGorde.Enabled = false;
                btnEzabatu.Enabled = false;
                lblAzkenEgoera.Text = "Gordetzen...";

                var berria = InprimakitikHartu();
                var erantzuna = await bezeroa.PutAsJsonAsync($"{ApiOinarria}/api/Erreserbak/{erreserbaId}", berria);

                if (!erantzuna.IsSuccessStatusCode)
                {
                    var edukia = await erantzuna.Content.ReadAsStringAsync();
                    MessageBox.Show("Ezin izan da gorde.\n" + edukia);
                    lblAzkenEgoera.Text = "Errorea";
                    return;
                }

                await HasieratuDatuak();
                MessageBox.Show("Aldaketak gorde dira.");
                lblAzkenEgoera.Text = "Gordeta";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea: " + ex.Message);
                lblAzkenEgoera.Text = "Errorea";
            }
            finally
            {
                btnGorde.Enabled = true;
                btnEzabatu.Enabled = true;
            }
        }

        private async void btnEzabatu_Click(object sender, EventArgs e)
        {
            var emaitza = MessageBox.Show(
                "Erreserba ezabatu egingo da, zihur zaude hau egin nahi duzula?",
                "Baieztapena",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (emaitza != DialogResult.Yes) return;

            try
            {
                btnGorde.Enabled = false;
                btnEzabatu.Enabled = false;
                lblAzkenEgoera.Text = "Ezabatzen...";

                var erantzuna = await bezeroa.DeleteAsync($"{ApiOinarria}/api/Erreserbak/{erreserbaId}");

                if (!erantzuna.IsSuccessStatusCode)
                {
                    var edukia = await erantzuna.Content.ReadAsStringAsync();
                    MessageBox.Show("Ezin izan da ezabatu.\n" + edukia);
                    lblAzkenEgoera.Text = "Errorea";
                    return;
                }

                MessageBox.Show("Erreserba ezabatu da.");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea: " + ex.Message);
                lblAzkenEgoera.Text = "Errorea";
            }
            finally
            {
                btnGorde.Enabled = true;
                btnEzabatu.Enabled = true;
            }
        }
    }
}
