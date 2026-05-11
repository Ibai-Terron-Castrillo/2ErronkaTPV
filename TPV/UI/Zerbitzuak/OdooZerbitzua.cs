using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TPV.BISTAK
{
    public class OdooZerbitzua : IDisposable
    {
        private readonly HttpClient _bezeroa;
        private bool _disposed;

        //public const string DESKONTU_API_OINARRIA = "http://localhost:5093";
        public const string DESKONTU_API_OINARRIA = "http://192.168.10.5:5093";

        private static readonly JsonSerializerOptions JsonAukerak = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public OdooZerbitzua(string? apiUrl = null, string? _ = null)
        {
            _bezeroa = new HttpClient
            {
                BaseAddress = new Uri((apiUrl ?? DESKONTU_API_OINARRIA).TrimEnd('/')),
                Timeout = TimeSpan.FromSeconds(30)
            };

            _bezeroa.DefaultRequestHeaders.Accept.Clear();
            _bezeroa.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        }

        public async Task<DeskuntuEmaitza> DeskuntuBalidatuAsync(string kodea)
        {
            if (string.IsNullOrWhiteSpace(kodea))
            {
                return Error("Deskuntu kodea ezin da hutsik egon.");
            }

            return await PostJsonAsync("/api/Deskontuak/validate", new
            {
                code = kodea.Trim().ToUpperInvariant()
            });
        }

        public async Task<DeskuntuEmaitza> DeskuntuAplikatuAsync(string kodea, int? eskaeraId = null)
        {
            if (string.IsNullOrWhiteSpace(kodea))
            {
                return Error("Deskuntu kodea ezin da hutsik egon.");
            }

            return await PostJsonAsync("/api/Deskontuak/apply", new
            {
                code = kodea.Trim().ToUpperInvariant(),
                order_id = eskaeraId
            });
        }

        public Task<SinkronizazioEmaitza> SinkronizazioEgoeraAsync()
        {
            return Task.FromResult(new SinkronizazioEmaitza
            {
                Status = "disabled",
                Message = "Sinkronizazio egoera Odoo-tik ez da erabiltzen deskontuak balidatzeko."
            });
        }

        private async Task<DeskuntuEmaitza> PostJsonAsync(string path, object payload)
        {
            try
            {
                var json = JsonSerializer.Serialize(payload);
                using var edukia = new StringContent(json, Encoding.UTF8, "application/json");
                var erantzuna = await _bezeroa.PostAsync(path, edukia);
                var testua = await erantzuna.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(testua))
                {
                    return Error($"Deskuntuen API errorea ({(int)erantzuna.StatusCode}): erantzun hutsa.");
                }

                if (HtmlErroreaDa(testua))
                {
                    return Error(
                        $"Deskuntuen API errorea ({(int)erantzuna.StatusCode}): HTML erantzuna jaso da, ez JSON. " +
                        "Ziurtatu TPV-a 5093 portuko APIra deitzen ari dela eta APIa berrabiarazita dagoela."
                    );
                }

                DeskuntuEmaitza? emaitza;
                try
                {
                    emaitza = JsonSerializer.Deserialize<DeskuntuEmaitza>(testua, JsonAukerak);
                }
                catch (JsonException)
                {
                    return Error(
                        $"Deskuntuen APIak ez du JSON erantzun. Erantzuna: {LaburtuErantzuna(testua)}"
                    );
                }

                if (emaitza == null)
                {
                    return Error("Deskuntuen APIaren erantzuna ezin izan da irakurri.");
                }

                if (!erantzuna.IsSuccessStatusCode && string.IsNullOrWhiteSpace(emaitza.Message))
                {
                    emaitza.Valid = false;
                    emaitza.Message = $"Deskuntuen API errorea ({(int)erantzuna.StatusCode} {erantzuna.ReasonPhrase})";
                }

                return emaitza;
            }
            catch (TaskCanceledException)
            {
                return Error("Deskuntuen APIarekin konexioa denboraz kanpo gelditu da.");
            }
            catch (Exception ex)
            {
                return Error($"Konexio errorea deskuntuen APIarekin: {ex.Message}");
            }
        }

        private static bool HtmlErroreaDa(string testua)
        {
            var garbia = testua.TrimStart();
            return garbia.StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase)
                   || garbia.StartsWith("<html", StringComparison.OrdinalIgnoreCase);
        }

        private static string LaburtuErantzuna(string testua)
        {
            var lerroBakarra = testua.Replace("\r", " ").Replace("\n", " ").Trim();
            return lerroBakarra.Length <= 250 ? lerroBakarra : lerroBakarra[..250] + "...";
        }

        private static DeskuntuEmaitza Error(string message)
        {
            return new DeskuntuEmaitza
            {
                Valid = false,
                Message = message
            };
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _bezeroa.Dispose();
            _disposed = true;
        }
    }

    public class DeskuntuEmaitza
    {
        [JsonPropertyName("valid")]
        public bool Valid { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("percentage")]
        public double Percentage { get; set; }

        [JsonPropertyName("code_id")]
        public int? CodeId { get; set; }
    }

    public class SinkronizazioEmaitza
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("sync_type")]
        public string SyncType { get; set; } = string.Empty;

        [JsonPropertyName("records_synced")]
        public int RecordsSynced { get; set; }

        [JsonPropertyName("error_message")]
        public string ErrorMessage { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("last_sync")]
        public DateTime? LastSync { get; set; }
    }
}
