using System;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;
using TPV;

class SslTestClient {
    public static async Task RunAsync() {
        var tcp = new TcpClient("localhost", 5555);
        var stream = tcp.GetStream();
        var reader = new StreamReader(stream);
        var writer = new StreamWriter(stream) { AutoFlush = true };
        
        string welcome = await reader.ReadLineAsync();
        Console.WriteLine("Server: " + welcome);
        
        string encUser = ZifratzeTresnak.Zifratu("TestUser");
        await writer.WriteLineAsync(encUser);
        Console.WriteLine("Sent encrypted username");
        
        while (true) {
            string line = await reader.ReadLineAsync();
            if (line == null) {
                Console.WriteLine("Connection closed by server.");
                break;
            }
            Console.WriteLine("Server: " + line);
        }
    }
}
