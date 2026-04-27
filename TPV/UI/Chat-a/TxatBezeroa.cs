using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;

namespace TPV
{
    public sealed class TxatBezeroa
    {
        public static TxatBezeroa Instantzia { get; } = new TxatBezeroa();
        private TxatBezeroa() { }

        private TcpClient? tcp;
        private Stream? kanal;
        private StreamReader? lerroIrakurlea;
        private StreamWriter? lerroIdazlea;
        private bool legacyModua;
        private CancellationTokenSource? cts;
        private string erabiltzaileIzena = "Anonimoa";
        //private string hostaUnekoa = "localhost";
        private string hostaUnekoa = "192.168.10.5";
        private int portuaUnekoa = 5555;
        private bool zifratuta = true;
        private readonly SemaphoreSlim bidalketaBlokeoa = new(1, 1);
        private readonly SemaphoreSlim konekzioBlokeoa = new(1, 1);
        private byte[]? saioAesGakoa;
        private readonly Dictionary<string, (string nor, string izena, int guztira)> fitxategiMetak = new();

        private const int GoiburuLuzera = 4;
        private const int GehienezkoFrameLuzera = 2 * 1024 * 1024;
        private const byte FrameClientHello = 1;
        private const byte FrameServerHello = 2;
        private const byte FrameEncryptedApp = 3;

        private const byte AppName = 1;
        private const byte AppText = 2;
        private const byte AppSystem = 3;
        private const byte AppFileMeta = 10;
        private const byte AppFileChunk = 11;
        private const byte AppFileEnd = 12;
        private const byte AppCall = 20;

        private const int SaioIvLuzera = 12;
        private const int SaioTagLuzera = 16;
        private const int FitxategiChunkByteak = 16 * 1024;

        public event Action<string>? RawJasota;
        public event Action<bool, string>? EgoeraAldatu;

        public bool Konektatuta =>
            tcp?.Connected == true &&
            tcp.Client is not null &&
            !SocketItxita(tcp) &&
            kanal is not null &&
            (legacyModua ? (lerroIdazlea is not null && lerroIrakurlea is not null) : saioAesGakoa is not null);
        public string ErabiltzaileIzena => erabiltzaileIzena;

        public async Task KonektatuAsync(string ip, int portua, int erabiltzaileId, int rolId, string erabiltzaileIzena, string? tokena = null)
        {
            if (!SaioGlobala.TxatBaimenduta)
            {
                try { Deskonektatu(); } catch { }
                EgoeraAldatu?.Invoke(false, "Baimenik gabe");
                return;
            }

            await KonektatuAsync(ip, portua, erabiltzaileIzena);
        }

        public async Task KonektatuAsync(string hosta, int portua, string erabiltzaileIzena)
        {
            if (Konektatuta)
            {
                return;
            }

            this.erabiltzaileIzena = string.IsNullOrWhiteSpace(erabiltzaileIzena) ? "Anonimoa" : erabiltzaileIzena.Trim();
            //hostaUnekoa = string.IsNullOrWhiteSpace(hosta) ? "localhost" : hosta.Trim();
            hostaUnekoa = string.IsNullOrWhiteSpace(hosta) ? "192.168.10.5" : hosta.Trim();
            portuaUnekoa = portua;
            zifratuta = true;
            legacyModua = false;
            saioAesGakoa = null;
            lerroIrakurlea = null;
            lerroIdazlea = null;

            var saiakerak = LortuKonexioSaiakerak();
            Exception? azkenErrorea = null;

            for (int i = 0; i < saiakerak.Length; i++)
            {
                var modua = saiakerak[i];
                TcpClient? tokikoTcp = null;
                Stream? tokikoKanal = null;

                try
                {
                    tokikoTcp = new TcpClient();
                    await tokikoTcp.ConnectAsync(hostaUnekoa, portua);
                    tokikoTcp.NoDelay = true;
                    tokikoKanal = tokikoTcp.GetStream();

                    if (modua == KonexioModua.Tls)
                    {
                        var ns = tokikoKanal as NetworkStream;
                        var tlsKanal = ns is null ? null : await SaiatuTlsKanalEdoNullAsync(ns, hostaUnekoa);
                        if (tlsKanal is null)
                        {
                            throw new InvalidOperationException("SSL/TLS handshake huts egin du zerbitzariarekin.");
                        }
                        tokikoKanal = tlsKanal;
                    }
                    else
                    {
                        await Task.Delay(200);
                        if (SocketItxita(tokikoTcp))
                        {
                            throw new InvalidOperationException("Zerbitzariak konexioa itxi du.");
                        }
                    }

                    tcp = tokikoTcp;
                    kanal = tokikoKanal;
                    cts = new CancellationTokenSource();
                    await KonektatuLegacyAsync(cts.Token);

                    EgoeraAldatu?.Invoke(true, "Konektatuta");
                    RawJasota?.Invoke("[SISTEMA] Konexio modua: " + (modua == KonexioModua.Tls ? "TLS" : "TCP"));
                    RawJasota?.Invoke("[SISTEMA] Txatera konektatu zara.");
                    return;
                }
                catch (Exception ex)
                {
                    azkenErrorea = ex;
                    try { tokikoKanal?.Dispose(); } catch { }
                    try { tokikoTcp?.Close(); } catch { }
                    Deskonektatu();
                }
            }

            string mezua = azkenErrorea?.Message ?? "Ezin izan da txatera konektatu.";
            RawJasota?.Invoke("[ERROREA] " + mezua);
            throw new InvalidOperationException(mezua, azkenErrorea);
        }

