using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TPV
{
    public sealed class EguraldiaDatuak
    {
        public string? Eguna { get; init; }
        public string? Herria { get; init; }
        public string? TenperaturaMinimoa { get; init; }
        public string? TenperaturaMaximoa { get; init; }
        public string? EgoeraKlimatikoa { get; init; }
        public string? EuriProbabilitatea { get; init; }
        public string? ElurProbabilitatea { get; init; }
        public string? HaizeNorabidea { get; init; }
        public string? HaizeBatezbestekoAbiadura { get; init; }
        public string? Errorea { get; init; }

        public bool OndoKargatuta => string.IsNullOrWhiteSpace(Errorea);
    }

    public static class EguraldiaZerbitzua
    {
        private const string OrdenagailuIp = "192.168.10.5";
        private const string SareErabiltzailea = "administrador";
        private const string SarePasahitza = "Lizapro-123456789";
        private static readonly string EguraldiaPartekatua = @$"\\{OrdenagailuIp}\Eguraldia";
        private static readonly string[] FitxategiBideak =
        {
            @$"\\{OrdenagailuIp}\Eguraldia\Fitxategiak\GaurkoEguraldia.txt",
            @$"\\{OrdenagailuIp}\Eguraldia\GaurkoEguraldia.txt",
            @$"\\{OrdenagailuIp}\C$\Eguraldia\Fitxategiak\GaurkoEguraldia.txt"
        };

        public static EguraldiaDatuak? GaurkoEguraldia { get; private set; }

        public static void KargatuGaurkoEguraldia()
        {
            var azkenErrorea = "";
            var konexioErrorea = KonektatuEguraldiKarpetara();
            if (!string.IsNullOrWhiteSpace(konexioErrorea))
            {
                azkenErrorea = konexioErrorea;
            }

            foreach (var bidea in FitxategiBideak)
            {
                try
                {
                    var edukia = File.ReadAllText(bidea, Encoding.UTF8);
                    GaurkoEguraldia = Parseatu(edukia);
                    return;
                }
                catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException)
                {
                    azkenErrorea = $"{bidea} ez da aurkitu.";
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Security.SecurityException)
                {
                    azkenErrorea = $"{bidea}: {ex.Message}";
                }
            }

            GaurkoEguraldia = new EguraldiaDatuak
            {
                Errorea = string.IsNullOrWhiteSpace(azkenErrorea)
                    ? "Eguraldi fitxategia ez da aurkitu sareko ordenagailuan."
                    : azkenErrorea
            };
        }

        private static string KonektatuEguraldiKarpetara()
        {
            var baliabidea = new NETRESOURCE
            {
                dwType = RESOURCETYPE_DISK,
                lpRemoteName = EguraldiaPartekatua
            };

            var emaitza = WNetAddConnection2(ref baliabidea, SarePasahitza, SareErabiltzailea, 0);
            if (emaitza is NO_ERROR or ERROR_ALREADY_ASSIGNED or ERROR_SESSION_CREDENTIAL_CONFLICT)
            {
                return "";
            }

            return $"Ezin izan da sareko karpetara konektatu. Windows errorea: {emaitza}";
        }

        private static EguraldiaDatuak Parseatu(string edukia)
        {
            var lerroak = edukia
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Select(l => l.Replace('\u00a0', ' ').Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToList();

            var balioak = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var lerroa in lerroak)
            {
                if (lerroa.Contains("->"))
                {
                    var zatiak = lerroa.Split("->", 2, StringSplitOptions.TrimEntries);
                    if (zatiak.Length == 2) balioak[zatiak[0]] = zatiak[1];
                    continue;
                }

                var biPuntuak = lerroa.IndexOf(':');
                if (biPuntuak <= 0) continue;

                var izena = lerroa[..biPuntuak].Trim();
                var balioa = lerroa[(biPuntuak + 1)..].Trim();
                if (!string.IsNullOrWhiteSpace(balioa)) balioak[izena] = balioa;
            }

            return new EguraldiaDatuak
            {
                Eguna = Hartu(balioak, "Eguna"),
                Herria = Hartu(balioak, "Herria"),
                TenperaturaMinimoa = Hartu(balioak, "Minimoa"),
                TenperaturaMaximoa = Hartu(balioak, "Maximoa"),
                EgoeraKlimatikoa = Hartu(balioak, "Egoera klimatikoa"),
                EuriProbabilitatea = Hartu(balioak, "Euri probabilitatea"),
                ElurProbabilitatea = Hartu(balioak, "Elur probabilitatea"),
                HaizeNorabidea = Hartu(balioak, "Norabidea"),
                HaizeBatezbestekoAbiadura = Hartu(balioak, "Batezbesteko abiadura")
            };
        }

        private static string? Hartu(Dictionary<string, string> balioak, string gakoa)
        {
            return balioak.TryGetValue(gakoa, out var balioa) ? balioa : null;
        }

        private const int NO_ERROR = 0;
        private const int ERROR_ALREADY_ASSIGNED = 85;
        private const int ERROR_SESSION_CREDENTIAL_CONFLICT = 1219;
        private const int RESOURCETYPE_DISK = 1;

        [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
        private static extern int WNetAddConnection2(ref NETRESOURCE lpNetResource, string? lpPassword, string? lpUsername, int dwFlags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct NETRESOURCE
        {
            public int dwScope;
            public int dwType;
            public int dwDisplayType;
            public int dwUsage;
            public string? lpLocalName;
            public string? lpRemoteName;
            public string? lpComment;
            public string? lpProvider;
        }
    }
}
