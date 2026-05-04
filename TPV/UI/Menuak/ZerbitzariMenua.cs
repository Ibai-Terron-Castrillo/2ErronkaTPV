using System.Windows.Forms;
using TPV.BISTAK;

namespace TPV
{
    public partial class ZerbitzariMenua : Form
    {
        private readonly int langileId;
        private readonly bool itzuliDezake;
        private const int TxatPanelaMaxZabalera = 420;
        private const int TxatPanelaMinZabalera = 260;
        private const int MenuMinZabalera = 520;

        public ZerbitzariMenua(int langileIdPasatua, bool itzuliDezake = false)
        {
            InitializeComponent();
            langileId = langileIdPasatua;
            this.itzuliDezake = itzuliDezake || SaioGlobala.RolaId == 3 || SaioGlobala.RolaId == 4;

            btnItzuli.Visible = this.itzuliDezake;
            btnItzuli.Click += btnItzuli_Click;

            Shown += (_, _) => EguneratuDiseinua();
            Resize += (_, _) => EguneratuDiseinua();
            EguneratuDiseinua();
            EguraldiaErakutsi();

            _ = TxataHasieratu();

            btnErreserbaSortu.Click += (s, e) =>
            {
                using (ErreserbaSortu erreserba = new ErreserbaSortu())
                {
                    erreserba.ShowDialog();
                }
            };

            btnErreserbaGestionatu.Click += (s, e) =>
            {
                using (ErreserbaGestionatu kudeatu = new ErreserbaGestionatu(langileId))
                {
                    kudeatu.ShowDialog();
                }
            };

            btnZerbitzuaHasi.Click += (s, e) =>
            {
                using (ZerbitzuaHasi zerbitzua = new ZerbitzuaHasi(langileId))
                {
                    zerbitzua.ShowDialog();
                }
            };

            btnFakturaSortu.Click += (s, e) =>
            {
                using (FakturaSortu faktura = new FakturaSortu(langileId))
                {
                    faktura.ShowDialog();
                }
            };
        }

        async System.Threading.Tasks.Task TxataHasieratu()
        {
            txatKontrola.EzarriTxatBaimena(SaioGlobala.TxatBaimenduta);

            if (!SaioGlobala.TxatBaimenduta)
            {
                try { TxatBezeroa.Instantzia.Deskonektatu(); } catch { }
                return;
            }

            try
            {
                await TxatBezeroa.Instantzia.KonektatuAsync("192.168.10.5", 5555, SaioGlobala.LangileId, SaioGlobala.RolaId, SaioGlobala.ErabiltzaileIzena, SaioGlobala.Tokena);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Txat konexio errorea", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EguneratuDiseinua()
        {
            if (IsDisposed) return;
            if (!IsHandleCreated) return;

            if (txatPanela is not null)
            {
                txatPanela.Dock = DockStyle.Right;

                var zabalera = ClientSize.Width;
                var txatZabalera = TxatPanelaMaxZabalera;

                if (zabalera - txatZabalera < MenuMinZabalera)
                {
                    txatZabalera = Math.Max(TxatPanelaMinZabalera, zabalera - MenuMinZabalera);
                }

                txatZabalera = Math.Max(TxatPanelaMinZabalera, Math.Min(TxatPanelaMaxZabalera, txatZabalera));
                txatPanela.Width = txatZabalera;
                txatPanela.BringToFront();
            }

            if (layoutNagusia is not null)
            {
                layoutNagusia.Dock = DockStyle.Fill;
                layoutNagusia.SendToBack();
            }

            if (eguraldiaPanel is not null)
            {
                var txatZabalera = txatPanela?.Width ?? 0;
                var zabaleraEskuragarri = Math.Max(260, ClientSize.Width - txatZabalera - 36);
                eguraldiaPanel.Width = Math.Min(430, zabaleraEskuragarri);
                eguraldiaPanel.Left = 18;
                eguraldiaPanel.Top = Math.Max(headerPanel.Bottom + 12, ClientSize.Height - eguraldiaPanel.Height - 18);
                eguraldiaPanel.BringToFront();
            }
        }

        private void EguraldiaErakutsi()
        {
            var eguraldia = EguraldiaZerbitzua.GaurkoEguraldia;

            if (eguraldia is null)
            {
                lblEguraldiaLaburpena.Text = "Ez dago daturik";
                lblEguraldiaXehetasunak.Text = "";
                return;
            }

            if (!eguraldia.OndoKargatuta)
            {
                lblEguraldiaLaburpena.Text = "Ezin izan da kargatu";
                lblEguraldiaXehetasunak.Text = eguraldia.Errorea ?? "";
                return;
            }

            lblEguraldiaLaburpena.Text = $"{eguraldia.Herria ?? "-"} | {eguraldia.EgoeraKlimatikoa ?? "-"}";
            lblEguraldiaXehetasunak.Text =
                $"Min {eguraldia.TenperaturaMinimoa ?? "-"} / Max {eguraldia.TenperaturaMaximoa ?? "-"} | " +
                $"Euria {eguraldia.EuriProbabilitatea ?? "-"} | Haizea {eguraldia.HaizeNorabidea ?? "-"} {eguraldia.HaizeBatezbestekoAbiadura ?? "-"}";
        }

        private void btnItzuli_Click(object sender, System.EventArgs e)
        {
            TxatBezeroa.Instantzia.Deskonektatu();
            DialogResult = DialogResult.Retry;
            Close();
        }
    }
}