        private enum KonexioModua
        {
            Auto = 0,
            Tls = 1,
            Tcp = 2
        }

        private static KonexioModua[] LortuKonexioSaiakerak()
        {
            var modua = IrakurriKonexioModua();
            if (modua == KonexioModua.Tls) return new[] { KonexioModua.Tls };
            if (modua == KonexioModua.Tcp) return new[] { KonexioModua.Tcp };
            return new[] { KonexioModua.Tls, KonexioModua.Tcp };
        }

        private static KonexioModua IrakurriKonexioModua()
        {
            string? balioa = Environment.GetEnvironmentVariable("TPV_CHAT_MODE");
            if (!string.IsNullOrWhiteSpace(balioa))
            {
                balioa = balioa.Trim();
                if (balioa.Equals("TLS", StringComparison.OrdinalIgnoreCase)) return KonexioModua.Tls;
                if (balioa.Equals("TCP", StringComparison.OrdinalIgnoreCase)) return KonexioModua.Tcp;
                if (balioa.Equals("AUTO", StringComparison.OrdinalIgnoreCase)) return KonexioModua.Auto;
                return KonexioModua.Auto;
            }

            string? tls = Environment.GetEnvironmentVariable("TPV_CHAT_TLS");
            if (!string.IsNullOrWhiteSpace(tls))
            {
                tls = tls.Trim();
                if (tls == "1" || tls.Equals("true", StringComparison.OrdinalIgnoreCase) || tls.Equals("yes", StringComparison.OrdinalIgnoreCase))
                {
                    return KonexioModua.Tls;
                }
            }

            return KonexioModua.Auto;
        }

        private static bool SocketItxita(TcpClient tcp)
        {
            try
            {
                var s = tcp.Client;
                return s.Poll(0, SelectMode.SelectRead) && s.Available == 0;
            }
            catch
            {
                return true;
            }
        }

        private static async Task<Stream?> SaiatuTlsKanalEdoNullAsync(NetworkStream ns, string hosta)
        {
            try
            {
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                var ssl = new SslStream(
                    ns,
                    leaveInnerStreamOpen: false,
                    userCertificateValidationCallback: (_, __, ___, ____) => true
                );

                var aukerak = new SslClientAuthenticationOptions
                {
                    TargetHost = hosta,
                    EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
                    CertificateRevocationCheckMode = X509RevocationMode.NoCheck,
                    RemoteCertificateValidationCallback = (_, __, ___, ____) => true
                };

                await ssl.AuthenticateAsClientAsync(aukerak, timeoutCts.Token);
                return ssl;
            }
            catch
            {
                return null;
            }
        }

