using System.Globalization;
using System.IO;

namespace KonturTest.Services;

public sealed class Task1ProcessingService
{
    private const int PacketSize   = 1456;
    private const int HeaderSize   = 16;
    private const int BlockCount   = 60;
    private const int ChannelCount = 6;

    public void Process(string inputPath, string outputPath,
        IProgress<double> progress, CancellationToken ct)
    {
        using var fs     = new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new BinaryReader(fs);
        using var writer = new StreamWriter(outputPath, append: false, System.Text.Encoding.UTF8);

        writer.WriteLine("Packet,Channel 1,Channel 2,Channel 3,Channel 4,Channel 5,Channel 6");

        long totalPackets = fs.Length / PacketSize;
        long packetIndex  = 0;

        while (fs.Position + PacketSize <= fs.Length)
        {
            ct.ThrowIfCancellationRequested();

            // header
            uint size     = reader.ReadUInt32();
            uint type     = reader.ReadUInt32();
            uint packetNo = reader.ReadUInt32();
            uint timeMs   = reader.ReadUInt32();

            // 60 blocks × 6 channels
            long[] sums = new long[ChannelCount];
            for (int b = 0; b < BlockCount; b++)
                for (int ch = 0; ch < ChannelCount; ch++)
                    sums[ch] += reader.ReadInt32();

            // write CSV row with averages
            writer.Write(packetNo);
            for (int ch = 0; ch < ChannelCount; ch++)
            {
                double avg = sums[ch] / (double)BlockCount;
                writer.Write(',');
                writer.Write(avg.ToString("G", CultureInfo.InvariantCulture));
            }
            writer.WriteLine();

            packetIndex++;
            if (packetIndex % 100 == 0 || packetIndex == totalPackets)
                progress.Report(packetIndex / (double)Math.Max(totalPackets, 1));
        }

        progress.Report(1.0);
    }
}
