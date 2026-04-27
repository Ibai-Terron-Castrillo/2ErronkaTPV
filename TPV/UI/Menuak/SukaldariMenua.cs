using System.Windows.Forms;
using TPV.BISTAK;

namespace TPV
{
    public partial class SukaldariMenua : Form
    {
        private readonly int langileId;
        private readonly bool itzuliDezake;
        private const int TxatPanelaMaxZabalera = 420;
        private const int TxatPanelaMinZabalera = 260;
        private const int MenuMinZabalera = 520;

        public SukaldariMenua(int langileIdPasatua, bool itzuliDezake = false)
        {
            InitializeComponent();
            langileId = langileIdPasatua;
            this.itzuliDezake = itzuliDezake || SaioGlobala.RolaId == 3 || SaioGlobala.RolaId == 4;

            btnItzuli.Visible = this.itzuliDezake;
            btnItzuli.Click += btnItzuli_Click;

            Shown += (_, _) => EguneratuDiseinua();
            Resize += (_, _) => EguneratuDiseinua();
            EguneratuDiseinua();

            _ = TxataHasieratu();

            btnInbentarioaKudeatu.Click += (s, e) =>
            {
                using (InbentarioaKudeatu inb = new InbentarioaKudeatu())
                {
                    inb.ShowDialog();
                }
            };

            btnPlaterenEgoera.Click += (s, e) =>
            {
                using (ZerbitzuenEgoera pl = new ZerbitzuenEgoera())
                {
                    pl.ShowDialog();
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
                //await TxatBezeroa.Instantzia.KonektatuAsync("localhost", 5555, SaioGlobala.LangileId, SaioGlobala.RolaId, SaioGlobala.ErabiltzaileIzena, SaioGlobala.Tokena);
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
        }

        private void btnItzuli_Click(object sender, System.EventArgs e)
        {
            TxatBezeroa.Instantzia.Deskonektatu();
            DialogResult = DialogResult.Retry;
            Close();
        }
    }
}