        public async Task BidaliAsync(string testua)
        {
            await bidalketaBlokeoa.WaitAsync();
            try
            {
                if (kanal is null || !Konektatuta)
                {
                    await SaiatuBerrizKonektatuAsync();
                }
                if (kanal is null || !Konektatuta)
                {
                    throw new InvalidOperationException("Txata ez dago konektatuta.");
                }

                string garbia = testua.Trim();
                if (garbia.StartsWith("/file ", StringComparison.OrdinalIgnoreCase) || garbia.StartsWith("/fitx ", StringComparison.OrdinalIgnoreCase))
                {
                    int esp = garbia.IndexOf(' ');
                    string bidea = (esp >= 0 ? garbia[(esp + 1)..] : "").Trim().Trim('"');
                    if (legacyModua)
                    {
                        await BidaliFitxategiaLegacyAsync(bidea, CancellationToken.None);
                    }
                    else
                    {
                        await BidaliFitxategiaAsync(bidea, CancellationToken.None);
                    }
                    return;
                }

                string testuFinala = OrdezkatuEmotikonoak(testua);
                if (legacyModua)
                {
                    string enc = zifratuta ? ZifratzeTresnak.Zifratu(testuFinala) : testuFinala;

                    var writer = lerroIdazlea;
                    if (writer is null)
                    {
                        await SaiatuBerrizKonektatuAsync();
                        writer = lerroIdazlea;
                    }
                    if (writer is null)
                    {
                        throw new InvalidOperationException("Txata ez dago konektatuta.");
                    }

                    try
                    {
                        await writer.WriteLineAsync(enc);
                        await writer.FlushAsync();
                    }
                    catch
                    {
                        Deskonektatu();
                        await SaiatuBerrizKonektatuAsync();
                        writer = lerroIdazlea;
                        if (writer is null)
                        {
                            throw new InvalidOperationException("Txata ez dago konektatuta.");
                        }
                        await writer.WriteLineAsync(enc);
                        await writer.FlushAsync();
                    }
                }
                else
                {
                    byte[] app = SortuAppText(testuFinala);
                    await BidaliEnkriptatutaAsync(app, CancellationToken.None);
                }
                RawJasota?.Invoke(erabiltzaileIzena + ":" + testuFinala);
            }
            finally
            {
                bidalketaBlokeoa.Release();
            }
        }

        private async Task SaiatuBerrizKonektatuAsync()
        {
            if (Konektatuta)
            {
                return;
            }

            await konekzioBlokeoa.WaitAsync();
            try
            {
                if (Konektatuta)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(hostaUnekoa) || portuaUnekoa <= 0)
                {
                    return;
                }

                await KonektatuAsync(hostaUnekoa, portuaUnekoa, erabiltzaileIzena);
            }
            finally
            {
                konekzioBlokeoa.Release();
            }
        }

        private static string OrdezkatuEmotikonoak(string testua)
        {
            return testua
                .Replace(":)", "🙂", StringComparison.Ordinal)
                .Replace(":(", "🙁", StringComparison.Ordinal)
                .Replace(":D", "😄", StringComparison.Ordinal)
                .Replace("<3", "❤️", StringComparison.Ordinal);
        }

        public void Deskonektatu()
        {
            try { cts?.Cancel(); } catch { }
            try { lerroIdazlea?.Dispose(); } catch { }
            try { lerroIrakurlea?.Dispose(); } catch { }
            try { kanal?.Dispose(); } catch { }
            try { tcp?.Close(); } catch { }
            tcp = null;
            kanal = null;
            lerroIrakurlea = null;
            lerroIdazlea = null;
            cts = null;
            saioAesGakoa = null;
            legacyModua = false;
            EgoeraAldatu?.Invoke(false, "Deskonektatuta");
        }

