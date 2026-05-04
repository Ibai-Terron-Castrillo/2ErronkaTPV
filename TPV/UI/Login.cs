using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Windows.Forms;

namespace TPV
{
    public partial class Login : Form
    {
        private int saiakerak = 3;
        //private readonly string apiUrl = "http://localhost:5093/api/Langileak";
        private readonly string apiUrl = "http://192.168.10.5:5093/api/Langileak";

        public Login()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(500, 420);
            AutoScaleMode = AutoScaleMode.Font;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string erabiltzailea = txtErabiltzailea.Text.Trim();
            string pasahitza = txtPasahitza.Text.Trim();

            if (string.IsNullOrEmpty(erabiltzailea) || string.IsNullOrEmpty(pasahitza))
            {
                MessageBox.Show("Mesedez, bete bi eremuak.", "Errorea", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using HttpClient client = new HttpClient();
                using var stream = await client.GetStreamAsync(apiUrl);
                using var doc = await JsonDocument.ParseAsync(stream);

                if (doc.RootElement.ValueKind != JsonValueKind.Array)
                {
                    MessageBox.Show("Ezin izan da APIaren erantzuna irakurri.", "Errorea", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                JsonElement? langileElem = null;
                foreach (var e2 in doc.RootElement.EnumerateArray())
                {
                    if (!TryLortuStringMulti(e2, out var eUser, "Erabiltzailea", "Erabiltzaile", "Usuario", "Username", "UserName", "user", "username")) continue;
                    if (!TryLortuStringMulti(e2, out var ePass, "Pasahitza", "Password", "Contrasena", "Contraseña", "pass", "password")) continue;

                    if (eUser == erabiltzailea && ePass == pasahitza)
                    {
                        langileElem = e2;
                        break;
                    }
                }

                if (langileElem is null)
                {
                    saiakerak--;
                    MessageBox.Show($"Erabiltzailea edo pasahitza okerra.\nSaiakerak: {saiakerak}", "Errorea", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (saiakerak <= 0)
                    {
                        MessageBox.Show("Saiakera kopurua gaindituta.\nAplikazioa itxiko da.", "Blokeoa", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        Application.Exit();
                    }

                    return;
                }

                var le = langileElem.Value;

                if (TryLortuStringMulti(le, out var aktibo, "Aktibo", "Activo", "Active") && aktibo != "Bai")
                {
                    MessageBox.Show("Erabiltzailea bajan emanda dago.\nJarri harremanetan administratzailearekin.", "Erabiltzailea ez aktiboa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int langileId = TryLortuIntMulti(le, "Id", "id") ?? 0;
                int rolaId = TryLortuIntMulti(le, "RolaId", "RolaID", "RolId", "rolId", "rolaId") ?? 0;
                string erabiltzaileIzenaApi = TryLortuStringMulti(le, out var eu, "Erabiltzailea", "Erabiltzaile", "Usuario", "Username", "UserName", "user", "username") ? eu : erabiltzailea;
                bool? txatBaimena = LortuTxatBaimena(le);

                SaioGlobala.Ezarri(langileId, rolaId, erabiltzaileIzenaApi, null, txatBaimena);

                if (rolaId == 1)
                {
                    new SukaldariMenua(langileId).ShowDialog();
                    Close();
                    return;
                }

                if (rolaId == 2)
                {
                    new ZerbitzariMenua(langileId).ShowDialog();
                    Close();
                    return;
                }

                if (rolaId == 3 || rolaId == 4)
                {
                    bool sukaldariModuan = false;

                    while (true)
                    {
                        DialogResult emaitza = !sukaldariModuan
                            ? new ZerbitzariMenua(langileId, true).ShowDialog()
                            : new SukaldariMenua(langileId, true).ShowDialog();

                        if (emaitza == DialogResult.Retry)
                        {
                            sukaldariModuan = !sukaldariModuan;
                            continue;
                        }

                        break;
                    }

                    Close();
                    return;
                }

                new ZerbitzariMenua(langileId).ShowDialog();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea APIarekin konektatzean:\n" + ex.Message, "Errorea", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static int? TryLortuInt(JsonElement e, string izena)
        {
            if (!TryLortuProp(e, izena, out var p)) return null;
            if (p.ValueKind == JsonValueKind.Number && p.TryGetInt32(out var v1)) return v1;
            if (p.ValueKind == JsonValueKind.String && int.TryParse(p.GetString(), out var v2)) return v2;
            return null;
        }

        static int? TryLortuIntMulti(JsonElement e, params string[] izenak)
        {
            foreach (var izena in izenak)
            {
                var v = TryLortuInt(e, izena);
                if (v is not null) return v;
            }
            return null;
        }

        static bool TryLortuString(JsonElement e, string izena, out string balioa)
        {
            balioa = "";
            if (!TryLortuProp(e, izena, out var p)) return false;
            if (p.ValueKind == JsonValueKind.String)
            {
                balioa = p.GetString() ?? "";
                return true;
            }
            return false;
        }

        static bool TryLortuStringMulti(JsonElement e, out string balioa, params string[] izenak)
        {
            foreach (var izena in izenak)
            {
                if (TryLortuString(e, izena, out balioa)) return true;
            }

            balioa = "";
            return false;
        }

        static bool TryLortuProp(JsonElement e, string izena, out JsonElement p)
        {
            foreach (var prop in e.EnumerateObject())
            {
                if (prop.NameEquals(izena) || prop.Name.Equals(izena, StringComparison.OrdinalIgnoreCase))
                {
                    p = prop.Value;
                    return true;
                }
            }

            p = default;
            return false;
        }

        static bool? LortuBoolMalgu(JsonElement e)
        {
            if (e.ValueKind == JsonValueKind.True) return true;
            if (e.ValueKind == JsonValueKind.False) return false;

            if (e.ValueKind == JsonValueKind.Number)
            {
                if (e.TryGetInt32(out var n)) return n != 0;
            }

            if (e.ValueKind == JsonValueKind.String)
            {
                var s = (e.GetString() ?? "").Trim();
                if (s.Equals("true", StringComparison.OrdinalIgnoreCase)) return true;
                if (s.Equals("false", StringComparison.OrdinalIgnoreCase)) return false;
                if (s.Equals("1", StringComparison.OrdinalIgnoreCase)) return true;
                if (s.Equals("0", StringComparison.OrdinalIgnoreCase)) return false;
                if (s.Equals("bai", StringComparison.OrdinalIgnoreCase)) return true;
                if (s.Equals("ez", StringComparison.OrdinalIgnoreCase)) return false;
                if (s.Equals("si", StringComparison.OrdinalIgnoreCase)) return true;
                if (s.Equals("no", StringComparison.OrdinalIgnoreCase)) return false;
            }

            return null;
        }

        static bool? LortuTxatBaimena(JsonElement langile)
        {
            string[] izenak =
            {
                "TxatBaimenduta",
                "TxatBaimena",
                "TxatBaimen",
                "Txat",
                "ChatPermiso",
                "PermisoChat",
                "Chat",
                "ChatAllowed",
                "CanChat"
            };

            foreach (var izena in izenak)
            {
                if (TryLortuProp(langile, izena, out var p))
                {
                    var b = LortuBoolMalgu(p);
                    if (b is not null) return b;
                }
            }

            string[] baimenArrayIzenak =
            {
                "Baimenak",
                "Permisos",
                "Permissions",
                "Claims"
            };

            foreach (var arrName in baimenArrayIzenak)
            {
                if (TryLortuProp(langile, arrName, out var p) && p.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in p.EnumerateArray())
                    {
                        if (item.ValueKind != JsonValueKind.String) continue;
                        var s = (item.GetString() ?? "").Trim();
                        if (s.Equals("Txat", StringComparison.OrdinalIgnoreCase) ||
                            s.Equals("Chat", StringComparison.OrdinalIgnoreCase) ||
                            s.Equals("UseChat", StringComparison.OrdinalIgnoreCase) ||
                            s.Equals("CanChat", StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }

            if (TryLortuProp(langile, "Rola", out var rola) && rola.ValueKind == JsonValueKind.Object)
            {
                foreach (var izena in izenak)
                {
                    if (TryLortuProp(rola, izena, out var p))
                    {
                        var b = LortuBoolMalgu(p);
                        if (b is not null) return b;
                    }
                }

                foreach (var arrName in baimenArrayIzenak)
                {
                    if (TryLortuProp(rola, arrName, out var p) && p.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in p.EnumerateArray())
                        {
                            if (item.ValueKind != JsonValueKind.String) continue;
                            var s = (item.GetString() ?? "").Trim();
                            if (s.Equals("Txat", StringComparison.OrdinalIgnoreCase) ||
                                s.Equals("Chat", StringComparison.OrdinalIgnoreCase) ||
                                s.Equals("UseChat", StringComparison.OrdinalIgnoreCase) ||
                                s.Equals("CanChat", StringComparison.OrdinalIgnoreCase))
                            {
                                return true;
                            }
                        }

                        return false;
                    }
                }
            }

            return null;
        }
    }
}
