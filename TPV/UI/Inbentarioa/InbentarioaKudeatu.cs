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
    public partial class InbentarioaKudeatu : Form
    {
        private readonly HttpClient bezeroa;
        private const string ApiOinarria = "http://192.168.10.5:5093";
        // private const string ApiOinarria = "http://localhost:5093";
        private List<Inbentarioa> inbentarioa;
        private readonly Dictionary<int, (decimal Kantitatea, DateTime Azkena)> aldaketak = new();

        public InbentarioaKudeatu()
        {
            InitializeComponent();
            bezeroa = new HttpClient();
            Shown += async (_, __) => await KargatuDatuak();
            btnGorde.Click += async (_, __) => await GordeAldaketak();
        }

        private async Task KargatuDatuak()
        {
            try
            {
                btnGorde.Enabled = false;
                cardsPanel.Controls.Clear();
                aldaketak.Clear();

                var lista = await bezeroa.GetFromJsonAsync<List<Inbentarioa>>($"{ApiOinarria}/api/Inbentarioa");
                inbentarioa = (lista ?? new List<Inbentarioa>())
                    .OrderBy(x => x.Izena)
                    .ToList();

                if (!inbentarioa.Any())
                {
                    btnGorde.Enabled = false;
                    return;
                }

                foreach (var item in inbentarioa)
                {
                    cardsPanel.Controls.Add(SortuTxartela(item));
                }

                btnGorde.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea: " + ex.Message);
            }
        }

        private Control SortuTxartela(Inbentarioa item)
        {
            var card = new Panel
            {
                Width = 360,
                Height = 220,
                BackColor = System.Drawing.Color.White,
                Margin = new Padding(12),
                Padding = new Padding(12),
                BorderStyle = BorderStyle.FixedSingle
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            for (int i = 0; i < 5; i++)
                layout.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));

            var lblIzena = SortuLbl("Izena");
            var txtIzena = SortuTxt(item.Izena);

            var lblKant = SortuLbl("Kantitatea");
            var nudKant = new NumericUpDown
            {
                DecimalPlaces = 2,
                Minimum = 0,
                Maximum = 1000000000,
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold)
            };

            decimal hasKant;
            try { hasKant = Convert.ToDecimal(item.Kantitatea); }
            catch { hasKant = 0; }

            if (hasKant < 0) hasKant = 0;
            if (hasKant > nudKant.Maximum) hasKant = nudKant.Maximum;
            nudKant.Value = hasKant;

            var lblNeurri = SortuLbl("Neurria");
            var txtNeurri = SortuTxt(item.NeurriaUnitatea);

            var lblStock = SortuLbl("Stock minimoa");
            var txtStock = SortuTxt(item.StockMinimoa.ToString());

            var lblAzken = SortuLbl("Azken eguneratzea");
            var lblAzkenBal = new Label
            {
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Text = AzkenTestua(item)
            };

            if (hasKant <= Convert.ToDecimal(item.StockMinimoa))
                card.BackColor = System.Drawing.Color.FromArgb(255, 250, 235);

            nudKant.ValueChanged += (_, __) =>
            {
                var orain = DateTime.Now;
                lblAzkenBal.Text = orain.ToString("yyyy-MM-dd HH:mm:ss");
                aldaketak[item.Id] = (nudKant.Value, orain);
            };

            layout.Controls.Add(lblIzena, 0, 0);
            layout.Controls.Add(txtIzena, 1, 0);

            layout.Controls.Add(lblKant, 0, 1);
            layout.Controls.Add(nudKant, 1, 1);

            layout.Controls.Add(lblNeurri, 0, 2);
            layout.Controls.Add(txtNeurri, 1, 2);

            layout.Controls.Add(lblStock, 0, 3);
            layout.Controls.Add(txtStock, 1, 3);

            layout.Controls.Add(lblAzken, 0, 4);
            layout.Controls.Add(lblAzkenBal, 1, 4);

            card.Controls.Add(layout);
            return card;
        }

        private string AzkenTestua(Inbentarioa item)
        {
            if (item == null) return "Ez dago";

            var prop = item.GetType().GetProperty("AzkenEguneratzea");
            if (prop == null) return "Ez dago";

            var bal = prop.GetValue(item);
            if (bal == null) return "Ez dago";

            if (bal is DateTime dt)
                return dt.ToString("yyyy-MM-dd HH:mm:ss");

            return bal.ToString();
        }


        private Label SortuLbl(string testua)
        {
            return new Label
            {
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Text = testua
            };
        }

        private TextBox SortuTxt(string balioa)
        {
            return new TextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold),
                Text = balioa ?? ""
            };
        }

        private async Task GordeAldaketak()
        {
            if (!aldaketak.Any())
            {
                MessageBox.Show("Ez dago aldaketarik gordetzeko.");
                return;
            }

            try
            {
                btnGorde.Enabled = false;

                foreach (var (id, datuak) in aldaketak.ToList())
                {
                    var item = inbentarioa.FirstOrDefault(x => x.Id == id);
                    if (item == null) continue;

                    var body = new
                    {
                        izena = item.Izena,
                        deskribapena = item.Deskribapena,
                        kantitatea = datuak.Kantitatea,
                        neurriaUnitatea = item.NeurriaUnitatea,
                        stockMinimoa = item.StockMinimoa
                    };

                    var put = await bezeroa.PutAsJsonAsync($"{ApiOinarria}/api/Inbentarioa/{id}", body);

                    if (!put.IsSuccessStatusCode)
                    {
                        var edukia = await put.Content.ReadAsStringAsync();
                        MessageBox.Show($"Ezin izan da gorde (ID {id}).\n{edukia}");
                        btnGorde.Enabled = true;
                        return;
                    }
                }

                MessageBox.Show("Aldaketak gorde dira.");
                await KargatuDatuak();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea: " + ex.Message);
            }
            finally
            {
                btnGorde.Enabled = true;
            }
        }
    }
}