        private async Task KonektatuLegacyAsync(CancellationToken tokena)
        {
            if (kanal is null) throw new InvalidOperationException("Txata ez dago konektatuta.");

            legacyModua = true;
            lerroIrakurlea = new StreamReader(kanal, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
            lerroIdazlea = new StreamWriter(kanal, Encoding.UTF8, bufferSize: 1024, leaveOpen: true) { AutoFlush = true };

            string? welcome = null;
            if (kanal is NetworkStream ns && ns.DataAvailable)
            {
                welcome = await lerroIrakurlea.ReadLineAsync();
            }
            if (!string.IsNullOrWhiteSpace(welcome))
            {
                RawJasota?.Invoke("[SISTEMA] " + welcome);
            }

            await lerroIdazlea.WriteLineAsync(string.IsNullOrWhiteSpace(erabiltzaileIzena) ? "Anonimoa" : erabiltzaileIzena);
            await Task.Delay(200, tokena);
            if (tcp is null || SocketItxita(tcp))
            {
                throw new InvalidOperationException("Zerbitzariak konexioa itxi du.");
            }

            var tcpUnekoa = tcp;
            var irakurleUnekoa = lerroIrakurlea;
            var idazleUnekoa = lerroIdazlea;
            if (irakurleUnekoa is not null)
            {
                _ = Task.Run(() => JasotzeBegiztaLegacy(tokena, tcpUnekoa, irakurleUnekoa, idazleUnekoa));
            }
        }

        private async Task JasotzeBegiztaLegacy(CancellationToken tokena, TcpClient? tcpUnekoa, StreamReader irakurleUnekoa, StreamWriter? idazleUnekoa)
        {
            try
            {
                while (!tokena.IsCancellationRequested)
                {
                    string? line = await irakurleUnekoa.ReadLineAsync();
                    if (line is null)
                    {
                        try { RawJasota?.Invoke("[SISTEMA] Zerbitzariak konexioa itxi du."); } catch { }
                        break;
                    }
                    RawJasota?.Invoke(line);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                if (tokena.IsCancellationRequested || ex is ObjectDisposedException)
                {
                    return;
                }
                if (ex is IOException ioEx)
                {
                    if (ioEx.Message.StartsWith("The stream is", StringComparison.OrdinalIgnoreCase) ||
                        ioEx.Message.IndexOf("disposed", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return;
                    }
                }
                try { RawJasota?.Invoke("[ERROREA] Txat jasotzean: " + ex.Message); } catch { }
            }
            finally
            {
                if (tcpUnekoa is not null && ReferenceEquals(tcp, tcpUnekoa))
                {
                    Deskonektatu();
                }
                else
                {
                    try { idazleUnekoa?.Dispose(); } catch { }
                    try { irakurleUnekoa.Dispose(); } catch { }
                }
            }
        }

        private async Task BidaliFitxategiaLegacyAsync(string bidea, CancellationToken tokena)
        {
            if (lerroIdazlea is null) throw new InvalidOperationException("Txata ez dago konektatuta.");

            if (!File.Exists(bidea))
            {
                throw new FileNotFoundException("Ez da fitxategia aurkitu.", bidea);
            }

            var fi = new FileInfo(bidea);
            long tamaina = fi.Length;
            if (tamaina <= 0)
            {
                throw new InvalidOperationException("Fitxategia hutsik dago.");
            }

            int chunkTamaina = FitxategiChunkByteak;
            int chunkKop = (int)((tamaina + chunkTamaina - 1) / chunkTamaina);
            if (chunkKop <= 0)
            {
                throw new InvalidOperationException("Fitxategi hutsa edo baliogabea");
            }

            string fileId = Guid.NewGuid().ToString();
            string izena = Path.GetFileName(bidea);
            string izenaEnc = Uri.EscapeDataString(izena);
            string mime = LortuMimeMota(izena);

            RawJasota?.Invoke("[SISTEMA] Fitxategia bidaltzen: " + izena);

            byte[] buffer = new byte[chunkTamaina];
            int idx = 0;
            using (var fs = File.OpenRead(bidea))
            {
                int read;
                while ((read = await fs.ReadAsync(buffer, 0, buffer.Length, tokena)) > 0)
                {
                    idx++;
                    byte[] zatia = buffer;
                    if (read != buffer.Length)
                    {
                        zatia = new byte[read];
                        Buffer.BlockCopy(buffer, 0, zatia, 0, read);
                    }

                    string base64 = Convert.ToBase64String(zatia);
                    string payload = "FILECHUNK|" + fileId + "|" + izenaEnc + "|" + mime + "|" + idx + "|" + chunkKop + "|" + base64;
                    string bidali = zifratuta ? ZifratzeTresnak.Zifratu(payload) : payload;
                    await lerroIdazlea.WriteLineAsync(bidali);
                }
            }

            RawJasota?.Invoke("[SISTEMA] Fitxategia bidalita: " + izena);
        }

        private static string LortuMimeMota(string fitxategiIzena)
        {
            string ext = Path.GetExtension(fitxategiIzena ?? "").Trim().ToLowerInvariant();
            return ext switch
            {
                ".txt" => "text/plain",
                ".csv" => "text/csv",
                ".json" => "application/json",
                ".pdf" => "application/pdf",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".zip" => "application/zip",
                ".rar" => "application/vnd.rar",
                ".7z" => "application/x-7z-compressed",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };
        }

        private async Task JasotzeBegizta(CancellationToken tokena)
        {
            try
            {
                while (!tokena.IsCancellationRequested && kanal is not null && saioAesGakoa is not null)
                {
                    Frame frame = await IrakurriFrameAsync(tokena);
                    if (frame.Mota != FrameEncryptedApp)
                    {
                        continue;
                    }

                    byte[] app;
                    try
                    {
                        app = DeszifratuSaioa(saioAesGakoa, frame.Payload);
                    }
                    catch
                    {
                        continue;
                    }

                    ProzesatuApp(app);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    RawJasota?.Invoke("[ERROREA] Txat jasotzean: " + ex.Message);
                }
                catch
                {
                }
            }
            finally
            {
                if (tcp is not null)
                {
                    Deskonektatu();
                }
            }
        }

        private async Task BidaliFrameAsync(byte mota, byte[] payload, CancellationToken tokena)
        {
            if (kanal is null)
            {
                throw new InvalidOperationException("Txata ez dago konektatuta.");
            }

            int len = 1 + payload.Length;
            if (len <= 0 || len > GehienezkoFrameLuzera)
            {
                throw new InvalidOperationException("Mezuaren luzera ez da onargarria.");
            }

            byte[] goiburua = new byte[GoiburuLuzera];
            BinaryPrimitives.WriteInt32BigEndian(goiburua, len);

            await bidalketaBlokeoa.WaitAsync(tokena);
            try
            {
                await kanal.WriteAsync(goiburua, tokena);
                await kanal.WriteAsync(new[] { mota }, tokena);
                await kanal.WriteAsync(payload, tokena);
                await kanal.FlushAsync(tokena);
            }
            finally
            {
                bidalketaBlokeoa.Release();
            }
        }

        private async Task<Frame> IrakurriFrameAsync(CancellationToken tokena)
        {
            if (kanal is null) throw new InvalidOperationException("Txata ez dago konektatuta.");

            byte[] goiburua = new byte[GoiburuLuzera];
            await kanal.ReadExactlyAsync(goiburua, tokena);
            int luzera = BinaryPrimitives.ReadInt32BigEndian(goiburua);
            if (luzera <= 0 || luzera > GehienezkoFrameLuzera)
            {
                throw new InvalidOperationException("Frame luzera baliogabea.");
            }

            int payloadLen = luzera - 1;
            byte[] motaBuf = new byte[1];
            await kanal.ReadExactlyAsync(motaBuf, tokena);
            byte mota = motaBuf[0];
            byte[] payload = new byte[payloadLen];
            if (payloadLen > 0)
            {
                await kanal.ReadExactlyAsync(payload, tokena);
            }
            return new Frame(mota, payload);
        }

        private async Task BidaliFitxategiaAsync(string bidea, CancellationToken tokena)
        {
            if (saioAesGakoa is null)
            {
                throw new InvalidOperationException("Txata ez dago konektatuta.");
            }

            if (!File.Exists(bidea))
            {
                throw new FileNotFoundException("Ez da fitxategia aurkitu.", bidea);
            }

            var fi = new FileInfo(bidea);
            long tamaina = fi.Length;
            if (tamaina <= 0)
            {
                throw new InvalidOperationException("Fitxategia hutsik dago.");
            }

            int chunkTamaina = FitxategiChunkByteak;
            int chunkKop = (int)((tamaina + chunkTamaina - 1) / chunkTamaina);
            if (chunkKop <= 0)
            {
                throw new InvalidOperationException("Fitxategi hutsa edo baliogabea");
            }

            string fileId = Guid.NewGuid().ToString();
            string izena = Path.GetFileName(bidea);

            RawJasota?.Invoke("[SISTEMA] Fitxategia bidaltzen: " + izena);
            await BidaliEnkriptatutaAsync(SortuAppFileMeta(fileId, izena, tamaina, chunkTamaina, chunkKop), tokena);

            using var sha = SHA256.Create();
            byte[] buffer = new byte[chunkTamaina];
            int idx = 0;
            using (var fs = File.OpenRead(bidea))
            {
                int read;
                while ((read = await fs.ReadAsync(buffer, 0, buffer.Length, tokena)) > 0)
                {
                    idx++;
                    sha.TransformBlock(buffer, 0, read, null, 0);
                    byte[] zatia = buffer;
                    if (read != buffer.Length)
                    {
                        zatia = new byte[read];
                        Buffer.BlockCopy(buffer, 0, zatia, 0, read);
                    }
                    await BidaliEnkriptatutaAsync(SortuAppFileChunk(fileId, idx, zatia), tokena);
                }
            }
            sha.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            byte[] hash = sha.Hash ?? Array.Empty<byte>();
            await BidaliEnkriptatutaAsync(SortuAppFileEnd(fileId, hash), tokena);
            RawJasota?.Invoke("[SISTEMA] Fitxategia bidalita: " + izena);
        }

        private async Task BidaliEnkriptatutaAsync(byte[] appMezua, CancellationToken tokena)
        {
            if (saioAesGakoa is null)
            {
                throw new InvalidOperationException("Txata ez dago konektatuta.");
            }
            byte[] zifratua = ZifratuSaioa(saioAesGakoa, appMezua);
            await BidaliFrameAsync(FrameEncryptedApp, zifratua, tokena);
        }

        private void ProzesatuApp(byte[] appMezua)
        {
            if (appMezua.Length == 0) return;

            var cur = new ByteCursor(appMezua);
            byte mota = cur.ReadByte();

            if (mota == AppSystem)
            {
                string mezua = cur.ReadJavaUtf();
                RawJasota?.Invoke("[SISTEMA] " + mezua);
                return;
            }

            if (mota == AppText)
            {
                string nor = cur.ReadJavaUtf();
                string mezua = cur.ReadJavaUtf();
                RawJasota?.Invoke(nor + ":" + mezua);
                return;
            }

            if (mota == AppFileMeta)
            {
                string nor = cur.ReadJavaUtf();
                string fileId = cur.ReadJavaUtf();
                string fitxIzena = cur.ReadJavaUtf();
                cur.ReadInt64BigEndian();
                cur.ReadInt32BigEndian();
                int chunkKop = cur.ReadInt32BigEndian();
                fitxategiMetak[fileId] = (nor, fitxIzena, chunkKop);
                RawJasota?.Invoke("[SISTEMA] " + nor + " fitxategi bat bidaltzen hasi da: " + fitxIzena);
                return;
            }

            if (mota == AppFileChunk)
            {
                string fileId = cur.ReadJavaUtf();
                int idx = cur.ReadInt32BigEndian();
                int luzera = cur.ReadInt32BigEndian();
                byte[] data = cur.ReadBytes(luzera);

                if (fitxategiMetak.TryGetValue(fileId, out var meta))
                {
                    string base64 = Convert.ToBase64String(data);
                    RawJasota?.Invoke(meta.nor + ":FILECHUNK|" + fileId + "|" + meta.izena + "|" + idx + "|" + meta.guztira + "|" + base64);
                }
                return;
            }

            if (mota == AppFileEnd)
            {
                string fileId = cur.ReadJavaUtf();
                if (fitxategiMetak.ContainsKey(fileId))
                {
                    fitxategiMetak.Remove(fileId);
                }
                return;
            }

            if (mota == AppCall)
            {
                byte ekintza = cur.ReadByte();
                string nork = cur.ReadJavaUtf();
                string nori = cur.ReadJavaUtf();
                RawJasota?.Invoke("[SISTEMA] Deia: " + ekintza + " - " + nork + " -> " + nori);
            }
        }

        private static AsymmetricCipherKeyPair SortuEcdhGakoParea()
        {
            var gen = new ECKeyPairGenerator();
            gen.Init(new ECKeyGenerationParameters(SecObjectIdentifiers.SecP256r1, new SecureRandom()));
            return gen.GenerateKeyPair();
        }

        private static byte[] ExportatuEcdhPublikoa(AsymmetricKeyParameter publikoa)
        {
            var info = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publikoa);
            return info.GetEncoded();
        }

        private static byte[] DeribatuSaioAesGakoa(ECPrivateKeyParameters nirePribatua, byte[] nirePublikoaEncoded, byte[] bestearenPublikoaEncoded)
        {
            AsymmetricKeyParameter bestearenPublikoa = PublicKeyFactory.CreateKey(bestearenPublikoaEncoded);
            var agreement = new ECDHBasicAgreement();
            agreement.Init(nirePribatua);
            BigInteger secretBi = agreement.CalculateAgreement(bestearenPublikoa);
            int fieldSize = nirePribatua.Parameters.Curve.FieldSize;
            int secretLen = (fieldSize + 7) / 8;
            byte[] sharedSecret = BigIntegers.AsUnsignedByteArray(secretLen, secretBi);

            byte[] salt = Sha256(KonkatOrdenatuta(nirePublikoaEncoded, bestearenPublikoaEncoded));
            byte[] info = Encoding.UTF8.GetBytes("chat-seguru-saio-gakoa");
            return HkdfSha256(sharedSecret, salt, info, 16);
        }

        private static byte[] ZifratuSaioa(byte[] aesGakoa, byte[] edukia)
        {
            byte[] iv = RandomNumberGenerator.GetBytes(SaioIvLuzera);
            byte[] cipher = new byte[edukia.Length];
            byte[] tag = new byte[SaioTagLuzera];

            using var aes = new AesGcm(aesGakoa, SaioTagLuzera);
            aes.Encrypt(iv, edukia, cipher, tag);

            byte[] output = new byte[iv.Length + cipher.Length + tag.Length];
            Buffer.BlockCopy(iv, 0, output, 0, iv.Length);
            Buffer.BlockCopy(cipher, 0, output, iv.Length, cipher.Length);
            Buffer.BlockCopy(tag, 0, output, iv.Length + cipher.Length, tag.Length);
            return output;
        }

        private static byte[] DeszifratuSaioa(byte[] aesGakoa, byte[] edukia)
        {
            if (edukia.Length < SaioIvLuzera + SaioTagLuzera)
            {
                throw new InvalidOperationException("Eduki baliogabea.");
            }

            byte[] iv = new byte[SaioIvLuzera];
            Buffer.BlockCopy(edukia, 0, iv, 0, iv.Length);

            int cipherLen = edukia.Length - SaioIvLuzera - SaioTagLuzera;
            byte[] cipher = new byte[cipherLen];
            Buffer.BlockCopy(edukia, SaioIvLuzera, cipher, 0, cipherLen);

            byte[] tag = new byte[SaioTagLuzera];
            Buffer.BlockCopy(edukia, SaioIvLuzera + cipherLen, tag, 0, tag.Length);

            byte[] plain = new byte[cipherLen];
            using var aes = new AesGcm(aesGakoa, SaioTagLuzera);
            aes.Decrypt(iv, cipher, tag, plain);
            return plain;
        }

        private static byte[] SortuAppName(string izena)
        {
            using var ms = new MemoryStream();
            ms.WriteByte(AppName);
            ByteCursor.WriteJavaUtf(ms, izena);
            return ms.ToArray();
        }

        private static byte[] SortuAppText(string mezua)
        {
            using var ms = new MemoryStream();
            ms.WriteByte(AppText);
            ByteCursor.WriteJavaUtf(ms, mezua);
            return ms.ToArray();
        }

        private static byte[] SortuAppFileMeta(string fileId, string izena, long tamaina, int chunkTamaina, int chunkKop)
        {
            using var ms = new MemoryStream();
            ms.WriteByte(AppFileMeta);
            ByteCursor.WriteJavaUtf(ms, fileId);
            ByteCursor.WriteJavaUtf(ms, izena);
            ByteCursor.WriteInt64BigEndian(ms, tamaina);
            ByteCursor.WriteInt32BigEndian(ms, chunkTamaina);
            ByteCursor.WriteInt32BigEndian(ms, chunkKop);
            return ms.ToArray();
        }

        private static byte[] SortuAppFileChunk(string fileId, int idx, byte[] data)
        {
            using var ms = new MemoryStream();
            ms.WriteByte(AppFileChunk);
            ByteCursor.WriteJavaUtf(ms, fileId);
            ByteCursor.WriteInt32BigEndian(ms, idx);
            ByteCursor.WriteInt32BigEndian(ms, data.Length);
            ms.Write(data, 0, data.Length);
            return ms.ToArray();
        }

        private static byte[] SortuAppFileEnd(string fileId, byte[] hash)
        {
            using var ms = new MemoryStream();
            ms.WriteByte(AppFileEnd);
            ByteCursor.WriteJavaUtf(ms, fileId);
            ByteCursor.WriteInt32BigEndian(ms, hash.Length);
            ms.Write(hash, 0, hash.Length);
            return ms.ToArray();
        }

        private static byte[] HkdfSha256(byte[] ikm, byte[] salt, byte[] info, int length)
        {
            using var hmac = new HMACSHA256(salt);
            byte[] prk = hmac.ComputeHash(ikm);

            using var hmac2 = new HMACSHA256(prk);
            byte[] okm = new byte[length];
            byte[] t = Array.Empty<byte>();
            int offset = 0;
            byte counter = 1;

            while (offset < length)
            {
                hmac2.Initialize();
                hmac2.TransformBlock(t, 0, t.Length, null, 0);
                hmac2.TransformBlock(info, 0, info.Length, null, 0);
                hmac2.TransformFinalBlock(new[] { counter }, 0, 1);
                t = hmac2.Hash ?? Array.Empty<byte>();

                int toCopy = Math.Min(t.Length, length - offset);
                Buffer.BlockCopy(t, 0, okm, offset, toCopy);
                offset += toCopy;
                counter++;
            }

            return okm;
        }

        private static byte[] Sha256(byte[] data)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(data);
        }

        private static byte[] KonkatOrdenatuta(byte[] a, byte[] b)
        {
            if (KonparatuLexikografikoki(a, b) <= 0)
            {
                return Konkat(a, b);
            }
            return Konkat(b, a);
        }

        private static byte[] Konkat(byte[] a, byte[] b)
        {
            byte[] output = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, output, 0, a.Length);
            Buffer.BlockCopy(b, 0, output, a.Length, b.Length);
            return output;
        }

