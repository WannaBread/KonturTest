using System.Globalization;
using System.IO;
using KonturTest.Models;

namespace KonturTest.Services;

public sealed class Task1ProcessingService
{
    public void Process(string inputPath, string outputPath,
        IProgress<double> progress, CancellationToken ct)
    {
        using var fs     = new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new BinaryReader(fs);
        using var writer = new StreamWriter(outputPath, append: false, System.Text.Encoding.UTF8);

        writer.WriteLine("Packet,Channel 1,Channel 2,Channel 3,Channel 4,Channel 5,Channel 6");

        long totalPackets = fs.Length / PacketRecord.PacketSize;
        long packetIndex  = 0;

        while (fs.Position + PacketRecord.PacketSize <= fs.Length)
        {
            ct.ThrowIfCancellationRequested();

            var p = PacketRecord.Read(reader);

            writer.Write(p.PacketNo);
            writer.Write(','); writer.Write(p.Channel1.ToString("G", CultureInfo.InvariantCulture));
            writer.Write(','); writer.Write(p.Channel2.ToString("G", CultureInfo.InvariantCulture));
            writer.Write(','); writer.Write(p.Channel3.ToString("G", CultureInfo.InvariantCulture));
            writer.Write(','); writer.Write(p.Channel4.ToString("G", CultureInfo.InvariantCulture));
            writer.Write(','); writer.Write(p.Channel5.ToString("G", CultureInfo.InvariantCulture));
            writer.Write(','); writer.Write(p.Channel6.ToString("G", CultureInfo.InvariantCulture));
            writer.WriteLine();

            packetIndex++;
            if (packetIndex % 100 == 0 || packetIndex == totalPackets)
                progress.Report(packetIndex / (double)Math.Max(totalPackets, 1));
        }

        progress.Report(1.0);
    }
}
