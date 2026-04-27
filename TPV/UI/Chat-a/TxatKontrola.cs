using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace TPV
{
    public partial class TxatKontrola : UserControl
    {
        DateTime? azkenData = null;
        readonly Dictionary<string, FitxategiZatiak> fitxategiak = new();
        readonly Queue<string> pendingRaw = new();
        (bool konektatuta, string mezua)? pendingEgoera = null;
        bool txatBaimenduta = true;
        bool baimenAbisatua = false;

        public TxatKontrola()
        {
            InitializeComponent();
            HandleCreated += TxatKontrola_HandleCreated;
            HandleDestroyed += TxatKontrola_HandleDestroyed;
        }

        public void EzarriTxatBaimena(bool baimenduta)
        {
            txatBaimenduta = baimenduta;
            baimenAbisatua = false;

            if (IsDisposed) return;
            if (!IsHandleCreated) return;

            BeginInvoke(new Action(AplikatuBaimenUi));
        }

        void TxatKontrola_HandleCreated(object? sender, EventArgs e)
        {
            TxatBezeroa.Instantzia.RawJasota += RawKudeatu;
            TxatBezeroa.Instantzia.EgoeraAldatu += EgoeraKudeatu;
            AplikatuBaimenUi();

            if (pendingEgoera is not null)
            {
                var (k, m) = pendingEgoera.Value;
                pendingEgoera = null;
                EgoeraKudeatu(k, m);
            }

            while (pendingRaw.Count > 0)
            {
                var raw = pendingRaw.Dequeue();
                RawKudeatu(raw);
            }
        }

        void TxatKontrola_HandleDestroyed(object? sender, EventArgs e)
        {
            TxatBezeroa.Instantzia.RawJasota -= RawKudeatu;
            TxatBezeroa.Instantzia.EgoeraAldatu -= EgoeraKudeatu;
        }

        void EgoeraKudeatu(bool konektatuta, string mezua)
        {
            if (IsDisposed) return;
            if (!IsHandleCreated)
            {
                pendingEgoera = (konektatuta, mezua);
                return;
            }

            BeginInvoke(new Action(() =>
            {
                if (!txatBaimenduta)
                {
                    lblEgoera.Text = "Baimenik gabe";
                    return;
                }

                lblEgoera.Text = konektatuta ? "Konektatuta" : "Deskonektatuta";
            }));
        }

        void RawKudeatu(string raw)
        {
            if (IsDisposed) return;
            if (!IsHandleCreated)
            {
                pendingRaw.Enqueue(raw);
                return;
            }

            BeginInvoke(new Action(() =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(raw))
                    {
                        return;
                    }

                    if (raw.StartsWith("[", StringComparison.OrdinalIgnoreCase))
                    {
                        if (raw.IndexOf("Ezezaguna irten da", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            raw.IndexOf("Ezezaguna sartu da", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            raw.IndexOf("CNXN|", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            return;
                        }

                        GehituSistema(raw);
                        BeheraJoan();
                        return;
                    }

                    int banatzailea = raw.IndexOf(':');
                    if (banatzailea > 0)
                    {
                        string norIzena = raw[..banatzailea].Trim();
                        string testuZifratua = raw[(banatzailea + 1)..].Trim();
                        string testua = testuZifratua;
                        if (BadirudiBase64(testuZifratua))
                        {
                            try
                            {
                                testua = ZifratzeTresnak.Deszifratu(testuZifratua);
                            }
                            catch
                            {
                                testua = testuZifratua;
                            }
                        }
                        if (SaiatuFitxategiaKudeatu(norIzena, testua, DateTime.Now))
                        {
                            BeheraJoan();
                            return;
                        }

                        GehituMezua(norIzena, testua, DateTime.Now);
                        BeheraJoan();
                        return;
                    }

                    GehituSistema(raw);
                    BeheraJoan();
                }
                catch (Exception ex)
                {
                    GehituSistema("[ERROREA] " + ex.Message);
                    BeheraJoan();
                }
            }));
        }

        static bool BadirudiBase64(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            if (s.Length < 8) return false;
            if (s.Length % 4 != 0) return false;
            try
            {
                Convert.FromBase64String(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        void GehituMezua(string norIzena, string testua, DateTime dataLokala)
        {
            if (azkenData is null || azkenData.Value.Date != dataLokala.Date)
            {
                azkenData = dataLokala.Date;
                GehituDataBereizlea(dataLokala);
            }

            var mezuaPanela = new TableLayoutPanel();
            mezuaPanela.ColumnCount = 2;
            mezuaPanela.RowCount = 2;
            mezuaPanela.AutoSize = true;
            mezuaPanela.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mezuaPanela.Margin = new Padding(0, 0, 0, 10);
            mezuaPanela.Padding = new Padding(10);
            mezuaPanela.BackColor = System.Drawing.Color.White;
            mezuaPanela.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mezuaPanela.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            mezuaPanela.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mezuaPanela.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mezuaPanela.Width = mezuakPanel.ClientSize.Width - 30;

            var lblTestua = new Label();
            lblTestua.AutoSize = true;
            lblTestua.MaximumSize = new System.Drawing.Size(mezuaPanela.Width - 160, 0);
            lblTestua.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular);
            lblTestua.Text = testua;

            var lblNor = new Label();
            lblNor.AutoSize = true;
            lblNor.TextAlign = System.Drawing.ContentAlignment.TopRight;
            lblNor.Dock = DockStyle.Fill;
            lblNor.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            lblNor.Text = norIzena;

            var lblOrdua = new Label();
            lblOrdua.AutoSize = true;
            lblOrdua.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            lblOrdua.Dock = DockStyle.Fill;
            lblOrdua.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            lblOrdua.Text = dataLokala.ToString("HH:mm", CultureInfo.InvariantCulture);

            mezuaPanela.Controls.Add(lblTestua, 0, 0);
            mezuaPanela.Controls.Add(lblNor, 1, 0);
            mezuaPanela.Controls.Add(new Panel(), 0, 1);
            mezuaPanela.Controls.Add(lblOrdua, 1, 1);
            mezuakPanel.Controls.Add(mezuaPanela);
        }

        bool SaiatuFitxategiaKudeatu(string norIzena, string testua, DateTime dataLokala)
        {
            if (testua.StartsWith("[FILE]|", StringComparison.OrdinalIgnoreCase))
            {
                testua = "FILE|" + testua[7..];
            }
            if (testua.StartsWith("[FILECHUNK]|", StringComparison.OrdinalIgnoreCase))
            {
                testua = "FILECHUNK|" + testua[12..];
            }

            if (!testua.StartsWith("FILE|", StringComparison.OrdinalIgnoreCase))
            {
                return SaiatuFitxategiZatiakKudeatu(norIzena, testua, dataLokala);
            }

            string[] zatiak = testua.Split('|');
            if (zatiak.Length < 3)
            {
                return false;
            }

            string fitxategiIzena = zatiak[1].Trim();
            if (fitxategiIzena.Length > 0)
            {
                try
                {
                    fitxategiIzena = WebUtility.UrlDecode(fitxategiIzena);
                }
                catch
                {
                }
            }

            string base64 = zatiak.Length >= 5 ? zatiak[^1].Trim() : zatiak[2].Trim();
            if (fitxategiIzena.Length == 0 || base64.Length == 0)
            {
                return false;
            }

            byte[] edukia;
            try
            {
                edukia = Convert.FromBase64String(base64);
            }
            catch
            {
                return false;
            }

            GehituFitxategia(norIzena, fitxategiIzena, edukia, dataLokala);
            return true;
        }

        bool SaiatuFitxategiZatiakKudeatu(string norIzena, string testua, DateTime dataLokala)
        {
            if (!testua.StartsWith("FILECHUNK|", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string[] zatiak = testua.Split('|');
            if (zatiak.Length < 6)
            {
                return true;
            }

            string fitxategiId = zatiak[1].Trim();
            string fitxategiIzena = zatiak[2].Trim();
            if (fitxategiIzena.Length > 0)
            {
                try
                {
                    fitxategiIzena = WebUtility.UrlDecode(fitxategiIzena);
                }
                catch
                {
                }
            }

            int zenbakia;
            int guztira;
            string base64;

            // Formatuak:
            // - FILECHUNK|id|name|idx|total|b64
            // - FILECHUNK|id|name|mime|idx|total|b64
            if (int.TryParse(zatiak[3], out zenbakia) && zenbakia > 0 && int.TryParse(zatiak[4], out guztira) && guztira > 0)
            {
                base64 = zatiak[5].Trim();
            }
            else if (zatiak.Length >= 7 && int.TryParse(zatiak[4], out zenbakia) && zenbakia > 0 && int.TryParse(zatiak[5], out guztira) && guztira > 0)
            {
                base64 = zatiak[6].Trim();
            }
            else
            {
                return true;
            }

            if (fitxategiId.Length == 0 || fitxategiIzena.Length == 0 || base64.Length == 0)
            {
                return true;
            }

            byte[] zatia;
            try
            {
                zatia = Convert.FromBase64String(base64);
            }
            catch
            {
                return true;
            }

            if (!fitxategiak.TryGetValue(fitxategiId, out var egoera))
            {
                egoera = new FitxategiZatiak(fitxategiIzena, guztira);
                fitxategiak[fitxategiId] = egoera;
            }

            if (egoera.Guztira != guztira)
            {
                fitxategiak.Remove(fitxategiId);
                return false;
            }

            if (!string.Equals(egoera.Izena, fitxategiIzena, StringComparison.Ordinal))
            {
                fitxategiak.Remove(fitxategiId);
                return false;
            }

            egoera.Gorde(zenbakia - 1, zatia);
            if (!egoera.Osatuta)
            {
                return true;
            }

            byte[] edukia = egoera.Bateratu();
            fitxategiak.Remove(fitxategiId);
            GehituFitxategia(norIzena, fitxategiIzena, edukia, dataLokala);
            return true;
        }

        void GehituFitxategia(string norIzena, string fitxategiIzena, byte[] edukia, DateTime dataLokala)
        {
            if (azkenData is null || azkenData.Value.Date != dataLokala.Date)
            {
                azkenData = dataLokala.Date;
                GehituDataBereizlea(dataLokala);
            }

            var mezuaPanela = new TableLayoutPanel();
            mezuaPanela.ColumnCount = 2;
            mezuaPanela.RowCount = 2;
            mezuaPanela.AutoSize = true;
            mezuaPanela.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mezuaPanela.Margin = new Padding(0, 0, 0, 10);
            mezuaPanela.Padding = new Padding(10);
            mezuaPanela.BackColor = System.Drawing.Color.White;
            mezuaPanela.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mezuaPanela.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            mezuaPanela.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mezuaPanela.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mezuaPanela.Width = mezuakPanel.ClientSize.Width - 30;

            var lblFitxategia = new LinkLabel();
            lblFitxategia.AutoSize = true;
            lblFitxategia.MaximumSize = new System.Drawing.Size(mezuaPanela.Width - 160, 0);
            lblFitxategia.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Underline);
            lblFitxategia.Text = fitxategiIzena;
            lblFitxategia.LinkClicked += (_, __) => IrekiFitxategia(fitxategiIzena, edukia);

            var ekintzak = new FlowLayoutPanel();
            ekintzak.AutoSize = true;
            ekintzak.WrapContents = false;
            ekintzak.FlowDirection = FlowDirection.LeftToRight;
            ekintzak.Margin = new Padding(0, 8, 0, 0);

            var btnDeskargatu = new Button();
            btnDeskargatu.AutoSize = true;
            btnDeskargatu.Text = "Gorde";
            btnDeskargatu.BackColor = System.Drawing.Color.Goldenrod;
            btnDeskargatu.ForeColor = System.Drawing.Color.Black;
            btnDeskargatu.FlatStyle = FlatStyle.Flat;
            btnDeskargatu.FlatAppearance.BorderSize = 0;
            btnDeskargatu.Click += (_, __) => GordeFitxategia(fitxategiIzena, edukia);

            var btnIkusi = new Button();
            btnIkusi.AutoSize = true;
            btnIkusi.Text = "Ireki";
            btnIkusi.BackColor = System.Drawing.Color.Goldenrod;
            btnIkusi.ForeColor = System.Drawing.Color.Black;
            btnIkusi.FlatStyle = FlatStyle.Flat;
            btnIkusi.FlatAppearance.BorderSize = 0;
            btnIkusi.Click += (_, __) => IrekiFitxategia(fitxategiIzena, edukia);

            ekintzak.Controls.Add(btnDeskargatu);
            ekintzak.Controls.Add(btnIkusi);

            var ezkerEdukia = new FlowLayoutPanel();
            ezkerEdukia.AutoSize = true;
            ezkerEdukia.FlowDirection = FlowDirection.TopDown;
            ezkerEdukia.WrapContents = false;
            ezkerEdukia.Controls.Add(lblFitxategia);
            ezkerEdukia.Controls.Add(ekintzak);

            var lblNor = new Label();
            lblNor.AutoSize = true;
            lblNor.TextAlign = System.Drawing.ContentAlignment.TopRight;
            lblNor.Dock = DockStyle.Fill;
            lblNor.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            lblNor.Text = norIzena;

            var lblOrdua = new Label();
            lblOrdua.AutoSize = true;
            lblOrdua.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            lblOrdua.Dock = DockStyle.Fill;
            lblOrdua.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            lblOrdua.Text = dataLokala.ToString("HH:mm", CultureInfo.InvariantCulture);

            mezuaPanela.Controls.Add(ezkerEdukia, 0, 0);
            mezuaPanela.Controls.Add(lblNor, 1, 0);
            mezuaPanela.Controls.Add(new Panel(), 0, 1);
            mezuaPanela.Controls.Add(lblOrdua, 1, 1);
            mezuakPanel.Controls.Add(mezuaPanela);
        }

        void IrekiFitxategia(string fitxategiIzena, byte[] edukia)
        {
            try
            {
                string temp = SortuTempFitxategia(fitxategiIzena, edukia);
                Process.Start(new ProcessStartInfo(temp) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ezin izan da fitxategia ireki", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void GordeFitxategia(string fitxategiIzena, byte[] edukia)
        {
            try
            {
                using var dlg = new SaveFileDialog();
                dlg.FileName = GarbituFitxategiIzena(fitxategiIzena);
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    File.WriteAllBytes(dlg.FileName, edukia);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ezin izan da fitxategia gorde", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static string SortuTempFitxategia(string fitxategiIzena, byte[] edukia)
        {
            string garbia = GarbituFitxategiIzena(fitxategiIzena);
            string ext = Path.GetExtension(garbia);
            if (string.IsNullOrWhiteSpace(ext)) ext = ".bin";
            string izena = Path.GetFileNameWithoutExtension(garbia);
            if (string.IsNullOrWhiteSpace(izena)) izena = "fitxategia";

            string tempDir = Path.Combine(Path.GetTempPath(), "TPV-Chat-Fitxategiak");
            Directory.CreateDirectory(tempDir);
            string tempFile = Path.Combine(tempDir, izena + "-" + Guid.NewGuid().ToString("N") + ext);
            File.WriteAllBytes(tempFile, edukia);
            return tempFile;
        }

        static string GarbituFitxategiIzena(string fitxategiIzena)
        {
            string izena = Path.GetFileName(fitxategiIzena ?? "");
            if (string.IsNullOrWhiteSpace(izena)) return "fitxategia";
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                izena = izena.Replace(c, '_');
            }
            return izena;
        }

        sealed class FitxategiZatiak
        {
            public string Izena { get; }
            public int Guztira { get; }
            readonly byte[][] zatiak;
            int jasota;

            public FitxategiZatiak(string izena, int guztira)
            {
                Izena = izena;
                Guztira = guztira;
                zatiak = new byte[guztira][];
            }

            public bool Osatuta => jasota == Guztira;

            public void Gorde(int index, byte[] datuak)
            {
                if (index < 0 || index >= zatiak.Length) return;
                if (zatiak[index] is not null) return;
                zatiak[index] = datuak;
                jasota++;
            }

            public byte[] Bateratu()
            {
                int guztiraLuzera = 0;
                for (int i = 0; i < zatiak.Length; i++)
                {
                    if (zatiak[i] is null) return Array.Empty<byte>();
                    guztiraLuzera += zatiak[i].Length;
                }

                byte[] output = new byte[guztiraLuzera];
                int offset = 0;
                for (int i = 0; i < zatiak.Length; i++)
                {
                    Buffer.BlockCopy(zatiak[i], 0, output, offset, zatiak[i].Length);
                    offset += zatiak[i].Length;
                }
                return output;
            }
        }

        void GehituDataBereizlea(DateTime dataLokala)
        {
            var lbl = new Label();
            lbl.AutoSize = false;
            lbl.Width = mezuakPanel.ClientSize.Width - 30;
            lbl.Height = 36;
            lbl.Margin = new Padding(0, 10, 0, 10);
            lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbl.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            lbl.ForeColor = System.Drawing.Color.Goldenrod;
            lbl.Text = dataLokala.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            mezuakPanel.Controls.Add(lbl);
        }

        void GehituSistema(string testua)
        {
            var lbl = new Label();
            lbl.AutoSize = false;
            lbl.Width = mezuakPanel.ClientSize.Width - 30;
            lbl.Height = 28;
            lbl.Margin = new Padding(0, 0, 0, 10);
            lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbl.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            lbl.ForeColor = System.Drawing.Color.DimGray;
            lbl.Text = testua;
            mezuakPanel.Controls.Add(lbl);
        }

        void BeheraJoan()
        {
            if (mezuakPanel.Controls.Count == 0) return;
            var azkena = mezuakPanel.Controls[mezuakPanel.Controls.Count - 1];
            mezuakPanel.ScrollControlIntoView(azkena);
        }

        async void btnBidali_Click(object sender, EventArgs e)
        {
            if (!txatBaimenduta)
            {
                AbisatuBaimenikGabe();
                return;
            }

            var t = txtMezua.Text.Trim();
            if (t.Length == 0) return;

            try
            {
                txtMezua.Clear();
                await TxatBezeroa.Instantzia.BidaliAsync(t);
            }
            catch (Exception ex)
            {
                GehituSistema("[ERROREA] " + ex.Message);
                BeheraJoan();
            }
        }

        void txtMezua_KeyDown(object sender, KeyEventArgs e)
        {
            if (!txatBaimenduta)
            {
                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnBidali.PerformClick();
            }
        }

        void btnEmoji_Click(object sender, EventArgs e)
        {
            if (!txatBaimenduta)
            {
                AbisatuBaimenikGabe();
                return;
            }

            var menu = new ContextMenuStrip();

            void Gehitu(string emoji)
            {
                var item = new ToolStripMenuItem(emoji);
                item.Click += (_, __) =>
                {
                    txtMezua.SelectedText = emoji;
                    txtMezua.Focus();
                    txtMezua.SelectionStart = txtMezua.TextLength;
                };
                menu.Items.Add(item);
            }

            Gehitu("🙂");
            Gehitu("🙁");
            Gehitu("😄");
            Gehitu("❤️");
            Gehitu("👍");

            menu.Show(btnEmoji, 0, btnEmoji.Height);
        }

        async void btnFitxategia_Click(object sender, EventArgs e)
        {
            if (!txatBaimenduta)
            {
                AbisatuBaimenikGabe();
                return;
            }

            using var dlg = new OpenFileDialog();
            dlg.Title = "Aukeratu fitxategia";
            dlg.Multiselect = false;
            if (dlg.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                await TxatBezeroa.Instantzia.BidaliAsync($"/file \"{dlg.FileName}\"");
            }
            catch (Exception ex)
            {
                GehituSistema("[ERROREA] " + ex.Message);
                BeheraJoan();
            }
        }

        void AbisatuBaimenikGabe()
        {
            if (baimenAbisatua) return;
            baimenAbisatua = true;
            GehituSistema("Ez daukazu txat baimenik.");
            BeheraJoan();
        }

        void AplikatuBaimenUi()
        {
            txtMezua.Enabled = txatBaimenduta;
            btnBidali.Enabled = txatBaimenduta;
            btnFitxategia.Enabled = txatBaimenduta;
            btnEmoji.Enabled = txatBaimenduta;

            if (!txatBaimenduta)
            {
                lblEgoera.Text = "Baimenik gabe";
            }
        }
    }
}