        private static int KonparatuLexikografikoki(byte[] a, byte[] b)
        {
            int min = Math.Min(a.Length, b.Length);
            for (int i = 0; i < min; i++)
            {
                int ai = a[i] & 0xFF;
                int bi = b[i] & 0xFF;
                if (ai != bi)
                {
                    return ai - bi;
                }
            }
            return a.Length - b.Length;
        }

        private readonly record struct Frame(byte Mota, byte[] Payload);

        private ref struct ByteCursor
        {
            private ReadOnlySpan<byte> data;
            private int offset;

            public ByteCursor(byte[] data)
            {
                this.data = data;
                offset = 0;
            }

            public byte ReadByte()
            {
                if (offset >= data.Length) return 0;
                return data[offset++];
            }

            public byte[] ReadBytes(int length)
            {
                if (length <= 0) return Array.Empty<byte>();
                if (offset + length > data.Length) length = data.Length - offset;
                byte[] output = data.Slice(offset, length).ToArray();
                offset += length;
                return output;
            }

            public int ReadInt32BigEndian()
            {
                var span = data.Slice(offset, 4);
                offset += 4;
                return BinaryPrimitives.ReadInt32BigEndian(span);
            }

            public long ReadInt64BigEndian()
            {
                var span = data.Slice(offset, 8);
                offset += 8;
                return BinaryPrimitives.ReadInt64BigEndian(span);
            }

            public string ReadJavaUtf()
            {
                ushort utflen = ReadUInt16BigEndian();
                if (utflen == 0) return "";
                byte[] bytearr = ReadBytes(utflen);
                return Encoding.UTF8.GetString(bytearr);
            }

            private ushort ReadUInt16BigEndian()
            {
                var span = data.Slice(offset, 2);
                offset += 2;
                return BinaryPrimitives.ReadUInt16BigEndian(span);
            }

            public static void WriteInt32BigEndian(Stream s, int v)
            {
                Span<byte> buf = stackalloc byte[4];
                BinaryPrimitives.WriteInt32BigEndian(buf, v);
                s.Write(buf);
            }

            public static void WriteInt64BigEndian(Stream s, long v)
            {
                Span<byte> buf = stackalloc byte[8];
                BinaryPrimitives.WriteInt64BigEndian(buf, v);
                s.Write(buf);
            }

            public static void WriteJavaUtf(Stream s, string value)
            {
                byte[] encoded = Encoding.UTF8.GetBytes(value ?? "");
                if (encoded.Length > ushort.MaxValue)
                {
                    throw new InvalidOperationException("Testua luzeegia da.");
                }

                Span<byte> len = stackalloc byte[2];
                BinaryPrimitives.WriteUInt16BigEndian(len, (ushort)encoded.Length);
                s.Write(len);
                s.Write(encoded, 0, encoded.Length);
            }

            
        }

    }
}
